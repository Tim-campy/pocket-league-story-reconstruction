using System.Collections.Generic;

namespace kairo.unity.graphics
{
	public class MeshRecord
	{
		internal List<MeshBuilder.Group> Groups = new List<MeshBuilder.Group>();

		internal List<int[]> Indicies = new List<int[]>();

		internal void Add(MeshBuilder.Group group, int index, int length)
		{
			Groups.Add(group);
			Indicies.Add(new int[2] { index, length });
		}

		internal void Dispose()
		{
			Groups.Clear();
			Groups = null;
			Indicies.Clear();
			Indicies = null;
		}

		public bool Delete()
		{
			if (Indicies == null || Groups == null)
			{
				return false;
			}
			for (int num = Groups.Count - 1; num >= 0; num--)
			{
				for (int i = 0; i < Indicies[num][1]; i++)
				{
					Groups[num].RemoveIndex(Indicies[num][0] + i);
				}
			}
			return true;
		}

		public void Combine(MeshRecord record)
		{
			for (int i = 0; i < record.Groups.Count; i++)
			{
				Groups.Add(record.Groups[i]);
				Indicies.Add(record.Indicies[i]);
			}
		}

		public static MeshRecord Combine(MeshRecord record1, MeshRecord record2)
		{
			if (record1 == null && record2 != null)
			{
				return record2;
			}
			if (record1 != null && record2 == null)
			{
				return record1;
			}
			record1.Combine(record2);
			return record1;
		}
	}
}
