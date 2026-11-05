"""Reject unsupported or stale matched claims in the method ledger."""
import hashlib
import json
import csv
from pathlib import Path


def digest(path): return hashlib.sha256(path.read_bytes()).hexdigest()


def validate_matched(root,methods):
    metadata_reports=None
    grouped={}
    for method in methods:
        if method['estado']=='matched': grouped.setdefault(method['assembly'],[]).append(method)
    for assembly,rows in grouped.items():
        receipt_path=root/'build'/f'verification-{assembly}.json'
        if not receipt_path.is_file(): raise ValueError('Matched state without verification receipt: '+assembly)
        receipt=json.loads(receipt_path.read_text(encoding='utf-8'))
        verifier={'KairoLibrary':'tools/verify_kairo.py','Assembly-CSharp':'tools/verify_csharp.py'}.get(assembly)
        if verifier is None:raise ValueError('Unsupported verification receipt assembly')
        if receipt['verifier_sha256']!=digest(root/verifier):
            raise ValueError('Verifier changed; regenerate matched receipts')
        if receipt['inspector_source_sha256']!=digest(root/'tools/ProbeInspector.cs'):
            raise ValueError('Inspector changed; regenerate matched receipts')
        for path,expected in receipt.get('helper_source_hashes',{}).items():
            if digest(root/path)!=expected: raise ValueError('Verification helper changed: '+path)
        if digest(root/receipt['build_artifact'])!=receipt['build_sha256']:
            raise ValueError('Build changed since method verification')
        for source in receipt['sources']:
            if digest(root/source['path'])!=source['sha256']:
                raise ValueError('Source changed since verification: '+source['path'])
        for row in rows:
            result=receipt['methods'].get(row['id'])
            if not result or result['state']!='matched' or not (result['body_equal'] or result.get('metadata_declaration_equal',False)) or result['generated_group_pending']:
                raise ValueError('Unsupported matched promotion: '+row['id'])
            if not result['body_equal']:
                if result.get('body_present',True):raise ValueError('Metadata proof cannot replace a method body')
                if metadata_reports is None:
                    from metadata_state import validate_type_metadata
                    with (root/'config/types.csv').open(encoding='utf-8',newline='') as file:types=list(csv.DictReader(file))
                    metadata_reports=validate_type_metadata(root,types)
                proof=metadata_reports.get(assembly,{}).get('declarations',{}).get(result.get('stable_id'),{})
                if not proof.get('proved',False):raise ValueError('Declaration promotion lacks a current metadata proof: '+row['id'])
            if row['percentual']!='100.0000': raise ValueError('Matched percentage must be 100')
