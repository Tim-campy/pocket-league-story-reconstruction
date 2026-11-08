"""Counterexamples for exception accounting; only synthetic byte arrays."""
import unittest
import struct
from round12_raw_compare import differences, field_map, regions


class RawAccounting(unittest.TestCase):
    def fields(self, at=8):
        return {'Module.Mvid': {'offset': at, 'size': 16}}

    def test_only_same_coordinate_field_changes_are_exempt(self):
        a = bytes(40); b = bytes(8) + bytes([1]) * 16 + bytes(16)
        rows = list(differences(a, b, self.fields(), self.fields()))
        self.assertEqual(16, len(rows))
        self.assertTrue(all(r['allowed'] for r in rows))

    def test_neighboring_byte_is_never_exempt(self):
        a = bytes(40); b = bytearray(a); b[24] = 7
        rows = list(differences(a, bytes(b), self.fields(), self.fields()))
        self.assertEqual([24], [r['offset'] for r in rows])
        self.assertFalse(rows[0]['allowed'])

    def test_moving_field_does_not_exempt_union_of_endpoints(self):
        a = bytes(40); b = bytes([1]) * 40
        rows = list(differences(a, b, self.fields(8), self.fields(9)))
        self.assertEqual(40, len(rows))
        self.assertFalse(any(r['allowed'] for r in rows))

    def test_different_fields_at_same_offset_are_not_interchangeable(self):
        left = {'COFF.TimeDateStamp': {'offset': 0, 'size': 4},
                'IMAGE_DEBUG_DIRECTORY.TimeDateStamp': {'offset': 4, 'size': 4}}
        right = {'COFF.TimeDateStamp': {'offset': 4, 'size': 4},
                 'IMAGE_DEBUG_DIRECTORY.TimeDateStamp': {'offset': 0, 'size': 4}}
        rows = list(differences(bytes(8), bytes([1]) * 8, left, right))
        self.assertFalse(any(r['allowed'] for r in rows))

    def test_extra_zero_tail_still_differs(self):
        rows = list(differences(bytes(4), bytes(6), {}, {}))
        self.assertEqual([4, 5], [r['offset'] for r in rows])
        self.assertTrue(all(r['original'] is None and not r['allowed'] for r in rows))

    def test_missing_tail_is_reported(self):
        rows = list(differences(bytes(6), bytes(4), {}, {}))
        self.assertEqual([4, 5], [r['offset'] for r in rows])
        self.assertTrue(all(r['rebuilt'] is None for r in rows))

    def test_overlapping_fields_are_rejected(self):
        with self.assertRaises(ValueError):
            field_map(40, dict(self.fields(), **{'COFF.TimeDateStamp': {'offset': 9, 'size': 4}}))

    def test_unknown_field_and_out_of_range_are_rejected(self):
        for fields in ({'PdbPath': {'offset': 0, 'size': 16}}, self.fields(30)):
            with self.assertRaises(ValueError): field_map(40, fields)

    def test_all_difference_categories_account_for_every_unequal_byte(self):
        a = bytes(range(40)); b = bytes([255]) * 43
        rows = list(differences(a, b, self.fields(), self.fields()))
        self.assertEqual(43, len(rows))
        self.assertEqual(16, sum(r['allowed'] for r in rows))
        self.assertEqual(27, sum(not r['allowed'] for r in rows))

    def layout_fixture(self):
        data = bytearray(1024)
        struct.pack_into('<I', data, 60, 0x80)
        data[0x80:0x84] = b'PE\0\0'
        struct.pack_into('<H', data, 0x80 + 20, 224)
        struct.pack_into('<H', data, 0x98, 0x10b)
        struct.pack_into('<I', data, 0x98 + 92, 16)
        data[512:514] = bytes([6, 42])
        layout = dict(headers=dict(offset=0, size=512),
                      sections=[dict(name='.text', offset=512, size=512, virtual_address=8192)],
                      metadata=dict(offset=768, size=128),
                      streams=[dict(name='#~', offset=800, size=14, header_offset=784)],
                      tables=[dict(name='MethodDef', offset=800, size=14, row_count=1, row_size=14)])
        cols = [dict(name=n, width_original=w, width_rebuilt=w) for n, w in
                [('RVA', 4), ('ImplFlags', 2), ('Flags', 2), ('Name', 2), ('Signature', 2), ('ParamList', 2)]]
        detail = dict(table_columns=[dict(table='MethodDef', columns=cols)],
                      coordinates=dict(original={}, rebuilt={}))
        method = dict(token='[Method:0x0001]', raw_body_hex='06-2A', stable_id='T::M()->System.Void',
                      header=dict(size=1, code_size=1, format='tiny'),
                      instructions=[dict(offset=0, opcode='ret')])
        return bytes(data), layout, detail, method

    def test_regions_cover_method_bytes_and_metadata_columns(self):
        data, layout, detail, method = self.layout_fixture()
        labels = regions(data, layout, detail, 'original', [dict(RVA=8192)], [method])
        self.assertEqual(len(data), len(labels))
        self.assertTrue(labels[512].endswith('/header'))
        self.assertTrue(labels[513].endswith('/IL_0000/ret'))
        self.assertEqual('METADATA/MethodDef/row=1/RVA', labels[800])
        self.assertEqual('SECTION/.text', labels[1023])

    def test_unbound_method_dump_cannot_supply_labels(self):
        data, layout, detail, method = self.layout_fixture()
        method['raw_body_hex'] = '06-00'
        with self.assertRaises(ValueError):
            regions(data, layout, detail, 'original', [dict(RVA=8192)], [method])

    def test_shared_method_rva_keeps_both_owners(self):
        data, layout, detail, method = self.layout_fixture()
        alias = dict(method, token='[Method:0x0002]', stable_id='U::N()->System.Void')
        labels = regions(data, layout, detail, 'original', [dict(RVA=8192)] * 2, [method, alias])
        self.assertIn('T::M()', labels[513])
        self.assertIn('U::N()', labels[513])


if __name__ == '__main__':
    unittest.main()
