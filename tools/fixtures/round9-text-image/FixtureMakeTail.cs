// PROPOSED FIXTURE ONLY. Not compiled, run, modeled or verified by its author.
// Select Current as method_source for transform_make_alignment_tail.
// Expected is an independently written control in the SAME declaring type so
// shared fields/helper member references need not differ during IL comparison.
public static class FixtureMakeTail
{
    public static float height_;
    public static int SideEffects;

    public static void Pin(ref float value) { }

    public static float Current(int num2, float input)
    {
        float num4 = input;
        Pin(ref num4);
        if ((num2 == 32)) { num4 = ((height_ != -1f) ? ((height_ - num4) / 2f) : ((-(num4)) / 2f));
        } else if ((num2 == 64)) { num4 = ((height_ != -1f) ? (height_ - num4) : (0f - num4));
        } else { num4 = 0f;
        }
        if (height_ != -1f && num4 < 0f)
        {
            num4 = 0f;
        }
        Pin(ref num4);
        return num4;
    }

    public static float Expected(int num2, float input)
    {
        float num4 = input;
        Pin(ref num4);
        if (num2 == 32)
        {
            if (height_ == -1f) { num4 = -num4 / 2f; }
            else { num4 = (height_ - num4) / 2f; }
        }
        else if (num2 == 64)
        {
            if (height_ == -1f) { num4 = -num4; }
            else { num4 = height_ - num4; }
        }
        else { num4 = 0f; }
        if (height_ != -1f && num4 < 0f)
        {
            num4 = 0f;
        }
        Pin(ref num4);
        return num4;
    }

    // Expected TRANSFORM GUARD rejection after the executor adds full old-tail
    // token-template equality. V1's narrow expression/count guard has a known
    // gap: this field write has no call/string/num4 token and could be dropped.
    // Do not claim this negative passed; it has not been executed at all.
    public static float NegativeExtraFieldWrite(int num2, float input)
    {
        float num4 = input;
        Pin(ref num4);
        if ((num2 == 32)) { num4 = ((height_ != -1f) ? ((height_ - num4) / 2f) : ((-(num4)) / 2f));
            SideEffects++;
        } else if ((num2 == 64)) { num4 = ((height_ != -1f) ? (height_ - num4) : (0f - num4));
        } else { num4 = 0f;
        }
        if (height_ != -1f && num4 < 0f)
        {
            num4 = 0f;
        }
        Pin(ref num4);
        return num4;
    }
}
