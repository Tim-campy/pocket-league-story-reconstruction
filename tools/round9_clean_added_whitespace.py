"""Remove whitespace only on newly added blank C# lines; preserve line endings."""
import argparse
import json
from pathlib import Path
import re
import subprocess

ROOT=Path(__file__).resolve().parents[1]


def main():
    parser=argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--report',required=True,type=Path)
    args=parser.parse_args()
    diff=subprocess.check_output(['git','-c','core.quotepath=false','diff','--no-color','--unified=0','--','src'],cwd=ROOT).decode('utf-8')
    changes={};name=None;line_number=0
    for line in diff.splitlines():
        if line.startswith('+++ b/'):
            name=line[6:];line_number=0
        elif line.startswith('@@'):
            line_number=int(re.search(r'\+(\d+)',line).group(1))
        elif line.startswith('+') and name:
            if not line[1:].strip():changes.setdefault(name,[]).append(line_number)
            line_number+=1
        elif line.startswith(' '):line_number+=1
    actual={}
    for name,numbers in changes.items():
        path=(ROOT/name).resolve()
        if not path.is_relative_to(ROOT/'src'):raise ValueError('Outside source tree')
        lines=path.read_bytes().splitlines(keepends=True);removed=[]
        for number in numbers:
            before=lines[number-1]
            if before.strip():raise ValueError('Refuse nonblank C# line')
            after=b'\r\n' if before.endswith(b'\r\n') else b'\n'
            if before!=after:lines[number-1]=after;removed.append(number)
        if removed:path.write_bytes(b''.join(lines));actual[name]=removed
    result={'changed_sources':actual,'blank_lines':sum(len(v) for v in actual.values()),
            'line_count_preserved':True,'line_endings_preserved':True,'nonblank_lines_edited':0}
    args.report.write_text(json.dumps(result,indent=2)+'\n',encoding='utf-8',newline='\n')
    print(json.dumps(result))


if __name__=='__main__':main()
