using kairo.unity.surface;
namespace kairo.unity.surface{
public class TouchOption{public int[] args;}
public class TouchComponent{public TouchOption option;public int id;}
public class TouchEvent{public TouchComponent component;public int dy;}
}
public class Fixture{public int Get(int key){return key;}public void Set(int key,int value,TouchComponent component){}
public void Run(int type,int v,TouchEvent e){TouchOption options=e.component.option;
int count=options.args[0];
int page=options.args[1];
int selection=(2<options.args.Length)?options.args[2]:-1;
if(selection==-1){return;}
int direction=v&0xFFFF;
switch(type){
case 13:
 if(direction==0&&Get(selection)>0){Set(selection,Get(selection)-1,e.component);}
 if(direction==1&&Get(selection)<count-1){Set(selection,Get(selection)+1,e.component);}
 if(Get(selection)>=Get(v)+page){Set(v,Get(selection)-(page-1),e.component);}
 if(Get(selection)<Get(v)){Set(v,Get(selection),e.component);}
 while(Get(v)>0&&count<Get(v)+page){Set(v,Get(v)-1,e.component);}
 break;
}}
}