using System;
public class Fixture{public int gameObjectPoolIndex_,zsortObjectStart_=4,zsortObjectLast_=-1;public int[][] gameObjectPool_=new int[2][];public byte[] zsortObjectCount_=new byte[4];public int[][] zsortObjects_=new int[4][];public Fixture(){for(int i=0;i<2;i++)gameObjectPool_[i]=new int[10];for(int i=0;i<4;i++)zsortObjects_[i]=new int[30];}
public void Run(int type,int id,int depth,int img,int seb,int frame,int x,int y,bool dark,int layer){if(gameObjectPoolIndex_>=gameObjectPool_.Length){return;}int num;while(true){num = depth + 360;
if (num < 0 || zsortObjectCount_.Length <= num) {return;}
if (zsortObjectCount_[num] >= 30) {depth++; continue;}
zsortObjects_[num][zsortObjectCount_[num]]=gameObjectPoolIndex_;zsortObjectCount_[num]++;if(zsortObjectStart_>num){zsortObjectStart_=num;}if(zsortObjectLast_<num){zsortObjectLast_=num;}int[] array=gameObjectPool_[gameObjectPoolIndex_];array[0]=type;array[1]=id;array[2]=depth;array[3]=img;array[4]=seb;array[5]=frame;array[6]=x;array[7]=y;array[8]=(dark?1:0);array[9]=layer;gameObjectPoolIndex_++;
break;
}}
public static int SemanticChecks(){byte[][] patterns=new byte[][]{new byte[]{0,0,0,0},new byte[]{30,0,0,0},new byte[]{30,30,30,30},new byte[]{29,30,30,0},new byte[]{255,30,29,0}};foreach(byte[] pattern in patterns)for(int pool=0;pool<4;pool++)foreach(int depth in new int[]{-361,-360,-359,-358,-357,-356})for(int dark=0;dark<2;dark++){
 Fixture f=new Fixture();f.gameObjectPoolIndex_=pool;Array.Copy(pattern,f.zsortObjectCount_,4);f.Run(12,99,depth,7,8,9,10,-11,dark!=0,13);int adjusted=depth;int bucket=adjusted+360;if(pool<2){while(bucket>=0&&bucket<4&&pattern[bucket]>=30){adjusted++;bucket=adjusted+360;}}
 bool valid=pool<2&&bucket>=0&&bucket<4;if(f.gameObjectPoolIndex_!=(valid?pool+1:pool))return 1;if(f.zsortObjectStart_!=(valid?bucket:4)||f.zsortObjectLast_!=(valid?bucket:-1))return 2;
 for(int i=0;i<4;i++)if(f.zsortObjectCount_[i]!=(valid&&i==bucket?pattern[i]+1:pattern[i]))return 3;
 if(valid){if(f.zsortObjects_[bucket][pattern[bucket]]!=pool)return 4;int[] want=new int[]{12,99,adjusted,7,8,9,10,-11,dark!=0?1:0,13};for(int i=0;i<10;i++)if(f.gameObjectPool_[pool][i]!=want[i])return 5;}
}return 0;}
public static float FloatNeg(float v){return -v;}public static float FloatZeroSub(float v){return 0f-v;}
}