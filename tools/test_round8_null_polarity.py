"""Independent Roslyn1.1 conditional-null orientation fixtures; no game Emit."""
import hashlib,json,subprocess,sys
from pathlib import Path
ROOT=next(p for p in Path(__file__).resolve().parents if (p/'AGENTS.md').is_file())
sys.path.insert(0,str(ROOT/'tools'))
from kairo_candidate import COMPILER_PATH,COMPILER_SHA256,CORELIB_PATH
from round8_engine import il_cells
FOLDER=ROOT/'build/round8/null-conditional-polarity'
FORMS_SOURCE='''using System;
public class Fixture {
 public static long Native(object dummy,Array array){return (long)(array?.GetLength(0) ?? -1);}
 public static long Positive(object dummy,Array array){return (long)(((object)array!=null)?array.GetLength(0):-1);}
 public static long Negative(object dummy,Array array){return (long)(((object)array==null)?-1:array.GetLength(0));}
 public static long NotWrappedPositive(object dummy,Array array){return (long)((!((object)array!=null))?-1:array.GetLength(0));}
 public static long NotWrappedNegative(object dummy,Array array){return (long)((!((object)array==null))?array.GetLength(0):-1);}
 public static long IfPositive(object dummy,Array array){int result;if((object)array!=null){result=array.GetLength(0);}else{result=-1;}return (long)result;}
 public static long IfNegative(object dummy,Array array){int result;if((object)array==null){result=-1;}else{result=array.GetLength(0);}return (long)result;}
}
public class Program {
 public static void Main(){Check(null);Check(new int[0]);Check(new int[3]);Check(new string[2]);}
 static void Check(Array value){long n=Fixture.Native(null,value);if(Fixture.Positive(null,value)!=n||Fixture.Negative(null,value)!=n||Fixture.NotWrappedPositive(null,value)!=n||Fixture.NotWrappedNegative(null,value)!=n||Fixture.IfPositive(null,value)!=n||Fixture.IfNegative(null,value)!=n)throw new Exception("mismatch");Console.WriteLine(n);}
}
'''


def compile_toy(label,source=FORMS_SOURCE,exe=False):
    folder=FOLDER/'fixtures'/label;folder.mkdir(parents=True,exist_ok=True)
    path=folder/'Fixture.cs';path.write_text(source,encoding='utf-8',newline='')
    binary=folder/('Fixture.exe' if exe else 'Fixture.dll')
    flags=['/nologo','/nostdlib+','/target:'+('exe' if exe else 'library'),'/platform:anycpu','/optimize+','/debug:pdbonly',
        '/deterministic-','/langversion:6','/unsafe-','/checked-','/codepage:65001','/utf8output',
        '/out:'+str(binary),'/reference:'+str(CORELIB_PATH),str(path)]
    compiler=ROOT/COMPILER_PATH;assert hashlib.sha256(compiler.read_bytes()).hexdigest()==COMPILER_SHA256
    rsp=folder/'compile.rsp';rsp.write_text('\n'.join('"'+flag+'"' for flag in flags)+'\n',encoding='utf-8')
    result=subprocess.run([str(compiler),'/noconfig','@'+str(rsp)],capture_output=True,text=True,timeout=30)
    assert result.returncode==0,result.stdout+result.stderr
    if exe:
        result=subprocess.run([str(binary)],capture_output=True,text=True,timeout=15)
        assert result.returncode==0,result.stdout+result.stderr
        return result.stdout.splitlines()
    result=subprocess.run([str(ROOT/'build/tools/ProbeInspector.exe'),str(binary),'--all'],capture_output=True,check=True)
    methods={m['stable_id']:m for m in json.loads(result.stdout.decode('utf-8-sig'))['selected_methods']}
    (folder/'methods.json').write_text(json.dumps(methods,indent=2)+'\n',encoding='utf-8')
    return methods


def run_forms():
    methods=compile_toy('variants');select=lambda name:next(m for key,m in methods.items() if key.startswith('Fixture::'+name+'('))
    facets=('header','locals','instructions','exception_regions','extra_sections','method_attributes','impl_attributes')
    native=select('Native');negative=select('Negative');rows=[]
    for name in ('Native','Positive','Negative','NotWrappedPositive','NotWrappedNegative','IfPositive','IfNegative'):
        method=select(name)
        rows.append({'form':name,'first_conditional':next(i for i in method['instructions'] if i['opcode'].startswith(('brfalse','brtrue'))),
            'original_call_first_body_equal':all(method[k]==negative[k] for k in facets),
            'native_default_first_body_equal':all(method[k]==native[k] for k in facets),
            'instructions':method['instructions'],'locals':method['locals']})
    byname={r['form']:r for r in rows}
    # ?. proves its receiver non-null to this compiler and can use call for
    # this nonvirtual method. Explicit source uses callvirt. Do not erase that
    # measured difference merely because both forms put fallback first.
    positive=select('Positive')
    changed=[(a['opcode'],b['opcode']) for a,b in zip(native['instructions'],positive['instructions']) if a!=b]
    assert changed==[('call','callvirt')],changed
    assert byname['Positive']['first_conditional']['opcode']=='brtrue.s'
    assert byname['Negative']['first_conditional']['opcode']=='brfalse.s'
    assert byname['NotWrappedPositive']['original_call_first_body_equal']
    assert all(select('NotWrappedNegative')[k]==positive[k] for k in facets)
    # Compare the independently written fixture's emitted expression prefix
    # against read-only game evidence; never copy IL into any generated output.
    measured=json.loads((ROOT/'reports/round8-cycle05-after-measurement.json').read_text(encoding='utf-8-sig'))['assemblies']['KairoLibrary']
    dump=json.loads((ROOT/measured['provenance']['original_dump']).read_text(encoding='utf-8-sig'))['selected_methods']
    original=next(m for m in dump if m['stable_id']=='kairo.unity.util.StreamUtil::WriteArray(java.io.OutputStream,System.Array,System.Int32)->System.Void')
    assert il_cells(negative)[:14]==il_cells(original)[:14]
    assert il_cells(native)[:14]!=il_cells(original)[:14]
    traces=compile_toy('runtime',exe=True);assert traces==['-1','0','3','2']
    report={'operation':'Roslyn1.1-null-conditional-ternary-orientation','compiler_sha256':COMPILER_SHA256,
        'toy_library_compilations':1,'toy_executable_compilations':1,'runtime_scenarios':len(traces),'game_compilations':0,
        'game_expression_prefix_identical_bytes':14,'forms':rows,'runtime_results':traces,
        'conclusion':'negative-equality ternary puts member call first; positive inequality ternary puts fallback first',
        'native_vs_positive_difference':[{'original':a,'explicit':b} for a,b in zip(native['instructions'],positive['instructions']) if a!=b],
        'earlier_assertion_failure':'Initial expectation that Native and Positive whole bodies match was refuted: Native uses call, Positive callvirt. Branch orientation remains the same, and the full difference is preserved.',
        'fixture_source_sha256':hashlib.sha256(FORMS_SOURCE.encode('utf-8')).hexdigest()}
    (FOLDER/'forms.json').write_text(json.dumps(report,indent=2)+'\n',encoding='utf-8')
    print(json.dumps({'forms':[(r['form'],r['first_conditional']['opcode'],r['original_call_first_body_equal']) for r in rows],
        'runtime_scenarios':len(traces),'game_prefix_bytes':14}))




"""Syntax safety + independent Roslyn1.1 bodies/runtime for composed rule."""
import hashlib,json,runpy,shutil,subprocess,sys
from pathlib import Path
sys.path.insert(0,str(ROOT/'tools'))
from compiler_probes import ROSLYN
from test_round7_smcs_rules import apply
from kairo_candidate import CORELIB_PATH
import round7_transform
BASE_HELPER=runpy.run_path(str(ROOT/'reports/round8-selector-continue-fixture-driver.py'))['ORIGINAL_HELPER']
RULE=ROOT/'tools/Round8NullConditionalPolarityRules.cs'


def ensure_rule_helper():
    command=BASE_HELPER();base=Path(command[-1]).parent
    rsp=(base/'compile.rsp').read_text(encoding='utf-8-sig')
    inputs=[Path(line.strip().strip('"')) for line in rsp.splitlines() if line.strip().strip('"').endswith('.cs')]
    sources={p.name:p.read_text(encoding='utf-8-sig') for p in inputs};sources[RULE.name]=RULE.read_text(encoding='utf-8-sig')
    anchor='AddRound7SmcsRules(input,text,mapped,methodBody,model,point,patches,seen);'
    assert anchor in sources['Round7TransformSyntax.cs']
    sources['Round7TransformSyntax.cs']=sources['Round7TransformSyntax.cs'].replace(anchor,anchor+'\n        AddRound8NullConditionalPolarityRules(input,text,mapped,methodBody,model,point,patches,seen);')
    identity=hashlib.sha256(''.join(sources.values()).encode()).hexdigest()
    folder=FOLDER/'helper'/identity[:16];binary=folder/'Round7TransformSyntax.dll'
    if not binary.exists():
        folder.mkdir(parents=True,exist_ok=True)
        for name,text in sources.items():(folder/name).write_text(text,encoding='utf-8',newline='\n')
        for p in inputs:rsp=rsp.replace(str(p),str(folder/p.name))
        rsp=rsp.replace(str(base/'Round7TransformSyntax.dll'),str(binary))+'"'+str(folder/RULE.name)+'"\n'
        (folder/'compile.rsp').write_text(rsp,encoding='utf-8')
        process=subprocess.run([str(ROSLYN),'/noconfig','@'+str(folder/'compile.rsp')],capture_output=True,text=True)
        assert process.returncode==0,process.stdout+process.stderr
        for path in base.glob('Microsoft.CodeAnalysis*.dll'):shutil.copy2(path,folder/path.name)
        shutil.copy2(base/'Round7TransformSyntax.runtimeconfig.json',folder/'Round7TransformSyntax.runtimeconfig.json')
    return [command[0],str(binary)]


RULE_SOURCE='''using System;
public class Probe {
 public static int EqualityCalls;public static int GetterCalls;
 public int Value {get{GetterCalls++;return 7;}}
 public int? Nullable {get{return null;}}
 public string Text {get{return null;}}
 public int this[int n] {get{GetterCalls++;return n+7;}}
 public static bool operator ==(Probe a,Probe b){EqualityCalls++;return (object)a==(object)b;}
 public static bool operator !=(Probe a,Probe b){EqualityCalls++;return (object)a!=(object)b;}
 public override bool Equals(object other){return false;}
 public override int GetHashCode(){return 0;}
}
public class Fixture {
 volatile Probe changing;
 static Probe GetProbe(){return new Probe();} static int Fallback(){return -1;}
 public static long Array(object dummy,Array value){return (long)(value?.GetLength(0) ?? -1);}
 public static long ExpectedArray(object dummy,Array value){return (long)(((object)value==null)?-1:value.GetLength(0));}
 public static int Member(Probe value){return value?.Value ?? -1;}
 public static int ExpectedMember(Probe value){return ((object)value==null)?-1:value.Value;}
 public static int Index(Probe value){return value?[0] ?? -1;}
 public static int ExpectedIndex(Probe value){return ((object)value==null)?-1:value[0];}
 public static int NullableMember(Probe value){return value?.Nullable ?? -1;}
 public static string ReferenceMember(Probe value){return value?.Text ?? "fallback";}
 public static int Unstable(){return GetProbe()?.Value ?? -1;}
 public int Volatile(){return changing?.Value ?? -1;}
 public static int NonconstantFallback(Probe value){return value?.Value ?? Fallback();}
}
public class Program {
 static void Check(Probe value){Probe.EqualityCalls=0;Probe.GetterCalls=0;int a=Fixture.Member(value);int b=Fixture.ExpectedMember(value);int c=Fixture.Index(value);int d=Fixture.ExpectedIndex(value);if(a!=b||c!=d||Probe.EqualityCalls!=0||Probe.GetterCalls!=((object)value==null?0:4))throw new Exception("probe");Console.WriteLine(a+":"+c);}
 public static void Main(){Check(null);Check(new Probe());Array a=null;if(Fixture.Array(null,a)!=Fixture.ExpectedArray(null,a))throw new Exception("array-null");Console.WriteLine(Fixture.Array(null,a));a=new int[3];if(Fixture.Array(null,a)!=Fixture.ExpectedArray(null,a))throw new Exception("array");Console.WriteLine(Fixture.Array(null,a));}
}
'''


def run_rule():
    round7_transform.ensure_helper=ensure_rule_helper
    folder=FOLDER/'syntax';folder.mkdir(exist_ok=True);path=folder/'Fixture.cs';path.write_text(RULE_SOURCE,encoding='utf-8')
    manifest=folder/'manifest.json';manifest.write_text(json.dumps({'sources':[str(path)],'references':[str(CORELIB_PATH)]}),encoding='utf-8')
    hints={'original_nullable_call_first':True};checks=[];patches=[]
    with round7_transform.TransformClient(manifest) as syntax:
        def query(name,**flags):
            begin=RULE_SOURCE.index(' '+name+'(');point=RULE_SOURCE.index('return ',begin)
            response=syntax.request(path,source=RULE_SOURCE,offset=len(RULE_SOURCE[:point].encode('utf-16-le'))//2,max_candidates=4096,**flags)
            return [p for p in response.get('patches',[]) if p['rule']=='nullable-coalesce-negative-ternary']
        for name in ('Array','Member','Index'):
            found=query(name,compiler='roslyn',round7_cycle_rule='nullable-coalesce-negative-ternary',**hints)
            assert len(found)==1,(name,found);patches.append((name,found[0]));checks.append(name+':positive')
        for name in ('NullableMember','ReferenceMember','Unstable','Volatile','NonconstantFallback'):
            assert not query(name,compiler='roslyn',round7_cycle_rule='nullable-coalesce-negative-ternary',**hints),name
            checks.append(name+':reject')
        assert not query('Array',compiler='roslyn',**hints);checks.append('no-opt-in:reject')
        assert not query('Array',compiler='roslyn',round7_cycle_rule='nullable-coalesce-negative-ternary');checks.append('no-IL-evidence:reject')
        assert not query('Array',compiler='smcs',round7_cycle_rule='nullable-coalesce-negative-ternary',**hints);checks.append('smcs:reject')
        assert len(query('Array',compiler='roslyn',round7_cycle_rules=['nullable-coalesce-negative-ternary'],**hints))==1;checks.append('learned-family:positive')
    baseline=compile_toy('rule-baseline',RULE_SOURCE);traces=compile_toy('rule-baseline-run',RULE_SOURCE,True)
    select=lambda methods,name:next(m for key,m in methods.items() if key.startswith('Fixture::'+name+'('))
    facets=('header','locals','instructions','exception_regions','extra_sections','method_attributes','impl_attributes')
    proofs=[]
    for name,patch in patches:
        changed=apply(RULE_SOURCE,patch);after=select(compile_toy('rule-'+name,changed),name);expected=select(baseline,'Expected'+name)
        different=[k for k in facets if expected[k]!=after[k]];assert not different,(name,different)
        assert compile_toy('rule-'+name+'-run',changed,True)==traces
        proofs.append({'method':name,'body_equal':True,'il_size':after['header']['code_size'],'runtime_equal':True,'runtime_scenarios':len(traces)})
    report={'operation':'null-conditional-negative-ternary-composition','syntax_controls':len(checks),'checks':checks,
        'toy_library_compilations':4,'toy_executable_compilations':4,'runtime_runs':4,'game_compilations':0,'proofs':proofs,
        'object_equality_overload_calls':0,'rule_sha256':hashlib.sha256(RULE.read_bytes()).hexdigest()}
    (FOLDER/'rule-fixtures.json').write_text(json.dumps(report,indent=2)+'\n',encoding='utf-8')
    print(json.dumps({'controls':len(checks),'exact_bodies':len(proofs),'runtime_runs':4,'runtime_scenarios_per_run':len(traces)}))





if __name__=='__main__':
    run_forms();run_rule()
    read=lambda p:json.loads(p.read_text(encoding='utf-8-sig'))
    sha=lambda p:hashlib.sha256(p.read_bytes()).hexdigest()
    report={'operation':'Roslyn1.1-null-conditional-negative-ternary-fixtures',
        'forms':read(FOLDER/'forms.json'),'rule':read(FOLDER/'rule-fixtures.json'),
        'game_compilations':0,'driver_sha256':sha(Path(__file__)),
        'rule_sha256':sha(RULE),'hint_sha256':sha(ROOT/'tools/round8_null_polarity_hints.py')}
    (ROOT/'reports/round8-null-polarity-fixtures.json').write_text(json.dumps(report,indent=2)+'\n',encoding='utf-8')
