"""Reject stale edits and preserve Roslyn UTF-16 coordinates in C# source."""
import unittest
import tempfile
from pathlib import Path
from apply_il_rules import patchtext,nonoverlap,receipt_is_fresh
from compare_kairo import sha

class PatchControls(unittest.TestCase):
    def test_utf16_crlf_and_non_bmp_source(self):
        source='😀\r\nif (x) {}'
        patch={'start':4,'end':13,'before':'if (x) {}','after':'if (!x) return;'}
        self.assertEqual(patchtext(source,[patch]),'😀\r\nif (!x) return;')
    def test_stale_span_is_rejected(self):
        with self.assertRaises(ValueError):
            patchtext('if (x) {}',[{'start':0,'end':9,'before':'different','after':'return;'}])
    def test_nested_hypotheses_are_not_combined(self):
        outer={'start':1,'end':10};inner={'start':3,'end':5}
        self.assertEqual(nonoverlap([inner,outer]),[outer])
    def test_compile_failure_cannot_reuse_receipt_for_old_source_or_binary(self):
        with tempfile.TemporaryDirectory() as folder:
            root=Path(folder);source=root/'source.cs';binary=root/'target.dll'
            source.write_text('original source');binary.write_text('compiled fixture')
            receipt={'build_artifact':'target.dll','build_sha256':sha(binary),
                     'sources':[{'path':'source.cs','sha256':sha(source)}]}
            self.assertTrue(receipt_is_fresh(root,receipt))
            source.write_text('new source that failed compilation')
            self.assertFalse(receipt_is_fresh(root,receipt))
            source.write_text('original source');binary.write_text('changed output')
            self.assertFalse(receipt_is_fresh(root,receipt))
if __name__=='__main__':unittest.main()
