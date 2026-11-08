using System;
public static class StringEx{public static int Length(this string s){return s.Length;}public static char CharAt(this string s,int index){return s[index];}}
public class BillingItem{public int Mask,Id;public void Init(int id,int mask){Id=id;Mask=mask;}}
public class Fixture{public int Trace,Count,LastMask;
public virtual void Run<T>(string[][] data,string paymentProduct,params string[] subscription) where T:BillingItem,new(){
 for(int spriteIndex=0;spriteIndex<data.Length;spriteIndex++){
  string[] array=data[spriteIndex];int id=int.Parse(array[0]);int num=0;
  for(int j=0;j<array[1].Length();j++){char c=array[1].CharAt(j);if(c=='1'){num |= 1 << (j & 0x1F);}}
  BillingItem item=new T();item.Init(id,num);LastMask=item.Mask;Trace+=item.Id;Count++;
 }
}
public static int SemanticChecks(){Fixture f=new Fixture();foreach(int length in new int[]{0,1,15,16,17,31,32,33,63,64,65,97})for(int pattern=0;pattern<3;pattern++){
 char[] bits=new char[length];int expected=0;for(int i=0;i<length;i++){bool on=pattern==0||pattern==1&&i%3==1||pattern==2&&i==length-1;bits[i]=on?'1':'0';if(on){int bit=i%32;expected=expected|(1<<bit);}}
 string[][] data=new string[][]{new string[]{"2",new string(bits)},new string[]{"3","01"}};f.Trace=0;f.Count=0;f.LastMask=0;f.Run<BillingItem>(new string[][]{data[0]},"x",new string[0]);if(f.Trace!=2||f.Count!=1||f.LastMask!=expected)return 1;
 f.Trace=0;f.Count=0;f.Run<BillingItem>(data,"x",new string[0]);if(f.Trace!=5||f.Count!=2||f.LastMask!=2)return 2;
 }return 0;}
public static float FloatNeg(float v){return -v;}public static float FloatZeroSub(float v){return 0f-v;}
}