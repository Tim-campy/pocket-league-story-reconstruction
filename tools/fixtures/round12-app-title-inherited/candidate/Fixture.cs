using System;public class Component{public int id,value;}public class Event{public Component component;public int type;}
public class Fixture{int state,sink;bool Allow(Event e){return e!=null;}void Use(int n){sink=n;}void Log(Exception e){sink=1;}
public bool Run(Event e){if(!Allow(e)){return false;}Component component=e.component;if(state==0){switch (component.id)
{
case 27:
if (e.type == 1)
{if(component.value==0){try{Use(1);}catch(Exception a){Log(a);}}else if(component.value==1){try{Use(2);}catch(Exception b){Log(b);}}return false;
}
break;
}}return true;}}
