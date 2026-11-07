// NOT_TESTED. No explicit .cctor: all lowering comes from field initializers.
public class Fixture
{
    public static bool Debug = false;
    public static bool Ptw = false;
    public static bool USE_UNUSE_CHECK = (!Debug) ? false : !Ptw;
    public static bool DEBUG_CMD = (!Debug) ? false : true;
    public static bool SHOW_DEBUG = (!Debug) ? false : !Ptw;
    public static bool SHOW_FPS = (!Debug) ? false : !Ptw;
    public static bool SHOW_MEM = (!Debug) ? false : false;
}
