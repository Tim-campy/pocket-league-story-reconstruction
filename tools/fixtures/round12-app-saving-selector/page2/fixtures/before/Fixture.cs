public class TouchComponent { public int paintX,paintY;public bool Check(int f){return f!=0;}public int GetKey(){return paintX;} }
public class TouchEvent { public TouchComponent component; }
public class TouchEffect { public float vx_,vy_; }
public class Canvas { public void KeyDown(int key,bool record=true){} public void KeyUp(int key){} public void KeyClick(int key){} }
public class Fixture {public Canvas canvas_;public int markComponentKey_;
TouchEffect AddEffect(int a,int b,int c,int d,TouchComponent e){return new TouchEffect();}
int GetKey(TouchComponent c,int key){return key;}void Set(int v,TouchComponent c){}
public void Run(int type,int v,TouchEvent e){
			TouchComponent component = e.component;
			switch (type)
			{
			case 5:
				canvas_.KeyClick(1 << v);
				switch (v)
				{
				case 16:
				{
					TouchEffect touchEffect = AddEffect(0, component.paintX, component.paintY, 2, component);
					touchEffect.vx_ = -10f;
					break;
				}
				case 18:
				{
					TouchEffect touchEffect = AddEffect(0, component.paintX, component.paintY, 3, component);
					touchEffect.vx_ = 10f;
					break;
				}
				case 17:
				{
					TouchEffect touchEffect = AddEffect(0, component.paintX, component.paintY, 14, component);
					touchEffect.vy_ = -10f;
					break;
				}
				case 19:
				{
					TouchEffect touchEffect = AddEffect(0, component.paintX, component.paintY, 15, component);
					touchEffect.vy_ = 10f;
					break;
				}
				}
				break;
			case 1:
			case 2:
				Set(v, e.component);
				if (type == 1)
				{
					if (component.Check(16) && markComponentKey_ != component.GetKey())
					{
						markComponentKey_ = e.component.GetKey();
					}
					else
					{
						canvas_.KeyClick(1048576);
					}
				}
				break;
			case 6:
			case 10:
				canvas_.KeyUp(GetKey(e.component, 1 << v));
				break;
			case 3:
			case 4:
			case 7:
			case 8:
			case 9:
				break;
			}
		}
}