"""Reject unsupported or stale matched claims in the method ledger."""
import hashlib
import json
from pathlib import Path


def digest(path): return hashlib.sha256(path.read_bytes()).hexdigest()


def validate_matched(root,methods):
    grouped={}
    for method in methods:
        if method['estado']=='matched': grouped.setdefault(method['assembly'],[]).append(method)
    for assembly,rows in grouped.items():
        receipt_path=root/'build'/f'verification-{assembly}.json'
        if not receipt_path.is_file(): raise ValueError('Matched state without verification receipt: '+assembly)
        receipt=json.loads(receipt_path.read_text(encoding='utf-8'))
        if receipt['verifier_sha256']!=digest(root/'tools/verify_kairo.py'):
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
            if not result or result['state']!='matched' or not result['body_equal'] or result['generated_group_pending']:
                raise ValueError('Unsupported matched promotion: '+row['id'])
            if row['percentual']!='100.0000': raise ValueError('Matched percentage must be 100')
