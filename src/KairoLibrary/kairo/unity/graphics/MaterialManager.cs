using System.Collections.Generic;
using kairo.unity.ui;
using UnityEngine;

namespace kairo.unity.graphics
{
	public class MaterialManager
	{
		private static MaterialManager instance_;

		private static Image empty_ = new Image();

		private Dictionary<Image, Dictionary<string, Material>> materlas_ = new Dictionary<Image, Dictionary<string, Material>>();

		private static ShaderBuilder GetMaterial_sb = new ShaderBuilder();

		protected MaterialManager()
		{
		}

		public static MaterialManager GetInstance()
		{
			if (instance_ == null)
			{
				instance_ = new MaterialManager();
			}
			return instance_;
		}

		public Material GetMaterial(kairo.unity.ui.Graphics g, Image image, Color[] colors, bool clipRegion = false)
		{
			float alpha = 1f;
			for (int i = 0; i < colors.Length; i++)
			{
				if (colors[i].a < 0.01f)
				{
					alpha = colors[i].a;
				}
				else if (colors[i].a < 0.99f)
				{
					alpha = colors[i].a;
					break;
				}
			}
			return GetMaterial(g, image, colors, alpha, clipRegion, -1, false, false, false);
		}

		public Material GetMaterial(kairo.unity.ui.Graphics g, bool clearDepth, bool clearColor, bool clearStencil)
		{
			return GetMaterial(g, null, 1f, false, -1, clearDepth, clearColor, clearStencil);
		}

		public Material GetMaterial(kairo.unity.ui.Graphics g, Image image = null, float alpha = 1f, bool clipRegion = false, int opaTransOrder = -1, bool clearDepth = false, bool clearColor = false, bool clearStencil = false)
		{
			return GetMaterial(g, image, null, alpha, clipRegion, opaTransOrder, clearDepth, clearColor, clearStencil);
		}

		public Material GetMaterial(kairo.unity.ui.Graphics g, Image image, Color[] colors, float alpha, bool clipRegion, int opaTransOrder, bool clearDepth, bool clearColor, bool clearStencil)
		{
			Image key = ((image != null) ? image : empty_);
			if (g.setFigureClip_)
			{
				clipRegion = true;
			}
			ShaderBuilder getMaterial_sb = GetMaterial_sb;
			getMaterial_sb.Init(g, image, colors, alpha, clipRegion, opaTransOrder, clearDepth, clearColor, clearStencil);
			string name = getMaterial_sb.GetName();
			Dictionary<string, Material> value;
			if (!materlas_.TryGetValue(key, out value))
			{
				materlas_[key] = (value = new Dictionary<string, Material>());
			}
			string text = name;
			if (!clipRegion && g.blendMode_ == 1)
			{
				Color blendColor_ = g.blendColor_;
				text = text + ":" + blendColor_.ToString();
			}
			int renderQueue = g.GetRenderQueue();
			if (renderQueue != 0)
			{
				text = text + ",q" + ((renderQueue > 0) ? "+" : "") + renderQueue;
			}
			Material value2 = null;
			if (!value.TryGetValue(text, out value2))
			{
				ShaderManager instance = ShaderManager.GetInstance();
				Shader shader = instance.Find(name);
				if (shader == null)
				{
					shader = getMaterial_sb.ToShader();
					instance.Regist(shader);
				}
				value[text] = (value2 = new Material(shader, image));
				value2.key_ = text;
				value2.UntiColor = getMaterial_sb.untiColor;
				value2.RenderQueueOffset = renderQueue;
				if (!clipRegion && g.blendMode_ == 1)
				{
					UnityEngine.Material material2 = value2.GetMaterial();
					Color blendColor_2 = g.blendColor_;
					material2.SetFloat("_BlendAlpha", blendColor_2.a);
					material2.SetColor("_BlendColor", new Color(blendColor_2.r, blendColor_2.g, blendColor_2.b, 1f));
				}
			}
			value2.NotAccessFrame = 0;
			return value2;
		}

		public Material GetMaterial(Shader shader, Image image, params object[] args)
		{
			Image key = ((image != null) ? image : empty_);
			string name = shader.name;
			Dictionary<string, Material> value;
			if (!materlas_.TryGetValue(key, out value))
			{
				materlas_[key] = (value = new Dictionary<string, Material>());
			}
			string text = name;
			for (int i = 0; i < args.Length; i++)
			{
				text = text + ":" + args[i];
			}
			Material value2 = null;
			if (value.TryGetValue(text, out value2))
			{
				value2.NotAccessFrame = 0;
				return value2;
			}
			ShaderManager instance = ShaderManager.GetInstance();
			if (!instance.Find(shader.name))
			{
				instance.Regist(shader);
			}
			value[text] = (value2 = new Material(shader, image));
			value2.key_ = text;
			UnityEngine.Material material2 = value2.GetMaterial();
			for (int j = 0; j < args.Length; j += 2)
			{
				string propertyName = (string)args[j + 0];
				object obj = args[j + 1];
				if (obj is int)
				{
					material2.SetInt(propertyName, (int)obj);
				}
				else if (obj is float)
				{
					material2.SetFloat(propertyName, (float)obj);
				}
				else if (obj is Color)
				{
					material2.SetColor(propertyName, (Color)obj);
				}
				else if (obj is Vector4)
				{
					material2.SetVector(propertyName, (Vector4)obj);
				}
				else if (obj is Matrix4x4)
				{
					material2.SetMatrix(propertyName, (Matrix4x4)obj);
				}
				else if (obj is Texture)
				{
					material2.SetTexture(propertyName, (Texture)obj);
				}
				else if (obj is Image)
				{
					material2.SetTexture(propertyName, ((Image)obj).GetImage());
				}
				else if (obj is ComputeBuffer)
				{
					material2.SetBuffer(propertyName, (ComputeBuffer)obj);
				}
			}
			return value2;
		}

		public Material GetMaterial(Material material, Image image)
		{
			Image key = ((image != null) ? image : empty_);
			Dictionary<string, Material> value;
			if (!materlas_.TryGetValue(key, out value))
			{
				value = (materlas_[key] = new Dictionary<string, Material>());
			}
			if (value.ContainsKey(material.key_))
			{
				material.NotAccessFrame = 0;
				return material;
			}
			Material material2 = (value[material.key_] = new Material(material.GetMaterial(), image));
			material2.RenderQueueOffset = material.RenderQueueOffset;
			return material2;
		}

		public void RemoveInvalidMaterials()
		{
			IApplication.GetCurrentApp().GetTotalFrames();
			List<Image> list = null;
			foreach (Image key in materlas_.Keys)
			{
				if (key != null && key != empty_ && key.IsDisposed)
				{
					foreach (KeyValuePair<string, Material> item in materlas_[key])
					{
						item.Value.Dispose();
					}
					if (list == null)
					{
						list = new List<Image>();
					}
					list.Add(key);
					continue;
				}
				Dictionary<string, Material> dictionary = materlas_[key];
				string text = null;
				foreach (KeyValuePair<string, Material> item2 in dictionary)
				{
					item2.Value.NotAccessFrame++;
					if (item2.Value.NotAccessFrame > 50)
					{
						bool flag = true;
						if (item2.Value.UntiColor && item2.Value.NotAccessFrame < 500)
						{
							flag = false;
						}
						if (flag)
						{
							item2.Value.Dispose();
							text = item2.Key;
							break;
						}
					}
				}
				if (text != null)
				{
					dictionary.Remove(text);
				}
			}
			if (list == null)
			{
				return;
			}
			foreach (Image item3 in list)
			{
				materlas_.Remove(item3);
			}
		}

		public void Clear()
		{
			foreach (Image key in materlas_.Keys)
			{
				foreach (KeyValuePair<string, Material> item in materlas_[key])
				{
					item.Value.Dispose();
				}
			}
			materlas_.Clear();
		}
	}
}
