using System;
using kairo.unity.ui;
using UnityEngine;

namespace kairo.unity.graphics
{
	public class Material : IDisposable
	{
		private Shader shader_;

		private Image image_;

		private UnityEngine.Material material_;

		private int hashCode_;

		internal string key_;

		internal int NotAccessFrame;

		internal bool UntiColor;

		private int renderQueueOffset_;

		private int renderQueue_;

		internal int RenderQueueOffset
		{
			set
			{
				renderQueueOffset_ = value;
				renderQueue_ = shader_.renderQueue + renderQueueOffset_;
			}
			get
			{
				return renderQueueOffset_;
			}
		}

		internal int RenderQueue => renderQueue_;

		public bool IsDisposed => material_ == null;

		public Material(Shader shader, Image image = null)
		{
			shader_ = shader;
			image_ = image;
			key_ = shader_.name;
			RenderQueueOffset = 0;
		}

		public Material(UnityEngine.Material material, Image image = null)
		{
			shader_ = material.shader;
			image_ = image;
			key_ = shader_.name;
			RenderQueueOffset = 0;
			material_ = new UnityEngine.Material(material);
			if (image_ != null)
			{
				material_.mainTexture = image_.GetImage();
			}
		}

		public Shader GetShader()
		{
			return shader_;
		}

		public Image GetImage()
		{
			return image_;
		}

		public UnityEngine.Material GetMaterial()
		{
			if (material_ == null)
			{
				material_ = new UnityEngine.Material(shader_);
			}
			if (image_ != null)
			{
				material_.mainTexture = image_.GetImage();
			}
			return material_;
		}

		public bool SetPass(int pass)
		{
			return GetMaterial().SetPass(0);
		}

		public override bool Equals(object obj)
		{
			return this == obj;
		}

		public override int GetHashCode()
		{
			return hashCode_;
		}

		public void Dispose()
		{
			UnityEngine.Object.Destroy(material_);
			material_ = null;
		}
	}
}
