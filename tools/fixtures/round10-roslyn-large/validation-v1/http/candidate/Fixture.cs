using System;
using System.Collections.Generic;
public class HttpStub { public byte[] bytes; }
public class StreamStub { public byte[] bytes; }
public class Vector {
    List<byte[]> items = new List<byte[]>();
    public void AddElement(byte[] value) { items.Add(value); }
    public int Size() { return items.Count; }
    public object ElementAt(int index) { return items[index]; }
}
public static class StreamUtil {
    public static byte[] Read(StreamStub stream, int size) { return new byte[size]; }
    public static int ReadInt(StreamStub stream) { return stream.bytes.Length; }
    public static sbyte ReadByte(StreamStub stream) { return (sbyte)stream.bytes.Length; }
}
public class Fixture {
    HttpStub http_ = new HttpStub();
    Exception error_;
    string errorText_;
    public byte[][] Run(byte[] input, bool check, bool split) {
        int totalSize = 0;
        int recvSize = 0;
        StreamStub bais = new StreamStub();
        byte[][] recv = null;
        http_.bytes = input;
        Action action = delegate {
            if (http_ == null) { error_ = new Exception("timeout"); }
            else {
                byte[] bytes = http_.bytes;
Vector vector;
                totalSize = bytes.Length;
                bais.bytes = bytes;
                if (check) {
                    sbyte num = StreamUtil.ReadByte(bais);
                    sbyte b = StreamUtil.ReadByte(bais);
                    recvSize += 2;
                    recvSize++;
                    if (num != 79 || b != 75) {
                        byte[] bytes2 = StreamUtil.Read(bais, totalSize - recvSize);
                        string value = System.Text.Encoding.UTF8.GetString(bytes2);
                        string[] array = value.Split('\n');
                        try {
                            if (1 < array.Length) { errorText_ = array[1].Replace('|', '\n'); }
                        } catch (Exception) { errorText_ = value; }
                    }
                }
                if (error_ != null) { return; }
{
                    vector = new Vector();
                    while (recvSize < totalSize)
{
                        int size = totalSize - recvSize;
                        if (split) { size = StreamUtil.ReadInt(bais); recvSize += 4; }
                        byte[] array2 = StreamUtil.Read(bais, size);
                        vector.AddElement(array2);

recvSize += array2.Length;
}
                    recv = new byte[vector.Size()][];
                    for (int i = 0; i < vector.Size(); i++) {
                        recv[i] = (byte[])vector.ElementAt(i);
                    }
                }
            }
        };
        action();
        return recv;
    }
}
