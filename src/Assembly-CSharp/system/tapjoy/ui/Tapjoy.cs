using kairo.unity.system.tapjoy.ui;
using kairo.unity.util;
using UnityEngine;

namespace system.tapjoy.ui
{
	public abstract class Tapjoy : MonoBehaviour, ITapjoy
	{
		public enum OfferState
		{
			None,
			Start,
			Failed,
			Disp
		}

		private const string PREFS_KEY = "_Tapjoy";

		protected static Tapjoy instance_;

		private string key_;

		private string offerwallName_;

		private TJPlacement offerwallPlacement_;

		private bool contentIsReadyForPlacement_;

		private bool initialize_;

		private bool backupMute_;

		private int balance_;

		private bool log_;

		private OfferState offerState_;

		protected Tapjoy(string key, string offerwallName, bool log = true)
		{
			instance_ = this;
			instance_ = this;
			key_ = key;
			offerwallName_ = offerwallName;
			log_ = log;
		}

		private void Start()
		{
		}

		private void OnApplicationQuit()
		{
		}

		public static Tapjoy GetInstance()
		{
			return instance_;
		}

		protected void Init()
		{
			if (initialize_) { return; }
{
Print("[Tapjoy] Init");
initialize_ = true;
balance_ = PlayerPrefs.GetInt("_Tapjoy", 0);
Print("[Tapjoy] Init_balance:" + balance_);
}
		}

		protected void Quit()
		{
			if (!(initialize_)) { return; }
{

Print("[Tapjoy] Quit");
}
		}

		public void CallBalance()
		{
			Print("[Tapjoy] CallBalance");
		}

		public bool ShowOffers()
		{
			offerState_ = OfferState.Failed;
			return false;
		}

		public OfferState GetOfferState()
		{
			return offerState_;
		}

		public int GetBalance()
		{
			return balance_;
		}

		public void AddBalance(int add)
		{
			balance_ += add;
			Print("[Tapjoy] AddBalance:" + balance_);
			PlayerPrefs.SetInt("_Tapjoy", balance_);
			PlayerPrefs.Save();
		}

		public void SubBalance(int sub)
		{
			balance_ -= sub;
			if (balance_ <= 0)
			{
				balance_ = 0;
			}
			Print("[Tapjoy] SubBalance:" + balance_);
			PlayerPrefs.SetInt("_Tapjoy", balance_);
			PlayerPrefs.Save();
		}

		protected void Print(string s)
		{
			if (log_)
			{
				Log.Info(s);
			}
		}
	}
}
