"""Compare every decompressed APK entry against a directory, byte for byte.

This checks file content and names, not ZIP layout, APK signatures, or behavior.
Uses only Python's standard library. Exit status: 0 equal, 1 different, 2 error.
"""

import argparse
import hashlib
import json
from pathlib import Path, PurePosixPath
import sys
import zipfile


def compare_files(apk_path, directory):
    apk_path = Path(apk_path).resolve(strict=True)
    directory = Path(directory).resolve(strict=True)
    if not directory.is_dir():
        raise ValueError("Comparison target must be a directory")
    with apk_path.open("rb") as apk_file:
        apk_digest = hashlib.file_digest(apk_file, "sha256").hexdigest()
    records = []
    expected = set()
    total_bytes = 0
    with zipfile.ZipFile(apk_path) as apk:
        for entry in apk.infolist():
            if entry.is_dir():
                continue
            name = PurePosixPath(entry.filename)
            if name.is_absolute() or ".." in name.parts or "\\" in entry.filename:
                raise ValueError(f"Unsafe archive path: {entry.filename!r}")
            key = name.as_posix()
            if key in expected:
                raise ValueError(f"Duplicate archive path: {key!r}")
            expected.add(key)
            target = directory.joinpath(*name.parts)
            if not target.resolve().is_relative_to(directory):
                raise ValueError(f"Path escapes comparison directory: {key!r}")
            if any(p.is_symlink() for p in [target, *target.parents]):
                raise ValueError(f"Symbolic link in comparison path: {key!r}")
            record = {"path": key, "apk_size": entry.file_size}
            if not target.is_file():
                record["status"] = "missing"
                records.append(record)
                continue
            source_hash = hashlib.sha256()
            target_hash = hashlib.sha256()
            source_size = target_size = compared = differences = 0
            first_difference = None
            with apk.open(entry) as source, target.open("rb") as candidate:
                while True:
                    left = source.read(1024 * 1024)
                    right = candidate.read(1024 * 1024)
                    if not left and not right:
                        break
                    source_hash.update(left)
                    target_hash.update(right)
                    source_size += len(left)
                    target_size += len(right)
                    if left != right:
                        common = min(len(left), len(right))
                        for index in range(common):
                            if left[index] != right[index]:
                                differences += 1
                                if first_difference is None:
                                    first_difference = compared + index
                        if len(left) != len(right):
                            differences += abs(len(left) - len(right))
                            if first_difference is None:
                                first_difference = compared + common
                    compared += max(len(left), len(right))
            total_bytes += source_size
            record.update(
                status="equal" if differences == 0 else "different",
                apk_sha256=source_hash.hexdigest(),
                candidate_sha256=target_hash.hexdigest(),
                candidate_size=target_size,
                differing_byte_positions=differences,
                first_different_offset=first_difference,
            )
            records.append(record)
    extra = sorted(
        p.relative_to(directory).as_posix()
        for p in directory.rglob("*")
        if (p.is_file() or p.is_symlink())
        and p.relative_to(directory).as_posix() not in expected
    )
    equal = sum(r["status"] == "equal" for r in records)
    return {
        "apk": str(apk_path),
        "apk_sha256": apk_digest,
        "directory": str(directory),
        "scope": "All regular APK entries, decompressed bytes and relative file names",
        "excludes": ["ZIP layout and metadata", "APK signing block", "Runtime behavior"],
        "equal": equal == len(records) and not extra,
        "files_total": len(records),
        "files_equal": equal,
        "files_different": sum(r["status"] == "different" for r in records),
        "files_missing": sum(r["status"] == "missing" for r in records),
        "source_bytes_compared": total_bytes,
        "extra_files": extra,
        "files": records,
    }


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("apk", type=Path)
    parser.add_argument("directory", type=Path)
    parser.add_argument("--report", type=Path)
    args = parser.parse_args()
    try:
        result = compare_files(args.apk, args.directory)
        if args.report:
            args.report.write_text(
                json.dumps(result, ensure_ascii=False, indent=2) + "\n", encoding="utf-8"
            )
        print(json.dumps({k: v for k, v in result.items() if k != "files"}, ensure_ascii=False))
        return 0 if result["equal"] else 1
    except (OSError, ValueError, zipfile.BadZipFile, RuntimeError) as error:
        print(str(error), file=sys.stderr)
        return 2


if __name__ == "__main__":
    sys.exit(main())
