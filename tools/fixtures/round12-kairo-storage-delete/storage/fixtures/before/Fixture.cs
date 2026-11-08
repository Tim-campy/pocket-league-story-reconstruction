using System;using System.IO;using System.Linq;
public class Vector{public void RemoveElementAt(int value){}}
public static class Config{public static bool USE_HEAP_STORAGE;public static string ConvertExtension(string name){return name;}}
public class NotPermitException:Exception{public enum PermissionType{Storage=0}public NotPermitException(PermissionType permission,string message,Exception cause):base(message,cause){}}
public class Fixture{int media_;string folder_;static Vector heapData_=new Vector(),heapName_=new Vector();
int _SearchHeapFile(string filename){return 0;}bool Exists(string filename){return false;}
public void Run(string filename){try{
filename=Config.ConvertExtension(filename);
if (Config.USE_HEAP_STORAGE && media_ == 2){
 int num=_SearchHeapFile(filename);if(num!=-1){heapData_.RemoveElementAt(num);heapName_.RemoveElementAt(num);}
}else{
 if (media_ == 3 || (media_ != 1 && media_ != 4)){return;}
 if (filename.Contains(':')){if(File.Exists(filename)){File.Delete(filename);}}
 else if(Exists(filename)){File.Delete(folder_+"/"+filename);}
}
}catch(UnauthorizedAccessException ex){throw new NotPermitException(NotPermitException.PermissionType.Storage,ex.Message,ex);}}}
