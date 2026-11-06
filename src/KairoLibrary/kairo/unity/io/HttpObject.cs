using System;
using System.IO;
using System.Text;
using java.io;
using java.lang;
using java.util;
using kairo.common.cfg;
using kairo.unity.native;
using kairo.unity.ui;
using kairo.unity.util;
using UnityEngine;

namespace kairo.unity.io
{
	public class HttpObject
	{
		protected WWW http_;

		protected string errorUrl_;

		protected int errorCode_;

		protected string errorText_;

		protected string errorTextOrg_;

		protected Exception error_;

		protected bool askRetry_;

		public HttpObject(bool retryEnable = false)
		{
			IsRetryEnable = retryEnable;
		}

		public byte[] Connect(string url, byte[] send, bool check)
		{
			return Connect(url, send, check, false)[0];
		}

		public byte[][] Connect(string url, byte[] send, bool check, bool split)
		{
			int num = 0;
			while (true)
			{
				try
				{
					return _connect(url, send, check, split);
				}
				catch (Exception ex)
				{
					if (!askRetry_)
					{
						goto IL_00ab;
					}
					string title = (Language.Japanese() ? "接続確認" : "Connection");
					string text = (Language.Japanese() ? "ネットワーク接続に失敗しました。\n電波が無いか、通信が許可されていない可能性があります。" : "Failure to network connection. \nOr radio waves do not, there is a possibility that the communication is not permitted.");
					string text2 = (Language.Japanese() ? "再接続,中止" : "Retry,Cancel");
					if (num > 0)
					{
						JThread.Sleep(500L);
					}
					Dialog dialog = new Dialog(3, title);
					dialog.SetText(text);
					dialog.SetButtons(text2.Split(','));
					if (dialog.Show() != 4)
					{
						goto IL_00ab;
					}
					num++;
					goto end_IL_0014;
					IL_00ab:
					throw ex;
					end_IL_0014:;
				}
			}
		}

		private byte[][] _connect(string url, byte[] send, bool check, bool split)
		{
			askRetry_ = false;
			if (url.IndexOf("get_score.php") != -1)
			{
				return new byte[1][] { "OK\n0,0".GetBytes() };
			}
			errorCode_ = 0;
			errorText_ = null;
			errorTextOrg_ = null;
			errorUrl_ = null;
			error_ = null;
			byte[][] recv = null;
			int totalSize = 0;
			int recvSize = 0;
			if (url.IndexOf("regist.php") != -1)
			{
				if (!Config.REGIST_ENABLE)
				{
					return new byte[1][] { "1\n100".GetBytes() };
				}
				switch (KairoPlugin.LicenseCheck())
				{
				case 0:
					return new byte[1][] { "1\n100".GetBytes() };
				case 4:
					errorCode_ = -98;
					errorText_ = Language.TranslateText("サーバーエラーが発生しました。");
					errorText_ += "\n";
					errorText_ += Language.TranslateText("認証に失敗しました。購入時のアカウントを設定し、電波状態を確認してください。");
					break;
				case 5:
				case 257:
					errorCode_ = -99;
					errorText_ = Language.TranslateText("通信を許可し電波状態を確認して下さい。");
					errorText_ += "\n";
					errorText_ += Language.TranslateText("認証に失敗しました。購入時のアカウントを設定し、電波状態を確認してください。");
					break;
				default:
					errorCode_ = -1;
					errorText_ = Language.TranslateText("認証に失敗しました。");
					errorText_ += "\n";
					errorText_ += Language.TranslateText("購入時のアカウントを設定し、電波状態を確認してください。");
					break;
				}
				if (errorText_ != null)
				{
					if (Language.Japanese())
					{
						errorText_ += "\thttp://kairopark.jp/android/faq.html#licence";
					}
					else
					{
						errorText_ += "\thttp://kairopark.jp/android/en/faq.html#licence";
					}
				}
				errorTextOrg_ = errorText_;
				throw new IOException();
			}
			while (url.IndexOf("NULLGWDOCOMO") != -1)
			{
			}
			if (url.Contains("?"))
			{
				url = url + "&" + GetTimeStampParam();
			}
			else
			{
				url = url + "?" + GetTimeStampParam();
			}
			if (Config.TRACE_URL && Config.PRINT)
			{
				Log.Info("[URL] " + url);
			}
			if (send != null && Config.POST_MARK)
			{
				ByteArrayOutputStream byteArrayOutputStream = new ByteArrayOutputStream();
				StreamUtil.Write(byteArrayOutputStream, send);
				StreamUtil.WriteInt(byteArrayOutputStream, 1234567890);
				send = byteArrayOutputStream.ToByteArray();
			}
			ByteArrayInputStream bais = null;
			try
			{
				int timeout = 60000;
				long begin = JSystem.CurrentTimeMillis();
				http_ = (WWW)IApplication.GetCurrentApp().RunOnUiThread(() => (send != null) ? (new WWW(url, send)) : (new WWW(url)), true);
				while (true)
				{
					bool completed = (bool)IApplication.GetCurrentApp().RunOnUiThread(delegate
				{
					if (http_.isDone)
					{
						return true;
					}
					if (JSystem.CurrentTimeMillis() - begin >= timeout)
					{
						http_.Dispose();
						http_ = null;
						return true;
					}
					return false;
				}, true);
					if (completed)
					{
						break;
					}
					JThread.Sleep(10L);
				}
				IApplication.GetCurrentApp().RunOnUiThread(delegate
				{
					if (http_ == null)
					{
						error_ = new IOException("timeout");
						askRetry_ = IsRetryEnable;
					}
					else if (!string.IsNullOrEmpty(http_.error))
					{
						error_ = new IOException(http_.error);
						askRetry_ = IsRetryEnable;
					}
					else
					{
						byte[] bytes = http_.bytes;
						totalSize = bytes.Length;
						bais = new ByteArrayInputStream(bytes);
						if (check)
						{
							if (2 <= totalSize)
							{
								sbyte num = StreamUtil.ReadByte(bais);
								sbyte b = StreamUtil.ReadByte(bais);
								recvSize += 2;
								if (3 <= totalSize)
								{
									StreamUtil.ReadByte(bais);
									recvSize++;
								}
								if (num != 79 || b != 75)
								{
									try
									{
										errorTextOrg_ = Encoding.UTF8.GetString(bytes);
									}
									catch (Exception)
									{
									}
									byte[] bytes2 = StreamUtil.Read(bais, totalSize - recvSize);
									string value = Encoding.UTF8.GetString(bytes2);
									string[] array = StringUtil.Split(value, "\n");
									try
									{
										if (array.Length != 0)
										{
											errorCode_ = JInteger.ParseInt(array[0]);
										}
										{ bool recoveredCondition_5948 = 1 < array.Length;
if (recoveredCondition_5948)
										{
											errorText_ = array[1].Replace('|', '\n');
										}
}
									}
									catch (Exception)
									{
										errorCode_ = -98;
										errorText_ = value;
										errorText_ = StringUtil.Replace(errorText_, "<", "(");
										errorText_ = StringUtil.Replace(errorText_, ">", ")");
										errorText_ = StringUtil.Replace(errorText_, "\n", " ");
									}
									if (Config.PRINT)
									{
										Log.Info("Http\t\t[error]" + errorCode_ + ":" + errorText_);
									}
									error_ = new IOException();
								}
							}
							else
							{
								error_ = new IOException("response " + totalSize + " bytes");
							}
						}
						if (error_ == null)
						{
							Vector vector = new Vector();
							byte[] array2;
							for (; recvSize < totalSize; recvSize += array2.Length)
							{
								int size = totalSize - recvSize;
								if (split)
								{
									size = StreamUtil.ReadInt(bais);
									recvSize += 4;
								}
								array2 = StreamUtil.Read(bais, size);
								vector.AddElement(array2);
							}
							if (vector.Size() == 0)
							{
								if (split)
								{
									recv = JTool.MakeArray<byte[][]>(new int[2]);
								}
								else
								{
									recv = JTool.MakeArray<byte[][]>(new int[2] { 1, 0 });
								}
							}
							else
							{
								recv = new byte[vector.Size()][];
							}
							for (int i = 0; i < vector.Size(); i++)
							{
								recv[i] = (byte[])vector.ElementAt(i);
							}
						}
					}
				}, true);
				if (error_ != null)
				{
					throw error_;
				}
			}
			catch (Exception ex)
			{
				errorUrl_ = url;
				if (errorCode_ == 0)
				{
					errorCode_ = -98;
				}
				throw ex;
			}
			finally
			{
				if (bais != null)
				{
					try
					{
						bais.Close();
					}
					catch (Exception)
					{
					}
				}
				bais = null;
				if (http_ != null)
				{
					IApplication.GetCurrentApp().RunOnUiThread(delegate
					{
						try
						{
							http_.Dispose();
						}
						catch (Exception)
						{
						}
					}, true);
				}
				http_ = null;
			}
			return recv;
		}

		public void Interrupt()
		{
			try
			{
				http_.Dispose();
			}
			catch (Exception)
			{
			}
			finally
			{
				http_ = null;
				errorCode_ = -98;
				errorUrl_ = null;
			}
		}

		public bool IsConnect()
		{
			return http_ != null;
		}

		public int GetErrorCode()
		{
			return errorCode_;
		}

		public string GetErrorText()
		{
			if (errorText_ == null)
			{
				if (!Language.Japanese())
				{
					return "Make sure to allow connection to network and<br>check your signal quality.";
				}
				return Language.TranslateText("通信を許可し電波状況<br>を確認して下さい");
			}
			return errorText_;
		}

		public string GetErrorText2()
		{
			return errorTextOrg_;
		}

		public string GetErrorUrl()
		{
			return errorUrl_;
		}

		public int PopErrorCode()
		{
			int errorCode = GetErrorCode();
			errorCode_ = 0;
			return errorCode;
		}

		public string PopErrorText()
		{
			string errorText = GetErrorText();
			errorText_ = null;
			return errorText;
		}

		public string PopErrorUrl()
		{
			string errorUrl = GetErrorUrl();
			errorUrl_ = null;
			return errorUrl;
		}

		private string GetTimeStampParam()
		{
			JCalendar instance = JCalendar.GetInstance();
			instance.SetTimeInMillis(ClockTime.GetTime());
			string text = string.Concat(instance.Get(1));
			string text2 = string.Concat(instance.Get(2) + 1);
			string text3 = string.Concat(instance.Get(5));
			string text4 = string.Concat(instance.Get(11));
			string text5 = string.Concat(instance.Get(12));
			string text6 = string.Concat(instance.Get(13));
			if (text2.Length == 1)
			{
				text2 = "0" + text2;
			}
			if (text3.Length == 1)
			{
				text3 = "0" + text3;
			}
			if (text4.Length == 1)
			{
				text4 = "0" + text4;
			}
			if (text5.Length == 1)
			{
				text5 = "0" + text5;
			}
			if (text6.Length == 1)
			{
				text6 = "0" + text6;
			}
			return "_ts=" + text + text2 + text3 + text4 + text5 + text6;
		}

		internal bool IsRetryEnable { get; set; }
	}
}
