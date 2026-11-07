// NOT_TESTED. Compile separately from FixtureStaticManual.cs.
public class Fixture
{
    public static bool Debug = false;
    public static bool Ptw = false;
    public static bool USE_UNUSE_CHECK = Debug && !Ptw;
    public static bool DEBUG_CMD = Debug ? true : false;
    public static bool SHOW_DEBUG = Debug && !Ptw;
    public static bool SHOW_FPS = Debug && !Ptw;
    public static bool SHOW_MEM = Debug && false;
}
