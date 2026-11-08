public class Storage{public string[] GetFiles(){return null;}public void Delete(string name){}}
public static class KairoPlugin{public static string[] GetPreferenceKeys(){return null;}public static void RemovePreference(string key){}}
public class Fixture{int rsId_;static string[] resourceFiles_;static bool usePreference_;static Storage storage_;public void Run(){if(rsId_==1&&resourceFiles_!=null){return;}if(!usePreference_){
string[] files=storage_.GetFiles();foreach (string filename in files){storage_.Delete(filename);}
}else{
string[] files = KairoPlugin.GetPreferenceKeys();for (int i = 0; i < files.Length; i++){KairoPlugin.RemovePreference(files[i]);}
}}}