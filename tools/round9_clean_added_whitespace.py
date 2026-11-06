"""Trim added-line trivia while preserving C# literals and exact line endings."""
import argparse
import json
from pathlib import Path
import re
import subprocess

ROOT=Path(__file__).resolve().parents[1]
TOKENS=re.compile(r'//[^\r\n]*|/\*.*?\*/|@"(?:[^"]|"")*"|"(?:\\.|[^"\\])*"|\'(?:\\.|[^\'\\\r\n])*\'',re.S)


def clean_source_lines(text,numbers):
    # Interpolated literals have nested lexical contexts. Be conservative here;
    # their files need a syntax-aware formatter rather than this trivia pass.
    interpolation=any(prefix in text for prefix in ('$"','$@"','@$"'))
    protected=[match.span() for match in TOKENS.finditer(text)
               if not match.group().startswith(('//','/*'))]
    pieces=text.split('\n');lines=[piece+'\n' for piece in pieces[:-1]]+([pieces[-1]] if pieces[-1] else [])
    offsets=[];at=0
    for line in lines:offsets.append(at);at+=len(line)
    changed=[];blank=0;nonblank=0;refused=[]
    for number in sorted(set(numbers)):
        if not 1<=number<=len(lines):raise ValueError('Added line outside source')
        before=lines[number-1]
        ending='\r\n' if before.endswith('\r\n') else '\n' if before.endswith('\n') else ''
        content=before[:-len(ending)] if ending else before
        trimmed=content.rstrip(' \t')
        if trimmed==content:continue
        start,end=offsets[number-1]+len(trimmed),offsets[number-1]+len(content)
        if interpolation or any(start<b and a<end for a,b in protected):
            refused.append(number);continue
        lines[number-1]=trimmed+ending;changed.append(number)
        if trimmed.strip():nonblank+=1
        else:blank+=1
    return ''.join(lines),changed,blank,nonblank,refused


def main():
    parser=argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--report',required=True,type=Path)
    args=parser.parse_args()
    diff=subprocess.check_output(['git','-c','core.quotepath=false','diff','HEAD','--no-color','--unified=0','--','src'],cwd=ROOT).decode('utf-8')
    changes={};name=None;line_number=0
    for line in diff.splitlines():
        if line.startswith('+++ b/'):
            name=line[6:];line_number=0
        elif line.startswith('@@'):
            line_number=int(re.search(r'\+(\d+)',line).group(1))
        elif line.startswith('+') and name:
            changes.setdefault(name,[]).append(line_number)
            line_number+=1
        elif line.startswith(' '):line_number+=1
    actual={};blank=0;nonblank=0;refused={}
    for name,numbers in changes.items():
        path=(ROOT/name).resolve()
        if not path.is_relative_to(ROOT/'src'):raise ValueError('Outside source tree')
        text=path.read_bytes().decode('utf-8')
        after,removed,b,n,protected=clean_source_lines(text,numbers)
        blank+=b;nonblank+=n
        if protected:refused[name]=protected
        if removed:path.write_bytes(after.encode('utf-8'));actual[name]=removed
    result={'changed_sources':actual,'blank_lines':blank,
            'line_count_preserved':True,'line_endings_preserved':True,'nonblank_lines_edited':nonblank,
            'literal_or_interpolation_protected_lines':refused}
    args.report.write_text(json.dumps(result,indent=2)+'\n',encoding='utf-8',newline='\n')
    print(json.dumps(result))


if __name__=='__main__':main()
