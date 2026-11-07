using System;
using data;
using form;
using java.io;
using java.util;
using kairo.unity.io;
using kairo.unity.m3d4.util3d;
using kairo.unity.ui;
using kairo.unity.util;
using main;
using surface;

namespace game
{
	public class Map
	{
		public const int STEP_W = 60;

		public const int STEP_H = 30;

		public const int MAP_GROUND_GROUND = 0;

		public const int MAP_GROUND_SNOW = 1;

		public const int MAP_GROUND_DESERT = 2;

		public const int MAP_GROUND_GRAVEL = 3;

		public const int BAR_H = 21;

		private const int LDEPTH = 1;

		private const int CCIRCLE_DEPTH = 0;

		public AppData apdat_;

		private UserData udat_;

		public int stadiumId_;

		public MapObject[] mapObjects_;

		public static int MAPCHIP_COAT_WNUM = 11;

		public static int MAPCHIP_COAT_HNUM = 16;

		public static int MAPCHIP_FIELD_WNUM;

		public static int MAPCHIP_FIELD_HNUM;

		public static int[][][] MAPCHIP_SPOS;

		public static int[][] MAPCHIP_IMG;

		public static int[] COAT_BASEPOINT;

		public static int[][] COAT_CONNER;

		public static int[][] COAT_CENTER;

		public static int[][] COAT_GOAL1;

		public static int[][] COAT_GOAL2;

		public static int[][] COAT_GOAL1_IN;

		public static int[][] COAT_GOAL2_IN;

		public static int[][] GOAL1;

		public static int[][] GOAL2;

		public static int[][] MAP_CONNER;

		public static int MAP_GROUND;

		public static readonly string[] MAP_GROUND_NAME = new string[0];

		public static MapchipMatch[] mapchipGrounds;

		private int[] screen_pos = new int[2];

		private int[] camera_pos = new int[2];

		private int[][] c_poss = JTool.MakeArray<int[][]>(new int[2] { 4, 2 });

		public static readonly Vector3D[] COAT_GOAL1_GLOBALPOS = new Vector3D[2]
		{
			new Vector3D(-175f, 0f, -900f),
			new Vector3D(225f, 0f, -675f)
		};

		public static readonly Vector3D[] COAT_GOAL1_IN_GLOBALPOS = new Vector3D[2]
		{
			new Vector3D(-87.5f, 0f, -900f),
			new Vector3D(112.5f, 0f, -787.5f)
		};

		public static readonly Vector3D[] COAT_GOAL2_GLOBALPOS = new Vector3D[2]
		{
			new Vector3D(225f, 0f, 900f),
			new Vector3D(-175f, 0f, 675f)
		};

		public static readonly Vector3D[] COAT_GOAL2_IN_GLOBALPOS = new Vector3D[2]
		{
			new Vector3D(112.5f, 0f, 900f),
			new Vector3D(-87.5f, 0f, 787.5f)
		};

		public static readonly Vector3D[] COAT_GOAL1_GPOS_CAREA = new Vector3D[2]
		{
			new Vector3D((COAT_GOAL1_GLOBALPOS[0].x_ + COAT_GOAL1_IN_GLOBALPOS[0].x_) / 2f, 0f, (COAT_GOAL1_GLOBALPOS[0].z_ + COAT_GOAL1_IN_GLOBALPOS[0].z_) / 2f + 56.25f),
			new Vector3D((COAT_GOAL1_GLOBALPOS[1].x_ + COAT_GOAL1_IN_GLOBALPOS[1].x_) / 2f, 0f, (COAT_GOAL1_GLOBALPOS[1].z_ + COAT_GOAL1_IN_GLOBALPOS[1].z_) / 2f)
		};

		public static readonly Vector3D[] COAT_GOAL2_GPOS_CAREA = new Vector3D[2]
		{
			new Vector3D((COAT_GOAL2_GLOBALPOS[0].x_ + COAT_GOAL2_IN_GLOBALPOS[0].x_) / 2f, 0f, (COAT_GOAL2_GLOBALPOS[0].z_ + COAT_GOAL2_IN_GLOBALPOS[0].z_) / 2f - 56.25f),
			new Vector3D((COAT_GOAL2_GLOBALPOS[1].x_ + COAT_GOAL2_IN_GLOBALPOS[1].x_) / 2f, 0f, (COAT_GOAL2_GLOBALPOS[1].z_ + COAT_GOAL2_IN_GLOBALPOS[1].z_) / 2f)
		};

		public static int[][] COAT_GOAL1_ADDRESS = JTool.MakeArray<int[][]>(new int[2] { 2, 2 });

		public static int[][] COAT_GOAL2_ADDRESS = JTool.MakeArray<int[][]>(new int[2] { 2, 2 });

		public static int[][] COAT_GOAL1_IN_ADDRESS = JTool.MakeArray<int[][]>(new int[2] { 2, 2 });

		public static int[][] COAT_GOAL2_IN_ADDRESS = JTool.MakeArray<int[][]>(new int[2] { 2, 2 });

		public static readonly Vector3D[] GOAL1_GLOBAL_POS = new Vector3D[4];

		public static readonly Vector3D[] GOAL2_GLOBAL_POS = new Vector3D[4];

		public int[][] GOAL1_CAMERA_POS = JTool.MakeArray<int[][]>(new int[2] { 4, 2 });

		public int[][] GOAL2_CAMERA_POS = JTool.MakeArray<int[][]>(new int[2] { 4, 2 });

		public static Vector3D[] DIRECT_VEC;

		public int temp_frame;

		private Vector3D[] cameraRect_gpos = new Vector3D[4];

		private int[][] cameraRect_spos = JTool.MakeArray<int[][]>(new int[2] { 4, 2 });

		private int[][] cameraRectAddress = JTool.MakeArray<int[][]>(new int[2] { 4, 2 });

		private int[] center_spos = new int[2];

		private int[] mapchip_off = new int[2];

		internal int[] hit_rect = new int[4];

		private static int[] old_camera_pos = new int[2];

		public static int[][] GOAL1_PARTS_POS = new int[8][]
		{
			new int[2] { -25, -12 },
			new int[2] { -1, 0 },
			new int[2] { 23, 12 },
			new int[2] { -16, -45 },
			new int[2] { 8, -33 },
			new int[2] { 32, -21 },
			new int[2] { -33, -23 },
			new int[2] { 38, 12 }
		};

		public static int[][] GOAL2_PARTS_POS = new int[8][]
		{
			new int[2] { -24, -12 },
			new int[2],
			new int[2] { 24, 12 },
			new int[2] { -32, -35 },
			new int[2] { -8, -23 },
			new int[2] { 16, -11 },
			new int[2] { -40, -17 },
			new int[2] { 34, 20 }
		};

		public static int[][] GOAL1_PARTS_GRA = new int[8][]
		{
			new int[7] { 3, 1, -5, 37, 46, 1, -1 },
			new int[7] { 3, 1, -5, 37, 46, 1, -1 },
			new int[7] { 3, 1, -5, 37, 46, 1, -1 },
			new int[7] { 5, 3, 29, 31, 18, 1, -1 },
			new int[7] { 5, 3, 29, 31, 18, 1, -1 },
			new int[7] { 5, 3, 29, 31, 18, 1, -1 },
			new int[7] { 4, 2, -8, 20, 41, -1, 1 },
			new int[7] { 4, 2, -4, 20, 41, 1, -1 }
		};

		public static int[][] GOAL2_PARTS_GRA = new int[8][]
		{
			new int[7] { 6, 4, -7, 36, 42, -1, 1 },
			new int[7] { 6, 4, -7, 36, 42, -1, 1 },
			new int[7] { 6, 4, -7, 36, 42, -1, 1 },
			new int[7] { 5, 3, 23, 31, 18, 1, -1 },
			new int[7] { 5, 3, 23, 31, 18, 1, -1 },
			new int[7] { 5, 3, 26, 31, 18, 1, -1 },
			new int[7] { 7, 5, -4, 20, 37, -1, 1 },
			new int[7] { 7, 5, -4, 20, 37, 1, -1 }
		};

		public static readonly string[] GOAL_PARTS_NAME = new string[8] { "バック_奥", "バック_真ん中", "バック_手前", "TOP_奥", "TOP_真ん中", "TOP_手前", "SIDE_奥", "SIDE_手前" };

		public int goalBackNetNum_ = 3;

		public Map()
		{
			apdat_ = AppData.GetInstance();
			udat_ = UserData.GetInstance();
		}

		public void Init()
		{
			COAT_CONNER = JTool.MakeArray<int[][]>(new int[2] { 4, 2 });
			COAT_CENTER = JTool.MakeArray<int[][]>(new int[2] { 3, 2 });
			COAT_GOAL1 = JTool.MakeArray<int[][]>(new int[2] { 4, 2 });
			COAT_GOAL2 = JTool.MakeArray<int[][]>(new int[2] { 4, 2 });
			COAT_GOAL1_IN = JTool.MakeArray<int[][]>(new int[2] { 4, 2 });
			COAT_GOAL2_IN = JTool.MakeArray<int[][]>(new int[2] { 4, 2 });
			GOAL1 = JTool.MakeArray<int[][]>(new int[2] { 1, 2 });
			GOAL2 = JTool.MakeArray<int[][]>(new int[2] { 1, 2 });
			MAP_CONNER = JTool.MakeArray<int[][]>(new int[2] { 4, 2 });
			Vector3D[] array = new Vector3D[4]
			{
				new Vector3D(-450f, 0f, -900f),
				new Vector3D(-450f, 0f, 900f),
				new Vector3D(450f, 0f, -900f),
				new Vector3D(450f, 0f, 900f)
			};
			for (int i = 0; i < MAP_CONNER.Length; i++)
			{
				Camera.ConvertGlobalToScreen(array[i], MAP_CONNER[i]);
			}
			DIRECT_VEC = new Vector3D[5];
			int[][] array2 = new int[5][]
			{
				new int[2] { 1, 2 },
				new int[2] { 2, 1 },
				new int[2] { 1, 0 },
				new int[2] { 0, 1 },
				new int[2] { 1, 1 }
			};
			for (int j = 0; j < DIRECT_VEC.Length; j++)
			{
				DIRECT_VEC[j] = new Vector3D();
				Camera.ConvertAddressToGlobal(array2[j][0], array2[j][1], DIRECT_VEC[j]);
			}
			for (int k = 0; k < 4; k++)
			{
				DIRECT_VEC[k].x_ = DIRECT_VEC[k].x_ - DIRECT_VEC[4].x_;
				DIRECT_VEC[k].y_ = 0f;
				DIRECT_VEC[k].z_ = DIRECT_VEC[k].z_ - DIRECT_VEC[4].z_;
			}
		}

		public static void Load2()
		{
			AppData instance = AppData.GetInstance();
			mapchipGrounds = new MapchipMatch[4];
			int[] array = new int[4] { 2, 4, 8, 16 };
			for (int i = 0; i < array.Length; i++)
			{
				for (int j = 0; j < instance.mapchipMatch_.Length; j++)
				{
					MapchipMatch mapchipMatch = instance.mapchipMatch_[j];
					if (BitUtil.Check(mapchipMatch.flag_, array[i]))
					{
						mapchipGrounds[i] = mapchipMatch;
						break;
					}
				}
			}
		}

		public void SetCoatLine()
		{
			Camera.ConvertGlobalToAddress(COAT_GOAL1_GLOBALPOS[0], COAT_GOAL1_ADDRESS[0]);
			Camera.ConvertGlobalToAddress(COAT_GOAL1_GLOBALPOS[1], COAT_GOAL1_ADDRESS[1]);
			Camera.ConvertGlobalToAddress(COAT_GOAL2_GLOBALPOS[0], COAT_GOAL2_ADDRESS[0]);
			Camera.ConvertGlobalToAddress(COAT_GOAL2_GLOBALPOS[1], COAT_GOAL2_ADDRESS[1]);
			Camera.ConvertGlobalToAddress(COAT_GOAL1_IN_GLOBALPOS[0], COAT_GOAL1_IN_ADDRESS[0]);
			Camera.ConvertGlobalToAddress(COAT_GOAL1_IN_GLOBALPOS[1], COAT_GOAL1_IN_ADDRESS[1]);
			Camera.ConvertGlobalToAddress(COAT_GOAL2_IN_GLOBALPOS[0], COAT_GOAL2_IN_ADDRESS[0]);
			Camera.ConvertGlobalToAddress(COAT_GOAL2_IN_GLOBALPOS[1], COAT_GOAL2_IN_ADDRESS[1]);
			int[] array = new int[2]
;
			array[0] = Camera.MAP_SCREEN_W1;
			array[1] = Camera.MAP_SCREEN_H1;
			float num = (float)(-COAT_BASEPOINT[0]) - 0.5f;
			float num2 = (float)(-COAT_BASEPOINT[1]) + 0.5f;
			int[] array2 = new int[2]
;
			array2[0] = (int)(num2 * 60f / 2f + num * 60f / 2f);
			array2[1] = (int)(num2 * 30f / 2f - num * 30f / 2f);
			MAPCHIP_SPOS = JTool.MakeArray<int[][][]>(new int[3] { MAPCHIP_FIELD_HNUM, MAPCHIP_FIELD_WNUM, 2 });
			for (int num3 = MAPCHIP_SPOS.Length - 1; num3 >= 0; num3--)
			{
				for (int i = 0; i < MAPCHIP_SPOS[num3].Length; i++)
				{
					MAPCHIP_SPOS[num3][i][0] = num3 * 60 / 2 + i * 60 / 2 - array[0] + array2[0];
					MAPCHIP_SPOS[num3][i][1] = num3 * 30 / 2 - i * 30 / 2 - array[1] + array2[1];
				}
			}
			for (int j = 0; j < mapObjects_.Length; j++)
			{
				mapObjects_[j].spos_[0] += (MAPCHIP_SPOS.Length - 1) * 60 / 2 - array[0] + array2[0];
				mapObjects_[j].spos_[1] += (MAPCHIP_SPOS.Length - 1) * 30 / 2 - array[1] + array2[1];
			}
			COAT_CONNER[0][0] = -array[0];
			COAT_CONNER[0][1] = -array[1];
			COAT_CONNER[1][0] = 0 + MAPCHIP_COAT_WNUM * 60 / 2 - array[0];
			COAT_CONNER[1][1] = -(MAPCHIP_COAT_WNUM * 30 / 2) - array[1];
			COAT_CONNER[2][0] = MAPCHIP_COAT_HNUM * 60 / 2 - array[0];
			COAT_CONNER[2][1] = MAPCHIP_COAT_HNUM * 30 / 2 - array[1];
			COAT_CONNER[3][0] = MAPCHIP_COAT_HNUM * 60 / 2 + MAPCHIP_COAT_WNUM * 60 / 2 - array[0];
			COAT_CONNER[3][1] = MAPCHIP_COAT_HNUM * 30 / 2 - MAPCHIP_COAT_WNUM * 30 / 2 - array[1];
			COAT_CENTER[0][0] = MAPCHIP_COAT_HNUM * 60 / 2 / 2 - array[0];
			COAT_CENTER[0][1] = MAPCHIP_COAT_HNUM * 30 / 2 / 2 - array[1];
			COAT_CENTER[1][0] = MAPCHIP_COAT_HNUM * 60 / 2 / 2 + MAPCHIP_COAT_WNUM * 60 / 2 - array[0];
			COAT_CENTER[1][1] = MAPCHIP_COAT_HNUM * 30 / 2 / 2 - MAPCHIP_COAT_WNUM * 30 / 2 - array[1];
			COAT_CENTER[2][0] = MAPCHIP_COAT_HNUM * 60 / 2 / 2 + MAPCHIP_COAT_WNUM * 60 / 2 / 2 - array[0];
			COAT_CENTER[2][1] = MAPCHIP_COAT_HNUM * 30 / 2 / 2 - MAPCHIP_COAT_WNUM * 30 / 2 / 2 - array[1];
			COAT_GOAL1[0][0] = 0 + MAPCHIP_COAT_WNUM * 60 / 2 / 4 - array[0];
			COAT_GOAL1[0][1] = -(MAPCHIP_COAT_WNUM * 30 / 2 / 4) - array[1];
			COAT_GOAL1[1][0] = 0 + MAPCHIP_COAT_WNUM * 3 * 60 / 2 / 4 - array[0];
			COAT_GOAL1[1][1] = -(MAPCHIP_COAT_WNUM * 3 * 30 / 2 / 4) - array[1];
			COAT_GOAL1[2][0] = MAPCHIP_COAT_HNUM / 8 * 60 / 2 + MAPCHIP_COAT_WNUM * 60 / 2 / 4 - array[0];
			COAT_GOAL1[2][1] = MAPCHIP_COAT_HNUM / 8 * 30 / 2 - MAPCHIP_COAT_WNUM * 30 / 2 / 4 - array[1];
			COAT_GOAL1[3][0] = MAPCHIP_COAT_HNUM / 8 * 60 / 2 + MAPCHIP_COAT_WNUM * 3 * 60 / 2 / 4 - array[0];
			COAT_GOAL1[3][1] = MAPCHIP_COAT_HNUM / 8 * 30 / 2 - MAPCHIP_COAT_WNUM * 3 * 30 / 2 / 4 - array[1];
			COAT_GOAL2[0][0] = MAPCHIP_COAT_HNUM * 60 / 2 + MAPCHIP_COAT_WNUM * 60 / 2 / 4 - array[0];
			COAT_GOAL2[0][1] = MAPCHIP_COAT_HNUM * 30 / 2 - MAPCHIP_COAT_WNUM * 30 / 2 / 4 - array[1];
			COAT_GOAL2[1][0] = MAPCHIP_COAT_HNUM * 60 / 2 + MAPCHIP_COAT_WNUM * 3 * 60 / 2 / 4 - array[0];
			COAT_GOAL2[1][1] = MAPCHIP_COAT_HNUM * 30 / 2 - MAPCHIP_COAT_WNUM * 3 * 30 / 2 / 4 - array[1];
			COAT_GOAL2[2][0] = (MAPCHIP_COAT_HNUM - MAPCHIP_COAT_HNUM / 8) * 60 / 2 + MAPCHIP_COAT_WNUM * 60 / 2 / 4 - array[0];
			COAT_GOAL2[2][1] = (MAPCHIP_COAT_HNUM - MAPCHIP_COAT_HNUM / 8) * 30 / 2 - MAPCHIP_COAT_WNUM * 30 / 2 / 4 - array[1];
			COAT_GOAL2[3][0] = (MAPCHIP_COAT_HNUM - MAPCHIP_COAT_HNUM / 8) * 60 / 2 + MAPCHIP_COAT_WNUM * 3 * 60 / 2 / 4 - array[0];
			COAT_GOAL2[3][1] = (MAPCHIP_COAT_HNUM - MAPCHIP_COAT_HNUM / 8) * 30 / 2 - MAPCHIP_COAT_WNUM * 3 * 30 / 2 / 4 - array[1];
			COAT_GOAL1_IN[0][0] = 0 + MAPCHIP_COAT_WNUM * 30 * 3 / 8 - array[0];
			COAT_GOAL1_IN[0][1] = -(MAPCHIP_COAT_WNUM * 15 * 3 / 8) - array[1];
			COAT_GOAL1_IN[1][0] = 0 + MAPCHIP_COAT_WNUM * 30 * 5 / 8 - array[0];
			COAT_GOAL1_IN[1][1] = -(MAPCHIP_COAT_WNUM * 15 * 5 / 8) - array[1];
			COAT_GOAL1_IN[2][0] = MAPCHIP_COAT_HNUM / 16 * 60 / 2 + MAPCHIP_COAT_WNUM * 30 * 3 / 8 - array[0];
			COAT_GOAL1_IN[2][1] = MAPCHIP_COAT_HNUM / 16 * 30 / 2 - MAPCHIP_COAT_WNUM * 15 * 3 / 8 - array[1];
			COAT_GOAL1_IN[3][0] = MAPCHIP_COAT_HNUM / 16 * 60 / 2 + MAPCHIP_COAT_WNUM * 30 * 5 / 8 - array[0];
			COAT_GOAL1_IN[3][1] = MAPCHIP_COAT_HNUM / 16 * 30 / 2 - MAPCHIP_COAT_WNUM * 15 * 5 / 8 - array[1];
			COAT_GOAL2_IN[0][0] = MAPCHIP_COAT_HNUM * 60 / 2 + MAPCHIP_COAT_WNUM * 30 * 3 / 8 - array[0];
			COAT_GOAL2_IN[0][1] = MAPCHIP_COAT_HNUM * 30 / 2 - MAPCHIP_COAT_WNUM * 15 * 3 / 8 - array[1];
			COAT_GOAL2_IN[1][0] = MAPCHIP_COAT_HNUM * 60 / 2 + MAPCHIP_COAT_WNUM * 30 * 5 / 8 - array[0];
			COAT_GOAL2_IN[1][1] = MAPCHIP_COAT_HNUM * 30 / 2 - MAPCHIP_COAT_WNUM * 15 * 5 / 8 - array[1];
			COAT_GOAL2_IN[2][0] = (MAPCHIP_COAT_HNUM - MAPCHIP_COAT_HNUM / 16) * 60 / 2 + MAPCHIP_COAT_WNUM * 30 * 3 / 8 - array[0];
			COAT_GOAL2_IN[2][1] = (MAPCHIP_COAT_HNUM - MAPCHIP_COAT_HNUM / 16) * 30 / 2 - MAPCHIP_COAT_WNUM * 15 * 3 / 8 - array[1];
			COAT_GOAL2_IN[3][0] = (MAPCHIP_COAT_HNUM - MAPCHIP_COAT_HNUM / 16) * 60 / 2 + MAPCHIP_COAT_WNUM * 30 * 5 / 8 - array[0];
			COAT_GOAL2_IN[3][1] = (MAPCHIP_COAT_HNUM - MAPCHIP_COAT_HNUM / 16) * 30 / 2 - MAPCHIP_COAT_WNUM * 15 * 5 / 8 - array[1];
			GOAL1[0][0] = 0 + MAPCHIP_COAT_WNUM * 60 / 2 / 2 - array[0];
			GOAL1[0][1] = -(MAPCHIP_COAT_WNUM * 30 / 2 / 2) - array[1];
			GOAL2[0][0] = MAPCHIP_COAT_HNUM * 60 / 2 + MAPCHIP_COAT_WNUM * 60 / 2 / 2 - array[0];
			GOAL2[0][1] = MAPCHIP_COAT_HNUM * 30 / 2 - MAPCHIP_COAT_WNUM * 30 / 2 / 2 - array[1];
			int num4 = 20;
			int num5 = 50;
			int num6 = 40;
			GOAL1_GLOBAL_POS[0] = new Vector3D(-900f / (float)MAPCHIP_COAT_WNUM * (float)goalBackNetNum_ / 2f + (float)num4, 0f, -900f);
			GOAL1_GLOBAL_POS[1] = new Vector3D(900f / (float)MAPCHIP_COAT_WNUM * (float)goalBackNetNum_ / 2f - (float)num4, 0f, -900f);
			GOAL1_GLOBAL_POS[2] = new Vector3D(900f / (float)MAPCHIP_COAT_WNUM * (float)goalBackNetNum_ / 2f - (float)num4, 0f, -900f - (float)num5);
			GOAL1_GLOBAL_POS[3] = new Vector3D(-900f / (float)MAPCHIP_COAT_WNUM * (float)goalBackNetNum_ / 2f + (float)num4, 0f, -900f - (float)num5);
			GOAL2_GLOBAL_POS[0] = new Vector3D(900f / (float)MAPCHIP_COAT_WNUM * (float)goalBackNetNum_ / 2f - (float)num4, 0f, 900f);
			GOAL2_GLOBAL_POS[1] = new Vector3D(-900f / (float)MAPCHIP_COAT_WNUM * (float)goalBackNetNum_ / 2f + (float)num4, 0f, 900f);
			GOAL2_GLOBAL_POS[2] = new Vector3D(-900f / (float)MAPCHIP_COAT_WNUM * (float)goalBackNetNum_ / 2f + (float)num4, 0f, 900f + (float)num6);
			GOAL2_GLOBAL_POS[3] = new Vector3D(900f / (float)MAPCHIP_COAT_WNUM * (float)goalBackNetNum_ / 2f - (float)num4, 0f, 900f + (float)num6);
		}

		public void SetDefaultCoatEva()
		{
			Team.DEAFULT_FORMATION_EVA = JTool.MakeArray<int[][]>(new int[2] { MAPCHIP_COAT_HNUM, MAPCHIP_COAT_WNUM });
			Team.DEAFULT_FORMATION_EVA_PENALTYAREA = JTool.MakeArray<int[][]>(new int[2] { MAPCHIP_COAT_HNUM, MAPCHIP_COAT_WNUM });
			float num = 100f;
global::System.Single recoveredUnusedSingleSlot1;
float num2 = 1000f;
global::System.Single originalUnusedSlot1;
float num3, num4;
for (int i = 0; i < MAPCHIP_COAT_HNUM; i++)
			{
				 num3 = num;
				 num4 = num2;
				int num5 = COAT_GOAL2_ADDRESS[1][1] - i;
				if (num5 > 0)
				{
					for (int j = 0; j < num5; j++)
					{
						num3 *= 0.8f;
						num4 *= 0.1f;
					}
				}
				for (int k = 0; k < MAPCHIP_COAT_WNUM; k++)
				{
					Team.DEAFULT_FORMATION_EVA[i][k] = (int)num3;
					Team.DEAFULT_FORMATION_EVA_PENALTYAREA[i][k] = (int)num4;
				}
			}
			for (int l = COAT_GOAL2_ADDRESS[1][1]; l <= COAT_GOAL2_ADDRESS[0][1]; l++)
			{
				 num3 = num;
				 num4 = num2;
				for (int num6 = COAT_GOAL1_ADDRESS[0][0] - 1; num6 >= 0; num6--)
				{
					num3 *= 0.8f;
					num4 *= 0.1f;
					Team.DEAFULT_FORMATION_EVA[l][num6] = (int)num3;
					Team.DEAFULT_FORMATION_EVA_PENALTYAREA[l][num6] = (int)num4;
				}
				num3 = num;
				num4 = num2;
				for (int m = COAT_GOAL1_ADDRESS[1][0] + 1; m < MAPCHIP_COAT_WNUM; m++)
				{
					num3 *= 0.8f;
					num4 *= 0.1f;
					Team.DEAFULT_FORMATION_EVA[l][m] = (int)num3;
					Team.DEAFULT_FORMATION_EVA_PENALTYAREA[l][m] = (int)num4;
				}
			}
			for (int num7 = COAT_GOAL2_ADDRESS[1][1] - 1; num7 >= 0; num7--)
			{
				for (int num8 = COAT_GOAL1_ADDRESS[0][0] - 1; num8 >= 0; num8--)
				{
					 num3 = Team.DEAFULT_FORMATION_EVA[num7][num8];
					if (num3 > (float)Team.DEAFULT_FORMATION_EVA[num7][num8 + 1])
					{
						num3 = Team.DEAFULT_FORMATION_EVA[num7][num8 + 1];
					}
					if (num3 > (float)Team.DEAFULT_FORMATION_EVA[num7 + 1][num8])
					{
						num3 = Team.DEAFULT_FORMATION_EVA[num7 + 1][num8];
					}
					Team.DEAFULT_FORMATION_EVA[num7][num8] = (int)num3;
					 num4 = Team.DEAFULT_FORMATION_EVA_PENALTYAREA[num7][num8];
					if (num3 > (float)Team.DEAFULT_FORMATION_EVA_PENALTYAREA[num7][num8 + 1])
					{
						num4 = Team.DEAFULT_FORMATION_EVA_PENALTYAREA[num7][num8 + 1];
					}
					if (num3 > (float)Team.DEAFULT_FORMATION_EVA_PENALTYAREA[num7 + 1][num8])
					{
						num4 = Team.DEAFULT_FORMATION_EVA_PENALTYAREA[num7 + 1][num8];
					}
					Team.DEAFULT_FORMATION_EVA_PENALTYAREA[num7][num8] = (int)num4;
				}
				for (int n = COAT_GOAL1_ADDRESS[1][0] + 1; n < MAPCHIP_COAT_WNUM; n++)
				{
					 num3 = Team.DEAFULT_FORMATION_EVA[num7][n];
					if (num3 > (float)Team.DEAFULT_FORMATION_EVA[num7][n - 1])
					{
						num3 = Team.DEAFULT_FORMATION_EVA[num7][n - 1];
					}
					if (num3 > (float)Team.DEAFULT_FORMATION_EVA[num7 + 1][n])
					{
						num3 = Team.DEAFULT_FORMATION_EVA[num7 + 1][n];
					}
					Team.DEAFULT_FORMATION_EVA[num7][n] = (int)num3;
					 num4 = Team.DEAFULT_FORMATION_EVA[num7][n];
					if (num4 > (float)Team.DEAFULT_FORMATION_EVA[num7][n - 1])
					{
						num4 = Team.DEAFULT_FORMATION_EVA[num7][n - 1];
					}
					if (num4 > (float)Team.DEAFULT_FORMATION_EVA[num7 + 1][n])
					{
						num4 = Team.DEAFULT_FORMATION_EVA[num7 + 1][n];
					}
					Team.DEAFULT_FORMATION_EVA[num7][n] = (int)num4;
				}
			}
			for (int num9 = 4; num9 <= 6; num9++)
			{
				Team.DEAFULT_FORMATION_EVA[Team.DEAFULT_FORMATION_EVA.Length - 1][num9] = Team.DEAFULT_FORMATION_EVA[Team.DEAFULT_FORMATION_EVA.Length - 1][num9] * 0 / 100;
				Team.DEAFULT_FORMATION_EVA_PENALTYAREA[Team.DEAFULT_FORMATION_EVA_PENALTYAREA.Length - 1][num9] = Team.DEAFULT_FORMATION_EVA_PENALTYAREA[Team.DEAFULT_FORMATION_EVA_PENALTYAREA.Length - 1][num9] * 0 / 100;
			}
		}

		public static void CreateMap(int stadiumId, string fileName)
		{
			AppData instance = AppData.GetInstance();
			RecordStore recordStore = null;
			JarInflater jarInflater = null;
			try
			{
				recordStore = RecordStore.OpenRecordStore(1);
				jarInflater = new JarInflater(recordStore.ReadRecord(2));
				instance.map_ = new Map();
				instance.map_.Init();
				instance.map_.LoadMap(stadiumId, jarInflater.GetData(fileName + ".gmap"));
				instance.map_.SetCoatLine();
				instance.map_.SetDefaultCoatEva();
				instance.map_.stadiumId_ = stadiumId;
			}
			catch (Exception ex)
			{
				throw ex;
			}
			finally
			{
				if (jarInflater != null)
				{
					jarInflater.Close();
				}
				jarInflater = null;
				if (recordStore != null)
				{
					recordStore.CloseRecordStore();
				}
				recordStore = null;
			}
		}

		public void LoadMap(int stadiumId, byte[] source)
		{
			ByteArrayInputStream byteArrayInputStream = null;
			try
			{
				byteArrayInputStream = new ByteArrayInputStream(source);
				MAPCHIP_FIELD_WNUM = StreamUtil.ReadShort(byteArrayInputStream);
				MAPCHIP_FIELD_HNUM = StreamUtil.ReadShort(byteArrayInputStream);
				MAPCHIP_IMG = JTool.MakeArray<int[][]>(new int[2] { MAPCHIP_FIELD_HNUM, MAPCHIP_FIELD_WNUM });
				for (int num = MAPCHIP_FIELD_HNUM - 1; num >= 0; num--)
				{
					for (int i = 0; i < MAPCHIP_FIELD_WNUM; i++)
					{
						MAPCHIP_IMG[num][i] = StreamUtil.ReadShort(byteArrayInputStream);
					}
				}
				COAT_BASEPOINT = new int[2];
				COAT_BASEPOINT[0] = StreamUtil.ReadShort(byteArrayInputStream);
				COAT_BASEPOINT[1] = StreamUtil.ReadShort(byteArrayInputStream);
				StadiumData stadiumData = apdat_.stadiumData_[stadiumId];
				COAT_BASEPOINT[1] += stadiumData.mapchip_oy_;
				int num2 = StreamUtil.ReadShort(byteArrayInputStream);
				mapObjects_ = new MapObject[num2];
				for (int j = 0; j < mapObjects_.Length; j++)
				{
					mapObjects_[j] = new MapObject();
					mapObjects_[j].img_ = StreamUtil.ReadShort(byteArrayInputStream);
					mapObjects_[j].spos_[0] = StreamUtil.ReadShort(byteArrayInputStream);
					mapObjects_[j].spos_[1] = -StreamUtil.ReadShort(byteArrayInputStream);
				}
				byteArrayInputStream.Close();
			}
			catch (Exception t)
			{
				Log.Info(t);
			}
			finally
			{
				try
				{
					if (byteArrayInputStream != null)
					{
						byteArrayInputStream.Close();
					}
					byteArrayInputStream = null;
				}
				catch (Exception t2)
				{
					Log.Info(t2);
				}
			}
		}

		public void Draw(Graphics g, bool offscreen)
		{
			if (offscreen)
			{
				Draw_coat(g, offscreen);
			}
			else
			{
				Draw_coat(g, offscreen);
				Draw_goal(g);
				Draw_lineAndGoal(g);
			}
			Draw_mapobject(g);
			temp_frame++;
		}

		public void Draw_coat(Graphics g, bool offscreen)
		{
			MapchipMatch mapchipMatch = mapchipGrounds[MAP_GROUND];
			Camera.camera_spos[0] = (int)Camera.pos.x_;
			Camera.camera_spos[1] = (int)Camera.pos.y_;
			Camera.ConvertScreenToGlobal(Camera.camera_spos, Camera.camera_gpos);
			int num = 180;
			cameraRect_spos[0][0] = Camera.camera_spos[0] - num;
			cameraRect_spos[0][1] = Camera.camera_spos[1] + num;
			cameraRect_spos[1][0] = Camera.camera_spos[0] + num;
			cameraRect_spos[1][1] = Camera.camera_spos[1] + num;
			cameraRect_spos[2][0] = Camera.camera_spos[0] + num;
			cameraRect_spos[2][1] = Camera.camera_spos[1] - num;
			cameraRect_spos[3][0] = Camera.camera_spos[0] - num;
			cameraRect_spos[3][1] = Camera.camera_spos[1] - num;
			for (int i = 0; i < cameraRect_gpos.Length; i++)
			{
				if (cameraRect_gpos[i] == null)
				{
					cameraRect_gpos[i] = new Vector3D();
				}
				Camera.ConvertScreenToGlobal(cameraRect_spos[i], cameraRect_gpos[i]);
				cameraRectAddress[i][0] = (int)GameUtil.RateConvert(cameraRect_gpos[i].x_, -450f, 450f, 0f, MAPCHIP_FIELD_WNUM - 1, true);
				cameraRectAddress[i][1] = (int)GameUtil.RateConvert(cameraRect_gpos[i].z_, -900f, 900f, 0f, MAPCHIP_FIELD_HNUM - 1, true);
			}
			int num2 = cameraRectAddress[0][0] - 1;
			int num3 = cameraRectAddress[2][0] + 1;
			int num4 = cameraRectAddress[1][1] + 1;
			int num5 = cameraRectAddress[3][1] - 2;
			center_spos[0] = Camera.MAP_SCREEN_W1;
			center_spos[1] = Camera.MAP_SCREEN_H1;
			float num6 = (float)(-COAT_BASEPOINT[0]) - 0.5f;
			float num7 = (float)(-COAT_BASEPOINT[1]) + 0.5f;
			mapchip_off[0] = (int)(num7 * 60f / 2f + num6 * 60f / 2f);
			mapchip_off[1] = (int)(num7 * 30f / 2f - num6 * 30f / 2f);
			int num8 = 0;
			for (int num9 = num4; num9 >= num5; num9--)
			{
				for (int j = num2; j < num3; j++)
				{
					MapchipMatch mapchipMatch2 = null;
					if (num9 < 0 || num9 >= MAPCHIP_SPOS.Length || j < 0 || j >= MAPCHIP_SPOS[0].Length)
					{
						screen_pos[0] = num9 * 60 / 2 + j * 60 / 2 - center_spos[0] + mapchip_off[0];
						screen_pos[1] = num9 * 30 / 2 - j * 30 / 2 - center_spos[1] + mapchip_off[1];
						Camera.ConvertScreenToCamera(screen_pos, camera_pos);
						num8 = camera_pos[1];
						mapchipMatch2 = apdat_.mapchipMatch_[apdat_.stadiumData_[stadiumId_].img_out_];
						Image image = apdat_.resMapChip_.img[mapchipMatch2.img_];
						if (!(GameUtil.ChRectHitRect(camera_pos[0], camera_pos[1] + 29 - image.GetHeight(), 60, image.GetHeight(), SoccerForm.VIEW_X, SoccerForm.VIEW_Y, SoccerForm.VIEW_W, SoccerForm.VIEW_H, false))) { continue; }{
							apdat_.InsertGameObject(4, 0, num8, mapchipMatch2.img_, mapchipMatch2.seb_, 0, camera_pos[0], camera_pos[1], false, 0);
						}
						continue;
					}
					Camera.ConvertScreenToCamera(MAPCHIP_SPOS[num9][j], camera_pos);
					num8 = camera_pos[1];
					if (MAPCHIP_IMG[num9][j] == -1)
					{
						mapchipMatch2 = apdat_.mapchipMatch_[apdat_.stadiumData_[stadiumId_].img_out_];
					}
					else
					{
						Camera.ConvertScreenToCamera(MAPCHIP_SPOS[num9][j], camera_pos);
						mapchipMatch2 = apdat_.mapchipMatch_[MAPCHIP_IMG[num9][j]];
						if (BitUtil.Check(mapchipMatch2.flag_, 1) && !offscreen)
						{
							if (!(GameUtil.ChRectHitRect(camera_pos[0], camera_pos[1], 60, 29, SoccerForm.VIEW_X, SoccerForm.VIEW_Y, SoccerForm.VIEW_W, SoccerForm.VIEW_H, false))) { continue; }{
								Draw_coatLine(g, j, num9, camera_pos[0], camera_pos[1]);
							}
							continue;
						}
						if (BitUtil.Check(mapchipMatch2.flag_, 2))
						{
							mapchipMatch2 = mapchipMatch;
						}
					}
					Image image2 = apdat_.resMapChip_.img[mapchipMatch2.img_];
					if (!(GameUtil.ChRectHitRect(camera_pos[0], camera_pos[1] + 29 - image2.GetHeight(), 60, image2.GetHeight(), SoccerForm.VIEW_X, SoccerForm.VIEW_Y, SoccerForm.VIEW_W, SoccerForm.VIEW_H, false))) { continue; }{
						if (!offscreen)
						{
							Draw_coatLine(g, j, num9, camera_pos[0], camera_pos[1]);
						}
						apdat_.InsertGameObject(4, 0, num8, mapchipMatch2.img_, mapchipMatch2.seb_, 0, camera_pos[0], camera_pos[1], false, 0);
					}
				}
			}
		}

		public void Draw_lineAndGoal(Graphics g)
		{
			int num = -1;
			if (udat_.ball_ != null)
			{
				if (udat_.ball_.owner_ != null)
				{
					num = udat_.ball_.owner_.team_id_;
				}
				else if (udat_.ball_.lastOwner_ != null)
				{
					num = udat_.ball_.lastOwner_.team_id_;
				}
			}
		}

		public void Draw_copyArea(Graphics g, int[] draw_rect)
		{
			bool flag = false;
			int num = 0;
			global::System.Int32 originalUnusedSlot2, recoveredUnusedSlot3;
bool flag2 = false;
int recoveredUnusedSlot5, recoveredUnusedSlot6, recoveredUnusedSlot7, recoveredUnusedSlot8, recoveredUnusedSlot9, recoveredUnusedSlot10, recoveredUnusedSlot11, recoveredUnusedSlot12, width, height;

			global::data.MapchipMatch mapchipMatch = mapchipGrounds[MAP_GROUND];
			int num2 = 0;
int num3 = 0;
			for (int num4 = MAPCHIP_FIELD_HNUM - 1; num4 >= 0; num4--)
			{
				for (int i = 0; i < MAPCHIP_FIELD_WNUM; i++)
				{
					num2++;
					if (MAPCHIP_IMG[num4][i] == -1)
					{
						continue;
					}
					MapchipMatch mapchipMatch2 = apdat_.mapchipMatch_[MAPCHIP_IMG[num4][i]];
					if (BitUtil.Check(mapchipMatch2.flag_, 2))
					{
						mapchipMatch2 = mapchipMatch;
					}
					Image image = apdat_.resMapChip_.img[mapchipMatch2.img_];
					 height = image.GetHeight();
					if (height >= 30)
					{
						continue;
					}
					 width = image.GetWidth();
					Camera.ConvertScreenToCamera(MAPCHIP_SPOS[num4][i], camera_pos);
					flag = false;
					if (draw_rect[0] != 0 && GameUtil.ChRectHitRect(camera_pos[0], camera_pos[1] + 29 - height, width, height, SoccerForm.VIEW_X, SoccerForm.VIEW_Y, SoccerForm.VIEW_W, draw_rect[0], false))
					{
						flag = true;
					}
					if (draw_rect[1] != 0 && !flag)
					{
						if (GameUtil.ChRectHitRect(camera_pos[0], camera_pos[1] + 29 - height, width, height, SoccerForm.VIEW_W - draw_rect[1], SoccerForm.VIEW_Y, draw_rect[1], SoccerForm.VIEW_H, false))
						{
							flag = true;
						}
						if (GameUtil.ChRectHitRect(camera_pos[0], camera_pos[1] + 29 - height, width, height, SoccerForm.VIEW_X, SoccerForm.VIEW_Y, draw_rect[3], SoccerForm.VIEW_H, false))
						{
							flag = true;
						}
					}
					if (draw_rect[2] != 0 && !flag && GameUtil.ChRectHitRect(camera_pos[0], camera_pos[1] + 29 - height, width, height, SoccerForm.VIEW_X, SoccerForm.VIEW_Y + SoccerForm.VIEW_H - draw_rect[2], SoccerForm.VIEW_W, draw_rect[2], false))
					{
						flag = true;
					}
					if (draw_rect[3] != 0 && !flag && GameUtil.ChRectHitRect(camera_pos[0], camera_pos[1] + 29 - height, width, height, SoccerForm.VIEW_X, SoccerForm.VIEW_Y, draw_rect[3], SoccerForm.VIEW_H, false))
					{
						flag = true;
					}
					if (flag)
					{
						num3++;
						apdat_.InsertGameObject(4, 0, camera_pos[1], mapchipMatch2.img_, mapchipMatch2.seb_, 0, camera_pos[0], camera_pos[1], false, 0);
						Draw_coatLine(g, i, num4, camera_pos[0], camera_pos[1]);
					}
				}
			}
		}

		public void Draw_coatLine(Graphics g, int address_x, int address_y, int dx, int dy)
		{
			int num = address_x - COAT_BASEPOINT[0];
			int num2 = address_y - COAT_BASEPOINT[1];
			if (num < 0) { return; }
if (num > MAPCHIP_COAT_WNUM) { return; }
if (num2 < -1) { return; }
if (num2 > MAPCHIP_COAT_HNUM) { return; }
			old_camera_pos[0] = camera_pos[0];
			old_camera_pos[1] = camera_pos[1];
			if (num == 0 && num2 >= 0 && num2 <= MAPCHIP_COAT_HNUM - 1)
			{
				apdat_.InsertGameObject(8, 0, camera_pos[1] + 1, 20, 17, 0, camera_pos[0], camera_pos[1], false, 0);
			}
			else if (num == MAPCHIP_COAT_WNUM && num2 >= 0 && num2 <= MAPCHIP_COAT_HNUM - 1)
			{
				apdat_.InsertGameObject(8, 0, camera_pos[1] + 1, 20, 17, 0, camera_pos[0], camera_pos[1], false, 0);
			}
			else if (num2 == -1 && num >= 0 && num <= MAPCHIP_COAT_WNUM - 1)
			{
				apdat_.InsertGameObject(8, 0, camera_pos[1] + 1, 19, 16, 0, camera_pos[0], camera_pos[1], false, 0);
			}
			else if (num2 == MAPCHIP_COAT_HNUM - 1 && num >= 0 && num <= MAPCHIP_COAT_WNUM)
			{
				apdat_.InsertGameObject(8, 0, camera_pos[1] + 1, 19, 16, 0, camera_pos[0], camera_pos[1], false, 0);
			}
			if (num == 0 && num2 == 0)
			{
				apdat_.InsertGameObject(8, 0, camera_pos[1] + 1, 14, 11, 0, camera_pos[0] - 2, camera_pos[1] + 1, false, 0);
			}
			else if (num == 0 && num2 == MAPCHIP_COAT_HNUM - 1)
			{
				apdat_.InsertGameObject(8, 0, camera_pos[1] + 1, 11, 8, 0, camera_pos[0], camera_pos[1], false, 0);
			}
			else if (num == MAPCHIP_COAT_WNUM - 1 && num2 == 0)
			{
				apdat_.InsertGameObject(8, 0, camera_pos[1] + 1, 13, 10, 0, camera_pos[0], camera_pos[1] + 2, false, 0);
			}
			else if (num == MAPCHIP_COAT_WNUM - 1 && num2 == MAPCHIP_COAT_HNUM - 1)
			{
				apdat_.InsertGameObject(8, 0, camera_pos[1] + 1, 12, 9, 0, camera_pos[0] + 2, camera_pos[1] + 1, false, 0);
			}
			if (num2 == MAPCHIP_COAT_HNUM / 2 - 1 && num >= 0 && num <= MAPCHIP_COAT_WNUM - 1)
			{
				apdat_.InsertGameObject(8, 0, camera_pos[1] + 1, 19, 16, 0, camera_pos[0], camera_pos[1], false, 0);
			}
			if (num2 == MAPCHIP_COAT_HNUM / 2 && num == MAPCHIP_COAT_WNUM / 2 - 1)
			{
				apdat_.InsertGameObject(8, 0, camera_pos[1] + 1, 15, 12, 0, camera_pos[0], camera_pos[1], false, 0);
			}
			else if (num2 == MAPCHIP_COAT_HNUM / 2 && num == MAPCHIP_COAT_WNUM / 2)
			{
				apdat_.InsertGameObject(8, 0, camera_pos[1] + 1, 16, 13, 0, camera_pos[0], camera_pos[1], false, 0);
			}
			else if (num2 == MAPCHIP_COAT_HNUM / 2 - 1 && num == MAPCHIP_COAT_WNUM / 2)
			{
				apdat_.InsertGameObject(8, 0, camera_pos[1] + 1, 17, 14, 0, camera_pos[0], camera_pos[1], false, 0);
			}
			else if (num2 == MAPCHIP_COAT_HNUM / 2 - 1 && num == MAPCHIP_COAT_WNUM / 2 - 1)
			{
				apdat_.InsertGameObject(8, 0, camera_pos[1] + 1, 18, 15, 0, camera_pos[0], camera_pos[1], false, 0);
			}
			if (num == MAPCHIP_COAT_WNUM / 4 && num2 == 2)
			{
				apdat_.InsertGameObject(8, 0, camera_pos[1] + 1, 20, 17, 0, camera_pos[0] - 1, camera_pos[1] + 15, false, 0);
				apdat_.InsertGameObject(8, 0, camera_pos[1] + 1, 19, 16, 0, camera_pos[0] + 1, camera_pos[1] + 15, false, 0);
			}
			else if (num == MAPCHIP_COAT_WNUM * 3 / 4 && num2 == 2)
			{
				apdat_.InsertGameObject(8, 0, camera_pos[1] + 1, 20, 17, 0, camera_pos[0] - 1, camera_pos[1] + 15, false, 0);
				apdat_.InsertGameObject(8, 0, camera_pos[1] + 1, 19, 16, 0, camera_pos[0] - 1 + 1 - 30, camera_pos[1] + 15 - 15, false, 0);
			}
			else if ((num == MAPCHIP_COAT_WNUM / 4 || num == MAPCHIP_COAT_WNUM * 3 / 4) && num2 >= 0 && num2 <= 2)
			{
				apdat_.InsertGameObject(8, 0, camera_pos[1] + 1, 20, 17, 0, camera_pos[0] + 15, camera_pos[1] + 7, false, 0);
			}
			else if (num >= MAPCHIP_COAT_WNUM / 4 && num <= MAPCHIP_COAT_WNUM * 3 / 4 && num2 == 2)
			{
				apdat_.InsertGameObject(8, 0, camera_pos[1] + 1, 19, 16, 0, camera_pos[0] - 15, camera_pos[1] + 7, false, 0);
			}
			if ((num == 4 || num == 7) && num2 == 0)
			{
				apdat_.InsertGameObject(8, 0, camera_pos[1] + 1, 20, 17, 0, camera_pos[0], camera_pos[1], false, 0);
			}
			if (num >= 4 && num <= 6 && num2 == 0)
			{
				apdat_.InsertGameObject(8, 0, camera_pos[1] + 1, 19, 16, 0, camera_pos[0], camera_pos[1], false, 0);
			}
			if (num == MAPCHIP_COAT_WNUM / 4 && num2 == MAPCHIP_COAT_HNUM - 1 - 2)
			{
				apdat_.InsertGameObject(8, 0, camera_pos[1] + 1, 20, 17, 0, camera_pos[0] - 1 + 30, camera_pos[1] + 15 - 15, false, 0);
				apdat_.InsertGameObject(8, 0, camera_pos[1] + 1, 19, 16, 0, camera_pos[0] + 1, camera_pos[1] + 15, false, 0);
			}
			else if (num == MAPCHIP_COAT_WNUM * 3 / 4 && num2 == MAPCHIP_COAT_HNUM - 1 - 2)
			{
				apdat_.InsertGameObject(8, 0, camera_pos[1] + 1, 20, 17, 0, camera_pos[0] - 1 + 30 - 2, camera_pos[1] + 15 - 15 + 1, false, 0);
				apdat_.InsertGameObject(8, 0, camera_pos[1] + 1, 19, 16, 0, camera_pos[0] - 1 + 1 - 30, camera_pos[1] + 15 - 15, false, 0);
			}
			else if ((num == MAPCHIP_COAT_WNUM / 4 || num == MAPCHIP_COAT_WNUM * 3 / 4) && num2 >= MAPCHIP_COAT_HNUM - 2 && num2 <= MAPCHIP_COAT_HNUM - 1)
			{
				apdat_.InsertGameObject(8, 0, camera_pos[1] + 1, 20, 17, 0, camera_pos[0] + 15 - 4, camera_pos[1] + 7 + 2, false, 0);
			}
			else if (num >= MAPCHIP_COAT_WNUM / 4 && num <= MAPCHIP_COAT_WNUM * 3 / 4 && num2 == MAPCHIP_COAT_HNUM - 1 - 2)
			{
				apdat_.InsertGameObject(8, 0, camera_pos[1] + 1, 19, 16, 0, camera_pos[0] - 15 - 1 + 2, camera_pos[1] + 7 + 1, false, 0);
			}
			if ((num == 4 || num == 7) && num2 == MAPCHIP_COAT_HNUM - 1)
			{
				apdat_.InsertGameObject(8, 0, camera_pos[1] + 1, 20, 17, 0, camera_pos[0], camera_pos[1], false, 0);
			}
			if (num >= 4 && num <= 6 && num2 == MAPCHIP_COAT_HNUM - 2)
			{
				apdat_.InsertGameObject(8, 0, camera_pos[1] + 1, 19, 16, 0, camera_pos[0], camera_pos[1], false, 0);
			}
			if ((num == MAPCHIP_COAT_WNUM / 2 - 1 || num == MAPCHIP_COAT_WNUM / 2) && num2 == 2)
			{
				if (num == MAPCHIP_COAT_WNUM / 2)
				{
					Camera.ConvertScreenToCamera(MAPCHIP_SPOS[address_y][address_x - 1], camera_pos);
				}
				apdat_.InsertGameObject(8, 0, camera_pos[1] + 1, 21, 18, 0, camera_pos[0] + 27, camera_pos[1], false, 0);
				camera_pos[0] = old_camera_pos[0];
				camera_pos[1] = old_camera_pos[1];
			}
			if ((num == MAPCHIP_COAT_WNUM / 2 || num == MAPCHIP_COAT_WNUM / 2 + 1) && num2 == 2)
			{
				if (num == MAPCHIP_COAT_WNUM / 2)
				{
					Camera.ConvertScreenToCamera(MAPCHIP_SPOS[address_y][address_x], camera_pos);
				}
				if (num == MAPCHIP_COAT_WNUM / 2 + 1)
				{
					Camera.ConvertScreenToCamera(MAPCHIP_SPOS[address_y][address_x - 1], camera_pos);
				}
				apdat_.InsertGameObject(8, 0, camera_pos[1] + 1, 22, 19, 0, camera_pos[0] + 27, camera_pos[1], false, 0);
				camera_pos[0] = old_camera_pos[0];
				camera_pos[1] = old_camera_pos[1];
			}
			if ((num == MAPCHIP_COAT_WNUM / 2 - 1 || num == MAPCHIP_COAT_WNUM / 2) && num2 == MAPCHIP_COAT_HNUM - 3)
			{
				if (num == MAPCHIP_COAT_WNUM / 2)
				{
					Camera.ConvertScreenToCamera(MAPCHIP_SPOS[address_y][address_x - 1], camera_pos);
				}
				apdat_.InsertGameObject(8, 0, camera_pos[1] + 1, 23, 20, 0, camera_pos[0] + 1 + 2, camera_pos[1] + 7 + 1, false, 0);
				camera_pos[0] = old_camera_pos[0];
				camera_pos[1] = old_camera_pos[1];
			}
			if ((num == MAPCHIP_COAT_WNUM / 2 || num == MAPCHIP_COAT_WNUM / 2 + 1) && num2 == MAPCHIP_COAT_HNUM - 3)
			{
				if (num == MAPCHIP_COAT_WNUM / 2)
				{
					Camera.ConvertScreenToCamera(MAPCHIP_SPOS[address_y][address_x], camera_pos);
				}
				if (num == MAPCHIP_COAT_WNUM / 2 + 1)
				{
					Camera.ConvertScreenToCamera(MAPCHIP_SPOS[address_y][address_x - 1], camera_pos);
				}
				apdat_.InsertGameObject(8, 0, camera_pos[1] + 1, 24, 21, 0, camera_pos[0] + 15 + 4 + 2 - 3, camera_pos[1] + 7 + 1 - 7, false, 0);
				camera_pos[0] = old_camera_pos[0];
				camera_pos[1] = old_camera_pos[1];
			}
			camera_pos[0] = old_camera_pos[0];
			camera_pos[1] = old_camera_pos[1];
		}

		public void Draw_miniMap(Graphics g)
		{

int sideSoftWidth, num, num2, num3, num4, num5, num7, num8, num10, num9;
sideSoftWidth = GameView.GetInstance().GetSideSoftWidth();
			num = 80;
			num2 = 40;
			num3 = SoccerForm.VIEW_X + SoccerForm.VIEW_W - 6 - num - sideSoftWidth;
			num4 = SoccerForm.VIEW_Y + SoccerForm.VIEW_H - 6 - num2;
			num5 = 4;
			g.DrawImage(apdat_.resCom2_.img[92], num3, num4);




for (int num6 = udat_.teams_.Length - 1; num6 >= 0; num6--)
			{
				for (int i = 0; i < udat_.teams_[num6].players_.Length; i++)
				{
					Player player = udat_.teams_[num6].players_[i];
					num7 = (int)GameUtil.RateConvert(player.pos_.z_, 900f, -900f, num3 + num, num3, false);
					num8 = (int)GameUtil.RateConvert(player.pos_.x_, -450f, 450f, num4, num4 + num2, false);
					g.DrawImage(apdat_.resCom2_.img[93], num7 - 2, num8 - 2, 0 + num6 * 5, 0, 4, 4);
				}
			}
			if (SoccerForm.SHOW_DEBUGDRAW2 == 1)
			{
				for (int j = 0; j < udat_.teams_.Length; j++)
				{
					if (j == 0)
					{
						g.SetColor(Graphics.GetColorOfRGB(255, 255, 0));
					}
					else
					{
						g.SetColor(Graphics.GetColorOfRGB(255, 0, 0));
					}
					num7 = (int)GameUtil.RateConvert(udat_.teams_[j].fPos_[1], 900f, -900f, num3 + num, num3, false);
					num8 = (int)GameUtil.RateConvert(udat_.teams_[j].fPos_[0], -450f, 450f, num4, num4 + num2, false);
					num9 = (int)GameUtil.RateConvert(udat_.teams_[j].fPos_[2], 0f, 900f, 0f, num2, false);
					num10 = (int)GameUtil.RateConvert(udat_.teams_[j].fPos_[3], 0f, 1800f, 0f, num, false);
					g.DrawRect(num7 - num10 / 2, num8 - num9 / 2, num10, num9);
				}
			}
			Ball ball_ = udat_.ball_;
			num7 = (int)GameUtil.RateConvert(ball_.pos_.z_, 900f, -900f, num3 + num, num3, false);
			num8 = (int)GameUtil.RateConvert(ball_.pos_.x_, -450f, 450f, num4, num4 + num2, false);
			g.DrawImage(apdat_.resCom2_.img[93], num7 - 2, num8 - 2, 10, 0, 4, 4);
		}

		public void Draw_goal(Graphics g)
		{
			int num = -13;
			int num2 = 17;
			for (int i = 0; i < GOAL1.Length; i++)
			{
				Camera.ConvertScreenToCamera(GOAL1[i], camera_pos);
			}
			for (int j = 0; j < GOAL1_PARTS_POS.Length; j++)
			{
				int x = camera_pos[0] + GOAL1_PARTS_POS[j][0] + num;
				int y = camera_pos[1] + GOAL1_PARTS_POS[j][1] + num2;
				int depth = camera_pos[1] + GOAL1_PARTS_POS[j][1] + num2 + GOAL1_PARTS_GRA[j][2];
				apdat_.InsertGameObject(8, 0, depth, GOAL1_PARTS_GRA[j][0], GOAL1_PARTS_GRA[j][1], 0, x, y, false, 0);
			}
			num = 12;
			num2 = 1;
			for (int k = 0; k < GOAL2.Length; k++)
			{
				Camera.ConvertScreenToCamera(GOAL2[k], camera_pos);
			}
			for (int l = 0; l < GOAL2_PARTS_POS.Length; l++)
			{
				int x = camera_pos[0] + GOAL2_PARTS_POS[l][0] + num;
				int y = camera_pos[1] + GOAL2_PARTS_POS[l][1] + num2;
				int depth = camera_pos[1] + GOAL2_PARTS_POS[l][1] + num2 + GOAL2_PARTS_GRA[l][2];
				apdat_.InsertGameObject(8, 0, depth, GOAL2_PARTS_GRA[l][0], GOAL2_PARTS_GRA[l][1], 0, x, y, false, 0);
			}
		}

		public void Draw_goalPoint(Graphics g)
		{
			for (int i = 0; i < GOAL1_GLOBAL_POS.Length; i++)
			{
				Camera.ConvertGlobalToCamera(GOAL1_GLOBAL_POS[i], GOAL1_CAMERA_POS[i]);
				Camera.ConvertGlobalToCamera(GOAL2_GLOBAL_POS[i], GOAL2_CAMERA_POS[i]);
			}
			int num = 4;
			g.SetColor(Graphics.GetColorOfRGB(255, 0, 0));
			for (int j = 0; j < GOAL1_CAMERA_POS.Length; j++)
			{
				g.FillRect(GOAL1_CAMERA_POS[j][0] - num / 2, GOAL1_CAMERA_POS[j][1] - num / 2, num, num);
				g.FillRect(GOAL2_CAMERA_POS[j][0] - num / 2, GOAL2_CAMERA_POS[j][1] - num / 2, num, num);
			}
		}

		public void Draw_mapobject(Graphics g)
		{
			for (int i = 0; i < mapObjects_.Length; i++)
			{
				Camera.ConvertScreenToCamera(mapObjects_[i].spos_, camera_pos);
				Image image = apdat_.resMapObject_.img[mapObjects_[i].img_];
				if (!(GameUtil.ChRectHitRect(camera_pos[0], camera_pos[1], image.GetWidth(), image.GetHeight(), SoccerForm.VIEW_X, SoccerForm.VIEW_Y, SoccerForm.VIEW_W, SoccerForm.VIEW_H, false))) { continue; }
apdat_.InsertGameObject(7, 0, camera_pos[1] + image.GetHeight(), mapObjects_[i].img_, 0, 0, camera_pos[0], camera_pos[1], false, 0);
			}
		}

		public int GetGoalPartsDepth(int parts_id)
		{
			if ((parts_id == 3)) { if (udat_.ball_.pos_.z_ < GOAL1_GLOBAL_POS[2].z_)
				{
					return udat_.ball_.camera_pos_[1] - 1;
				} }
			return 0;
		}
	}
}
