using System;
public class Fixture {
    float[] mul_=new float[9];float[] mtx_=new float[9];int sink;
    void Observe(int value){sink+=value;}
    Fixture SetValues(float[] value){mtx_=(float[])value.Clone();return this;}
    public Fixture Run(float[] transform) {
        if(transform==null){throw new NullReferenceException();}
        for(int i=0;i<mul_.Length;i++){mul_[i]=0f;}
        for(int j=0;j<3;j++){
            for(int k=0;k<3;k++){
                Observe(j+k);mul_[j*3+k]=0f;
                for(int l=0;l<3;l++){
                    mul_[j*3+k]+=transform[j*3+l]*mtx_[l*3+k];
                }
            }
        }
        SetValues(mul_);return this;
    }
}
