"""Normal C# alternatives for the sole struct array-load difference."""
import re
def tokens(s):
 p=r'//[^\n]*|/\*[\s\S]*?\*/|@"(?:[^"]|"")*"|"(?:\\.|[^"\\])*"|[A-Za-z_][A-Za-z_0-9]*|\d+|==|!=|<=|>=|&&|\|\||\+\+|[^\s]'
 return [(m.group(),m.start(),m.end()) for m in re.finditer(p,s) if not m.group().startswith(('//','/*'))]
def replace(s,old,new):
 ts=tokens(s);w=[x[0] for x in tokens(old)];hits=[i for i in range(len(ts)-len(w)+1) if [x[0] for x in ts[i:i+len(w)]]==w]
 if len(hits)!=1:raise ValueError('reviewed-template-not-unique')
 i=hits[0];return s[:ts[i][1]]+new+s[ts[i+len(w)-1][2]:]
FOR='''GameAppli[] appList = instance.GetAppList();
for(int i=0;i<appList.Length;i++){
if(appList[i].Pid==gameId_){flag=true;}
}'''
def transform_local_copy(s,implicit=False):
 if any(t[0]=='recoveredApp' for t in tokens(s)):raise ValueError('copy-local-name-collision')
 typ='var' if implicit else 'GameAppli'
 new='''GameAppli[] appList=instance.GetAppList();
for(int i=0;i<appList.Length;i++){
'''+typ+''' recoveredApp=appList[i];
if(recoveredApp.Pid==gameId_){flag=true;}
}'''
 return replace(s,FOR,new)
def transform_foreach_copy(s):
 if any(t[0]=='recoveredApp' for t in tokens(s)):raise ValueError('copy-local-name-collision')
 return replace(s,FOR,'foreach(GameAppli recoveredApp in instance.GetAppList()){if(recoveredApp.Pid==gameId_){flag=true;}}')
def transform_cast_copy(s):
 return replace(s,FOR,FOR.replace('appList[i].Pid','((GameAppli)appList[i]).Pid'))
