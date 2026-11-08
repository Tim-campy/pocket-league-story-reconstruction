public static class Fixture{public static long Run(long c){if(c==0L){return 0L;}long answer;int index;int passes=10;answer=0L;
do{answer++;}while(answer*answer<c);
for(index=0;index<passes;index++){answer=(answer+c/answer)/2;}return answer;}}