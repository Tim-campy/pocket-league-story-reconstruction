"""Reject stale edits and preserve Roslyn UTF-16 coordinates in C# source."""
import unittest
from apply_il_rules import patchtext,nonoverlap

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
if __name__=='__main__':unittest.main()
