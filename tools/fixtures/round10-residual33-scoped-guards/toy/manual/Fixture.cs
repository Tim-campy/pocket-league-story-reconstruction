using System; using System.Collections;
class PlayerData {public int game_goalInLeague_;public string name_;}
class Player {public int[] address_=new int[2];public bool ChTrapOrHeading(){return address_[0]>0;}}
class Ball {public int[] address_=new int[2];}
class Vector {ArrayList items=new ArrayList();public int Size(){return items.Count;}public object ElementAt(int i){return items[i];}public void AddElement(object o){items.Add(o);}public void InsertElementAt(object o,int i){items.Insert(i,o);}}
class ByteArrayInputStream {public ByteArrayInputStream(byte[] b){}public void Close(){}}
class StreamUtil {public static bool ReadBoolean(ByteArrayInputStream b){return b!=null;}}
class TradeData {public bool new_;public bool IsPlayerInMember(){return new_;}public void ChangeState(int v){new_=v>0;}}
class Save {public byte[][] bins=new byte[5][];}
class AppData {public Save sysSave_=new Save();public TradeData[] tradeData_=new TradeData[2];}
public class Fixture {Vector members=new Vector(),out_ary1=new Vector(),out_ary2=new Vector(),hitList=new Vector();Player[] players=new Player[2];Ball ball_=new Ball();AppData apdat_=new AppData();
public void Ranking(){for(int l=0;l<members.Size();l++){PlayerData playerData=(PlayerData)members.ElementAt(l);if(playerData.game_goalInLeague_>0){int num3=0;for(num3=0;num3<out_ary2.Size();num3++){int[] array3=(int[])out_ary2.ElementAt(num3);if(playerData.game_goalInLeague_>=array3[0]){break;}}if(num3>=0&&num3<=out_ary1.Size()&&num3<=out_ary2.Size()){out_ary1.InsertElementAt(playerData.name_,num3);out_ary2.InsertElementAt(new int[3]{playerData.game_goalInLeague_,0,0},num3);}}}}
public void Repetition(){for(int i=1;i<players.Length;i++){Player player=players[i];if(player.address_[0]-ball_.address_[0] < -2 ||player.address_[0]-ball_.address_[0] >2){continue;}if(player.address_[1]-ball_.address_[1] < -2 ||player.address_[1]-ball_.address_[1] >2){continue;}if(player.ChTrapOrHeading()){hitList.AddElement(player);}}}
public void Reflect(){if(apdat_.sysSave_.bins[4].Length>0){ByteArrayInputStream byteArrayInputStream5=null;try{byteArrayInputStream5=new ByteArrayInputStream(apdat_.sysSave_.bins[4]);for(int n=0;n<apdat_.tradeData_.Length;n++){TradeData tradeData=apdat_.tradeData_[n];bool recoveredTradePresent=StreamUtil.ReadBoolean(byteArrayInputStream5);if(recoveredTradePresent){if(tradeData.IsPlayerInMember()){tradeData.ChangeState(2);tradeData.new_=false;}else{tradeData.ChangeState(1);tradeData.new_=false;}}}}finally{if(byteArrayInputStream5!=null){byteArrayInputStream5.Close();}byteArrayInputStream5=null;}}}
}
