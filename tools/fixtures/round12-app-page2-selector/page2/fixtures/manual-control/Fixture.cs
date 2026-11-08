public class TouchComponent { public int paintX,paintY;public bool Check(int f){return f!=0;}public int GetKey(){return paintX;} }
public class TouchEvent { public TouchComponent component; }
public class TouchEffect { public float vx_,vy_; }
public class Canvas { public void KeyDown(int key,bool record=true){} public void KeyUp(int key){} public void KeyClick(int key){} }
public class Fixture {public Canvas canvas_;public int markComponentKey_;
TouchEffect AddEffect(int a,int b,int c,int d,TouchComponent e){return new TouchEffect();}
int GetKey(TouchComponent c,int key){return key;}void Set(int v,TouchComponent c){}
public void Run(int type,int v,TouchEvent e){TouchComponent comp=e.component;
TouchEffect active;
switch(type){
case 5:
 canvas_.KeyClick(1<<v);
 if(v==16){active=AddEffect(0,comp.paintX,comp.paintY,2,comp);active.vx_=-10f;}
 else if(v==18){active=AddEffect(0,comp.paintX,comp.paintY,3,comp);active.vx_=10f;}
 else if(v==17){active=AddEffect(0,comp.paintX,comp.paintY,14,comp);active.vy_=-10f;}
 else if(v==19){active=AddEffect(0,comp.paintX,comp.paintY,15,comp);active.vy_=10f;}
 break;
case 1:case 2:
 Set(v,e.component);
 if(type==1){if(comp.Check(16)&&markComponentKey_!=comp.GetKey()){markComponentKey_=e.component.GetKey();}else{canvas_.KeyClick(1048576);}}
 break;
case 6:case 10:canvas_.KeyUp(GetKey(e.component,1<<v));break;
}}
}