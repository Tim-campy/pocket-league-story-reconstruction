using System;
using System.Collections.Generic;
public class Node {
    public Node parent_,left_,right_; public int weight_,value_;
    public Node(Node p,Node l,Node r,int w,int v){parent_=p;left_=l;right_=r;weight_=w;value_=v;}
}
public class FastVector {
    public List<Node> items=new List<Node>(); public int elementCount;
    public Node[] elementData=new Node[32];
    public void AddElement(Node n){items.Add(n);elementData[elementCount++]=n;}
    public void RemoveElement(Node n){items.Remove(n);elementCount--;}
}
public class Output {
    public int count; public void Close(){count=0;}
    public byte[] ToByteArray(){return new byte[count];}
}
public class Fixture {
    static int[] weights_=new int[8]; static Node[] nodeTbl_=new Node[8];
    static long Clock(){return 17;} static void Mark(long value){weights_[0]=(int)value;}
    static int[] Lowest(FastVector v){return new int[]{0,1};}
    static byte[] Bits(){return new byte[5];} static void Write(Output output,byte[] data){output.count+=data.Length;}
    public static byte[] Run(byte[] data) {
        if(data==null||data.Length==0)return data;
        long time=Clock();
        FastVector fastVector;
        Node node;
        for(int i=0;i<weights_.Length;i++){weights_[i]=0;}
        for(int j=0;j<nodeTbl_.Length;j++){nodeTbl_[j]=null;}
        for(int k=0;k<data.Length;k++){weights_[data[k]&7]++;}
        fastVector=new FastVector();
        for(int l=0;l<weights_.Length;l++){
            if(weights_[l]>0){node = new Node(null, null, null, weights_[l], l);fastVector.AddElement(node);nodeTbl_[l]=node;}
        }
        if(fastVector.elementCount==1){
            Node node2=fastVector.elementData[0];
            Node node3=(node2.parent_ = new Node(null, node2, node2, node2.weight_ + node2.weight_, 0));
            fastVector.elementData[0]=node3;
        }else{
            while(fastVector.elementCount>=2){
                int[] array=Lowest(fastVector);
                Node node4=fastVector.elementData[array[1]];
                Node node5=fastVector.elementData[array[0]];
                fastVector.AddElement(node5.parent_ = (node4.parent_ = new Node(null, node4, node5, node4.weight_ + node5.weight_, 0)));
                fastVector.RemoveElement(node4);fastVector.RemoveElement(node5);
            }
        }
        int counter=0;
        for(int m=0;m<data.Length;m++){counter+=data[m];}
        Mark(time+counter);
        byte[] array2=null;
        Output byteArrayOutputStream=null;
        try{
            byteArrayOutputStream=new Output();
            byte[] data2=Bits();Write(byteArrayOutputStream,data2);
            return byteArrayOutputStream.ToByteArray();
        }finally{byteArrayOutputStream?.Close();byteArrayOutputStream=null;}
    }
}
