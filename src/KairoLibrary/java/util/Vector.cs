namespace java.util
{
	public class Vector : Vector<object>
	{
		public Vector()
		{
		}

		public Vector(int initialCapacity)
			: base(initialCapacity)
		{
		}

		public Vector(int initialCapacity, int capacityIncrement)
			: base(initialCapacity, capacityIncrement)
		{
		}

		public override bool Equals(object obj)
		{
			if (obj is IVector)
			{
				return list_ == ((IVector)obj).List;
			}
			return base.Equals(obj);
		}
	}
}
