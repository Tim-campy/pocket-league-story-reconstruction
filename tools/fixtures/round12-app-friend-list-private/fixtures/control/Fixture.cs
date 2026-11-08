using System;using System.Collections;using System.Text;
public class Vector{ArrayList list=new ArrayList();public static int Creates;public Vector(){Creates++;}public void AddElement(object value){list.Add(value);}public object ElementAt(int n){return list[n];}public int Size(){return list.Count;}public void RemoveAllElements(){list.Clear();}}
public class State{int value;public State(int n){value=n;}public int Get(){return value;}}
public class Friend{public State state_;public Friend(string value){state_=new State(int.Parse(value));}}
public class KairoBase{public static int Calls;public static byte[] Connect(string path){Calls++;return Encoding.UTF8.GetBytes("1,3,1");}}
public class JString{public static string New(byte[] data){return Encoding.UTF8.GetString(data);}}
public class StringUtil{public static string[] Split(string s,string separator){return s.Split(separator.ToCharArray());}}
public class FixtureBase{public Vector friendList_=new Vector(),applyList_=new Vector(),BaseVector=new Vector();public int BaseCalls;public virtual Vector CgiGetFriendList(){BaseCalls++;return BaseVector;}}
public class Fixture:FixtureBase{
public override Vector CgiGetFriendList(){bool useCustomFriendList=false;if(useCustomFriendList==false){return base.CgiGetFriendList();}Vector vector=new Vector();Vector vector2=new Vector();byte[] array=KairoBase.Connect("owned/mock");if(array.Length==0){return friendList_;}string[] array2=StringUtil.Split(JString.New(array),",");int num=0;while(num<array2.Length){Friend friend=new Friend(array2[num]);if(friend.state_.Get()==1){vector2.AddElement(friend);}else if(friend.state_.Get()==3){vector.AddElement(friend);}num++;}friendList_.RemoveAllElements();int num2=0;while(num2<vector.Size()){friendList_.AddElement(vector.ElementAt(num2));num2++;}applyList_.RemoveAllElements();int num3=0;while(num3<vector2.Size()){applyList_.AddElement(vector2.ElementAt(num3));num3++;}return friendList_;}
public static int SemanticChecks(){Fixture f=new Fixture();f.BaseCalls=0;KairoBase.Calls=0;Vector.Creates=0;Vector result=f.CgiGetFriendList();if(!Object.ReferenceEquals(result,f.BaseVector)||f.BaseCalls!=1||KairoBase.Calls!=0||Vector.Creates!=0)return 1;return 0;}
public static float FloatNeg(float v){return -v;}public static float FloatZeroSub(float v){return 0f-v;}
}