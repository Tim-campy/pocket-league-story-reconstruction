using System;
using System.Collections.Generic;
public class Image {}
public class Material {
 public string key_;
 public int NotAccessFrame;
 public int RenderQueueOffset {get;set;}
 public int RenderQueue {get;set;}
 public bool UntiColor {get;set;}
 public static int Creates;
 public Material(object source,Image image){Creates++;key_="copy";}
 public object GetMaterial(){return this;}
}
public class MeshBuilder {
 public static int Creates;
 public MeshBuilder(CacheFixture owner,bool mode){Creates++;}
}
public class CacheFixture {
 Image empty_=new Image();
 Dictionary<Image,Dictionary<string,Material>> materlas_=new Dictionary<Image,Dictionary<string,Material>>();
 Dictionary<Material,MeshBuilder> meshList_=new Dictionary<Material,MeshBuilder>();
 List<KeyValuePair<Material,MeshBuilder>> sortedMeshList_=new List<KeyValuePair<Material,MeshBuilder>>();
 public Material RunMaterial(Material material,Image image){
  Image key=((image!=null)?image:empty_);
  Dictionary<string,Material> value;
  if(!materlas_.TryGetValue(key,out value)){
   materlas_[key] = (value = new Dictionary<string, Material>());
  }
  if(value.ContainsKey(material.key_)){material.NotAccessFrame=0;return material;}
  Material material2=(value[material.key_]=new Material(material.GetMaterial(),image));
  material2.RenderQueueOffset=material.RenderQueueOffset;
  return material2;
 }
 public MeshBuilder RunMesh(Material material){
  MeshBuilder value;
  if(!meshList_.TryGetValue(material,out value)){
   meshList_[material] = (value = new MeshBuilder(this, !material.UntiColor));
   int renderQueue=material.RenderQueue;
   for(int i=0;i<sortedMeshList_.Count+1;i++){
    if(i==sortedMeshList_.Count||renderQueue<=sortedMeshList_[i].Key.RenderQueue){
     sortedMeshList_.Insert(i,new KeyValuePair<Material,MeshBuilder>(material,value));break;
    }
   }
  }
  return value;
 }
 public int Count {get{return sortedMeshList_.Count;}}
 public int QueueAt(int i){return sortedMeshList_[i].Key.RenderQueue;}
}
public static class Fixture {
 public static int SemanticChecks(){
  CacheFixture cache=new CacheFixture();Material a=new Material(null,null);a.key_="a";a.RenderQueueOffset=7;a.RenderQueue=20;a.UntiColor=true;
  Image image=new Image();int before=Material.Creates;
  Material copy=cache.RunMaterial(a,image);if(object.ReferenceEquals(copy,a)||copy.RenderQueueOffset!=7||Material.Creates!=before+1)return 1;
  a.NotAccessFrame=17;Material hit=cache.RunMaterial(a,image);if(!object.ReferenceEquals(hit,a)||a.NotAccessFrame!=0||Material.Creates!=before+1)return 1;
  Material nullCopy=cache.RunMaterial(a,null);if(object.ReferenceEquals(nullCopy,a)||Material.Creates!=before+2)return 1;
  MeshBuilder.Creates=0;MeshBuilder x=cache.RunMesh(a);if(!object.ReferenceEquals(x,cache.RunMesh(a))||MeshBuilder.Creates!=1||cache.Count!=1)return 2;
  Material b=new Material(null,null);b.key_="b";b.RenderQueue=10;cache.RunMesh(b);
  Material c=new Material(null,null);c.key_="c";c.RenderQueue=30;cache.RunMesh(c);
  if(MeshBuilder.Creates!=3||cache.Count!=3||cache.QueueAt(0)!=10||cache.QueueAt(1)!=20||cache.QueueAt(2)!=30)return 2;
  return 0;
 }
 public static float FloatNeg(float x){return -x;}
 public static float FloatZeroSub(float x){return 0f-x;}
}
