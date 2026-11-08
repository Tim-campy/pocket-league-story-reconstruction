"""Lossless report transport; originals stay local and the comparator is untouched."""
import argparse
import gzip
import hashlib
import json
from pathlib import Path
import platform
import re
import zlib

ROOT = Path(__file__).resolve().parents[1]
NAME = re.compile(r'round12-[A-Za-z0-9_.-]+-app-threshold-Assembly-CSharp-(?:raw\.json|all-different-bytes\.csv|remaining-bytes\.csv)')
BLOCK = 1024 * 1024


def sha(path):
    result = hashlib.sha256()
    with Path(path).open('rb') as stream:
        for data in iter(lambda: stream.read(BLOCK), b''):
            result.update(data)
    return result.hexdigest()


def checked_path(value):
    value = str(value).replace('\\', '/')
    relative = Path(value)
    path = (ROOT / relative).resolve()
    if relative.is_absolute() or not path.is_relative_to((ROOT / 'reports').resolve()):
        raise ValueError('Only generated report paths are admitted')
    if value != path.relative_to(ROOT).as_posix():
        raise ValueError('Noncanonical report path')
    return path


def checked_item(item):
    source = checked_path(item['original_path'])
    archive = checked_path(item['archive_path'])
    if not NAME.fullmatch(source.name) or archive != Path(str(source) + '.gz'):
        raise ValueError('Transport cannot write targets, configuration, or arbitrary files')
    if not re.fullmatch('[0-9a-f]{64}', item['original_sha256']) or not isinstance(item['original_bytes'], int):
        raise ValueError('Missing exact artifact identity')
    return source, archive


def verify_archive(item, output=None):
    _, archive = checked_item(item)
    if sha(archive) != item['archive_sha256']:
        raise ValueError('Compressed artifact hash differs')
    digest = hashlib.sha256()
    count = 0
    with gzip.open(archive, 'rb') as stream:
        for data in iter(lambda: stream.read(BLOCK), b''):
            count += len(data)
            if count > item['original_bytes']:
                raise ValueError('Decompressed size exceeds manifest')
            digest.update(data)
            if output is not None:
                output.write(data)
    if count != item['original_bytes'] or digest.hexdigest() != item['original_sha256']:
        raise ValueError('Lossless decompression proof failed')


def archive_one(path, expected):
    relative = path.relative_to(ROOT).as_posix()
    item = dict(original_path=relative, original_sha256=expected,
                original_bytes=path.stat().st_size, archive_path=relative + '.gz')
    _, archive = checked_item(item)
    if sha(path) != expected:
        raise ValueError('Original report differs from comparator manifest')
    if not archive.exists():
        temporary = Path(str(archive) + '.tmp')
        created = False
        try:
            with temporary.open('xb') as raw:
                created = True
                with gzip.GzipFile(filename='', mode='wb', compresslevel=9, mtime=0, fileobj=raw) as compressed:
                    with path.open('rb') as source:
                        for data in iter(lambda: source.read(BLOCK), b''):
                            compressed.write(data)
            temporary.replace(archive)
        finally:
            if created and temporary.exists():
                temporary.unlink()
    item.update(archive_sha256=sha(archive), archive_bytes=archive.stat().st_size)
    verify_archive(item)
    if sha(path) != expected:
        raise ValueError('Original changed during archiving')
    return item


def archive_report(report_path):
    report_path = checked_path(str(report_path).replace('\\', '/'))
    if not NAME.fullmatch(report_path.name) or not report_path.name.endswith('-raw.json'):
        raise ValueError('Expected an Assembly-CSharp threshold report')
    manifest_path = report_path.with_name(report_path.name[:-len('-raw.json')] + '-archive-manifest.json')
    report_hash = sha(report_path)
    report = json.loads(report_path.read_text(encoding='utf-8-sig'))
    producer = ROOT / 'tools/round12_raw_compare.py'
    if sha(producer) != report['producer_sha256']:
        raise ValueError('Comparator identity differs; do not hide changed evidence')
    if manifest_path.exists():
        manifest = json.loads(manifest_path.read_text(encoding='utf-8'))
        if manifest['raw_report_sha256'] != report_hash:
            raise ValueError('Archive label already belongs to another report')
        for item in manifest['files']:
            verify_archive(item)
        return dict(manifest=manifest_path.relative_to(ROOT).as_posix(), sha256=sha(manifest_path), lossless=True)
    sources = [(report_path, report_hash)]
    for key in ('all_bytes', 'residual_bytes'):
        proof = report['outputs'][key]
        sources.append((checked_path(proof['path']), proof['sha256']))
    files = [archive_one(path, expected) for path, expected in sources]
    manifest = dict(operation='lossless-raw-report-transport', raw_report=report_path.relative_to(ROOT).as_posix(),
                    raw_report_sha256=report_hash, comparator_sha256=report['producer_sha256'],
                    gzip=dict(level=9, mtime=0, filename='', python=platform.python_version(),
                              zlib=zlib.ZLIB_VERSION, zlib_runtime=zlib.ZLIB_RUNTIME_VERSION),
                    files=files, originals_modified=False, DLL_writes=0, target_compilations=0,
                    method_or_type_states_modified=False, normalization_applied=False,
                    transport_source_sha256=sha(Path(__file__)))
    with manifest_path.open('x', encoding='utf-8', newline='\n') as stream:
        stream.write(json.dumps(manifest, indent=2) + '\n')
    return dict(manifest=manifest_path.relative_to(ROOT).as_posix(), sha256=sha(manifest_path), lossless=True)


def restore(manifest_path):
    manifest_path = checked_path(manifest_path)
    manifest = json.loads(manifest_path.read_text(encoding='utf-8'))
    for item in manifest['files']:
        source, _ = checked_item(item)
        if source.exists():
            if sha(source) != item['original_sha256']:
                raise ValueError('Refusing to overwrite a different existing report')
            verify_archive(item)
            continue
        temporary = Path(str(source) + '.restore.tmp')
        created = False
        try:
            with temporary.open('xb') as stream:
                created = True
                verify_archive(item, stream)
            temporary.replace(source)
        finally:
            if created and temporary.exists():
                temporary.unlink()
    print(json.dumps(dict(restored_or_verified=len(manifest['files']), byte_identity_verified=True,
                          DLL_writes=0, target_compilations=0)))


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('operation', choices=['archive', 'restore'])
    parser.add_argument('path', help='Repository-relative raw report or archive manifest')
    args = parser.parse_args()
    if args.operation == 'archive':
        print(json.dumps(archive_report(args.path)))
    else:
        restore(args.path)


if __name__ == '__main__':
    main()
