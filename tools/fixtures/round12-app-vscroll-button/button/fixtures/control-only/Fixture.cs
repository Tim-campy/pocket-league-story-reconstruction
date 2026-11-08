using kairo.unity.surface;
namespace kairo.unity.surface{
public class TouchOption{public int[] args;}
public class TouchComponent{public TouchOption option;public int id;}
public class TouchEvent{public TouchComponent component;public int dy;}
}
public class Fixture{public int Get(int key){return key;}public void Set(int key,int value,TouchComponent component){}
public void Run(int type,int v,TouchEvent e){
			TouchOption option = e.component.option;
			int num = option.args[0];
			int num2 = option.args[1];
			int num3;
if (2 < option.args.Length) { num3 = option.args[2]; } else { num3 = (-1); }
			if (num3 == -1)
			{
				return;
			}
			int num4 = v & 0xFFFF;
			switch (type) {
case 13:

				if (num4 == 0 && Get(num3) > 0)
				{
					Set(num3, Get(num3) - 1, e.component);
				}
				if (num4 == 1 && Get(num3) < num - 1)
				{
					Set(num3, Get(num3) + 1, e.component);
				}
				if (Get(num3) >= Get(v) + num2)
				{
					Set(v, Get(num3) - (num2 - 1), e.component);
				}
				if (Get(num3) < Get(v))
				{
					Set(v, Get(num3), e.component);
				}
				while (Get(v) > 0 && num < Get(v) + num2)
				{
					Set(v, Get(v) - 1, e.component);
				}

break;
}
		}
}