// NOT_TESTED: private toy source, not game code. Compile separately from Manual.
public class Fixture
{
    public static bool Debug;
    public static bool Ptw;
    public static bool And() { return Debug && !Ptw; }
    public static bool Flag() { return Debug ? true : false; }
    public static bool Masked() { return Debug && false; }
}
