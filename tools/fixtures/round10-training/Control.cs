// Independent historical-compiler fixture.
public class Fixture
{
    public static int[] WIN_CHA = new int[3];
    public static class GameUtil
    {
        public static float SetVStop(float v, int n) { return v + n; }
        public static float SetAStop(float v, int n) { return v - n; }
    }
    public void Run(int time, int tMode)
    {
        if (tMode == 2)
        {
            int num = 3;
            int num2 = time + num;
            float num3 = GameUtil.SetVStop(1f, 2);
            float num4 = GameUtil.SetAStop(1f, 2);
            WIN_CHA[1] = (int)(num3 + num4) + num2;
        }
        else
        {
            WIN_CHA[1] = 0;
            int num5 = 0;
            if (tMode == 0)
            {
                WIN_CHA[0] = 1;
                num5 = time % 7;
                if (num5 < 2) { WIN_CHA[2] = 0; }
                else { WIN_CHA[2] = 1; }
            }
            else if (tMode == 1)
            {
                WIN_CHA[0] = 10;
                num5 = time % 9;
                WIN_CHA[2] = time / 3;
            }
            else if (tMode == 3)
            {
                WIN_CHA[0] = 4;
                num5 = time % 11;
                WIN_CHA[2] = time / 5;
            }
        }
    }
}
