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

namespace game
{
	public class MapMain
	{
		public const int STEP_W = 60;

		public const int STEP_H = 30;

		public const int MAID_COAT1 = 0;

		public const int MAID_COAT2 = 1;

		public const int MAID_COAT3 = 2;

		public const int MAID_COAT4 = 3;

		public const int MAID_BUS_GO = 4;

		public const int MAID_BUS_STOP = 5;

		public const int MAID_SVISOR_P1 = 6;

		public const int MAID_SVISOR_P2 = 7;

		public const int MAID_END = 8;

		public const int MP_BUSGO = 0;

		public const int MP_BUSSTOP = 1;

		public const int MP_BUSSTOP2 = 2;

		public const int MP_BUSSTOP_C = 3;

		public const int MP_BUSSTOP_C2 = 4;

		public const int MP_BUSSTOP_R = 5;

		public const int MP_BUSSTOP_L = 6;

		public const int MP_TENANT = 7;

		public const int MP_SVISOR_P1 = 8;

		public const int MP_SVISOR_P2 = 9;

		public const int MP_MAX = 10;

		public const int MAP_GROUND_NORMAL = 0;

		public const int MAP_GROUND_GROUND = 1;

		public const int MAP_GROUND_SNOW = 2;

		public const int MAP_GROUND_GRAVEL = 3;

		public const int MAP_GROUND_GRASS2 = 4;

		public const int FENCE_NOTHING = -1;

		public const int FENCE_LOAD_OFF = 5;

		public AppData apdat_;

		private UserData udat_;

		public MapObject[] mapObjects_;

		public static int MAPCHIP_FIELD_WNUM;

		public static int MAPCHIP_FIELD_HNUM;

		public static int[][] MAPCHIP_IMG;

		public static int[][] MAPCHIP_IMG_SAVE;

		public static int[][] COAT_BASEPOINT;

		public static int[][][] MAPCHIP_SPOS;

		public static Vector3D[] MA_POS;

		private int[] camera_pos = new int[2];

		private int[][] c_poss = JTool.MakeArray<int[][]>(new int[2] { 4, 2 });

		public static readonly int[] DEFAULT_INIT_CAMERA_POS = new int[2] { 930, -190 };

		public int temp_frame;

		public static Vector3D[] MOVE_POINT;

		public static float TRAIN_COAT_W = 240f;

		public static float TRAIN_COAT_H = 720f;

		public static Vector3D[] TRAIN_COAT_GPOS;

		public static Vector3D[][] TRAIN_COAT_BLOCK;

		public int[][] TRAIN_COAT_CPOS = JTool.MakeArray<int[][]>(new int[2] { 4, 2 });

		public static int[][] GOAL1_PARTS_POS = new int[6][]
		{
			new int[2] { -21, 10 },
			new int[2] { 1, 21 },
			new int[2] { -14, -13 },
			new int[2] { 8, -2 },
			new int[2] { -30, -1 },
			new int[2] { 16, 22 }
		};

		public static int[][] GOAL2_PARTS_POS = new int[6][]
		{
			new int[2] { -3, -6 },
			new int[2] { 19, 5 },
			new int[2] { -8, -20 },
			new int[2] { 12, -10 },
			new int[2] { -19, -11 },
			new int[2] { 26, 11 }
		};

		public static int[][] GOAL1_PARTS_GRA = new int[6][]
		{
			new int[7] { 43, 39, 0, 28, 36, 1, -1 },
			new int[7] { 43, 39, 0, 28, 36, 1, -1 },
			new int[7] { 45, 41, 10, 30, 17, 1, -1 },
			new int[7] { 45, 41, 10, 30, 17, 1, -1 },
			new int[7] { 44, 40, -5, 15, 29, -1, 1 },
			new int[7] { 44, 40, 0, 15, 29, 1, -1 }
		};

		public static int[][] GOAL2_PARTS_GRA = new int[6][]
		{
			new int[7] { 46, 42, -6, 29, 31, -1, 1 },
			new int[7] { 46, 42, -10, 29, 31, -1, 1 },
			new int[7] { 45, 41, 18, 27, 18, 1, -1 },
			new int[7] { 45, 41, 18, 27, 18, 1, -1 },
			new int[7] { 47, 43, 0, 16, 26, -1, 1 },
			new int[7] { 47, 43, 0, 16, 26, 1, -1 }
		};

		public static readonly string[] GOAL_PARTS_NAME = new string[0];

		public static int MAP_GROUND;

		public static readonly string[] MAP_GROUND_NAME = new string[0];

		public static MapchipMain[] mapchipGrounds;

		public static MapchipMain mapchip_outMap;

		public static MapchipMain mapchip_vacant;

		public static MapchipMain mapchip_tree;

		public static MapchipMain mapchip_fenceGrass;

		public static readonly int[][] FENCE_DATA = new int[10][]
		{
			new int[3] { 30, -46, 46 },
			new int[3] { 30, -46, 47 },
			new int[3] { 0, -46, 48 },
			new int[3] { 0, -46, 49 },
			new int[3] { 30, -31, 50 },
			new int[3] { 30, -45, 51 },
			new int[3] { 30, -45, 52 },
			new int[3] { 30, -45, 53 },
			new int[3] { 1, -9, 54 },
			new int[3] { 5, -2, 55 }
		};

		public static readonly int[][] SET_TMCHP_OFF = new int[4][]
		{
			new int[2],
			new int[2] { 0, -1 },
			new int[2] { 1, 0 },
			new int[2] { 1, -1 }
		};

		internal int[] hit_rect = new int[4];

		public MapMain()
		{
			apdat_ = AppData.GetInstance();
			udat_ = UserData.GetInstance();
			MA_POS = new Vector3D[8];
			for (int i = 0; i < MA_POS.Length; i++)
			{
				MA_POS[i] = new Vector3D();
			}
		}

		public void Init()
		{
			MAP_GROUND = 0;
		}

		public static void CreateMap()
		{
			AppData instance = AppData.GetInstance();
			RecordStore recordStore = null;
			JarInflater jarInflater = null;
			try
			{
				recordStore = RecordStore.OpenRecordStore(1);
				jarInflater = new JarInflater(recordStore.ReadRecord(2));
				instance.mapMain_ = new MapMain();
				instance.mapMain_.Init();
				string text = "test";
				instance.mapMain_.LoadMap(jarInflater.GetData(text + ".gmap"));
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

		public void LoadMap(byte[] source)
		{
			ByteArrayInputStream byteArrayInputStream = null;
			try
			{
				byteArrayInputStream = new ByteArrayInputStream(source);
				MAPCHIP_FIELD_WNUM = StreamUtil.ReadShort(byteArrayInputStream);
				MAPCHIP_FIELD_HNUM = StreamUtil.ReadShort(byteArrayInputStream);
				MAPCHIP_IMG_SAVE = JTool.MakeArray<int[][]>(new int[2] { MAPCHIP_FIELD_HNUM, MAPCHIP_FIELD_WNUM });
				MAPCHIP_IMG = JTool.MakeArray<int[][]>(new int[2] { MAPCHIP_FIELD_HNUM, MAPCHIP_FIELD_WNUM });
				for (int num = MAPCHIP_FIELD_HNUM - 1; num >= 0; num--)
				{
					for (int i = 0; i < MAPCHIP_FIELD_WNUM; i++)
					{
						MAPCHIP_IMG_SAVE[num][i] = StreamUtil.ReadShort(byteArrayInputStream);
					}
				}
				for (int num2 = MAPCHIP_FIELD_HNUM - 1; num2 >= 0; num2--)
				{
					for (int j = 0; j < MAPCHIP_FIELD_WNUM; j++)
					{
						MAPCHIP_IMG[num2][j] = MAPCHIP_IMG_SAVE[num2][j];
					}
				}
				COAT_BASEPOINT = JTool.MakeArray<int[][]>(new int[2] { 8, 2 });
				for (int k = 0; k < COAT_BASEPOINT.Length; k++)
				{
					COAT_BASEPOINT[k][0] = StreamUtil.ReadShort(byteArrayInputStream);
					COAT_BASEPOINT[k][1] = StreamUtil.ReadShort(byteArrayInputStream);
					COAT_BASEPOINT[k][1] = MAPCHIP_FIELD_HNUM - 1 - COAT_BASEPOINT[k][1];
				}
				MOVE_POINT = new Vector3D[10];
				for (int l = 0; l < MOVE_POINT.Length; l++)
				{
					MOVE_POINT[l] = new Vector3D();
				}
				Camera.ConvertAddressToGlobal(COAT_BASEPOINT[4][0], COAT_BASEPOINT[4][1], MOVE_POINT[0]);
				Camera.ConvertAddressToGlobal(COAT_BASEPOINT[5][0], COAT_BASEPOINT[5][1], MOVE_POINT[1]);
				MOVE_POINT[3].Set(MOVE_POINT[1].x_, MOVE_POINT[1].y_, MOVE_POINT[1].z_ + 270f);
				MOVE_POINT[0].z_ += 1800f;
				MOVE_POINT[1].z_ += 1800f;
				MOVE_POINT[3].z_ += 1800f;
				MOVE_POINT[5].Set(MOVE_POINT[3].x_ + 200f, 0f, MOVE_POINT[3].z_);
				MOVE_POINT[6].Set(MOVE_POINT[3].x_ - 200f, 0f, MOVE_POINT[3].z_);
				MOVE_POINT[7].Set(MOVE_POINT[3].x_, 0f, MOVE_POINT[3].z_ + 200f);
				MOVE_POINT[0].z_ += -50f;
				MOVE_POINT[1].z_ += -50f;
				MOVE_POINT[1].x_ += -20f;
				MOVE_POINT[2].Set(MOVE_POINT[1].x_ + 20f, MOVE_POINT[1].y_, MOVE_POINT[1].z_);
				MOVE_POINT[4].Set(MOVE_POINT[3].x_ + 20f, MOVE_POINT[3].y_, MOVE_POINT[3].z_);
				Camera.ConvertAddressToGlobal(COAT_BASEPOINT[6][0], COAT_BASEPOINT[6][1], MOVE_POINT[8]);
				Camera.ConvertAddressToGlobal(COAT_BASEPOINT[7][0], COAT_BASEPOINT[7][1], MOVE_POINT[9]);
				MOVE_POINT[8].z_ += 1800f;
				MOVE_POINT[9].z_ += 1800f;
				TRAIN_COAT_GPOS = new Vector3D[4];
				for (int m = 0; m < TRAIN_COAT_GPOS.Length; m++)
				{
					TRAIN_COAT_GPOS[m] = new Vector3D();
				}
				Camera.ConvertAddressToGlobal(COAT_BASEPOINT[0][0], COAT_BASEPOINT[0][1], TRAIN_COAT_GPOS[0]);
				TRAIN_COAT_GPOS[0].z_ += 1800f;
				TRAIN_COAT_GPOS[1].Set(TRAIN_COAT_GPOS[0].x_ + TRAIN_COAT_W, 0f, TRAIN_COAT_GPOS[0].z_);
				TRAIN_COAT_GPOS[2].Set(TRAIN_COAT_GPOS[0].x_ + TRAIN_COAT_W, 0f, TRAIN_COAT_GPOS[0].z_ - TRAIN_COAT_H);
				TRAIN_COAT_GPOS[3].Set(TRAIN_COAT_GPOS[0].x_, 0f, TRAIN_COAT_GPOS[0].z_ - TRAIN_COAT_H);
				TRAIN_COAT_BLOCK = JTool.MakeArray<Vector3D[][]>(new int[2] { 4, 2 });
				for (int n = 0; n < TRAIN_COAT_BLOCK.Length; n++)
				{
					TRAIN_COAT_BLOCK[n][0] = new Vector3D();
					TRAIN_COAT_BLOCK[n][1] = new Vector3D();
				}
				Camera.ConvertAddressToGlobal(COAT_BASEPOINT[0][0], COAT_BASEPOINT[0][1], TRAIN_COAT_BLOCK[0][0]);
				Camera.ConvertAddressToGlobal(COAT_BASEPOINT[2][0], COAT_BASEPOINT[2][1], TRAIN_COAT_BLOCK[2][0]);
				TRAIN_COAT_BLOCK[0][0].z_ += 1800f;
				TRAIN_COAT_BLOCK[2][0].z_ += 1800f;
				TRAIN_COAT_BLOCK[0][1].Set(TRAIN_COAT_BLOCK[0][0].x_ + TRAIN_COAT_W, 0f, TRAIN_COAT_BLOCK[0][0].z_ - TRAIN_COAT_H / 2f);
				TRAIN_COAT_BLOCK[1][0].Set(TRAIN_COAT_BLOCK[0][0].x_, 0f, TRAIN_COAT_BLOCK[0][0].z_ - TRAIN_COAT_H / 2f);
				TRAIN_COAT_BLOCK[1][1].Set(TRAIN_COAT_BLOCK[0][0].x_ + TRAIN_COAT_W, 0f, TRAIN_COAT_BLOCK[0][0].z_ - TRAIN_COAT_H);
				TRAIN_COAT_BLOCK[2][1].Set(TRAIN_COAT_BLOCK[2][0].x_ + TRAIN_COAT_W, 0f, TRAIN_COAT_BLOCK[2][0].z_ - TRAIN_COAT_H / 2f);
				TRAIN_COAT_BLOCK[3][0].Set(TRAIN_COAT_BLOCK[2][0].x_, 0f, TRAIN_COAT_BLOCK[2][0].z_ - TRAIN_COAT_H / 2f);
				TRAIN_COAT_BLOCK[3][1].Set(TRAIN_COAT_BLOCK[2][0].x_ + TRAIN_COAT_W, 0f, TRAIN_COAT_BLOCK[2][0].z_ - TRAIN_COAT_H);
				MAPCHIP_SPOS = JTool.MakeArray<int[][][]>(new int[3] { MAPCHIP_FIELD_HNUM, MAPCHIP_FIELD_WNUM, 2 });
				for (int num3 = MAPCHIP_SPOS.Length - 1; num3 >= 0; num3--)
				{
					for (int num4 = 0; num4 < MAPCHIP_SPOS[num3].Length; num4++)
					{
						MAPCHIP_SPOS[num3][num4][0] = num3 * 60 / 2 + num4 * 60 / 2;
						MAPCHIP_SPOS[num3][num4][1] = num3 * 30 / 2 - num4 * 30 / 2;
					}
				}
				int num5 = StreamUtil.ReadShort(byteArrayInputStream);
				int[][] array = JTool.MakeArray<int[][]>(new int[2] { num5, 3 });
				for (int num6 = 0; num6 < array.Length; num6++)
				{
					array[num6][0] = StreamUtil.ReadShort(byteArrayInputStream);
					array[num6][1] = StreamUtil.ReadShort(byteArrayInputStream);
					array[num6][2] = StreamUtil.ReadShort(byteArrayInputStream);
					array[num6][1] = MAPCHIP_FIELD_HNUM - 1 - array[num6][1];
					udat_.tenants_[num6].address_[0] = array[num6][0];
					udat_.tenants_[num6].address_[1] = array[num6][1];
				}
				int num7 = StreamUtil.ReadShort(byteArrayInputStream);
				mapObjects_ = new MapObject[num7];
				for (int num8 = 0; num8 < mapObjects_.Length; num8++)
				{
					mapObjects_[num8] = new MapObject();
					mapObjects_[num8].img_ = StreamUtil.ReadShort(byteArrayInputStream);
					mapObjects_[num8].spos_[0] = StreamUtil.ReadShort(byteArrayInputStream);
					mapObjects_[num8].spos_[1] = -StreamUtil.ReadShort(byteArrayInputStream);
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
			UpdateTenantMapcip();
		}

		public static void Load2()
		{
			AppData instance = AppData.GetInstance();
			mapchipGrounds = new MapchipMain[5];
			int[] array = new int[4] { 8, 16, 32, 64 };
			for (int i = 0; i < array.Length; i++)
			{
				for (int j = 0; j < instance.mapchipMain_.Length; j++)
				{
					MapchipMain mapchipMain = instance.mapchipMain_[j];
					if (BitUtil.Check(mapchipMain.flag_, array[i]))
					{
						mapchipGrounds[i] = mapchipMain;
						break;
					}
				}
			}
			mapchip_outMap = null;
			for (int k = 0; k < instance.mapchipMain_.Length; k++)
			{
				MapchipMain mapchipMain2 = instance.mapchipMain_[k];
				if (BitUtil.Check(mapchipMain2.flag_, 2))
				{
					mapchip_outMap = mapchipMain2;
					break;
				}
			}
			for (int l = 0; l < instance.mapchipMain_.Length; l++)
			{
				MapchipMain mapchipMain3 = instance.mapchipMain_[l];
				if (BitUtil.Check(mapchipMain3.flag_, 128))
				{
					mapchip_vacant = mapchipMain3;
					break;
				}
			}
			for (int m = 0; m < instance.mapchipMain_.Length; m++)
			{
				MapchipMain mapchipMain4 = instance.mapchipMain_[m];
				if (BitUtil.Check(mapchipMain4.flag_, 256))
				{
					mapchip_tree = mapchipMain4;
					break;
				}
			}
			for (int n = 0; n < instance.mapchipMain_.Length; n++)
			{
				MapchipMain mapchipMain5 = instance.mapchipMain_[n];
				if (BitUtil.Check(mapchipMain5.flag_, 512))
				{
					mapchip_fenceGrass = mapchipMain5;
					break;
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
			}
			temp_frame++;
		}

		public void Draw_coat(Graphics g, bool offscreen)
		{
			MapchipMain mapchipMain = null;
			mapchipMain = ((MAP_GROUND != 0) ? mapchipGrounds[MAP_GROUND - 1] : null);
			for (int num = MAPCHIP_FIELD_HNUM - 1; num >= 0; num--)
			{
				for (int i = 0; i < MAPCHIP_FIELD_WNUM; i++)
				{
					int num2 = -1;
					Camera.ConvertScreenToCamera(MAPCHIP_SPOS[num][i], camera_pos);
					MapchipMain mapchipMain2 = null;
					if (MAPCHIP_IMG[num][i] == -1)
					{
						mapchipMain2 = mapchip_outMap;
					}
					else
					{
						Camera.ConvertScreenToCamera(MAPCHIP_SPOS[num][i], camera_pos);
						mapchipMain2 = apdat_.mapchipMain_[MAPCHIP_IMG[num][i]];
						num2 = mapchipMain2.fenceId_;
						if (udat_.tAreaLv_ < 2)
						{
							switch (num2)
							{
							case 0:
								mapchipMain2 = apdat_.mapchipMain_[MAPCHIP_IMG[num][i] + 1];
								break;
							case 3:
								mapchipMain2 = apdat_.mapchipMain_[MAPCHIP_IMG[num][i] - 1];
								break;
							case 6:
								num2 = 5;
								break;
							}
						}
						if (BitUtil.Check(mapchipMain2.flag_, 1))
						{
							if (!offscreen)
							{
								continue;
							}
						}
						else if (offscreen)
						{
							continue;
						}
						if (mapchipMain != null && (BitUtil.Check(mapchipMain2.flag_, 4) || num2 >= 5))
						{
							mapchipMain2 = mapchipMain;
						}
						else if (num2 == 5)
						{
							mapchipMain2 = mapchip_fenceGrass;
						}
					}
					Image image = apdat_.resMapChipMain_.img[mapchipMain2.img_];
					if (GameUtil.ChRectHitRect(camera_pos[0], camera_pos[1] + 29 - image.GetHeight(), 60, image.GetHeight(), GameForm.VIEW_X, GameForm.VIEW_Y, GameForm.VIEW_W, GameForm.VIEW_H, false))
					{
						apdat_.InsertGameObject(5, 0, camera_pos[1], mapchipMain2.img_, mapchipMain2.seb_, 0, camera_pos[0], camera_pos[1], false, 0);
						if (num2 >= 5)
						{
							apdat_.InsertGameObject(6, 0, camera_pos[1] + 1, FENCE_DATA[num2][2], 0, 0, camera_pos[0] + FENCE_DATA[num2][0], camera_pos[1] + FENCE_DATA[num2][1], false, 0);
						}
					}
				}
			}
		}

		public void Draw_copyArea(Graphics g, int[] draw_rect)
		{
			bool flag = false;
			int num = 0;
			bool flag2 = false;
			MapchipMain mapchipMain = null;
			mapchipMain = ((MAP_GROUND != 0) ? mapchipGrounds[MAP_GROUND - 1] : null);
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
					MapchipMain mapchipMain2 = apdat_.mapchipMain_[MAPCHIP_IMG[num4][i]];
					if (mapchipMain != null && BitUtil.Check(mapchipMain2.flag_, 4))
					{
						mapchipMain2 = mapchipMain;
					}
					Image image = apdat_.resMapChipMain_.img[mapchipMain2.img_];
					int height = image.GetHeight();
					if (height >= 30)
					{
						continue;
					}
					int width = image.GetWidth();
					Camera.ConvertScreenToCamera(MAPCHIP_SPOS[num4][i], camera_pos);
					flag = false;
					if (draw_rect[0] != 0 && GameUtil.ChRectHitRect(camera_pos[0], camera_pos[1] + 29 - height, width, height, GameForm.VIEW_X, GameForm.VIEW_Y, GameForm.VIEW_W, draw_rect[0], false))
					{
						flag = true;
					}
					if (draw_rect[1] != 0 && !flag)
					{
						if (GameUtil.ChRectHitRect(camera_pos[0], camera_pos[1] + 29 - height, width, height, GameForm.VIEW_W - draw_rect[1], GameForm.VIEW_Y, draw_rect[1], GameForm.VIEW_H, false))
						{
							flag = true;
						}
						if (GameUtil.ChRectHitRect(camera_pos[0], camera_pos[1] + 29 - height, width, height, GameForm.VIEW_X, GameForm.VIEW_Y, draw_rect[3], GameForm.VIEW_H, false))
						{
							flag = true;
						}
					}
					if (draw_rect[2] != 0 && !flag && GameUtil.ChRectHitRect(camera_pos[0], camera_pos[1] + 29 - height, width, height, GameForm.VIEW_X, GameForm.VIEW_Y + GameForm.VIEW_H - draw_rect[2], GameForm.VIEW_W, draw_rect[2], false))
					{
						flag = true;
					}
					if (draw_rect[3] != 0 && !flag && GameUtil.ChRectHitRect(camera_pos[0], camera_pos[1] + 29 - height, width, height, GameForm.VIEW_X, GameForm.VIEW_Y, draw_rect[3], GameForm.VIEW_H, false))
					{
						flag = true;
					}
					if (flag)
					{
						num3++;
						apdat_.InsertGameObject(5, 0, camera_pos[1], mapchipMain2.img_, mapchipMain2.seb_, 0, camera_pos[0], camera_pos[1], false, 0);
					}
				}
			}
		}

		public void Draw_trainingCoat(Graphics g)
		{
			for (int i = 0; i < TRAIN_COAT_GPOS.Length; i++)
			{
				Camera.ConvertGlobalToCamera(TRAIN_COAT_GPOS[i], TRAIN_COAT_CPOS[i]);
			}
			g.SetColor(Graphics.GetColorOfRGB(0, 255, 0));
			for (int j = 0; j < TRAIN_COAT_CPOS.Length; j++)
			{
				g.FillRect(TRAIN_COAT_CPOS[j][0] - 4, TRAIN_COAT_CPOS[j][1] - 4, 8f, 8f);
			}
			g.SetColor(Graphics.GetColorOfRGB(0, 0, 255));
			for (int k = 0; k < TRAIN_COAT_CPOS.Length; k++)
			{
				g.DrawLine(TRAIN_COAT_CPOS[k][0], TRAIN_COAT_CPOS[k][1], TRAIN_COAT_CPOS[(k + 1) % 4][0], TRAIN_COAT_CPOS[(k + 1) % 4][1]);
			}
			for (int l = 0; l < udat_.tAreas_.Length; l++)
			{
				for (int m = 0; m < udat_.tAreas_[l].Length; m++)
				{
					if (udat_.tAreas_[l][m].tMode_ == 4 || udat_.tAreas_[l][m].tMode_ == 5)
					{
						continue;
					}
					for (int n = 0; n < udat_.tAreas_[l][m].tGroupAreas_.Length; n++)
					{
						TGroupArea tGroupArea = udat_.tAreas_[l][m].tGroupAreas_[n];
						for (int num = 0; num < tGroupArea.filed_gpos.Length; num++)
						{
							Camera.ConvertGlobalToCamera(tGroupArea.filed_gpos[num], tGroupArea.filed_cpos[num]);
						}
						for (int num2 = 0; num2 < tGroupArea.filed_gpos.Length; num2++)
						{
							tGroupArea.cx[num2] = tGroupArea.filed_cpos[num2][0];
							tGroupArea.cy[num2] = tGroupArea.filed_cpos[num2][1];
						}
						int r = GameUtil.RateConvert(l, 0, udat_.tAreas_.Length, 0, 255, true);
						int g2 = GameUtil.RateConvert(n, 0, udat_.tAreas_[l][m].tGroupAreas_.Length, 0, 255, true);
						g.SetColor(Graphics.GetColorOfRGB(r, g2, 255));
					}
					if (udat_.tAreas_[l][m].tMode_ != 3)
					{
						continue;
					}
					for (int num3 = 0; num3 < udat_.tAreas_[l][m].tGroupAreas_.Length; num3++)
					{
						TGroupArea tGroupArea2 = udat_.tAreas_[l][m].tGroupAreas_[num3];
						for (int num4 = 0; num4 < tGroupArea2.goal_gpos.Length; num4++)
						{
							Camera.ConvertGlobalToCamera(tGroupArea2.goal_gpos[num4], tGroupArea2.goal_cpos[num4]);
							int num5 = 6;
							g.SetColor(Graphics.GetColorOfRGB(255, 255, 0));
							g.FillRect(tGroupArea2.goal_cpos[num4][0] - num5 / 2, tGroupArea2.goal_cpos[num4][1] - num5 / 2, num5, num5);
						}
						for (int num6 = 0; num6 < tGroupArea2.goal_gpos.Length - 1; num6++)
						{
							g.SetColor(Graphics.GetColorOfRGB(255, 0, 0));
							g.DrawLine(tGroupArea2.goal_cpos[num6][0], tGroupArea2.goal_cpos[num6][1], tGroupArea2.goal_cpos[(num6 + 1) % 4][0], tGroupArea2.goal_cpos[(num6 + 1) % 4][1]);
						}
					}
				}
			}
		}

		public void UpdateTenantMapcip()
		{
			for (int i = 0; i < udat_.tenants_.Length; i++)
			{
				Tenant tenant = udat_.tenants_[i];
				TenantData tenantData = null;
				if (tenant.state_ == 1 || tenant.state_ == 2)
				{
					tenantData = apdat_.tenantData_[tenant.tenantDataId_];
				}
				if (tenant.state_ == 1)
				{
					for (int j = 0; j < tenantData.img_.Length; j++)
					{
						MAPCHIP_IMG[tenant.address_[1] + SET_TMCHP_OFF[j][1]][tenant.address_[0] + SET_TMCHP_OFF[j][0]] = tenantData.img_[j];
					}
				}
				else
				{
					for (int k = 0; k < 4; k++)
					{
						MAPCHIP_IMG[tenant.address_[1] + SET_TMCHP_OFF[k][1]][tenant.address_[0] + SET_TMCHP_OFF[k][0]] = mapchip_vacant.id_ + k;
					}
				}
			}
			if (udat_.tAreaRootNum_ != 1)
			{
				return;
			}
			for (int l = 5; l <= 15; l++)
			{
				for (int m = 4; m <= 13; m++)
				{
					MAPCHIP_IMG[l][m] = mapchip_tree.id_;
				}
			}
		}

		public void Draw_tenantAround(Graphics g)
		{
			bool flag = false;
			for (int i = 0; i < udat_.tenants_.Length; i++)
			{
				Tenant tenant = udat_.tenants_[i];
				if (tenant.root_ == 1 && udat_.tAreaRootNum_ == 1)
				{
					continue;
				}
				Camera.ConvertScreenToCamera(MAPCHIP_SPOS[tenant.address_[1]][tenant.address_[0]], camera_pos);
				camera_pos[0] -= 30;
				flag = false;
				if ((Camera.camera_address[0] == tenant.address_[0] || Camera.camera_address[0] == tenant.address_[0] + 1) && (Camera.camera_address[1] == tenant.address_[1] || Camera.camera_address[1] == tenant.address_[1] - 1) && apdat_.camera.flg_disp)
				{
					flag = true;
				}
				Image image = null;
				if (flag)
				{
					image = apdat_.resCom2_.img[140];
				}
				else
				{
					image = apdat_.resCom2_.img[142];
				}
				if (GameUtil.ChRectHitRect(camera_pos[0], camera_pos[1], 110, 56, GameForm.VIEW_X, GameForm.VIEW_Y, GameForm.VIEW_W, GameForm.VIEW_H, false))
				{
					if (flag)
					{
						int frame = apdat_.frmGame_.stateCnt_ % 16 / 8;
						apdat_.DrawSeb(g, camera_pos[0], camera_pos[1], apdat_.resCom2_.seb[77], apdat_.resCom2_.img[140], frame);
					}
					else if (apdat_.frmGame_.state_ == 1)
					{
						apdat_.DrawSeb(g, camera_pos[0], camera_pos[1], apdat_.resCom2_.seb[80], apdat_.resCom2_.img[142], 0);
					}
				}
			}
		}

		public void UpdateMapGround()
		{
			int mAP_GROUND = MAP_GROUND;
			if (apdat_.save_.ints[2] == 11 || apdat_.save_.ints[2] == 0)
			{
				MAP_GROUND = 2;
			}
			else if (udat_.tAreaLv_ == 0)
			{
				MAP_GROUND = 1;
			}
			else if (udat_.tAreaLv_ == 1)
			{
				MAP_GROUND = 0;
			}
			else
			{
				MAP_GROUND = 4;
			}
			if (mAP_GROUND != MAP_GROUND)
			{
				apdat_.frmGame_.SetAllDraw();
			}
		}

		public void UpdateTrainingCoatNum()
		{
			for (int num = MAPCHIP_FIELD_HNUM - 1; num >= 0; num--)
			{
				for (int i = 0; i < MAPCHIP_FIELD_WNUM; i++)
				{
					MAPCHIP_IMG[num][i] = MAPCHIP_IMG_SAVE[num][i];
				}
			}
			UpdateTenantMapcip();
			UpdateMapGround();
		}
	}
}
