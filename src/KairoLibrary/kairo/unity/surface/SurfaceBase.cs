using System.Collections;
using System.Threading;
using System;
using System.Collections.Generic;
using System.Linq;
using java.lang;
using java.util;
using kairo.unity.ui;
using kairo.unity.util;
using UnityEngine;

namespace kairo.unity.surface
{
	public abstract class SurfaceBase
	{
		public class TouchPoint
		{
			public long key;

			public int id;

			public int value;

			public float x;

			public float y;

			public float dragBaseX;

			public float dragBaseY;

			public TouchComponent component;

			public void Init(long key, float x, float y, TouchComponent component)
			{
				int[] array = BitUtil.Split(key);
				this.key = key;
				this.x = x;
				this.y = y;
				id = array[0];
				value = array[1];
				dragBaseX = x;
				dragBaseY = y;
				this.component = component;
			}
		}

		public class MotionEventRecord
		{
			private class MultiTouch
			{
				private MotionEvent event_;

				public void Set(MotionEvent evt)
				{
					event_ = evt;
				}

				public int GetPointerCount()
				{
					return event_.GetPointerCount();
				}

				public int GetPointerId(int index)
				{
					return event_.GetPointerId(index);
				}

				public float GetX(int index)
				{
					try
					{
						return event_.GetX(index);
					}
					catch (Exception)
					{
						return 0f;
					}
				}

				public float GetY(int index)
				{
					try
					{
						return event_.GetY(index);
					}
					catch (Exception)
					{
						return 0f;
					}
				}

				public float GetDeltaX(int index)
				{
					try
					{
						return event_.GetDeltaX(index);
					}
					catch (Exception)
					{
						return 0f;
					}
				}

				public float GetDeltaY(int index)
				{
					try
					{
						return event_.GetDeltaY(index);
					}
					catch (Exception)
					{
						return 0f;
					}
				}

				public float GetDeltaTime(int index)
				{
					try
					{
						return event_.GetDeltaTime(index);
					}
					catch (Exception)
					{
						return 0f;
					}
				}
			}

			private static MultiTouch multiTouch_;

			public int action;

			public float x;

			public float y;

			public float deltaX;

			public float deltaY;

			public float deltaTime;

			public int pointerCount;

			public int[] pointerIds;

			public float[] pointerPos;

			public float[] pointerDelta;

			public bool isMousePointer;

			public MotionEventRecord()
			{
				Clear();
			}

			public bool IsEmpty()
			{
				return action == -1;
			}

			public void Clear()
			{
				action = -1;
			}

			public void Copy(SurfaceBase surface, MotionEvent evt)
			{
				action = evt.GetAction();
				x = evt.GetX();
				y = evt.GetY() + (float)((evt.IsMousePointer()) ? (0) : (surface.GetTouchOffset()));
				deltaX = evt.GetDeltaX();
				deltaY = evt.GetDeltaY();
				deltaTime = evt.GetDeltaTime();
				if (multiTouch_ == null)
				{
					multiTouch_ = new MultiTouch();
				}
				multiTouch_.Set(evt);
				pointerCount = multiTouch_.GetPointerCount();
				if (pointerIds == null || pointerIds.Length != pointerCount)
				{
					pointerIds = new int[pointerCount];
					pointerPos = new float[pointerCount * 2];
					pointerDelta = new float[pointerCount * 3];
				}
				for (int i = 0; i < pointerCount; i++)
				{
					pointerIds[i] = multiTouch_.GetPointerId(i);
					pointerPos[i * 2 + 0] = multiTouch_.GetX(i);
					pointerPos[i * 2 + 1] = multiTouch_.GetY(i);
					pointerDelta[i * 3 + 0] = multiTouch_.GetDeltaX(i);
					pointerDelta[i * 3 + 1] = multiTouch_.GetDeltaY(i);
					pointerDelta[i * 3 + 2] = multiTouch_.GetDeltaTime(i);
				}
				isMousePointer = evt.IsMousePointer();
			}

			public void Copy(SurfaceBase surface, MotionEventRecord evt)
			{
				action = evt.action;
				x = evt.x;
				y = evt.y;
				deltaX = evt.deltaX;
				deltaY = evt.deltaY;
				deltaTime = evt.deltaTime;
				isMousePointer = evt.isMousePointer;
				pointerCount = evt.pointerCount;
				if (evt.pointerIds != null)
				{
					if (pointerIds == null || pointerIds.Length != evt.pointerIds.Length)
					{
						pointerIds = new int[evt.pointerIds.Length];
					}
					JSystem.Arraycopy(evt.pointerIds, 0, pointerIds, 0, pointerIds.Length);
				}
				if (evt.pointerPos != null)
				{
					if (pointerPos == null || pointerPos.Length != evt.pointerPos.Length)
					{
						pointerPos = new float[evt.pointerPos.Length];
					}
					JSystem.Arraycopy(evt.pointerPos, 0, pointerPos, 0, pointerPos.Length);
				}
				if (evt.pointerDelta != null)
				{
					if (pointerDelta == null || pointerDelta.Length != evt.pointerDelta.Length)
					{
						pointerDelta = new float[evt.pointerDelta.Length];
					}
					JSystem.Arraycopy(evt.pointerDelta, 0, pointerDelta, 0, pointerDelta.Length);
				}
			}

			public int GetAction()
			{
				return action;
			}

			public float GetX()
			{
				return x;
			}

			public float GetY()
			{
				return y;
			}

			public float GetDeltaX()
			{
				return deltaX;
			}

			public float GetDeltaY()
			{
				return deltaY;
			}

			public float GetDeltaTime()
			{
				return deltaTime;
			}

			public bool IsMousePointer()
			{
				return isMousePointer;
			}

			public int GetPointerCount()
			{
				return pointerCount;
			}

			public int GetPointerId(int pointerIndex)
			{
				return pointerIds[pointerIndex];
			}

			public float GetX(int index)
			{
				return pointerPos[index * 2 + 0];
			}

			public float GetY(int index)
			{
				return pointerPos[index * 2 + 1];
			}

			public float GetDeltaX(int index)
			{
				return pointerDelta[index * 3 + 0];
			}

			public float GetDeltaY(int index)
			{
				return pointerDelta[index * 3 + 1];
			}

			public float GetDeltaTime(int index)
			{
				return pointerDelta[index * 3 + 2];
			}
		}

		private Vector<TouchComponent> componentBuffer_;

		private Vector<TouchComponent> components_;

		protected Vector<TouchPoint> startTouchList_;

		protected Vector<TouchPoint> lastTouchList_;

		protected Vector<TouchPoint> touchListBuffer_;

		private int touchDownComponentId_;

		private bool delayEraseTouchDownComponentId_;

		private bool touch_;

		private Vector<TouchEvent> touchEvents_;

		private Vector<TouchEvent> taskBuffer_;

		private Vector<TouchEvent>[] touchEventBuffer_;

		private int[] touchEventBufferNum_;

		private MotionEventRecord start_;

		private MotionEventRecord last_;

		private MotionEventRecord last2_;

		private MotionEventRecord[] delay_;

		private int delayNum_;

		private bool lock_;

		private bool multiTouchEnable_;

		private int[][] pinchPointer_;

		private int pinchDistance_;

		private int[][] pinchStartPoint_;

		private bool pinchStart_;

		private int x_;

		private int y_;

		private int width_;

		private int height_;

		protected float scaleRatio_;

		protected List<float> windowsScaleBackup_ = new List<float>();

		protected List<kairo.unity.ui.Graphics> windowsScaleGraphics_ = new List<kairo.unity.ui.Graphics>();

		protected kairo.unity.ui.Graphics graphics_;

		private int touchOffset_;

		public int lastTouchX;

		public int lastTouchY;

		private bool registTouch_;

		private object OnReceiveTouchEvent_lock1 = new object();

		private static MotionEventRecord onReceiveTouchEvent_tmp = new MotionEventRecord();

		private object OnReceiveTouchEvent_lock2 = new object();

		private object OnReceiveTrackballEvent_lock = new object();

		private static int[] screenTouchToLocal_result = new int[2];

		private static int[] localTouchToScreen_result = new int[2];

		public bool registTouch
		{
			set
			{
				registTouch_ = value;
			}
			get
			{
				if (registTouch_)
				{
					return true;
				}
				if (kairo.unity.ui.Graphics.current_ != null && !kairo.unity.ui.Graphics.current_.IsGUI())
				{
					return true;
				}
				return false;
			}
		}

		protected SurfaceBase()
		{
			componentBuffer_ = new Vector<TouchComponent>();
			components_ = new Vector<TouchComponent>();
			start_ = new MotionEventRecord();
			last_ = new MotionEventRecord();
			last2_ = new MotionEventRecord();
			startTouchList_ = new Vector<TouchPoint>();
			lastTouchList_ = new Vector<TouchPoint>();
			touchListBuffer_ = new Vector<TouchPoint>();
			touchEvents_ = new Vector<TouchEvent>();
			multiTouchEnable_ = Input.multiTouchEnabled;
			pinchPointer_ = new int[2][]
			{
				new int[3] { 0, 0, -1 },
				new int[3] { 0, 0, -1 }
			};
			pinchStartPoint_ = JTool.MakeArray<int[][]>(new int[2] { 2, 2 });
			pinchStart_ = false;
			scaleRatio_ = 100f;
			delay_ = new MotionEventRecord[0];
			delayNum_ = 0;
			lock_ = false;
			touchEventBuffer_ = new Vector<TouchEvent>[2]
			{
				new Vector<TouchEvent>(),
				new Vector<TouchEvent>()
			};
			touchEventBufferNum_ = new int[2];
			touchDownComponentId_ = -1;
			touchOffset_ = -18;
		}

		public TouchComponent AddTouchComponent(int id, int x, int y, int width, int height, int value, TouchOption option)
		{
			kairo.unity.ui.Graphics.GetCurrent();
			if (!registTouch)
			{
				return null;
			}
			TouchComponent touchComponent = null;
			for (int i = 0; i < componentBuffer_.Size(); i++)
			{
				if (!componentBuffer_.ElementAt(i).isLock)
				{
					touchComponent = componentBuffer_.ElementAt(i);
					touchComponent.Init();
					break;
				}
			}
			if (touchComponent == null)
			{
				componentBuffer_.Add(touchComponent = new TouchComponent());
			}
			touchComponent.surface_ = this;
			touchComponent.id = id;
			touchComponent.x = x;
			touchComponent.y = y;
			touchComponent.width = width;
			touchComponent.height = height;
			touchComponent.value = value;
			touchComponent.option = option;
			touchComponent.isLock = true;
			AddTouchComponent(touchComponent);
			return touchComponent;
		}

		public TouchComponent AddTouchComponent(TouchComponent component)
		{
			if (!registTouch)
			{
				return null;
			}
			if (CanRegistComponent())
			{
				components_.Add(component);
			}
			return component;
		}

		public bool ExistTouchComponent(TouchComponent component)
		{
			foreach (TouchComponent item in components_)
			{
				if (item.GetKey() == component.GetKey())
				{
					return true;
				}
			}
			return false;
		}

		public void ClearTouchComponent()
		{
			components_.RemoveAllElements();
		}

		internal TouchPoint CreateTouchPoint(long key, float x, float y, TouchComponent component)
		{
			TouchPoint touchPoint = null;
			for (int i = 0; i < touchListBuffer_.Size(); i++)
			{
				TouchPoint touchPoint2 = touchListBuffer_.ElementAt(i);
				if (!startTouchList_.Contains(touchPoint2) && !lastTouchList_.Contains(touchPoint2))
				{
					touchPoint = touchPoint2;
					break;
				}
			}
			if (touchPoint == null)
			{
				touchListBuffer_.Add(touchPoint = new TouchPoint());
			}
			touchPoint.Init(key, x, y, component);
			return touchPoint;
		}

		internal virtual void DoBegin()
		{
			lock_ = true;
			double num = scaleRatio_ / 100f;
			touchEvents_.RemoveAllElements();
			for (int i = 0; i < touchEventBufferNum_.Length; i++)
			{
				touchEventBufferNum_[i] = 0;
			}
			for (int j = 0; j < 2; j++)
			{
				Vector<TouchPoint> vector = ((j == 0) ? startTouchList_ : lastTouchList_);
				for (int num2 = vector.Size() - 1; num2 >= 0; num2--)
				{
					bool flag = true;
					for (int k = 0; k < components_.Size(); k++)
					{
						if (CheckTouchKey(vector, components_.ElementAt(k).GetKey()))
						{
							flag = false;
							break;
						}
					}
					if (flag)
					{
						touchEvents_.Add(CreateTouchEvent(10, vector.ElementAt(num2).component));
						vector.RemoveElementAt(num2);
					}
				}
			}
			for (int l = 0; l < components_.Size() - 1; l++)
			{
				for (int num3 = components_.Size() - 1; num3 > l; num3--)
				{
					TouchComponent touchComponent = components_.ElementAt(num3 - 1);
					TouchComponent touchComponent2 = components_.ElementAt(num3);
					if (touchComponent.GetPriority() > touchComponent2.GetPriority())
					{
						components_.SetElementAt(touchComponent, num3);
						components_.SetElementAt(touchComponent2, num3 - 1);
					}
				}
			}
			if (!last_.IsEmpty())
			{
				lastTouchX = (int)((double)(last_.x - (float)x_) / num);
				lastTouchY = (int)((double)(last_.y - (float)y_) / num);
			}
			if (taskBuffer_ == null)
			{
				taskBuffer_ = new Vector<TouchEvent>();
			}
			taskBuffer_.RemoveAllElements();
		}

		public virtual void DoEnd()
		{
			double num = scaleRatio_ / 100f;
			int num2 = touchEvents_.Size();
			if (!last_.IsEmpty())
			{
				int num3 = (int)((double)(last_.x - (float)x_) / num);
				int num4 = (int)((double)(last_.y - (float)y_) / num);
				for (int num5 = lastTouchList_.Size() - 1; num5 >= 0; num5--)
				{
					TouchPoint touchPoint = lastTouchList_.ElementAt(num5);
					int num6 = taskBuffer_.Size();
					for (int i = 0; i < taskBuffer_.Size(); i++)
					{
						TouchEvent touchEvent = taskBuffer_.ElementAt(i);
						if (touchEvent.component.GetKey() == touchPoint.key)
						{
							num6 = ((!touchEvent.IsPositive()) ? (i + 1) : (-1));
						}
					}
					if (num6 != -1)
					{
						TouchComponent component = GetComponent(touchPoint.id, touchPoint.value);
						if (component != null)
						{
							taskBuffer_.InsertElementAt(CreateTouchEvent(3, component), num6);
							if (CheckTouchKey(startTouchList_, component.GetKey()))
							{
								taskBuffer_.InsertElementAt(CreateTouchEvent(6, component), num6 + 1);
							}
						}
					}
				}
				lastTouchList_.RemoveAllElements();
				for (int j = 0; j < components_.Size(); j++)
				{
					TouchComponent touchComponent = components_.ElementAt(j);
					bool flag = false;
					for (int num7 = taskBuffer_.Size() - 1; num7 >= 0; num7--)
					{
						TouchEvent touchEvent2 = taskBuffer_.ElementAt(num7);
						if (touchEvent2.component == touchComponent && touchEvent2.IsPositive())
						{
							flag = true;
							break;
						}
					}
					if (flag)
					{
						lastTouchList_.Add(CreateTouchPoint(touchComponent.GetKey(), num3, num4, touchComponent));
					}
				}
				for (int num8 = startTouchList_.Size() - 1; num8 >= 0; num8--)
				{
					TouchPoint touchPoint2 = startTouchList_.ElementAt(num8);
					bool flag2 = true;
					for (int k = 0; k < lastTouchList_.Size(); k++)
					{
						TouchPoint touchPoint3 = lastTouchList_.ElementAt(k);
						if (touchPoint2.key == touchPoint3.key)
						{
							flag2 = false;
							break;
						}
					}
					if (flag2)
					{
						startTouchList_.RemoveElementAt(num8);
					}
				}
				pinchStart_ = false;
				for (int l = 0; l < taskBuffer_.Size(); l++)
				{
					touchEvents_.Add(taskBuffer_.ElementAt(l));
				}
				touch_ = last_.action == 0 || last_.action == 2;
				if (!touch_)
				{
					lastTouchList_.RemoveAllElements();
					startTouchList_.RemoveAllElements();
				}
			}
			for (int m = 0; m < lastTouchList_.Size(); m++)
			{
				TouchPoint touchPoint4 = lastTouchList_.ElementAt(m);
				int num9 = -1;
				for (int n = num2; n < touchEvents_.Size(); n++)
				{
					if (touchEvents_.ElementAt(n).IsPositive())
					{
						num9 = n + 1;
						break;
					}
				}
				if (num9 == -1)
				{
					num9 = touchEvents_.Size();
				}
				touchEvents_.InsertElementAt(CreateTouchEvent(13, touchPoint4.component), num9);
			}
			MotionEventRecord motionEventRecord = ((!last_.IsEmpty()) ? last_ : last2_);
			if (!motionEventRecord.IsEmpty())
			{
				int x = (int)((double)(motionEventRecord.x - (float)x_) / num);
				int y = (int)((double)(motionEventRecord.y - (float)y_) / num);
				float speedX = 0f;
				float speedY = 0f;
				if (motionEventRecord.deltaTime > 0f)
				{
					speedX = motionEventRecord.deltaX / motionEventRecord.deltaTime;
					speedY = motionEventRecord.deltaY / motionEventRecord.deltaTime;
				}
				for (int num10 = 0; num10 < touchEvents_.Size(); num10++)
				{
					TouchEvent touchEvent3 = touchEvents_.ElementAt(num10);
					if (touchEvent3.type != 10)
					{
						touchEvent3.x = x;
						touchEvent3.y = y;
						touchEvent3.speedX = speedX;
						touchEvent3.speedY = speedY;
					}
				}
			}
			while (touchEvents_.Size() > 0)
			{
				TouchEvent evt = touchEvents_.FirstElement();
				touchEvents_.RemoveElementAt(0);
				OnTouchEvent(evt);
			}
			if (!last_.IsEmpty())
			{
				last2_.Copy(this, last_);
			}
			start_.Clear();
			last_.Clear();
			lock_ = false;
			for (int num11 = 0; num11 < delayNum_; num11++)
			{
				OnReceiveTouchEvent(delay_[num11]);
			}
			delayNum_ = 0;
			if (delayEraseTouchDownComponentId_)
			{
				touchDownComponentId_ = -1;
				delayEraseTouchDownComponentId_ = false;
			}
			if (touchDownComponentId_ != -1 && Input.touchCount == 0 && !Input.GetMouseButton(0))
			{
				delayEraseTouchDownComponentId_ = true;
			}
		}

		internal virtual bool DoEvent(TouchComponent c)
		{
			if (last_.IsEmpty())
			{
				return false;
			}
			double num = scaleRatio_ / 100f;
			int num2 = (int)((double)(last_.x - (float)x_) / num);
			int num3 = (int)((double)(last_.y - (float)y_) / num);
			int num4 = (int)((double)(start_.x - (float)x_) / num);
			int num5 = (int)((double)(start_.y - (float)y_) / num);
			int action = last_.action;
			TouchOption option = c.option;
			if (!CheckHitPointRect(num2, num3, c.x, c.y, c.width, c.height))
			{
				return false;
			}
			bool flag = CheckHitPointRect(num4, num5, c.x, c.y, c.width, c.height);
			bool flag2 = CheckTouchKey(lastTouchList_, c.GetKey());
			int num6 = SearchTouchKey(startTouchList_, c.GetKey());
			SearchTouchKey(lastTouchList_, c.GetKey());
			int num7 = taskBuffer_.Size();
			TouchEvent touchEvent = null;
			if (c.Check(1024) && action != 0 && touchDownComponentId_ != -1 && touchDownComponentId_ != c.id)
			{
				return false;
			}
			switch (action)
			{
			case 0:
				if (!flag2)
				{
					taskBuffer_.Add(CreateTouchEvent(2, c));
					if (num6 == -1)
					{
						touchDownComponentId_ = c.id;
						taskBuffer_.Add(CreateTouchEvent(0, c));
						startTouchList_.Add(CreateTouchPoint(c.GetKey(), num2, num3, c));
						num6 = startTouchList_.Size() - 1;
					}
				}
				break;
			case 1:
				if (flag2)
				{
					if (num6 != -1)
					{
						TouchPoint touchPoint = startTouchList_.ElementAt(num6);
						float num8 = touchPoint.x - (float)num2;
						float num9 = touchPoint.y - (float)num3;
						if (!(num8 * num8 + num9 * num9 < 225f))
						{
							touchEvent = CreateTouchEvent(1, c);
							goto case 2;
						}
						taskBuffer_.Add(CreateTouchEvent(5, c));
					}
					taskBuffer_.Add(CreateTouchEvent(1, c));
				}
				else if (flag && start_.action == 0)
				{
					touchDownComponentId_ = c.id;
					taskBuffer_.Add(CreateTouchEvent(0, c));
					taskBuffer_.Add(CreateTouchEvent(5, c));
					taskBuffer_.Add(CreateTouchEvent(1, c));
					taskBuffer_.Add(CreateTouchEvent(3, c));
					taskBuffer_.Add(CreateTouchEvent(6, c));
				}
				else
				{
					taskBuffer_.Add(CreateTouchEvent(2, c));
					taskBuffer_.Add(CreateTouchEvent(1, c));
					taskBuffer_.Add(CreateTouchEvent(3, c));
				}
				break;
			case 2:
				if (flag2)
				{
					if (num6 != -1)
					{
						TouchPoint touchPoint2 = startTouchList_.ElementAt(num6);
						int num10 = 0;
						int num11 = 0;
						if (option != null && (option.dragGridW > 0f || option.dragGridH > 0f))
						{
							float num12 = (float)num2 - touchPoint2.dragBaseX;
							float num13 = (float)num3 - touchPoint2.dragBaseY;
							if (option.dragGridW > 0f)
							{
								num10 = (int)(num12 / option.dragGridW);
							}
							if (option.dragGridH > 0f)
							{
								num11 = (int)(num13 / option.dragGridH);
							}
						}
						if (num10 != 0 || num11 != 0)
						{
							TouchEvent touchEvent2 = CreateTouchEvent(8, c);
							touchEvent2.dx = num10;
							touchEvent2.dy = num11;
							taskBuffer_.Add(touchEvent2);
							touchPoint2.dragBaseX += (float)num10 * option.dragGridW;
							touchPoint2.dragBaseY += (float)num11 * option.dragGridH;
						}
						else if (option != null && option.dragCell != null)
						{
							TouchEvent touchEvent3 = CreateTouchEvent(9, c);
							if (option.dragCell[4] > 1)
							{
								touchEvent3.dx = (int)(((float)num2 - (float)option.dragCell[0]) / ((float)option.dragCell[2] / (float)option.dragCell[4]));
							}
							if (option.dragCell[5] > 1)
							{
								touchEvent3.dy = (int)(((float)num3 - (float)option.dragCell[1]) / ((float)option.dragCell[3] / (float)option.dragCell[5]));
							}
							if (7 < option.dragCell.Length)
							{
								int num14 = option.dragCell[6];
								int num15 = option.dragCell[7];
								if (num14 < touchEvent3.dx && touchEvent3.dx < num14 + num15)
								{
									touchEvent3.dx = num14;
								}
								else if (num14 + num15 <= touchEvent3.dx)
								{
									touchEvent3.dx -= num15 - 1;
								}
								if (num14 < touchEvent3.dy && touchEvent3.dy < num14 + num15)
								{
									touchEvent3.dy = num14;
								}
								else if (num14 + num15 <= touchEvent3.dy)
								{
									touchEvent3.dy -= num15 - 1;
								}
							}
							if (touchEvent3.dx >= option.dragCell[4])
							{
								touchEvent3.dx = option.dragCell[4] - 1;
							}
							if (touchEvent3.dx < 0)
							{
								touchEvent3.dx = 0;
							}
							if (touchEvent3.dy >= option.dragCell[5])
							{
								touchEvent3.dy = option.dragCell[5] - 1;
							}
							if (touchEvent3.dy < 0)
							{
								touchEvent3.dy = 0;
							}
							taskBuffer_.Add(touchEvent3);
						}
						else
						{
							TouchEvent touchEvent4 = CreateTouchEvent(7, c);
							touchEvent4.dx = (int)((float)num2 - touchPoint2.x);
							touchEvent4.dy = (int)((float)num3 - touchPoint2.y);
							taskBuffer_.Add(touchEvent4);
						}
					}
					else
					{
						taskBuffer_.Add(CreateTouchEvent(4, c));
					}
					break;
				}
				taskBuffer_.Add(CreateTouchEvent(2, c));
				if (flag && start_.action == 0)
				{
					touchDownComponentId_ = c.id;
					taskBuffer_.Add(CreateTouchEvent(0, c));
					TouchEvent touchEvent5 = CreateTouchEvent(7, c);
					touchEvent5.dx = num2 - num4;
					touchEvent5.dy = num3 - num5;
					taskBuffer_.Add(touchEvent5);
					if (num6 != -1)
					{
						startTouchList_.RemoveElementAt(num6);
					}
					startTouchList_.Add(CreateTouchPoint(c.GetKey(), num2, num3, c));
					num6 = startTouchList_.Size() - 1;
				}
				break;
			}
			if (touchEvent != null)
			{
				taskBuffer_.Add(touchEvent);
			}
			if (multiTouchEnable_ && (double)pinchDistance_ / num != 0.0)
			{
				int[][] array = pinchStartPoint_;
				if (CheckHitPointRect((int)((double)(array[0][0] - x_) / num), (int)((double)(array[0][1] - y_) / num), c.x, c.y, c.width, c.height) && CheckHitPointRect((int)((double)(array[1][0] - x_) / num), (int)((double)(array[1][1] - y_) / num), c.x, c.y, c.width, c.height))
				{
					int i;
					for (i = num7; i < taskBuffer_.Size() && taskBuffer_.ElementAt(i).IsPositive(); i++)
					{
					}
					if (pinchStart_)
					{
						TouchEvent touchEvent6 = CreateTouchEvent(11, c);
						touchEvent6.pinch = 0;
						JSystem.Arraycopy(pinchStartPoint_[0], 0, touchEvent6.pinchPointBegin_[0], 0, 2);
						JSystem.Arraycopy(pinchStartPoint_[1], 0, touchEvent6.pinchPointBegin_[1], 0, 2);
						JSystem.Arraycopy(pinchPointer_[0], 0, touchEvent6.pinchPointBegin_[0], 0, 2);
						JSystem.Arraycopy(pinchPointer_[1], 0, touchEvent6.pinchPointBegin_[1], 0, 2);
						taskBuffer_.InsertElementAt(touchEvent6, i++);
					}
					TouchEvent touchEvent7 = CreateTouchEvent(12, c);
					touchEvent7.pinch = (int)((double)pinchDistance_ / num);
					JSystem.Arraycopy(pinchStartPoint_[0], 0, touchEvent7.pinchPointBegin_[0], 0, 2);
					JSystem.Arraycopy(pinchStartPoint_[1], 0, touchEvent7.pinchPointBegin_[1], 0, 2);
					JSystem.Arraycopy(pinchPointer_[0], 0, touchEvent7.pinchPointNow_[0], 0, 2);
					JSystem.Arraycopy(pinchPointer_[1], 0, touchEvent7.pinchPointNow_[1], 0, 2);
					taskBuffer_.InsertElementAt(touchEvent7, i++);
					pinchDistance_ = 0;
				}
			}
			if (c.Check(1))
			{
				return false;
			}
			return true;
		}

		protected abstract void OnTouchEvent(TouchEvent evt);

		public virtual void BeginTouchRegist(kairo.unity.ui.Graphics g)
		{
			for (int i = 0; i < componentBuffer_.Size(); i++)
			{
				TouchComponent touchComponent = componentBuffer_.ElementAt(i);
				if (touchComponent.hasCount <= 0)
				{
					touchComponent.isLock = false;
				}
			}
			for (int j = 0; j < components_.Size(); j++)
			{
				TouchComponent touchComponent2 = components_.ElementAt(j);
				touchComponent2.isLock = true;
				if (touchComponent2.option != null)
				{
					touchComponent2.option.Lock = true;
				}
			}
			for (int k = 0; k < startTouchList_.Size(); k++)
			{
				TouchComponent component = startTouchList_.ElementAt(k).component;
				if (component != null)
				{
					component.isLock = true;
					if (component.option != null)
					{
						component.option.Lock = true;
					}
				}
			}
			for (int l = 0; l < lastTouchList_.Size(); l++)
			{
				TouchComponent component2 = lastTouchList_.ElementAt(l).component;
				if (component2 != null)
				{
					component2.isLock = true;
					if (component2.option != null)
					{
						component2.option.Lock = true;
					}
				}
			}
			registTouch = true;
		}

		public virtual void EndTouchRegist(kairo.unity.ui.Graphics g)
		{
			registTouch = false;
		}

		public virtual void SetTouchRegistEnable(bool enable)
		{
			registTouch = enable;
		}

		public abstract void Draw(kairo.unity.ui.Graphics g);

		internal void OnDraw(kairo.unity.ui.Graphics g)
		{
			graphics_ = g;
			windowsScaleBackup_.Clear();
			windowsScaleGraphics_.Clear();
			try
			{
				Draw(g);
			}
			finally
			{
				graphics_ = null;
				windowsScaleBackup_.Clear();
				windowsScaleGraphics_.Clear();
			}
		}

		public bool IsTouch()
		{
			return touch_;
		}

		public bool IsMultiTouchEnable()
		{
			return multiTouchEnable_;
		}

		public abstract bool IsGameSurface();

		public abstract void RotateClear();

		public int GetComponentLength()
		{
			kairo.unity.ui.Graphics.GetCurrent();
			return components_.Size();
		}

		public TouchComponent GetComponent(int index)
		{
			kairo.unity.ui.Graphics.GetCurrent();
			return components_.ElementAt(index);
		}

		public TouchComponent GetComponent(int id, int value)
		{
			int componentLength = GetComponentLength();
			for (int i = 0; i < componentLength; i++)
			{
				TouchComponent component = GetComponent(i);
				if (component.id == id && component.value == value)
				{
					return component;
				}
			}
			return null;
		}

		public TouchComponent GetComponentId(int id)
		{
			int componentLength = GetComponentLength();
			for (int i = 0; i < componentLength; i++)
			{
				TouchComponent component = GetComponent(i);
				if (component.id == id)
				{
					return component;
				}
			}
			return null;
		}

		public bool CheckTouch(int id, int value)
		{
			return CheckTouchKey(lastTouchList_, TouchComponent.GetKey(id, value));
		}

		public bool CheckTouch(int id)
		{
			return CheckTouchId(lastTouchList_, id);
		}

		public bool CheckFirstTouch(int id, int value)
		{
			return CheckTouchKey(startTouchList_, TouchComponent.GetKey(id, value));
		}

		public bool CheckFirstTouch(int id)
		{
			return CheckTouchId(startTouchList_, id);
		}

		private int SearchTouchKey(Vector<TouchPoint> list, long key)
		{
			for (int i = 0; i < list.Size(); i++)
			{
				if (list.ElementAt(i).key == key)
				{
					return i;
				}
			}
			return -1;
		}

		private int SearchTouchId(Vector<TouchPoint> list, int id)
		{
			for (int i = 0; i < list.Size(); i++)
			{
				if (list.ElementAt(i).id == id)
				{
					return i;
				}
			}
			return -1;
		}

		private bool CheckTouchKey(Vector<TouchPoint> list, long key)
		{
			return SearchTouchKey(list, key) != -1;
		}

		private bool CheckTouchId(Vector<TouchPoint> list, int id)
		{
			return SearchTouchId(list, id) != -1;
		}

		protected Vector<TouchEvent> GetTouchEventsId(int id)
		{
			return GetTouchEventsId(id, -1);
		}

		protected Vector<TouchEvent> GetTouchEventsId(int id, int type)
		{
			Vector<TouchEvent> vector = new Vector<TouchEvent>();
			for (int i = 0; i < touchEvents_.Size(); i++)
			{
				TouchEvent touchEvent = touchEvents_.ElementAt(i);
				if (touchEvent.component.id == id && (type == -1 || touchEvent.type == type))
				{
					vector.Add(touchEvent);
				}
			}
			return vector;
		}

		protected Vector<TouchEvent> GetTouchEventsKey(long key)
		{
			return GetTouchEventsKey(key, -1);
		}

		protected Vector<TouchEvent> GetTouchEventsKey(long key, int type)
		{
			Vector<TouchEvent> vector = new Vector<TouchEvent>();
			for (int i = 0; i < touchEvents_.Size(); i++)
			{
				TouchEvent touchEvent = touchEvents_.ElementAt(i);
				if (touchEvent.component.GetKey() == key && (type == -1 || touchEvent.type == type))
				{
					vector.Add(touchEvent);
				}
			}
			return vector;
		}

		protected TouchEvent GetTouchEventId(int id)
		{
			return GetTouchEventId(id, -1);
		}

		protected TouchEvent GetTouchEventId(int id, int type)
		{
			for (int i = 0; i < touchEvents_.Size(); i++)
			{
				TouchEvent touchEvent = touchEvents_.ElementAt(i);
				if (touchEvent.component.id == id && (type == -1 || touchEvent.type == type))
				{
					return touchEvent;
				}
			}
			return null;
		}

		protected TouchEvent GetTouchEventKey(long key)
		{
			return GetTouchEventKey(key, -1);
		}

		protected TouchEvent GetTouchEventKey(long key, int type)
		{
			for (int i = 0; i < touchEvents_.Size(); i++)
			{
				TouchEvent touchEvent = touchEvents_.ElementAt(i);
				if (touchEvent.component.GetKey() == key && (type == -1 || touchEvent.type == type))
				{
					return touchEvent;
				}
			}
			return null;
		}

		protected TouchEvent GetTouchEventFlag(int flag)
		{
			return GetTouchEventKey(flag, -1);
		}

		protected TouchEvent GetTouchEventFlag(int flag, int type)
		{
			for (int i = 0; i < touchEvents_.Size(); i++)
			{
				TouchEvent touchEvent = touchEvents_.ElementAt(i);
				if (touchEvent.component.Check(flag) && (type == -1 || touchEvent.type == type))
				{
					return touchEvent;
				}
			}
			return null;
		}

		protected Vector<TouchEvent> GetTouchEventsFlag(int flag)
		{
			return GetTouchEventsFlag(flag, -1);
		}

		protected Vector<TouchEvent> GetTouchEventsFlag(int flag, int type)
		{
			Vector<TouchEvent> vector = new Vector<TouchEvent>();
			for (int i = 0; i < touchEvents_.Size(); i++)
			{
				TouchEvent touchEvent = touchEvents_.ElementAt(i);
				if (touchEvent.component.Check(flag) && (type == -1 || touchEvent.type == type))
				{
					vector.Add(touchEvent);
				}
			}
			return vector;
		}

		protected TouchEvent CreateTouchEvent(int type)
		{
			return CreateTouchEvent(type, null);
		}

		protected TouchEvent CreateTouchEvent(int type, TouchComponent component)
		{
			int num = 0;
			if (type == 12 || type == 11)
			{
				num = 1;
			}
			if (touchEventBufferNum_[num] >= touchEventBuffer_[num].Size())
			{
				touchEventBuffer_[num].Add(new TouchEvent());
			}
			TouchEvent touchEvent = touchEventBuffer_[num].ElementAt(touchEventBufferNum_[num]);
			touchEventBufferNum_[num]++;
			if (num == 1 && touchEvent.pinchPointBegin_ == null)
			{
				touchEvent.pinchPointBegin_ = JTool.MakeArray<int[][]>(new int[2] { 2, 2 });
				touchEvent.pinchPointNow_ = JTool.MakeArray<int[][]>(new int[2] { 2, 2 });
			}
			touchEvent.Init();
			touchEvent.type = type;
			touchEvent.component = component;
			return touchEvent;
		}

		public void SetScaleRatio(float scale)
		{
			scaleRatio_ = scale;
		}

		public float GetScaleRatio()
		{
			return scaleRatio_;
		}

		public void SetX(int x)
		{
			x_ = x;
		}

		public int GetX()
		{
			return x_;
		}

		public void SetY(int y)
		{
			y_ = y;
		}

		public int GetY()
		{
			return y_;
		}

		public void SetWidth(int width)
		{
			width_ = width;
		}

		public int GetWidth()
		{
			return width_;
		}

		public void SetHeight(int height)
		{
			height_ = height;
		}

		public int GetHeight()
		{
			return height_;
		}

		public void SetLocation(int x, int y)
		{
			x_ = x;
			y_ = y;
		}

		public void SetSize(int width, int height)
		{
			width_ = width;
			height_ = height;
		}

		public int GetGameWidth()
		{
			int num = (int)((float)(GetWidth() * 100) / GetScaleRatio() + 0.01f);
			if (graphics_ != null && graphics_.GetScale() != GetScaleRatio())
			{
				num = (int)((float)(num * 100) / (graphics_.GetScale() * 100f / GetScaleRatio()));
			}
			return num;
		}

		public int GetGameHeight()
		{
			int num = (int)((float)(GetHeight() * 100) / GetScaleRatio() + 0.01f);
			if (graphics_ != null && graphics_.GetScale() != GetScaleRatio())
			{
				num = (int)((float)(num * 100) / (graphics_.GetScale() * 100f / GetScaleRatio()));
			}
			return num;
		}

		public void BeginWindow(kairo.unity.ui.Graphics g)
		{
			int[] viewport = IApplication.GetCurrentApp().GetViewport();
			int gameWidth = this.GetGameWidth();
			int gameHeight = this.GetGameHeight();
			int num = gameWidth * 100 / viewport[0];
			int num1 = gameHeight * 100 / viewport[1];
			if (num > 100)
			{
				num = 100;
			}
			if (num1 > 100)
			{
				num1 = 100;
			}
			int num2 = (num > num1 ? num1 : num);
			if (num2 == 100)
			{
				return;
			}
			this.windowsScaleGraphics_.Add(g);
			this.windowsScaleBackup_.Add(g.GetScale());
			g.Scale(this.GetScaleRatio() * (float)num2 / 100f);
		}

		public void EndWindow()
		{
			if (windowsScaleBackup_.Count > 0 && windowsScaleGraphics_.Count > 0)
			{
				float scaleRatio = windowsScaleBackup_[windowsScaleBackup_.Count - 1];
				windowsScaleGraphics_.Last().Scale(scaleRatio);
				windowsScaleGraphics_.RemoveAt(windowsScaleGraphics_.Count - 1);
				windowsScaleBackup_.RemoveAt(windowsScaleBackup_.Count - 1);
			}
		}

		protected bool CheckHitPointRect(int x, int y, int rcx, int rcy, int rcw, int rch)
		{
			if (rcx <= x && x < rcx + rcw && rcy <= y)
			{
				return y < rcy + rch;
			}
			return false;
		}

		public virtual void OnReceiveTouchEvent(MotionEvent evt)
		{
			lock (OnReceiveTouchEvent_lock1)
			{
				onReceiveTouchEvent_tmp.Copy(this, evt);
				OnReceiveTouchEvent(onReceiveTouchEvent_tmp);
			}
		}

		public void OnReceiveTouchEvent(MotionEventRecord evt)
		{
			lock (OnReceiveTouchEvent_lock2)
			{
				if (lock_)
				{
					if (delayNum_ >= delay_.Length)
					{
						MotionEventRecord[] array = new MotionEventRecord[delay_.Length + 1];
						JSystem.Arraycopy(delay_, 0, array, 0, delay_.Length);
						array[array.Length - 1] = new MotionEventRecord();
						delay_ = array;
					}
					delay_[delayNum_].Copy(this, evt);
					delayNum_++;
					return;
				}
				{
					if (start_.IsEmpty())
					{
						start_.Copy(this, evt);
					}
					last_.Copy(this, evt);
					if (multiTouchEnable_)
					{
						int action = evt.GetAction();
						if (action == 3 || action == 4)
						{
							pinchPointer_[0][2] = (pinchPointer_[1][2] = -1);
							pinchDistance_ = 0;
							return;
						}
						{
							int num = -1;
							if (pinchPointer_[0][2] != -1 && pinchPointer_[1][2] != -1)
							{
								int num2 = pinchPointer_[1][0] - pinchPointer_[0][0];
								int num3 = pinchPointer_[1][1] - pinchPointer_[0][1];
								num = (int)JMath.Sqrt(num2 * num2 + num3 * num3);
							}
							pinchPointer_[0][2] = (pinchPointer_[1][2] = -1);
							int pointerCount = evt.GetPointerCount();
							for (int i = 0; i < pointerCount; i++)
							{
								int pointerId = evt.GetPointerId(i);
								if (pointerId < 2 && (pointerId != 0 || (action & 6) != 6) && (pointerId != 1 || (action & 0x106) != 262))
								{
									pinchPointer_[i][0] = (int)evt.GetX(i);
									pinchPointer_[i][1] = (int)evt.GetY(i) + (evt.IsMousePointer() ? 0 : GetTouchOffset());
									pinchPointer_[i][2] = pointerId;
								}
							}
							int num4 = -1;
							if (pinchPointer_[0][2] != -1 && pinchPointer_[1][2] != -1)
							{
								int num5 = pinchPointer_[1][0] - pinchPointer_[0][0];
								int num6 = pinchPointer_[1][1] - pinchPointer_[0][1];
								num4 = (int)JMath.Sqrt(num5 * num5 + num6 * num6);
							}
							if (num == -1 && num4 != -1)
							{
								pinchStartPoint_[0][0] = pinchPointer_[0][0];
								pinchStartPoint_[0][1] = pinchPointer_[0][1];
								pinchStartPoint_[1][0] = pinchPointer_[1][0];
								pinchStartPoint_[1][1] = pinchPointer_[1][1];
								pinchStart_ = true;
							}
							if (num == -1 || num4 == -1)
							{
								pinchDistance_ = 0;
							}
							else
							{
								pinchDistance_ += num4 - num;
							}
						}
					}
				}
			}
		}

		public void OnReceiveTrackballEvent(MotionEvent evt)
		{
			lock (OnReceiveTrackballEvent_lock)
			{
			}
		}

		public virtual void OnKeyDown(int key)
		{
		}

		public void OnKeyUp(int key)
		{
		}

		public virtual void OnKeyDown(int keyCode, KeyEvent evt)
		{
		}

		public virtual void OnKeyUp(int keyCode, KeyEvent evt)
		{
		}

		public int[] ScreenTouchToLocal(int x, int y)
		{
			double num = scaleRatio_ / 100f;
			screenTouchToLocal_result[0] = (int)((double)(x - x_) / num);
			screenTouchToLocal_result[1] = (int)((double)(y - y_ + GetTouchOffset()) / num);
			return screenTouchToLocal_result;
		}

		public int[] LocalTouchToScreen(int x, int y)
		{
			double num = scaleRatio_ / 100f;
			localTouchToScreen_result[0] = (int)((double)x * num + (double)x_);
			localTouchToScreen_result[1] = (int)((double)y * num + (double)y_ - (double)GetTouchOffset());
			return localTouchToScreen_result;
		}

		public int GetTouchOffset()
		{
			kairo.unity.ui.Canvas instance = kairo.unity.ui.Canvas.GetInstance();
			int num = (int)((float)touchOffset_ * Screen.dpi / 320f * IApplication.GetCurrentApp().GetResolutionScaleRatio() / 100f);
			if (!instance.IsFullscreen() && instance.GetStatusBarPosition() == 0)
			{
				num -= instance.GetStatusBarHeight();
			}
			return num;
		}

		public virtual bool CanRegistComponent()
		{
			return true;
		}
	}
}
