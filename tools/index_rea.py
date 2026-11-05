"""Index retained REA managed evidence and verify raw CIL hashes from DLL bytes."""

import argparse
import csv
import hashlib
import json
from pathlib import Path
import struct


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("bundle", type=Path)
    parser.add_argument("extracted", type=Path)
    parser.add_argument("output", type=Path)
    args = parser.parse_args()
    bundle = json.loads(args.bundle.read_text(encoding="utf-8"))
    args.output.mkdir(parents=True, exist_ok=True)
    summaries, rows = [], []
    for record in bundle["records"]:
        if record["operation"] != "inspect_managed_members":
            continue
        result = record["normalized_result"]
        name = result["module"]["name"]
        dll = args.extracted / "assets/bin/Data/Managed" / name
        data = dll.read_bytes()
        dll_hash = hashlib.sha256(data).hexdigest()
        if dll_hash != result["artifact"]["sha256"]:
            raise ValueError(f"Assembly differs from recorded evidence: {name}")
        bodies_verified = 0
        states = {}
        for method in result["methods"]:
            body = method["body"]
            states[body["status"]] = states.get(body["status"], 0) + 1
            row = {
                "assembly": name,
                "assembly_sha256": dll_hash,
                "mvid": result["module"]["mvid"],
                "method_token": method["token"],
                "declaring_type_token": method["declaring_type_token"],
                "declaring_type": method["declaring_type"],
                "method": method["name"],
                "body_status": body["status"],
                "body_file_offset": body["file_offset"],
                "code_file_offset": None,
                "il_size": body["il_size"],
                "il_sha256": body["il_sha256"],
                "truncated_instructions": body["truncated_instructions"],
                "raw_cil_verified": False,
            }
            if body["status"] == "present":
                offset = body["file_offset"]
                first = data[offset]
                if first & 3 == 2:
                    header_size, il_size = 1, first >> 2
                elif first & 3 == 3:
                    header_size = (struct.unpack_from("<H", data, offset)[0] >> 12) * 4
                    il_size = struct.unpack_from("<I", data, offset + 4)[0]
                else:
                    raise ValueError(f"Unknown method header: {name} {method['token']}")
                code_start = offset + header_size
                code = data[code_start:code_start + il_size]
                if il_size != body["il_size"] or len(code) != il_size:
                    raise ValueError(f"Method size mismatch: {name} {method['token']}")
                if hashlib.sha256(code).hexdigest() != body["il_sha256"]:
                    raise ValueError(f"CIL hash mismatch: {name} {method['token']}")
                row["code_file_offset"] = code_start
                row["raw_cil_verified"] = True
                bodies_verified += 1
            rows.append(row)
        strings = result.get("user_strings") or {}
        summary = {
            "assembly": name,
            "sha256": dll_hash,
            "mvid": result["module"]["mvid"],
            "evidence_id": record["evidence_id"],
            "coverage": result["coverage"],
            "types": len(result["types"]),
            "fields": len(result["fields"]),
            "methods": len(result["methods"]),
            "body_states": states,
            "raw_cil_bodies_verified": bodies_verified,
            "methods_with_truncated_instructions": sum(
                m["body"]["truncated_instructions"] > 0 for m in result["methods"]
            ),
            "user_strings_state": strings.get("state"),
            "user_strings_issues": len(strings.get("issues", [])),
            "limitations": result["limitations"],
        }
        summaries.append(summary)
        (args.output / (name + ".types.json")).write_text(
            json.dumps({
                "assembly_sha256": dll_hash,
                "mvid": result["module"]["mvid"],
                "evidence_id": record["evidence_id"],
                "types": result["types"],
            }, ensure_ascii=False, indent=2) + "\n", encoding="utf-8"
        )
    if not rows:
        raise ValueError("No managed member evidence found in the bundle")
    with (args.output / "managed-methods.csv").open("w", newline="", encoding="utf-8") as file:
        writer = csv.DictWriter(file, fieldnames=list(rows[0]))
        writer.writeheader()
        writer.writerows(rows)
    (args.output / "managed-summary.json").write_text(
        json.dumps(summaries, ensure_ascii=False, indent=2) + "\n", encoding="utf-8"
    )
    for summary in summaries:
        print(json.dumps({k: v for k, v in summary.items() if k != "limitations"}))


if __name__ == "__main__":
    main()
