using System;public class Transform{float[] mtx_;public void Set(float[] values){mtx_=values;}}
public class Fixture{static bool _checkArgumentException(float x,float y,float z){return false;}void Multiply(Transform other){}public void Run(float x,float y,float z){if(_checkArgumentException(x,y,z)){throw new ArgumentException();}
Transform transform = new Transform();
float[] obj = new float[16]{1f,0f,0f,0f,0f,1f,0f,0f,0f,0f,1f,0f,0f,0f,0f,1f};
obj[3] = x;obj[7] = y;obj[11] = z;transform.Set(obj);Multiply(transform);}}