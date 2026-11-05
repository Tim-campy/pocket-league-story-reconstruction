using System.Collections.Generic;
using System.Threading;
using java.lang;

namespace kairo.unity.ui
{
	public class Handler
	{
		private class PostData
		{
			public ThreadStart start;

			public long uptimeMillis;

			public long delayMillis;

			public long beginTime;

			public PostData(ThreadStart start, object token = null)
			{
				this.start = start;
				uptimeMillis = long.MaxValue;
				delayMillis = long.MaxValue;
				beginTime = JSystem.CurrentTimeMillis();
			}
		}

		private List<PostData> postQueue_ = new List<PostData>();

		private Thread thread_;

		public bool Post(ThreadStart start)
		{
			IApplication.GetCurrentApp().RunOnUiThread(delegate
			{
				start();
			});
			return true;
		}

		public bool PostAtTime(ThreadStart start, long uptimeMillis)
		{
			PostData postData = new PostData(start);
			postData.uptimeMillis = uptimeMillis;
			postQueue_.Add(postData);
			if (thread_ == null)
			{
				thread_ = new Thread(Run);
				thread_.Start();
			}
			return true;
		}

		public bool PostDelayed(ThreadStart start, long delayMillis)
		{
			PostData postData = new PostData(start);
			postData.delayMillis = delayMillis;
			postQueue_.Add(postData);
			if (thread_ == null)
			{
				thread_ = new Thread(Run);
				thread_.Start();
			}
			return true;
		}

		private void Run()
		{
			try
			{
				do
				{
					long num = 1000L;
					for (int i = 0; i < postQueue_.Count; i++)
					{
						PostData postData = postQueue_[i];
						bool flag = false;
						long num2 = 0L;
						if (postData.uptimeMillis != long.MaxValue)
						{
							if (postData.uptimeMillis <= JSystem.CurrentTimeMillis())
							{
								flag = true;
							}
							else
							{
								num2 = JSystem.CurrentTimeMillis() - postData.uptimeMillis;
							}
						}
						if (postData.delayMillis != long.MaxValue)
						{
							if (postData.delayMillis <= JSystem.CurrentTimeMillis() - postData.beginTime)
							{
								flag = true;
							}
							else
							{
								num2 = postData.delayMillis - (JSystem.CurrentTimeMillis() - postData.beginTime);
							}
						}
						if (flag)
						{
							postQueue_.RemoveAt(i);
							Post(postData.start);
							i--;
						}
						else if (num2 < num)
						{
							num = num2;
						}
					}
					if (num > 0)
					{
						JThread.Sleep(num);
					}
				}
				while (postQueue_.Count > 0);
			}
			finally
			{
				thread_ = null;
			}
		}
	}
}
