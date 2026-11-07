"""Mapping tests: refuse ambiguous/mismatched stores; preserve implicit .cctor."""
import json
from pathlib import Path
import tempfile
import unittest

from round10_initializer_mapping import bind_initializer, field_matches
from round10_transform import TransformClient

ROOT = Path(__file__).resolve().parents[1]


class InitializerMappingTests(unittest.TestCase):
    def test_store_requires_exact_owner_and_name(self):
        def ins(operand): return {'opcode':'stsfld','offset':20,'operand':{'field':operand}}
        self.assertTrue(field_matches('Example::value', ins('class Example @ Target::value:valuetype System.Int32')))
        self.assertFalse(field_matches('Example::value', ins('class Other @ Target::value:valuetype System.Int32')))
        self.assertFalse(field_matches('Example::value', ins('class Example @ Target::value2:valuetype System.Int32')))

    def test_implicit_constructor_and_candidate(self):
        manifest=json.loads((ROOT/'build/round10/baseline/Assembly-CSharp/build-manifest.json').read_text(encoding='utf-8-sig'))
        with tempfile.TemporaryDirectory(dir=ROOT/'build/round10', prefix='initializer-fixture-') as tmp:
            folder=Path(tmp)
            source=folder/'Fixture.cs'
            text=('class InitializerFixture { public static int flag; '
                  'public static string value = flag != 0 ? "right" : "left"; '
                  'public const string excluded = "constant"; '
                  'public string instance = "instance"; }')
            source.write_text(text,encoding='utf-8')
            manifest['sources']=[{'path':str(source)}]
            mp=folder/'manifest.json';mp.write_text(json.dumps(manifest),encoding='utf-8')
            key='InitializerFixture::.cctor()->System.Void'
            with TransformClient(mp) as client:
                mapped=client.request(path=source,method_id=key,mode='map-static-initializers')
                self.assertEqual([f['field_id'] for f in mapped['fields']],['InitializerFixture::value'])
                self.assertFalse(mapped['fields'][0]['explicit_static_constructor'])
                response=client.request(path=source,method_id=key,offset=mapped['fields'][0]['start'])
                self.assertEqual(response['mapped']['kind'],'StaticFieldInitializer')
                self.assertEqual(response['mapped']['method_id'],key)
                self.assertEqual(response['patches'][0]['after'],'(flag == 0) ? "left" : "right"')
                self.assertTrue(response['disable_target_screening'])
                refused=client.request(path=source,method_id='Other::.cctor()->System.Void',offset=mapped['fields'][0]['start'])
                self.assertEqual(refused['error'],'static_initializer_owner_mismatch')
                left={'stable_id':key,'instructions':[{'offset':20,'opcode':'stsfld','operand':{'field':'class InitializerFixture @ Target::value:class System.String'}}]}
                found=bind_initializer(client,{'Fixture.cs':source},left,left,{'original_offset':0,'rebuilt_offset':0})
                self.assertEqual(found['field']['field_id'],'InitializerFixture::value')
                other={'stable_id':key,'instructions':[{'offset':20,'opcode':'stsfld','operand':{'field':'class Other @ Target::value:class System.String'}}]}
                self.assertIsNone(bind_initializer(client,{'Fixture.cs':source},left,other,{'original_offset':0,'rebuilt_offset':0}))


if __name__=='__main__': unittest.main()
