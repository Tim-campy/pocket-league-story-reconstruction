using System;
using System.Collections.Generic;
using UnityEngine;

namespace kairo.unity.graphics
{
	public class MeshBuilder : IDisposable
	{
		internal class Group
		{
			internal MeshBuilder builder;

			internal Mesh mesh = new Mesh();

			internal int vertexCount;

			internal Vector3[] verticies = new Vector3[0];

			internal Vector2[] texcoords = new Vector2[0];

			internal Color[] colors = new Color[0];

			internal int indexCount;

			internal int[] indicies = new int[0];

			internal List<int> triangles = new List<int>();

			internal bool vertexColor;

			internal int head;

			internal List<int> removeIndicies = new List<int>();

			internal bool change;

			internal Group(MeshBuilder mb)
			{
				builder = mb;
				vertexColor = mb.vertexColor;
				mesh.MarkDynamic();
			}

			internal void RemoveIndex(int index)
			{
				removeIndicies.Add(index);
				change = true;
				builder.meshMan.AlterStaticMesh = true;
			}

			public void UpdateRemoveMesh(List<MeshRecord> list)
			{
				if (removeIndicies.Count <= 0)
				{
					return;
				}
				int[] array = new int[vertexCount];
				for (int i = 0; i < removeIndicies.Count; i++)
				{
					array[indicies[removeIndicies[i]]] = -1;
				}
				int num = 0;
				for (int j = 0; j < vertexCount; j++)
				{
					if (array[j] != -1)
					{
						array[j] = num;
						verticies[num] = verticies[j];
						if (texcoords != null && j < texcoords.Length)
						{
							texcoords[num] = texcoords[j];
						}
						if (colors != null && j < colors.Length)
						{
							colors[num] = colors[j];
						}
						num++;
					}
				}
				vertexCount = num;
				for (int k = 0; k < removeIndicies.Count - 1; k++)
				{
					for (int num2 = removeIndicies.Count - 1; num2 > k; num2--)
					{
						if (removeIndicies[num2 - 1] > removeIndicies[num2])
						{
							int value = removeIndicies[num2];
							removeIndicies[num2] = removeIndicies[num2 - 1];
							removeIndicies[num2 - 1] = value;
						}
					}
				}
				List<int[]> list2 = new List<int[]>();
				for (int l = 0; l < indexCount; l++)
				{
					list2.Add(null);
				}
				for (int m = 0; m < list.Count; m++)
				{
					for (int n = 0; n < list[m].Groups.Count; n++)
					{
						if (list[m].Groups[n] == this)
						{
							list2[list[m].Indicies[n][0]] = list[m].Indicies[n];
						}
					}
				}
				for (int num3 = removeIndicies.Count - 1; num3 >= 0; num3--)
				{
					list2.RemoveAt(removeIndicies[num3]);
				}
				for (int num4 = 0; num4 < list2.Count; num4++)
				{
					if (list2[num4] != null)
					{
						list2[num4][0] = num4;
					}
				}
				int num5 = removeIndicies[0];
				num = 0;
				for (int num6 = 0; num6 < indexCount; num6++)
				{
					if (num6 == num5)
					{
						removeIndicies.RemoveAt(0);
						if (removeIndicies.Count > 0)
						{
							num5 = removeIndicies[0];
						}
					}
					else
					{
						indicies[num] = array[indicies[num6]];
						num++;
					}
				}
				indexCount = num;
			}
		}

		internal class Layer
		{
			internal int layer;

			internal List<Group> groups = new List<Group>();

			internal int currentGroup;

			private const int DEPTH_MAX = 1498976;

			internal Group Current => groups[currentGroup];

			internal Layer(MeshBuilder mb, int layer)
			{
				this.layer = layer;
				groups.Add(new Group(mb));
			}

			internal int GetIndexCount()
			{
				int num = 0;
				for (int i = 0; i < groups.Count; i++)
				{
					num += groups[i].indexCount;
				}
				return num;
			}
		}

		private MeshManager meshMan;

		internal Dictionary<int, Layer> layers = new Dictionary<int, Layer>();

		internal Layer currentLayer;

		internal bool vertexColor;

		internal int emptyFrames;

		internal float DepthTotal;

		private float DEPTH_SCALE;

		private float DEPTH_STEP;

		public MeshBuilder(MeshManager meshMan, bool vertexColor)
		{
			this.meshMan = meshMan;
			this.vertexColor = vertexColor;
			layers[0] = new Layer(this, 0);
			DEPTH_SCALE = ((meshMan.GetGraphics().GetDepthBits() > 16) ? 1 : 1024);
			if (meshMan.offscreen_ == null)
			{
				DEPTH_SCALE = 1024f;
			}
			DEPTH_STEP = 0.1f * DEPTH_SCALE;
			Reset();
		}

		public void Reset()
		{
			foreach (Layer value in layers.Values)
			{
				foreach (Group group in value.groups)
				{
					group.vertexCount = 0;
					group.indexCount = 0;
					group.head = 0;
				}
			}
			DepthTotal = 0f;
		}

		public void ReadyAddition()
		{
			CheckAndExpandGroup();
			meshMan.CurrentDepth -= DEPTH_STEP;
			currentLayer = GetLayer();
			currentLayer.Current.head = currentLayer.Current.vertexCount;
		}

		protected void CheckAndExpandGroup()
		{
			Layer layer = GetLayer();
			if (layer.Current != null)
			{
				Group obj = layer.Current;
				if (obj.vertexCount >= 60000 || obj.indexCount >= 55000)
				{
					layer.groups.Add(obj = new Group(this));
					layer.currentGroup++;
				}
				obj.change = true;
			}
		}

		protected void CheckAndExpand(int add)
		{
			Group current = currentLayer.Current;
			if (current.verticies.Length < current.vertexCount + add)
			{
				int num = current.vertexCount + add + current.vertexCount / 2;
				Vector3[] array = new Vector3[num];
				Vector2[] array2 = new Vector2[num];
				Array.Copy(current.verticies, 0, array, 0, current.verticies.Length);
				Array.Copy(current.texcoords, 0, array2, 0, current.texcoords.Length);
				current.verticies = array;
				current.texcoords = array2;
				if (vertexColor)
				{
					Color[] array3 = new Color[num];
					Array.Copy(current.colors, 0, array3, 0, current.colors.Length);
					current.colors = array3;
				}
			}
		}

		public void AddVertex(float x, float y, float z, Color color)
		{
			AddVertex(x, y, z, 0f, 0f, color);
		}

		public void AddVertex(Vector3 vertex, Color color)
		{
			AddVertex(vertex.x, vertex.y, vertex.z, 0f, 0f, color);
		}

		public void AddVertex(float x, float y, float z, float u, float v)
		{
			AddVertex(x, y, z, u, v, Color.white);
		}

		public void AddVertex(Vector3 vertex, float u, float v)
		{
			AddVertex(vertex.x, vertex.y, vertex.z, u, v, Color.white);
		}

		public void AddVertex(float x, float y, float z, float u, float v, Color color)
		{
			Group current = currentLayer.Current;
			CheckAndExpand(1);
			current.verticies[current.vertexCount].x = x;
			current.verticies[current.vertexCount].y = y;
			current.verticies[current.vertexCount].z = z * DEPTH_SCALE + meshMan.CurrentDepth;
			current.texcoords[current.vertexCount].x = u;
			current.texcoords[current.vertexCount].y = v;
			if (vertexColor)
			{
				current.colors[current.vertexCount] = color;
			}
			DepthTotal += current.verticies[current.vertexCount].z;
			current.vertexCount++;
		}

		public void AddVertex(Vector3 vertex, float u, float v, Color color)
		{
			AddVertex(vertex.x, vertex.y, vertex.z, u, v, color);
		}

		public void AddQuad(Vector3[] vertices, float tu, float tv, float tw, float th)
		{
			AddQuad(vertices, tu, tv, tw, th, Color.white);
		}

		public void AddQuad(Vector3[] vertices, float tu, float tv, float tw, float th, Color color)
		{
			CheckAndExpandGroup();
			meshMan.CurrentDepth -= DEPTH_STEP;
			Group current = (currentLayer = GetLayer()).Current;
			CheckAndExpand(4);
			current.head = current.vertexCount;
			int num = 0;
			while (num < vertices.Length)
			{
				current.verticies[current.vertexCount].x = vertices[num].x;
				current.verticies[current.vertexCount].y = vertices[num].y;
				current.verticies[current.vertexCount].z = vertices[num].z * DEPTH_SCALE + meshMan.CurrentDepth;
				current.texcoords[current.vertexCount].x = tu;
				current.texcoords[current.vertexCount].y = tv;
				if (num == 1 || num == 2)
				{
					current.texcoords[current.vertexCount].x += tw;
				}
				if (num == 0 || num == 1)
				{
					current.texcoords[current.vertexCount].y += th;
				}
				if (vertexColor)
				{
					current.colors[current.vertexCount] = color;
				}
				DepthTotal += current.verticies[current.vertexCount].z;
				num++;
				current.vertexCount++;
			}
			if (current.indicies.Length < current.indexCount + 6)
			{
				int[] array = new int[current.indexCount + 6 + 6 + current.indexCount / 2];
				Array.Copy(current.indicies, 0, array, 0, current.indicies.Length);
				current.indicies = array;
			}
			if (meshMan.meshRecord_ != null)
			{
				meshMan.meshRecord_.Add(current, current.indexCount, 6);
			}
			current.indicies[current.indexCount + 0] = current.head + 0;
			current.indicies[current.indexCount + 1] = current.head + 1;
			current.indicies[current.indexCount + 2] = current.head + 2;
			current.indicies[current.indexCount + 3] = current.head + 0;
			current.indicies[current.indexCount + 4] = current.head + 2;
			current.indicies[current.indexCount + 5] = current.head + 3;
			current.indexCount += 6;
		}

		public void AddQuad(RectangleF dst, float tu, float tv, float tw, float th, Color color)
		{
			CheckAndExpandGroup();
			meshMan.CurrentDepth -= DEPTH_STEP;
			Group current = (currentLayer = GetLayer()).Current;
			CheckAndExpand(4);
			current.head = current.vertexCount;
			float currentDepth = meshMan.CurrentDepth;
			DepthTotal += currentDepth * 4f;
			current.verticies[current.vertexCount].x = dst.left;
			current.verticies[current.vertexCount].y = dst.top;
			current.verticies[current.vertexCount].z = currentDepth;
			current.texcoords[current.vertexCount].x = tu;
			current.texcoords[current.vertexCount].y = tv + th;
			if (vertexColor)
			{
				current.colors[current.vertexCount] = color;
			}
			current.vertexCount++;
			current.verticies[current.vertexCount].x = dst.right;
			current.verticies[current.vertexCount].y = dst.top;
			current.verticies[current.vertexCount].z = currentDepth;
			current.texcoords[current.vertexCount].x = tu + tw;
			current.texcoords[current.vertexCount].y = tv + th;
			if (vertexColor)
			{
				current.colors[current.vertexCount] = color;
			}
			current.vertexCount++;
			current.verticies[current.vertexCount].x = dst.right;
			current.verticies[current.vertexCount].y = dst.bottom;
			current.verticies[current.vertexCount].z = currentDepth;
			current.texcoords[current.vertexCount].x = tu + tw;
			current.texcoords[current.vertexCount].y = tv;
			if (vertexColor)
			{
				current.colors[current.vertexCount] = color;
			}
			current.vertexCount++;
			current.verticies[current.vertexCount].x = dst.left;
			current.verticies[current.vertexCount].y = dst.bottom;
			current.verticies[current.vertexCount].z = currentDepth;
			current.texcoords[current.vertexCount].x = tu;
			current.texcoords[current.vertexCount].y = tv;
			if (vertexColor)
			{
				current.colors[current.vertexCount] = color;
			}
			current.vertexCount++;
			if (current.indicies.Length < current.indexCount + 6)
			{
				int[] array = new int[current.indexCount + 6 + 6 + current.indexCount / 2];
				Array.Copy(current.indicies, 0, array, 0, current.indicies.Length);
				current.indicies = array;
			}
			if (meshMan.meshRecord_ != null)
			{
				meshMan.meshRecord_.Add(current, current.indexCount, 6);
			}
			current.indicies[current.indexCount + 0] = current.head + 0;
			current.indicies[current.indexCount + 1] = current.head + 1;
			current.indicies[current.indexCount + 2] = current.head + 2;
			current.indicies[current.indexCount + 3] = current.head + 0;
			current.indicies[current.indexCount + 4] = current.head + 2;
			current.indicies[current.indexCount + 5] = current.head + 3;
			current.indexCount += 6;
		}

		public void AddQuad(RectangleF dst, float tu, float tv, float tw, float th)
		{
			AddQuad(dst, tu, tv, tw, th, Color.white);
		}

		public void AddIndex(int tris)
		{
			Group current = currentLayer.Current;
			if (current.indicies.Length < current.indexCount + 1)
			{
				int[] array = new int[current.indexCount + 1 + 6 + current.indexCount / 2];
				Array.Copy(current.indicies, 0, array, 0, current.indicies.Length);
				current.indicies = array;
			}
			if (meshMan.meshRecord_ != null)
			{
				meshMan.meshRecord_.Add(current, current.indexCount, 1);
			}
			current.indicies[current.indexCount] = current.head + tris;
			current.indexCount++;
		}

		public void AddIndicies(params int[] tris)
		{
			Group current = currentLayer.Current;
			if (current.indicies.Length < current.indexCount + tris.Length)
			{
				int[] array = new int[current.indexCount + tris.Length + 6 + current.indexCount / 2];
				Array.Copy(current.indicies, 0, array, 0, current.indicies.Length);
				current.indicies = array;
			}
			if (meshMan.meshRecord_ != null)
			{
				meshMan.meshRecord_.Add(current, current.indexCount, tris.Length);
			}
			int num = 0;
			while (num < tris.Length)
			{
				current.indicies[current.indexCount] = current.head + tris[num];
				num++;
				current.indexCount++;
			}
		}

		public void AddIndicies(int[] tris, int count)
		{
			Group current = currentLayer.Current;
			if (current.indicies.Length < current.indexCount + count)
			{
				int[] array = new int[current.indexCount + count + 6 + current.indexCount / 2];
				Array.Copy(current.indicies, 0, array, 0, current.indicies.Length);
				current.indicies = array;
			}
			if (meshMan.meshRecord_ != null)
			{
				meshMan.meshRecord_.Add(current, current.indexCount, count);
			}
			int num = 0;
			while (num < count)
			{
				current.indicies[current.indexCount] = current.head + tris[num];
				num++;
				current.indexCount++;
			}
		}

		public void AddTriangleIndicies()
		{
			Group current = currentLayer.Current;
			if (current.indicies.Length < current.indexCount + 3)
			{
				int[] array = new int[current.indexCount + 3 + 6 + current.indexCount / 2];
				Array.Copy(current.indicies, 0, array, 0, current.indicies.Length);
				current.indicies = array;
			}
			if (meshMan.meshRecord_ != null)
			{
				meshMan.meshRecord_.Add(current, current.indexCount, 3);
			}
			current.indicies[current.indexCount++] = current.head + 0;
			current.indicies[current.indexCount++] = current.head + 1;
			current.indicies[current.indexCount++] = current.head + 2;
		}

		public void AddQuadIndicies()
		{
			Group current = currentLayer.Current;
			if (current.indicies.Length < current.indexCount + 6)
			{
				int[] array = new int[current.indexCount + 6 + 6 + current.indexCount / 2];
				Array.Copy(current.indicies, 0, array, 0, current.indicies.Length);
				current.indicies = array;
			}
			if (meshMan.meshRecord_ != null)
			{
				meshMan.meshRecord_.Add(current, current.indexCount, 6);
			}
			current.indicies[current.indexCount++] = current.head + 0;
			current.indicies[current.indexCount++] = current.head + 1;
			current.indicies[current.indexCount++] = current.head + 2;
			current.indicies[current.indexCount++] = current.head + 0;
			current.indicies[current.indexCount++] = current.head + 2;
			current.indicies[current.indexCount++] = current.head + 3;
		}

		public Mesh ConvertMesh(int layer = 0)
		{
			foreach (KeyValuePair<int, Layer> layer2 in layers)
			{
				if (layer2.Key == layer)
				{
					return ConvertMesh(layer2.Value);
				}
			}
			return null;
		}

		internal Mesh ConvertMesh(Layer layer)
		{
			if (layer == null)
			{
				return null;
			}
			return ConvertMesh(layer.Current);
		}

		internal Mesh ConvertMesh(Group group)
		{
			if (group == null)
			{
				return null;
			}
			while (group.triangles.Count > group.indexCount)
			{
				group.triangles.RemoveAt(group.triangles.Count - 1);
			}
			for (int i = 0; i < group.indexCount; i++)
			{
				if (i < group.triangles.Count)
				{
					group.triangles[i] = group.indicies[i];
				}
				else
				{
					group.triangles.Add(group.indicies[i]);
				}
			}
			group.mesh.Clear();
			group.mesh.vertices = group.verticies;
			group.mesh.uv = group.texcoords;
			if (vertexColor)
			{
				group.mesh.colors = group.colors;
			}
			group.mesh.SetTriangles(group.triangles, 0);
			return group.mesh;
		}

		public Mesh GetMesh(int layer = 0)
		{
			foreach (KeyValuePair<int, Layer> layer2 in layers)
			{
				if (layer2.Key == layer)
				{
					return layer2.Value.Current.mesh;
				}
			}
			return null;
		}

		public int GetTriangleCount()
		{
			int num = 0;
			foreach (KeyValuePair<int, Layer> layer in layers)
			{
				foreach (Group group in layer.Value.groups)
				{
					num += group.indexCount;
				}
			}
			return num / 3;
		}

		public int GetVertexCount()
		{
			int num = 0;
			foreach (KeyValuePair<int, Layer> layer in layers)
			{
				foreach (Group group in layer.Value.groups)
				{
					num += group.vertexCount;
				}
			}
			return num;
		}

		public override bool Equals(object obj)
		{
			return this == obj;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public void Dispose()
		{
			foreach (Layer value in layers.Values)
			{
				foreach (Group group in value.groups)
				{
					if (group.mesh != null)
					{
						UnityEngine.Object.Destroy(group.mesh);
					}
					group.mesh = null;
					group.verticies = null;
					group.texcoords = null;
					group.colors = null;
					group.indicies = null;
					group.triangles = null;
				}
			}
		}

		public bool HasVertexColor()
		{
			return vertexColor;
		}

		private Layer GetLayer()
		{
			Layer value;
			if (!layers.TryGetValue(meshMan.CurrentDepthLayer, out value))
			{
				value = new Layer(this, meshMan.CurrentDepthLayer);
				layers[meshMan.CurrentDepthLayer] = value;
			}
			return value;
		}
	}
}
