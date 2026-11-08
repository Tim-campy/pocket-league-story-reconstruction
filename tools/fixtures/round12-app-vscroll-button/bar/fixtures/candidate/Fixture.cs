using kairo.unity.surface;
namespace kairo.unity.surface{
public class TouchOption{public int[] args;}
public class TouchComponent{public TouchOption option;public int id;}
public class TouchEvent{public TouchComponent component;public int dy;}
}
public class Fixture{public int Get(int key){return key;}public void Set(int key,int value,TouchComponent component){}
public void Run(int type,int v,TouchEvent e){
			TouchComponent component = e.component;
			int[] args = e.component.option.args;
			int num = args[0];
			int num2 = args[1];
			int num3 = (2 < args.Length) ? args[2] : -1;
			int num4 = int.MaxValue;
			switch (type)
			{
			case 8:
				num4 = Get(v) + e.dy;
				break;
			case 9:
				num4 = e.dy;
				break;
			}
			if (num4 == int.MaxValue)
			{
				return;
			}
			if (num4 > num - num2)
			{
				num4 = num - num2;
			}
			if (num4 < 0)
			{
				num4 = 0;
			}
			if (Get(v) != num4) {

			int num5 = num4 - Get(v);
			Set(v, num4, e.component);
			if (num3 != -1) {

			int num6 = Get(num3);
			if (component.id == 14)
			{
				num6 += num5 * 2;
				if (num6 < 0)
				{
					num6 = 0;
				}
			}
			else
			{
				num6 += num5;
				if (num6 < 0)
				{
					num6 = 0;
				}
				if (num6 >= num)
				{
					num6 = num - 1;
				}
			}
			Set(num3, num6, e.component);

}
}}
}