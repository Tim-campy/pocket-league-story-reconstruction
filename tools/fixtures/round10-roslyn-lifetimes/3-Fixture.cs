class KairoPlugin { public static int status; public static int calls; public static int LicenseCheck(){calls++;return status;} }
static class Bytes { public static byte[] GetBytes(this string value){return System.Text.Encoding.UTF8.GetBytes(value);} }
class Fixture {
 int errorCode_; string errorText_;
 public byte[][] Run() {
  int recoveredLicenseStatus = KairoPlugin.LicenseCheck();
  if (recoveredLicenseStatus == 0) { return new byte[1][] { "1\n100".GetBytes() }; }
  switch (recoveredLicenseStatus) {
   case 4: errorCode_ = -98; errorText_ = "four"; break;
   case 5:
   case 257: errorCode_ = -99; errorText_ = "five"; break;
   case 1:
   case 2:
   case 3:
   default: errorCode_ = -1; errorText_ = "default"; break;
  }
  System.GC.KeepAlive(errorCode_);
  System.GC.KeepAlive(errorText_);
  return null;
 }
}
