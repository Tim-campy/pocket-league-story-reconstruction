using System;
using System.Collections.Generic;
using kairo.common.cfg;
using kairo.unity.ui;
using kairo.unity.util;
using UnityEngine;

namespace kairo.unity.graphics
{
	public class Offscreen : MonoBehaviour, IDisposable
	{
		public delegate bool RenderMethod(kairo.unity.ui.Graphics g, Offscreen offscreen);

		public delegate bool MultiRenderMethod(kairo.unity.ui.Graphics g, Offscreen offscreen, int id);

		public delegate void GetLayoutMethod(out int width, out int height, out Image.Format format, out Image.Filter filterMode);

		private static GameObject prefabOffscreen_;

		private static List<Offscreen> offscreenList_ = new List<Offscreen>();

		private RenderMethod DynamicRender;

		private RenderMethod StaticRedner;

		private GetLayoutMethod GetLayout;

		private Image image_;

		private bool isEmpty_ = true;

		private bool clear_ = true;

		private bool dispose_;

		private Matrix4x4[] staticTransform_ = new Matrix4x4[1] { Matrix4x4.identity };

		private Matrix4x4 staticTransformGen_ = Matrix4x4.identity;

		internal Matrix4x4 dynamicTransform_ = Matrix4x4.identity;

		private bool staticRenderd_;

		private kairo.unity.ui.Graphics graDynamics_;

		private kairo.unity.ui.Graphics graStatics_;

		private static bool rendering_;

		internal static int DepthBits;

		internal static float lowDepthScaling_ = 1f;

		private Matrix4x4[] renderMatrix_ = new Matrix4x4[2];

		private MeshManager[] renderMesh_ = new MeshManager[2];

		private int[] renderIndex_ = new int[2];

		private int[] renderLength_ = new int[2];

		private List<MeshManager.LayerMesh>[] renderList_ = new List<MeshManager.LayerMesh>[2];

		private int lastRenderMaterialCount_;

		private List<Material> lastRenderMaterials_ = new List<Material>();

		public Image Image => image_;

		public bool IsEmpty => isEmpty_;

		private Offscreen()
		{
		}

		public static Offscreen CreateOffscreen(RenderMethod dynamicRender, RenderMethod staticRender, GetLayoutMethod layout, int order = 0)
		{
			return (Offscreen)IApplication.GetCurrentApp().RunOnUiThread(delegate
			{
				if (prefabOffscreen_ == null)
				{
					prefabOffscreen_ = (GameObject)Resources.Load("Prefabs/Offscreen");
				}
				GameObject obj = UnityEngine.Object.Instantiate(prefabOffscreen_);
				obj.GetComponent<Camera>().depth = -100 + order;
				obj.AddComponent<Offscreen>();
				Offscreen component = obj.GetComponent<Offscreen>();
				component.DynamicRender = dynamicRender;
				component.StaticRedner = staticRender;
				component.GetLayout = layout;
				offscreenList_.Add(component);
				component.image_ = new Image(new RenderTexture(1, 1, 24, RenderTextureFormat.ARGB32)
				{
					enableRandomWrite = false
				});
				int width;
				int height;
				Image.Format format;
				Image.Filter filterMode;
				component.GetLayout(out width, out height, out format, out filterMode);
				component.Resize(width, height, format, filterMode, true);
				component.graStatics_ = new kairo.unity.ui.Graphics(component, false);
				return component;
			}, true);
		}

		public static void Render(Image image, RenderMethod render)
		{
			IApplication.GetCurrentApp().RunOnUiThread(delegate
			{
				if (IApplication.GetCurrentApp().IsRendering())
				{
					Log.Info("Offscreen#Render:アプリケーションがレンダリング中の為、この関数は実行出来ません。");
				}
				if (prefabOffscreen_ == null)
				{
					prefabOffscreen_ = (GameObject)Resources.Load("Prefabs/Offscreen");
				}
				GameObject obj = UnityEngine.Object.Instantiate(prefabOffscreen_);
				obj.GetComponent<Camera>().clearFlags = CameraClearFlags.Color;
				obj.GetComponent<Camera>().backgroundColor = new Color(0f, 0f, 0f, 0f);
				obj.AddComponent<Offscreen>();
				Offscreen component = obj.GetComponent<Offscreen>();
				component.DynamicRender = render;
				component.image_ = new Image(new RenderTexture(1, 1, 24, RenderTextureFormat.ARGB32)
				{
					enableRandomWrite = false
				});
				component.Resize(image.GetWidth(), image.GetHeight(), (Image.Format)((Texture2D)image.GetImage()).format, (Image.Filter)image.GetImage().filterMode, true);
				component.image_.SetAlphaType(image.GetAlphaType());
				obj.GetComponent<Camera>().Render();
				Texture2D texture2D = (Texture2D)image.GetImage();
				RenderTexture.active = (RenderTexture)component.Image.GetImage();
				if (Config.UNITY_EDITOR)
				{
					texture2D.ReadPixels(new Rect(0f, 0f, image.GetWidth(), image.GetHeight()), 0, 0);
				}
				else if (Config.APPLI_TYPE == 3)
				{
					Texture2D texture2D2 = new Texture2D(component.GetWidth(), component.GetHeight());
					texture2D2.ReadPixels(new Rect(0f, 0f, texture2D2.width, texture2D2.height), 0, 0);
					texture2D2.Apply();
					int y = component.GetHeight() - image.GetHeight();
					texture2D.SetPixels(0, 0, image.GetWidth(), image.GetHeight(), texture2D2.GetPixels(0, y, image.GetWidth(), image.GetHeight()));
					UnityEngine.Object.Destroy(texture2D2);
					texture2D2 = null;
				}
				else
				{
					texture2D.ReadPixels(new Rect(0f, component.GetHeight() - image.GetHeight(), image.GetWidth(), image.GetHeight()), 0, 0);
				}
				texture2D.Apply();
				RenderTexture.active = null;
				component.Dispose();
			}, true);
		}

		private bool Resize(int width, int height, Image.Format format = Image.Format.ARGB32, Image.Filter filterMode = Image.Filter.Default, bool force = false)
		{
			if (image_ == null || dispose_)
			{
				return false;
			}
			if (format == Image.Format.Default)
			{
				format = Image.Format.ARGB32;
			}
			if (filterMode == Image.Filter.Default)
			{
				filterMode = Image.DEFAULT_FILTER;
			}
			return (bool)IApplication.GetCurrentApp().RunOnUiThread(delegate
			{
				int num = Image.GetPowerOf2Value(width);
				int num2 = Image.GetPowerOf2Value(height);
				if (num < 32)
				{
					num = 32;
				}
				if (num2 < 32)
				{
					num2 = 32;
				}
				RenderTexture renderTexture = (RenderTexture)image_.GetImage();
				if (!force && num == image_.GetWidth() && num2 == image_.GetHeight() && Image.GetRenderTextureFormat(format) == renderTexture.format)
				{
					if (filterMode != (Image.Filter)renderTexture.filterMode)
					{
						renderTexture.filterMode = (FilterMode)filterMode;
					}
					if (Image.IsAlphaFormat(format))
					{
						image_.SetAlphaType(1);
					}
					else
					{
						image_.SetAlphaType(0);
					}
					return false;
				}
				GetComponent<Camera>().targetTexture = null;
				image_.Dispose();
				RenderTexture renderTexture2 = new RenderTexture(num, num2, 24, Image.GetRenderTextureFormat(format))
				{
					enableRandomWrite = false,
					filterMode = (FilterMode)filterMode
				};
				image_ = new Image(renderTexture2);
				if (Image.IsAlphaFormat(format))
				{
					image_.SetAlphaType(1);
				}
				else
				{
					image_.SetAlphaType(0);
				}
				if (Config.PRINT && Image.GetRenderTextureFormat(format) != renderTexture2.format)
				{
					Log.Info(string.Concat(format, " は使用しないでください。"));
				}
				GetComponent<Camera>().orthographicSize = image_.GetHeight() / 2;
				GetComponent<Camera>().transform.position = new Vector3(image_.GetWidth() / 2, image_.GetHeight() / 2, GetComponent<Camera>().transform.position.z);
				GetComponent<Camera>().targetTexture = renderTexture2;
				if (graDynamics_ != null)
				{
					graDynamics_.Dispose();
				}
				graDynamics_ = new kairo.unity.ui.Graphics(this);
				isEmpty_ = true;
				clear_ = true;
				return true;
			}, true);
		}

		public void SetStaticsTransform(int x, int y, int depth = 0, float scaleX = 1f, float scaleY = 1f)
		{
			SetStaticsTransform(Matrix4x4.TRS(new Vector3(x, -y, -depth), Quaternion.identity, new Vector3(scaleX, scaleY, 1f)));
		}

		public void SetStaticsTransform(Matrix4x4 matrix)
		{
			staticTransformGen_ = matrix;
			for (int i = 0; i < staticTransform_.Length; i++)
			{
				staticTransform_[i] = matrix;
			}
		}

		public void SetMultiStaticsTransform(int id, int x, int y, int depth = 0, float scaleX = 1f, float scaleY = 1f)
		{
			SetMultiStaticsTransform(id, Matrix4x4.TRS(new Vector3(x, -y, -depth), Quaternion.identity, new Vector3(scaleX, scaleY, 1f)));
		}

		public void SetMultiStaticsTransform(int id, Matrix4x4 matrix)
		{
			if (id >= staticTransform_.Length)
			{
				Matrix4x4[] array = new Matrix4x4[id + 1];
				for (int i = 0; i < array.Length; i++)
				{
					array[i] = staticTransformGen_;
				}
				Array.Copy(staticTransform_, array, staticTransform_.Length);
				staticTransform_ = array;
			}
			staticTransform_[id] = matrix;
		}

		public void SetDynamicsTransform(int x, int y, int depth = 0, float scaleX = 1f, float scaleY = 1f)
		{
			dynamicTransform_ = Matrix4x4.TRS(new Vector3(x, -y, -depth), Quaternion.identity, new Vector3(scaleX, scaleY, 1f));
		}

		public void SetDynamicsTransform(Matrix4x4 matrix)
		{
			dynamicTransform_ = matrix;
		}

		public static void SetLowDepthScaling(float scale = 1024f)
		{
			lowDepthScaling_ = scale;
		}

		public void RenderStatics()
		{
			if (graStatics_ == null || dispose_)
			{
				return;
			}
			kairo.unity.ui.Canvas instance = kairo.unity.ui.Canvas.GetInstance();
			if (StaticRedner != null)
			{
				kairo.unity.ui.Graphics graphics = instance.BeginPaint(graStatics_);
				if (graphics != null)
				{
					try
					{
						StaticRedner(graphics, this);
					}
					finally
					{
						instance.EndPaint(graphics);
						isEmpty_ = false;
					}
				}
			}
			staticRenderd_ = true;
			isEmpty_ = true;
		}

		public void RenderAlterStatics(RenderMethod method)
		{
			kairo.unity.ui.Canvas instance = kairo.unity.ui.Canvas.GetInstance();
			kairo.unity.ui.Graphics graphics = instance.BeginAlterStaticRender(graStatics_);
			if (graphics != null)
			{
				try
				{
					method?.Invoke(graphics, this);
				}
				finally
				{
					instance.EndAlterStaticRender(graphics);
					isEmpty_ = false;
				}
			}
		}

		public void RenderMultiStatics(int id, MultiRenderMethod method)
		{
			if (graStatics_ == null || dispose_)
			{
				return;
			}
			kairo.unity.ui.Canvas instance = kairo.unity.ui.Canvas.GetInstance();
			kairo.unity.ui.Graphics graphics = instance.BeginPaint(graStatics_, id);
			if (graphics != null)
			{
				try
				{
					method(graphics, this, id);
				}
				finally
				{
					instance.EndPaint(graphics, id);
				}
			}
			isEmpty_ = true;
		}

		public void Dispose()
		{
			dispose_ = true;
			GetComponent<Camera>().targetTexture = null;
			if (SystemInfo.deviceModel == "motorola Nexus 6")
			{
				UnityEngine.Object.Destroy(base.gameObject, 5f);
			}
			else
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
		}

		private void OnDestroy()
		{
			if (image_ != null)
			{
				image_.Dispose();
			}
			image_ = null;
			if (graDynamics_ != null)
			{
				graDynamics_.Dispose();
			}
			graDynamics_ = null;
			if (graStatics_ != null)
			{
				graStatics_.Dispose();
			}
			graStatics_ = null;
			DynamicRender = null;
			StaticRedner = null;
			isEmpty_ = true;
		}

		public static void DisposeAll()
		{
			for (int num = offscreenList_.Count - 1; num >= 0; num--)
			{
				offscreenList_[num].Dispose();
				offscreenList_.RemoveAt(num);
			}
		}

		private void LateUpdate()
		{
			if (image_ != null && !dispose_ && GetLayout != null)
			{
				int width;
				int height;
				Image.Format format;
				Image.Filter filterMode;
				GetLayout(out width, out height, out format, out filterMode);
				Resize(width, height, format, filterMode);
			}
		}

		private void OnPostRender()
		{
			if (image_ == null || dispose_)
			{
				return;
			}
			rendering_ = true;
			try
			{
				IApplication currentApp = IApplication.GetCurrentApp();
				kairo.unity.ui.Canvas instance = kairo.unity.ui.Canvas.GetInstance();
				currentApp.GetFormManager();
				MeshManager meshManager = graDynamics_.GetMeshManager();
				if (clear_)
				{
					Clear(0, 0, 0, 0);
					clear_ = false;
				}
				if (meshManager.CheckBrokenMesh())
				{
					isEmpty_ = true;
				}
				if (graStatics_ != null)
				{
					for (int i = 0; i < graStatics_.meshManList_.Length; i++)
					{
						if (graStatics_.meshManList_[i] != null && graStatics_.meshManList_[i].CheckBrokenMesh())
						{
							staticRenderd_ = false;
							isEmpty_ = true;
							break;
						}
					}
				}
				if (!staticRenderd_)
				{
					RenderStatics();
				}
				bool flag = false;
				MeshManager[] array = renderMesh_;
				foreach (MeshManager meshManager2 in array)
				{
					if (meshManager2 != null && meshManager2.AlterStaticMesh)
					{
						flag = true;
					}
				}
				if (flag)
				{
					RenderAlterStatics(null);
				}
				kairo.unity.ui.Graphics graphics = instance.BeginPaint(graDynamics_);
				if (graphics == null)
				{
					return;
				}
				List<Image.TextRenderTask> textRenderTasks_ = image_.textRenderTasks_;
				List<Image.BalloonRenderTask> balloonRenderTasks_ = image_.balloonRenderTasks_;
				List<Image.TouchTask> touchTasks_ = image_.touchTasks_;
				image_.textRenderTasks_ = new List<Image.TextRenderTask>();
				image_.balloonRenderTasks_ = new List<Image.BalloonRenderTask>();
				image_.touchTasks_ = new List<Image.TouchTask>();
				bool flag2 = false;
				try
				{
					flag2 = DynamicRender == null || DynamicRender(graphics, this);
				}
				finally
				{
					instance.EndPaint(graphics);
					isEmpty_ = false;
				}
				if (flag2)
				{
					textRenderTasks_.Clear();
					for (int k = 0; k < touchTasks_.Count; k++)
					{
						touchTasks_[k].Component.hasCount--;
					}
					touchTasks_.Clear();
					int num = graphics.cameraDepth_;
					if (GetDepthBits() <= 16)
					{
						num = (int)((float)num * lowDepthScaling_);
					}
					Matrix4x4 matrix4x = Matrix4x4.TRS(new Vector3(0f, 0f, num), Quaternion.identity, new Vector3(1f, 1f, 1f));
					int num2 = 1;
					if (graStatics_ != null)
					{
						num2 += graStatics_.meshManList_.Length;
					}
					if (num2 > renderMesh_.Length)
					{
						renderMesh_ = new MeshManager[num2];
						renderMatrix_ = new Matrix4x4[num2];
						renderIndex_ = new int[num2];
						renderLength_ = new int[num2];
						renderList_ = new List<MeshManager.LayerMesh>[num2];
					}
					for (int l = 0; l < num2; l++)
					{
						if (l == 0)
						{
							renderMesh_[l] = meshManager;
							renderMatrix_[l] = dynamicTransform_ * matrix4x;
						}
						else
						{
							int num3 = l - 1;
							renderMesh_[l] = graStatics_.meshManList_[num3];
							if (num3 < staticTransform_.Length)
							{
								renderMatrix_[l] = staticTransform_[num3];
							}
							else
							{
								renderMatrix_[l] = staticTransformGen_;
							}
							renderMatrix_[l] *= Matrix4x4.TRS(new Vector3(0f, GetHeight(), 0f), Quaternion.identity, new Vector3(1f, 1f, 1f));
							renderMatrix_[l] *= matrix4x;
						}
						renderIndex_[l] = 0;
						renderLength_[l] = 0;
						if (renderMesh_[l] != null)
						{
							renderLength_[l] = renderMesh_[l].layerMeshesNum_;
							renderList_[l] = renderMesh_[l].layerMeshes_;
						}
					}
					lastRenderMaterialCount_ = 0;
					int num4 = int.MinValue;
					while (true)
					{
						int num5 = 0;
						int num6 = -1;
						for (int m = 0; m < num2; m++)
						{
							if (renderIndex_[m] < renderLength_[m])
							{
								int queue = renderList_[m][renderIndex_[m]].Queue;
								if (num6 == -1 || queue < num5)
								{
									num6 = m;
									num5 = queue;
								}
							}
						}
						if (num6 == -1)
						{
							break;
						}
						List<MeshManager.LayerMesh> list = renderList_[num6];
						int queue2 = list[renderIndex_[num6]].Queue;
						while (renderIndex_[num6] < renderLength_[num6] && queue2 == list[renderIndex_[num6]].Queue)
						{
							if (list[renderIndex_[num6]].Layer.GetIndexCount() > 0)
							{
								if (num4 != int.MinValue && num4 != list[renderIndex_[num6]].Layer.layer)
								{
									GL.Clear(true, false, Color.white);
								}
								num4 = list[renderIndex_[num6]].Layer.layer;
								Material material = list[renderIndex_[num6]].Material;
								material.NotAccessFrame = 0;
								material.SetPass(0);
								foreach (MeshBuilder.Group group in list[renderIndex_[num6]].Layer.groups)
								{
									UnityEngine.Graphics.DrawMeshNow(group.mesh, renderMatrix_[num6]);
								}
								Image image = material.GetImage();
								if (image != null)
								{
									image.checkCount_ = Config.checkCount_;
								}
								if (lastRenderMaterialCount_ >= lastRenderMaterials_.Count)
								{
									lastRenderMaterials_.Add(null);
								}
								lastRenderMaterials_[lastRenderMaterialCount_] = material;
								lastRenderMaterialCount_++;
							}
							renderIndex_[num6]++;
						}
					}
					for (int n = 0; n < image_.textRenderTasks_.Count; n++)
					{
						Image.TextRenderTask value = image_.textRenderTasks_[n];
						Vector3 vector = dynamicTransform_.MultiplyPoint(new Vector3(value.X, 0f - value.Y, 0f));
						value.X = vector.x;
						value.Y = 0f - vector.y;
						image_.textRenderTasks_[n] = value;
					}
					for (int num7 = 0; num7 < image_.balloonRenderTasks_.Count; num7++)
					{
						Image.BalloonRenderTask value2 = image_.balloonRenderTasks_[num7];
						Vector3 vector2 = dynamicTransform_.MultiplyPoint(new Vector3(value2.X, 0f - value2.Y, 0f));
						value2.X = vector2.x;
						value2.Y = 0f - vector2.y;
						image_.balloonRenderTasks_[num7] = value2;
					}
					return;
				}
				image_.textRenderTasks_ = textRenderTasks_;
				image_.balloonRenderTasks_ = balloonRenderTasks_;
				image_.touchTasks_ = touchTasks_;
				for (int num8 = 0; num8 < lastRenderMaterialCount_; num8++)
				{
					Material material2 = lastRenderMaterials_[num8];
					material2.NotAccessFrame = 0;
					Image image2 = material2.GetImage();
					if (image2 != null)
					{
						image2.checkCount_ = Config.checkCount_;
					}
				}
			}
			catch (Exception t)
			{
				Log.Info(t);
			}
			finally
			{
				rendering_ = false;
			}
		}

		internal void Flush()
		{
			if (image_ != null && !dispose_)
			{
				MeshManager meshManager = graDynamics_.GetMeshManager();
				meshManager.EndMesh();
				meshManager.RenderMesh(dynamicTransform_);
				meshManager.BeginMesh();
			}
		}

		public int GetWidth()
		{
			return image_.GetWidth();
		}

		public int GetHeight()
		{
			return image_.GetHeight();
		}

		public static bool IsRendering()
		{
			return rendering_;
		}

		public void Clear(int color)
		{
			if (image_ != null && !dispose_)
			{
				Color backgroundColor = kairo.unity.ui.Graphics.ArgbToColor(color);
				if (image_.format_ == Image.Format.RGB24 || image_.format_ == Image.Format.RGB565)
				{
					backgroundColor.a = 1f;
				}
				isEmpty_ = true;
				GL.Clear(false, true, backgroundColor);
			}
		}

		public void Clear(int r, int g, int b, int a = 255)
		{
			Clear(kairo.unity.ui.Graphics.GetColorOfRGB(r, g, b, a));
		}

		public static int GetDepthBits()
		{
			return DepthBits;
		}
	}
}
