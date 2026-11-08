using System;
using System.Collections.Generic;
using kairo.unity.form;
using kairo.unity.ui;
using UnityEngine;

namespace kairo.unity.graphics
{
	public class MeshManager
	{
		internal class LayerMesh
		{
			public Material Material;

			public MeshBuilder.Layer Layer;

			public int Queue;

			public void Init(Material material, MeshBuilder.Layer layer)
			{
				Material = material;
				Layer = layer;
				Queue = layer.layer * 100000 + Material.RenderQueue;
			}
		}

		internal const int DEPTH_MAX = 1500000;

		internal kairo.unity.ui.Graphics graphics_;

		internal bool dynamic_;

		internal Dictionary<Material, MeshBuilder> meshList_ = new Dictionary<Material, MeshBuilder>();

		internal List<KeyValuePair<Material, MeshBuilder>> sortedMeshList_ = new List<KeyValuePair<Material, MeshBuilder>>();

		internal Offscreen offscreen_;

		internal bool isLowDepth_;

		internal float frontDepth_;

		internal float CurrentDepth;

		internal int CurrentDepthLayer;

		internal bool AlterStaticMesh;

		internal List<LayerMesh> layerMeshes_ = new List<LayerMesh>();

		internal int layerMeshesNum_;

		internal MeshRecord meshRecord_;

		internal List<MeshRecord> meshRecordList_ = new List<MeshRecord>();

		public MeshManager(MeshManager meshMan)
		{
			graphics_ = meshMan.graphics_;
			dynamic_ = meshMan.dynamic_;
			offscreen_ = meshMan.offscreen_;
			isLowDepth_ = meshMan.isLowDepth_;
		}

		public MeshManager(kairo.unity.ui.Graphics g, Offscreen offscreen = null, bool dynamic = true)
		{
			graphics_ = g;
			dynamic_ = dynamic;
			offscreen_ = offscreen;
			isLowDepth_ = g.GetDepthBits() <= 16;
		}

		public MeshBuilder GetMeshBuilder(Material material)
		{
			MeshBuilder value;
			if (!meshList_.TryGetValue(material, out value))
			{
				meshList_[material] = (value = new MeshBuilder(this, !material.UntiColor));
				int renderQueue = material.RenderQueue;
				for (int i = 0; i < sortedMeshList_.Count + 1; i++)
				{
					if (i == sortedMeshList_.Count || renderQueue <= sortedMeshList_[i].Key.RenderQueue)
					{
						sortedMeshList_.Insert(i, new KeyValuePair<Material, MeshBuilder>(material, value));
						break;
					}
				}
			}
			return value;
		}

		public void BeginMesh()
		{
			RemoveInvalidMeshes();
			foreach (KeyValuePair<Material, MeshBuilder> item in meshList_)
			{
				item.Value.Reset();
			}
			for (int i = 0; i < meshRecordList_.Count; i++)
			{
				meshRecordList_[i].Dispose();
			}
			meshRecordList_.Clear();
			int num = 1498976;
			CurrentDepth = num;
			CurrentDepthLayer = 0;
			frontDepth_ = num;
		}

		public void EndMesh()
		{
			AlterStaticMesh = false;
			FormManagerBase formManager = IApplication.GetCurrentApp().GetFormManager();
			if (formManager != null && formManager.GetTopForm() != null && formManager.GetTopForm().CheckMainWindow())
			{
				List<KeyValuePair<Material, MeshBuilder>> list = null;
				foreach (KeyValuePair<Material, MeshBuilder> item in meshList_)
				{
					if (item.Value.GetTriangleCount() == 0)
					{
						item.Value.emptyFrames++;
						if (item.Value.emptyFrames == 100)
						{
							if (list == null)
							{
								list = new List<KeyValuePair<Material, MeshBuilder>>();
							}
							list.Add(item);
						}
					}
					else
					{
						item.Value.emptyFrames = 0;
					}
				}
				if (list != null)
				{
					foreach (KeyValuePair<Material, MeshBuilder> item2 in list)
					{
						item2.Value.Dispose();
						meshList_.Remove(item2.Key);
						for (int num = sortedMeshList_.Count - 1; num >= 0; num--)
						{
							if (sortedMeshList_[num].Key == item2.Key)
							{
								sortedMeshList_.RemoveAt(num);
							}
						}
					}
				}
			}
			UpdateLayerMeshes();
		}

		public void RenderMesh(Matrix4x4 matrix)
		{
			if (offscreen_ == null)
			{
				RenderMeshGL(matrix);
				return;
			}
			for (int i = 0; i < layerMeshesNum_; i++)
			{
				if (i != 0 && layerMeshes_[i - 1].Layer.layer != layerMeshes_[i].Layer.layer)
				{
					graphics_.GLClear(true, false, false);
				}
				MeshBuilder.Layer layer = layerMeshes_[i].Layer;
				Material material = layerMeshes_[i].Material;
				bool flag = false;
				foreach (MeshBuilder.Group group in layer.groups)
				{
					if (group.indexCount > 0)
					{
						if (!flag)
						{
							material.NotAccessFrame = 0;
							material.SetPass(0);
							flag = true;
						}
						UnityEngine.Graphics.DrawMeshNow(group.mesh, matrix);
					}
				}
			}
		}

		private void RenderMeshGL(Matrix4x4 matrix)
		{
			GL.PushMatrix();
			GL.LoadPixelMatrix();
			GL.Viewport(new Rect(0f, 0f, Screen.width, Screen.height));
			for (int i = 0; i < layerMeshesNum_; i++)
			{
				if (i != 0 && layerMeshes_[i - 1].Layer.layer != layerMeshes_[i].Layer.layer)
				{
					graphics_.GLClear(true, false, false);
				}
				MeshBuilder.Layer layer = layerMeshes_[i].Layer;
				Material material = layerMeshes_[i].Material;
				bool flag = false;
				foreach (MeshBuilder.Group group in layer.groups)
				{
					if (group.indexCount <= 0)
					{
						continue;
					}
					if (!flag)
					{
						material.NotAccessFrame = 0;
						material.SetPass(0);
						flag = true;
					}
					GL.Begin(4);
					for (int j = 0; j < group.indexCount; j++)
					{
						int num = group.indicies[j];
						if (group.vertexColor)
						{
							GL.Color(group.colors[num]);
						}
						GL.TexCoord2(group.texcoords[num].x, group.texcoords[num].y);
						GL.Vertex3(group.verticies[num].x, group.verticies[num].y, group.verticies[num].z * -99.9f / 1500000f);
					}
					GL.End();
				}
			}
			GL.PopMatrix();
		}

		public void RemoveInvalidMeshes()
		{
			List<kairo.unity.graphics.Material> materials = null;
			for (int i = this.sortedMeshList_.Count - 1; i >= 0; i--)
			{
				KeyValuePair<kairo.unity.graphics.Material, MeshBuilder> item = this.sortedMeshList_[i];
				Image image = item.Key.GetImage();
				if (item.Key.IsDisposed || image != null && image.IsDisposed)
				{
					item.Value.Dispose();
					if (materials == null)
					{
						materials = new List<kairo.unity.graphics.Material>();
					}
					materials.Add(item.Key);
					for (int j = this.layerMeshesNum_ - 1; j >= 0; j--)
					{
						if (this.layerMeshes_[j].Material == item.Key)
						{
							this.layerMeshes_.RemoveAt(j);
							this.layerMeshesNum_--;
						}
					}
					this.sortedMeshList_.RemoveAt(i);
				}
			}
			if (materials != null)
			{
				foreach (kairo.unity.graphics.Material material in materials)
				{
					this.meshList_.Remove(material);
				}
			}
		}

		public bool CheckBrokenMesh()
		{
			foreach (KeyValuePair<Material, MeshBuilder> item in meshList_)
			{
				Material key = item.Key;
				if (key.IsDisposed || (key.GetImage() != null && key.GetImage().IsDisposed))
				{
					return true;
				}
			}
			return false;
		}

		public void SetDepth(int depth, bool back = false)
		{
			int num = depth;
			if (back)
			{
				num += GetDepthBack();
			}
			if (isLowDepth_)
			{
				num = (int)((float)num * Offscreen.lowDepthScaling_);
			}
			depth = 750000 - num;
			if (CurrentDepth < frontDepth_)
			{
				frontDepth_ = CurrentDepth;
			}
			CurrentDepth = depth;
		}

		public void SetDepthLayer(int layer)
		{
			CurrentDepthLayer = layer;
		}

		public int GetDepthLayer()
		{
			return CurrentDepthLayer;
		}

		public int GetDepthBack()
		{
			int num = -750000;
			if (isLowDepth_)
			{
				num = (int)((float)num / Offscreen.lowDepthScaling_);
			}
			return num;
		}

		public int GetDepthMin()
		{
			return -749999;
		}

		public int GetDepthMax()
		{
			return 750000;
		}

		public void SetDepthFront()
		{
			if (frontDepth_ < CurrentDepth)
			{
				CurrentDepth = frontDepth_;
			}
		}

		public void Clear()
		{
			foreach (KeyValuePair<Material, MeshBuilder> item in meshList_)
			{
				if (item.Value.GetTriangleCount() > 0)
				{
					item.Value.Dispose();
				}
			}
			meshList_.Clear();
			sortedMeshList_.Clear();
			layerMeshesNum_ = 0;
		}

		public bool IsDynamic()
		{
			return dynamic_;
		}

		public kairo.unity.ui.Graphics GetGraphics()
		{
			return graphics_;
		}

		public void UpdateLayerMeshes()
		{
			int num = int.MaxValue;
			int num2 = int.MinValue;
			layerMeshesNum_ = 0;
			foreach (KeyValuePair<Material, MeshBuilder> item in sortedMeshList_)
			{
				MeshBuilder value = item.Value;
				foreach (MeshBuilder.Layer value2 in value.layers.Values)
				{
					int num3 = 0;
					foreach (MeshBuilder.Group group in value2.groups)
					{
						num3 += group.indexCount;
					}
					if (num3 <= 0)
					{
						continue;
					}
					if (value2.layer < num)
					{
						num = value2.layer;
					}
					if (value2.layer > num2)
					{
						num2 = value2.layer;
					}
					if (layerMeshesNum_ >= layerMeshes_.Count)
					{
						layerMeshes_.Add(new LayerMesh());
					}
					if (offscreen_ != null)
					{
						foreach (MeshBuilder.Group group2 in value2.groups)
						{
							if (group2.indexCount > 0)
							{
								group2.mesh = value.ConvertMesh(group2);
								group2.mesh.Optimize();
								group2.mesh.RecalculateBounds();
							}
						}
					}
					layerMeshes_[layerMeshesNum_].Init(item.Key, value2);
					layerMeshesNum_++;
				}
			}
			if (layerMeshesNum_ > 0 && num != num2)
			{
				QuickSort(layerMeshes_, 0, layerMeshesNum_ - 1);
			}
		}

		internal void EndAlterRender()
		{
			int num = int.MaxValue;
			int num2 = int.MinValue;
			AlterStaticMesh = false;
			layerMeshesNum_ = 0;
			foreach (KeyValuePair<Material, MeshBuilder> item in sortedMeshList_)
			{
				MeshBuilder value = item.Value;
				foreach (MeshBuilder.Layer value2 in value.layers.Values)
				{
					int num3 = 0;
					foreach (MeshBuilder.Group group in value2.groups)
					{
						if (group.change)
						{
							group.UpdateRemoveMesh(meshRecordList_);
						}
						num3 += group.indexCount;
					}
					if (num3 <= 0)
					{
						continue;
					}
					if (value2.layer < num)
					{
						num = value2.layer;
					}
					if (value2.layer > num2)
					{
						num2 = value2.layer;
					}
					if (layerMeshesNum_ >= layerMeshes_.Count)
					{
						layerMeshes_.Add(new LayerMesh());
					}
					if (offscreen_ != null)
					{
						foreach (MeshBuilder.Group group2 in value2.groups)
						{
							if (group2.indexCount > 0 && group2.change)
							{
								group2.mesh = value.ConvertMesh(group2);
								group2.mesh.Optimize();
								group2.mesh.RecalculateBounds();
							}
						}
					}
					layerMeshes_[layerMeshesNum_].Init(item.Key, value2);
					layerMeshesNum_++;
				}
			}
			if (layerMeshesNum_ > 0 && num != num2)
			{
				QuickSort(layerMeshes_, 0, layerMeshesNum_ - 1);
			}
		}

		private void QuickSort(List<LayerMesh> a, int i, int j)
		{
			if (i == j)
			{
				return;
			}
			int num = this.Pivot(a, i, j);
			if (num != -1)
			{
				int num1 = this.Partition(a, i, j, a[num].Queue);
				this.QuickSort(a, i, num1 - 1);
				this.QuickSort(a, num1, j);
			}
		}

		private int Pivot(List<LayerMesh> a, int i, int j)
		{
			int k;
			for (k = i + 1; k <= j && a[i].Queue == a[k].Queue; k++)
			{
			}
			if (k > j)
			{
				return -1;
			}
			if (a[i].Queue >= a[k].Queue)
			{
				return i;
			}
			return k;
		}

		private int Partition(List<LayerMesh> a, int i, int j, int x)
		{
			int k = i;
			int num = j;
			while (k <= num)
			{
				for (; k <= j && a[k].Queue < x; k++)
				{
				}
				while (num >= i && a[num].Queue >= x)
				{
					num--;
				}
				if (k > num)
				{
					break;
				}
				LayerMesh value = a[k];
				a[k] = a[num];
				a[num] = value;
				k++;
				num--;
			}
			return k;
		}
	}
}
