class Fixture {
 int left;
 int Read(){return left-- > 0 ? 1 : -1;}
 public long Run(long n) {
  long num = 0L; long num2 = 0L;
  while (num2 < n) { if (Read() == -1) { break; } num2++; num++; }
  return num;
 }
}
