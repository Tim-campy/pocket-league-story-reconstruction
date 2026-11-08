public class Image{}
public class Graphics{public void SetRenderMode(int a,int b,int c){}public void DrawImage(Image i,float x,float y,int sx,int sy,int width,int height){}}
public class Seb{public int[] GetSprite(int a,int b){return null;}}
public class Res{public Seb[] seb;public Image[] img;}
public class Property{public int[] ints;}
public static class Graph{public static int Easing(int a,int b,int c,int d,int e){return a;}}
public class Fixture{public Res resCom2_;public Property sysSave_;
public void DrawSeb(Graphics g,int x,int y,Seb seb,Image img,int id){}
public void Run(Graphics g,int trophy_id,int subform_frame,int px,int py){
			DrawSeb(g, px, py, resCom2_.seb[106], resCom2_.img[190], sysSave_.ints[10] - 1);
			int trophyOriginX = px;
int trophyOriginY = py;
int num = (subform_frame - 20) % 66;
			int[] array = resCom2_.seb[106].GetSprite(trophy_id, 0);
			int num2 = array[5];
			int[][] array2 = new int[2][]
			{
				new int[4] { 1, 4, 1, 128 },
				new int[4] { 1, 2, 5, 255 }
			};
			int num3 = 4;
			int num4 = num - 2;
			for (int i = 0; i < array2.Length; i++)
			{
				if (i == 1)
				{
					continue;
				}
				int[] array3 = array2[i];
				int num5 = num4 * num3;
				int num6 = 0;
				if (num5 <= array[4] / 2)
				{
					num6 = Graph.Easing(array3[0], array3[1], array[4] / 2, num5, 0);
				}
				if (num5 >= array[4] / 2)
				{
					num6 = Graph.Easing(array3[1], array3[0], array[4] / 2, num5 - array[4] / 2, 0);
				}
				int num7 = trophyOriginX + array[6] + num5 - num6 / 2;
				int num8 = trophyOriginY + array[7];
				int num9 = array[2] + num5 - num6 / 2;
				int sy = array[3];
				int num10 = num6;
				int height = num2;
				if (num9 < array[2])
				{
					int num11 = array[2] - num9;
					num9 += num11;
					num10 -= num11;
					num7 += num11;
				}
				if (num9 + num10 > array[2] + array[4])
				{
					num10 = array[2] + array[4] - num9;
				}
				if (num10 > 0)
				{
					g.SetRenderMode(1, array3[3], 255);
					for (int j = 0; j < array3[2]; j++)
					{
						g.DrawImage(resCom2_.img[array[1]], num7, num8, num9, sy, num10, height);
					}
					g.SetRenderMode(0, 255, 255);
				}
			}
		}
}