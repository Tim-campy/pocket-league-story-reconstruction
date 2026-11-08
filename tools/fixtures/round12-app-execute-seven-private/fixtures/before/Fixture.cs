using System;
public class SubForm { public int humanCharaType_=-8;public int humanCharaIndex_=-1; }
public class DelayEvent {public int hour_=-91;public int id,position,value,minus,save;public string args;public int[] argi; public DelayEvent(int a,int b,int c,int d,int e,string f,int[] g){id=a;position=b;value=c;minus=d;save=e;args=f;argi=g;}}
public class Property {public int[] ints=new int[10];}
public class Vector {public object last;public int count;public virtual void AddElement(object a){last=a;count++;}}
public class Fixture {
public Property save_=new Property();public Vector delayEvents_=new Vector();public int calls;
public int GetDelayId(int[][] script){return script.Length+100;}
public SubForm ExecuteScript(int[] scr,bool popup,string args,int[] argi,int[][] script){calls++;if(scr[0]==0)return null;SubForm result=new SubForm();if(scr[0]==2)result.humanCharaIndex_=42;return result;}
public SubForm Run(int[][] script, bool popup, string args, int[] argi, int line, int talkCharaType, int talkCharaIndex)
		{
			if (script == null)
			{
				return null;
			}
			SubForm subForm = null;
			DelayEvent delayEvent = null;
			for (int i = line; i < script.Length; i++)
			{
				int[] array = script[i];
				int num = array[0];
				int num2 = num;
				if (num2 == 5)
				{
					delayEvent = new DelayEvent(GetDelayId(script), i + 1, array[1], -1, save_.ints[9], args, argi);
					if (2 < array.Length)
					{
						delayEvent.hour_ = array[2];
					}
					delayEvents_.AddElement(delayEvent);
					return subForm;
				}
				subForm = ExecuteScript(array, popup, args, argi, script);
				if ((talkCharaIndex == -1) || (subForm == null)) { continue; }
if (subForm.humanCharaIndex_ != -1) { continue; }
subForm.humanCharaType_ = talkCharaType;
subForm.humanCharaIndex_ = talkCharaIndex;
			}
			return subForm;
		}
public static int SemanticChecks(){
int failures=0;
int[][][] cases=new int[][][]{null,new int[0][],new int[][]{new int[]{0,1}},new int[][]{new int[]{1,9},new int[]{2,8}},new int[][]{new int[]{1,3},new int[]{5,77,44},new int[]{9,4}},new int[][]{new int[]{5,51},new int[]{1,5}}};
for(int ci=0;ci<cases.Length;ci++)for(int line=0;line<3;line++)for(int ti=0;ti<3;ti++)for(int p=0;p<2;p++){
int talkIndex=ti==0?-1:ti==1?7:22;int talkType=18+p;int[] input=new int[]{91,92};string text="own fixture";Fixture f=new Fixture();f.save_.ints[9]=27;SubForm got=f.Run(cases[ci],p==1,text,input,line,talkType,talkIndex);
int expectedCalls=0,expectedType=-8,expectedIndex=-1,expectedResultKind=0;int delay=-1;
if(cases[ci]!=null){for(int z=line;z<cases[ci].Length;z++){
int code=cases[ci][z][0];if(code==5){delay=z;break;}
expectedCalls++;expectedResultKind=code==0?0:1;expectedType=-8;expectedIndex=code==2?42:-1;
if(expectedResultKind==1&&talkIndex!=-1&&expectedIndex==-1){expectedType=talkType;expectedIndex=talkIndex;}
}}
if(f.calls!=expectedCalls)failures++;
if((got==null)!=(expectedResultKind==0))failures++;
if(got!=null&&(got.humanCharaType_!=expectedType||got.humanCharaIndex_!=expectedIndex))failures++;
if(f.delayEvents_.count!=(delay<0?0:1))failures++;
if(delay>=0 && f.delayEvents_.last is DelayEvent){DelayEvent de=(DelayEvent)f.delayEvents_.last;int[] item=cases[ci][delay];if(de.id!=cases[ci].Length+100||de.position!=delay+1||de.value!=item[1]||de.minus!=-1||de.save!=27||de.args!=text||!object.ReferenceEquals(de.argi,input)||de.hour_!=(item.Length>2?item[2]:-91))failures++;}
}
return failures;
}
public static float FloatNeg(float v){return -v;}public static float FloatZeroSub(float v){return 0f-v;}
}