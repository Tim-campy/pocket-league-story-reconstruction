// TEXT TEMPLATE ONLY: NOT_TESTED / NOT_COMPILED / API_ACCEPTANCE_UNKNOWN.
// No transformation was invoked to produce this independent source template.
using System;
public static class FixtureStream19CommonExit {
 public sealed class OutputStream { public int Count; }
 public interface ISerializable { void Serialize(OutputStream os); }
 public struct SecureByte { public byte Value; }
 public struct SecureShort { public short Value; }
 public struct SecureInt { public int Value; }
 public struct SecureLong { public long Value; }
 public struct SecureString { public string Value; }
 static void Record(OutputStream os, object value) { os.Count++; }
 static void WriteArray(OutputStream os, Array array, int arrayLengthBytes) { Record(os,array); }
 static void WriteBoolean(OutputStream os, object value) { Record(os,value); }
 static void WriteByte(OutputStream os, object value) { Record(os,value); }
 static void WriteShort(OutputStream os, object value) { Record(os,value); }
 static void WriteInt(OutputStream os, object value) { Record(os,value); }
 static void WriteLong(OutputStream os, object value) { Record(os,value); }
 static void WriteFloat(OutputStream os, object value) { Record(os,value); }
 static void WriteDouble(OutputStream os, object value) { Record(os,value); }
 static void WriteString(OutputStream os, object value) { Record(os,value); }
 public static void Current(OutputStream os, Array array, int arrayLengthBytes = 4) {
  if (array == null) { return; }
  Type elementType = array.GetType().GetElementType();
  if (elementType.IsArray) {
   foreach (object item in array) { WriteArray(os, (Array)item, arrayLengthBytes); }
   return;
  }
  if (elementType == typeof(bool)) {
   foreach (object item2 in array) { WriteBoolean(os, (bool)item2); }
   return;
  }
  if (elementType == typeof(byte)) {
   foreach (object item3 in array) { WriteByte(os, (byte)item3); }
   return;
  }
  if (elementType == typeof(sbyte)) {
   foreach (object item4 in array) { WriteByte(os, (sbyte)item4); }
   return;
  }
  if (elementType == typeof(short)) {
   foreach (object item5 in array) { WriteShort(os, (short)item5); }
   return;
  }
  if (elementType == typeof(ushort)) {
   foreach (object item6 in array) { WriteShort(os, (ushort)item6); }
   return;
  }
  if (elementType == typeof(int)) {
   foreach (object item7 in array) { WriteInt(os, (int)item7); }
   return;
  }
  if (elementType == typeof(uint)) {
   foreach (object item8 in array) { WriteInt(os, (uint)item8); }
   return;
  }
  if (elementType == typeof(long)) {
   foreach (object item9 in array) { WriteLong(os, (long)item9); }
   return;
  }
  if (elementType == typeof(ulong)) {
   foreach (object item10 in array) { WriteLong(os, (ulong)item10); }
   return;
  }
  if (elementType == typeof(float)) {
   foreach (object item11 in array) { WriteFloat(os, (float)item11); }
   return;
  }
  if (elementType == typeof(double)) {
   foreach (object item12 in array) { WriteDouble(os, (double)item12); }
   return;
  }
  if (elementType == typeof(string)) {
   foreach (object item13 in array) { WriteString(os, (string)item13); }
   return;
  }
  if (elementType == typeof(SecureByte)) {
   foreach (object item14 in array) { WriteByte(os, (SecureByte)item14); }
   return;
  }
  if (elementType == typeof(SecureShort)) {
   foreach (object item15 in array) { WriteShort(os, (SecureShort)item15); }
   return;
  }
  if (elementType == typeof(SecureInt)) {
   foreach (object item16 in array) { WriteInt(os, (SecureInt)item16); }
   return;
  }
  if (elementType == typeof(SecureLong)) {
   foreach (object item17 in array) { WriteLong(os, (SecureLong)item17); }
   return;
  }
  if (elementType == typeof(SecureString)) {
   foreach (object item18 in array) { WriteString(os, (SecureString)item18); }
   return;
  }
  if (elementType.GetInterface(typeof(ISerializable).Name) != null) {
   foreach (ISerializable item19 in array) { WriteBoolean(os, item19 != null); item19?.Serialize(os); }
   return;
  }
  throw new ArgumentException("StreamUtil.WriteArray : unsupported by " + elementType.FullName);
 }
 public static void Expected(OutputStream os, Array array, int arrayLengthBytes = 4) {
  if (array == null) { return; }
  Type elementType = array.GetType().GetElementType();
  if (elementType.IsArray) {
   foreach (object item in array) { WriteArray(os, (Array)item, arrayLengthBytes); }
  }
  else if (elementType == typeof(bool)) {
   foreach (object item2 in array) { WriteBoolean(os, (bool)item2); }
  }
  else if (elementType == typeof(byte)) {
   foreach (object item3 in array) { WriteByte(os, (byte)item3); }
  }
  else if (elementType == typeof(sbyte)) {
   foreach (object item4 in array) { WriteByte(os, (sbyte)item4); }
  }
  else if (elementType == typeof(short)) {
   foreach (object item5 in array) { WriteShort(os, (short)item5); }
  }
  else if (elementType == typeof(ushort)) {
   foreach (object item6 in array) { WriteShort(os, (ushort)item6); }
  }
  else if (elementType == typeof(int)) {
   foreach (object item7 in array) { WriteInt(os, (int)item7); }
  }
  else if (elementType == typeof(uint)) {
   foreach (object item8 in array) { WriteInt(os, (uint)item8); }
  }
  else if (elementType == typeof(long)) {
   foreach (object item9 in array) { WriteLong(os, (long)item9); }
  }
  else if (elementType == typeof(ulong)) {
   foreach (object item10 in array) { WriteLong(os, (ulong)item10); }
  }
  else if (elementType == typeof(float)) {
   foreach (object item11 in array) { WriteFloat(os, (float)item11); }
  }
  else if (elementType == typeof(double)) {
   foreach (object item12 in array) { WriteDouble(os, (double)item12); }
  }
  else if (elementType == typeof(string)) {
   foreach (object item13 in array) { WriteString(os, (string)item13); }
  }
  else if (elementType == typeof(SecureByte)) {
   foreach (object item14 in array) { WriteByte(os, (SecureByte)item14); }
  }
  else if (elementType == typeof(SecureShort)) {
   foreach (object item15 in array) { WriteShort(os, (SecureShort)item15); }
  }
  else if (elementType == typeof(SecureInt)) {
   foreach (object item16 in array) { WriteInt(os, (SecureInt)item16); }
  }
  else if (elementType == typeof(SecureLong)) {
   foreach (object item17 in array) { WriteLong(os, (SecureLong)item17); }
  }
  else if (elementType == typeof(SecureString)) {
   foreach (object item18 in array) { WriteString(os, (SecureString)item18); }
  }
  else if (elementType.GetInterface(typeof(ISerializable).Name) != null) {
   foreach (ISerializable item19 in array) { WriteBoolean(os, item19 != null); item19?.Serialize(os); }
  }
  else { throw new ArgumentException("StreamUtil.WriteArray : unsupported by " + elementType.FullName); }
 }
 // Proposed controls/negatives (NOT executed):
 // - Replace one type condition/order: API must reject the unproved sequence.
 // - Insert Record(os,null) between foreach and its terminal return: reject.
 // - Preserve an extra effect inside a foreach body rather than delete it.
 // - Add an extra terminal effect before the unsupported throw: reject.
 // - Keep the single initial null-array return in both forms.
 // Compare all 19 finally clauses, leave destinations, widths, whole IL and
 // local signatures; do not judge only a prefix or total byte count.
}
