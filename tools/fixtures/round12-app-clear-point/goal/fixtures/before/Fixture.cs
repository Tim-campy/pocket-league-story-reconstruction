public class Graphics { }
public static class Camera { public static void ConvertScreenToCamera(int[] input,int[] output) { output[0]=input[0]; } }
public class AppData { public void InsertGameObject(int a,int b,int c,int d,int e,int f,int x,int y,bool z,int w) { } }
public class Fixture {
public static int[][] GOAL1,GOAL2,GOAL1_PARTS_POS,GOAL2_PARTS_POS,GOAL1_PARTS_GRA,GOAL2_PARTS_GRA;
public int[] camera_pos; public AppData apdat_;
public void Run(Graphics g) { int num = -13;
int num2 = 17;
for (int i = 0; i < GOAL1.Length; i++) { Camera.ConvertScreenToCamera(GOAL1[i], camera_pos); }
for (int j = 0; j < GOAL1_PARTS_POS.Length; j++) {
 int x = camera_pos[0] + GOAL1_PARTS_POS[j][0] + num;
 int y = camera_pos[1] + GOAL1_PARTS_POS[j][1] + num2;
 int depth = camera_pos[1] + GOAL1_PARTS_POS[j][1] + num2 + GOAL1_PARTS_GRA[j][2];
 apdat_.InsertGameObject(8,0,depth,GOAL1_PARTS_GRA[j][0],GOAL1_PARTS_GRA[j][1],0,x,y,false,0);
}
num=12;num2=1;
for (int k = 0; k < GOAL2.Length; k++) { Camera.ConvertScreenToCamera(GOAL2[k], camera_pos); }
for (int l = 0; l < GOAL2_PARTS_POS.Length; l++) {
 int x = camera_pos[0] + GOAL2_PARTS_POS[l][0] + num;
 int y = camera_pos[1] + GOAL2_PARTS_POS[l][1] + num2;
 int depth = camera_pos[1] + GOAL2_PARTS_POS[l][1] + num2 + GOAL2_PARTS_GRA[l][2];
 apdat_.InsertGameObject(8,0,depth,GOAL2_PARTS_GRA[l][0],GOAL2_PARTS_GRA[l][1],0,x,y,false,0);
} }
}