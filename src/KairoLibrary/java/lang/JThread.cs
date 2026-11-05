using System.Collections.Generic;
using System.Threading;

namespace java.lang
{
	public class JThread
	{
		public const int MAX_PRIORITY = 1;

		public const int MIN_PRIORITY = 2;

		public const int NORM_PRIORITY = 0;

		private Thread thread_;

		private int priority_;

		private static List<JThread> threadBuffer_ = new List<JThread>();

		public static bool operator ==(JThread obj1, JThread obj2)
		{
			if ((object)obj1 == null || (object)obj2 == null)
			{
				return (object)obj1 == obj2;
			}
			return obj1.Equals(obj2);
		}

		public static bool operator !=(JThread obj1, JThread obj2)
		{
			return !(obj1 == obj2);
		}

		public JThread()
		{
		}

		public JThread(JRunnable target)
		{
			thread_ = new Thread(target.Run);
		}

		public JThread(ThreadStart start)
		{
			thread_ = new Thread(start);
		}

		public int GetPriority()
		{
			return priority_;
		}

		public void SetPriority(int newPriority)
		{
			priority_ = newPriority;
			if (thread_ != null)
			{
				if (priority_ == 0)
				{
					thread_.Priority = ThreadPriority.Normal;
				}
				else if (priority_ == 2)
				{
					thread_.Priority = ThreadPriority.Lowest;
				}
				else if (priority_ == 1)
				{
					thread_.Priority = ThreadPriority.Highest;
				}
			}
		}

		public void Start()
		{
			if (thread_ != null)
			{
				thread_.Start();
			}
		}

		public void Interrupt()
		{
			if (thread_ != null)
			{
				thread_.Interrupt();
			}
		}

		public static void Sleep(long millis)
		{
			Thread.Sleep((int)millis);
		}

		public static void Yield()
		{
			Thread.Sleep(0);
		}

		public static JThread CurrentThread()
		{
			for (int i = 0; i < threadBuffer_.Count; i++)
			{
				if (threadBuffer_[i].thread_ == Thread.CurrentThread)
				{
					return threadBuffer_[i];
				}
			}
			if (threadBuffer_.Count > 6)
			{
				for (int num = threadBuffer_.Count - 1; num >= 0; num--)
				{
					if (threadBuffer_[num].thread_.ThreadState != ThreadState.Running)
					{
						threadBuffer_.RemoveAt(num);
					}
				}
			}
			JThread jThread = new JThread();
			jThread.thread_ = Thread.CurrentThread;
			threadBuffer_.Add(jThread);
			return jThread;
		}

		public static bool IsCurrentThread(JThread thread)
		{
			if (thread != null)
			{
				return thread.thread_ == Thread.CurrentThread;
			}
			return false;
		}

		public override bool Equals(object obj)
		{
			if (obj is JThread)
			{
				return thread_ == ((JThread)obj).thread_;
			}
			if (obj is Thread)
			{
				return thread_ == (Thread)obj;
			}
			return this == obj;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}
}
