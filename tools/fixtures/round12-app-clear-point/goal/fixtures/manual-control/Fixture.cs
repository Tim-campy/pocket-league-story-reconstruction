public class Graphics { }
public static class Camera { public static void ConvertScreenToCamera(int[] input,int[] output) { output[0]=input[0]; } }
public class AppData { public void InsertGameObject(int a,int b,int c,int d,int e,int f,int x,int y,bool z,int w) { } }
public class Fixture {
public static int[][] GOAL1,GOAL2,GOAL1_PARTS_POS,GOAL2_PARTS_POS,GOAL1_PARTS_GRA,GOAL2_PARTS_GRA;
public int[] camera_pos; public AppData apdat_;
public void Run(Graphics g) { int horizontal=-13;
int vertical=17;
int xCoord,yCoord,depthCoord;
for(int index=0;index<GOAL1.Length;index++) { Camera.ConvertScreenToCamera(GOAL1[index],camera_pos); }
for(int part=0;part<GOAL1_PARTS_POS.Length;part++) {
 xCoord=camera_pos[0]+GOAL1_PARTS_POS[part][0]+horizontal;
 yCoord=camera_pos[1]+GOAL1_PARTS_POS[part][1]+vertical;
 depthCoord=camera_pos[1]+GOAL1_PARTS_POS[part][1]+vertical+GOAL1_PARTS_GRA[part][2];
 apdat_.InsertGameObject(8,0,depthCoord,GOAL1_PARTS_GRA[part][0],GOAL1_PARTS_GRA[part][1],0,xCoord,yCoord,false,0);
}
horizontal=12;vertical=1;
for(int index2=0;index2<GOAL2.Length;index2++) { Camera.ConvertScreenToCamera(GOAL2[index2],camera_pos); }
for(int part2=0;part2<GOAL2_PARTS_POS.Length;part2++) {
 xCoord=camera_pos[0]+GOAL2_PARTS_POS[part2][0]+horizontal;
 yCoord=camera_pos[1]+GOAL2_PARTS_POS[part2][1]+vertical;
 depthCoord=camera_pos[1]+GOAL2_PARTS_POS[part2][1]+vertical+GOAL2_PARTS_GRA[part2][2];
 apdat_.InsertGameObject(8,0,depthCoord,GOAL2_PARTS_GRA[part2][0],GOAL2_PARTS_GRA[part2][1],0,xCoord,yCoord,false,0);
} }
}