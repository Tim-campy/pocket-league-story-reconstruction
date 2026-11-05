using System;
using System.Collections;
using System.Collections.Generic;
using form;
using java.lang;
using kairo.common.cfg;
using kairo.unity.io;
using kairo.unity.ui;
using kairo.unity.util;
using surface;

namespace system.form
{
	public class HttpForm : MyFormBase, JRunnable
	{
		public class ResultData : IEnumerable
		{
			private Dictionary<string, object> data_ = new Dictionary<string, object>();

			public Exception Exception { get; set; }

			public string Error { get; set; }

			public bool Succeed { get; set; }

			IEnumerator IEnumerable.GetEnumerator()
			{
				return data_.GetEnumerator();
			}

			public void Add(string key, object data)
			{
				data_[key] = data;
			}

			public object Get(string key)
			{
				object value = null;
				if (data_.TryGetValue(key, out value))
				{
					return value;
				}
				return null;
			}

			public bool HasKey(string key)
			{
				return data_.ContainsKey(key);
			}
		}

		public delegate void HttpMethod(HttpForm form, ResultData data);

		public const int FORMID_HTTP = 909;

		private HttpMethod connectMethod_;

		private HttpMethod resultMethod_;

		private static int THSTATE_NONE;

		private static int THSTATE_RUN = 1;

		private static int THSTATE_SUCCESS = 2;

		private static int THSTATE_FAILURE = 3;

		private int threadState_;

		private JThread thread_;

		private ResultData resultData_;

		private Dictionary<string, object> commonData_ = new Dictionary<string, object>();

		public bool ProcessError { get; set; }

		public bool IsCancelable { get; set; }

		public HttpForm(HttpMethod connectMethod = null, HttpMethod resultMethod = null)
			: base(909)
		{
			connectMethod_ = connectMethod;
			resultMethod_ = resultMethod;
			ProcessError = true;
		}

		public override string ToString()
		{
			return "HttpForm";
		}

		public void SetConnectMethod(HttpMethod method)
		{
			connectMethod_ = method;
		}

		public void SetResultMethod(HttpMethod method)
		{
			resultMethod_ = method;
		}

		public override bool Init()
		{
			base.Init();
			threadState_ = THSTATE_RUN;
			resultData_ = new ResultData();
			thread_ = new JThread(this);
			thread_.Start();
			SetSoftLabel(string.Empty, string.Empty);
			return true;
		}

		public override void Draw(Graphics g)
		{
			base.Draw(g);
			if (threadState_ != THSTATE_NONE)
			{
				DrawConnect(g);
			}
		}

		public static void DrawConnect(Graphics g)
		{
			try
			{
				g.PushOrigin(0, 0);
				GameView instance = GameView.GetInstance();
				int num = instance.GetGameWidth() / 2;
				int num2 = instance.GetGameHeight() / 2;
				int totalFrames = IApplication.GetCurrentApp().GetTotalFrames();
				ResourceManager resource = IApplication.GetCurrentApp().GetResource();
				g.PushMatrix(Matrix.ScaleTemporary(0.5f, 0.5f));
				resource.DrawSeb(g, num, num2, 1);
				g.PushMatrix(Matrix.RotateTemporary(totalFrames / 2 * 30));
				resource.DrawSeb(g, num, num2, 2);
				g.PopMatrix();
				g.PopMatrix();
				g.PushFont(10);
				string text = "Transmission";
				int num3 = g.StringWidth(text);
				for (int i = 0; i < totalFrames / 4 % 4; i++)
				{
					text += ".";
				}
				g.SetColor(255, 255, 255);
				g.DrawString(text, num - num3 / 2 - 7, num2 - 42);
				g.PopFont();
				g.PopOrigin();
			}
			catch
			{
			}
		}

		public override bool Update()
		{
			base.Update();
			if (threadState_ != THSTATE_NONE)
			{
				if (threadState_ == THSTATE_RUN)
				{
					return true;
				}
				if (threadState_ == THSTATE_SUCCESS)
				{
					threadState_ = THSTATE_NONE;
					Pop();
					if (resultMethod_ != null)
					{
						resultMethod_(this, resultData_);
					}
					return true;
				}
				if (threadState_ == THSTATE_FAILURE)
				{
					threadState_ = THSTATE_NONE;
					if (Config.PRINT)
					{
						Log.Info(resultData_.Error);
					}
					Pop();
					if (ProcessError)
					{
						Push(SubForm.MakeDialog(resultData_.Error));
					}
					else if (resultMethod_ != null)
					{
						resultMethod_(this, resultData_);
					}
					return true;
				}
			}
			return true;
		}

		public override bool Finish()
		{
			base.Finish();
			return true;
		}

		public override bool CheckMainWindow()
		{
			return false;
		}

		public void Run()
		{
			try
			{
				if (connectMethod_ != null)
				{
					connectMethod_(this, resultData_);
				}
				resultData_.Succeed = true;
				threadState_ = THSTATE_SUCCESS;
			}
			catch (Exception ex)
			{
				Log.Info(string.Empty + ex);
				resultData_.Error = Http.PopErrorText();
				resultData_.Exception = ex;
				resultData_.Succeed = false;
				threadState_ = THSTATE_FAILURE;
			}
			finally
			{
				thread_ = null;
			}
		}
	}
}
