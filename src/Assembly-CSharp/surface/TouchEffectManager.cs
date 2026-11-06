using System;
using kairo.common.cfg;
using kairo.unity.surface;
using kairo.unity.ui;
using kairo.unity.util;

namespace surface
{
	public class TouchEffectManager
	{
		internal static TouchEffectManager instance_;

		public FastVector<TouchEffect> effects = new FastVector<TouchEffect>();

		private FastVector<TouchEffect> effectsBuffer_ = new FastVector<TouchEffect>();

		private TouchEffectManager()
		{
		}

		public static TouchEffectManager GetInstance()
		{
			if (instance_ != null)
			{
				return instance_;
			}
			instance_ = new TouchEffectManager();
			return instance_;
		}

		public void Add(TouchEffect effect)
		{
			effects.Add(effect);
		}

		public void Frame()
		{
			for (int num = effects.Size() - 1; num >= 0; num--)
			{
				{ bool recoveredCondition_793 = effects.ElementAt(num).Frame();
if (recoveredCondition_793)
				{
					effects.RemoveElementAt(num);
				}
}
			}
		}

		public void Draw(Graphics g)
		{
			for (int i = 0; i < effects.Size(); i++)
			{
				try
				{
					effects.ElementAt(i).Draw(g);
				}
				catch (Exception t)
				{
					if (Config.PRINT)
					{
						Log.Info(t);
					}
				}
			}
		}

		public void Clear()
		{
			for (int num = effects.Size() - 1; num >= 0; num--)
			{
				RemoveEffect(num);
			}
			effects.RemoveAllElementsKeep();
		}

		public void RemoveEffect(TouchEffect effect)
		{
			effects.RemoveElement(effect);
		}

		public void RemoveEffect(int index)
		{
			effects.RemoveElementAt(index);
		}

		public void RemoveSurface(SurfaceBase surface)
		{
			for (int num = effects.Size() - 1; num >= 0; num--)
			{
				if (effects.ElementAt(num).surface_ == surface)
				{
					RemoveEffect(num);
				}
			}
		}

		public void RemoveId(int id)
		{
			for (int num = effects.Size() - 1; num >= 0; num--)
			{
				if (effects.ElementAt(num).id_ == id)
				{
					RemoveEffect(num);
				}
			}
		}

		public void RemoveComponentId(SurfaceBase surface, int componentId)
		{
			for (int num = effects.Size() - 1; num >= 0; num--)
			{
				TouchEffect touchEffect = effects.ElementAt(num);
				if (touchEffect.surface_ == surface && TouchComponent.GetId(touchEffect.key_) == componentId)
				{
					RemoveEffect(num);
				}
			}
		}

		public void RemoveComponentKey(SurfaceBase surface, long componentKey)
		{
			for (int num = effects.Size() - 1; num >= 0; num--)
			{
				TouchEffect touchEffect = effects.ElementAt(num);
				if (touchEffect.surface_ == surface && touchEffect.key_ == componentKey)
				{
					RemoveEffect(num);
				}
			}
		}

		public TouchEffect AddEffect(SurfaceBase surface, int id, float x, float y, TouchComponent component)
		{
			return AddEffect(surface, id, x, y, component, null);
		}

		public TouchEffect AddEffect(SurfaceBase surface, int id, float x, float y, TouchComponent component, object arg)
		{
			TouchEffect touchEffect = null;
			for (int i = 0; i < effectsBuffer_.Size(); i++)
			{
				TouchEffect touchEffect2 = effectsBuffer_.ElementAt(i);
				if (effects.Contains(touchEffect2)) { continue; }
touchEffect = touchEffect2;
break;
			}
			if (touchEffect == null)
			{
				effectsBuffer_.Add(touchEffect = new TouchEffect());
			}
			touchEffect.Init(surface, id, x, y, component, arg);
			Add(touchEffect);
			return touchEffect;
		}
	}
}
