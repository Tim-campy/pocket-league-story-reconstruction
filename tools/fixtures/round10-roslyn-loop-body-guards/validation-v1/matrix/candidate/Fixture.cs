using System;
class Matrix { public void Reset(){} public void SetConcat(Matrix a,Matrix b){} }
class Fixture {
 int localMatrixIndex_; Matrix[][] localMatrixStack_=new Matrix[1][];
 void SetMatrix(Matrix matrix){}
 public void Run(Matrix matrix=null) {
  localMatrixIndex_++;
  if(localMatrixIndex_>=localMatrixStack_.Length) {
   Matrix[][] array=new Matrix[localMatrixIndex_+1][];
   Array.Copy(localMatrixStack_,0,array,0,localMatrixStack_.Length);
   int num=array.Length-1;
   while (num >= 0) {
if ((object)array[num] != null) { break; }
 array[num]=new Matrix[2]{new Matrix(),new Matrix()};num--; }
   localMatrixStack_=array;
  }
  if(localMatrixIndex_-1<0) { localMatrixStack_[localMatrixIndex_][1].Reset(); }
  else {localMatrixStack_[localMatrixIndex_][1].SetConcat(localMatrixStack_[localMatrixIndex_-1][0],localMatrixStack_[localMatrixIndex_-1][1]);}
  SetMatrix(matrix);
 }
}
