"""Store final expression back in the existing local before returning it."""
RULE='r12-smcs-final-expression-store-back-before-return'
CASES=[('main.AppData::GetTime(System.Int32,System.Int32)->System.Int32','src/Assembly-CSharp/main/AppData.cs','public int GetTime(int week, int time)','return (num + 43200) % 86400;','num = (num + 43200) % 86400;\nreturn num;'),('system.billing.util.Json::MakeJson(System.String[])->System.String','src/Assembly-CSharp/system/billing/util/Json.cs','private static string MakeJson(string[] records)','return text + "}";','text += "}";\nreturn text;')]
def transform(method):
 hits=[c for c in CASES if method.count(c[3])==1]
 if len(hits)!=1:raise ValueError('One exact final expression required')
 c=hits[0];return method.replace(c[3],c[4],1)
