using System;
public class PlayerLike {public int[] address_=new int[2];public int screen_pos_;public float score;}
public class TouchComponent {public int id,value;}
public class TouchEvent {public TouchComponent component;public int type;}
public class LanguageForm {}
public class FormManager {static FormManager manager=new FormManager();public static FormManager GetInstance(){return manager;}public bool Push(LanguageForm f){return f!=null;}}
public class GameUtil {public static bool ChOnLine(int a,int b,int n,int c){return a+b+n>c;}}
public class Fixture {int[] moveDirectBox=new int[3];int[] address_=new int[2];int p_id_,screen_pos_;PlayerLike[] players=new PlayerLike[2];PlayerLike player=new PlayerLike();float score;
public bool Move(int ch_direct){int num=0;int num2;int num3;int[] array=new int[1];for(int i=0;i<moveDirectBox.Length;i++){num++;if (moveDirectBox[i] == -1) { continue; }
if (ch_direct != moveDirectBox[i]) {if(ch_direct>0){num2=ch_direct;num3=-ch_direct;}else{num2=-ch_direct;num3=ch_direct;}if(num2>num3){num2=num3;}if(array[0]<=num){return true;}return false;
}}return true;}

public bool Pid(){bool flag=false;for(int j=0;j<players.Length;j++){PlayerLike player2=players[j];if (address_[0] == player2.address_[0] && address_[1] == player2.address_[1]) { continue; }
if (GameUtil.ChOnLine(screen_pos_,player.screen_pos_,6,player2.screen_pos_)) {flag=true;break;
}}for(int l=1;l<players.Length;l++){if (l == p_id_) { continue; }PlayerLike player3=players[l];float num7=player3.score;if(num7>0f){score=num7;}}return flag;}

public bool Touch(TouchEvent e){TouchComponent component=e.component;FormManager instance=FormManager.GetInstance();if(component.id==3){if (component.value == 20160614) { switch (e.type) { case 1: instance.Push(new LanguageForm()); break; }return false;
}}return true;}

}
