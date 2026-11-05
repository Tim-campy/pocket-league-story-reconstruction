using System.Collections.Generic;
using UnityEngine;

namespace kairo.unity.graphics
{
	public class ShaderManager
	{
		private static ShaderManager instance_;

		private Dictionary<string, Shader> shaders_ = new Dictionary<string, Shader>();

		protected ShaderManager()
		{
		}

		public static ShaderManager GetInstance()
		{
			if (instance_ == null)
			{
				instance_ = new ShaderManager();
			}
			return instance_;
		}

		public Shader Find(string name)
		{
			Shader value = null;
			if (shaders_.TryGetValue(name, out value))
			{
				return value;
			}
			value = Shader.Find(name);
			if (value != null)
			{
				Regist(value);
			}
			return value;
		}

		public void Regist(Shader shader)
		{
			shaders_[shader.name] = shader;
		}
	}
}
