public class Node { public Node left_,right_,parent_; public int value_; }
public class Fixture {
    static int cursor; static int _readBit(){return cursor++&1;} static int _readByte(){return cursor++;}
    static Node Child(int limit){return new Node{value_=limit};}
    public static Node Run(int limit) {
        Node node=new Node();int num=0;
        num=_readBit();
        if(num==1){node.value_=_readByte();}
        else{
            if(num==0){node.left_=Child(limit);node.left_.parent_=node;}
            num=_readBit();
            if(num==0){node.right_=Child(limit);node.right_.parent_=node;}
        }
        return node;
    }
}
