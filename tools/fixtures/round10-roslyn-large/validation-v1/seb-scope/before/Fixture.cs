using System;
using System.Collections.Generic;
public class InputStream { public int value; }
public class DataInputStream {
    InputStream stream;
    public DataInputStream(InputStream value) { stream = value; }
    public int ReadByte() { return stream.value++; }
    public int ReadShort() { return stream.value++; }
    public void Close() { stream.value = 0; }
}
public class Vector { public List<short[]> data = new List<short[]>(); public void AddElement(short[] value) { data.Add(value); } }
public class Fixture {
    int sink;
    void Sink(int value) { sink += value; }
    public void Run(InputStream stream) {
        DataInputStream dataInputStream = null;
        try {
            dataInputStream = new DataInputStream(stream);
            int num = 0;
            short[][] array;
            int num2 = dataInputStream.ReadByte();
            int num3;
            if (num2 < 0) { num = -num2; num3 = dataInputStream.ReadShort(); }
            else { int num4 = dataInputStream.ReadByte(); num3 = (short)(((num2 & 255) << 8) | (num4 & 255)); }
            array = new short[num3][];
            sbyte[][] array2 = new sbyte[num3][];
            int[] array3 = new int[12];
            Vector vector = null;
            for (int i = 0; i < num3; i++) {
                int count = dataInputStream.ReadShort();
                array[i] = new short[count]; array2[i] = new sbyte[count];
                for (int j = 0; j < count; j++) {
                    switch (num) {
                        case 0: {
                            array3[0] = dataInputStream.ReadShort();
                            int num8 = array3[0] & 0xFF00;
                            switch (num8) {
                                case 8192: array3[10] = 1; break;
                                case 16384: array3[10] = 2; break;
                                case 32768: array3[10] = 3; break;
                            }
                            break;
                        }
                        case 1: array3[0] = dataInputStream.ReadShort(); break;
                    }
                    array[i][j] = (short)array3[0];
                    array2[i][j] = (sbyte)array3[10];
                    if (array3[10] != 0) {
                        if (vector == null) vector = new Vector();
                        vector.AddElement(new short[7] { (short)array3[1], (short)array3[6], (short)array3[7], (short)array3[2], (short)array3[3], (short)array3[4], (short)array3[5] });
                    }
                }
            }
            int num15 = 0;
            for (int k = 0; k < array2.Length; k++) { num15 += array2[k].Length; }
            Sink(num15);
            int l = 0;
            int num16 = 0;
            for (; l < array.Length; l++) { num16 += array[l].Length; }
            Sink(num16);
            vector = null;
            int[] array5 = new int[4];
            for (int n = 0; n < array2.Length; n++) { array5[2] += array2[n].Length; }
            array5[2] -= array5[0]; array5[3] -= array5[1];
            Sink(array5[2] + array5[3]);
        } finally { dataInputStream?.Close(); dataInputStream = null; }
    }
}
