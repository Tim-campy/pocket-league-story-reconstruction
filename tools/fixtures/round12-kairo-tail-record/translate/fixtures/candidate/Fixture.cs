using System;public class Transform{float[] mtx_;public void Set(float[] values){mtx_=values;}}
public class Fixture{static bool _checkArgumentException(float x,float y,float z){return false;}void Multiply(Transform other){}public void Run(float x,float y,float z){if(_checkArgumentException(x,y,z)){throw new ArgumentException();}
Transform transform = new Transform();
transform.Set(new float[16] { 1f, 0f, 0f, x, 0f, 1f, 0f, y, 0f, 0f, 1f, z, 0f, 0f, 0f, 1f });Multiply(transform);}}