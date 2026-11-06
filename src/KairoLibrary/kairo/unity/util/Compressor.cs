using System;
using java.io;
using java.lang;
using kairo.common.cfg;

namespace kairo.unity.util
{
	public class Compressor
	{
		private class Node
		{
			public Node parent_;

			public Node left_;

			public Node right_;

			public int weight_;

			public int value_;

			public Node()
			{
			}

			public Node(Node parent, Node left, Node right, int weight, int value)
			{
				parent_ = parent;
				left_ = left;
				right_ = right;
				weight_ = weight;
				value_ = value;
			}
		}

		private static int[] weights_ = new int[256];

		private static Node[] nodeTbl_ = new Node[256];

		private static byte[] bits_ = new byte[256];

		private static byte[] nullbits_ = new byte[256];

		private static int[] _getLowestNode_result = new int[2];

		private static int bitPoint_;

		private static byte[] bitBuffer_;

		private Compressor()
		{
		}

		public static byte[] Compress(byte[] data)
		{
			if (data == null || data.Length == 0)
			{
				return data;
			}
			long num = JSystem.CurrentTimeMillis();
			global::kairo.unity.util.FastVector fastVector;
global::kairo.unity.util.Compressor.Node node;
for (int i = 0; i < weights_.Length; i++)
			{
				weights_[i] = 0;
			}
			for (int j = 0; j < nodeTbl_.Length; j++)
			{
				nodeTbl_[j] = null;
			}
			JSystem.Arraycopy(nullbits_, 0, bits_, 0, bits_.Length);
			for (int k = 0; k < data.Length; k++)
			{
				weights_[data[k] & 0xFF]++;
			}
			fastVector = new FastVector();
			for (int l = 0; l < weights_.Length; l++)
			{
				if (weights_[l] > 0)
				{
					node = new Node(null, null, null, weights_[l], l);
					fastVector.AddElement(node);
					nodeTbl_[l] = node;
				}
			}
			if (fastVector.elementCount == 1)
			{
				Node node2 = (Node)fastVector.elementData[0];
				Node node3 = (node2.parent_ = new Node(null, node2, node2, node2.weight_ + node2.weight_, 0));
				fastVector.elementData[0] = node3;
			}
			else
			{
				while (fastVector.elementCount >= 2)
				{
					int[] array = _getLowestNode(fastVector);
					Node node4 = (Node)fastVector.elementData[array[1]];
					Node node5 = (Node)fastVector.elementData[array[0]];
					fastVector.AddElement(node5.parent_ = (node4.parent_ = new Node(null, node4, node5, node4.weight_ + node5.weight_, 0)));
					fastVector.RemoveElement(node4);
					fastVector.RemoveElement(node5);
				}
			}
			if (Config.PRINT)
			{
				Log.Info("符号化時間=" + (JSystem.CurrentTimeMillis() - num));
			}
			num = JSystem.CurrentTimeMillis();
			_setBitBuffer(new byte[1024]);
			_writeNodeToBitBuffer((Node)fastVector.elementData[0]);
			int num2 = 0;
			for (int m = 0; m < data.Length; m++)
			{
				int num3 = data[m] & 0xFF;
				Node node6 = nodeTbl_[num3];
				while (node6.parent_ != null)
				{
					Node parent_ = node6.parent_;
					if (parent_.left_ == node6)
					{
						bits_[num2++] = 0;
					}
					else if (parent_.right_ == node6)
					{
						bits_[num2++] = 1;
					}
					node6 = node6.parent_;
					parent_ = node6.parent_;
				}
				int length = num2;
				for (num2--; num2 >= 0; num2--)
				{
					_writeBit(bits_[num2]);
				}
				num2 = 0;
				JSystem.Arraycopy(nullbits_, 0, bits_, 0, length);
			}
			if (Config.PRINT)
			{
				Log.Info("圧縮時間=" + (JSystem.CurrentTimeMillis() - num));
			}
			byte[] array2 = null;
			ByteArrayOutputStream byteArrayOutputStream = null;
			try
			{
				byteArrayOutputStream = new ByteArrayOutputStream();
				byte[] data2 = _getBitBuffer();
				StreamUtil.WriteInt(byteArrayOutputStream, bitPoint_);
				StreamUtil.Write(byteArrayOutputStream, data2);
				return byteArrayOutputStream.ToByteArray();
			}
			finally
			{
				byteArrayOutputStream?.Close();
				byteArrayOutputStream = null;
			}
		}

		public static byte[] Expand(byte[] data)
		{
			if (data == null || data.Length == 0)
			{
				return data;
			}
			long num = JSystem.CurrentTimeMillis();
			_setBitBuffer(data);
			int num2 = EncodeUtil.ByteAryToInt(data, true) + 32;
			bitPoint_ += 32;
			Node node = _readNodeFromBitBuffer();
			ByteArrayOutputStream byteArrayOutputStream = null;
			try
			{
				byteArrayOutputStream = new ByteArrayOutputStream();
				while (bitPoint_ < num2)
				{
					Node node2 = node;
					while (node2.left_ != null)
					{
						node2 = ((_readBit() != 0) ? node2.right_ : node2.left_);
					}
					byteArrayOutputStream.Write((byte)node2.value_);
				}
				if (Config.PRINT)
				{
					Log.Info("解凍時間=" + (JSystem.CurrentTimeMillis() - num));
				}
				return byteArrayOutputStream.ToByteArray();
			}
			finally
			{
				if (byteArrayOutputStream != null)
				{
					try
					{
						byteArrayOutputStream.Close();
					}
					catch (Exception)
					{
					}
				}
				byteArrayOutputStream = null;
			}
		}

		private static void _writeNodeToBitBuffer(Node node)
		{
			if (node.left_ == null && node.right_ == null)
			{
				_writeBit(1);
				_writeByte(node.value_);
				return;
			}
			if (node.left_ != null)
			{
				_writeBit(0);
				_writeNodeToBitBuffer(node.left_);
			}
			if (node.right_ != null)
			{
				_writeBit(0);
				_writeNodeToBitBuffer(node.right_);
			}
		}

		private static Node _readNodeFromBitBuffer()
		{
			Node node = new Node();
			int num = 0;
			num = _readBit();
			if (num == 1)
			{
				node.value_ = _readByte();
			}
			else
			{
				if (num == 0)
				{
					node.left_ = _readNodeFromBitBuffer();
					node.left_.parent_ = node;
				}
				if (_readBit() == 0)
				{
					node.right_ = _readNodeFromBitBuffer();
					node.right_.parent_ = node;
				}
			}
			return node;
		}

		private static int[] _getLowestNode(FastVector list)
		{
			int num = -1;
			int num2 = -1;
			for (int i = 0; i < list.elementCount; i++)
			{
				if (num == -1 || ((Node)list.elementData[num]).weight_ >= ((Node)list.elementData[i]).weight_)
				{
					num = i;
				}
			}
			for (int j = 0; j < list.elementCount; j++)
			{
				if (j != num && (num2 == -1 || ((Node)list.elementData[num2]).weight_ >= ((Node)list.elementData[j]).weight_))
				{
					num2 = j;
				}
			}
			_getLowestNode_result[0] = num;
			_getLowestNode_result[1] = num2;
			return _getLowestNode_result;
		}

		private static void _setBitBuffer(byte[] data)
		{
			bitBuffer_ = data;
			bitPoint_ = 0;
		}

		private static byte[] _getBitBuffer()
		{
			byte[] array = new byte[(bitPoint_ + 8 - 1) / 8];
			JSystem.Arraycopy(bitBuffer_, 0, array, 0, array.Length);
			return array;
		}

		private static void _writeBit(int bit)
		{
			if (bitBuffer_.Length * 8 <= bitPoint_)
			{
				byte[] dst = new byte[bitBuffer_.Length * 2];
				JSystem.Arraycopy(bitBuffer_, 0, dst, 0, bitBuffer_.Length);
				bitBuffer_ = dst;
			}
			bitBuffer_[bitPoint_ / 8] = (byte)((bitBuffer_[bitPoint_ / 8] & 0xFF) | (bit << bitPoint_ % 8));
			bitPoint_++;
		}

		private static void _writeByte(int v)
		{
			for (int i = 0; i < 8; i++)
			{
				_writeBit((v & (1 << i)) >> i);
			}
		}

		private static int _readBit()
		{
			int result = (bitBuffer_[bitPoint_ / 8] & 0xFF & (1 << bitPoint_ % 8)) >> bitPoint_ % 8;
			bitPoint_++;
			return result;
		}

		private static int _readByte()
		{
			int num = 0;
			for (int i = 0; i < 8; i++)
			{
				int num2 = _readBit();
				num |= num2 << i;
			}
			return num;
		}
	}
}
