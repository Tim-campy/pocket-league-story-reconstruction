using System;
using System.Collections.Generic;
using System.Linq;
using kairo.unity.graphics;
using kairo.unity.math;
using kairo.unity.native;
using kairo.unity.util;
using UnityEngine;

namespace kairo.unity.ui
{
	public class Balloon
	{
		public delegate void BalloonRender(Graphics g, float y, RectangleF left, RectangleF right);

		public const int COLOR_BLUE = -15972869;

		public const int COLOR_RED = -51144;

		public const int COLOR_BLACK = -14803426;

		public const int COLOR_SHADOW = -5460820;

		public string Text
		{
			get
			{
				return text_;
			}
			set
			{
				text_ = Language.LT(value);
			}
		}

		public int TextColor { get; set; }

		public float Scale { get; set; }

		public Seb Seb { get; set; }

		public int FontSize { get; set; }

		internal float scale2_;

		private string text_;

		private bool isTail_;

		private int leftSpace_;

		private int rightSpace_;

		private int topSpace_;

		private int bottomSpace_;

		private bool isClip_;

		private int shadowColor_;

		private bool isShadow_;

		private Seb leftSpaceSeb_;

		private int leftSpaceSebFrame_;

		private Seb rightSpaceSeb_;

		private int rightSpaceSebFrame_;

		private BalloonRender render_;

		private List<Rectangle> maskList_ = new List<Rectangle>();

		private RectangleF clip_ = new RectangleF();

		private RectangleF leftSpaceRect_ = new RectangleF();

		private RectangleF rightSpaceRect_ = new RectangleF();

		private static RectangleF Draw_tmp = new RectangleF();

		public Balloon(Balloon balloon)
		{
			Init(balloon);
		}

		public Balloon(string text, int color = -15972869)
		{
			Init(text, color);
		}

		public void Init(string text, int color = -15972869)
		{
			Text = text;
			TextColor = color;
			shadowColor_ = -5460820;
			isShadow_ = true;
			Scale = 1f;
			scale2_ = 1f;
			isTail_ = true;
			leftSpaceSeb_ = null;
			rightSpaceSeb_ = null;
			leftSpace_ = 0;
			rightSpace_ = 0;
			topSpace_ = 0;
			bottomSpace_ = 0;
			render_ = null;
			isClip_ = false;
			Seb = null;
			FontSize = 9;
			maskList_.Clear();
		}

		public void Init(Balloon from)
		{
			Text = from.Text;
			TextColor = from.TextColor;
			shadowColor_ = from.shadowColor_;
			isShadow_ = from.isShadow_;
			Scale = from.Scale;
			scale2_ = from.scale2_;
			isTail_ = from.isTail_;
			leftSpaceSeb_ = from.leftSpaceSeb_;
			leftSpaceSebFrame_ = from.leftSpaceSebFrame_;
			rightSpaceSeb_ = from.rightSpaceSeb_;
			rightSpaceSebFrame_ = from.rightSpaceSebFrame_;
			leftSpace_ = from.leftSpace_;
			rightSpace_ = from.rightSpace_;
			topSpace_ = from.topSpace_;
			bottomSpace_ = from.bottomSpace_;
			render_ = from.render_;
			isClip_ = from.isClip_;
			Seb = from.Seb;
			FontSize = from.FontSize;
			clip_.Set(from.clip_);
			maskList_.Clear();
			maskList_.AddRange(from.maskList_);
		}

		internal void AddMask(int x, int y, int width, int height)
		{
			maskList_.Add(new Rectangle(x, y, x + width, y + height));
		}

		internal void SetClip(int x, int y, int width, int height)
		{
			clip_.Set(x, y, x + width, y + height);
			isClip_ = true;
		}

		public void SetSpace(int left, int right, int top = 0, int bottom = 0, BalloonRender render = null)
		{
			leftSpace_ = left;
			rightSpace_ = right;
			topSpace_ = top;
			bottomSpace_ = bottom;
			render_ = render;
			leftSpaceSeb_ = null;
			rightSpaceSeb_ = null;
		}

		public void SetSpace(Seb leftSeb, int leftFrame, Seb rightSeb, int rightFrame, int top = 0, int bottom = 0, BalloonRender render = null)
		{
			leftSpace_ = 0;
			rightSpace_ = 0;
			topSpace_ = top;
			bottomSpace_ = bottom;
			render_ = render;
			leftSpaceSeb_ = leftSeb;
			leftSpaceSebFrame_ = leftFrame;
			rightSpaceSeb_ = rightSeb;
			rightSpaceSebFrame_ = rightFrame;
			if (leftSeb != null)
			{
				leftSpace_ = leftSeb.GetBoundingRect(leftSpaceSebFrame_)[2] + 1;
			}
			if (rightSeb != null)
			{
				rightSpace_ = rightSeb.GetBoundingRect(rightSpaceSebFrame_)[2] + 1;
			}
		}

		public void SetTail(bool show)
		{
			isTail_ = show;
		}

		public void SetShadow(bool show, int color = -5460820)
		{
			isShadow_ = show;
			shadowColor_ = color;
		}

		public void Draw(Graphics g, float x, float y)
		{
			Draw(g, x, y, Matrix4x4.identity);
		}

		public void Draw(Graphics g, float x, float y, Matrix4x4 matrix)
		{
			Text = Language.LT(Text);
			if (!g.IsGUI()) {
if (g.render_ != null) {
int num = 60;
				int num2 = 20;
				RectangleF transRectGUI = g.GetTransRectGUI(0f, 0f, num, num2, Draw_tmp);
				float num3 = transRectGUI.Width() / (float)num;
				float num4 = transRectGUI.Height() / (float)num2;
				float num5 = ((num3 < num4) ? num3 : num4);
				Image.BalloonRenderTask item = new Image.BalloonRenderTask(this, x, y, g.offscreen_);
				item.Balloon.Scale *= num5;
				if (g.IsClipEnable())
				{
					int[] clip = g.GetClip();
					if (clip != null)
					{
						item.Balloon.SetClip(clip[0], clip[1], clip[2], clip[3]);
					}
				}
				else
				{
					float[] imageClip = g.GetImageClip();
					if (imageClip != null)
					{
						item.Balloon.SetClip((int)imageClip[0], (int)imageClip[1], (int)imageClip[2], (int)imageClip[3]);
					}
				}
				g.render_.balloonRenderTasks_.Add(item);
				return;
}
} else {
float num6 = x;
			float num7 = y;
			g.PushMatrix(Matrix.ScaleTemporary(Scale * scale2_, Scale * scale2_));
			g.BeginLocalCoordination(ref x, ref y);
			g.PushFont(FontSize);
			Seb seb = Seb;
			if (seb == null)
			{
				seb = IApplication.GetCurrentApp().GetResource().seb[3];
			}
			Image[] img = seb.manager_.img;
			Font font = g.GetFont();
			int num8 = font.StringWidth(Text);
			int num9 = (num8 - 3) / 2 * 2;
			if (num9 < 0)
			{
				num9 = 0;
			}
			int num10 = seb.GetBoundingRect(0)[2];
			int num11 = seb.GetBoundingRect(2)[2];
			int num12 = seb.GetBoundingRect(0)[3];
			int num13 = seb.GetBoundingRect(6)[3];
			int num14 = num10 + leftSpace_ + num9 + rightSpace_ + num11;
			int num15 = num12 + topSpace_ + bottomSpace_ + num13;
			float num16 = x - (float)(num14 / 2) - 1f;
			float num17 = y - (float)num15 - 2f;
			float num18 = num16;
			float num19 = num17;
			float num20 = num16 + (float)num14;
			float num21 = num17 + (float)num15;
			if (isTail_)
			{
				num21 += 2f;
			}
			num18 /= Scale;
			num19 /= Scale;
			num20 /= Scale;
			num21 /= Scale;
			float num22 = num18 - 200f;
			float num23 = num19 - 200f;
			float num24 = num20 + 200f;
			float num25 = num21 + 200f;
			if (isClip_)
			{
				Vector3 vector = matrix.MultiplyPoint(new Vector3(clip_.left, clip_.top));
				Vector3 vector2 = matrix.MultiplyPoint(new Vector3(clip_.right, clip_.bottom));
				RectangleF draw_tmp = Draw_tmp;
				draw_tmp.Set(vector.x / Scale / Scale - num6 / Scale / Scale / scale2_, vector.y / Scale / Scale - num7 / Scale / Scale / scale2_, vector2.x / Scale / Scale - num6 / Scale / Scale / scale2_, vector2.y / Scale / Scale - num7 / Scale / Scale / scale2_);
				if (draw_tmp.right < num24)
				{
					num24 = draw_tmp.right;
				}
				if (draw_tmp.bottom < num25)
				{
					num25 = draw_tmp.bottom;
				}
				if (num22 < draw_tmp.left)
				{
					num22 = draw_tmp.left;
				}
				if (num23 < draw_tmp.top)
				{
					num23 = draw_tmp.top;
				}
				if (num24 < num22)
				{
					num22 = num24;
				}
				if (num25 < num23)
				{
					num23 = num25;
				}
				if (num18 < num22)
				{
					num18 = num22;
				}
				if (num19 < num23)
				{
					num19 = num23;
				}
				if (num20 > num24)
				{
					num20 = num24;
				}
				if (num21 > num25)
				{
					num21 = num25;
				}
			}
			bool flag = false;
			foreach (Rectangle item2 in maskList_)
			{
				Vector3 vector3 = matrix.MultiplyPoint(new Vector3(item2.left, item2.top));
				Vector3 vector4 = matrix.MultiplyPoint(new Vector3(item2.right, item2.bottom));
				RectangleF draw_tmp2 = Draw_tmp;
				draw_tmp2.Set(vector3.x / Scale / Scale - num6 / Scale / Scale / scale2_, vector3.y / Scale / Scale - num7 / Scale / Scale / scale2_, vector4.x / Scale / Scale - num6 / Scale / Scale / scale2_, vector4.y / Scale / Scale - num7 / Scale / Scale / scale2_);
				if (KairoMath.CheckHitRect(num18, num19, num20 - num18, num21 - num19, draw_tmp2.left, draw_tmp2.top, draw_tmp2.Width(), draw_tmp2.Height()))
				{
					bool flag2 = draw_tmp2.left <= num18 && num18 <= draw_tmp2.right;
					bool flag3 = draw_tmp2.left <= num20 && num20 <= draw_tmp2.right;
					bool flag4 = draw_tmp2.top <= num19 && num19 <= draw_tmp2.bottom;
					bool flag5 = draw_tmp2.top <= num21 && num21 <= draw_tmp2.bottom;
					int num26 = 0;
					if (flag2)
					{
						num26++;
					}
					if (flag3)
					{
						num26++;
					}
					if (flag4)
					{
						num26++;
					}
					if (flag5)
					{
						num26++;
					}
					if (num26 == 3)
					{
						if (!flag5)
						{
							num23 = (num19 = draw_tmp2.bottom);
						}
						if (!flag4)
						{
							num25 = (num21 = draw_tmp2.top);
						}
						if (!flag2)
						{
							num24 = (num20 = draw_tmp2.left);
						}
						if (!flag3)
						{
							num22 = (num18 = draw_tmp2.right);
						}
					}
					else
					{
						flag = true;
					}
					continue;
				}
				bool flag6 = draw_tmp2.right < num18;
				bool flag7 = num20 < draw_tmp2.left;
				bool flag8 = draw_tmp2.bottom < num19;
				bool flag9 = num21 < draw_tmp2.top;
				int num27 = 0;
				if (flag6)
				{
					num27++;
				}
				if (flag7)
				{
					num27++;
				}
				if (flag8)
				{
					num27++;
				}
				if (flag9)
				{
					num27++;
				}
				if (num27 == 1)
				{
					if (flag6 && draw_tmp2.right > num22)
					{
						num22 = draw_tmp2.right;
					}
					if (flag7 && draw_tmp2.left < num24)
					{
						num24 = draw_tmp2.left;
					}
					if (flag8 && draw_tmp2.bottom > num23)
					{
						num23 = draw_tmp2.bottom;
					}
					if (flag9 && draw_tmp2.top < num25)
					{
						num25 = draw_tmp2.top;
					}
				}
			}
			if (!flag)
			{
				num22 = num22 * Scale + num6;
				num23 = num23 * Scale + num7;
				num24 = num24 * Scale + num6;
				num25 = num25 * Scale + num7;
				g.ClipRect(num22 - num6, num23 - num7, num24 - num22, num25 - num23);
				float num28 = num16 + (float)num10 - (float)(num8 - num9) / 2f;
				float num29 = num16 + (float)num14 - (float)num11 - (float)rightSpace_ + (float)(num8 - num9) / 2f;
				float num30 = num17;
				seb.DrawFrame(g, num16, num17, img, 0);
				DrawRepeat(g, num16 + (float)num10, num17, num14 - num10 - num11, -1, seb, 1);
				seb.DrawFrame(g, num16 + (float)num14 - (float)num11, num17, img, 2);
				DrawRepeat(g, num16, num17 + (float)num12, -1, num15 - num12 - num13, seb, 3);
				DrawRepeat(g, num16 + (float)num10, num17 + (float)num12, num14 - num10 - num11, num15 - num12 - num13, seb, 4);
				DrawRepeat(g, num16 + (float)num14 - (float)num11, num17 + (float)num12, -1, num15 - num12 - num13, seb, 5);
				seb.DrawFrame(g, num16, num17 + (float)num15 - (float)num13, img, 6);
				DrawRepeat(g, num16 + (float)num10, num17 + (float)num15 - (float)num13, num14 - num10 - num11, -1, seb, 7);
				seb.DrawFrame(g, num16 + (float)num14 - (float)num11, num17 + (float)num15 - (float)num13, img, 8);
				if (isTail_)
				{
					seb.DrawFrame(g, x - 5f, y - 6f, img, 9);
				}
				num17 = y - (float)num15 + 2f + (float)topSpace_;
				float x2 = num16 + (float)num10 + (float)leftSpace_ - (float)(num8 - num9) / 2f;
				float num31 = num17 + (float)(9 - FontSize) / 2f;
				bool flag10 = Text.Contains('<');
				if (isShadow_)
				{
					bool pictographVisible = TextLayout.IsPictographVisible();
					TextLayout.SetPictographVisible(false);
					g.SetColor(shadowColor_);
					string str = TextLayout.RemoveColorTag(Text);
					if (KairoPlugin.IsWeakDrawDevice())
					{
						if (flag10)
						{
							g.DrawString2(str, x2, num31 + 0.5f);
						}
						else
						{
							g.DrawString(str, x2, num31 + 0.5f);
						}
					}
					else if (flag10)
					{
						g.DrawString2(str, x2, num31 + 0.5f);
						g.DrawString2(str, x2, num31 + 0.25f);
					}
					else
					{
						g.DrawString(str, x2, num31 + 0.5f);
						g.DrawString(str, x2, num31 + 0.25f);
					}
					TextLayout.SetPictographVisible(pictographVisible);
				}
				g.SetColor(TextColor);
				if (flag10)
				{
					g.DrawString2(Text, x2, num31);
				}
				else
				{
					g.DrawString(Text, x2, num31);
				}
				if (leftSpaceSeb_ != null)
				{
					short[] boundingRect = leftSpaceSeb_.GetBoundingRect(leftSpaceSebFrame_);
					leftSpaceSeb_.DrawFrame(g, num28 - (float)boundingRect[0], num17 - (float)boundingRect[1] + (float)(font.GetHeight() - boundingRect[3]) / 2f, leftSpaceSeb_.manager_.img, leftSpaceSebFrame_);
				}
				if (rightSpaceSeb_ != null)
				{
					short[] boundingRect2 = rightSpaceSeb_.GetBoundingRect(rightSpaceSebFrame_);
					rightSpaceSeb_.DrawFrame(g, num29 - (float)boundingRect2[0] + 1f, num17 - (float)boundingRect2[1] + (float)(font.GetHeight() - boundingRect2[3]) / 2f, rightSpaceSeb_.manager_.img, rightSpaceSebFrame_);
				}
				if (render_ != null)
				{
					leftSpaceRect_.Set(num28, num30 + (float)num12 - 4f, num28 + (float)leftSpace_, num30 + (float)num15 - (float)num13 + 5f);
					rightSpaceRect_.Set(num29, num30 + (float)num12 - 4f, num29 + (float)rightSpace_, num30 + (float)num15 - (float)num13 + 5f);
					render_(g, num17, leftSpaceRect_, rightSpaceRect_);
				}
				g.PopClip();
			}
			g.PopFont();
			g.EndLocalCoordination();
			g.PopMatrix();
}
		}

		private void DrawRepeat(Graphics g, float x, float y, int width, int height, Seb seb, int frame)
		{
			int[] sprite = seb.GetSprite(frame, 0);
			if (width == -1)
			{
				width = sprite[4];
			}
			if (height == -1)
			{
				height = sprite[5];
			}
			int num = 0;
			while (num < height)
			{
				int num2 = height - num;
				if (num2 > sprite[5])
				{
					num2 = sprite[5];
				}
				int num3 = 0;
				while (num3 < width)
				{
					int num4 = width - num3;
					if (num4 > sprite[4])
					{
						num4 = sprite[4];
					}
					g.DrawImage(seb.manager_.img[sprite[1]], x + (float)num3, y + (float)num, sprite[2], sprite[3], num4, num2);
					num3 += num4;
				}
				num += num2;
			}
		}

		private static float Clamp(float val, float min, float max)
		{
			if (val > max)
			{
				val = max;
			}
			if (val < min)
			{
				val = min;
			}
			return val;
		}

		private static float ClampMin(float val, float min)
		{
			if (val < min)
			{
				return min;
			}
			return val;
		}

		private static float ClampMax(float val, float max)
		{
			if (val > max)
			{
				return max;
			}
			return val;
		}
	}
}
