using System;
public class Canvas{public bool CheckKeyRepeat(int key){return key>0;}}
public class Data{public int GetMatchInfoJoin(){return 0;}}
public class Fixture{
 Canvas canvas_=new Canvas();Data udat_=new Data();int value;
 void Use(int n){value+=n;}bool CheckAlreadyEvent(int n){return n==value;}
 public bool Run(int seed){
 int num=seed;
 if(canvas_.CheckKeyRepeat(131072)){value=(value-1+num)%num;}
 if(seed>0){int captured=seed;Action action=delegate{Use(captured);};action();}
 int recoveredMatchInfoJoin=udat_.GetMatchInfoJoin();
 if(recoveredMatchInfoJoin==0 && !CheckAlreadyEvent(176)){Use(176);}
 return true;
}
}