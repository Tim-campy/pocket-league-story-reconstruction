// PROPOSED FIXTURE ONLY. Not compiled, run, modeled or verified by its author.
// Select Current for transform_drawimage_composition. The early local references
// and protected returns are intended to expose the late shared lookup scope.
public sealed class FixtureDrawImage
{
    public sealed class Image
    {
        public int optimizePass_;
        public int[] optimizeSeb_;
        public sbyte[] optimizeAccessL_;
        public byte[] optimizeAccess_;
        public int[] Data;
        public void GetOptimizeSeb(int index) { }
        public int[] GetOptimize(int index) { return Data; }
    }

    public int Finished;
    public float Observed;
    private void Pin(ref int a, ref int b, ref Image image, float x, float y)
    {
        Observed = a + b + x + y;
    }
    private void Use(int[] data, float x, float y) { Observed = data[0] + x + y; }
    private void DefaultDraw(Image image, float width, float height) { Observed = width + height; }

    public void Current(Image image, float width, float height, float swidth, float sheight)
    {
        try
        {
            if (image.optimizePass_ != 0)
            {
                float num17 = width / swidth;
                float num18 = height / sheight;
                if (image.optimizeSeb_ == null)
                {
                    int num19 = 1;
                    int num20 = 2;
                    Image image2 = image;
                    Pin(ref num19, ref num20, ref image2, num17, num18);
                    return;
                }
                int num25 = -1;
                int num26 = image.optimizeSeb_.Length / 4;
                for (int j = 0; j < num26; j++)
                {
                    int num27 = j * 4;
                    if (image.optimizeSeb_[num27] == 0)
                    {
                        num25 = j;
                        break;
                    }
                }
                if (num25 == -1)
                {
                    return;
                }
                image.GetOptimizeSeb(num25);
                for (int k = 0; k < image.optimizePass_; k++)
                {
                    int num28 = num25 * image.optimizePass_ + k;
                    int[] array = null;
                    array = ((image.optimizeAccess_ == null) ? image.GetOptimize(image.optimizeAccessL_[num28]) : image.GetOptimize(image.optimizeAccess_[num28]));
                    if (array == null) { continue; }
                    Use(array, num17, num18);
                }
            }
            else
            {
                DefaultDraw(image, width, height);
            }
        }
        finally
        {
            Finished++;
        }
    }

    public void Expected(Image image, float width, float height, float swidth, float sheight)
    {
        try
        {
            if (image.optimizePass_ != 0)
            {
                float num17 = (float)width / swidth;
                float num18 = (float)height / sheight;
                if (image.optimizeSeb_ == null)
                {
                    int num19 = 1;
                    int num20 = 2;
                    Image image2 = image;
                    Pin(ref num19, ref num20, ref image2, num17, num18);
                    return;
                }
                {
                    int num25 = -1;
                    int num26 = image.optimizeSeb_.Length / 4;
                    for (int j = 0; j < num26; j++)
                    {
                        int num27 = j * 4;
                        if (image.optimizeSeb_[num27] == 0)
                        {
                            num25 = j;
                            break;
                        }
                    }
                    if (num25 != -1)
                    {
                        image.GetOptimizeSeb(num25);
                        for (int k = 0; k < image.optimizePass_; k++)
                        {
                            int num28 = num25 * image.optimizePass_ + k;
                            int[] array = null;
                            if (image.optimizeAccess_ == null) { array = image.GetOptimize(image.optimizeAccessL_[num28]); }
                            else { array = image.GetOptimize(image.optimizeAccess_[num28]); }
                            if (array == null) { continue; }
                            Use(array, num17, num18);
                        }
                    }
                }
            }
            else
            {
                DefaultDraw(image, width, height);
            }
        }
        finally
        {
            Finished++;
        }
    }
}
