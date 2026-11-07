// NOT_TESTED: private manually authored expected lowering; no compiler invoked.
// Compile separately with the same Fixture name to avoid signature drift.
public class Fixture
{
    public static bool Debug;
    public static bool Ptw;
    public static bool And() { return (!Debug) ? false : !Ptw; }
    public static bool Flag() { return (!Debug) ? false : true; }
    public static bool Masked() { return (!Debug) ? false : false; }
}
