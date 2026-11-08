public static class Fixture{public static long Run(long c){if(c==0L){return 0L;}int num = 10;long num2 = 0L;
do{num2++;}while(num2*num2<c);
for (int i = 0; i < num; i++){num2=(num2+c/num2)/2;}
return num2;}}