"""Integrity checks on own temporary report data; no game compilation."""
import gzip
import hashlib
import json
from pathlib import Path
import tempfile
import unittest
from unittest.mock import patch

import round12_raw_transport as transport


class TransportTests(unittest.TestCase):
    def setUp(self):
        self.folder = tempfile.TemporaryDirectory(dir=transport.ROOT / 'build/round12', prefix='transport-test-')
        self.root = Path(self.folder.name).resolve()
        assert self.root.is_relative_to((transport.ROOT / 'build/round12').resolve())
        self.addCleanup(self.folder.cleanup)
        self.patch = patch.object(transport, 'ROOT', self.root)
        self.patch.start()
        self.addCleanup(self.patch.stop)
        (self.root / 'reports').mkdir()
        (self.root / 'tools').mkdir()
        self.producer = self.root / 'tools/round12_raw_compare.py'
        self.producer.write_bytes(b'own-test-producer\n')
        self.prefix = 'reports/round12-fixture-app-threshold-Assembly-CSharp-'
        self.raw = self.root / (self.prefix + 'raw.json')
        self.left = self.root / (self.prefix + 'all-different-bytes.csv')
        self.right = self.root / (self.prefix + 'remaining-bytes.csv')
        self.data = ('first,second\r\nalpha,\u65e5\u672c\r\n' * 100).encode('utf-8')
        self.left.write_bytes(self.data)
        self.right.write_bytes(self.data + b'extra\r\n')
        self.raw.write_text(json.dumps(dict(producer_sha256=transport.sha(self.producer), outputs={
            'all_bytes': dict(path=self.left.relative_to(self.root).as_posix(), sha256=transport.sha(self.left)),
            'residual_bytes': dict(path=self.right.relative_to(self.root).as_posix(), sha256=transport.sha(self.right))})), encoding='utf-8')

    def archive(self):
        proof = transport.archive_report(self.raw.relative_to(self.root).as_posix())
        manifest = self.root / proof['manifest']
        return manifest, json.loads(manifest.read_text())

    def test_roundtrip_exact_crlf_and_unicode(self):
        original = {p: p.read_bytes() for p in (self.raw, self.left, self.right)}
        manifest, record = self.archive()
        for p in original:
            self.assertEqual(p.read_bytes(), original[p])
            p.unlink()
        transport.restore(manifest.relative_to(self.root).as_posix())
        for p, data in original.items():
            self.assertEqual(p.read_bytes(), data)
        self.assertEqual(len(record['files']), 3)

    def test_deterministic_gzip_header_and_payload(self):
        _, record = self.archive()
        archive = self.root / record['files'][1]['archive_path']
        self.assertEqual(archive.read_bytes()[4:8], b'\0' * 4)
        self.assertEqual(gzip.decompress(archive.read_bytes()), self.data)
        first = archive.read_bytes()
        archive.unlink()
        transport.archive_one(self.left, transport.sha(self.left))
        self.assertEqual(archive.read_bytes(), first)

    def test_changed_archive_refused(self):
        manifest, record = self.archive()
        archive = self.root / record['files'][0]['archive_path']
        archive.write_bytes(archive.read_bytes()[:-1] + b'X')
        with self.assertRaises(ValueError):
            transport.restore(manifest.relative_to(self.root).as_posix())

    def test_existing_different_report_never_overwritten(self):
        manifest, _ = self.archive()
        self.raw.write_bytes(b'own changed report')
        with self.assertRaises(ValueError):
            transport.restore(manifest.relative_to(self.root).as_posix())
        self.assertEqual(self.raw.read_bytes(), b'own changed report')

    def test_target_or_config_path_refused(self):
        with self.assertRaises(ValueError):
            transport.checked_path('src/game.dll')
        with self.assertRaises(ValueError):
            transport.checked_item(dict(original_path='reports/game.dll', archive_path='reports/game.dll.gz',
                                        original_sha256='0' * 64, original_bytes=4))

    def test_noncanonical_traversal_refused(self):
        with self.assertRaises(ValueError):
            transport.checked_path('reports/../reports/raw.json')

    def test_new_report_cannot_reuse_archive_label(self):
        self.archive()
        report = json.loads(self.raw.read_text())
        report['own_test'] = True
        self.raw.write_text(json.dumps(report))
        with self.assertRaises(ValueError):
            transport.archive_report(self.raw.relative_to(self.root).as_posix())

    def test_comparator_identity_required(self):
        self.producer.write_bytes(b'changed')
        with self.assertRaises(ValueError):
            transport.archive_report(self.raw.relative_to(self.root).as_posix())

    def test_decompression_size_bound(self):
        _, record = self.archive()
        item = dict(record['files'][0], original_bytes=1)
        with self.assertRaises(ValueError):
            transport.verify_archive(item)


if __name__ == '__main__':
    unittest.main()
