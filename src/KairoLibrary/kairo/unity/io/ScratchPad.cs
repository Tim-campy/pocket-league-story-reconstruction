using java.io;

namespace kairo.unity.io
{
	public class ScratchPad
	{
		protected ScratchPad()
		{
		}

		public static DataInputStream OpenDataInputStream(int spid, int pos, int length)
		{
			return null;
		}

		public static DataOutputStream OpenDataOutputStream(int spid, int pos, int length)
		{
			return null;
		}

		public static object Open(int spid, int pos, int length, int mode)
		{
			return null;
		}

		public static byte[] ReadByteArray(int spid, int pos, int length)
		{
			return null;
		}

		public static void WriteByteArray(int spid, int pos, byte[] data)
		{
		}

		public static int ReadInt(int spid, int pos)
		{
			return 0;
		}

		public static void WriteInt(int spid, int pos, int data)
		{
		}

		public static long ReadLong(int spid, int pos)
		{
			return 0L;
		}

		public static void WriteLong(int spid, int pos, long data)
		{
		}

		public static int GetSPSize(int spid)
		{
			return 0;
		}
	}
}
