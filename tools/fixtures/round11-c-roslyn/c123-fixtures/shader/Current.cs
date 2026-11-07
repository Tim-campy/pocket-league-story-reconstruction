public struct Color{public float r,g,b;}
public class Fixture{
 bool untiColor=true;int alphaType;
 public void Run(object g,object img,Color[] colors,float alpha,bool clipRegion,int opaTransOrder){
 if(untiColor && colors!=null){
 for(int i=0;i<colors.Length;i++){Color color=colors[i];if(color.r<1f || color.g<1f || color.b<1f){untiColor=false;break;}}
 }
 alphaType=-1;
 switch(opaTransOrder){case 1:alphaType=1;break;case 2:alphaType=2;break;}
 }
 public void ForeachControl(object g,object img,Color[] colors,float alpha,bool clipRegion,int opaTransOrder){
 if(untiColor && colors!=null){foreach(Color color in colors){if(color.r<1f || color.g<1f || color.b<1f){untiColor=false;break;}}}
 alphaType=-1;if(opaTransOrder==1){alphaType=1;}else if(opaTransOrder==2){alphaType=2;}
 }
}
