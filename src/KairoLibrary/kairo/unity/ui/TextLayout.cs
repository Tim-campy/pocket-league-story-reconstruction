using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using java.lang;
using kairo.common.cfg;
using kairo.unity.graphics;
using kairo.unity.util;

namespace kairo.unity.ui
{
	public class TextLayout
	{
		internal class TextBlock
		{
			protected TextLayout parent_;

			public Font font_;

			public int color_;

			public string text_;

			public float x_;

			public float y_;

			public int blink_;

			public int blinkCnt_;

			public float width_;

			public bool isPictograph_;

			public string[] pictographParam_;

			public TextBlock(TextLayout parent, Font font, int color, float x, float y, int blink)
			{
				parent_ = parent;
				font_ = new Font(font);
				color_ = color;
				x_ = x;
				y_ = y;
				blink_ = blink;
				text_ = "";
			}

			public int Draw(Graphics g, float x, float y, float boxY, float boxH, int len)
			{
				float num = x_;
				float num2 = y_;
				if (g.CheckLocalMatrix())
				{
					g.PushMatrix(Matrix.TranslateTemporary(num, num2));
					num = (num2 = 0f);
				}
				try
				{
					if (isPictograph_)
					{
						int result = 1;
						if (len == 0)
						{
							return 0;
						}
						Pictograph pictograph = GetPictograph(pictographParam_[0]);
						if (pictograph == null)
						{
							return result;
						}
						int num3 = pictograph.Frame;
						int num4 = pictograph.Wait;
						int num5 = pictograph.X;
						int num6 = pictograph.Y;
						if (1 < pictographParam_.Length && pictographParam_[1].Length > 0)
						{
							if (pictographParam_[1] == "∞") { num3 = -1; } else { num3 = int.Parse(pictographParam_[1]); }
						}
						if (2 < pictographParam_.Length && pictographParam_[2].Length > 0)
						{
							num4 = int.Parse(pictographParam_[2]);
						}
						if (3 < pictographParam_.Length && pictographParam_[3].Length > 0)
						{
							num5 = int.Parse(pictographParam_[3]);
						}
						if (4 < pictographParam_.Length && pictographParam_[4].Length > 0)
						{
							num6 = int.Parse(pictographParam_[4]);
						}
						x += (float)num5;
						y += (float)num6;
						if (num4 < 1)
						{
							num4 = 1;
						}
						if (num3 == -1)
						{
							num3 = parent_.draw_ / num4 % pictograph.Seb.GetMaxFrame();
						}
						float num7 = (float)font_.GetHeight() / 12f;
						short[] boundingRect = pictograph.Seb.GetBoundingRect(num3);
						if (boxH != -1f && (y + y_ + (float)(boundingRect[1] + boundingRect[3]) * num7 < boxY || boxY + boxH < y + y_ + (float)boundingRect[1] * num7))
						{
							return result;
						}
						if (pictographVisible_)
						{
							int[][] sprites = pictograph.Seb.GetSprites(num3);
							foreach (int[] array in sprites)
							{
								if (array[1] < 0)
								{
									continue;
								}
								bool flag = false;
								if (array[10] != 0)
								{
									flag = true;
								}
								if (flag)
								{
									int num8 = (int)(((array[11] & 0xFF000000u) >> 24) & 0xFF);
									if (g.GetRenderModeOperator() == 0 && g.GetRenderModeSrcRatio() != 255)
									{
										num8 = num8 * g.GetRenderModeSrcRatio() / 255;
										if (num8 > 255)
										{
											num8 = 255;
										}
									}
									if (array[10] == 1)
									{
										g.PushRenderMode(1, num8, 255 - num8);
									}
									else if (array[10] == 2)
									{
										g.PushRenderMode(1, num8, 255);
									}
									else if (array[10] == 3)
									{
										g.PushRenderMode(2, num8, 255);
									}
								}
								float dx = x + num + (float)array[6] * num7;
								float dy = y + num2 + (float)array[7] * num7;
								int num9 = (int)((float)array[4] * num7 + 0.01f);
								int num10 = (int)((float)array[5] * num7 + 0.01f);
								if (!pictograph.Transform.IsIdentity())
								{
									g.PushMatrix(pictograph.Transform);
								}
								g.DrawScaledImage(pictograph.Seb.manager_.img[array[1]], dx, dy, num9, num10, array[2], array[3], array[4], array[5]);
								if (!pictograph.Transform.IsIdentity())
								{
									g.PopMatrix();
								}
								if (flag)
								{
									g.PopRenderMode();
								}
							}
						}
						return result;
					}
					{
					int num11 = len;
					if (num11 > text_.Length())
					{
						num11 = text_.Length();
					}
					if ((blink_ > 0 && blinkCnt_ >= blink_) || len == 0)
					{
						return num11;
					}
					if (boxH != -1f && (y + y_ + (float)font_.GetHeight() < boxY || boxY + boxH < y + y_))
					{
						return num11;
					}
					g.SetColor(color_);
					if (len >= text_.Length())
					{
						font_.Draw(g, text_, x + num, y + num2);
					}
					else
					{
						font_.Draw(g, text_.Substring(0, len), x + num, y + num2);
					}
					return num11;
					}
				}
				finally
				{
					if (g.CheckLocalMatrix())
					{
						g.PopMatrix();
					}
				}
			}

			public void Dispose()
			{
				font_.Dispose();
				font_ = null;
				text_ = null;
			}
		}

		internal TextBlock[] textBlocks_ = new TextBlock[0];

		private static Dictionary<string, Pictograph> pictographList_;

		private Font font_;

		private int color_;

		private string text_;

		private int textHashCode_;

		private int length_;

		private float width_;

		private float height_;

		private int pos_;

		private int speed_;

		private long beginTime_;

		private int lineSpace_;

		private int anchor_;

		private float textWidth_;

		private bool format_;

		private int frame_;

		private int draw_;

		private bool autoScale_;

		private float scaleRatio_;

		private int textCheckSize_;

		private bool multiLine_;

		private bool localizeFontEnable_;

		private bool adjustFontSize_;

		public bool overArea_;

		private static bool renderRectVisible_;

		private static bool pictographVisible_;

		private static readonly string[] TAG;

		private static readonly int DEFAULT_CACHE_SIZE;

		private static TextLayout[] cache_;

		private static int cachePointer_;

		static TextLayout()
		{
			pictographList_ = new Dictionary<string, Pictograph>();
			renderRectVisible_ = false;
			pictographVisible_ = true;
			TAG = new string[10] { "<co=>", "</co>", "<si=>", "</si>", "<st=>", "</st>", "<bl=>", "</bl>", "<po=>", "<pic=>" };
			DEFAULT_CACHE_SIZE = 20;
			cache_ = new TextLayout[DEFAULT_CACHE_SIZE];
			cachePointer_ = 0;
		}

		public static void SetCacheSize(int size)
		{
			for (int i = 0; i < cache_.Length; i++)
			{
				cache_[i].Dispose();
				cache_[i] = null;
			}
			cache_ = new TextLayout[size];
			cachePointer_ = 0;
		}

		public static void ClearCache()
		{
			cachePointer_ = 0;
		}

		public TextLayout()
			: this(!Language.Japanese())
		{
		}

		public TextLayout(bool autoScale, bool localizeFontEnable = true)
		{
			Init();
			autoScale_ = autoScale;
			localizeFontEnable_ = localizeFontEnable;
		}

		public void Init()
		{
			font_ = new Font();
			color_ = Graphics.GetColorOfRGB(0, 0, 0);
			text_ = "";
			textHashCode_ = text_.GetHashCode();
			pos_ = 0;
			beginTime_ = 0L;
			width_ = -1f;
			height_ = -1f;
			speed_ = 1000;
			lineSpace_ = Graphics.GetLineSpace();
			textWidth_ = 0f;
			anchor_ = 17;
			format_ = false;
			multiLine_ = true;
			SetGraphicsScale(IApplication.GetCurrentApp().GetScaleRatio());
		}

		public void SetText(string text)
		{
			if (text == null)
			{
				text = "";
			}
			text_ = text;
			textHashCode_ = text_.GetHashCode();
			pos_ = 0;
			beginTime_ = 0L;
			if (!adjustFontSize_ && multiLine_)
			{
				int num = font_.GetHeight() * 100 / Font.requestFontScale_;
				font_.SetSize(num * 100 / Font.requestFontScale_, false);
				adjustFontSize_ = true;
			}
			_makeTextBlocks();
			Font font = new Font(font_);
			if (autoScale_ && height_ > 0f)
			{
				if (multiLine_ && height_ > 0f)
				{
					while (height_ + 1f < (float)GetHeight())
					{
						int num2 = font_.GetHeight() - 1;
						font_.SetSize(num2, false);
						_makeTextBlocks();
						if (num2 == 1)
						{
							break;
						}
					}
					if (height_ < (float)GetHeight())
					{
						height_ = GetHeight();
					}
				}
				else if (!multiLine_ && width_ > 0f)
				{
					while (width_ < textWidth_)
					{
						int num3 = font_.GetHeight() - 1;
						font_.SetSize(num3, false);
						_makeTextBlocks();
						if (num3 == 1)
						{
							break;
						}
					}
				}
			}
			length_ = 0;
			for (int i = 0; i < textBlocks_.Length; i++)
			{
				if (textBlocks_[i].isPictograph_)
				{
					length_++;
				}
				else
				{
					length_ += textBlocks_[i].text_.Length();
				}
			}
			font_ = font;
		}

		public string GetText()
		{
			return text_;
		}

		private void _makeTextBlocks()
		{
			overArea_ = false;
			if (text_.Length() == 0)
			{
				textBlocks_ = new TextBlock[0];
				return;
			}
			Graphics.PushAutoTextLayout(false);
			bool flag = false;
			if (!Language.Japanese() && multiLine_ && text_.Contains(' '))
			{
				flag = true;
			}
			Font font = font_;
			if (!localizeFontEnable_)
			{
				Font.PushLocalizeSetting(false);
			}
			bool flag2 = font_.IsNeedLocalizeScale();
			if (flag2)
			{
				font_ = new Font(font_);
				font_.SetSize(font_.GetLocalizeScale());
				Font.PushLocalizeSetting(false);
			}
			try
			{
				Font.LT_DISABLE_TEXTLAYOUT = true;
				int blink = 0;
				int colorOfRGB = color_;
				int num = anchor_ & 7;
				int num2 = anchor_ & 0x70;
				Font font2 = new Font(font_);
				font2.AdjustFontSize(scaleRatio_);
				int height = font_.GetHeight();
				float num3 = 0f;
				float num4 = 0f;
				float num5 = 0f;
				int num6 = 0;
				textWidth_ = 0f;
				string text = StringUtil.Replace(Language.TranslateText(text_), "<br>", "\n");
				bool flag3 = true;
				bool flag4 = true;
				TextBlock textBlock = null;
				FastVector fastVector = new FastVector();
				for (int i = 0; i < text.Length(); i++)
				{
					char c = text.CharAt(i);
					bool flag5 = false;
					int num7 = 0;
					string[] array = null;
					int num8 = 0;
					if (c == '<' && (num8 = text.IndexOf('>', i)) != -1)
					{
						if (num8 - i > 32)
						{
							continue;
						}
						int num9 = -1;
						string text2 = text.SubstringJ(i, num8 + 1);
						for (int j = 0; j < TAG.Length; j++)
						{
							if (TAG[j].CompareTo(text2) == 0 || (TAG[j].IndexOf('=') != -1 && text2.IndexOf('=') != -1 && text2.StartsWith(TAG[j].Substring(0, TAG[j].IndexOf('=') + 1))))
							{
								num9 = j;
								break;
							}
						}
						if (num9 != -1)
						{
							i = num8;
							int num10 = 0;
							int num11 = 0;
							string text3 = "";
							if (text2.IndexOf('=') != -1)
							{
								text3 = text2.SubstringJ(text2.IndexOf('=') + 1, text2.Length() - 1);
								try
								{
									num10 = JInteger.ParseInt(text3);
								}
								catch (Exception)
								{
								}
								try
								{
									num11 = JInteger.ParseInt(text3, 16);
								}
								catch (Exception)
								{
								}
								if (num9 == 0 && num11 == 0 && text3.IndexOf(',') != -1)
								{
									string[] array2 = StringUtil.Split(text3, ",");
									if (array2.Length != 0)
									{
										num11 |= JInteger.ParseInt(array2[0]) << 16;
									}
									if (1 < array2.Length)
									{
										num11 |= JInteger.ParseInt(array2[1]) << 8;
									}
									if (2 < array2.Length)
									{
										num11 |= JInteger.ParseInt(array2[2]);
									}
								}
							}
							{ if ((num9 == 0)) { colorOfRGB = Graphics.GetColorOfRGB((num11 >> 16) & 0xFF, (num11 >> 8) & 0xFF, num11 & 0xFF);
								 } else if ((num9 == 1)) { colorOfRGB = color_;
								 } else if ((num9 == 2)) { font2.SetSize(num10);
								if (height < font2.GetHeight())
								{
									height = font2.GetHeight();
								}
								 } else if ((num9 == 3)) { font2.SetSize(font_.GetSize());
								if (height < font2.GetHeight())
								{
									height = font2.GetHeight();
								}
								 } else if ((num9 == 4)) { font2.SetStyle(num10);
								 } else if ((num9 == 5)) { font2.SetStyle(font_.GetStyle());
								 } else if ((num9 == 6)) { blink = num10;
								 } else if ((num9 == 7)) { blink = 0;
								 } else if ((num9 == 8)) { if (text3.IndexOf('l') != -1)
								{
									num = 1;
								}
								if (text3.IndexOf('c') != -1)
								{
									num = 2;
								}
								if (text3.IndexOf('r') != -1)
								{
									num = 4;
								}
								if (text3.IndexOf('t') != -1)
								{
									num2 = 16;
								}
								if (text3.IndexOf('m') != -1)
								{
									num2 = 32;
								}
								if (text3.IndexOf('b') != -1)
								{
									num2 = 64;
								}
								num3 = 0f;
								num4 = 0f;
								 } else if ((num9 == 9)) { {
								flag5 = true;
								array = StringUtil.Split(text3, ",");
								Pictograph pictograph = GetPictograph(array[0]);
								if (pictograph == null)
								{
									goto recoveredSwitchExit_11784;
								}
								int num12 = pictograph.Frame;
								if (1 < array.Length && array[1].Length > 0)
								{
									if ((array[1] == "∞")) { num12 = (-1); } else { num12 = int.Parse(array[1]); }
								}
								int num13 = pictograph.X;
								if (3 < array.Length && array[3].Length > 0)
								{
									num13 = int.Parse(array[3]);
								}
								float x;
								float y;
								if (num12 == -1)
								{
									int[] boundingRect = pictograph.Seb.GetBoundingRect();
									x = boundingRect[0] + boundingRect[2];
									y = boundingRect[1] + boundingRect[3];
								}
								else
								{
									if (num12 >= pictograph.Seb.GetMaxFrame())
									{
										num12 = pictograph.Seb.GetMaxFrame() - 1;
									}
									short[] boundingRect2 = pictograph.Seb.GetBoundingRect(num12);
									x = boundingRect2[0] + boundingRect2[2];
									y = boundingRect2[1] + boundingRect2[3];
								}
								pictograph.Transform.MapPoints(ref x, ref y);
								num7 = num13 + (int)(x + 0.9f) + pictograph.Margin;
								float num14 = (float)font2.GetHeight() / 12f;
								num7 = (int)((float)num7 * num14 + 0.01f);

							} }
recoveredSwitchExit_11784: ; }
							flag3 = true;
							if (!flag5)
							{
								continue;
							}
						}
					}
					float num15 = 0f;
					if (flag5)
					{
						num15 = num7;
					}
					else if (c != '\n' && c != '\r')
					{
						if (textBlock != null) { num15 = (font2.StringWidthF(textBlock.text_ + c) - textBlock.width_); } else { num15 = font2.StringWidthF(JString.ValueOf(c)); }
					}
					bool flag6 = c == '\n';
					if (!flag6 && width_ != -1f && num5 + num15 > width_ - num3 && multiLine_)
					{
						if (format_ && (c == '。' || c == '、'))
						{
							num15 = 0f;
						}
						else
						{
							flag6 = true;
						}
					}
					if (flag && flag4 && c != ' ')
					{
						int num16 = text.Length();
						for (int k = i; k < text.Length(); k++)
						{
							if (text.CharAt(k) == ' ' || text.CharAt(k) == '\n' || text.StartsWith("<br>", k))
							{
								num16 = k;
								break;
							}
						}
						if (num16 - i > 0)
						{
							float num17 = 0f;
							for (int l = i; l < num16; l++)
							{
								char c2 = text.CharAt(l);
								if (c2 == '<' && (text.StartsWith("<co=", l) || text.StartsWith("<si=", l) || text.StartsWith("<st=", l) || text.StartsWith("<po=", l) || text.StartsWith("<bl=", l) || text.StartsWith("</", l) || text.StartsWith("<br>", l)))
								{
									int num18 = text.IndexOf('>', l + 1);
									if (num18 != -1)
									{
										l = num18;
										continue;
									}
								}
								num17 += font2.StringWidthF(JString.ValueOf(c2));
							}
							if (width_ != -1f && num5 + num17 > width_ - num3)
							{
								if (i == 0)
								{
									flag = false;
								}
								else
								{
									flag6 = true;
								}
							}
						}
						flag4 = false;
					}
					if (!Language.Japanese() && c == ' ')
					{
						flag4 = true;
						if (flag6)
						{
							num15 = 0f;
						}
					}
					if (flag6)
					{
						if (num == 2)
						{
							if (width_ == -1f) { num3 = (num3 - num5 / 2f); } else { num3 = (num3 + (width_ - num3) / 2f - num5 / 2f); }
						}
						if (num == 4)
						{
							if (!((width_ != -1f)))
							{
								num3 = (num3 - num5);
							}
							else
							{
								num3 = (width_ - num5);
							}
						}
						for (int m = num6; m < fastVector.Size(); m++)
						{
							TextBlock textBlock2 = (TextBlock)fastVector.ElementAt(m);
							textBlock2.x_ = num3;
							textBlock2.y_ = num4 + (float)((height - textBlock2.font_.GetHeight()) / 2);
							num3 += textBlock2.width_;
						}
						num5 = 0f;
						num6 = fastVector.Size();
						num3 = 0f;
						num4 += (float)(height + lineSpace_);
						height = font2.GetHeight();
						flag3 = true;
					}
					if ((num15 > 0f || flag5) && flag3)
					{
						textBlock = new TextBlock(this, font2, colorOfRGB, 0f, 0f, blink);
						fastVector.AddElement(textBlock);
						flag3 = false;
					}
					if (flag5)
					{
						textBlock.isPictograph_ = true;
						textBlock.pictographParam_ = array;
						textBlock.width_ = num7;
						num5 += (float)num7;
						if (textWidth_ < num5)
						{
							textWidth_ = num5;
						}
						textBlock = new TextBlock(this, font2, colorOfRGB, 0f, 0f, blink);
						fastVector.AddElement(textBlock);
					}
					else if (num15 > 0f)
					{
						num15 = font2.StringWidthF(textBlock.text_ + c) - textBlock.width_;
						textBlock.text_ += c;
						textBlock.width_ += num15;
						num5 += num15;
						if (textWidth_ < num5)
						{
							textWidth_ = num5;
						}
					}
				}
				if (num6 < fastVector.Size())
				{
					if (num == 2)
					{
						if (!((width_ != -1f)))
						{
							num3 = (num3 - num5 / 2f);
						}
						else
						{
							num3 = (num3 + (width_ - num3) / 2f - num5 / 2f);
						}
					}
					if (num == 4)
					{
						if (!((width_ != -1f)))
						{
							num3 = (num3 - num5);
						}
						else
						{
							num3 = (width_ - num5);
						}
					}
					for (int n = num6; n < fastVector.Size(); n++)
					{
						TextBlock textBlock3 = (TextBlock)fastVector.ElementAt(n);
						textBlock3.x_ = num3;
						textBlock3.y_ = num4 + (float)((height - textBlock3.font_.GetHeight()) / 2);
						num3 += textBlock3.width_;
					}
					num4 += (float)height;
				}
				if (num2 == 32) { if (height_ == -1f) { num4 = -num4 / 2f; } else { num4 = (height_ - num4) / 2f; } } else if (num2 == 64) { if (height_ == -1f) { num4 = -num4; } else { num4 = height_ - num4; } } else { num4 = 0f; }
				if (height_ != -1f && num4 < 0f)
				{
					num4 = 0f;
				}
				textBlocks_ = new TextBlock[fastVector.Size()];
				for (int num19 = 0; num19 < textBlocks_.Length; num19++)
				{
					textBlocks_[num19] = (TextBlock)fastVector.ElementAt(num19);
					textBlocks_[num19].y_ += num4;
				}
			}
			finally
			{
				Font.LT_DISABLE_TEXTLAYOUT = false;
				if (flag2)
				{
					Font.PopLocalizeSetting();
					font_ = font;
				}
				if (!localizeFontEnable_)
				{
					Font.PopLocalizeSetting();
				}
				Graphics.PopAutoTextLayout();
				if (width_ != -1f)
				{
					for (int num20 = 0; num20 < textBlocks_.Length; num20++)
					{
						if (textBlocks_[num20].x_ + textBlocks_[num20].width_ > width_ || textBlocks_[num20].y_ + (float)textBlocks_[num20].font_.GetHeight() > height_)
						{
							overArea_ = true;
							break;
						}
					}
				}
			}
		}

		public int GetHeight()
		{
			float num = 0f;
			for (int i = 0; i < textBlocks_.Length; i++)
			{
				TextBlock textBlock = textBlocks_[i];
				if (textBlock.text_.Length() != 0 && textBlock.y_ + (float)textBlock.font_.GetHeight() > num)
				{
					num = textBlock.y_ + (float)textBlock.font_.GetHeight();
				}
			}
			return (int)(num + 0.9f);
		}

		public int GetNextHeight()
		{
			long num = beginTime_;
			if (num == 0L)
			{
				num = JSystem.CurrentTimeMillis() - 50;
			}
			int num2 = (int)(JSystem.CurrentTimeMillis() - num) * speed_ / 50;
			if (num2 < 0)
			{
				num2 = 0;
			}
			if (num2 > length_ * 1000)
			{
				num2 = length_ * 1000;
			}
			int num3 = 0;
			int num4 = num2 / 1000;
			for (int i = 0; i < textBlocks_.Length; i++)
			{
				if (num4 <= 0)
				{
					break;
				}
				TextBlock textBlock = textBlocks_[i];
				num4 -= textBlock.text_.Length();
				if (textBlock.text_.Length() != 0 && textBlock.y_ + (float)textBlock.font_.GetHeight() > (float)num3)
				{
					num3 = (int)(textBlock.y_ + (float)textBlock.font_.GetHeight());
				}
			}
			return num3;
		}

		public void Draw(Graphics g, float dx, float dy)
		{
			Draw(g, dx, dy, 0);
		}

		public void Draw(Graphics g, float dx, float dy, int scrollV)
		{
			bool flag = false;
			if (g.CheckLocalMatrix())
			{
				g.GetFont();
				font_.GetHeight();
				font_.GetWidth();
				RectangleF transRectGUI = g.GetTransRectGUI(0f, 0f, font_.GetWidth(), font_.GetHeight());
				float num = transRectGUI.Width() / font_.GetWidth();
				float num2 = transRectGUI.Height() / (float)font_.GetHeight() / num;
				if (num2 < 1f)
				{
					flag = true;
					g.PushMatrix(Matrix.ScaleTemporary(num2, 1f));
				}
			}
			g.BeginLocalCoordination(ref dx, ref dy);
			try
			{
				Graphics.PushAutoTextLayout(false);
				if (g.GetScale() != scaleRatio_)
				{
					SetGraphicsScale(g.GetScale());
					_makeTextBlocks();
				}
				Font.PushLocalizeSetting(false);
				int num3 = 0;
				if (!Language.Japanese() && 0f < height_)
				{
					num3 = 2;
				}
				if (renderRectVisible_ && width_ != -1f && height_ != -1f)
				{
					g.SetColor(0, 255, 0);
					g.DrawRect2(dx - 2f, dy - 2f, width_ + 4f, height_ + 4f);
				}
				if (textCheckSize_ != 0 && autoScale_ && width_ != -1f && height_ != -1f)
				{
					int height = textCheckSize_;
					if (height == -1)
					{
						height = font_.GetHeight();
					}
					int renderingFontSize = GetRenderingFontSize();
					if (renderingFontSize != -1 && renderingFontSize != font_.GetHeight() && renderingFontSize < height)
					{
						g.SetColor(255, 0, 0);
						g.DrawRect2(dx - 2f, dy - 2f, width_ + 4f, height_ + 4f);
						g.DrawRect2(dx - 3f, dy - 3f, width_ + 6f, height_ + 6f);
					}
				}
				if (overArea_)
				{
					g.ClipRect(dx, dy, width_, height_ + (float)num3);
				}
				int num4 = pos_ / 1000;
				for (int i = 0; i < textBlocks_.Length; i++)
				{
					if (num4 <= 0)
					{
						break;
					}
					num4 -= textBlocks_[i].Draw(g, dx, dy - (float)scrollV, dy, height_, num4);
				}
			}
			finally
			{
				Font.PopLocalizeSetting();
				g.EndLocalCoordination();
				if (flag)
				{
					g.PopMatrix();
				}
				Graphics.PopAutoTextLayout();
			}
			if (overArea_)
			{
				g.PopClip();
			}
			if (draw_ < int.MaxValue)
			{
				draw_++;
			}
		}

		public void Frame()
		{
			if (beginTime_ == 0L)
			{
				beginTime_ = JSystem.CurrentTimeMillis() - 50;
			}
			long num = (JSystem.CurrentTimeMillis() - beginTime_) * speed_ / 50;
			if (num < 0)
			{
				num = 0L;
			}
			if (num > length_ * 1000)
			{
				num = length_ * 1000;
			}
			pos_ = (int)num;
			for (int i = 0; i < textBlocks_.Length; i++)
			{
				if (textBlocks_[i].blink_ > 0)
				{
					textBlocks_[i].blinkCnt_ = (textBlocks_[i].blinkCnt_ + 1) % (textBlocks_[i].blink_ * 2);
				}
			}
			if (frame_ < int.MaxValue)
			{
				frame_++;
			}
		}

		public void Reset()
		{
			pos_ = 0;
			beginTime_ = 0L;
		}

		public void SetSpeed(int speed)
		{
			speed_ = speed;
			if (!Language.Japanese())
			{
				speed_ = speed_ * 3 / 2;
			}
		}

		public int GetSpeed()
		{
			return speed_;
		}

		public void SetColor(int color)
		{
			color_ = color;
		}

		public int GetColor()
		{
			return color_;
		}

		public void SetFontSize(int size)
		{
			adjustFontSize_ = false;
			font_.SetSize(size);
		}

		public void SetFont(Font font)
		{
			adjustFontSize_ = false;
			font_ = new Font(font);
		}

		public Font GetFont()
		{
			return font_;
		}

		public void SetAnchor(int anchor)
		{
			anchor_ = anchor;
		}

		public int GetAnchor()
		{
			return anchor_;
		}

		public void Flush()
		{
			pos_ = length_ * 1000;
			beginTime_ = JSystem.CurrentTimeMillis() - (long)(length_ * 1000) * 1000L / speed_ / 20;
		}

		public bool IsEnd()
		{
			if (pos_ != -1)
			{
				return pos_ >= length_ * 1000;
			}
			return true;
		}

		public void SetSize(float w, float h)
		{
			width_ = w;
			height_ = h;
		}

		public void SetLineSpace(int lineSpace)
		{
			lineSpace_ = lineSpace;
		}

		public void SetTextFormat(bool enable)
		{
			format_ = enable;
		}

		public void Dispose()
		{
			for (int num = textBlocks_.Length - 1; num >= 0; num--)
			{
				textBlocks_[num].Dispose();
				textBlocks_[num] = null;
			}
			font_.Dispose();
			font_ = null;
			text_ = "";
		}

		public static void Draw(Graphics g, string text, float x, float y, int color, int anchor = 0)
		{
			Draw(null, g, text, x, y, -1f, -1f, color, Graphics.GetLineSpace(), anchor);
		}

		public static void Draw(TextFormat lf, Graphics g, string text, float x, float y, int color, int anchor = 0)
		{
			Draw(lf, g, text, x, y, -1f, -1f, color, Graphics.GetLineSpace(), anchor);
		}

		public static void Draw(Graphics g, string text, float x, float y, float w, float h, int color, int lineSpace, int anchor = 0)
		{
			Draw(null, g, text, x, y, w, h, color, lineSpace, anchor);
		}

		public static void Draw(TextFormat lf, Graphics g, string text, float x, float y, float w, float h, int color, int lineSpace, int anchor = 0)
		{
			Font font = g.GetFont();
			float scale = g.GetScale();
			if (anchor == 0)
			{
				anchor = 17;
			}
			int num = -1;
			if (!Language.Japanese() && lf != null)
			{
				if (lf.GetFont() != -1 && lf.GetFont() != font.GetSize())
				{
					num = font.GetSize();
					font.SetSize(lf.GetFont());
				}
				x = (int)(lf.GetX(x) + 0.01f);
				y = (int)(lf.GetY(y) + 0.01f);
				anchor = lf.GetAnchor(anchor);
			}
			bool num2 = (lf == null || lf.GetFont() == -1) && font.IsNeedLocalizeScale();
if (num2) {
					num = font.GetSize();
					int height = font.GetHeight();
					font.SetSize(font.GetLocalizeScale());
					if ((anchor & 0x60) == 0)
					{
						y += (float)(height - font.GetHeight()) / 2f;
					}
					Font.LockLocalizeSetting();
				}
			if ((anchor & 0x60) != 0)
			{
				g.fontScaleOffsetEnable_ = false;
			}
			TextLayout textLayout = SearchCache(text, w, h, color, lineSpace, anchor, font.GetSize(), scale);
			if (textLayout != null)
			{
				textLayout.Draw(g, x, y);
			}
			else
			{
				textLayout = new TextLayout(true);
				textLayout.SetGraphicsScale(scale);
				textLayout.SetSize(w, h);
				textLayout.SetColor(color);
				textLayout.SetLineSpace(lineSpace);
				textLayout.SetAnchor(anchor);
				textLayout.multiLine_ = lineSpace > 0;
				textLayout.SetFontSize(font.GetSize());
				textLayout.SetText(text);
				textLayout.Flush();
				AddCache(textLayout);
				textLayout.Draw(g, x, y);
			}
			if ((anchor & 0x60) != 0)
			{
				g.fontScaleOffsetEnable_ = true;
			}
			if (num2)
			{
				Font.UnlockLocalizeSetting();
			}
			if (num != -1)
			{
				font.SetSize(num);
			}
		}

		internal static int StringWidth(Font font, string text)
		{
			return (int)(StringWidthF(null, font, text) + 0.01f);
		}

		internal static int StringWidth(TextFormat lf, Font font, string text)
		{
			return (int)(StringWidthF(lf, font, text) + 0.01f);
		}

		internal static float StringWidthF(Font font, string text)
		{
			return StringWidthF(null, font, text);
		}

		internal static float StringWidthF(TextFormat lf, Font font, string text)
		{
			int num = -1;
			if (!Language.Japanese() && lf != null && lf.GetFont() != -1 && lf.GetFont() != font.GetSize())
			{
				num = font.GetSize();
				font.SetSize(lf.GetFont());
			}
			bool flag = (lf == null || lf.GetFont() == -1) && font.IsNeedLocalizeScale();
			if (flag)
			{
				num = font.GetSize();
				font.GetHeight();
				font.SetSize(font.GetLocalizeScale());
				Font.LockLocalizeSetting();
			}
			int anchor = 17;
			int num2 = 1000;
			int height = font.GetHeight();
			int colorOfRGB = Graphics.GetColorOfRGB(0, 0, 0);
			TextLayout textLayout = SearchCache(text, num2, height, Graphics.GetColorOfRGB(0, 0, 0), 0, anchor, font.GetSize(), 100f);
			if (textLayout == null)
			{
				textLayout = new TextLayout(true);
				textLayout.SetGraphicsScale(100f);
				textLayout.SetSize(num2, height);
				textLayout.SetColor(colorOfRGB);
				textLayout.SetLineSpace(0);
				textLayout.SetAnchor(anchor);
				textLayout.multiLine_ = false;
				textLayout.SetFontSize(font.GetSize());
				textLayout.SetText(text);
				textLayout.Flush();
				AddCache(textLayout);
			}
			float textWidthF = textLayout.GetTextWidthF();
			if (flag)
			{
				Font.UnlockLocalizeSetting();
			}
			if (num != -1)
			{
				font.SetSize(num);
			}
			return textWidthF;
		}

		private static TextLayout SearchCache(string text, float w, float h, int color, int lineSpace, int anchor, int font, float scaleRatio)
		{
			int hashCode = text.GetHashCode();
			for (int i = 0; i < cache_.Length; i++)
			{
				TextLayout textLayout = cache_[i];
				if (textLayout != null && textLayout.textHashCode_ == hashCode && textLayout.text_.Length() == text.Length() && textLayout.text_.Equals(text) && textLayout.width_ == w && textLayout.height_ == h && textLayout.color_ == color && textLayout.lineSpace_ == lineSpace && textLayout.anchor_ == anchor && textLayout.font_.GetSize() == font && textLayout.scaleRatio_ == scaleRatio)
				{
					return textLayout;
				}
			}
			return null;
		}

		private static void AddCache(TextLayout lay)
		{
			if (cache_[cachePointer_] != null)
			{
				cache_[cachePointer_].Dispose();
			}
			cache_[cachePointer_] = lay;
			cachePointer_ = (cachePointer_ + 1) % cache_.Length;
		}

		public int GetTextWidth()
		{
			return (int)(textWidth_ + 0.9f);
		}

		public float GetTextWidthF()
		{
			return textWidth_;
		}

		public void SetGraphicsScale(float scaleRatio)
		{
			scaleRatio_ = scaleRatio;
		}

		public int GetRenderingFontSize()
		{
			if (textBlocks_.Length != 0)
			{
				return textBlocks_[0].font_.GetHeight();
			}
			return -1;
		}

		public int GetRenderingWidth()
		{
			return (int)width_;
		}

		public int GetRenderingHeight()
		{
			return (int)height_;
		}

		public float GetRenderingWidthF()
		{
			return width_;
		}

		public float GetRenderingHeightF()
		{
			return height_;
		}

		public void SetTextCheckSize(int size = -1)
		{
			if (Config.DEBUG)
			{
				textCheckSize_ = size;
			}
		}

		public static void SetRenderRectVisible(bool visible)
		{
			renderRectVisible_ = visible;
		}

		public static bool GetRenderRectVisible()
		{
			return renderRectVisible_;
		}

		public static Pictograph AddPictograph(string key, Seb seb, int frame = -1, int wait = 1, int x = 0, int y = 0, int margin = 0)
		{
			if (wait < 1)
			{
				wait = 1;
			}
			Pictograph pictograph = GetPictograph(key);
			if (pictograph != null) {
				pictograph.Seb = seb;
				pictograph.Frame = frame;
				pictograph.Wait = wait;
				pictograph.X = x;
				pictograph.Y = y;
				pictograph.Margin = margin;
				pictograph.Transform = new Matrix();
			} else {
				pictographList_[key] = (pictograph = new Pictograph(key, seb, frame, wait, x, y, margin));
			}
			return pictograph;
		}

		public static Pictograph GetPictograph(string key)
		{
			Pictograph value;
			pictographList_.TryGetValue(key, out value);
			return value;
		}

		public static void RemovePictograph(string key)
		{
			pictographList_.Remove(key);
		}

		public static void ClearPictographs()
		{
			pictographList_.Clear();
		}

		public static void SetPictographVisible(bool visible)
		{
			pictographVisible_ = visible;
		}

		public static bool IsPictographVisible()
		{
			return pictographVisible_;
		}

		public void SetLocalizeFontEnable(bool enable)
		{
			localizeFontEnable_ = enable;
		}

		public static string RemovePositionTag(string text)
		{
			return Regex.Replace(text, "<(po=[a-zA-Z0-9]+|/po)>", "");
		}

		public static string RemoveColorTag(string text)
		{
			return Regex.Replace(text, "<(co=[a-zA-Z0-9,]+|/co)>", "");
		}
	}
}
