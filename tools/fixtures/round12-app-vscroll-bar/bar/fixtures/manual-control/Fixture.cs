using kairo.unity.surface;
namespace kairo.unity.surface{
public class TouchOption{public int[] args;}
public class TouchComponent{public TouchOption option;public int id;}
public class TouchEvent{public TouchComponent component;public int dy;}
}
public class Fixture{public int Get(int key){return key;}public void Set(int key,int value,TouchComponent component){}
public void Run(int type,int v,TouchEvent e){TouchComponent target=e.component;
int[] parameters=e.component.option.args;
int count=parameters[0];
int page=parameters[1];
int selection=(2<parameters.Length)?parameters[2]:-1;
int offset=int.MaxValue;
switch(type){case 8:offset=Get(v)+e.dy;break;case 9:offset=e.dy;break;}
if(offset==int.MaxValue){return;}
if(offset>count-page){offset=count-page;}
if(offset<0){offset=0;}
if(Get(v)!=offset){
 int delta=offset-Get(v);
 Set(v,offset,e.component);
 if(selection!=-1){
  int selected=Get(selection);
  if(target.id==14){selected+=delta*2;if(selected<0){selected=0;}}
  else{selected+=delta;if(selected<0){selected=0;}if(selected>=count){selected=count-1;}}
  Set(selection,selected,e.component);
 }
}}
}