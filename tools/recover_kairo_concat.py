"""Isolated source trials for three remaining methods labelled concatenation.

All three already use the original String.Concat overloads. Trials address
source control flow/local variables, compile the entire frozen assembly and
never edit main sources, original artifacts or the method ledger.
"""
import hashlib
import argparse
import json
from pathlib import Path
import re
import subprocess
import sys
import time
from collections import Counter
from compare_kairo import differences
from fix_kairo_callvirt import mask_non_code

ROOT=Path(__file__).resolve().parents[1]
FOLDER=ROOT/'analysis/variants/kairo-concat-agent'
SHADER='src/KairoLibrary/kairo/unity/graphics/ShaderBuilder.cs'
STREAM='src/KairoLibrary/kairo/unity/util/StreamUtil.cs'
LANGUAGE='src/KairoLibrary/kairo/unity/util/Language.cs'
KEYS={
    SHADER:'kairo.unity.graphics.ShaderBuilder::GetName(System.Int32)->System.String',
    STREAM:'kairo.unity.util.StreamUtil::ReadArray(java.io.InputStream,System.Array,System.Type,System.Int32)->System.Object',
    LANGUAGE:'kairo.unity.util.Language::LoadLanguagePack(System.Byte[])->System.Void'}


def sha(data):return hashlib.sha256(data).hexdigest()


def method_span(text,header):
    if text.count(header)!=1:raise ValueError('Method header not unique: '+header)
    masked=mask_non_code(text);start=text.index(header);opening=masked.index('{',start+len(header));depth=0
    for end in range(opening,len(text)):
        depth+=(masked[end]=='{')-(masked[end]=='}')
        if depth==0:return start,opening,end+1
    raise ValueError('Unclosed method')


def shader_edit(text):
    start,opening,end=method_span(text,'private static string GetName(int nameBits)')
    body=text[opening:end];masked=mask_non_code(body)
    for match in reversed(list(re.finditer(r'\bnameBits\b',masked))):
        body=body[:match.start()]+'bits'+body[match.end():]
    newline='\r\n' if '\r\n' in text else '\n'
    body=body[:1]+newline+'\t\t\tint bits = nameBits;'+body[1:]
    replacements={
        'value = (((bits & 8) == 0) ? (value + "Clip/") : (value + "Mask/"));':
            ['if ((bits & 8) != 0)','\t\t\t\t\t{','\t\t\t\t\t\tvalue += "Mask/";','\t\t\t\t\t}',
             '\t\t\t\t\telse','\t\t\t\t\t{','\t\t\t\t\t\tvalue += "Clip/";','\t\t\t\t\t}'],
        'value = (((bits & 0x10) == 0) ? (value + "Shape") : (value + "Texture"));':
            ['if ((bits & 0x10) != 0)','\t\t\t\t{','\t\t\t\t\tvalue += "Texture";','\t\t\t\t}',
             '\t\t\t\telse','\t\t\t\t{','\t\t\t\t\tvalue += "Shape";','\t\t\t\t}']}
    for old,new in replacements.items():
        if body.count(old)!=1:raise ValueError('Unexpected ShaderBuilder ternary form')
        body=body.replace(old,newline.join(new))
    return text[:opening]+body+text[end:]


def closing_brace(masked,opening):
    depth=0
    for pos in range(opening,len(masked)):
        depth+=(masked[pos]=='{')-(masked[pos]=='}')
        if depth==0:return pos
    raise ValueError('Unclosed block')


def stream_edit(text):
    header='private static object ReadArray(InputStream stream, Array dst, Type type, int arrayLengthBytes = 4)'
    start,opening,end=method_span(text,header);body=text[opening:end]
    newline='\r\n' if '\r\n' in text else '\n'
    switch=body.index('switch (arrayLengthBytes)');masked=mask_non_code(body)
    begin=masked.index('{',switch);close=closing_brace(masked,begin)
    replacement=newline.join(['if (arrayLengthBytes == 1)','\t\t\t{','\t\t\t\tnum = ReadByte(stream);','\t\t\t}',
        '\t\t\telse if (arrayLengthBytes == 2)','\t\t\t{','\t\t\t\tnum = ReadShort(stream);','\t\t\t}',
        '\t\t\telse if (arrayLengthBytes == 8)','\t\t\t{','\t\t\t\tnum = ReadLong(stream);','\t\t\t}',
        '\t\t\telse','\t\t\t{','\t\t\t\tnum = ReadInt(stream);','\t\t\t}'])
    body=body[:switch]+replacement+body[close+1:]
    marker='if (type.GetInterface(typeof(ISerializable).Name) == null)'
    if body.count(marker)!=1:raise ValueError('Expected one serializable interface check')
    pos=body.index(marker);outer=body.rfind('else',0,pos);masked=mask_non_code(body)
    outer_open=masked.index('{',outer);outer_close=closing_brace(masked,outer_open)
    guard_open=masked.index('{',pos);guard_close=closing_brace(masked,guard_open)
    throw=body[guard_open+1:guard_close].strip()
    if throw!='throw new ArgumentException("StreamUtil.ReadArray : unsupported by " + type.FullName);':
        raise ValueError('Unexpected unsupported-type branch')
    loop=body[guard_close+1:outer_close]
    replacement='else if (type.GetInterface(typeof(ISerializable).Name) != null)'+newline+'\t\t\t{'+loop+'}'+newline+\
        '\t\t\telse'+newline+'\t\t\t{'+newline+'\t\t\t\t'+throw+newline+'\t\t\t}'
    body=body[:outer]+replacement+body[outer_close+1:]
    return text[:opening]+body+text[end:]


def language_edit(text):
    start,opening,end=method_span(text,'private static void LoadLanguagePack(byte[] data)')
    body=text[opening:end];newline='\r\n' if '\r\n' in text else '\n'
    # Preserve dictionary/key evaluation before construction, with the store to
    # the named list local nested inside the dictionary assignment.
    for variable,expression in [('value','dictionary[langPackRecord.File] = new List<LangPackRecord>()'),
            ('value4','dictionary2[key] = new List<int>()')]:
        old=variable+' = ('+expression+');'
        if body.count(old)!=1:raise ValueError('Expected one dictionary chained assignment')
        lhs,rhs=expression.split(' = ',1)
        body=body.replace(old,lhs+' = ('+variable+' = '+rhs+');')
    pattern=re.compile(r'(?m)^([\t ]*)(langPackRecord|jarInflater)\?\.([^\r\n]+);')
    def expand(match):
        indent,receiver,call=match.groups()
        return newline.join([indent+'if ('+receiver+' != null)',indent+'{',indent+'\t'+receiver+'.'+call+';',indent+'}'])
    body,count=pattern.subn(expand,body)
    if count!=3:raise ValueError('Expected three null-conditional sites')
    array_loop='string[] array4 = text6.Split(\'&\');'+newline+'\t\t\t\t\t\t\tfor (int i = 0; i < array4.Length; i++)'
    if body.count(array_loop)!=1 or body.count('array4[i]')!=1:raise ValueError('Expected one template position loop')
    body=body.replace(array_loop,'foreach (string positionText in text6.Split(\'&\'))')
    body=body.replace('array4[i]','positionText')
    # The original local signature places the three translation strings after
    # the template-loop locals, consistent with a nested lexical source scope.
    beginning=body.index('string text8 = null;')
    limit=body.index('if (num7 != Config.PID)',beginning)
    body=body[:beginning]+'{'+newline+'\t\t\t\t\t'+body[beginning:limit]+'}'+newline+'\t\t\t\t\t'+body[limit:]
    guard='if (langPackRecords_ == null)';position=body.index(guard);masked=mask_non_code(body)
    guard_open=masked.index('{',position);guard_close=closing_brace(masked,guard_open)
    if body[guard_open+1:guard_close].strip()!='return;':raise ValueError('Unexpected language-record guard')
    try_start=masked.index('try');try_open=masked.index('{',try_start);try_close=closing_brace(masked,try_open)
    if not guard_close<try_close:raise ValueError('Guard is not inside the expected outer try')
    body=body[:position]+'if (langPackRecords_ != null)'+newline+'\t\t\t\t{'+body[guard_close+1:try_close]+\
        '\t}'+newline+'\t\t\t'+body[try_close:]
    return text[:opening]+body+text[end:]


class FrozenTrial:
    def __init__(self,resume=False):
        if resume:
            report=json.loads((ROOT/'reports/KairoLibrary-concat-recovery.json').read_text())
            self.receipt_sha=report['snapshot_receipt_build_sha256'];self.records=report['trials']
            self.base=json.loads((FOLDER/'concat-agent-baseline-source-map.json').read_text())
            self.accepted=json.loads((ROOT/'analysis/kairo-concat-agent-source-map.json').read_text())
            self.baseline_label='concat-agent-baseline'
            self.baseline=json.loads((ROOT/'build/KairoLibrary-concat-agent-baseline/methods.json').read_text(encoding='utf-8-sig'))
            self.before={m['stable_id']:m for m in self.baseline['selected_methods']}
            self.original=json.loads((ROOT/'build/KairoLibrary-roslyn-1.3.2/original-methods.json').read_text(encoding='utf-8-sig'))
            self.oracle={m['stable_id']:m for m in self.original['selected_methods']}
            manifest=json.loads((ROOT/'build/KairoLibrary-concat-agent-baseline/build-manifest.json').read_text())
            for row in manifest['sources']:
                if sha(Path(self.base[row['path']]).read_bytes())!=row['sha256']:raise ValueError('Frozen source changed')
            return
        for attempt in range(20):
            receipt=json.loads((ROOT/'build/verification-KairoLibrary.json').read_text())
            blobs={row['path']:(ROOT/row['path']).read_bytes() for row in receipt['sources']}
            if all(sha(blobs[row['path']])==row['sha256'] for row in receipt['sources']):break
            time.sleep(0.5)
        else:raise ValueError('Could not capture source bytes matching the current verification receipt')
        self.receipt_sha=receipt['build_sha256'];self.base={};self.records=[];self.accepted={}
        for logical,data in blobs.items():
            path=FOLDER/'snapshot'/logical;path.parent.mkdir(parents=True,exist_ok=True);path.write_bytes(data)
            self.base[logical]=str(path)
        self.baseline_label='concat-agent-baseline'
        self.build(self.base,self.baseline_label)
        self.baseline=self.inspect(self.baseline_label,'--structure')
        self.before={m['stable_id']:m for m in self.baseline['selected_methods']}
        original=json.loads((ROOT/'build/KairoLibrary-roslyn-1.3.2/original-methods.json').read_text(encoding='utf-8-sig'))
        self.oracle={m['stable_id']:m for m in original['selected_methods']}
        self.original=original

    def build(self,mapping,label):
        path=FOLDER/(label+'-source-map.json');path.parent.mkdir(parents=True,exist_ok=True)
        path.write_text(json.dumps(mapping,indent=2)+'\n')
        result=subprocess.run([sys.executable,str(ROOT/'tools/build_kairo.py'),'--compiler',
            str(ROOT/'analysis/tools/roslyn-1.3.2/tools/csc.exe'),'--label',label,'--source-map',str(path)],
            capture_output=True,text=True,cwd=ROOT)
        if result.returncode:raise ValueError('Full trial build failed: '+result.stdout+result.stderr)

    def inspect(self,label,selector):
        folder=ROOT/'build'/('KairoLibrary-'+label);output=folder/('methods.json' if selector=='--structure' else 'target.json')
        with output.open('wb') as file:
            subprocess.run([str(ROOT/'build/tools/ProbeInspector.exe'),str(folder/'KairoLibrary.dll'),selector],stdout=file,check=True)
        return json.loads(output.read_text(encoding='utf-8-sig'))

    def trial(self,logical,text,label,description):
        path=FOLDER/label/logical;path.parent.mkdir(parents=True,exist_ok=True);path.write_bytes(text.encode('utf-8'))
        mapping=dict(self.base);mapping[logical]=str(path);self.build(mapping,label)
        key=KEYS[logical];selector=key.split('(')[0]
        target=next(m for m in self.inspect(label,selector)['selected_methods'] if m['stable_id']==key)
        delta,percent=differences(self.oracle[key],target)
        record={'id':key,'label':label,'description':description,'body_equal':not delta,'differences':delta,
            'percent':percent,'candidate_source':path.relative_to(ROOT).as_posix(),
            'base_source_sha256':sha(Path(self.base[logical]).read_bytes()),'candidate_sha256':sha(path.read_bytes()),
            'original_il_bytes':self.oracle[key]['header']['code_size'],'candidate_il_bytes':target['header']['code_size'],
            'original_locals':len(self.oracle[key]['locals']),'candidate_locals':len(target['locals'])}
        if not delta:
            candidate=self.inspect(label,'--structure');after={m['stable_id']:m for m in candidate['selected_methods']}
            regressions=[k for k,m in self.oracle.items() if 'raw_body_hex' in m and
                not differences(m,self.before.get(k))[0] and differences(m,after.get(k))[0]]
            record['previously_equal_bodies_lost']=regressions
            record['full_regression_check']='PASS' if not regressions else 'FAIL'
            if not regressions:self.accepted[logical]=str(path)
        self.records.append(record);self.save()
        print(json.dumps(record),flush=True)
        return record

    def save(self):
        accepted=ROOT/'analysis/kairo-concat-agent-source-map.json';accepted.write_text(json.dumps(self.accepted,indent=2)+'\n')
        diagnostics=[]
        for logical,key in KEYS.items():
            def concatenations(method):return Counter(i['operand']['method'] for i in method['instructions']
                if isinstance(i['operand'],dict) and 'method' in i['operand'] and '::Concat' in i['operand']['method'])
            old=concatenations(self.oracle[key]);new=concatenations(self.before[key])
            diagnostics.append({'id':key,'concat_signatures_and_counts_equal':old==new,
                'original_concat_signatures':dict(old),'baseline_concat_signatures':dict(new)})
        report={'snapshot_receipt_build_sha256':self.receipt_sha,'applied_to_main':False,'source_map':accepted.relative_to(ROOT).as_posix(),
            'trials':self.records,'concat_signature_diagnostics':diagnostics,
            'interpretation':'Concat overload identity already matches; source-local/control-flow differences diagnosed'}
        if hasattr(self,'combined'):report['combined']=self.combined
        (ROOT/'reports/KairoLibrary-concat-recovery.json').write_text(json.dumps(report,indent=2)+'\n',encoding='utf-8',newline='\n')

    def verify_accepted(self):
        mapping=dict(self.base);mapping.update(self.accepted);label='concat-agent-combined'
        self.build(mapping,label);document=self.inspect(label,'--structure')
        after={m['stable_id']:m for m in document['selected_methods']}
        regressions=[key for key,method in self.oracle.items() if 'raw_body_hex' in method and
            not differences(method,self.before.get(key))[0] and differences(method,after.get(key))[0]]
        targets={key:not differences(self.oracle[key],after.get(key))[0] for logical,key in KEYS.items() if logical in self.accepted}
        self.combined={'label':label,'full_assembly_build':'PASS','targets':targets,'previously_equal_bodies_lost':regressions,
            'validated':not regressions and all(targets.values())}
        self.save();print(json.dumps({'combined':self.combined}),flush=True)
        if not self.combined['validated']:raise ValueError('Combined candidates failed verification')


def main():
    parser=argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--resume',action='store_true');parser.add_argument('--method',choices=['shader','stream','language','all'],default='all')
    options=parser.parse_args();trial=FrozenTrial(options.resume)
    if options.method in ('shader','all'):
        shader=Path(trial.base[SHADER]).read_bytes().decode('utf-8')
        trial.trial(SHADER,shader_edit(shader),'concat-agent-shader-1',
            'Restore copied Int32 argument local and separate assignments in two if/else arms')
    if options.method in ('stream','all'):
        stream=Path(trial.base[STREAM]).read_bytes().decode('utf-8')
        trial.trial(STREAM,stream_edit(stream),'concat-agent-stream-1',
            'Restore ordered if/else length decoding and positive serializable arm before unsupported-type throw')
    if options.method in ('language','all'):
        language=Path(trial.base[LANGUAGE]).read_bytes().decode('utf-8')
        trial.trial(LANGUAGE,language_edit(language),'concat-agent-language-2',
            'Restore lexical scope, null checks, nested list assignments, array foreach and positive final guard')
    if len(trial.accepted)>1:trial.verify_accepted()


if __name__=='__main__':main()
