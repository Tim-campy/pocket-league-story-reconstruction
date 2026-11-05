using System.Runtime.CompilerServices;
using kairo.unity.form;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using java.io;
using java.lang;
using java.net;
using java.util;
using kairo.common.cfg;
using kairo.unity.analytics;
using kairo.unity.io;
using kairo.unity.native;
using kairo.unity.system.appli;
using kairo.unity.system.notification.ui;
using kairo.unity.system.payment;
using kairo.unity.system.tapjoy.ui;
using kairo.unity.ui;
using kairo.unity.util;
using UnityEngine;

namespace kairo.unity.system
{
	public abstract class KairoBase
	{

	protected KairoBase()
	{
	}
		protected class AccessCount
		{
			public string name_;

			public long value_;

			public AccessCount()
			{
			}

			public AccessCount(string name, long value)
			{
				name_ = name;
				value_ = value;
			}

			public override string ToString()
			{
				return name_ + ((value_ >= 0) ? "+" : "") + value_;
			}

			public override int GetHashCode()
			{
				return base.GetHashCode();
			}
		}

		protected class ConnectTask
		{
			public SecureString url_;

			public byte[] send_;

			public long time_;

			public int count_;

			public int dayMax_;

			public int countMax_;

			public void Serialize(OutputStream os)
			{
				StreamUtil.WriteString(os, SecureString.ToString(url_));
				StreamUtil.WriteBoolean(os, send_ != null);
				if (send_ != null)
				{
					StreamUtil.Write(os, send_, true);
				}
				StreamUtil.WriteLong(os, time_);
				StreamUtil.WriteByte(os, (byte)count_);
				StreamUtil.WriteByte(os, (byte)dayMax_);
				StreamUtil.WriteByte(os, (byte)countMax_);
			}

			public void Deserialize(InputStream stream)
			{
				url_ = SecureString.ValueOf(StreamUtil.ReadString(stream));
				if (StreamUtil.ReadBoolean(stream))
				{
					send_ = StreamUtil.Read(stream, true);
				}
				time_ = StreamUtil.ReadLong(stream);
				count_ = StreamUtil.ReadByte(stream);
				dayMax_ = StreamUtil.ReadByte(stream);
				countMax_ = StreamUtil.ReadByte(stream);
			}
		}

		private class KairoDummy : KairoBase
		{
			public override void Save()
			{
			}

			public override void SaveAll()
			{
			}

			public override void OnSaveGameBegin()
			{
			}

			public override void OnSaveGameEnd()
			{
			}
		}

		public class ConnectObject : HttpObject, IDisposable
		{
			internal KairoBase base_;

			internal ConnectObject(KairoBase kb)
			{
				base_ = kb;
				base_.connects_.Push(this);
			}

			public void Dispose()
			{
				if (base_.connects_.Count > 0)
				{
					base_.connects_.Pop();
				}
			}
		}

		protected static KairoBase instance_;

		public int reserveSave;

		public bool reserveSaveEnable = true;

		protected SecureString host_;

		protected SecureString pubHost_;

		protected int appliVersion_;

		protected int latestVersion_;

		protected SecureString userId_ = new SecureString("");

		protected SecureString friendId_ = new SecureString("");

		protected int pkgId_ = -1;

		protected int networkLevel_;

		protected bool rootBan_;

		protected bool login_;

		protected int startCount_;

		protected SecureInt flag_ = new SecureInt();

		protected bool init_;

		protected bool start_;

		protected Vector accessCounter_;

		protected Vector userCounter_;

		protected long cheat_;

		protected int accountType_;

		protected bool illegalPackage_;

		protected bool illegalCountry_;

		protected string[] arguments_ = new string[0];

		protected int[] argumentsInt_ = new int[0];

		protected string gcmId_;

		protected bool isPlaying_;

		protected Dictionary<string, int> cgiLogLevel_ = new Dictionary<string, int>();

		public const int NETLEVEL_LOW = 0;

		public const int NETLEVEL_MIDDLE = 1;

		public const int NETLEVEL_HIGH = 2;

		public const int ACCESSCOUNT_COMMON = 0;

		public const int ACCESSCOUNT_USER = 1;

		protected Vector connectTaskList_ = new Vector();

		public const int MIGRATION_NONE = -1;

		public const int MIGRATION_START = 0;

		public const int MIGRATION_COMPLETE = 1;

		private const string APPLIST_FILE = "app/list.bin";

		private const string APPLIST_RESOURCE = "applist.txt";

		private GameAppli[] appList_ = new GameAppli[0];

		private int appListDate_;

		private string COMMON_SERVER;

		private const string COM_PUB = "https://appli.kairogame.jp/";

		private const string COM_DEV = "https://applidev.kairogame.jp/";

		private static KairoDummy dummy_;

		private int[] prgStart_ = new int[2];

		private Stack<ConnectObject> connects_ = new Stack<ConnectObject>();

		public static KairoBase GetInstance()
		{
			if (instance_ == null)
			{
				if (dummy_ == null)
				{
					dummy_ = new KairoDummy();
					dummy_.LoadAppList();
				}
				return dummy_;
			}
			return instance_;
		}

		public void Init(int ver, string devHost, string pubHost, byte[] data, int networkLevel, bool rootBan)
		{
			if (init_)
			{
				return;
			}
			pubHost_ = new SecureString(pubHost);
			Finish();
			appliVersion_ = ver;
			accountType_ = -1;
			networkLevel_ = networkLevel;
			rootBan_ = rootBan;
			accessCounter_ = new Vector();
			userCounter_ = new Vector();
			prgStart_[0] = (prgStart_[1] = 0);
			OnInit();
			LoadAppList();
			Load(data);
			string text = (Config.DEBUG ? devHost : pubHost);
			if (accountType_ == 0)
			{
				text = devHost;
			}
			else if (accountType_ == 1)
			{
				text = pubHost;
			}
			COMMON_SERVER = (Config.DEBUG ? "https://applidev.kairogame.jp/" : "https://appli.kairogame.jp/");
			if (accountType_ == 0)
			{
				COMMON_SERVER = "https://applidev.kairogame.jp/";
			}
			else if (accountType_ == 1)
			{
				COMMON_SERVER = "https://appli.kairogame.jp/";
			}
			bool flag = false;
			if ((accountType_ == 0 && !Config.DEBUG) || (accountType_ == 1 && Config.DEBUG))
			{
				if (accountType_ == 0)
				{
					Dialog dialog = new Dialog(0, "ﾒｯｾｰｼﾞ");
					dialog.SetText("開発サーバーのユーザーIDです。\n開発サーバーへ接続します。");
					dialog.Show();
				}
				else
				{
					while (true)
					{
						Dialog dialog2 = new Dialog(4, "ﾒｯｾｰｼﾞ");
						dialog2.SetText("本番サーバーのユーザーIDです。\n本番サーバーへ接続しますか？");
						int num = dialog2.Show();
						if (num == 4)
						{
							Dialog dialog3 = new Dialog(3, "ﾒｯｾｰｼﾞ");
							dialog3.SetText("本番サーバーへ接続します。");
							if (dialog3.Show() == 4)
							{
								break;
							}
						}
						else if (num == 8)
						{
							Dialog dialog4 = new Dialog(3, "ﾒｯｾｰｼﾞ");
							dialog4.SetText("開発サーバーへ接続します。");
							if (dialog4.Show() == 4)
							{
								text = devHost;
								COMMON_SERVER = "https://applidev.kairogame.jp/";
								accountType_ = 0;
								break;
							}
						}
						else if (num == 2)
						{
							flag = true;
							break;
						}
					}
				}
			}
			if (flag)
			{
				IApplication currentApp = IApplication.GetCurrentApp();
				currentApp.GetFormManager().RemoveAllForms();
				currentApp.Terminate();
			}
			else
			{
				if (!text.StartsWith("http"))
				{
					text = "http://" + text;
				}
				if (!text.EndsWith("/"))
				{
					text += "/";
				}
				host_ = new SecureString(text);
				if (startCount_ == 0)
				{
					AccessCounter(0, "first_boot");
				}
				AccessCounter(0, "boot");
				if (startCount_ < int.MaxValue)
				{
					startCount_++;
				}
			}
			init_ = true;
			if (!flag)
			{
				Save();
			}
		}

		public void Reboot()
		{
			init_ = false;
		}

		public void Start()
		{
			if (!init_)
			{
				throw new Exception("KairoServiceが初期化されていません");
			}
			if (start_)
			{
				return;
			}
			prgStart_[0] = (prgStart_[1] = 0);
			if ((networkLevel_ >= 2 || networkLevel_ == 1) && !(bool)IApplication.GetCurrentApp().RunOnUiThread(delegate
			{
				int num = 0;
				while (true)
				{
					try
					{
						if (Application.internetReachability != NetworkReachability.NotReachable)
						{
							return true;
						}
						string title = (Language.Japanese() ? "接続確認" : "Connection");
						string text = (Language.Japanese() ? "ネットワークが検出されませんでした。\n電波が無いか、通信が許可されていない可能性があります。" : "Network was not detected. \nOr radio waves do not, there is a possibility that the communication is not permitted.");
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
							return false;
						}
						num++;
					}
					catch
					{
						return true;
					}
				}
			}, true))
			{
				IApplication currentApp = IApplication.GetCurrentApp();
				try
				{
					currentApp.GetFormManager().RemoveAllForms();
				}
				catch (Exception)
				{
				}
				currentApp.Terminate();
				return;
			}
			if (Config.DEBUG && Regex.IsMatch(pubHost_, "([a-zA-Z0-9_]+\\.vir\\.kagoya\\.net|[0-9]+\\.[0-9.]+)"))
			{
				KairoPlugin.ShowToast("『" + pubHost_ + "』は海外でブロックされる可能性があります。\n別の名前でドメインを登録しましょう。");
			}
			ConnectLogin(false, networkLevel_ >= 2 || (networkLevel_ == 1 && !CheckNetwork()));
			SetSystemStartProgress(50);
			try
			{
				byte[] array = ConnectAppList(appListDate_);
				if (array != null)
				{
					appList_ = LoadAppList(array, out appListDate_);
					Encrypter.Encode(array);
					Storage.Write(1, true, "app/list.bin", array);
				}
			}
			catch (Exception)
			{
			}
			SetSystemStartProgress(100);
			OnStart();
			prgStart_[1] = 100;
			start_ = true;
			AnalyticsTracker.DispatchPageView();
		}

		private List<PaymentDataRecord> paymemtList_ = new List<PaymentDataRecord>();

		private const int PAY_MODE_GET = 1;

		private const int PAY_MODE_ADD = 2;

		private const int PAY_MODE_USE = 3;

		private static readonly int[] CONNECT_TRY = new int[4] { 1, 2, 3, 1 };

		private const int CONNECT_TRY_SLEEP = 100;

		private const bool NEXT_STARTUP_ASYSC = false;

		private const string PAY_ASYNC_TAG = "startUpSync";

		private const string PAY_ASYNC_TAG_S = "sus";

		private const string PAYMENT_CGI = "virtual_currency.php";

		public List<string> connectPaymemtMoneyResponse_ = new List<string>();

		protected void Load(byte[] data)
		{
			if (data == null || data.Length == 0)
			{
				return;
			}
			ByteArrayInputStream byteArrayInputStream = null;
			try
			{
				byteArrayInputStream = new ByteArrayInputStream(data);
				StreamUtil.ReadByte(byteArrayInputStream);
				userId_ = new SecureString(StreamUtil.ReadString(byteArrayInputStream));
				friendId_ = new SecureString(StreamUtil.ReadString(byteArrayInputStream));
				accountType_ = StreamUtil.ReadByte(byteArrayInputStream);
				startCount_ = StreamUtil.ReadInt(byteArrayInputStream);
				cheat_ = StreamUtil.ReadLong(byteArrayInputStream);
				flag_ = new SecureInt(StreamUtil.ReadInt(byteArrayInputStream));
				accessCounter_ = new Vector();
				int num = StreamUtil.ReadByte(byteArrayInputStream);
				for (int i = 0; i < num; i++)
				{
					accessCounter_.AddElement(new AccessCount(StreamUtil.ReadString(byteArrayInputStream), StreamUtil.ReadLong(byteArrayInputStream)));
				}
				userCounter_ = new Vector();
				num = StreamUtil.ReadByte(byteArrayInputStream);
				for (int j = 0; j < num; j++)
				{
					userCounter_.AddElement(new AccessCount(StreamUtil.ReadString(byteArrayInputStream), StreamUtil.ReadLong(byteArrayInputStream)));
				}
				connectTaskList_ = new Vector();
				num = StreamUtil.ReadInt(byteArrayInputStream);
				for (int k = 0; k < num; k++)
				{
					ConnectTask connectTask = new ConnectTask();
					connectTask.Deserialize(byteArrayInputStream);
					connectTaskList_.AddElement(connectTask);
				}
				OnLoad(byteArrayInputStream);
			}
			finally
			{
				byteArrayInputStream?.Close();
				byteArrayInputStream = null;
			}
		}

		public byte[] Write()
		{
			if (!IsInit())
			{
				return new byte[0];
			}
			ByteArrayOutputStream byteArrayOutputStream = null;
			try
			{
				byteArrayOutputStream = new ByteArrayOutputStream();
				StreamUtil.WriteByte((OutputStream)byteArrayOutputStream, (byte)0);
				StreamUtil.WriteString(byteArrayOutputStream, userId_.ToString());
				StreamUtil.WriteString(byteArrayOutputStream, friendId_.ToString());
				StreamUtil.WriteByte(byteArrayOutputStream, (byte)accountType_);
				StreamUtil.WriteInt(byteArrayOutputStream, startCount_);
				StreamUtil.WriteLong(byteArrayOutputStream, cheat_);
				StreamUtil.WriteInt(byteArrayOutputStream, flag_.Get() & 0xFFFF);
				StreamUtil.WriteByte(byteArrayOutputStream, (byte)accessCounter_.Size());
				for (int i = 0; i < accessCounter_.Size(); i++)
				{
					AccessCount accessCount = (AccessCount)accessCounter_.ElementAt(i);
					StreamUtil.WriteString(byteArrayOutputStream, accessCount.name_);
					StreamUtil.WriteLong(byteArrayOutputStream, accessCount.value_);
				}
				StreamUtil.WriteByte(byteArrayOutputStream, (byte)userCounter_.Size());
				for (int j = 0; j < userCounter_.Size(); j++)
				{
					AccessCount accessCount2 = (AccessCount)userCounter_.ElementAt(j);
					StreamUtil.WriteString(byteArrayOutputStream, accessCount2.name_);
					StreamUtil.WriteLong(byteArrayOutputStream, accessCount2.value_);
				}
				StreamUtil.WriteInt(byteArrayOutputStream, connectTaskList_.Size());
				for (int k = 0; k < connectTaskList_.Size(); k++)
				{
					((ConnectTask)connectTaskList_.ElementAt(k)).Serialize(byteArrayOutputStream);
				}
				OnWrite(byteArrayOutputStream);
				return byteArrayOutputStream.ToByteArray();
			}
			finally
			{
				byteArrayOutputStream?.Close();
				byteArrayOutputStream = null;
			}
		}

		public void NewGame(bool take)
		{
			ITapjoy tapjoy = GetTapjoy();
			if (tapjoy != null)
			{
				tapjoy.CallBalance();
			}
			flag_ = new SecureInt(flag_.Get() & 0xFFFF);
			OnNewGame(take);
			KairoBase instance = GetInstance();
			if (instance != null)
			{
				instance.SetGameState(true);
			}
		}

		public void LoadGame(byte[] data)
		{
			ITapjoy tapjoy = GetTapjoy();
			if (tapjoy != null)
			{
				tapjoy.CallBalance();
			}
			if (data != null && data.Length != 0)
			{
				ByteArrayInputStream byteArrayInputStream = null;
				try
				{
					byteArrayInputStream = new ByteArrayInputStream(data);
					flag_ = new SecureInt((flag_.Get() & 0xFFFF) | StreamUtil.ReadInt(byteArrayInputStream));
					OnLoadGame(byteArrayInputStream);
				}
				finally
				{
					if (byteArrayInputStream != null)
					{
						byteArrayInputStream.Close();
					}
					byteArrayInputStream = null;
				}
			}
			KairoBase instance = GetInstance();
			if (instance != null)
			{
				instance.SetGameState(true);
			}
		}

		public byte[] WriteGame()
		{
			ByteArrayOutputStream byteArrayOutputStream = null;
			try
			{
				byteArrayOutputStream = new ByteArrayOutputStream();
				StreamUtil.WriteInt(byteArrayOutputStream, (int)(flag_.Get() & 0xFFFF0000u));
				OnWriteGame(byteArrayOutputStream);
				return byteArrayOutputStream.ToByteArray();
			}
			finally
			{
				byteArrayOutputStream?.Close();
				byteArrayOutputStream = null;
			}
		}

		protected virtual void OnInit()
		{
		}

		public virtual void OnStart()
		{
		}

		protected virtual void OnLoad(InputStream stream)
		{
		}

		protected virtual void OnWrite(OutputStream os)
		{
		}

		protected virtual void OnNewGame(bool take)
		{
		}

		protected virtual void OnLoadGame(InputStream stream)
		{
		}

		protected virtual void OnWriteGame(OutputStream os)
		{
		}

		public virtual void Finish()
		{
			start_ = false;
		}

		public bool ConnectLogin(bool enforce = false, bool retry = false)
		{
			JLocale jLocale = JLocale.GetDefault();
			string[] array = null;
			bool isRetryEnable = Http.IsRetryEnable;
			try
			{
				byte[] array2 = GetSignature().GetBytes();
				if (array2 != null && array2.Length == 0)
				{
					array2 = null;
				}
				if (retry)
				{
					Http.IsRetryEnable = true;
				}
				array = CgiLogin(SecureString.ToString(userId_), jLocale.GetCountry(), IApplication.GetCurrentApp().GetPackageName(), (rootBan_ && CheckRoot()) ? "1" : "0", array2);
			}
			catch (Exception ex)
			{
				if (enforce)
				{
					throw ex;
				}
				if (networkLevel_ >= 2 || (networkLevel_ == 1 && !CheckNetwork()))
				{
					throw ex;
				}
				return false;
			}
			finally
			{
				Http.IsRetryEnable = isRetryEnable;
			}
			SetSystemStartProgress(25);
			latestVersion_ = JInteger.ParseInt(array[0]);
			long time = JLong.ParseLong(array[1]);
			userId_ = new SecureString(array[2]);
			friendId_ = new SecureString(array[3]);
			pkgId_ = JInteger.ParseInt(array[4]);
			if (5 < array.Length)
			{
				illegalPackage_ = JInteger.ParseInt(array[5]) == 1;
			}
			if (6 < array.Length)
			{
				arguments_ = array[6].Split(',');
				argumentsInt_ = new int[arguments_.Length];
				for (int i = 0; i < arguments_.Length; i++)
				{
					bool flag = int.TryParse(arguments_[i], out argumentsInt_[i]);
					if (!flag && arguments_[i].StartsWith("0x"))
					{
						flag = int.TryParse(arguments_[i].Substring(2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out argumentsInt_[i]);
					}
					if (!flag)
					{
						argumentsInt_[i] = 0;
					}
				}
			}
			ClockTime.SetTime(time);
			string country = JLocale.GetDefault().GetCountry();
			if (Config.LANGUAGE == 0 && country != "JP")
			{
				illegalCountry_ = true;
			}
			if (accountType_ == -1)
			{
				accountType_ = ((!Config.DEBUG) ? 1 : 0);
			}
			Save();
			if (Config.PRINT && IsCheater())
			{
				Log.Info("不正ユーザー");
			}
			login_ = true;
			Connect(null);
			SetSystemStartProgress(50);
			return true;
		}

		public void AccessCounter(int type, string name)
		{
			AccessCounter(type, name, 1, true);
		}

		public void AccessCounter(int type, string name, int up)
		{
			AccessCounter(type, name, up, true);
		}

		public void AccessCounter(int type, string name, int up, bool save)
		{
			Vector vector = ((type == 0) ? accessCounter_ : userCounter_);
			AccessCount accessCount = null;
			int num = 0;
			while (num < vector.Size() && accessCount == null)
			{
				AccessCount accessCount2 = (AccessCount)vector.ElementAt(num);
				if (name.Equals(accessCount2.name_))
				{
					accessCount = accessCount2;
				}
				num++;
			}
			if (accessCount == null)
			{
				accessCount = new AccessCount(name, 0L);
				vector.AddElement(accessCount);
			}
			accessCount.value_ += up;
			try
			{
				if (save)
				{
					Save();
				}
			}
			catch (Exception)
			{
			}
		}

		private void ConnectAccessCounter()
		{
			if (!this.login_)
			{
				return;
			}
			if (this.accessCounter_.Size() > 0)
			{
				string str = "";
				for (int i = 0; i < this.accessCounter_.Size(); i++)
				{
					if (str.Length() > 0)
					{
						str = string.Concat(str, ",");
					}
					str = string.Concat(str, this.accessCounter_.ElementAt(i));
				}
				str = KairoBase.MakeQuery("values", str);
				string str1 = KairoBase.MakeQuery("pkg", IApplication.GetCurrentApp().GetPackageName());
				string str2 = string.Concat("counter.php", str, str1);
				Vector vector = this.accessCounter_;
				this.accessCounter_ = new Vector();
				try
				{
					this.HttpConnect(this.MakeUrl(str2), null, true);
					this.Save();
				}
				catch (Exception exception1)
				{
					Exception exception = exception1;
					if (Config.PRINT)
					{
						Debug.LogException(exception);
					}
					this.accessCounter_ = vector;
					throw exception;
				}
			}
			if (this.userCounter_.Size() > 0)
			{
				string str3 = "";
				for (int j = 0; j < this.userCounter_.Size(); j++)
				{
					if (str3.Length() > 0)
					{
						str3 = string.Concat(str3, ",");
					}
					str3 = string.Concat(str3, this.userCounter_.ElementAt(j));
				}
				str3 = KairoBase.MakeQuery("values", str3);
				string str4 = string.Concat("user_counter.php", str3);
				Vector vector1 = this.userCounter_;
				this.userCounter_ = new Vector();
				try
				{
					this.HttpConnect(this.MakeUrl(str4), null, true);
					this.Save();
				}
				catch (Exception exception3)
				{
					Exception exception2 = exception3;
					if (Config.PRINT)
					{
						Debug.LogException(exception2);
					}
					this.userCounter_ = vector1;
					throw exception2;
				}
			}
		}

		public string[] ConnectGift(string pw)
		{
			pw = MakeQuery("pw", pw);
			string text = JString.New(Connect("gift.php" + pw, null), "UTF-8");
			if (text.Length() == 0)
			{
				return new string[0];
			}
			return StringUtil.Split(text, "\n", true);
		}

		public GameDataRecord ConnectGameData(GameDataRecord gameData)
		{
			return ConnectGameData(null, gameData);
		}

		public GameDataRecord ConnectGameData(string tbname, GameDataRecord gameData)
		{
			return ConnectGameData(tbname, new GameDataRecord[1] { gameData })[0];
		}

		public GameDataRecord[] ConnectGameData(GameDataRecord[] dataList)
		{
			return ConnectGameData(null, dataList);
		}

		public GameDataRecord[] ConnectGameData(string tbname, GameDataRecord[] dataList)
		{
			if (dataList.Length == 0)
			{
				return dataList;
			}
			Vector[] array = new Vector[3];
			int[] array2 = new int[dataList.Length];
			for (int i = 0; i < array2.Length; i++)
			{
				array2[i] = i;
			}
			for (int j = 0; j < array2.Length - 1; j++)
			{
				for (int num = array2.Length - 1; num > j; num--)
				{
					GameDataRecord gameDataRecord = dataList[array2[num - 1]];
					GameDataRecord gameDataRecord2 = dataList[array2[num]];
					long num2 = (long)gameDataRecord.friendId_.Get() * 1000L + gameDataRecord.GetNumber();
					long num3 = (long)gameDataRecord2.friendId_.Get() * 1000L + gameDataRecord2.GetNumber();
					if (num2 > num3)
					{
						int num4 = array2[num];
						array2[num] = array2[num - 1];
						array2[num - 1] = num4;
					}
				}
			}
			byte[] send = null;
			ByteArrayOutputStream byteArrayOutputStream = null;
			try
			{
				byteArrayOutputStream = new ByteArrayOutputStream();
				for (int k = 0; k < array.Length; k++)
				{
					Vector vector = (array[k] = new Vector());
					for (int l = 0; l < array2.Length; l++)
					{
						GameDataRecord gameDataRecord3 = dataList[array2[l]].Copy(k == 0 || k == 1);
						if (gameDataRecord3.dataSet_.Size() > 0 && (k != 0 || gameDataRecord3.isInsert_) && (k != 1 || !gameDataRecord3.isInsert_))
						{
							vector.AddElement(gameDataRecord3);
						}
					}
					if (vector.Size() != 0)
					{
						StreamUtil.WriteInt(byteArrayOutputStream, vector.Size());
						for (int m = 0; m < vector.Size(); m++)
						{
							((GameDataRecord)vector.ElementAt(m)).Pack(byteArrayOutputStream);
						}
					}
				}
				send = byteArrayOutputStream.ToByteArray();
			}
			finally
			{
				byteArrayOutputStream?.Close();
				byteArrayOutputStream = null;
			}
			if (array[0].Size() == 0 && array[2].Size() == 0 && array[1].Size() == 0)
			{
				return dataList;
			}
			string text = "";
			for (int n = 0; n < array.Length; n++)
			{
				if (array[n].Size() != 0)
				{
					if (text.Length() > 0)
					{
						text += ",";
					}
					switch (n)
					{
					case 0:
						text += "set";
						break;
					case 1:
						text += "upd";
						break;
					case 2:
						text += "get";
						break;
					}
				}
			}
			text = MakeQuery("mode", text);
			tbname = MakeQuery("tbname", tbname);
			byte[][] array3 = Connect("gamedata.php" + text + tbname, send, true);
			if (array3 == null)
			{
				return dataList;
			}
			GameDataRecord[] array4 = new GameDataRecord[dataList.Length];
			for (int num5 = 0; num5 < dataList.Length; num5++)
			{
				array4[num5] = dataList[num5].Clone();
				array4[num5].exist_ = false;
			}
			for (int num6 = 0; num6 < array3.Length; num6++)
			{
				if (array3[num6].Length == 0)
				{
					continue;
				}
				ByteArrayInputStream byteArrayInputStream = null;
				try
				{
					byteArrayInputStream = new ByteArrayInputStream(array3[num6]);
					int num7 = StreamUtil.ReadInt(byteArrayInputStream);
					int num8 = StreamUtil.ReadInt(byteArrayInputStream);
					int num9 = StreamUtil.ReadInt(byteArrayInputStream);
					GameDataRecord gameDataRecord4 = null;
					foreach (GameDataRecord gameDataRecord5 in array4)
					{
						if ((gameDataRecord5.userId_.Get() == num7 || gameDataRecord5.friendId_.Get() == num8) && gameDataRecord5.number_.Get() == num9)
						{
							gameDataRecord4 = gameDataRecord5;
							break;
						}
					}
					gameDataRecord4.exist_ = true;
					int num11 = StreamUtil.ReadInt(byteArrayInputStream);
					for (int num12 = 0; num12 < num11; num12++)
					{
						string name = StreamUtil.ReadString(byteArrayInputStream);
						byte[] value = null;
						int num13 = StreamUtil.ReadInt(byteArrayInputStream);
						if (num13 != -1)
						{
							value = StreamUtil.Read(byteArrayInputStream, num13);
						}
						gameDataRecord4.Acquire(name, FromDBValue("t_gamedata", name, value));
					}
				}
				finally
				{
					byteArrayInputStream?.Close();
					byteArrayInputStream = null;
				}
			}
			return array4;
		}

		public int[] ConnectGameDataNumbers(string tbname = null)
		{
			string[] array = StringUtil.Split(JString.New(Connect("gamedata.php" + MakeQuery("mode", "num") + MakeQuery("tbname", tbname))), ",");
			int[] array2 = new int[array.Length];
			for (int i = 0; i < array2.Length; i++)
			{
				array2[i] = JInteger.ParseInt(array[i]);
			}
			return array2;
		}

		public GameDataRecord[] ConnectGameDataSearch(string get, string where, string order, string count, int logic)
		{
			return ConnectGameDataSearch(null, get, where, order, count, logic);
		}

		public GameDataRecord[] ConnectGameDataSearch(string tbname, string get, string where, string order, string count, int logic)
		{
			return ConnectGameDataSearch(tbname, get, where, order, null, long.MinValue, 0L, 0L, count, logic);
		}

		public GameDataRecord[] ConnectGameDataSearch(string get, string where, string order, string column, long value, long low, long high, string count, int logic)
		{
			return ConnectGameDataSearch(null, get, where, order, column, value, low, high, count, logic);
		}

		public GameDataRecord[] ConnectGameDataSearch(string tbname, string get, string where, string order, string column, long value, long low, long high, string count, int logic)
		{
			if (get == null)
			{
				get = "friendid,number,v0";
			}
			if (where == null)
			{
				where = "pid=" + Config.PID + ",ban=0,is_alive=1,reg_friend=1";
			}
			get = CorrectGetGameData(get);
			if ((logic == 1 || logic == 2) && column != null && value == long.MinValue)
			{
				long num = high - low;
				if (num > 0)
				{
					value = low + num / 2;
				}
			}
			where = CorrectWhereGameData(where);
			string param = null;
			string param2 = null;
			string param3 = null;
			if (column != null)
			{
				param = ((value != long.MinValue) ? (column + "," + value) : column);
				param2 = JString.ValueOf(low);
				param3 = JString.ValueOf(high);
			}
			string[] array = StringUtil.Split(get, ",");
			string param4 = ((logic < 0) ? "0" : JString.ValueOf(logic));
			string param5 = "search";
			tbname = MakeQuery("tbname", tbname);
			param5 = MakeQuery("mode", param5);
			get = MakeQuery("get", get);
			where = MakeQuery("where", where);
			order = MakeQuery("order", order);
			param = MakeQuery("value", param);
			param2 = MakeQuery("low", param2);
			param3 = MakeQuery("high", param3);
			count = MakeQuery("count", count);
			param4 = MakeQuery("logic", param4);
			byte[][] array2 = Connect("gamedata.php" + tbname + param5 + get + where + order + param + param2 + param3 + count + param4, true);
			GameDataRecord[] array3 = new GameDataRecord[array2.Length];
			for (int i = 0; i < array2.Length; i++)
			{
				GameDataRecord gameDataRecord = (array3[i] = new GameDataRecord());
				ByteArrayInputStream byteArrayInputStream = null;
				try
				{
					byteArrayInputStream = new ByteArrayInputStream(array2[i]);
					gameDataRecord.friendId_ = new SecureInt(StreamUtil.ReadInt(byteArrayInputStream));
					gameDataRecord.number_ = new SecureInt(StreamUtil.ReadInt(byteArrayInputStream));
					for (int j = 0; j < array.Length; j++)
					{
						string name = array[j].Trim();
						int num2 = StreamUtil.ReadInt(byteArrayInputStream);
						byte[] data = null;
						if (num2 != -1)
						{
							data = StreamUtil.Read(byteArrayInputStream, num2);
						}
						gameDataRecord.Add(name, data);
					}
				}
				finally
				{
					byteArrayInputStream?.Close();
				}
			}
			return array3;
		}

		public GameDataRecord[][] ConnectGameDataRanking(string friendId, string get, string where, string column, string top, bool self, string order)
		{
			return ConnectGameDataRanking(null, friendId, get, where, column, top, self, order);
		}

		public GameDataRecord[][] ConnectGameDataRanking(string tbname, string friendId, string get, string where, string column, string top, bool self, string order)
		{
			if (get == null)
			{
				get = "friendid,number,v0";
			}
			if (where == null)
			{
				where = "pid=" + Config.PID + ",ban=0,is_alive=1";
			}
			if (top == null)
			{
				top = "0";
			}
			if (order == null)
			{
				order = "DESC";
			}
			get = CorrectGetGameData(get);
			where = CorrectWhereGameData(where);
			string[] array = StringUtil.Split(get, ",");
			string param = "rank";
			tbname = MakeQuery("tbname", tbname);
			param = MakeQuery("mode", param);
			friendId = MakeQuery("friendid", friendId);
			get = MakeQuery("get", get);
			where = MakeQuery("where", where);
			column = MakeQuery("column", column);
			top = MakeQuery("top", top);
			string text = MakeQuery("self", self ? "1" : "0");
			order = MakeQuery("order", order);
			byte[][] array2 = Connect("gamedata.php" + tbname + param + friendId + get + where + column + top + text + order, true);
			GameDataRecord[][] array3 = new GameDataRecord[2][];
			for (int i = 0; i < array2.Length; i++)
			{
				ByteArrayInputStream byteArrayInputStream = null;
				try
				{
					byteArrayInputStream = new ByteArrayInputStream(array2[i]);
					int num = StreamUtil.ReadInt(byteArrayInputStream);
					array3[i] = new GameDataRecord[num];
					for (int j = 0; j < array3[i].Length; j++)
					{
						GameDataRecord gameDataRecord = (array3[i][j] = new GameDataRecord());
						gameDataRecord.friendId_ = new SecureInt(StreamUtil.ReadInt(byteArrayInputStream));
						gameDataRecord.number_ = new SecureInt(StreamUtil.ReadInt(byteArrayInputStream));
						gameDataRecord.Add("_rank", StreamUtil.ReadInt(byteArrayInputStream));
						for (int k = 0; k < array.Length; k++)
						{
							string name = array[k].Trim();
							int num2 = StreamUtil.ReadInt(byteArrayInputStream);
							byte[] data = null;
							if (num2 != -1)
							{
								data = StreamUtil.Read(byteArrayInputStream, num2);
							}
							gameDataRecord.Add(name, data);
						}
					}
				}
				finally
				{
					byteArrayInputStream?.Close();
				}
			}
			return array3;
		}

		private string CorrectGetGameData(string get)
		{
			if (get == null)
			{
				return null;
			}
			string[] array = StringUtil.Split(get, ",");
			get = "";
			for (int i = 0; i < array.Length; i++)
			{
				if (get.Length() > 0)
				{
					get += ",";
				}
				get += array[i].Trim();
			}
			return get;
		}

		private string CorrectWhereGameData(string where)
		{
			if (where == null)
			{
				return null;
			}
			string[] array = new string[4] { "<=>", "<>", "!=", "=" };
			string[] array2 = StringUtil.Split(where, ",");
			where = "";
			for (int i = 0; i < array2.Length; i++)
			{
				string text = array2[i];
				for (int j = 0; j < array.Length; j++)
				{
					int num = text.IndexOf(array[j]);
					if (num != -1 && num != 0 && num != text.Length() - 1)
					{
						string text2 = text.SubstringJ(0, num);
						if (GetColumnType("t_gamedata", text2) == 1)
						{
							string text3 = text.SubstringJ(num, num + array[j].Length());
							string str = text.SubstringJ(num + array[j].Length());
							byte[] bytes = ToDBValue("t_gamedata", text2, str.GetBytes());
							text = text2 + text3 + JString.New(bytes, "UTF-8");
							break;
						}
					}
				}
				if (where.Length() > 0)
				{
					where += ",";
				}
				where += text;
			}
			return where;
		}

		public byte[][] ConnectLineupList(string time)
		{
			byte[][] array = Connect("lineup.php" + MakeQuery("mode", "list") + MakeQuery("time", time), true);
			if (array.Length == 0)
			{
				return null;
			}
			return array;
		}

		public byte[][] ConnectLineupImage(string files)
		{
			return Connect("lineup.php" + MakeQuery("mode", "img") + MakeQuery("files", files), true);
		}

		public byte[] ConnectRegistScore(string m, string s, string r)
		{
			return Connect("regist_score.php" + MakeQuery("m", m) + MakeQuery("s", s) + MakeQuery("r", r));
		}

		public byte[] ConnectGetScore(string m)
		{
			return Connect("get_score.php" + MakeQuery("m", m));
		}

		public byte[] ConnectRanking(string m, string r, string p)
		{
			return Connect("ranking.php" + MakeQuery("m", m) + MakeQuery("r", r) + MakeQuery("p", p));
		}

		public void ConnectTime()
		{
			ClockTime.SetTime(JLong.ParseLong(CgiTime()));
		}

		public void ConnectVersion()
		{
			string s = CgiVersion();
			latestVersion_ = JInteger.ParseInt(s);
		}

		protected byte[] FromDBValue(string table, string name, byte[] value)
		{
			if (value == null)
			{
				return value;
			}
			byte[] array = value;
			int columnType = GetColumnType(table, name);
			if (columnType == 1)
			{
				try
				{
					array = JString.New(value, "UTF-8").GetBytes();
				}
				catch (Exception)
				{
				}
			}
			else if (columnType == 2 && value.Length != 0)
			{
				bool num = value[0] == 1;
				array = new byte[value.Length - 1];
				JSystem.Arraycopy(value, 1, array, 0, array.Length);
				if (num)
				{
					array = Compressor.Expand(array);
				}
			}
			return array;
		}

		internal byte[] ToDBValue(string table, string name, byte[] value)
		{
			if (value == null)
			{
				return value;
			}
			byte[] array = value;
			int columnType = GetColumnType(table, name);
			if (columnType == 1)
			{
				string text = JString.New(value);
				string text2 = text.Trim();
				if (!text2.Equals("CURRENT_TIMESTAMP()"))
				{
					StringBuilder stringBuilder = new StringBuilder();
					int num = 0;
					while (num < text2.Length())
					{
						char c = text2.CharAt(num);
						stringBuilder.Append(c);
						if (c == '\'')
						{
							stringBuilder.Append(c);
						}
						num++;
					}
					text = "'" + stringBuilder.ToString() + "'";
				}
				try
				{
					array = text.GetBytes("UTF-8");
				}
				catch (Exception)
				{
					array = text.GetBytes();
				}
			}
			else if (columnType == 2)
			{
				byte[] array2 = null;
				try
				{
					array2 = Compressor.Compress(value);
					if (array2.Length >= value.Length)
					{
						array2 = null;
					}
				}
				catch (Exception)
				{
				}
				if (array2 == null)
				{
					array = new byte[1 + value.Length];
					array[0] = 0;
					JSystem.Arraycopy(value, 0, array, 1, value.Length);
				}
				else
				{
					array = new byte[1 + array2.Length];
					array[0] = 1;
					JSystem.Arraycopy(array2, 0, array, 1, array2.Length);
				}
			}
			return array;
		}

		protected int GetColumnType(string table, string column)
		{
			if (table == null || column == null)
			{
				return -1;
			}
			column = column.Trim();
			if (table == "t_gamedata")
			{
				switch (column)
				{
				case "country":
				case "ins_date":
				case "login":
				case "upload":
					return 1;
				default:
				{
					column = StringUtil.Split(column, "_")[0];
					if (column == "s" || column == "str")
					{
						return 1;
					}
					int result;
					if (column.StartsWith("s") && int.TryParse(column.Substring(1), out result))
					{
						return 1;
					}
					if (column == "d" || column == "dat")
					{
						return 2;
					}
					int result2;
					if (column.StartsWith("d") && int.TryParse(column.Substring(1), out result2))
					{
						return 2;
					}
					return 0;
				}
				}
			}
			if (table == "t_user" && column == "name")
			{
				return 1;
			}
			return -1;
		}

		private string[] CgiLogin(string userid, string c, string pkg, string r, byte[] signature)
		{
			userid = MakeQuery("userid", userid);
			c = MakeQuery("c", c);
			pkg = MakeQuery("pkg", pkg);
			r = MakeQuery("r", r);
			string cgi = "login.php" + userid + c + pkg + r;
			return StringUtil.Split(JString.New(HttpConnect(MakeUrl(cgi, signature), signature, true)), "\n");
		}

		private string CgiTime()
		{
			string cgi = "time.php";
			return JString.New(HttpConnect(MakeUrl(cgi), null, true));
		}

		private string CgiVersion()
		{
			string cgi = "version.php";
			return JString.New(HttpConnect(MakeUrl(cgi), null, true));
		}

		public static string MakeQuery(string query, int param)
		{
			return MakeQuery(query, param.ToString());
		}

		public static string MakeQuery(string query, string param)
		{
			if (param == null || param.Length() == 0)
			{
				return "";
			}
			param = StringUtil.Replace(param, "\b", "");
			try
			{
				param = URLEncoder.Encode(param, "UTF-8");
			}
			catch (ArgumentException)
			{
				param = URLEncoder.Encode(param);
			}
			if (!query.StartsWith("&"))
			{
				query = "&" + query;
			}
			if (!query.EndsWith("="))
			{
				query += "=";
			}
			return query + param;
		}

		public string MakeUrl(string cgi)
		{
			return MakeUrl(cgi, null);
		}

		public string MakeUrl(string cgi, byte[] send)
		{
			string text = null;
			int num;
			if (cgi.StartsWith("http://") || cgi.StartsWith("https://"))
			{
				num = cgi.LastIndexOf('/');
				if (num != -1)
				{
					text = cgi.Substring(0, num + 1);
					cgi = cgi.Substring(num + 1);
				}
			}
			string text2 = cgi;
			num = text2.IndexOf(".php");
			if (num == -1)
			{
				num = text2.IndexOf(".cgi");
			}
			if (num != -1)
			{
				text2 = text2.Substring(0, num);
				num = text2.LastIndexOf('/');
				if (num != -1)
				{
					text2 = text2.Substring(num + 1);
				}
			}
			string text3 = "";
			int num2 = cgi.IndexOf("?");
			if (num2 == -1)
			{
				num2 = cgi.IndexOf("&");
			}
			if (num2 != -1)
			{
				text3 = cgi.SubstringJ(num2 + 1);
				cgi = cgi.SubstringJ(0, num2);
				if (text3.Length() > 0)
				{
					text3 = "&" + text3;
				}
			}
			if (!text3.Contains("&pid="))
			{
				text3 += MakeQuery("pid=", JString.ValueOf(Config.PID));
			}
			if (!text3.Contains("&ver="))
			{
				text3 += MakeQuery("ver=", JString.ValueOf(appliVersion_));
			}
			if (!text3.Contains("&userid="))
			{
				text3 += MakeQuery("userid=", GetUserId());
			}
			if (!text3.Contains("&pkg="))
			{
				text3 += MakeQuery("pkg=", IApplication.GetCurrentApp().GetPackageName());
			}
			text3 = StringUtil.Replace(text3, "&&", "&");
			if (text3.Length() > 0)
			{
				cgi = cgi + "?" + text3.Substring(1);
			}
			bool flag = cgi.Contains("virtual_currency.php");
			string text4;
			if (text != null && text.Contains(GetCommonServer()))
			{
				text4 = text + cgi;
			}
			else if (flag)
			{
				text4 = GetPaymentHost() + "vc/cgi/" + cgi;
			}
			else
			{
				text4 = host_.ToString();
				if (Config.APPLI_TYPE == 2)
				{
					text4 += "android";
				}
				else if (Config.APPLI_TYPE == 3)
				{
					text4 += "android";
				}
				else if (Config.APPLI_TYPE == 0)
				{
					text4 += "docomo";
				}
				else if (Config.APPLI_TYPE == 1)
				{
					text4 += "softbank";
				}
				text4 = text4 + "/gm" + Config.PID + "/cgi/" + cgi;
			}
			while (text4.IndexOf("NULLGWDOCOMO") != -1)
			{
			}
			text3 = "";
			int num3 = text4.IndexOf('?');
			if (num3 != -1)
			{
				string str = text4.SubstringJ(num3 + 1);
				text4 = text4.SubstringJ(0, num3);
				byte[] bytes = str.GetBytes();
				Encrypter.Encode(bytes, new byte[3] { 72, 123, 54 });
				str = Base64.Encode(bytes);
				text3 = text3 + "&_1=" + URLEncoder.Encode(str);
				text3 = text3 + "&_2=" + MD5.GetValue(str.GetBytes());
			}
			if (send != null)
			{
				text3 = text3 + "&_3=" + MD5.GetValue(send);
			}
			text3 = text3 + "&hl=" + (Language.Japanese() ? "ja" : "en");
			int value;
			if (!cgiLogLevel_.TryGetValue(text2, out value))
			{
				value = 222;
			}
			text3 = text3 + "&log=" + value;
			text3 = text3 + "&pkg=" + IApplication.GetCurrentApp().GetPackageName();
			if (cgi.Contains("startUpSync"))
			{
				text3 += "&sus=1";
			}
			if (text3.Length() > 0)
			{
				text4 = text4 + "?" + text3.Substring(1);
			}
			return text4;
		}

		public static byte[] Connect(string cgi)
		{
			byte[][] array = Connect(cgi, null, false);
			if (array == null || array.Length == 0)
			{
				return null;
			}
			return array[0];
		}

		public static byte[][] Connect(string cgi, bool split)
		{
			return Connect(cgi, null, split);
		}

		public static byte[] Connect(string cgi, byte[] send)
		{
			byte[][] array = Connect(cgi, send, false);
			if (array == null || array.Length == 0)
			{
				return null;
			}
			return array[0];
		}

		public static byte[][] Connect(string cgi, byte[] send, bool split)
		{
			KairoBase instance = GetInstance();
			if (cgi != null && !instance.login_)
			{
				instance.ConnectLogin();
			}
			byte[][] result = null;
			if (cgi != null)
			{
				result = instance.HttpConnect(instance.MakeUrl(cgi, send), send, true, split);
			}
			instance.Connected(cgi, send, split);
			return result;
		}

		public static void ExecuteAsyncConnects()
		{
			KairoBase instance = GetInstance();
			try
			{
				instance.ExecuteConnectTask();
			}
			catch (Exception)
			{
			}
		}

		protected virtual void Connected(string cgi, byte[] send, bool split)
		{
			NotifyManager instance = NotifyManager.GetInstance();
			if (instance != null && instance.IsPushEnable())
			{
				string gCMRegistrationId = KairoPlugin.GetGCMRegistrationId();
				if (gCMRegistrationId != null && gCMRegistrationId.Length > 0 && gCMRegistrationId != gcmId_)
				{
					try
					{
						ConnectNotificationRegist(gCMRegistrationId);
						gcmId_ = gCMRegistrationId;
					}
					catch (Exception t)
					{
						Log.Info(t);
					}
				}
			}
			try
			{
				ConnectAccessCounter();
			}
			catch (Exception)
			{
			}
			try
			{
				ExecuteConnectTask();
			}
			catch (Exception)
			{
			}
		}

		public static void ConnectAsync(string cgi, byte[] send)
		{
			ConnectAsync(cgi, send, 2, 1);
		}

		public static void ConnectAsync(string cgi, byte[] send, int count, int day)
		{
			KairoBase instance = GetInstance();
			ConnectTask connectTask = new ConnectTask();
			connectTask.url_ = SecureString.ValueOf(instance.MakeUrl(cgi, send));
			if (send != null)
			{
				connectTask.send_ = new byte[send.Length];
				JSystem.Arraycopy(send, 0, connectTask.send_, 0, send.Length);
			}
			if (count > 100)
			{
				count = 100;
			}
			if (day > 100)
			{
				day = 100;
			}
			connectTask.countMax_ = count;
			connectTask.dayMax_ = day;
			connectTask.time_ = ClockTime.GetTime();
			instance.connectTaskList_.AddElement(connectTask);
		}

		public string GetUserId()
		{
			return SecureString.ToString(userId_);
		}

		public int GetUserIdInt()
		{
			string userId = GetUserId();
			if (userId == null)
			{
				return -1;
			}
			return int.Parse(userId);
		}

		public bool CheckUserId()
		{
			return GetUserId().Length() > 0;
		}

		public string GetFriendId()
		{
			return SecureString.ToString(friendId_);
		}

		public int GetFriendIdInt()
		{
			string friendId = GetFriendId();
			if (friendId == null)
			{
				return -1;
			}
			return int.Parse(friendId);
		}

		public bool CheckFriendId()
		{
			return GetFriendId().Length() > 0;
		}

		public int GetPackageId()
		{
			return pkgId_;
		}

		protected static string GetSignature()
		{
			return KairoPlugin.GetSignature();
		}

		protected void ExecuteConnectTask()
		{
			if (!login_)
			{
				return;
			}
			while (connectTaskList_.Size() > 0)
			{
				ConnectTask connectTask = (ConnectTask)connectTaskList_.FirstElement();
				connectTaskList_.RemoveElementAt(0);
				try
				{
					connectTask.count_++;
					string text = SecureString.ToString(connectTask.url_);
					if (!text.Contains("&userid="))
					{
						text = text + "&userid=" + GetUserId();
					}
					HttpConnect(text, connectTask.send_, true);
				}
				catch (Exception)
				{
					if (Http.GetErrorCode() == 2)
					{
						connectTask.count_--;
						break;
					}
					long num = (ClockTime.GetTime() - connectTask.time_) / 86400000;
					if ((connectTask.dayMax_ == -1 || num < connectTask.dayMax_) && (connectTask.countMax_ == -1 || connectTask.count_ < connectTask.countMax_))
					{
						connectTaskList_.AddElement(connectTask);
					}
					break;
				}
			}
		}

		public int GetAsyncConnectNum()
		{
			return connectTaskList_.Size();
		}

		public bool CheckNetwork()
		{
			return Application.internetReachability != NetworkReachability.NotReachable;
		}

		public int GetStartCount()
		{
			return startCount_;
		}

		public int GetLatestVersion()
		{
			if (latestVersion_ == 0)
			{
				return appliVersion_;
			}
			return latestVersion_;
		}

		public int GetAppliVersion()
		{
			return appliVersion_;
		}

		public virtual void Update()
		{
			if (cheat_ == 0L && SecureNumber.GetHackingCount() > 0)
			{
				cheat_ = SecureNumber.GetHackingCount();
				AccessCounter(0, "cheat", 1, false);
				AccessCounter(1, "cheat", 1, false);
				try
				{
					Save();
				}
				catch (Exception)
				{
				}
			}
		}

		public abstract void Save();

		public abstract void SaveAll();

		public bool CheckRoot()
		{
			return false;
		}

		private string Encode(string src)
		{
			if (!Config.DEBUG)
			{
				return "";
			}
			byte[] bytes = Base64.Encode(src).GetBytes();
			Encrypter.Encode(bytes, new byte[4] { 38, 76, 49, 27 });
			src = "";
			for (int i = 0; i < bytes.Length; i++)
			{
				if (i != 0)
				{
					src += ",";
				}
				src += bytes[i];
			}
			return src;
		}

		private string Decode(int[] src)
		{
			byte[] array = new byte[src.Length];
			for (int i = 0; i < src.Length; i++)
			{
				array[i] = (byte)src[i];
			}
			Encrypter.Decode(array, new byte[4] { 38, 76, 49, 27 });
			return JString.New(Base64.Decode(array));
		}

		public bool IsLogin()
		{
			return login_;
		}

		public bool IsStart()
		{
			return start_;
		}

		public bool Check(int bit)
		{
			return (flag_.Get() & bit) != 0;
		}

		public void On(int bit)
		{
			flag_ = new SecureInt(flag_.Get() | bit);
		}

		public void Off(int bit)
		{
			flag_ = new SecureInt(flag_.Get() & ~bit);
		}

		public bool IsInit()
		{
			return init_;
		}

		public void SetReserveSave(bool all)
		{
			int num = (all ? 2 : 1);
			if (this.reserveSave < num)
			{
				this.reserveSave = num;
			}
		}

		public void SetReserveSaveEnable(bool enable)
		{
			reserveSaveEnable = enable;
		}

		public virtual bool HandleActivityResult(int requestCode, int resultCode, object data)
		{
			return false;
		}

		public void SetLogLevel(string cgi, int send, int recv, int query)
		{
			if (send == -1)
			{
				send = 2;
			}
			if (recv == -1)
			{
				recv = 2;
			}
			if (query == -1)
			{
				query = 3;
			}
			cgiLogLevel_[cgi] = send * 100 + recv * 10 + query;
		}

		public long GetCheatCount()
		{
			return cheat_;
		}

		public bool IsCheater()
		{
			if (cheat_ <= 0 && !illegalPackage_)
			{
				return illegalCountry_;
			}
			return true;
		}

		public bool IsIllegalPackage()
		{
			return illegalPackage_;
		}

		public bool IsIllegalCountry()
		{
			return illegalCountry_;
		}

		public void SetCheater()
		{
			if (cheat_ == 0L)
			{
				cheat_ = 1L;
			}
		}

		public virtual void OnSaveGameBegin()
		{
		}

		public virtual void OnSaveGameEnd()
		{
		}

		public bool AddPaymentMoney(string product_id, string order_code, string purchaseToken, int amount, int action, bool async = false)
		{
			try
			{
				paymemtList_ = new List<PaymentDataRecord>();
				PaymentDataRecord paymentDataRecord = new PaymentDataRecord(product_id, order_code, amount, action);
				paymentDataRecord.purchaseToken_ = purchaseToken;
				paymentDataRecord.SetTimeStamp(ClockTime.GetTime());
				paymentDataRecord.send_ = true;
				paymemtList_.Add(paymentDataRecord);
				if (IApplication.GetCurrentApp().CheckUiThread() && !async)
				{
					if (ConnectPaymemtMoney(2, paymemtList_, true) == 0)
					{
						try
						{
							Save();
						}
						catch (Exception)
						{
						}
					}
				}
				else
				{
					int num = ConnectPaymemtMoney(2, paymemtList_, async);
					if (num == 1)
					{
						return true;
					}
					if (num == 2 || num == 3)
					{
						throw new IllegalPurchaseException();
					}
					if (!async && num != 1)
					{
						num = ConnectPaymemtMoney(2, paymemtList_, true);
						if (num == 0)
						{
							try
							{
								Save();
							}
							catch (Exception)
							{
							}
						}
					}
					if (async && num == 0)
					{
						try
						{
							Save();
						}
						catch (Exception)
						{
						}
						return true;
					}
				}
			}
			catch (IllegalPurchaseException ex4)
			{
				throw ex4;
			}
			catch (Exception)
			{
				return false;
			}
			return false;
		}

		public bool UsePaymentMoney(List<PaymentDataRecord> payList, bool async = false)
		{
			return UsePaymentMoney(0, 0, payList, async);
		}

		public bool UsePaymentMoney(int use_amount, int action)
		{
			return UsePaymentMoney(use_amount, action, null);
		}

		public bool UsePaymentMoney(int use_amount, int action, List<PaymentDataRecord> payList, bool async = false)
		{
			try
			{
				if (payList != null)
				{
					paymemtList_ = payList;
				}
				else
				{
					paymemtList_ = new List<PaymentDataRecord>();
					PaymentDataRecord paymentDataRecord = new PaymentDataRecord("", "", use_amount, action);
					paymentDataRecord.SetTimeStamp(ClockTime.GetTime());
					paymentDataRecord.SetPurchaseToken("ex");
					paymentDataRecord.SetSendFlags(true);
					paymemtList_.Add(paymentDataRecord);
				}
				if (IApplication.GetCurrentApp().CheckUiThread() && !async)
				{
					if (ConnectPaymemtMoney(3, paymemtList_, true) == 0)
					{
						try
						{
							Save();
						}
						catch (Exception)
						{
						}
					}
				}
				else
				{
					int num = ConnectPaymemtMoney(3, paymemtList_, async);
					if (num == 1)
					{
						return true;
					}
					if (!async && num != 1)
					{
						num = ConnectPaymemtMoney(3, paymemtList_, true);
						if (num == 0)
						{
							try
							{
								Save();
							}
							catch (Exception)
							{
							}
						}
					}
					if (async && num == 0)
					{
						try
						{
							Save();
						}
						catch (Exception)
						{
						}
						return true;
					}
				}
			}
			catch (Exception)
			{
				return false;
			}
			return false;
		}

		public int GetPaymentMoney()
		{
			try
			{
				paymemtList_ = new List<PaymentDataRecord>();
				PaymentDataRecord paymentDataRecord = new PaymentDataRecord("get_order", "get_product", 0, 0);
				paymentDataRecord.send_ = true;
				paymemtList_.Add(paymentDataRecord);
				return ConnectPaymemtMoney(1, paymemtList_);
			}
			catch (Exception)
			{
				return -1;
			}
		}

		public int ConnectPaymemtMoney(int mode, List<PaymentDataRecord> paymemtListData, bool async = false)
		{
			if (!CheckUserId())
			{
				return -1;
			}
			if (paymemtListData.Count == 0)
			{
				return -1;
			}
			if (mode != 2 && mode != 3 && mode != 1)
			{
				return -1;
			}
			string text = "";
			for (int i = 0; i < paymemtListData.Count; i++)
			{
				PaymentDataRecord paymentDataRecord = paymemtListData.ElementAt(i);
				if (paymentDataRecord.send_)
				{
					if (text.Length > 0)
					{
						text += "\t";
					}
					text = ((mode != 3 || Application.platform != RuntimePlatform.IPhonePlayer) ? (text + mode + "," + paymentDataRecord.orderId_ + "," + paymentDataRecord.productId_ + "," + IApplication.GetCurrentApp().GetPackageName() + "," + paymentDataRecord.amount_ + "," + paymentDataRecord.actCode_ + "," + paymentDataRecord.timestamp_ + "," + paymentDataRecord.purchaseToken_) : (text + mode + "," + paymentDataRecord.orderId_ + "_" + GetUserId() + "," + paymentDataRecord.productId_ + "," + IApplication.GetCurrentApp().GetPackageName() + "," + paymentDataRecord.amount_ + "," + paymentDataRecord.actCode_ + "," + paymentDataRecord.timestamp_ + "," + paymentDataRecord.purchaseToken_));
					if (mode == 2)
					{
						text += ",1";
					}
				}
			}
			if (text.Length == 0)
			{
				return -1;
			}
			if (mode == 3 && !async)
			{
				try
				{
					ExecuteConnectTask();
				}
				catch (Exception)
				{
				}
			}
			int num = CONNECT_TRY[mode];
			int num2 = 0;
			int result = 0;
			do
			{
				num2++;
				byte[] bytes = text.GetBytes();
				string cgi = "virtual_currency.php" + MakeQuery("startUpSync", async ? "1" : null);
				try
				{
					if (async)
					{
						ConnectAsync(cgi, bytes, 15, -1);
						break;
					}
					byte[][] array = Connect(cgi, bytes, false);
					if (array == null || array.Length == 0)
					{
						result = -1;
						goto IL_0304;
					}
					for (int j = 0; j < array.Length; j++)
					{
						if (4 < array[j].Length)
						{
							result = 1;
							connectPaymemtMoneyResponse_.Add(Encoding.UTF8.GetString(array[j]));
						}
						else if (array[j].Length != 0)
						{
							if (array[j].Length == 1)
							{
								throw new Exception();
							}
							result = EncodeUtil.ByteAryToInt(array[j], true);
						}
					}
				}
				catch (Exception t)
				{
					if (Config.DEBUG)
					{
						Log.Info(t);
					}
					result = -1;
					goto IL_0304;
				}
				break;
				IL_0304:
				JThread.Sleep(100L);
			}
			while (num2 < num);
			return result;
		}

		private string GetPaymentHost()
		{
			if (!Config.DEBUG && accountType_ == 1)
			{
				return "http://virtualCurrency.kairogame.jp/";
			}
			return "http://kairopark.kir.jp/";
		}

		public string ConnectMigrationStart(byte[] data)
		{
			return JString.New(Connect("migration.php" + MakeQuery("mode", "start"), data), "UTF-8");
		}

		public byte[] ConnectMigrationRead(string pw)
		{
			return Connect("migration.php" + MakeQuery("mode", "read") + MakeQuery("pw", pw));
		}

		public int ConnectMigrationComplete(string pw)
		{
			return int.Parse(JString.New(Connect("migration.php" + MakeQuery("mode", "complete") + MakeQuery("pw", pw))));
		}

		public int ConnectMigrationCancel(string pw)
		{
			return int.Parse(JString.New(Connect("migration.php" + MakeQuery("mode", "cancel") + MakeQuery("pw", pw))));
		}

		public int ConnectMigrationState(string pw)
		{
			return int.Parse(JString.New(Connect("migration.php" + MakeQuery("mode", "state") + MakeQuery("pw", pw))));
		}

		public byte[] ConnectAppList(int localFileDate)
		{
			byte[] array = Connect(GetCommonServer() + "com/cgi/appli.php?mode=update&date=" + localFileDate);
			if (array == null || array.Length == 0)
			{
				return null;
			}
			return array;
		}

		private void ConnectNotificationRegist(string gcmId)
		{
			string cgi = "notification.php" + MakeQuery("friendid", GetFriendId()) + MakeQuery("mode", "regist") + MakeQuery("gcmid", gcmId);
			HttpConnect(MakeUrl(cgi, null), null, true, false);
		}

		public void ConnectNotificationSend(string friendId, string text, string key = null)
		{
			NotifyManager instance = NotifyManager.GetInstance();
			if (instance != null && !instance.IsPushEnable())
			{
				Log.Info("プッシュ通知が無効になっています。KairoBase.SetPushNotification で有効にしてください。");
				return;
			}
			Connect("notification.php" + MakeQuery("friendid", GetFriendId()) + MakeQuery("mode", "send") + MakeQuery("to_friendid", friendId) + MakeQuery("key", key), Encoding.UTF8.GetBytes(text));
		}

		public bool ConnectLangPackCheck(string file, int ver, int hash)
		{
			return int.Parse(JString.New(Connect(GetCommonServer() + "com/cgi/langpack.php" + MakeQuery("mode", "check") + MakeQuery("file", file) + MakeQuery("ver", ver) + MakeQuery("hash", hash)))) == 1;
		}

		public void ConnectLangPackSet(string file, int ver, int hash, string data)
		{
			Connect(GetCommonServer() + "com/cgi/langpack.php" + MakeQuery("mode", "set") + MakeQuery("file", file) + MakeQuery("ver", ver) + MakeQuery("hash", hash), Encoding.UTF8.GetBytes(data));
		}

		public void ConnectLangPackWeb(string file)
		{
			kairo.unity.ui.Canvas.OpenBrowser(GetCommonServer() + "com/cgi/langpack.php?file=" + file.Replace(" ", "").Replace("\u3000", ""));
		}

		public int GetArgumentsNum()
		{
			return arguments_.Length;
		}

		public string GetArgument(int index)
		{
			if (0 <= index && index < arguments_.Length)
			{
				return arguments_[index];
			}
			return "";
		}

		public int GetArgumentInt(int index)
		{
			if (0 <= index && index < argumentsInt_.Length)
			{
				return argumentsInt_[index];
			}
			return 0;
		}

		private void LoadAppList()
		{
			for (int i = 0; i < 2; i++)
			{
				try
				{
					byte[] array = null;
					if (i == 0)
					{
						array = Storage.Read(3, true, "applist.txt");
					}
					else
					{
						array = Storage.Read(1, true, "app/list.bin");
						if (array != null)
						{
							Encrypter.Decode(array);
						}
					}
					if (array != null)
					{
						int date;
						GameAppli[] array2 = LoadAppList(array, out date);
						if (appListDate_ < date)
						{
							appListDate_ = date;
							appList_ = array2;
						}
					}
				}
				catch (Exception t)
				{
					Log.Info(t);
				}
			}
		}

		private GameAppli[] LoadAppList(byte[] data, out int date)
		{
			date = -1;
			if (data.Length == 0)
			{
				return new GameAppli[0];
			}
			string text = null;
			ByteArrayInputStream byteArrayInputStream = new ByteArrayInputStream(data);
			try
			{
				date = StreamUtil.ReadInt(byteArrayInputStream);
				text = StreamUtil.ReadString(byteArrayInputStream);
			}
			finally
			{
				byteArrayInputStream?.Close();
			}
			List<GameAppli> list = new List<GameAppli>();
			int[] array = null;
			string[] array2 = text.Split('\n');
			for (int i = 0; i < array2.Length; i++)
			{
				string[] array3 = array2[i].Split(',');
				if (array == null)
				{
					array = new int[50];
					for (int j = 0; j < array.Length; j++)
					{
						array[j] = -1;
					}
					for (int k = 0; k < array3.Length; k++)
					{
						array[int.Parse(array3[k])] = k;
					}
					continue;
				}
				GameAppli item = default(GameAppli);
				if (array[0] != -1)
				{
					item.Pid = int.Parse(array3[array[0]]);
				}
				if (array[1] != -1)
				{
					item.Language = array3[array[1]].ToLower().Trim();
				}
				if (array[2] != -1)
				{
					item.Name = array3[array[2]];
				}
				long num = -1L;
				if (array[10] != -1)
				{
					item.AndPkg = array3[array[10]];
				}
				if (array[11] != -1)
				{
					item.AndPrice = int.Parse(array3[array[11]]);
				}
				if (array[12] != -1)
				{
					num = long.Parse(array3[array[12]]);
					if (num != -1 && num < 0)
					{
						num = 0L;
					}
				}
				item.AndDate = (int)num;
				num = -1L;
				if (array[23] != -1)
				{
					item.IOSPkg = array3[array[23]];
				}
				if (array[20] != -1)
				{
					item.IOSId = array3[array[20]];
				}
				if (array[21] != -1)
				{
					item.IOSPrice = int.Parse(array3[array[21]]);
				}
				if (array[22] != -1)
				{
					num = long.Parse(array3[array[22]]);
					if (num != -1 && num < 0)
					{
						num = 0L;
					}
				}
				item.IOSDate = (int)num;
				list.Add(item);
			}
			return list.ToArray();
		}

		public GameAppli[] GetAppList(bool languageOnly = true, bool releaseOnly = true)
		{
			if (appList_.Length == 0)
			{
				LoadAppList();
			}
			List<GameAppli> list = new List<GameAppli>();
			GameAppli[] array = appList_;
			for (int i = 0; i < array.Length; i++)
			{
				GameAppli item = array[i];
				if ((!languageOnly || item.Language.Length <= 0 || ((!Language.Japanese() || !(item.Language != "ja")) && (!Language.English() || !(item.Language != "en")))) && (!releaseOnly || ((Config.APPLI_TYPE != 2 || item.IsReleaseAndroid()) && (Config.APPLI_TYPE != 3 || item.IsReleaseIOS()))))
				{
					list.Add(item);
				}
			}
			return list.ToArray();
		}

		public ConnectObject ReadyConnection()
		{
			return new ConnectObject(this);
		}

		private byte[] HttpConnect(string url, byte[] send, bool check)
		{
			if (connects_.Count <= 0)
			{
				return Http.Connect(url, send, check);
			}
			return connects_.Peek().Connect(url, send, check);
		}

		private byte[][] HttpConnect(string url, byte[] send, bool check, bool split)
		{
			if (connects_.Count <= 0)
			{
				return Http.Connect(url, send, check, split);
			}
			return connects_.Peek().Connect(url, send, check, split);
		}

		public ITapjoy GetTapjoy()
		{
			return (ITapjoy)IApplication.GetCurrentApp().RunOnUiThread(() => GameObject.Find("Main").GetComponent<ITapjoy>(), true);
		}

		internal void SetGameState(bool playing)
		{
			if (this.isPlaying_ != playing)
			{
				this.isPlaying_ = playing;
				NotifyManager instance = NotifyManager.GetInstance();
				if (instance != null)
				{
					if (playing)
					{
						instance.DoCancel();
						return;
					}
					instance.DoNotify();
				}
			}
		}

		public bool IsGamePlaying()
		{
			return isPlaying_;
		}

		public int GetStartProgress()
		{
			return (prgStart_[0] + prgStart_[1]) / 2;
		}

		protected void SetOnStartProgress(int progress)
		{
			if (progress > 100)
			{
				progress = 100;
			}
			if (prgStart_[1] < progress)
			{
				prgStart_[1] = progress;
			}
		}

		private void SetSystemStartProgress(int progress)
		{
			if (progress > 100)
			{
				progress = 100;
			}
			if (prgStart_[0] < progress)
			{
				prgStart_[0] = progress;
			}
		}

		private string GetCommonServer()
		{
			if (COMMON_SERVER == null)
			{
				COMMON_SERVER = (Config.DEBUG ? "https://applidev.kairogame.jp/" : "https://appli.kairogame.jp/");
			}
			return COMMON_SERVER;
		}
	}
}
