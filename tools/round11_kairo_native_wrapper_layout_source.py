"""Managed parent C# only; two independently selectable layout subfamilies."""
import re
def tokens(s):
 p=r'//[^\n]*|/\*[\s\S]*?\*/|@"(?:[^"]|"")*"|"(?:\\.|[^"\\])*"|[A-Za-z_][A-Za-z_0-9]*|\d+(?:\.\d+)?[fFdDmMuUlL]*|==|!=|<=|>=|&&|\|\||[^\s]'
 return [(m.group(),m.start(),m.end()) for m in re.finditer(p,s) if not m.group().startswith(('//','/*'))]
def replace(s,old,new):
 ts=tokens(s);w=[t[0] for t in tokens(old)];hits=[i for i in range(len(ts)-len(w)+1) if [x[0] for x in ts[i:i+len(w)]]==w]
 if len(hits)!=1:raise ValueError('reviewed-template-not-unique:'+old[:65])
 i=hits[0];return s[:ts[i][1]]+new+s[ts[i+len(w)-1][2]:]
def transform_lobi(s):
 old='''AndroidJavaObject androidJavaObject = ((type == 0) ? new AndroidJavaObject("android.content.Intent", "net.kairosoft.android.LOBI_CHAT") : ((type != 1) ? new AndroidJavaObject("android.content.Intent", androidJavaClass.GetStatic<string>("ACTION_MAIN")) : new AndroidJavaObject("android.content.Intent", "net.kairosoft.android.LOBI_RANKING")));'''
 new='''AndroidJavaObject androidJavaObject;
if(type==0){androidJavaObject=new AndroidJavaObject("android.content.Intent","net.kairosoft.android.LOBI_CHAT");}
else if(type==1){androidJavaObject=new AndroidJavaObject("android.content.Intent","net.kairosoft.android.LOBI_RANKING");}
else{androidJavaObject=new AndroidJavaObject("android.content.Intent",androidJavaClass.GetStatic<string>("ACTION_MAIN"));}'''
 return replace(s,old,new)
def transform_save(s):
 old='''if (!kairo.common.cfg.Config.UNITY_EDITOR) {
int aPPLI_TYPE = kairo.common.cfg.Config.APPLI_TYPE;
int num = 2;
return (object)null;
}
string text2 = app_.GetDataPath() + "/tmp/capture";
if (!Directory.Exists(text2)) { Directory.CreateDirectory(text2); }
File.WriteAllBytes(text2 + "/" + file, data);
Log.Info(text2 + "/" + file);
return (object)null;'''
 new='''if (kairo.common.cfg.Config.UNITY_EDITOR) {
string text2 = app_.GetDataPath() + "/tmp/capture";
if (!Directory.Exists(text2)) { Directory.CreateDirectory(text2); }
File.WriteAllBytes(text2 + "/" + file, data);
Log.Info(text2 + "/" + file);
} else { if(kairo.common.cfg.Config.APPLI_TYPE==2){} return (object)null; }
return (object)null;'''
 # == is a representative, NOT a recovered historical relation: the original
 # dead conditional branch has been replaced by pop;pop. Record ambiguity.
 return replace(s,old,new)
