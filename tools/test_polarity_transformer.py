"""Test integral/Boolean negation and reject floating/nullable operator flipping."""
import json,subprocess,tempfile,unittest
from pathlib import Path
from compiler_probes import ROSLYN,UNITY
ROOT=Path(__file__).resolve().parents[1]
class PolarityControls(unittest.TestCase):
    @classmethod
    def setUpClass(cls):
        cls.tmp=tempfile.TemporaryDirectory(dir=ROOT/'build',prefix='polarity-controls-');folder=Path(cls.tmp.name)
        src=folder/'Fixture.cs'
        src.write_text('''public class PolarityFixture {
 public string Eq(int a,int b){return a!=b?"a":"b";}
 public string Rel(int a,int b){return a<b?"a":"b";}
 public string Float(float a,float b){return a<b?"a":"b";}
 public string Double(double a,double b){return a>=b?"a":"b";}
 public string Nullable(int? a,int? b){return a<b?"a":"b";}
 public string And(int a,int b){return a==0&&b<3?"a":"b";}
 public string Or(int a,int b){return a==0||b<3?"a":"b";}
}''',encoding='utf-8')
        manifest=folder/'input.json';manifest.write_text(json.dumps({'sources':[str(src)],'references':[str(UNITY/'Editor/Data/Mono/lib/mono/2.0/mscorlib.dll')]}))
        vs=next(p for p in ROSLYN.parents if (p/'dotnet').is_dir())
        p=subprocess.run([str(vs/'dotnet/net10.0/runtime/dotnet.exe'),str(ROOT/'build/source-recovery/IlRuleSyntax.dll'),str(manifest)],capture_output=True,check=True)
        doc=json.loads(p.stdout);cls.patches={m['id'].split('::')[1].split('(')[0]:next(p['after'] for p in m['patches'] if p['rule']=='ternary-polarity') for m in doc['methods']}
        if doc['errors']:raise ValueError(doc['errors'])
    @classmethod
    def tearDownClass(cls):cls.tmp.cleanup()
    def test_equality_rewritten(self):self.assertIn('a == b',self.patches['Eq'])
    def test_integral_relational_rewritten(self):self.assertIn('a >= b',self.patches['Rel'])
    def test_float_relational_keeps_explicit_not(self):self.assertIn('!(a<b)',self.patches['Float'])
    def test_double_relational_keeps_explicit_not(self):self.assertIn('!(a>=b)',self.patches['Double'])
    def test_nullable_relational_keeps_explicit_not(self):self.assertIn('!(a<b)',self.patches['Nullable'])
    def test_de_morgan_and(self):self.assertIn('(a != 0) || (b >= 3)',self.patches['And'])
    def test_de_morgan_or(self):self.assertIn('(a != 0) && (b >= 3)',self.patches['Or'])
if __name__=='__main__':unittest.main()
