using System;
public static class JInteger { public static int ParseInt(string value) { return int.Parse(value); } }
public static class StringExtensions { public static char CharAt(this string value, int index) { return value[index]; } }
public class Fixture {
    int sink;
    void Sink(int value) { sink += value; }
    public void Run(string[] array7, string[] array9) {
        Action action = delegate {
            int imageAtlasId_ = -1;
            for (int n = 0; n < array7.Length; n++) {
                string text4 = array7[n];
                if (text4 == "res") { Sink(1); }
                else if (text4.StartsWith("@")) {
                    if (text4.Length == 1) { imageAtlasId_ = 0; } else { imageAtlasId_ = int.Parse(text4.Substring(1)); }
                }
                else { Sink(2); }
            }
            Sink(imageAtlasId_);
            bool u = false;
            bool v = false;
            int num21 = 0;
            for (int num22 = 1; num22 < array9.Length; num22++) {
                char recoveredOptionChar = array9[num22].CharAt(0);
if (recoveredOptionChar == 'u') { u = true; }
else if (recoveredOptionChar == 'v') { v = true; }
else if (recoveredOptionChar == 'c') { num21 = JInteger.ParseInt(array9[num22].Substring(1)); }
            }
            if (u) Sink(3);
            if (v) Sink(4);
            Sink(num21);
        };
        action();
    }
}
