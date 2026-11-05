using System.Collections.Generic;
using java.lang;
using kairo.common.cfg;
using kairo.unity.native;
using kairo.unity.ui;

namespace kairo.unity.io
{
	public class Http
	{
		private class HttpThread
		{
			public JThread thread_;

			public HttpObject http_;

			public bool connect_;
		}

		public const int HTTP_OK = 0;

		public const int HTTP_NOMYMENU = -1;

		public const int HTTP_NG = -98;

		public const int HTTP_PERMISSION = -99;

		private static HttpObject http_ = new HttpObject();

		private static object lock_ = new object();

		private static List<HttpThread> httpThreads_ = new List<HttpThread>();

		internal static bool IsRetryEnable { get; set; }

		protected Http()
		{
		}

		public static byte[] Connect(string url, byte[] send, bool check)
		{
			return Connect(url, send, check, false)[0];
		}

		public static byte[][] Connect(string url, byte[] send, bool check, bool split)
		{
			HttpThread httpThread = MakeHttp();
			try
			{
				if (Config.DEBUG && IApplication.GetCurrentApp().CheckUiThread())
				{
					KairoPlugin.ShowToast("UIスレッドで通信が行われました。\niOS版ではフリーズします。\n" + url);
				}
				return httpThread.http_.Connect(url, send, check, split);
			}
			finally
			{
				lock (lock_)
				{
					httpThreads_.Remove(httpThread);
					httpThreads_.Insert(0, httpThread);
				}
				httpThread.connect_ = false;
			}
		}

		public static void Interrupt()
		{
			lock (lock_)
			{
				foreach (HttpThread item in httpThreads_)
				{
					try
					{
						if (item.connect_)
						{
							item.http_.Interrupt();
						}
					}
					catch
					{
					}
				}
			}
		}

		public static int GetErrorCode()
		{
			HttpThread http = GetHttp();
			if (http == null)
			{
				return 0;
			}
			return http.http_.GetErrorCode();
		}

		public static string GetErrorText()
		{
			HttpThread http = GetHttp();
			if (http == null)
			{
				return "";
			}
			return http.http_.GetErrorText();
		}

		public static string GetErrorText2()
		{
			HttpThread http = GetHttp();
			if (http == null)
			{
				return "";
			}
			return http.http_.GetErrorText2();
		}

		public static string GetErrorUrl()
		{
			HttpThread http = GetHttp();
			if (http == null)
			{
				return "";
			}
			return http.http_.GetErrorUrl();
		}

		public static int PopErrorCode()
		{
			HttpThread http = GetHttp();
			if (http == null)
			{
				return 0;
			}
			return http.http_.PopErrorCode();
		}

		public static string PopErrorText()
		{
			HttpThread http = GetHttp();
			if (http == null)
			{
				return "";
			}
			return http.http_.PopErrorText();
		}

		public static string PopErrorUrl()
		{
			HttpThread http = GetHttp();
			if (http == null)
			{
				return "";
			}
			return http.http_.PopErrorUrl();
		}

		private static HttpThread MakeHttp()
		{
			JThread jThread = JThread.CurrentThread();
			lock (lock_)
			{
				HttpThread httpThread = null;
				for (int i = 0; i < httpThreads_.Count; i++)
				{
					HttpThread httpThread2 = httpThreads_[i];
					if (!(httpThread2.thread_ != jThread))
					{
						if (i != 0)
						{
							httpThreads_.RemoveAt(i);
							httpThreads_.Insert(0, httpThread2);
						}
						httpThread = httpThread2;
						break;
					}
				}
				if (httpThread == null)
				{
					httpThread = new HttpThread();
					httpThread.http_ = new HttpObject(IsRetryEnable);
					httpThread.http_.IsRetryEnable = IsRetryEnable;
					httpThread.thread_ = jThread;
					httpThreads_.Insert(0, httpThread);
					if (httpThreads_.Count > 20)
					{
						for (int num = httpThreads_.Count - 1; num >= 0; num--)
						{
							if (!httpThreads_[num].connect_)
							{
								httpThreads_.RemoveAt(num);
								break;
							}
						}
					}
				}
				httpThread.http_ = new HttpObject(IsRetryEnable);
				httpThread.connect_ = true;
				return httpThread;
			}
		}

		private static HttpThread GetHttp()
		{
			if (httpThreads_.Count <= 0)
			{
				return null;
			}
			JThread jThread = JThread.CurrentThread();
			lock (lock_)
			{
				for (int i = 0; i < httpThreads_.Count; i++)
				{
					HttpThread httpThread = httpThreads_[i];
					if (httpThread.thread_ == jThread)
					{
						return httpThread;
					}
				}
				for (int j = 0; j < httpThreads_.Count; j++)
				{
					HttpThread httpThread2 = httpThreads_[j];
					if (!httpThread2.connect_)
					{
						return httpThread2;
					}
				}
				return httpThreads_[0];
			}
		}
	}
}
