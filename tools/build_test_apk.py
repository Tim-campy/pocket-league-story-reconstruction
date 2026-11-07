"""Build a TEST-ONLY APK from verified reconstructed DLLs and the user's APK.

No target assembly is patched. Original ZIP payloads are preserved except the
two rebuilt DLLs and the three JAR-signature entries. JDK tools create a NEW key.
The output is not the byte-identical reconstruction.
"""
import argparse
from collections import Counter
import hashlib
import json
import os
from pathlib import Path
import secrets
import shutil
import struct
import subprocess
import zipfile
import zlib

ROOT = Path(__file__).resolve().parents[1]
SIG_NAMES = {"META-INF/MANIFEST.MF", "META-INF/CERT.SF", "META-INF/CERT.RSA"}
DLL_NAMES = {
    "assets/bin/Data/Managed/KairoLibrary.dll": "KairoLibrary",
    "assets/bin/Data/Managed/Assembly-CSharp.dll": "Assembly-CSharp",
}


def sha(data):
    return hashlib.sha256(data).hexdigest()


def parse_archive(path):
    data = Path(path).read_bytes()
    with zipfile.ZipFile(path) as z:
        infos = z.infolist()
        assert len({i.filename for i in infos}) == len(infos)
        assert [i.header_offset for i in infos] == sorted(i.header_offset for i in infos)
        central = z.start_dir
        comment = z.comment
    cursor = central
    rows = []
    for index, info in enumerate(infos):
        assert data[cursor:cursor + 4] == b"PK\x01\x02"
        name_len, extra_len, comment_len = struct.unpack_from("<HHH", data, cursor + 28)
        size = 46 + name_len + extra_len + comment_len
        cd = data[cursor:cursor + size]
        cursor += size
        header = data[info.header_offset:info.header_offset + 30]
        assert header[:4] == b"PK\x03\x04"
        n, e = struct.unpack_from("<HH", header, 26)
        filename = data[info.header_offset + 30:info.header_offset + 30 + n]
        extra = data[info.header_offset + 30 + n:info.header_offset + 30 + n + e]
        start = info.header_offset + 30 + n + e
        stop = start + info.compress_size
        next_offset = infos[index + 1].header_offset if index + 1 < len(infos) else central
        tail = data[stop:next_offset]
        assert not (info.flag_bits & 1), "Encrypted ZIP unsupported"
        if info.flag_bits & 8:
            assert len(tail) in (12, 16), (info.filename, "unexpected descriptor/gap", len(tail))
        else:
            assert not tail, (info.filename, "unaccounted gap")
        rows.append(dict(info=info, header=header, filename=filename, extra=extra,
                         payload=data[start:stop], tail=tail, data_offset=start, cd=cd))
    assert cursor <= len(data)
    return rows, comment


def compressed(data, method):
    if method == zipfile.ZIP_STORED:
        return data
    assert method == zipfile.ZIP_DEFLATED
    compressor = zlib.compressobj(9, zlib.DEFLATED, -15)
    return compressor.compress(data) + compressor.flush()


def write_archive(template, output, replacements, excluded=frozenset()):
    rows, comment = parse_archive(template)
    out = bytearray()
    centrals = []
    for row in rows:
        info = row["info"]
        if info.filename in excluded:
            continue
        local_offset = len(out)
        header = bytearray(row["header"])
        extra = row["extra"]
        # Preserve every entry's original data-offset congruence modulo4096,
        # including the original APK's nonzero modulo4 offsets.
        padding = (row["data_offset"] - (local_offset + 30 + len(row["filename"]) + len(extra))) % 4096
        if 0 < padding < 4:
            padding += 4096
        if padding:
            extra += struct.pack("<HH", 0xF00D, padding - 4) + bytes(padding - 4)
        assert len(extra) <= 65535
        struct.pack_into("<H", header, 28, len(extra))
        cd = bytearray(row["cd"])
        if info.filename in replacements:
            plain = replacements[info.filename]
            crc = zlib.crc32(plain) & 0xFFFFFFFF
            payload = compressed(plain, info.compress_type)
            if info.flag_bits & 8:
                struct.pack_into("<III", header, 14, 0, 0, 0)
                prefix = b"PK\x07\x08" if row["tail"].startswith(b"PK\x07\x08") else b""
                tail = prefix + struct.pack("<III", crc, len(payload), len(plain))
            else:
                struct.pack_into("<III", header, 14, crc, len(payload), len(plain))
                tail = b""
            struct.pack_into("<III", cd, 16, crc, len(payload), len(plain))
        else:
            payload, tail = row["payload"], row["tail"]
        struct.pack_into("<I", cd, 42, local_offset)
        out += header + row["filename"] + extra
        assert len(out) % 4096 == row["data_offset"] % 4096
        out += payload + tail
        centrals.append(bytes(cd))
    central_offset = len(out)
    for cd in centrals:
        out += cd
    central_size = len(out) - central_offset
    count = len(centrals)
    assert max(len(out), central_offset, central_size) < 0xFFFFFFFF and count < 65535
    out += struct.pack("<IHHHHIIH", 0x06054B50, 0, 0, count, count, central_size, central_offset, len(comment))
    out += comment
    output = Path(output)
    assert not output.exists(), output
    output.write_bytes(out)


def run_logged(args, log, env=None):
    result = subprocess.run([str(x) for x in args], cwd=ROOT, env=env, capture_output=True)
    Path(log).write_bytes(result.stdout + result.stderr)
    if result.returncode:
        raise RuntimeError(f"Command failed ({result.returncode}); see {log}")
    return (result.stdout + result.stderr).decode("utf-8", errors="replace")


def verify_repack(original, final, replacements):
    old_rows, _ = parse_archive(original)
    new_rows, _ = parse_archive(final)
    assert [r["info"].filename for r in old_rows] == [r["info"].filename for r in new_rows]
    results = []
    with zipfile.ZipFile(original) as old_zip, zipfile.ZipFile(final) as new_zip:
        assert new_zip.testzip() is None
        for a, b in zip(old_rows, new_rows):
            name = a["info"].filename
            old_plain, new_plain = old_zip.read(name), new_zip.read(name)
            assert a["info"].compress_type == b["info"].compress_type, name
            assert a["data_offset"] % 4096 == b["data_offset"] % 4096, name
            if name in replacements:
                assert new_plain == replacements[name], name
            else:
                assert old_plain == new_plain and a["payload"] == b["payload"], name
            results.append({
                "path": name, "changed_for_test": name in replacements,
                "original_sha256": sha(old_plain), "test_sha256": sha(new_plain),
                "compression_method": b["info"].compress_type,
                "original_data_mod4096": a["data_offset"] % 4096,
                "test_data_mod4096": b["data_offset"] % 4096,
                "unchanged_compressed_payload_equal": a["payload"] == b["payload"] if name not in replacements else None,
            })
    return results


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--output-dir", type=Path, default=ROOT / "build/round10/runtime")
    args = parser.parse_args()
    folder = args.output_dir.resolve()
    assert folder.is_relative_to((ROOT / "build").resolve()), "Test artifacts must remain in ignored build/"
    protected = ("app-rebuilt-unsigned.apk", "app-rebuilt-jarsigned.apk", "app-rebuilt-test.apk",
                 "test-password.txt", "test-signing.p12", "test-public.der", "apk-build-proof.json")
    assert not any((folder / name).exists() for name in protected), "Use a new output directory"
    keytool = shutil.which("keytool")
    jarsigner = shutil.which("jarsigner")
    assert keytool and jarsigner, "JDK signing tools unavailable"
    folder.mkdir(parents=True, exist_ok=True)
    original = ROOT / "original/app.apk"
    layout = json.loads((ROOT / "original/zip-layout.json").read_text(encoding="utf-8-sig"))
    assert sha(original.read_bytes()) == layout["apk_sha256"]
    assert layout["apk_signing_block_magic_present"] is False
    artifacts = json.loads((ROOT / "build/round10/baseline/assemblies/artifacts.json").read_text(encoding="utf-8-sig"))
    replacements = {}
    for entry, assembly in DLL_NAMES.items():
        spec = artifacts[assembly]
        data = (ROOT / spec["path"]).read_bytes()
        assert sha(data) == spec["sha256"]
        receipt = json.loads((ROOT / spec["receipt"]).read_text(encoding="utf-8-sig"))
        assert receipt["build_sha256"] == spec["sha256"], "Frozen receipt does not authenticate the DLL"
        assert not any(receipt.get(k) for k in ("regressions", "body_regressions", "type_regressions"))
        replacements[entry] = data
    with zipfile.ZipFile(original) as z:
        assert {i.filename for i in z.infolist() if i.filename.startswith("META-INF/")} == SIG_NAMES
    unsigned = folder / "app-rebuilt-unsigned.apk"
    intermediate = folder / "app-rebuilt-jarsigned.apk"
    final = folder / "app-rebuilt-test.apk"
    write_archive(original, unsigned, replacements, SIG_NAMES)
    shutil.copyfile(unsigned, intermediate)
    password_file = folder / "test-password.txt"
    keystore = folder / "test-signing.p12"
    assert not keystore.exists() and not password_file.exists(), "Use a new output directory"
    password = secrets.token_urlsafe(24)
    password_file.write_text(password, encoding="ascii")
    env = dict(os.environ, PLS_TEST_KEY_PASSWORD=password)
    run_logged([keytool, "-genkeypair", "-alias", "pls-test", "-keyalg", "RSA", "-keysize", "3072",
                "-sigalg", "SHA256withRSA", "-validity", "3650", "-storetype", "PKCS12",
                "-keystore", keystore, "-storepass:env", "PLS_TEST_KEY_PASSWORD",
                "-dname", "CN=Pocket League Story Reconstruction Test,OU=Testing,O=Local Project,C=BR",
                "-noprompt"], folder / "key-generation.log", env)
    cert = folder / "test-public.der"
    run_logged([keytool, "-exportcert", "-alias", "pls-test", "-keystore", keystore,
                "-storepass:env", "PLS_TEST_KEY_PASSWORD", "-file", cert], folder / "certificate-export.log", env)
    run_logged([jarsigner, "-keystore", keystore, "-storepass:env", "PLS_TEST_KEY_PASSWORD",
                "-sigfile", "CERT", "-sigalg", "SHA256withRSA", "-digestalg", "SHA-256",
                intermediate, "pls-test"], folder / "signing.log", env)
    with zipfile.ZipFile(intermediate) as z:
        replacements.update({name: z.read(name) for name in SIG_NAMES})
    # V1 signatures authenticate uncompressed entries. Rebuild ZIP metadata/order
    # AFTER signing, preserving the original payloads and alignment congruences.
    write_archive(original, final, replacements)
    verification = run_logged([jarsigner, "-verify", "-verbose", "-certs", final], folder / "signature-verification.log")
    assert "jar verified." in verification.lower(), "JAR signature not verified"
    entries = verify_repack(original, final, replacements)
    proof = {
        "operation": "TEST_ONLY_APK_NOT_RECONSTRUCTION", "signing_scheme": "v1 JAR/SHA-256/RSA3072",
        "original_apk_sha256": sha(original.read_bytes()), "test_apk_sha256": sha(final.read_bytes()),
        "test_certificate_der_sha256": sha(cert.read_bytes()),
        "test_key_generated_locally": True, "original_private_key_used": False,
        "dll_artifacts": artifacts, "entry_count": len(entries), "entry_order_equal": True,
        "compression_methods_equal": True, "all_original_data_mod4096_preserved": True,
        "unchanged_compressed_payloads_equal": True, "changed_entries": sorted(replacements),
        "compression_level_original": "UNKNOWN; unchanged compressed payloads copied exactly; changed payloads use raw DEFLATE level9",
        "assembly_bytes_patched": False, "counts_toward_byte_identical_reconstruction": False,
        "signature_verified": True, "entries": entries,
    }
    (folder / "apk-build-proof.json").write_text(json.dumps(proof, indent=2) + "\n", encoding="utf-8")
    print(json.dumps({k: proof[k] for k in ["operation", "test_apk_sha256", "entry_count", "changed_entries",
          "entry_order_equal", "compression_methods_equal", "all_original_data_mod4096_preserved", "signature_verified"]}))


if __name__ == "__main__":
    main()
