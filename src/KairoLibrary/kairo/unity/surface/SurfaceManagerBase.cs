using System.Collections.Generic;
using java.util;
using kairo.common.cfg;
using kairo.unity.ui;
using UnityEngine;

namespace kairo.unity.surface
{
	public abstract class SurfaceManagerBase
	{
		protected Vector<SurfaceBase> surfaceList_ = new Vector<SurfaceBase>();

		private TouchComponent[] components_ = new TouchComponent[0];

		private List<Vector2[]> paintRects_ = new List<Vector2[]>();

		private int currentPaintRect_ = -1;

		private int currentPaintPoint_ = -1;

		private static bool autoGLRender_;

		private static bool autoGLRenderClipEnable_;

		private int[] getBoundingRect_tmp = new int[4];

		public Vector<SurfaceBase> GetSurfaceList()
		{
			return surfaceList_;
		}

		public abstract void AdjustSurfaceLayout();

		public void ClearTouchComponent()
		{
			for (int num = surfaceList_.Size() - 1; num >= 0; num--)
			{
				surfaceList_.ElementAt(num).ClearTouchComponent();
			}
		}

		public void DoEvents()
		{
			for (int i = 0; i < surfaceList_.Size(); i++)
			{
				surfaceList_.ElementAt(i).DoBegin();
			}
			int num = 0;
			for (int j = 0; j < surfaceList_.Size(); j++)
			{
				num += surfaceList_.ElementAt(j).GetComponentLength();
			}
			if (components_.Length < num)
			{
				components_ = new TouchComponent[num];
			}
			int num2 = 0;
			for (int k = 0; k < surfaceList_.Size(); k++)
			{
				SurfaceBase surfaceBase = surfaceList_.ElementAt(k);
				for (int l = 0; l < surfaceBase.GetComponentLength(); l++)
				{
					components_[num2++] = surfaceBase.GetComponent(l);
				}
			}
			for (int m = 0; m < num - 1; m++)
			{
				for (int num3 = num - 1; num3 > m; num3--)
				{
					if (components_[num3 - 1].GetPriority() > components_[num3].GetPriority())
					{
						TouchComponent touchComponent = components_[num3];
						components_[num3] = components_[num3 - 1];
						components_[num3 - 1] = touchComponent;
					}
				}
			}
			for (int num4 = num - 1; num4 >= 0; num4--)
			{
				TouchComponent touchComponent2 = components_[num4];
				if (touchComponent2.surface_.DoEvent(touchComponent2))
				{
					break;
				}
			}
			for (int num5 = surfaceList_.Size() - 1; num5 >= 0; num5--)
			{
				surfaceList_.ElementAt(num5).DoEnd();
			}
			if (Config.DEBUG && surfaceList_.Size() > 0 && (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)))
			{
				SurfaceBase surfaceBase2 = surfaceList_.FirstElement();
				float num6 = (Input.mousePosition.x - (float)surfaceBase2.GetX()) * 100f / surfaceBase2.GetScaleRatio();
				float num7 = ((float)Screen.height - Input.mousePosition.y - (float)surfaceBase2.GetY()) * 100f / surfaceBase2.GetScaleRatio();
				float num8 = num6;
				float num9 = num7;
				if (num8 < 0f)
				{
					num8 = 0f;
				}
				if (num8 > (float)(surfaceBase2.GetWidth() * 100) / surfaceBase2.GetScaleRatio())
				{
					num8 = (float)(surfaceBase2.GetWidth() * 100) / surfaceBase2.GetScaleRatio();
				}
				if (num9 < 0f)
				{
					num9 = 0f;
				}
				if (num9 > (float)(surfaceBase2.GetHeight() * 100) / surfaceBase2.GetScaleRatio())
				{
					num9 = (float)(surfaceBase2.GetHeight() * 100) / surfaceBase2.GetScaleRatio();
				}
				if (Input.GetMouseButtonDown(0))
				{
					int rectIndex;
					int handleIndex;
					if (!GetSelectPaintPoint(num6, num7, out rectIndex, out handleIndex))
					{
						paintRects_.Add(new Vector2[2]
						{
							new Vector2(num8, num9),
							new Vector2(num8, num9)
						});
						currentPaintRect_ = paintRects_.Count - 1;
						currentPaintPoint_ = 1;
						return;
					}
					currentPaintRect_ = rectIndex;
					int[] paintRect = GetPaintRect(paintRects_[currentPaintRect_]);
					if (handleIndex == 0)
					{
						paintRects_[currentPaintRect_][0].Set(paintRect[0], paintRect[1]);
						paintRects_[currentPaintRect_][1].Set(paintRect[0] + paintRect[2], paintRect[1] + paintRect[3]);
						currentPaintPoint_ = 0;
					}
					if (handleIndex == 1)
					{
						paintRects_[currentPaintRect_][0].Set(paintRect[0], paintRect[1] + paintRect[3]);
						paintRects_[currentPaintRect_][1].Set(paintRect[0] + paintRect[2], paintRect[1]);
						currentPaintPoint_ = 1;
					}
					if (handleIndex == 2)
					{
						paintRects_[currentPaintRect_][0].Set(paintRect[0], paintRect[1]);
						paintRects_[currentPaintRect_][1].Set(paintRect[0] + paintRect[2], paintRect[1] + paintRect[3]);
						currentPaintPoint_ = 1;
					}
					if (handleIndex == 3)
					{
						paintRects_[currentPaintRect_][0].Set(paintRect[0], paintRect[1] + paintRect[3]);
						paintRects_[currentPaintRect_][1].Set(paintRect[0] + paintRect[2], paintRect[1]);
						currentPaintPoint_ = 0;
					}
				}
				else if (Input.GetMouseButtonDown(1))
				{
					if (paintRects_.Count > 0)
					{
						if (currentPaintRect_ != -1)
						{
							paintRects_.RemoveAt(currentPaintRect_);
						}
						else
						{
							paintRects_.RemoveAt(paintRects_.Count - 1);
						}
					}
					currentPaintRect_ = (currentPaintPoint_ = -1);
				}
				else if (Input.GetMouseButton(0) & (currentPaintRect_ != -1))
				{
					paintRects_[currentPaintRect_][currentPaintPoint_].x = num8;
					paintRects_[currentPaintRect_][currentPaintPoint_].y = num9;
				}
				else
				{
					currentPaintRect_ = -1;
				}
			}
			else
			{
				currentPaintRect_ = -1;
			}
		}

		private bool GetSelectPaintPoint(float x, float y, out int rectIndex, out int handleIndex)
		{
			rectIndex = -1;
			handleIndex = -1;
			float num = 36f;
			for (int i = 0; i < paintRects_.Count; i++)
			{
				Vector2[] paintHandlePoint = GetPaintHandlePoint(paintRects_[i]);
				for (int j = 0; j < paintHandlePoint.Length; j++)
				{
					float num2 = x - paintHandlePoint[j].x;
					float num3 = y - paintHandlePoint[j].y;
					float num4 = num2 * num2 + num3 * num3;
					if (num4 <= num)
					{
						num = num4;
						rectIndex = i;
						handleIndex = j;
					}
				}
			}
			return rectIndex != -1;
		}

		private int[] GetPaintRect(Vector2[] points)
		{
			float num = float.MaxValue;
			float num2 = float.MaxValue;
			float num3 = float.MinValue;
			float num4 = float.MinValue;
			for (int i = 0; i < points.Length; i++)
			{
				Vector2 vector = points[i];
				if (vector.x < num)
				{
					num = vector.x;
				}
				if (vector.y < num2)
				{
					num2 = vector.y;
				}
				if (vector.x > num3)
				{
					num3 = vector.x;
				}
				if (vector.y > num4)
				{
					num4 = vector.y;
				}
			}
			int num5 = (int)(num + 0.1f);
			int num6 = (int)(num2 + 0.1f);
			int num7 = (int)(num3 + 0.1f) - num5;
			int num8 = (int)(num4 + 0.1f) - num6;
			return new int[4] { num5, num6, num7, num8 };
		}

		private Vector2[] GetPaintHandlePoint(Vector2[] points)
		{
			int[] paintRect = GetPaintRect(points);
			return new Vector2[4]
			{
				new Vector2(paintRect[0] - 1, paintRect[1] - 1),
				new Vector2(paintRect[0] + paintRect[2], paintRect[1] - 1),
				new Vector2(paintRect[0] + paintRect[2], paintRect[1] + paintRect[3]),
				new Vector2(paintRect[0] - 1, paintRect[1] + paintRect[3])
			};
		}

		public void BeginTouchRegist(kairo.unity.ui.Graphics g)
		{
			for (int i = 0; i < TouchOption.buffer.Size(); i++)
			{
				TouchOption touchOption = TouchOption.buffer.ElementAt(i);
				if (touchOption.hasCount <= 0)
				{
					touchOption.Lock = false;
				}
			}
			for (int num = surfaceList_.Size() - 1; num >= 0; num--)
			{
				surfaceList_.ElementAt(num).BeginTouchRegist(g);
			}
		}

		public void EndTouchRegist(kairo.unity.ui.Graphics g)
		{
			for (int num = surfaceList_.Size() - 1; num >= 0; num--)
			{
				surfaceList_.ElementAt(num).EndTouchRegist(g);
			}
		}

		public virtual void Draw(kairo.unity.ui.Graphics g)
		{
			bool flag = IsAutoGLRender();
			for (int i = 0; i < surfaceList_.Size(); i++)
			{
				g.ResetRender();
				if (flag)
				{
					g.BeginGLRender(autoGLRenderClipEnable_);
				}
				surfaceList_.ElementAt(i).OnDraw(g);
				if (flag)
				{
					g.EndGLRender();
				}
			}
			if (!Config.DEBUG || surfaceList_.Size() <= 0 || paintRects_.Count <= 0)
			{
				return;
			}
			SurfaceBase surfaceBase = surfaceList_.FirstElement();
			g.SetRenderRect(surfaceBase.GetX(), surfaceBase.GetY(), surfaceBase.GetWidth(), surfaceBase.GetHeight());
			g.Scale(surfaceBase.GetScaleRatio());
			try
			{
				float x = (Input.mousePosition.x - (float)surfaceBase.GetX()) * 100f / surfaceBase.GetScaleRatio();
				float y = ((float)Screen.height - Input.mousePosition.y - (float)surfaceBase.GetY()) * 100f / surfaceBase.GetScaleRatio();
				int rectIndex;
				int handleIndex;
				GetSelectPaintPoint(x, y, out rectIndex, out handleIndex);
				for (int j = 0; j < paintRects_.Count; j++)
				{
					int[] paintRect = GetPaintRect(paintRects_[j]);
					g.SetColor(255, 0, 0);
					g.DrawRect2(paintRect[0] - 1, paintRect[1] - 1, paintRect[2] + 2, paintRect[3] + 2);
					if (currentPaintRect_ != j && (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)))
					{
						Vector2[] paintHandlePoint = GetPaintHandlePoint(paintRects_[j]);
						for (int k = 0; k < paintHandlePoint.Length; k++)
						{
							g.SetColor(255, 150, 255);
							if (currentPaintRect_ == -1 && rectIndex == j && handleIndex == k)
							{
								g.SetColor(255, 255, 255);
							}
							g.FillRect(paintHandlePoint[k].x - 0.5f, paintHandlePoint[k].y - 0.5f, 2f, 2f);
						}
					}
					g.PushFont(6);
					int height = g.GetFont().GetHeight();
					string text = paintRect[0] + "," + paintRect[1];
					int num = paintRect[0];
					int num2 = paintRect[1] - 1 - height;
					if (num2 < 0)
					{
						num2 = 0;
					}
					g.SetColor(0, 0, 0, 150);
					g.FillRect((float)num - 0.5f, (float)num2 - 0.5f, g.StringWidth(text) + 1, height + 1);
					g.SetColor(255, 255, 255);
					g.DrawString(text, num, num2);
					text = paintRect[2] + "x" + paintRect[3];
					num = paintRect[0] + paintRect[2] - g.StringWidth(text);
					num2 = paintRect[1] + paintRect[3] + 1;
					if ((float)(num2 + height) > (float)(surfaceBase.GetHeight() * 100) / surfaceBase.GetScaleRatio())
					{
						num2 = (int)((float)(surfaceBase.GetHeight() * 100) / surfaceBase.GetScaleRatio() - (float)height);
					}
					g.SetColor(0, 0, 0, 150);
					g.FillRect((float)num - 0.5f, (float)num2 - 0.5f, g.StringWidth(text) + 1, height + 1);
					g.SetColor(255, 255, 255);
					g.DrawString(text, num, num2);
					g.PopFont();
				}
			}
			finally
			{
				g.Scale(100f);
				g.ClearRenderRect();
			}
		}

		public int[] GetBoundingRect()
		{
			int[] array = getBoundingRect_tmp;
			if (surfaceList_.Size() == 0)
			{
				array[0] = (array[1] = (array[2] = (array[3] = 0)));
				return array;
			}
			int num = int.MaxValue;
			int num2 = int.MinValue;
			int num3 = int.MaxValue;
			int num4 = int.MinValue;
			for (int i = 0; i < surfaceList_.Size(); i++)
			{
				SurfaceBase surfaceBase = surfaceList_.ElementAt(i);
				if (surfaceBase.GetX() < num)
				{
					num = surfaceBase.GetX();
				}
				if (surfaceBase.GetY() < num3)
				{
					num3 = surfaceBase.GetY();
				}
				if (surfaceBase.GetX() + surfaceBase.GetWidth() > num2)
				{
					num2 = surfaceBase.GetX() + surfaceBase.GetWidth();
				}
				if (surfaceBase.GetY() + surfaceBase.GetHeight() > num4)
				{
					num4 = surfaceBase.GetY() + surfaceBase.GetHeight();
				}
			}
			array[0] = num;
			array[1] = num3;
			array[2] = num2 - num;
			array[3] = num4 - num3;
			return array;
		}

		public void CheckScreenRotation()
		{
		}

		public void CheckScreenRotation(bool skip)
		{
		}

		public virtual void DrawComponents(kairo.unity.ui.Graphics g)
		{
			for (int i = 0; i < surfaceList_.Size(); i++)
			{
				SurfaceBase surfaceBase = surfaceList_.ElementAt(i);
				double num = (double)surfaceBase.GetScaleRatio() / 100.0;
				for (int num2 = surfaceBase.GetComponentLength() - 1; num2 >= 0; num2--)
				{
					TouchComponent component = surfaceBase.GetComponent(num2);
					g.SetColor(0, 255, 255);
					g.SetRenderMode(1, 32, 223);
					g.FillRect(surfaceBase.GetX() + (int)((double)component.x * num), surfaceBase.GetY() + (int)((double)component.y * num), (int)((double)component.width * num), (int)((double)component.height * num));
					g.SetRenderMode();
					g.SetColor(255, 0, 0);
					g.DrawRect(surfaceBase.GetX() + (int)((double)component.x * num), surfaceBase.GetY() + (int)((double)component.y * num), (int)((double)component.width * num) - 1, (int)((double)component.height * num) - 1);
				}
			}
		}

		public void OnKeyDown(int key)
		{
			for (int num = surfaceList_.Size() - 1; num >= 0; num--)
			{
				surfaceList_.ElementAt(num).OnKeyDown(key);
			}
		}

		public void OnKeyUp(int key)
		{
			for (int num = surfaceList_.Size() - 1; num >= 0; num--)
			{
				surfaceList_.ElementAt(num).OnKeyUp(key);
			}
		}

		public void OnKeyDown(int keyCode, KeyEvent evt)
		{
			for (int num = surfaceList_.Size() - 1; num >= 0; num--)
			{
				surfaceList_.ElementAt(num).OnKeyDown(keyCode, evt);
			}
		}

		public void OnKeyUp(int keyCode, KeyEvent evt)
		{
			for (int num = surfaceList_.Size() - 1; num >= 0; num--)
			{
				surfaceList_.ElementAt(num).OnKeyUp(keyCode, evt);
			}
		}

		public void DisposeRotatingBuffer()
		{
		}

		public bool CheckRotatingBuffer()
		{
			return false;
		}

		public bool CheckNativeRotation()
		{
			return true;
		}

		public bool IsRotating()
		{
			return false;
		}

		public static void SetAutoGLRender(bool use, bool clipEnable = true)
		{
			autoGLRender_ = use;
			autoGLRenderClipEnable_ = clipEnable;
		}

		public static bool IsAutoGLRender()
		{
			return autoGLRender_;
		}
	}
}
