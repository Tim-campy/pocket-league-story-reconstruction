using System;
using System.Collections.Generic;
using java.io;
using java.lang;
using java.util;

namespace kairo.unity.ui
{
	public class Seb
	{
		private class WorkStaticValue
		{
			internal int[][][] works_ = new int[1][][] { new int[0][] };

			internal int[][][] result_ = new int[0][][];

			internal int[] getBoundingRectI_result = new int[4];

			internal short[] getBoundingRect_result = new short[4];

			internal int[] getSprite_sp1 = new int[12];

			internal int[] getSprite_sp2 = new int[12];
		}

		private class WorkValue
		{
			internal int[][] works_;

			internal int[][] result_;
		}

		public const int SP_FRAME_NO = 0;

		public const int SP_TEX_ID = 1;

		public const int SP_U = 2;

		public const int SP_V = 3;

		public const int SP_W = 4;

		public const int SP_H = 5;

		public const int SP_TRANS_X = 6;

		public const int SP_TRANS_Y = 7;

		public const int SP_REVERS_U = 8;

		public const int SP_REVERS_V = 9;

		public const int SP_BLEND = 10;

		public const int SP_COLOR = 11;

		public const int SP_END = 12;

		public const int TEXID_NONE = -1;

		public const int TEXID_FRECT = -2;

		public const int TEXID_RECT = -3;

		public const int TEXID_LINE = -4;

		public const int TEXID_HIDELINE = -8;

		public const int TEXID_HIDERECT = -9;

		private string name_ = "";

		private int currentFrame_;

		private int maxFrame_;

		private int[] layerHeads_;

		private short[] spriteFrames_;

		private const int SPBUF_TEX = 0;

		private const int SPBUF_X = 1;

		private const int SPBUF_Y = 2;

		private const int SPBUF_U = 3;

		private const int SPBUF_V = 4;

		private const int SPBUF_W = 5;

		private const int SPBUF_H = 6;

		private const int SPBUF_FLAG = 7;

		private const int SPBUF_END = 8;

		private sbyte[] spriteBuffer_;

		private const int SPBUFEX_TEX = 0;

		private const int SPBUFEX_X = 1;

		private const int SPBUFEX_Y = 2;

		private const int SPBUFEX_U = 3;

		private const int SPBUFEX_V = 4;

		private const int SPBUFEX_W = 5;

		private const int SPBUFEX_H = 6;

		private const int SPBUFEX_END = 7;

		private short[] spritesBufferEx_;

		private sbyte flip_;

		private int[][][] cache_;

		private long boundingRect_;

		private sbyte[] boundingRects_;

		private short[] boundingRectsEx_;

		internal ResourceManager manager_;

		private DepthInfo[][] depthInfoList_;

		private float[] offset_ = new float[2];

		[ThreadStatic]
		private static WorkStaticValue threadLocalStatic_ = new WorkStaticValue();

		[ThreadStatic]
		private WorkValue threadLocal_ = new WorkValue();

		private static WorkStaticValue GetWorkStaticValue()
		{
			Seb.WorkStaticValue workStaticValue = Seb.threadLocalStatic_;
			if (workStaticValue == null)
			{
				Seb.WorkStaticValue workStaticValue1 = new Seb.WorkStaticValue();
				workStaticValue = workStaticValue1;
				Seb.threadLocalStatic_ = workStaticValue1;
			}
			return workStaticValue;
		}

		private WorkValue GetWorkValue()
		{
			WorkValue workValue = threadLocal_;
			if (workValue == null)
			{
				threadLocal_ = (workValue = new WorkValue());
			}
			return workValue;
		}

		public static void SetClip(int x, int y, int w, int h)
		{
		}

		public static int[] GetClip()
		{
			return null;
		}

		public static void ClearClip()
		{
		}

		public Seb(InputStream stream)
		{
			_load(stream);
		}

		public Seb(byte[] data)
		{
			_load(data);
		}

		public Seb(Seb seb)
		{
			name_ = seb.name_;
			maxFrame_ = seb.maxFrame_;
			layerHeads_ = seb.layerHeads_;
			spriteFrames_ = seb.spriteFrames_;
			spriteBuffer_ = seb.spriteBuffer_;
			spritesBufferEx_ = seb.spritesBufferEx_;
			boundingRect_ = seb.boundingRect_;
			boundingRects_ = seb.boundingRects_;
			boundingRectsEx_ = seb.boundingRectsEx_;
			manager_ = seb.manager_;
		}

		public void Dispose()
		{
			currentFrame_ = 0;
			maxFrame_ = 0;
			if (cache_ != null)
			{
				for (int num = cache_.Length - 1; num >= 0; num--)
				{
					cache_[num] = null;
				}
			}
			cache_ = null;
			layerHeads_ = null;
			spriteFrames_ = null;
			spriteBuffer_ = null;
			spritesBufferEx_ = null;
		}

		private void _load(byte[] data)
		{
			ByteArrayInputStream byteArrayInputStream = null;
			try
			{
				byteArrayInputStream = new ByteArrayInputStream(data);
				_load(byteArrayInputStream);
			}
			finally
			{
				byteArrayInputStream?.Close();
				byteArrayInputStream = null;
			}
		}

		private void _load(InputStream stream)
		{
			Dispose();
			DataInputStream dataInputStream = null;
			try
			{
				dataInputStream = new DataInputStream(stream);
				int num = 0;
				int num3;
int num2 = dataInputStream.ReadByte();
				if (num2 < 0)
				{
					num = -num2;
					num3 = dataInputStream.ReadShort();
				}
				else
				{
					int num4 = dataInputStream.ReadByte();
					num3 = (short)(((num2 & 0xFF) << 8) | (num4 & 0xFF));
				}
				maxFrame_ = dataInputStream.ReadShort();
				short[][] array = new short[num3][];
				sbyte[][] array2 = new sbyte[num3][];
				int[] array3 = new int[12];
				Vector vector = null;
				for (int i = 0; i < num3; i++)
				{
					int num5 = dataInputStream.ReadShort();
					if (num == 0)
					{
						dataInputStream.ReadShort();
					}
					array[i] = new short[num5];
					array2[i] = new sbyte[num5 * 8];
					for (int j = 0; j < num5; j++)
					{
						if (num == 0) {
							array3[0] = dataInputStream.ReadShort();
							array3[1] = dataInputStream.ReadShort();
							array3[2] = dataInputStream.ReadShort();
							array3[3] = dataInputStream.ReadShort();
							array3[4] = dataInputStream.ReadShort();
							array3[5] = dataInputStream.ReadShort();
							array3[6] = dataInputStream.ReadShort();
							array3[7] = dataInputStream.ReadShort();
							array3[8] = dataInputStream.ReadShort();
							array3[9] = dataInputStream.ReadShort();
							array3[10] = 0;
							uint num7 = uint.MaxValue;
							array3[11] = (int)num7;
							if (array3[1] < 0)
							{
								int num8 = array3[8] & 0xFF00;
								int num9 = 255;
								int num10 = array3[8] & 0xFF;
								int num11 = (array3[9] & 0xFF00) >> 8;
								int num12 = array3[9] & 0xFF;
								if (num8 == 8192) { array3[10] = 1; } else if (num8 == 16384) { array3[10] = 2; } else if (num8 == 32768) { array3[10] = 3; }
								if (array3[10] != 0)
								{
									num9 = 128;
								}
								array3[11] = (num9 << 24) | (num10 << 16) | (num11 << 8) | num12;
							}
							} else if (num == 1) {
							array3[0] = dataInputStream.ReadShort();
							array3[1] = dataInputStream.ReadShort();
							array3[6] = dataInputStream.ReadShort();
							array3[7] = dataInputStream.ReadShort();
							array3[10] = dataInputStream.Read();
							array3[8] = 0;
							array3[9] = 0;
							if ((array3[1] >= 0 && array3[10] != 0) || array3[1] == -2 || array3[1] == -3 || array3[1] == -4 || array3[1] == -5 || array3[1] == -6)
							{
								array3[11] = (dataInputStream.Read() << 24) | (dataInputStream.Read() << 16) | (dataInputStream.Read() << 8) | dataInputStream.Read();
							}
							if (array3[1] >= 0 || array3[1] == -8)
							{
								array3[2] = dataInputStream.ReadShort();
								array3[3] = dataInputStream.ReadShort();
							}
							array3[4] = dataInputStream.ReadShort();
							array3[5] = dataInputStream.ReadShort();
							if (array3[1] >= 0)
							{
								sbyte num6 = dataInputStream.ReadByte();
								if ((num6 & 1) != 0)
								{
									array3[8] = 1;
								}
								if ((num6 & 2) != 0)
								{
									array3[9] = 1;
								}
							}
							}
						int num13 = (int)(((array3[11] & 0xFF000000u) >> 24) & 0xFF);
						if (array3[10] == 1 && num13 == 255)
						{
							array3[10] = 0;
						}
						array[i][j] = (short)array3[0];
						sbyte[] array4 = array2[i];
						int num14 = j * 8;
						array4[num14 + 0] = (sbyte)array3[1];
						array4[num14 + 1] = (sbyte)array3[6];
						array4[num14 + 2] = (sbyte)array3[7];
						array4[num14 + 3] = (sbyte)array3[2];
						array4[num14 + 4] = (sbyte)array3[3];
						array4[num14 + 5] = (sbyte)array3[4];
						array4[num14 + 6] = (sbyte)array3[5];
						array4[num14 + 7] = (sbyte)((array3[8] << 6) | (array3[9] << 5) | array3[10]);
						if (array3[10] != 0 || array3[1] == -2 || array3[1] == -3 || array3[1] == -4 || array3[1] == -8 || array4[num14 + 0] != array3[1] || array4[num14 + 1] != array3[6] || array4[num14 + 2] != array3[7] || (array4[num14 + 3] & 0xFF) != array3[2] || (array4[num14 + 4] & 0xFF) != array3[3] || (array4[num14 + 5] & 0xFF) != array3[4] || (array4[num14 + 6] & 0xFF) != array3[5])
						{
							if (vector == null)
							{
								vector = new Vector();
							}
							array4[num14 + 0] = sbyte.MinValue;
							array4[num14 + 1] = (sbyte)vector.Size();
							array4[num14 + 2] = (sbyte)((array3[11] >> 24) & 0xFF);
							array4[num14 + 3] = (sbyte)((array3[11] >> 16) & 0xFF);
							array4[num14 + 4] = (sbyte)((array3[11] >> 8) & 0xFF);
							array4[num14 + 5] = (sbyte)(array3[11] & 0xFF);
							short[] recoveredSpriteRecord = new short[7];
recoveredSpriteRecord[0] = (short)array3[1];
recoveredSpriteRecord[1] = (short)array3[6];
recoveredSpriteRecord[2] = (short)array3[7];
recoveredSpriteRecord[3] = (short)array3[2];
recoveredSpriteRecord[4] = (short)array3[3];
recoveredSpriteRecord[5] = (short)array3[4];
recoveredSpriteRecord[6] = (short)array3[5];
vector.AddElement(recoveredSpriteRecord);
						}
					}
				}
				int num15 = 0;
				for (int k = 0; k < array2.Length; k++)
				{
					num15 += array2[k].Length / 8;
				}
				layerHeads_ = new int[array2.Length];
				spriteFrames_ = new short[num15];
				spriteBuffer_ = new sbyte[num15 * 8];
				int[] array5;
{
int l = 0;
				int num16 = 0;
				for (; l < layerHeads_.Length; l++)
				{
					layerHeads_[l] = num16;
					JSystem.Arraycopy(array2[l], 0, spriteBuffer_, num16 * 8, array2[l].Length);
					JSystem.Arraycopy(array[l], 0, spriteFrames_, num16, array[l].Length);
					num16 += array[l].Length;
				}
				if (vector != null)
				{
					spritesBufferEx_ = new short[vector.Size() * 7];
					for (int m = 0; m < vector.Size(); m++)
					{
						JSystem.Arraycopy((short[])vector.ElementAt(m), 0, spritesBufferEx_, m * 7, 7);
					}
					vector.RemoveAllElements();
				}
				vector = null;
				boundingRects_ = new sbyte[maxFrame_ * 4];
				array5 = new int[4];
				for (int n = 0; n < maxFrame_; n++)
				{
					int[][] spritesLocal = GetSpritesLocal(n);
					if (spritesLocal == null)
					{
						continue;
					}
					int num17 = 0;
					int num18 = 0;
					int num19 = 0;
					int num20 = 0;
					int num21 = 0;
					for (int num22 = 0; num22 < spritesLocal.Length; num22++)
					{
						if (spritesLocal[num22] == null)
						{
							continue;
						}
						int num23 = spritesLocal[num22][1];
						if (num23 == -1 || num23 == -9 || num23 == -8)
						{
							continue;
						}
						if (num21 == 0)
						{
							num17 = spritesLocal[num22][6];
							num18 = spritesLocal[num22][7];
							num19 = spritesLocal[num22][6] + spritesLocal[num22][4];
							num20 = spritesLocal[num22][7] + spritesLocal[num22][5];
						}
						else
						{
							if (num17 > spritesLocal[num22][6])
							{
								num17 = spritesLocal[num22][6];
							}
							if (num18 > spritesLocal[num22][7])
							{
								num18 = spritesLocal[num22][7];
							}
							if (num19 < spritesLocal[num22][6] + spritesLocal[num22][4])
							{
								num19 = spritesLocal[num22][6] + spritesLocal[num22][4];
							}
							if (num20 < spritesLocal[num22][7] + spritesLocal[num22][5])
							{
								num20 = spritesLocal[num22][7] + spritesLocal[num22][5];
							}
						}
						num21++;
					}
					int num24 = num19 - num17;
					int num25 = num20 - num18;
					int num26 = n * 4;
					boundingRects_[num26 + 0] = (sbyte)num17;
					boundingRects_[num26 + 1] = (sbyte)num18;
					boundingRects_[num26 + 2] = (sbyte)num24;
					boundingRects_[num26 + 3] = (sbyte)num25;
					if (boundingRects_[num26 + 0] == sbyte.MinValue || (sbyte)num17 != num17 || (sbyte)num18 != num18 || (sbyte)num24 != num24 || (sbyte)num25 != num25)
					{
						if (vector == null)
						{
							vector = new Vector();
						}
						boundingRects_[num26 + 0] = sbyte.MinValue;
						boundingRects_[num26 + 1] = (sbyte)(vector.Size() / 4);
						vector.AddElement(new JShort((short)num17));
						vector.AddElement(new JShort((short)num18));
						vector.AddElement(new JShort((short)num24));
						vector.AddElement(new JShort((short)num25));
					}
					if (n == 0)
					{
						array5[0] = num17;
						array5[1] = num18;
						array5[2] = num19;
						array5[3] = num20;
						continue;
					}
					if (array5[0] > num17)
					{
						array5[0] = num17;
					}
					if (array5[1] > num18)
					{
						array5[1] = num18;
					}
					if (array5[2] < num19)
					{
						array5[2] = num19;
					}
					if (array5[3] < num20)
					{
						array5[3] = num20;
					}
				}
				array5[2] = array5[2] - array5[0];
				array5[3] = array5[3] - array5[1];
				boundingRect_ = (long)((((ulong)(short)array5[0] & 0xFFFFuL) << 48) | (((ulong)(short)array5[1] & 0xFFFFuL) << 32) | (((ulong)(short)array5[2] & 0xFFFFuL) << 16) | ((ulong)(short)array5[3] & 0xFFFFuL));
				if (vector != null)
				{
					boundingRectsEx_ = new short[vector.Size()];
					for (int num27 = 0; num27 < boundingRectsEx_.Length; num27++)
					{
						boundingRectsEx_[num27] = ((JShort)vector.ElementAt(num27)).ShortValue();
					}
				}

}
}
			finally
			{
				dataInputStream?.Close();
				dataInputStream = null;
			}
		}

		public int[] GetBoundingRect()
		{
			int[] getBoundingRectI_result = GetWorkStaticValue().getBoundingRectI_result;
			getBoundingRectI_result[0] = (short)((ulong)(boundingRect_ & -281474976710656L) >> 48);
			getBoundingRectI_result[1] = (short)((ulong)(boundingRect_ & 0xFFFF00000000L) >> 32);
			getBoundingRectI_result[2] = (short)((ulong)(boundingRect_ & 0xFFFF0000u) >> 16);
			getBoundingRectI_result[3] = (short)(boundingRect_ & 0xFFFF);
			if ((flip_ & 2) != 0)
			{
				getBoundingRectI_result[0] = -getBoundingRectI_result[2] / 2 + (-getBoundingRectI_result[2] / 2 - getBoundingRectI_result[0]);
			}
			if ((flip_ & 1) != 0)
			{
				getBoundingRectI_result[1] = -getBoundingRectI_result[3] / 2 + (-getBoundingRectI_result[3] / 2 - getBoundingRectI_result[1]);
			}
			return getBoundingRectI_result;
		}

		public short[] GetBoundingRect(int frame)
		{
			short[] getBoundingRect_result = GetWorkStaticValue().getBoundingRect_result;
			int num = frame * 4;
			if (boundingRects_[num + 0] == sbyte.MinValue)
			{
				num = boundingRects_[num + 1] * 4;
				getBoundingRect_result[0] = boundingRectsEx_[num + 0];
				getBoundingRect_result[1] = boundingRectsEx_[num + 1];
				getBoundingRect_result[2] = boundingRectsEx_[num + 2];
				getBoundingRect_result[3] = boundingRectsEx_[num + 3];
			}
			else
			{
				getBoundingRect_result[0] = boundingRects_[num + 0];
				getBoundingRect_result[1] = boundingRects_[num + 1];
				getBoundingRect_result[2] = boundingRects_[num + 2];
				getBoundingRect_result[3] = boundingRects_[num + 3];
			}
			if ((flip_ & 2) != 0)
			{
				getBoundingRect_result[0] = (short)(-getBoundingRect_result[2] / 2 + (-getBoundingRect_result[2] / 2 - getBoundingRect_result[0]));
			}
			if ((flip_ & 1) != 0)
			{
				getBoundingRect_result[1] = (short)(-getBoundingRect_result[3] / 2 + (-getBoundingRect_result[3] / 2 - getBoundingRect_result[1]));
			}
			return getBoundingRect_result;
		}

		public int GetCurFrame()
		{
			return currentFrame_;
		}

		public void SetCurFrame(int frame)
		{
			currentFrame_ = frame;
		}

		public int GetMaxFrame()
		{
			return maxFrame_;
		}

		public void Frame()
		{
			currentFrame_ = (currentFrame_ + 1) % maxFrame_;
		}

		public int[][] GetSprites(int frameNo)
		{
			WorkValue workValue = GetWorkValue();
			int layersNum = GetLayersNum();
			if (workValue.works_ == null)
			{
				workValue.works_ = JTool.MakeArray<int[][]>(new int[2] { layersNum, 12 });
			}
			if (workValue.result_ == null)
			{
				workValue.result_ = new int[layersNum][];
			}
			for (int i = 0; i < layersNum; i++)
			{
				workValue.result_[i] = GetSprite(frameNo, i);
			}
			return workValue.result_;
		}

		private int[][] GetSpritesLocal(int frameNo)
		{
			WorkStaticValue workStaticValue = GetWorkStaticValue();
			int layersNum = GetLayersNum();
			if (workStaticValue.result_ == null || workStaticValue.result_.Length <= layersNum)
			{
				workStaticValue.result_ = new int[layersNum + 1][][];
				for (int i = 0; i < workStaticValue.result_.Length; i++)
				{
					workStaticValue.result_[i] = new int[i][];
				}
			}
			int[][] array = workStaticValue.result_[layersNum];
			for (int j = 0; j < layersNum; j++)
			{
				array[j] = GetSpriteLocal(frameNo, j);
			}
			return array;
		}

		public int[] GetSprite(int frameNo, int lineNo)
		{
			WorkValue workValue = GetWorkValue();
			if (workValue.works_ == null)
			{
				workValue.works_ = JTool.MakeArray<int[][]>(new int[2]
				{
					GetLayersNum(),
					12
				});
			}
			if (cache_ != null)
			{
				for (int i = 0; i < cache_.Length; i++)
				{
					int num = cache_[i][lineNo][0];
					if (num == -1)
					{
						break;
					}
					if (num == frameNo)
					{
						int[] array = cache_[i][lineNo];
						for (int num2 = i - 1; num2 >= 0; num2--)
						{
							cache_[num2 + 1][lineNo] = cache_[num2][lineNo];
						}
						cache_[0][lineNo] = array;
						return cache_[0][lineNo];
					}
				}
				int[] array2 = cache_[cache_.Length - 1][lineNo];
				for (int num3 = cache_.Length - 1; num3 >= 1; num3--)
				{
					cache_[num3][lineNo] = cache_[num3 - 1][lineNo];
				}
				cache_[0][lineNo] = array2;
			}
			int[] array3 = ((cache_ != null) ? cache_[0][lineNo] : workValue.works_[lineNo]);
			int num4 = 0;
			int num5 = 0;
			int num6 = 0;
			int num7 = 0;
			int num8 = layerHeads_[lineNo];
			int num9 = spriteBuffer_.Length / 8 - 1;
			if (lineNo + 1 < layerHeads_.Length)
			{
				num9 = layerHeads_[lineNo + 1] - 1;
			}
			if (num8 > num9)
			{
				return null;
			}
			if (frameNo < spriteFrames_[num8] || frameNo > spriteFrames_[num9])
			{
				return null;
			}
			if (frameNo >= 2)
			{
				while (true)
				{
					int num10 = num8 + (num9 - num8) / 2;
					if (num10 == num8 || spriteFrames_[num10] == frameNo)
					{
						break;
					}
					if (spriteFrames_[num10] < frameNo)
					{
						num8 = num10;
					}
					else
					{
						num9 = num10;
					}
				}
			}
			for (int num11 = num8, num12 = num8 * 8; num11 <= num9;)
			{
				if (frameNo == spriteFrames_[num11])
				{
					ConvBufferToSprite(num11, array3);
					return array3;
				}
				if (frameNo < spriteFrames_[num11])
				{
					num4 = num11 - 1;
					num5 = num11;
					num6 = spriteFrames_[num5] - spriteFrames_[num4];
					num7 = frameNo - spriteFrames_[num4];
					break;
				}
				num11++;
				num12 += 8;
			}
			WorkStaticValue workStaticValue = GetWorkStaticValue();
			int[] array4 = ConvBufferToSprite(num4, workStaticValue.getSprite_sp1);
			int[] array5 = ConvBufferToSprite(num5, workStaticValue.getSprite_sp2);
			array3[0] = frameNo;
			array3[1] = array4[1];
			if (array3[1] < 0)
			{
				array3[2] = array4[2] + (array5[2] - array4[2]) * 4096 / num6 * num7 / 4096;
				array3[3] = array4[3] + (array5[3] - array4[3]) * 4096 / num6 * num7 / 4096;
				array3[4] = array4[4] + (array5[4] - array4[4]) * 4096 / num6 * num7 / 4096;
				array3[5] = array4[5] + (array5[5] - array4[5]) * 4096 / num6 * num7 / 4096;
			}
			else
			{
				array3[2] = array4[2];
				array3[3] = array4[3];
				array3[4] = array4[4];
				array3[5] = array4[5];
			}
			array3[6] = array4[6] + (array5[6] - array4[6]) * 4096 / num6 * num7 / 4096;
			array3[7] = array4[7] + (array5[7] - array4[7]) * 4096 / num6 * num7 / 4096;
			array3[8] = array4[8];
			array3[9] = array4[9];
			array3[2] = array4[2];
			array3[3] = array4[3];
			array3[4] = array4[4];
			array3[5] = array4[5];
			array3[10] = array4[10];
			array3[11] = array4[11];
			return array3;
		}

		private int[] GetSpriteLocal(int frameNo, int lineNo)
		{
			WorkStaticValue workStaticValue = GetWorkStaticValue();
			if (layerHeads_.Length >= workStaticValue.works_.Length)
			{
				int num = layerHeads_.Length + 5;
				workStaticValue.works_ = new int[num][][];
				for (int i = 0; i < workStaticValue.works_.Length; i++)
				{
					workStaticValue.works_[i] = JTool.MakeArray<int[][]>(new int[2] { i, 12 });
				}
			}
			if (cache_ != null)
			{
				for (int j = 0; j < cache_.Length; j++)
				{
					int num2 = cache_[j][lineNo][0];
					if (num2 == -1)
					{
						break;
					}
					if (num2 == frameNo)
					{
						int[] array = cache_[j][lineNo];
						for (int num3 = j - 1; num3 >= 0; num3--)
						{
							cache_[num3 + 1][lineNo] = cache_[num3][lineNo];
						}
						cache_[0][lineNo] = array;
						return cache_[0][lineNo];
					}
				}
				int[] array2 = cache_[cache_.Length - 1][lineNo];
				for (int num4 = cache_.Length - 1; num4 >= 1; num4--)
				{
					cache_[num4][lineNo] = cache_[num4 - 1][lineNo];
				}
				cache_[0][lineNo] = array2;
			}
			int[] array3 = ((cache_ != null) ? cache_[0][lineNo] : workStaticValue.works_[layerHeads_.Length][lineNo]);
			int num5 = 0;
			int num6 = 0;
			int num7 = 0;
			int num8 = 0;
			int num9 = layerHeads_[lineNo];
			int num10 = spriteBuffer_.Length / 8 - 1;
			if (lineNo + 1 < layerHeads_.Length)
			{
				num10 = layerHeads_[lineNo + 1] - 1;
			}
			if (num9 > num10)
			{
				return null;
			}
			if (frameNo < spriteFrames_[num9] || frameNo > spriteFrames_[num10])
			{
				return null;
			}
			if (frameNo >= 2)
			{
				while (true)
				{
					int num11 = num9 + (num10 - num9) / 2;
					if (num11 == num9 || spriteFrames_[num11] == frameNo)
					{
						break;
					}
					if (spriteFrames_[num11] < frameNo)
					{
						num9 = num11;
					}
					else
					{
						num10 = num11;
					}
				}
			}
			for (int num12 = num9, num13 = num9 * 8; num12 <= num10;)
			{
				if (frameNo == spriteFrames_[num12])
				{
					ConvBufferToSprite(num12, array3);
					return array3;
				}
				if (frameNo < spriteFrames_[num12])
				{
					num5 = num12 - 1;
					num6 = num12;
					num7 = spriteFrames_[num6] - spriteFrames_[num5];
					num8 = frameNo - spriteFrames_[num5];
					break;
				}
				num12++;
				num13 += 8;
			}
			int[] array4 = ConvBufferToSprite(num5, workStaticValue.getSprite_sp1);
			int[] array5 = ConvBufferToSprite(num6, workStaticValue.getSprite_sp2);
			array3[0] = frameNo;
			array3[1] = array4[1];
			if (array3[1] < 0)
			{
				array3[2] = array4[2] + (array5[2] - array4[2]) * 4096 / num7 * num8 / 4096;
				array3[3] = array4[3] + (array5[3] - array4[3]) * 4096 / num7 * num8 / 4096;
				array3[4] = array4[4] + (array5[4] - array4[4]) * 4096 / num7 * num8 / 4096;
				array3[5] = array4[5] + (array5[5] - array4[5]) * 4096 / num7 * num8 / 4096;
			}
			else
			{
				array3[2] = array4[2];
				array3[3] = array4[3];
				array3[4] = array4[4];
				array3[5] = array4[5];
			}
			array3[6] = array4[6] + (array5[6] - array4[6]) * 4096 / num7 * num8 / 4096;
			array3[7] = array4[7] + (array5[7] - array4[7]) * 4096 / num7 * num8 / 4096;
			array3[8] = array4[8];
			array3[9] = array4[9];
			array3[2] = array4[2];
			array3[3] = array4[3];
			array3[4] = array4[4];
			array3[5] = array4[5];
			array3[10] = array4[10];
			array3[11] = array4[11];
			return array3;
		}

		private int[] ConvBufferToSprite(int index, int[] sprite)
		{
			sprite[0] = spriteFrames_[index];
			int num = index * 8;
			if (spriteBuffer_[num + 0] == sbyte.MinValue)
			{
				sprite[11] = ((spriteBuffer_[num + 2] & 0xFF) << 24) | ((spriteBuffer_[num + 3] & 0xFF) << 16) | ((spriteBuffer_[num + 4] & 0xFF) << 8) | (spriteBuffer_[num + 5] & 0xFF);
				int num2 = (spriteBuffer_[num + 1] & 0xFF) * 7;
				sprite[1] = spritesBufferEx_[num2 + 0];
				sprite[6] = spritesBufferEx_[num2 + 1];
				sprite[7] = spritesBufferEx_[num2 + 2];
				sprite[2] = spritesBufferEx_[num2 + 3];
				sprite[3] = spritesBufferEx_[num2 + 4];
				sprite[4] = spritesBufferEx_[num2 + 5];
				sprite[5] = spritesBufferEx_[num2 + 6];
			}
			else
			{
				sprite[1] = spriteBuffer_[num + 0];
				sprite[6] = spriteBuffer_[num + 1];
				sprite[7] = spriteBuffer_[num + 2];
				sprite[2] = spriteBuffer_[num + 3] & 0xFF;
				sprite[3] = spriteBuffer_[num + 4] & 0xFF;
				sprite[4] = spriteBuffer_[num + 5] & 0xFF;
				sprite[5] = spriteBuffer_[num + 6] & 0xFF;
			}
			sprite[10] = spriteBuffer_[num + 7] & 0xF;
			sprite[8] = (spriteBuffer_[num + 7] >> 6) & 1;
			sprite[9] = (spriteBuffer_[num + 7] >> 5) & 1;
			if ((flip_ & 2) != 0)
			{
				sprite[8] = (sprite[8] + 1) % 2;
				if (sprite[1] == -8)
				{
					sprite[6] *= -1;
					sprite[2] *= -1;
					sprite[4] *= -1;
				}
				else
				{
					sprite[6] = -sprite[4] / 2 + (-sprite[4] / 2 - sprite[6]);
				}
			}
			if ((flip_ & 1) != 0)
			{
				sprite[9] = (sprite[9] + 1) % 2;
				if (sprite[1] == -8)
				{
					sprite[7] *= -1;
					sprite[3] *= -1;
					sprite[5] *= -1;
				}
				else
				{
					sprite[7] = -sprite[5] / 2 + (-sprite[5] / 2 - sprite[7]);
				}
			}
			return sprite;
		}

		public void DrawFrame(Graphics g, float x, float y, Image[] img, int frame, int lineNo = -1)
		{
			_draw(g, x, y, img, frame, lineNo);
		}

		public void Draw(Graphics g, float x, float y, Image[] img, int lineNo = -1)
		{
			_draw(g, x, y, img, currentFrame_, lineNo);
		}

		private void _draw(Graphics g, float x, float y, Image[] img, int frame, int lineNo)
		{
			if (lineNo == -1)
			{
				int[][] spritesLocal = GetSpritesLocal(frame);
				if (spritesLocal == null) { return; }
for (int i = 0; i < layerHeads_.Length; i++)
					{
						Draw(g, x, y, img, spritesLocal[i]);
					}
			}
			else
			{
				int[] spriteLocal = GetSpriteLocal(frame, lineNo);
				Draw(g, x, y, img, spriteLocal);
			}
		}

		public void Draw(Graphics g, float x, float y, Image[] img, int[] sp)
		{
			if (sp == null)
			{
				return;
			}
			GetWorkStaticValue();
			float num = (float)sp[6] + offset_[0];
			float num2 = (float)sp[7] + offset_[1];
			if (g.CheckLocalMatrix())
			{
				g.PushMatrix(Matrix.TranslateTemporary(num, num2));
			}
			else
			{
				x += num;
				y += num2;
			}
			bool flag = false;
			if (sp[10] != 0)
			{
				flag = true;
			}
			if (flag)
			{
				int num3 = (int)(((sp[11] & 0xFF000000u) >> 24) & 0xFF);
				if (g.GetRenderModeOperator() == 0 && g.GetRenderModeSrcRatio() != 255)
				{
					num3 = num3 * g.GetRenderModeSrcRatio() / 255;
					if (num3 > 255)
					{
						num3 = 255;
					}
				}
				if (sp[10] == 1)
				{
					g.PushRenderMode(1, num3, 255 - num3);
				}
				else if (sp[10] == 2)
				{
					g.PushRenderMode(1, num3, 255);
				}
				else if (sp[10] == 3)
				{
					g.PushRenderMode(2, num3, 255);
				}
			}
			if (sp[1] > -1)
			{
				int sx = sp[2];
				int sy = sp[3];
				int width = sp[4];
				int height = sp[5];
				if (true)
				{
					int num4 = sp[8] * 1 + sp[9] * 2;
					if (num4 != 0)
					{
						g.SetFlipMode(num4);
					}
					g.DrawImage(img[sp[1]], x, y, sx, sy, width, height);
					if (num4 != 0)
					{
						g.SetFlipMode(0);
					}
				}
			}
			else if (sp[1] == -2)
			{
				g.SetColor(Graphics.GetColorOfRGB(((sp[11] & 0xFF0000) >> 16) & 0xFF, ((sp[11] & 0xFF00) >> 8) & 0xFF, sp[11] & 0xFF & 0xFF));
				g.FillRect(x, y, sp[4], sp[5]);
			}
			else if (sp[1] == -3)
			{
				g.SetColor(Graphics.GetColorOfRGB(((sp[11] & 0xFF0000) >> 16) & 0xFF, ((sp[11] & 0xFF00) >> 8) & 0xFF, sp[11] & 0xFF & 0xFF));
				g.DrawRect(x, y, sp[4], sp[5]);
			}
			else if (sp[1] == -4)
			{
				g.SetColor(Graphics.GetColorOfRGB(((sp[11] & 0xFF0000) >> 16) & 0xFF, ((sp[11] & 0xFF00) >> 8) & 0xFF, sp[11] & 0xFF & 0xFF));
				g.DrawLine(x, y, x + (float)sp[4], y + (float)sp[5]);
			}
			if (flag)
			{
				g.PopRenderMode();
			}
			if (g.CheckLocalMatrix())
			{
				g.PopMatrix();
			}
		}

		public int GetLayersNum()
		{
			return layerHeads_.Length;
		}

		public void SetName(string name)
		{
			name_ = name;
		}

		public string GetName()
		{
			return name_;
		}

		public override string ToString()
		{
			return "[" + name_ + "] " + base.ToString();
		}

		public ResourceManager GetManager()
		{
			return manager_;
		}

		public void Reverse(bool u, bool v)
		{
			if (cache_ != null)
			{
				for (int i = 0; i < cache_.Length; i++)
				{
					for (int j = 0; j < cache_[i].Length; j++)
					{
						cache_[i][j][0] = -1;
					}
				}
			}
			flip_ = 0;
			if (u)
			{
				flip_ |= 2;
			}
			if (v)
			{
				flip_ |= 1;
			}
		}

		public void SetCacheSize(int size)
		{
			cache_ = new int[size][][];
			for (int i = 0; i < cache_.Length; i++)
			{
				cache_[i] = JTool.MakeArray<int[][]>(new int[2] { layerHeads_.Length, 12 });
				for (int j = 0; j < cache_[i].Length; j++)
				{
					cache_[i][j][0] = -1;
				}
			}
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public bool IsFlipHorizontal()
		{
			return (flip_ & 2) != 0;
		}

		public bool IsFlipVertical()
		{
			return (flip_ & 1) != 0;
		}

		public DepthInfo[] GetDepthInfo(int frame, int scale, int defaultDepth)
		{
			int maxFrame = GetMaxFrame();
			if (frame < 0 || maxFrame <= frame)
			{
				return null;
			}
			if (depthInfoList_ == null)
			{
				depthInfoList_ = new DepthInfo[maxFrame][];
			}
			if (depthInfoList_[frame] != null)
			{
				DepthInfo[] array = depthInfoList_[frame];
				if (array.Length != 0 && array[0].scale != scale)
				{
					for (int i = 0; i < array.Length; i++)
					{
						array[i].depth = array[i].depth / array[i].scale * scale;
						array[i].scale = scale;
					}
				}
				return array;
			}
			defaultDepth *= scale;
			int[][] sprites = GetSprites(frame);
			List<DepthInfo> list = new List<DepthInfo>();
			DepthInfo depthInfo = new DepthInfo(this);
			depthInfo.depth = defaultDepth;
			depthInfo.scale = scale;
			int num = 0;
			for (int j = 0; j < sprites.Length + 1; j++)
			{
				int[] array2 = null;
				int num2 = -1;
				if (j < sprites.Length)
				{
					array2 = sprites[j];
					num2 = array2[1];
				}
				if ((j == sprites.Length || num2 == -9 || num2 == -8) && j - num > 0)
				{
					depthInfo.start = num;
					depthInfo.length = j - num;
					int num3 = int.MaxValue;
					int num4 = int.MinValue;
					int num5 = int.MaxValue;
					int num6 = int.MinValue;
					for (int k = 0; k < depthInfo.length; k++)
					{
						int[] array3 = sprites[depthInfo.start + k];
						if (array3[1] >= 0 || array3[1] == -2 || array3[1] == -4 || array3[1] == -3)
						{
							if (array3[6] < num3)
							{
								num3 = array3[6];
							}
							if (array3[7] < num5)
							{
								num5 = array3[7];
							}
							if (num4 < array3[6] + array3[4])
							{
								num4 = array3[6] + array3[4];
							}
							if (num6 < array3[7] + array3[5])
							{
								num6 = array3[7] + array3[5];
							}
						}
					}
					if (num3 != int.MaxValue)
					{
						int x = num3;
						int y = num5;
						int width = num4 - num3 + 1;
						int height = num6 - num5 + 1;
						depthInfo.SetBoundingRect(x, y, width, height);
					}
					list.Add(depthInfo);
				}
				if (array2 == null)
				{
					continue;
				}
				if (num2 == -9)
				{
					depthInfo = new DepthInfo(this);
					if (array2[5] == 1)
					{
						depthInfo.depth = array2[7] * scale;
					}
					else
					{
						depthInfo.depth = array2[5] * scale;
					}
					depthInfo.scale = scale;
					num = j + 1;
				}
				else if (num2 == -8)
				{
					depthInfo = new DepthInfo(this);
					depthInfo.depth = defaultDepth;
					depthInfo.scale = scale;
					num = j + 1;
					depthInfo.CreateDepthLines(array2, scale);
					int num7 = int.MaxValue;
					for (int l = 0; l < depthInfo.depthLines.Length; l += 2)
					{
						if (depthInfo.depthLines[l] < num7)
						{
							num7 = depthInfo.depthLines[l];
						}
					}
					if (num7 != int.MaxValue)
					{
						depthInfo.depth = num7;
					}
				}
				else if (num2 < 0 && num2 != -2 && num2 != -3 && num2 != -4 && num == j)
				{
					num++;
				}
			}
			depthInfoList_[frame] = list.ToArray();
			return depthInfoList_[frame];
		}

		public void SetOffset(float x, float y)
		{
			offset_[0] = x;
			offset_[1] = y;
		}

		public void ClearOffset()
		{
			offset_[0] = 0f;
			offset_[1] = 0f;
		}
	}
}
