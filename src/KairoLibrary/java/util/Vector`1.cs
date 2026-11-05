using System.Collections;
using System.Collections.Generic;

namespace java.util
{
	public class Vector<T> : IEnumerable, IVector
	{
		protected List<object> list_;

		public List<object> List => list_;

		public static implicit operator Vector(Vector<T> list)
		{
			return new Vector
			{
				list_ = list.list_
			};
		}

		public static bool operator !=(Vector<T> a, Vector b)
		{
			return !(a == b);
		}

		public static bool operator ==(Vector<T> a, Vector b)
		{
			if ((object)a != null && (object)b != null)
			{
				return a.list_ == b.list_;
			}
			return (object)a == b;
		}

		public override bool Equals(object obj)
		{
			if (obj is IVector)
			{
				return list_ == ((IVector)obj).List;
			}
			return base.Equals(obj);
		}

		public Vector()
		{
			list_ = new List<object>();
		}

		public Vector(int initialCapacity)
		{
			list_ = new List<object>(initialCapacity);
		}

		public Vector(int initialCapacity, int capacityIncrement)
		{
			list_ = new List<object>(initialCapacity);
		}

		public void AddElement(T obj)
		{
			list_.Add(obj);
		}

		public void Add(T obj)
		{
			AddElement(obj);
		}

		public void Add(int index, T obj)
		{
			InsertElementAt(obj, index);
		}

		public int Capacity()
		{
			return list_.Capacity;
		}

		public bool Contains(T elem)
		{
			return list_.Contains(elem);
		}

		public void CopyInto(T[] anArray)
		{
			if (anArray != null)
			{
				int num = ((anArray.Length < list_.Count) ? anArray.Length : list_.Count);
				for (int i = 0; i < num; i++)
				{
					anArray[i] = (T)list_[i];
				}
			}
		}

		public T Get(int index)
		{
			return ElementAt(index);
		}

		public T ElementAt(int index)
		{
			return (T)list_[index];
		}

		public void EnsureCapacity(int minCapacity)
		{
			if (minCapacity > list_.Capacity)
			{
				list_.Capacity = minCapacity;
			}
		}

		public T FirstElement()
		{
			return (T)list_[0];
		}

		public int IndexOf(T elem)
		{
			return list_.IndexOf(elem);
		}

		public int IndexOf(T elem, int index)
		{
			return list_.IndexOf(elem, index);
		}

		public void InsertElementAt(T obj, int index)
		{
			list_.Insert(index, obj);
		}

		public bool IsEmpty()
		{
			return list_.Count == 0;
		}

		public T LastElement()
		{
			return (T)list_[list_.Count - 1];
		}

		public int LastIndexOf(T elem)
		{
			return list_.LastIndexOf(elem);
		}

		public int LastIndexOf(T elem, int index)
		{
			return list_.LastIndexOf(elem, index);
		}

		public void RemoveAllElements()
		{
			list_.Clear();
		}

		public void Clear()
		{
			RemoveAllElements();
		}

		public bool Remove(T obj)
		{
			return RemoveElement(obj);
		}

		public bool RemoveElement(T obj)
		{
			int num = list_.IndexOf(obj);
			if (num == -1)
			{
				return false;
			}
			list_.RemoveAt(num);
			return true;
		}

		public void Remove(int index)
		{
			RemoveElementAt(index);
		}

		public void RemoveElementAt(int index)
		{
			list_.RemoveAt(index);
		}

		public void Set(T obj, int index)
		{
			SetElementAt(obj, index);
		}

		public void SetElementAt(T obj, int index)
		{
			list_[index] = obj;
		}

		public void SetSize(int newSize)
		{
			while (list_.Count < newSize)
			{
				list_.Add(default(T));
			}
			while (list_.Count > newSize)
			{
				list_.RemoveAt(list_.Count - 1);
			}
		}

		public int Size()
		{
			return list_.Count;
		}

		public override string ToString()
		{
			return list_.ToString();
		}

		public void TrimToSize()
		{
			list_.Capacity = list_.Count;
		}

		public T[] ToArray()
		{
			T[] array = new T[list_.Count];
			for (int i = 0; i < list_.Count; i++)
			{
				array[i] = (T)list_[i];
			}
			return array;
		}

		public IEnumerator GetEnumerator()
		{
			foreach (T item in list_)
			{
				yield return item;
			}
		}
	}
}
