using System;
using java.lang;

namespace java.util
{
	public class JRandom
	{
		protected Random random_;

		public JRandom()
			: this(JSystem.CurrentTimeMillis())
		{
		}

		public JRandom(long seed)
		{
			random_ = new Random((int)seed);
		}

		public int NextInt()
		{
			return random_.Next();
		}

		public int NextInt(int n)
		{
			return random_.Next() % n;
		}

		public long NextLong()
		{
			return random_.Next();
		}

		public void SetSeed(long seed)
		{
			random_ = new Random((int)seed);
		}
	}
}
