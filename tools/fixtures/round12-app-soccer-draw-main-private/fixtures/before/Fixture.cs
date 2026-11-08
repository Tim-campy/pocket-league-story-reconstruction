using System;
public class Config {public static bool DEBUG_CMD;}
public class Fixture {public static int SHOW_DRAWNUM;public bool DrawBody=true;public int Trace,Calls;int DrawValue(){Calls++;return 4;}
public void Run(){int n=DrawValue();if(DrawBody){for(int i=0;i<2;i++){Trace+=n+i;}}
 if ((!(Config.DEBUG_CMD)) || (SHOW_DRAWNUM == 1)) {return;}
}
public static int SemanticChecks(){Fixture f=new Fixture();for(int debug=0;debug<2;debug++)for(int mode=-1;mode<=3;mode++)for(int body=0;body<2;body++){
 Config.DEBUG_CMD=debug!=0;SHOW_DRAWNUM=mode;f.DrawBody=body!=0;f.Trace=0;f.Calls=0;f.Run();if(f.Trace!=(body!=0?9:0)||f.Calls!=1)return 1;
}return 0;}
public static float FloatNeg(float v){return -v;}public static float FloatZeroSub(float v){return 0f-v;}
}