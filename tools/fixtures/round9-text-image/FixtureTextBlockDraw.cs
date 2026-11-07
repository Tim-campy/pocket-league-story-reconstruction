// PROPOSED FIXTURE ONLY. Not compiled, run, modeled or verified by its author.
// Select Current for transform_textblock_draw_composition; select its components
// independently only if the executor separately supplies appropriate controls.
public sealed class FixtureTextBlockDraw
{
    public sealed class Pictograph
    {
        public int Frame;
    }

    public bool isPictograph_;
    public string[] pictographParam_ = new string[] { "demo", "∞" };
    public string text_ = "demo";
    public float x_;
    public float y_;
    public Pictograph Item = new Pictograph();
    public int Observed;

    private Pictograph GetPictograph(string name) { return Item; }
    private void Pin(ref float x, ref float y) { }
    private void Consume(Pictograph item, int frame, float x, float y)
    {
        Observed = item.Frame + frame + (int)x + (int)y;
    }

    public int Current(int len)
    {
        float num = x_;
        float num2 = y_;
        Pin(ref num, ref num2);
        try
        {
            if (isPictograph_)
            {
                int result = 1;
                if (len == 0) { return 0; }
                Pictograph pictograph = GetPictograph(pictographParam_[0]);
                if (pictograph == null) { return result; }
                int num3 = pictograph.Frame;
                if (1 < pictographParam_.Length && pictographParam_[1].Length > 0)
                {
                    num3 = ((!(pictographParam_[1] == "∞")) ? int.Parse(pictographParam_[1]) : (-1));
                }
                Consume(pictograph, num3, num, num2);
                return result;
            }
            int num11 = len;
            if (num11 > text_.Length) { num11 = text_.Length; }
            if (len == 0) { return num11; }
            Pin(ref num, ref num2);
            return num11;
        }
        finally
        {
            Pin(ref num, ref num2);
        }
    }

    public int Expected(int len)
    {
        float num = x_;
        float num2 = y_;
        Pin(ref num, ref num2);
        try
        {
            if (isPictograph_)
            {
                int result = 1;
                if (len == 0) { return 0; }
                Pictograph pictograph = GetPictograph(pictographParam_[0]);
                if (pictograph == null) { return result; }
                int num3 = pictograph.Frame;
                if (1 < pictographParam_.Length && pictographParam_[1].Length > 0)
                {
                    if (pictographParam_[1] == "∞") { num3 = -1; }
                    else { num3 = int.Parse(pictographParam_[1]); }
                }
                Consume(pictograph, num3, num, num2);
                return result;
            }
            {
                int num11 = len;
                if (num11 > text_.Length) { num11 = text_.Length; }
                if (len == 0) { return num11; }
                Pin(ref num, ref num2);
                return num11;
            }
        }
        finally
        {
            Pin(ref num, ref num2);
        }
    }
}
