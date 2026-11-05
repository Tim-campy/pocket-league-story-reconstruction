using System;
using System.Collections.Generic;
using System.Linq;
using java.io;
using java.lang;
using java.util;
using kairo.common.cfg;
using kairo.unity.io;
using kairo.unity.util;

namespace kairo.unity.ui
{
	public class ResourceManager
	{
		private struct JarCache
		{
			public int RsId;

			public int RcId;

			public JarInflater Jar;

			public JarCache(int rsId, int rcId, JarInflater jar)
			{
				RsId = rsId;
				RcId = rcId;
				Jar = jar;
			}
		}

		public const int RES_IMAGE = 0;

		public const int RES_SEB = 1;

		public const int RES_SOUND = 2;

		public const int RES_FIGURE = 3;

		public const int RES_ACT_TABLE = 4;

		public const int RES_TEXTURE = 5;

		public const int RES_GROUP = 6;

		private const int SOUND_MLD = 0;

		private const int SOUND_MMF = 1;

		private const int SOUND_SPF = 2;

		private const int SOUND_MIDI = 3;

		private const int SOUND_WAVE = 4;

		private const int SOUND_JET = 5;

		private const int SOUND_OGG = 6;

		private static readonly string[] SOUND_EXT = new string[7] { "mld", "mmf", "spf", "mid", "wav", "jet", "ogg" };

		public Image[] img;

		public Seb[] seb;

		public SoundBase[] sound;

		public string[][] list_;

		private static readonly string[] LIST_FILE = new string[7] { "img.inf", "seb.inf", "snd.inf", "fig.inf", "act.inf", "tex.inf", "grp.inf" };

		protected static string[] listFile_;

		protected string directory_;

		protected int rsId_ = -1;

		protected int rcId_ = -1;

		protected int loadNum_;

		protected int loadCnt_;

		protected bool loaded_;

		protected int recordNameBits_ = 255;

		private static List<JarCache> jarCache_ = new List<JarCache>();

		private static int yieldTime_ = 150;

		public static int YieldTime
		{
			get
			{
				return yieldTime_;
			}
			set
			{
				yieldTime_ = value;
			}
		}

		public static Image LoadImage(byte[] src, Image.Format format = Image.Format.Default, Image.Filter filterMode = Image.Filter.Default)
		{
			return Image.Load(src, format, filterMode);
		}

		public static Image LoadImage(InputStream stream, Image.Format format = Image.Format.Default, Image.Filter filterMode = Image.Filter.Default)
		{
			return Image.Load(stream, format, filterMode);
		}

		public static Seb LoadSeb(byte[] src)
		{
			return new Seb(src);
		}

		public static Seb LoadSeb(InputStream stream)
		{
			return new Seb(stream);
		}

		public static SoundBase LoadSound(byte[] src, int format)
		{
			throw new Exception("loadSound:バイナリからのロードは対応していません。");
		}

		public static SoundBase LoadSound(InputStream stream, int format)
		{
			throw new Exception("loadSound:ストリームからのロードは対応していません。");
		}

		public static SoundBase LoadSound(string file, int format)
		{
			file = Config.ConvertExtension(file);
			SoundBase result = null;
			if (format == 6)
			{
				result = SoundOGG.Load(file);
			}
			return result;
		}

		public ResourceManager(string imgList = null, string sebList = null, string sndList = null, string figList = null, string actList = null, string texList = null, string grpList = null)
		{
			listFile_ = new string[LIST_FILE.Length];
			JSystem.Arraycopy(LIST_FILE, 0, listFile_, 0, LIST_FILE.Length);
			if (imgList != null)
			{
				listFile_[0] = imgList;
			}
			if (sebList != null)
			{
				listFile_[1] = sebList;
			}
			if (sndList != null)
			{
				listFile_[2] = sndList;
			}
			if (figList != null)
			{
				listFile_[3] = figList;
			}
			if (actList != null)
			{
				listFile_[4] = actList;
			}
			if (texList != null)
			{
				listFile_[5] = texList;
			}
			if (grpList != null)
			{
				listFile_[6] = grpList;
			}
			_init();
		}

		protected void _init()
		{
			_init(false);
		}

		protected void _init(bool keepFileList)
		{
			img = new Image[0];
			seb = new Seb[0];
			sound = new SoundBase[0];
			if (!keepFileList)
			{
				list_ = JTool.MakeArray<string[][]>(new int[2] { LIST_FILE.Length, 0 });
			}
			loadNum_ = (loadCnt_ = 0);
			loaded_ = false;
		}

		public void Load(byte[] data, int m3dVer = 3)
		{
			JarInflater jarInflater = null;
			try
			{
				jarInflater = new JarInflater(data);
				Load(jarInflater, m3dVer);
			}
			finally
			{
				jarInflater?.Close();
				jarInflater = null;
			}
		}

		public void Load(JarInflater jar, int m3dVer = 3)
		{
			LoadReady(jar);
			LoadStart(jar, m3dVer);
		}

		public void Load(string directory, int m3dVer = 3)
		{
			int num = directory.LastIndexOf('.');
			if (num != -1)
			{
				directory = directory.SubstringJ(0, num);
			}
			JarInflater jarInflater = null;
			try
			{
				jarInflater = new JarInflater(directory);
				LoadReady(jarInflater);
				LoadStart(jarInflater, m3dVer);
			}
			finally
			{
				jarInflater?.Close();
				jarInflater = null;
			}
		}

		public void Load(int rsId, int rcId, int m3dVer = 3)
		{
			JarInflater jarInflater = null;
			try
			{
				jarInflater = new JarInflater(RecordStore.ReadRecord(rsId, rcId));
				rsId_ = rsId;
				rcId_ = rcId;
				LoadReady(jarInflater);
				LoadStart(jarInflater, m3dVer);
			}
			finally
			{
				jarInflater?.Close();
				jarInflater = null;
			}
		}

		public void LoadReady(byte[] data)
		{
			JarInflater jarInflater = null;
			try
			{
				jarInflater = new JarInflater(data);
				LoadReady(jarInflater);
			}
			finally
			{
				jarInflater?.Close();
				jarInflater = null;
			}
		}

		public void LoadReady(JarInflater jar)
		{
			if (loaded_)
			{
				return;
			}
			Dispose();
			if (jar.GetDirectory() != null)
			{
				directory_ = "Data/" + jar.GetDirectory();
			}
			loadNum_ = 0;
			for (int i = 0; i < listFile_.Length; i++)
			{
				if (listFile_[i].Length() != 0)
				{
					string[] array = StringUtil.LoadStringArray(jar, listFile_[i]);
					if (array != null)
					{
						list_[i] = array;
					}
					for (int j = 0; j < list_[i].Length; j++)
					{
						list_[i][j] = list_[i][j].Trim();
					}
					loadNum_ += list_[i].Length;
				}
			}
		}

		public void LoadStart(byte[] data, int m3dVer = 3)
		{
			JarInflater jarInflater = null;
			try
			{
				jarInflater = new JarInflater(data);
				LoadStart(jarInflater, m3dVer);
			}
			finally
			{
				jarInflater?.Close();
				jarInflater = null;
			}
		}

		public void LoadStart(JarInflater jar, int m3dVer = 3)
		{
			int[][] array = null;
			if (!loaded_)
			{
				loadCnt_ = 0;
				array = new int[list_.Length][];
				for (int i = 0; i < list_.Length; i++)
				{
					array[i] = new int[list_[i].Length];
					for (int j = 0; j < list_[i].Length; j++)
					{
						array[i][j] = -1;
						string[] array2 = StringUtil.Split(list_[i][j], "\t");
						if (array2.Length >= 2)
						{
							array[i][j] = JInteger.ParseInt(array2[0]);
							list_[i][j] = array2[1];
						}
					}
				}
			}
			if (!loaded_)
			{
				img = new Image[list_[0].Length];
				seb = new Seb[list_[1].Length];
				sound = new SoundBase[list_[2].Length];
			}
			int[][] loadTask = new int[3][]
			{
				new int[2],
				new int[2] { 1, 0 },
				new int[2] { 2, 0 }
			};
			string[] sebFileName = new string[list_[1].Length];
			bool flag;
			do
			{
				IApplication.GetCurrentApp().RunOnUiThread(delegate
				{
					long num17 = JSystem.CurrentTimeMillis();
					for (int k = 0; k < loadTask.Length; k++)
					{
						int num18 = loadTask[k][0];
						for (int l = loadTask[k][1]; l < list_[num18].Length; l++, loadTask[k][1]++, loadCnt_++)
						{
							if (CheckFileEffective(list_[num18][l]))
							{
								int num19 = 0;
								switch (num18)
								{
								case 0:
								{
									string fileName2 = GetFileName(list_[0][l]);
									string[] array7 = StringUtil.Split(fileName2, ",");
									fileName2 = array7[0];
									bool flag3 = false;
									bool flag4 = false;
									string text3 = null;
									Image.Filter filterMode = Image.Filter.Default;
									Image.Format format = Image.Format.Default;
									int renderQueue_ = 0;
									int imageAtlasId_ = -1;
									int width = 0;
									int height = 0;
									bool flag5 = false;
									for (int n = 1; n < array7.Length; n++)
									{
										string text4 = array7[n].ToLower();
										switch (text4)
										{
										case "bin":
											flag3 = true;
											break;
										case "res":
											flag4 = true;
											break;
										case "pt":
											filterMode = Image.Filter.Point;
											break;
										case "bi":
											filterMode = Image.Filter.Bilinear;
											break;
										case "tri":
											filterMode = Image.Filter.Trilinear;
											break;
										case "24":
											format = Image.Format.RGB24;
											break;
										case "565":
											format = Image.Format.RGB565;
											break;
										case "32":
											format = Image.Format.ARGB32;
											break;
										case "4444":
											format = Image.Format.ARGB4444;
											break;
										default:
											if (text4.EndsWith(".pal"))
											{
												text3 = array7[n];
											}
											else if (text4.StartsWith("q+") || text4.StartsWith("q-"))
											{
												text4 = ((text4[1] != '+') ? text4.Substring(1) : text4.Substring(2));
												renderQueue_ = int.Parse(text4);
											}
											else if (text4.StartsWith("@"))
											{
												imageAtlasId_ = ((text4.Length != 1) ? int.Parse(text4.Substring(1)) : 0);
											}
											else if (text4.Contains('x'))
											{
												if (text4[0] == 'e' || text4[0] == 'E')
												{
													text4 = text4.Substring(1);
													if (!Language.English())
													{
														break;
													}
												}
												string[] array8 = text4.Split('x');
												width = int.Parse(array8[0]);
												height = int.Parse(array8[1]);
											}
											else
											{
												flag5 = true;
											}
											break;
										}
									}
									if (Config.PRINT && flag5)
									{
										Log.Info("ｲﾒｰｼﾞのｵﾌﾟｼｮﾝが[bin][res][pt][bi][tri][24][32][565][4444][*.pal][q][@]のどれにも当てはまりません:" + list_[0][l]);
									}
									try
									{
										if (rsId_ != -1)
										{
											img[l] = new Image(rsId_, rcId_, fileName2, text3, format, filterMode);
										}
										else if (flag3)
										{
											if (!loaded_ || jar.Exist(fileName2))
											{
												byte[] src = Image.OverwritePaletteData(jar.GetData(fileName2), (text3 != null) ? jar.GetData(text3) : null);
												img[l] = new Image(src, format, filterMode);
											}
										}
										else if (flag4)
										{
											img[l] = new Image(jar.GetDirectory() + fileName2, text3, format, filterMode);
										}
										else if (!loaded_ || jar.Exist(fileName2))
										{
											byte[] src2 = Image.OverwritePaletteData(jar.GetData(fileName2), (text3 != null) ? jar.GetData(text3) : null);
											img[l] = LoadImage(src2, format, filterMode);
										}
										img[l].SetStandardResolution(width, height);
										if (flag3 && img[l] != null)
										{
											img[l].useBin_ = true;
										}
										if (img[l] != null)
										{
											img[l].imageAtlasId_ = imageAtlasId_;
										}
										img[l].renderQueue_ = renderQueue_;
										if ((recordNameBits_ & 1) != 0 && img[l] != null)
										{
											img[l].SetName(fileName2);
										}
										num19++;
									}
									catch (Exception t2)
									{
										Log.Error("loadImageに失敗:" + fileName2, t2);
									}
									break;
								}
								case 1:
								{
									string fileName3 = GetFileName(list_[1][l]);
									string[] array9 = StringUtil.Split(fileName3, ",");
									bool u = false;
									bool v = false;
									int num21 = 0;
									for (int num22 = 1; num22 < array9.Length; num22++)
									{
										switch (array9[num22].CharAt(0))
										{
										case 'u':
											u = true;
											break;
										case 'v':
											v = true;
											break;
										case 'c':
											num21 = JInteger.ParseInt(array9[num22].Substring(1));
											break;
										}
									}
									fileName3 = array9[0];
									sebFileName[l] = fileName3;
									try
									{
										Seb seb = null;
										for (int num23 = 0; num23 < l; num23++)
										{
											if (fileName3.Equals(sebFileName[num23]))
											{
												seb = this.seb[num23];
												break;
											}
										}
										if (seb == null)
										{
											this.seb[l] = LoadSeb(jar.GetData(fileName3));
										}
										else
										{
											this.seb[l] = new Seb(seb);
										}
										if ((recordNameBits_ & 2) != 0)
										{
											this.seb[l].SetName(fileName3);
										}
										this.seb[l].manager_ = this;
										this.seb[l].Reverse(u, v);
										if (num21 > 0)
										{
											this.seb[l].SetCacheSize(num21);
										}
										num19++;
									}
									catch (Exception t3)
									{
										Log.Error("loadSebに失敗:" + fileName3, t3);
									}
									break;
								}
								case 2:
								{
									string text = Config.ConvertExtension(list_[2][l].ToLowerCase());
									int num20 = -1;
									for (int m = 0; m < SOUND_EXT.Length; m++)
									{
										if (text.EndsWith(SOUND_EXT[m]))
										{
											num20 = m;
											break;
										}
									}
									if (num20 == -1)
									{
										continue;
									}
									try
									{
										string text2 = ((directory_ == null) ? "" : directory_);
										sound[l] = LoadSound(text2 + text, num20);
										if ((recordNameBits_ & 4) != 0)
										{
											sound[l].SetName(text);
										}
										num19++;
									}
									catch (Exception t)
									{
										Log.Error("loadSoundに失敗:" + text, t);
									}
									break;
								}
								}
								if (num19 > 0 && JSystem.CurrentTimeMillis() - num17 >= YieldTime)
								{
									return;
								}
							}
						}
					}
				}, true);
				flag = true;
				for (int num = 0; num < loadTask.Length; num++)
				{
					if (loadTask[num][1] < list_[loadTask[num][0]].Length)
					{
						flag = false;
					}
				}
			}
			while (!flag);
			if (!loaded_)
			{
				for (int num2 = 0; num2 < array.Length; num2++)
				{
					int num3 = array[num2].Length;
					for (int num4 = 0; num4 < array[num2].Length; num4++)
					{
						if (array[num2][num4] + 1 > num3)
						{
							num3 = array[num2][num4] + 1;
						}
					}
					for (int num5 = 0; num5 < array[num2].Length; num5++)
					{
						int num6 = 0;
						while (array[num2][num5] == -1)
						{
							bool flag2 = false;
							for (int num7 = 0; num7 < array[num2].Length; num7++)
							{
								if (array[num2][num7] == num6)
								{
									flag2 = true;
								}
							}
							if (!flag2)
							{
								array[num2][num5] = num6;
							}
							num6++;
						}
					}
					string[] array3 = new string[num3];
					for (int num8 = 0; num8 < list_[num2].Length; num8++)
					{
						array3[array[num2][num8]] = list_[num2][num8];
					}
					list_[num2] = array3;
					switch (num2)
					{
					case 0:
					{
						Image[] array6 = new Image[num3];
						for (int num11 = 0; num11 < img.Length; num11++)
						{
							array6[array[num2][num11]] = img[num11];
						}
						img = array6;
						break;
					}
					case 1:
					{
						Seb[] array5 = new Seb[num3];
						for (int num10 = 0; num10 < seb.Length; num10++)
						{
							array5[array[num2][num10]] = seb[num10];
						}
						seb = array5;
						break;
					}
					case 2:
					{
						SoundBase[] array4 = new SoundBase[num3];
						for (int num9 = 0; num9 < sound.Length; num9++)
						{
							array4[array[num2][num9]] = sound[num9];
						}
						sound = array4;
						break;
					}
					}
				}
			}
			if (img != null)
			{
				for (int num12 = 0; num12 < img.Length; num12++)
				{
					if (img[num12] != null)
					{
						string fileName = GetFileName(list_[0][num12]);
						fileName = StringUtil.Split(fileName, ",")[0];
						fileName = fileName.Substring(0, fileName.Length() - 3) + "opt";
						if (jar.Exist(fileName))
						{
							img[num12].LoadOptimize(jar.GetData(fileName), img);
						}
					}
				}
			}
			List<int[]> atlasIamges = new List<int[]>();
			for (int num13 = 0; num13 < img.Length; num13++)
			{
				if (img[num13] != null && img[num13].imageAtlasId_ != -1)
				{
					int num14 = img[num13].GetHeight() * 100 + img[num13].GetWidth();
					atlasIamges.Add(new int[2] { num13, num14 });
				}
			}
			for (int num15 = 0; num15 < atlasIamges.Count - 1; num15++)
			{
				for (int num16 = atlasIamges.Count - 1; num16 > num15; num16--)
				{
					if (atlasIamges[num16 - 1][1] < atlasIamges[num16][1])
					{
						int[] value = atlasIamges[num16];
						atlasIamges[num16] = atlasIamges[num16 - 1];
						atlasIamges[num16 - 1] = value;
					}
				}
			}
			IApplication.GetCurrentApp().RunOnUiThread(delegate
			{
				for (int k = 0; k < atlasIamges.Count; k++)
				{
					Image obj = img[atlasIamges[k][0]];
					obj.SetImageAtlasId(obj.imageAtlasId_);
				}
			}, true);
			loaded_ = true;
		}

		private bool CheckFileEffective(string file)
		{
			if (file == null || file.Length() == 0)
			{
				return false;
			}
			if (file.IndexOf("\\") == -1 && file.IndexOf("/") == -1)
			{
				return !loaded_;
			}
			return loaded_;
		}

		private string GetFileName(string file)
		{
			int num = file.LastIndexOf('\\');
			if (num != -1)
			{
				file = file.Substring(num + 1);
			}
			num = file.LastIndexOf('/');
			if (num != -1)
			{
				file = file.Substring(num + 1);
			}
			return file.Trim();
		}

		public void Dispose()
		{
			Dispose(false);
		}

		public void Dispose(bool keepFileList)
		{
			for (int num = sound.Length - 1; num >= 0; num--)
			{
				if (sound[num] != null)
				{
					sound[num].Dispose();
				}
				sound[num] = null;
			}
			for (int num2 = seb.Length - 1; num2 >= 0; num2--)
			{
				if (seb[num2] != null)
				{
					seb[num2].Dispose();
				}
				seb[num2] = null;
			}
			for (int num3 = img.Length - 1; num3 >= 0; num3--)
			{
				if (img[num3] != null)
				{
					img[num3].Dispose();
				}
				img[num3] = null;
			}
			_init(keepFileList);
		}

		public int GetProgTotal()
		{
			return loadNum_;
		}

		public int GetProgNow()
		{
			return loadCnt_;
		}

		public int GetProgLevel()
		{
			if (loadNum_ == 0)
			{
				return 0;
			}
			return loadCnt_ * 100 / loadNum_;
		}

		public void DrawSeb(Graphics g, float x, float y, int seb)
		{
			this.seb[seb].Draw(g, x, y, img);
		}

		public void DrawSeb(Graphics g, float x, float y, int seb, int frame)
		{
			frame %= this.seb[seb].GetMaxFrame();
			this.seb[seb].DrawFrame(g, x, y, img, frame);
		}

		public void DrawSeb(Graphics g, float x, float y, int seb, int frame, int layer)
		{
			frame %= this.seb[seb].GetMaxFrame();
			this.seb[seb].DrawFrame(g, x, y, img, frame, layer);
		}

		public void DrawSebAnchor(Graphics g, float x, float y, int seb, int anchor)
		{
			float[] sebAnchorPosition = GetSebAnchorPosition(0f, 0f, seb, this.seb[seb].GetCurFrame(), anchor);
			this.seb[seb].SetOffset(sebAnchorPosition[0], sebAnchorPosition[1]);
			try
			{
				this.seb[seb].Draw(g, x, y, img);
			}
			finally
			{
				this.seb[seb].ClearOffset();
			}
		}

		public void DrawSebAnchor(Graphics g, float x, float y, int seb, int frame, int anchor)
		{
			frame %= this.seb[seb].GetMaxFrame();
			float[] sebAnchorPosition = GetSebAnchorPosition(0f, 0f, seb, frame, anchor);
			this.seb[seb].SetOffset(sebAnchorPosition[0], sebAnchorPosition[1]);
			try
			{
				this.seb[seb].DrawFrame(g, x, y, img, frame);
			}
			finally
			{
				this.seb[seb].ClearOffset();
			}
		}

		public void DrawSebAnchor(Graphics g, float x, float y, int seb, int frame, int layer, int anchor)
		{
			frame %= this.seb[seb].GetMaxFrame();
			float[] sebAnchorPosition = GetSebAnchorPosition(0f, 0f, seb, frame, anchor);
			this.seb[seb].SetOffset(sebAnchorPosition[0], sebAnchorPosition[1]);
			try
			{
				this.seb[seb].DrawFrame(g, x, y, img, frame, layer);
			}
			finally
			{
				this.seb[seb].ClearOffset();
			}
		}

		protected float[] GetSebAnchorPosition(float x, float y, int seb, int frame, int anchor)
		{
			short[] boundingRect = this.seb[seb].GetBoundingRect(frame);
			x -= (float)boundingRect[0];
			y -= (float)boundingRect[1];
			return Graphics.GetAnchorPositionF(x, y, boundingRect[2], boundingRect[3], anchor);
		}

		protected float[] GetSebAnchorPositionF(float x, float y, int seb, int frame, int anchor)
		{
			short[] boundingRect = this.seb[seb].GetBoundingRect(frame);
			x -= (float)boundingRect[0];
			y -= (float)boundingRect[1];
			return Graphics.GetAnchorPositionF(x, y, boundingRect[2], boundingRect[3], anchor);
		}

		public void SetLoadFileList(int type, string[] list)
		{
			if (list_[type] != null)
			{
				loadNum_ -= list_[type].Length;
			}
			if (list == null)
			{
				list_[type] = new string[0];
				return;
			}
			list_[type] = new string[list.Length];
			if (list.Length != 0)
			{
				JSystem.Arraycopy(list, 0, list_[type], 0, list.Length);
				for (int i = 0; i < list_[type].Length; i++)
				{
					list_[type][i] = list_[type][i].Trim();
				}
				loadNum_ += list_[type].Length;
			}
		}

		public ResourceManager SetRecordNameBits(int bits)
		{
			recordNameBits_ = bits;
			return this;
		}

		public int GetRecordNameBits()
		{
			return recordNameBits_;
		}

		public void UseAllImages()
		{
			if (img == null)
			{
				return;
			}
			IApplication.GetCurrentApp().RunOnUiThread(delegate
			{
				for (int i = 0; i < img.Length; i++)
				{
					if (img[i] != null)
					{
						img[i].Use();
					}
				}
			}, true);
		}

		public void UnuseAllImages()
		{
			if (img == null)
			{
				return;
			}
			IApplication.GetCurrentApp().RunOnUiThread(delegate
			{
				for (int i = 0; i < img.Length; i++)
				{
					if (img[i] != null)
					{
						img[i].Unuse();
					}
				}
			}, true);
		}

		public void LoadJarImages()
		{
			if (img == null)
			{
				return;
			}
			IApplication.GetCurrentApp().RunOnUiThread(delegate
			{
				for (int i = 0; i < img.Length; i++)
				{
					if (img[i] != null)
					{
						Image image = img[i];
						if (image.useBin_ && rsId_ != -1)
						{
							JarInflater jarInflater = OpenJarCache(rsId_, rcId_);
							image.src_ = Image.OverwritePaletteData(jarInflater.GetData(image.file_), (image.paletteFile_ != null) ? jarInflater.GetData(image.paletteFile_) : null);
						}
						else
						{
							image.Use();
							image.lockExpiration_ = true;
						}
					}
				}
			}, true);
		}

		public void DisposeJarImages()
		{
			if (img == null)
			{
				return;
			}
			IApplication.GetCurrentApp().RunOnUiThread(delegate
			{
				for (int i = 0; i < img.Length; i++)
				{
					if (img[i] != null)
					{
						img[i].Unuse();
						if (rsId_ != -1)
						{
							img[i].src_ = null;
						}
						img[i].lockExpiration_ = true;
					}
				}
			}, true);
		}

		internal static JarInflater OpenJarCache(int rsId, int rcId)
		{
			foreach (JarCache item in jarCache_)
			{
				if (item.RsId == rsId && item.RcId == rcId)
				{
					return item.Jar;
				}
			}
			JarInflater jarInflater = new JarInflater(RecordStore.ReadRecord(rsId, rcId));
			jarCache_.Add(new JarCache(rsId, rcId, jarInflater));
			return jarInflater;
		}

		internal static void CloseJarCache()
		{
			if (jarCache_.Count == 0)
			{
				return;
			}
			for (int num = jarCache_.Count - 1; num >= 0; num--)
			{
				if (jarCache_[num].Jar != null)
				{
					jarCache_[num].Jar.Close();
				}
			}
			jarCache_.Clear();
		}
	}
}
