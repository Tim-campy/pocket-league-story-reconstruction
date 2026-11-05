using System;

namespace java.util
{
	public static class JTool
	{
		public static T MakeArray<T>(params int[] counts)
		{
			if (counts.Length == 0)
			{
				return default(T);
			}
			Type typeFromHandle = typeof(T);
			typeFromHandle = typeFromHandle.Assembly.GetType(typeFromHandle.FullName.Substring(0, typeFromHandle.FullName.Length - 2));
			return (T)(object)MakeArrayRecursive(typeFromHandle, counts, 0);
		}

		private static Array MakeArrayRecursive(Type type, int[] counts, int index)
		{
			Array array = Array.CreateInstance(type, counts[index]);
			if (index + 1 >= counts.Length)
			{
				return array;
			}
			type = type.Assembly.GetType(type.FullName.Substring(0, type.FullName.Length - 2));
			if (type == null)
			{
				return array;
			}
			for (int i = 0; i < array.Length; i++)
			{
				array.SetValue(MakeArrayRecursive(type, counts, index + 1), i);
			}
			return array;
		}
	}
}
