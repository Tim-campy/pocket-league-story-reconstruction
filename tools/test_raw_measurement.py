"""Controls for raw differences, complete partitions and malformed PE/CLI input."""
import json
from pathlib import Path
import struct
import subprocess
import tempfile
import unittest
from measure_raw_assemblies import (ROOT,byte_difference,checked_region,
    complement_regions,compare_regions,inspect_layout,measure_pair,row_measurement)

class RawMeasurementTests(unittest.TestCase):
    def test_unpaired_zero_byte_is_not_padded_away(self):
        r=byte_difference(b'\0',b'\0\0')
        self.assertEqual(r['different_common_bytes'],0)
        self.assertEqual(r['rebuilt_unpaired_bytes'],1)
        self.assertEqual(r['different_or_unpaired_bytes'],1)
        self.assertFalse(r['raw_bytes_equal'])

    def test_every_byte_difference_is_retained(self):
        r=byte_difference(bytes(range(32)),bytes([0]*32))
        self.assertEqual(r['different_common_bytes'],31)
        self.assertEqual(r['original_unpaired_bytes'],0)
        self.assertEqual(r['equal_common_bytes'],1)

    def test_region_bounds_and_duplicate_names_rejected(self):
        for region in [{'offset':-1,'size':1},{'offset':0,'size':-1},{'offset':2,'size':0}]:
            with self.assertRaises(ValueError):checked_region(b'x',region)
        with self.assertRaises(ValueError):
            compare_regions(b'xx',b'xx',[{'name':'same','offset':0,'size':1},{'name':'same','offset':1,'size':1}],[])

    def test_gaps_partition_all_bytes_and_overlaps_rejected(self):
        container={'offset':0,'size':10}
        regions=[{'name':'A','offset':1,'size':3},{'name':'B','offset':6,'size':2}]
        gaps=complement_regions(container,regions)
        self.assertEqual(sum(r['size'] for r in gaps),5)
        self.assertEqual([(r['offset'],r['size']) for r in gaps],[(0,1),(4,2),(8,2)])
        with self.assertRaises(ValueError):
            complement_regions(container,regions+[{'name':'overlap','offset':2,'size':1}])
        with self.assertRaises(ValueError):
            complement_regions(container,[{'name':'outside-zero','offset':11,'size':0}])

    def test_region_alignment_is_relative_and_separate_from_file_alignment(self):
        a=b'AApayload';b=b'xxxxpayload'
        regions_a=[{'name':'code','offset':2,'size':7}]
        regions_b=[{'name':'code','offset':4,'size':7}]
        r=compare_regions(a,b,regions_a,regions_b)[0]
        self.assertTrue(r['raw_bytes_equal']);self.assertEqual(r['offset_delta'],2)
        self.assertFalse(byte_difference(a,b)['raw_bytes_equal'])

    def test_row_comparison_preserves_raw_indexes_and_row_widths(self):
        a=b'\x01\x02\x03\x04';b=b'\x01\x02\0\x03\x05\0'
        r=row_measurement(a,b,{'offset':0,'row_count':2,'row_size':2},
                          {'offset':0,'row_count':2,'row_size':3})
        self.assertEqual(r['different_common_rows'],2)
        self.assertEqual(r['different_bytes_within_common_rows'],1)
        self.assertEqual(r['rebuilt_unpaired_bytes_within_common_rows'],2)

class RawParserControls(unittest.TestCase):
    @classmethod
    def setUpClass(cls):
        cls.source=ROOT/'build/round3/raw-baseline/KairoLibrary.dll'
        if not cls.source.is_file():raise unittest.SkipTest('Provide the user-owned reconstruction snapshot')
        cls.data=cls.source.read_bytes();cls.layout=inspect_layout(cls.source)

    def test_identity_measures_zero_without_normalization(self):
        r=measure_pair(self.source,self.source)
        self.assertEqual(r['raw_whole_file']['different_or_unpaired_bytes'],0)
        self.assertTrue(all(x['raw_bytes_equal'] for x in r['metadata_tables']))
        self.assertEqual(r['partition_accounting']['original_headers_sections_outside_total'],len(self.data))
        self.assertFalse(r['metric']['normalization_applied'])

    def _rejected(self,data):
        with tempfile.TemporaryDirectory(prefix='raw-parser-control-') as directory:
            p=Path(directory)/'Bad.dll';p.write_bytes(data)
            with self.assertRaises((ValueError,subprocess.CalledProcessError)):
                measure_pair(self.source,p)

    def test_invalid_mz_is_rejected(self):
        self._rejected(b'XX'+self.data[2:])

    def test_truncated_file_is_rejected(self):
        self._rejected(self.data[:100])

    def test_invalid_metadata_signature_is_rejected(self):
        data=bytearray(self.data);at=self.layout['metadata']['offset'];data[at:at+4]=b'BAD!'
        self._rejected(data)

    def test_overlapping_streams_are_rejected(self):
        data=bytearray(self.data);streams=self.layout['streams'];root=self.layout['metadata']['offset']
        source=next(s for s in streams if s['name']=='#~');target=next(s for s in streams if s['name']=='#Strings')
        struct.pack_into('<I',data,target['header_offset'],source['offset']-root)
        self._rejected(data)

if __name__=='__main__':
    suite=unittest.defaultTestLoader.loadTestsFromModule(__import__(__name__))
    result=unittest.TextTestRunner(verbosity=1).run(suite)
    report={'controls_total':result.testsRun,'controls_passed':result.testsRun-len(result.errors)-len(result.failures)-len(result.skipped),
        'errors':len(result.errors),'failures':len(result.failures),'skipped':len(result.skipped),
        'all_passed':result.wasSuccessful() and not result.skipped,
        'scope':'raw metrics and parser rejection; no project source/state edits'}
    (ROOT/'reports/round3-raw-negative-controls.json').write_text(json.dumps(report,indent=2)+'\n',encoding='utf-8')
    raise SystemExit(0 if report['all_passed'] else 1)
