
public static class Fixture {
 static string[][] softlabelTable_,BUILTIN_SOFTLABEL_TABLE;static object langPackRecords_;static int setting;
 static int Get(){return setting;}
 public static string Run(string softlabel){
  if (softlabel == null || softlabel.Length > 10){return softlabel;}
  string originalInput=softlabel;
  if(softlabelTable_!=null){for(int i=0;i<softlabelTable_.Length;i++){
   if(originalInput.Length==softlabelTable_[i][0].Length&&originalInput.Equals(softlabelTable_[i][0])){return softlabelTable_[i][1];}
  }}
  int num=Get();if(num!=0){try{for(int j=0;j<BUILTIN_SOFTLABEL_TABLE.Length;j++){
   if(originalInput.Length==BUILTIN_SOFTLABEL_TABLE[j][0].Length&&originalInput.Equals(BUILTIN_SOFTLABEL_TABLE[j][0])){
    if(num>=BUILTIN_SOFTLABEL_TABLE[j].Length){num=1;}
    if(langPackRecords_!=null&&BUILTIN_SOFTLABEL_TABLE[j].Length>=3){num=2;}
    return BUILTIN_SOFTLABEL_TABLE[j][num];
   }
  }}catch{}}
  return softlabel;
 }
}
