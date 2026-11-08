
public class Fixture {
 int transparentColor_,alphaType_;bool format;int[] pixels=new int[8];
 int[] Read(){return pixels;}void Write(int[] a){pixels=a;}
 public void Run(bool enabled){
  int num=transparentColor_&0xFFFFFF;int[] aRGBPixels=Read();
  if(enabled){bool flag=false;if(!format){Write(new int[8]);flag=true;}
   for(int i=0;i<aRGBPixels.Length;i++){
    if((aRGBPixels[i]&0xFFFFFF)==num){aRGBPixels[i]=aRGBPixels[i]&16777215;}
    else if(flag){aRGBPixels[i]=-16777216|aRGBPixels[i];}
   }
   Write(aRGBPixels);if(alphaType_==0){alphaType_=1;}
  }else{if(format){Write(new int[8]);}Write(aRGBPixels);alphaType_=0;}
 }
}
