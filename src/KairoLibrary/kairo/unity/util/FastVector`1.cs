using System.Collections;
using java.lang;

namespace kairo.unity.util
{
	public class FastVector<T> : IEnumerable
	{
		public int capacityIncrement = 2;

		public int elementCount;

		public T[] elementData;

		public int bufSize_;

		public FastVector()
		{
		}

		public FastVector(int initialCapacity)
		{
			if (initialCapacity > 0)
			{
				elementData = new T[initialCapacity];
				bufSize_ = elementData.Length;
			}
		}

		public FastVector(int initialCapacity, int capacityIncrement)
		{
			if (initialCapacity > 0)
			{
				elementData = new T[initialCapacity];
				bufSize_ = elementData.Length;
			}
			this.capacityIncrement = capacityIncrement;
		}

		public void AddElement(T obj)
		{
			if (elementCount + 1 > bufSize_)
			{
				T[] dst = new T[bufSize_ + capacityIncrement];
				if (bufSize_ > 0)
				{
					JSystem.Arraycopy(elementData, 0, dst, 0, bufSize_);
				}
				elementData = dst;
				bufSize_ = elementData.Length;
			}
			elementData[elementCount] = obj;
			elementCount++;
		}

		public void Add(T obj)
		{
			AddElement(obj);
		}

		public int Capacity()
		{
			return bufSize_;
		}

		public void CopyInto(T[] anArray)
		{
			for (int i = 0; i < elementCount; i++)
			{
				anArray[i] = elementData[i];
			}
		}

		public T ElementAt(int index)
		{
			return elementData[index];
		}

		public void EnsureCapacity(int minCapacity)
		{
		}

		public T FirstElement()
		{
			return elementData[0];
		}

		public bool Contains(T elem)
		{
			for (int i = 0; i < elementCount; i++)
			{
				if ((object)elementData[i] == (object)elem || (elem != null && elem.Equals(elementData[i])))
				{
					return true;
				}
			}
			return false;
		}

		public int IndexOf(T elem)
		{
			for (int i = 0; i < elementCount; i++)
			{
				if ((object)elementData[i] == (object)elem || (elem != null && elem.Equals(elementData[i])))
				{
					return i;
				}
			}
			return -1;
		}

		public int IndexOf(T elem, int index)
		{
			for (int i = index; i < elementCount; i++)
			{
				if ((object)elementData[i] == (object)elem || (elem != null && elem.Equals(elementData[i])))
				{
					return i;
				}
			}
			return -1;
		}

		public bool ContainsFast(T elem)
		{
			for (int i = 0; i < elementCount; i++)
			{
				if ((object)elementData[i] == (object)elem)
				{
					return true;
				}
			}
			return false;
		}

		public int IndexOfFast(T elem)
		{
			for (int i = 0; i < elementCount; i++)
			{
				if ((object)elementData[i] == (object)elem)
				{
					return i;
				}
			}
			return -1;
		}

		public int IndexOfFast(T elem, int index)
		{
			for (int i = index; i < elementCount; i++)
			{
				if ((object)elementData[i] == (object)elem)
				{
					return i;
				}
			}
			return -1;
		}

		public int LastIndexOfFast(T elem)
		{
			for (int num = elementCount - 1; num >= 0; num--)
			{
				if ((object)elementData[num] == (object)elem)
				{
					return num;
				}
			}
			return -1;
		}

		public int LastIndexOfFast(T elem, int index)
		{
			for (int num = index; num >= 0; num--)
			{
				if ((object)elementData[num] == (object)elem)
				{
					return num;
				}
			}
			return -1;
		}

		public bool RemoveElementFast(T obj)
		{
			int num = IndexOfFast(obj);
			if (num == -1)
			{
				return false;
			}
			RemoveElementAt(num);
			return true;
		}

		public void InsertElementAt(T obj, int index)
		{
			if (elementCount + 1 > bufSize_)
			{
				T[] dst = new T[bufSize_ + capacityIncrement];
				if (bufSize_ > 0)
				{
					JSystem.Arraycopy(elementData, 0, dst, 0, bufSize_);
				}
				elementData = dst;
				bufSize_ = elementData.Length;
			}
			for (int num = elementCount - 1; num >= index; num--)
			{
				elementData[num + 1] = elementData[num];
			}
			elementData[index] = obj;
			elementCount++;
		}

		public bool IsEmpty()
		{
			return elementCount == 0;
		}

		public T LastElement()
		{
			return elementData[elementCount - 1];
		}

		public int LastIndexOf(T elem)
		{
			for (int num = elementCount - 1; num >= 0; num--)
			{
				if ((object)elementData[num] == (object)elem || (elem != null && elem.Equals(elementData[num])))
				{
					return num;
				}
			}
			return -1;
		}

		public int LastIndexOf(T elem, int index)
		{
			for (int num = index; num >= 0; num--)
			{
				if ((object)elementData[num] == (object)elem || (elem != null && elem.Equals(elementData[num])))
				{
					return num;
				}
			}
			return -1;
		}

		public bool RemoveElement(T obj)
		{
			int num = IndexOf(obj);
			if (num == -1)
			{
				return false;
			}
			RemoveElementAt(num);
			return true;
		}

		public bool Remove(T obj)
		{
			return RemoveElement(obj);
		}

		public void RemoveAllElements()
		{
			if (elementData != null)
			{
				for (int i = 0; i < elementData.Length; i++)
				{
					elementData[i] = default(T);
				}
				elementData = null;
			}
			elementCount = 0;
			bufSize_ = 0;
		}

		public void RemoveAll()
		{
			RemoveAllElements();
		}

		public void Clear()
		{
			RemoveAllElements();
		}

		public void RemoveAllElementsKeep()
		{
			elementCount = 0;
		}

		public void RemoveElementAt(int index)
		{
			elementData[index] = default(T);
			for (int i = index + 1; i < elementCount; i++)
			{
				elementData[i - 1] = elementData[i];
			}
			elementCount--;
		}

		public void Remove(int index)
		{
			RemoveElementAt(index);
		}

		public void SetElementAt(T obj, int index)
		{
			elementData[index] = obj;
		}

		public void SetSize(int newSize)
		{
			if (bufSize_ < newSize)
			{
				T[] dst = new T[newSize];
				if (elementCount > 0)
				{
					JSystem.Arraycopy(elementData, 0, dst, 0, elementCount);
				}
				elementData = dst;
				bufSize_ = elementData.Length;
			}
			else if (elementCount > newSize)
			{
				T[] dst2 = new T[newSize];
				if (newSize > 0)
				{
					JSystem.Arraycopy(elementData, 0, dst2, 0, newSize);
				}
				elementData = dst2;
				bufSize_ = elementData.Length;
			}
			elementCount = newSize;
		}

		public int Size()
		{
			return elementCount;
		}

		public void TrimToSize()
		{
			if (elementCount == bufSize_) { return; }
if (elementCount == 0)
				{
					elementData = null;
				}
				else
				{
					T[] array = new T[elementCount];
					JSystem.Arraycopy(elementData, 0, array, 0, array.Length);
					elementData = array;
				}
bufSize_ = elementCount;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public override string ToString()
		{
			return base.ToString() + ":size=" + bufSize_;
		}

		public IEnumerator GetEnumerator()
		{
			foreach (T val in elementData)
			{
				yield return val;
			}
		}
	}
}
