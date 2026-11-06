using System;
using form;
using java.lang;
using java.util;
using kairo.common.cfg;
using kairo.unity.form;
using kairo.unity.io;
using kairo.unity.surface;
using kairo.unity.ui;
using kairo.unity.util;
using main;
using surface;
using system.billing;
using system.billing.ui;

namespace system.form
{
	public class BillingForm : MyFormBase, JRunnable
	{
		public const int FORMID_BILLING = 900;

		public const int TYPE_MARKET_NOTES = 0;

		public const int TYPE_HTTP_CONNECT_ERR = 1;

		public const int TYPE_SHOP = 2;

		public const int TYPE_BILLING_ITEM = 3;

		public const int TYPE_RECEIVED_MARKET = 4;

		public const int TYPE_SHOP_CHOICE = 5;

		private const int SHOW_SALE_H = 6;

		private const int THTYPE_GETLIST = 0;

		private const int THTYPE_PURCHASE = 1;

		private const int THTYPE_GIFT = 2;

		private const int THSTATE_NONE = 0;

		private const int THSTATE_RUN = 1;

		private const int THSTATE_SUCCESS = 2;

		private const int THSTATE_FAILURE = 3;

		internal const int RESBILLING_SEB_OWNER_L = 0;

		internal const int RESBILLING_SEB_OWNER_R = 1;

		internal const int RESBILLING_SEB_TICKET = 2;

		internal const int RESBILLING_SEB_TALK_ASSIST00R = 3;

		internal const int RESBILLING_SEB_TALK_ASSIST01R = 4;

		internal const int RESBILLING_SEB_FOUNDATION = 5;

		internal const int RESBILLING_SEB_JUMP_ASSIST00 = 6;

		internal const int RESBILLING_SEB_JUMP_ASSIST01 = 7;

		internal const int RESBILLING_SEB_GET = 8;

		internal const int RESBILLING_SEB_EFFECT00 = 9;

		internal const int RESBILLING_SEB_AD_OFF = 10;

		public int type_;

		private static readonly int[][] configData_ = new int[6][]
		{
			new int[3] { 0, 2, 13 },
			new int[3] { 0, 2, 13 },
			new int[3] { 0, 2, 13 },
			new int[3] { 0, 2, 13 },
			new int[3] { 0, 2, 13 },
			new int[3] { 0, 2, 13 }
		};

		private int scrollValue_;

		public bool untilActiveHide_;

		private int select_;

		private int select2_;

		private int x_;

		private int y_;

		private int loopCounter_;

		private int frame_;

		private int page_;

		private int mode_;

		private TextLayout textLay_;

		public int value1_;

		public int value2_;

		public int value3_;

		private BillingForm bform_;

		public ApplyTask task_;

		public BillingItem item_;

		public BillingItem[] sales_;

		public string[] marketNotes_;

		private bool skipUpdateSales_;

		private string password_;

		public static bool admobSkipPurchase_;

		private static int MAKET_ITEM_GET_END_FRAME = 25;

		private static int MAKET_ITEM_GET_END_FRAME2 = 50;

		public int dialogId_ = -1;

		private int threadType_;

		private int threadState_;

		private long threadBeginTime_;

		private int threadFrame_;

		private string error_;

		private JThread th_;

		private string[] gift_;

		internal static ResourceManager resBilling_;

		private static int[] drawAdOffButtoan_result = new int[4];

		public BillingForm()
			: base(900)
		{
			untilActiveHide_ = true;
		}

		public BillingForm(int type)
			: base(900)
		{
			type_ = type;
			select_ = 0;
			untilActiveHide_ = true;
		}

		public BillingForm(int type, ApplyTask provide)
			: this(type)
		{
			task_ = provide;
		}

		public override string ToString()
		{
			return "BillingForm:" + type_;
		}

		internal static void LoadResource()
		{
			if (resBilling_ != null)
			{
				return;
			}
			try
			{
				resBilling_ = new ResourceManager(null, null, null, null, null, null, null).SetRecordNameBits(0);
				resBilling_.Load(Storage.Read(3, false, "billing.dat"));
			}
			catch (Exception)
			{
				resBilling_ = null;
			}
		}

		public override bool Init()
		{
			base.Init();
			LoadResource();
			frame_ = 0;
			threadState_ = 0;
			Vector vector = new Vector();
			int num = configData_[type_][0];
			int num2 = configData_[type_][1];
			SetSoftLabel(AppData.softLabels_[num], AppData.softLabels_[num2]);
			if (type_ == 0)
			{
				if (Config.APPLI_TYPE == 2)
				{
					string url;
if (Language.Japanese()) { url = ("http://kairopark.jp/android/policy/app_gm" + Config.PID + "_kiyaku.html"); } else { url = ("http://kairopark.jp/android/policy/en/app_gm" + Config.PID + "_kiyaku.html"); }
					if (!Canvas.OpenBrowser(url))
					{
						Push(SubForm.MakeDialog(KairoText.LT("通信に失敗しました")));
					}
					return Pop();
				}
				textLay_ = new TextLayout();
				textLay_.SetSize(146f, 124f);
				textLay_.SetLineSpace(6);
				textLay_.SetColor(Graphics.GetColorOfRGB(30, 30, 30));
				textLay_.SetText(marketNotes_[page_]);
				textLay_.Flush();
			}
			else if (type_ == 1)
			{
				string text;
if (Config.APPLI_TYPE == 0) { text = "通信を許可し電波を確認して再起動してください。\nおまかせロック等をご利用の方は解除をご確認ください。\nｷｯｽﾞiﾓｰﾄﾞﾌｨﾙﾀ設定中は一部機能がご利用頂けません。"; } else { text = KairoText.LT("通信を許可し電波を確認して再起動してください。\n通信OFFの場合は一部の機能がご利用頂けない場合がございます。"); }
				textLay_ = new TextLayout();
				textLay_.SetSize(166f, 124f);
				textLay_.SetLineSpace(6);
				textLay_.SetColor(Graphics.GetColorOfRGB(30, 30, 30));
				textLay_.SetText(text);
				textLay_.Flush();
			}
			else if (type_ == 2) { } else {
				if (type_ == 3)
				{
					KairoService instance = KairoService.GetInstance();
					MyBilling instance2 = MyBilling.GetInstance();
					if (instance2.CheckPlayingGame())
					{
						if (instance.Check(1048576))
						{
							ThreadStart(1);
							SetSoftlabel();
							return true;
						}
						instance.On(1048576);
						instance.Save();
					}
					if (item_.GetId() == 0 && admobSkipPurchase_)
					{
						ThreadStart(1);
					}
					else
					{
						if (!skipUpdateSales_)
						{
							ThreadStart(0);
						}
						select_ = 1;
					}
				}
				else if (type_ != 4)
				{
				}
			}
			SetSoftlabel();
			return true;
		}

		public override void Draw(Graphics g)
		{
			base.Draw(g);
			int num = x_;
			int num2 = y_;
			if (formMan_.GetForm(0) == apdat_.frmGame_)
			{
				num2 += configData_[type_][2];
			}
			if (view_.GetGameWidth() > 240 || view_.GetGameHeight() > 240)
			{
				num += (view_.GetGameWidth() - 240) / 2;
				num2 += (view_.GetGameHeight() - 240) / 2;
			}
			g.SetOrigin(num, num2);
			try
			{
				_draw(g);
			}
			catch (Exception)
			{
			}
			finally
			{
				g.SetOrigin(0, 0);
			}
		}

		private void _draw(Graphics g)
		{
			if (untilActiveHide_)
			{
				return;
			}
			if (!apdat_.IsTopForm(this))
			{
				FormBase topForm = formMan_.GetTopForm();
				if (topForm.GetId() != 4)
				{
					return;
				}
				SubForm subForm = (SubForm)topForm;
				if (!subForm.IsMenu() && subForm.type_ != type_)
				{
					return;
				}
				for (int num = formMan_.GetFormsNum() - 2; num >= 0; num--)
				{
					topForm = formMan_.GetForm(num);
					if (topForm == this)
					{
						break;
					}
					if (topForm.GetId() != 4 || !((SubForm)topForm).IsMenu())
					{
						return;
					}
				}
			}
			if (!Active())
			{
				return;
			}
			if (threadState_ != 0)
			{
				if (threadFrame_ == 1)
				{
					FormManager instance = FormManager.GetInstance();
					instance.DrawAllForms();
				}
				else if (threadFrame_ > 1)
				{
					ConnectForm.DrawConnect(g);
				}
				if (threadFrame_ <= 1)
				{
					try
					{
						JThread.Yield();
						JThread.Sleep(50L);
					}
					catch (Exception)
					{
					}
				}
			}
			else if (type_ == 0)
			{
				int num2 = MyFormBase.WNDFLAG_LEFT_ARW | MyFormBase.WNDFLAG_RIGHT_ARW;
				apdat_.DrawBox(g, 37, 55, 203, 202);
				textLay_.Draw(g, 48f, 70f);
			}
			else if (type_ == 1)
			{
				int gWidth = 190;
				int gHeight = 180;
				apdat_.DrawWindow(g, gWidth, gHeight, 0, KairoText.LT("メッセージ"));
				apdat_.DrawBox(g, 34, 54, 206, 181);
				textLay_.Draw(g, 40f, 68f);
				DrawCommand(g, KairoText.LT("了解"), 120, 189, 2, true, true);
				GameView.AddTouch(4);
			}
			else
			{
				if (type_ == 2 || type_ == 3 || type_ == 4 || type_ != 5)
				{
					return;
				}
				int num3 = 174;
				int num4 = 107;
				int num5 = 120 - num3 / 2;
				int num6 = 120 - num4 / 2;
				int num7 = num5 + num3;
				int num8 = num6 + num4;
				FillRect3(g, num5, num6, num3, num4, Graphics.GetColorOfRGB(150, 153, 225), Graphics.GetColorOfRGB(85, 87, 171), Graphics.GetColorOfRGB(253, 253, 253));
				apdat_.DrawBox(g, num5 + 9, num6 + 24, num7 - 9, num8 - 6);
				g.SetColor(55, 55, 55);
				g.DrawString(KairoText.LT("どちらに入店しますか"), 120f, num6 + 8, 2);
				string[] array = new string[2]
				{
					KairoText.LT("パンプキン商会"),
					KairoText.LT("チケットショップ")
				};
				int num9 = num6 + 30 + 5;
				for (int i = 0; i < array.Length; i++)
				{
					g.SetColor(55, 55, 55);
					int[] array2 = DrawCommand(g, array[i], 125, num9 + 2, 2, select_ == i, 1, select_ == i);
					GameView.AddTouch(g, 3, array2[0], array2[1], array2[2], array2[3], 0x20000 | i);
					g.SetClip(0f, 0f, view_.GetGameWidth(), num9 + 16);
					if (i == 0)
					{
						resBilling_.DrawSeb(g, num5 + 22, num9 + 24, 1, 0);
					}
					else
					{
						resBilling_.DrawSeb(g, num5 + 22, num9 + 23, 1, 1);
					}
					g.ClearClip();
					num9 += 37;
				}
			}
		}

		public override bool Finish()
		{
			base.Finish();
			if (type_ == 3 && item_.GetId() == 0)
			{
				admobSkipPurchase_ = true;
			}
			return true;
		}

		public override bool Resume()
		{
			base.Resume();
			return true;
		}

		public override bool Update()
		{
			base.Update();
			bool result = _update();
			SetSoftlabel();
			return result;
		}

		private bool _update()
		{
			KairoService instance = KairoService.GetInstance();
			untilActiveHide_ = false;
			if (threadState_ != 0)
			{
				if (threadState_ != 1 && threadType_ == 0)
				{
					long num = 350 - (JSystem.CurrentTimeMillis() - threadBeginTime_);
					if (num > 0)
					{
						JThread.Sleep(num);
					}
				}
				if (threadState_ == 1)
				{
					threadFrame_++;
					return true;
				}
				if (threadState_ == 2)
				{
					threadState_ = 0;
					if (threadType_ == 0)
					{
						if (type_ == 2)
						{
							UpdateSaleList();
						}
					}
					else
					{
						if (threadType_ == 1)
						{
							return Pop();
						}
						if (threadType_ == 2)
						{
							BillingManager instance2 = BillingManager.GetInstance();
							for (int num2 = gift_.Length - 1; num2 >= 0; num2--)
							{
								ApplyTask applyTask = new ApplyTask(gift_[num2]);
								if (applyTask.Enable())
								{
									instance2.AddApplyTask(applyTask);
								}
							}
							instance.SaveAll();
							instance2.UpdateApplyTask(true);
							return true;
						}
					}
				}
				else if (threadState_ == 3)
				{
					threadState_ = 0;
					if (Config.PRINT)
					{
						Log.Info(error_);
					}
					if (threadType_ == 0 && type_ == 2 && !instance.Check(65536))
					{
						Push(SubForm.MakeTalkForm(4, KairoText.LT("おっと…<br>本部との<co=60,100,200>通信に失敗</co>してしまいました。"), string.Empty));
						Push(SubForm.MakeTalkForm(4, KairoText.LT("電波や通信の状態を確認して<br>もう一回お店に来ると…"), string.Empty));
						Push(SubForm.MakeTalkForm(4, KairoText.LT("本来のステキな商品が陳列されますよ。"), string.Empty));
						Push(SubForm.MakeTalkForm(4, KairoText.LT("というわけで…<br>またお待ちしています。"), string.Empty));
						instance.On(65536);
						return Pop();
					}
					if (threadType_ == 2 && Http.PopErrorCode() == 210)
					{
						return Push(SubForm.MakeDialog(Http.PopErrorText()));
					}
					if (threadType_ == 0)
					{
						Pop();
					}
					if (threadType_ == 1)
					{
						return Pop();
					}
					return Push(new BillingForm(1));
				}
			}
			int num3 = 0;
			if (canvas_.CheckKeyState(131072))
			{
				num3++;
			}
			if (canvas_.CheckKeyState(524288))
			{
				num3++;
			}
			if (canvas_.CheckKeyState(65536))
			{
				num3++;
			}
			if (canvas_.CheckKeyState(262144))
			{
				num3++;
			}
			if (num3 > 1)
			{
				canvas_.ClearKeyBuffer();
			}
			loopCounter_ = (loopCounter_ + 1) % int.MaxValue;
			if (type_ == 0)
			{
				frame_ = (frame_ + 1) % int.MaxValue;
				int num4 = page_;
				if (canvas_.CheckKeyPulse(1048576) || canvas_.CheckKeyRepeat(262144))
				{
					page_ = (page_ + 1) % marketNotes_.Length;
				}
				if (canvas_.CheckKeyRepeat(65536))
				{
					page_ = (page_ - 1 + marketNotes_.Length) % marketNotes_.Length;
				}
				if (num4 != page_)
				{
					textLay_.SetText(marketNotes_[page_]);
					textLay_.Flush();
				}
				if (apdat_.IsPushSoftLabel(2, 1048576))
				{
					return Pop();
				}
			}
			else if (type_ == 1)
			{
				if (canvas_.CheckKeyPulse(1048576) || apdat_.IsPushSoftLabel(2))
				{
					return Pop();
				}
			}
			else if (type_ == 2)
			{
				if (bform_ != null)
				{
					UpdateSaleList();
					bform_ = null;
				}
				if (mode_ == 0)
				{
					mode_++;
					if (!instance.Check(131072))
					{
						Push(SubForm.MakeTalkForm(4, KairoText.LT("ここは、世界的に見て超レアな<br>｢チケット｣を売るお店ですよ。"), string.Empty));
						Push(SubForm.MakeTalkForm(4, KairoText.LT("なかなかの金額だからよく考えて買っていってくださいね。"), string.Empty));
						Push(SubForm.MakeTalkForm(4, KairoText.LT("フフフ。"), string.Empty));
						instance.On(131072);
						try
						{
							instance.Save();
						}
						catch (Exception)
						{
						}
					}
					else
					{
						Push(SubForm.MakeTalkForm(4, KairoText.LT("フフフ<br>いらっしゃい。"), string.Empty));
					}
					return true;
				}
				if (mode_ == 1)
				{
					mode_++;
					if (!skipUpdateSales_)
					{
						ThreadStart(0);
						return true;
					}
				}
				if (sales_ != null)
				{
					ScrollControl(sales_.Length, 6);
					if (canvas_.CheckKeyPulse(1048576))
					{
						if (select_ < sales_.Length)
						{
							BillingItem billingItem = sales_[select_];
							if (billingItem.GetState() == 1)
							{
								return Push(SubForm.MakeTalkForm(4, KairoText.LT("あー、残念ながら<br>そちらは<co=60,100,200>売り切れ</co>です。<br>他の商品もご覧下さいませ。"), JString.ValueOf(999)));
							}
							if (!instance.Check(524288))
							{
								Push(SubForm.MakeTalkForm(4, KairoText.LT("おや、お目が高いですね。"), string.Empty));
								Push(SubForm.MakeTalkForm(4, KairoText.LT("こちらは<co=60,100,200>現実のお金で購入する</co>ステキな商品です。"), string.Empty));
								Push(SubForm.MakeTalkForm(4, KairoText.LT("ちびっこは、<co=60,100,200>保護者の方</co>に確認してから購入するようにして下さいね。フフフ。"), string.Empty));
								instance.On(524288);
								try
								{
									instance.Save();
								}
								catch (Exception)
								{
								}
							}
							bform_ = new BillingForm(3);
							bform_.item_ = billingItem;
							bform_.skipUpdateSales_ = true;
							return Push(bform_);
						}
					}
					else if (apdat_.IsPushSoftLabel(2))
					{
						if (!instance.Check(262144))
						{
							Push(SubForm.MakeTalkForm(4, KairoText.LT("これからいつでもお店はありますからいつでも来て下さいね。"), string.Empty));
							instance.On(262144);
							try
							{
								instance.Save();
							}
							catch (Exception)
							{
							}
						}
						else
						{
							Push(SubForm.MakeTalkForm(4, KairoText.LT("フフフ<br>また来てくださいね。"), string.Empty));
						}
						return Pop();
					}
				}
				if (Config.DEBUG && canvas_.CheckKeyPulse(2048))
				{
					MyBilling instance3 = MyBilling.GetInstance();
					if (select2_ == 0)
					{
						Vector purchaseLog = instance3.GetPurchaseLog();
						string text = string.Empty;
						for (int i = 0; i < purchaseLog.Size(); i++)
						{
							PurchaseRecord purchaseRecord = (PurchaseRecord)purchaseLog.ElementAt(i);
							if (text.Length() > 0)
							{
								text += "\n\n";
							}
							text += purchaseRecord;
						}
						PurchaseLogView purchaseLogView = new PurchaseLogView("過去の購入履歴");
						purchaseLogView.SetLog(purchaseLog.Size(), text, true);
						purchaseLogView.Show();
					}
					else
					{
						BillingItem[] items = instance3.GetItems();
						string text2 = string.Empty;
						int num5 = 0;
						for (int j = 0; j < items.Length; j++)
						{
							if ((items[j] == null) || (items[j].GetState() != 1)) { continue; }
if (items[j].GetPurchase() == null) { continue; }
if (text2.Length() > 0)
								{
									text2 += "\n\n";
								}
text2 = text2 + items[j].GetProductId() + "\n" + items[j].GetPurchase().GetOrderId();
num5++;
						}
						PurchaseLogView purchaseLogView2 = new PurchaseLogView("現在の購入状態");
						purchaseLogView2.SetLog(num5, text2, false);
						purchaseLogView2.Show();
					}
				}
			}
			else if (type_ == 3)
			{
				if (canvas_.CheckKeyRepeat(131072))
				{
					select_ = (select_ - 1 + 2) % 2;
				}
				else if (canvas_.CheckKeyRepeat(524288))
				{
					select_ = (select_ + 1 + 2) % 2;
				}
				if (canvas_.CheckKeyPulse(1048576))
				{
					if (select_ == 0)
					{
						return Push(new BillingForm(0));
					}
					if (select_ == 1)
					{
						ThreadStart(1);
						return true;
					}
				}
				else if (apdat_.IsPushSoftLabel(2))
				{
					return Pop();
				}
			}
			else if (type_ != 4 && type_ == 5)
			{
				int num6 = 2;
				if (canvas_.CheckKeyRepeat(131072))
				{
					select_ = (select_ - 1 + num6) % num6;
				}
				else if (canvas_.CheckKeyRepeat(524288))
				{
					select_ = (select_ + 1 + num6) % num6;
				}
				if (canvas_.CheckKeyPulse(1048576))
				{
					return Pop();
				}
				if (apdat_.IsPushSoftLabel(2))
				{
					return Pop();
				}
			}
			return true;
		}

		private void ScrollControl(int num, int showMax)
		{
			if (num <= 0) { return; }
{
if (canvas_.CheckKeyRepeat(131072))
				{
					select_ = (select_ - 1 + num) % num;
				}
				else if (canvas_.CheckKeyRepeat(524288))
				{
					select_ = (select_ + 1) % num;
				}
ScrollUpdate(num, showMax);
}
		}

		public void ScrollUpdate(int num, int showMax)
		{
			ScrollUpdate(num, showMax, select_);
		}

		public void ScrollUpdate(int num, int showMax, int value)
		{
			if (num <= 0) { return; }
{
if (value >= scrollValue_ + showMax)
				{
					scrollValue_ = value - (showMax - 1);
				}
if (value < scrollValue_)
				{
					scrollValue_ = value;
				}
while (scrollValue_ > 0 && num < scrollValue_ + showMax)
				{
					scrollValue_--;
				}
}
		}

		public int[] DrawCommand(Graphics g, string str, int x, int y, bool active, bool cursor)
		{
			return DrawCommand(g, str, x, y, 0, active, 1, cursor);
		}

		public int[] DrawCommand(Graphics g, string str, int x, int y, int anchor, bool active, bool cursor)
		{
			return DrawCommand(g, str, x, y, anchor, active, 1, cursor);
		}

		public int[] DrawCommand(Graphics g, string str, int x, int y, int anchor, bool active, int margin, bool cursor)
		{
			return null;
		}

		public override int GetTouchValue(int type, int index)
		{
			type = (int)(type & 0xFFFF0000u);
			switch (type)
			{
			case 131072:
				return select_;
			case 262144:
				return scrollValue_;
			default:
				return base.GetTouchValue(type, index);
			}
		}

		public override void SetTouchValue(int type, int value)
		{
			type = (int)(type & 0xFFFF0000u);
			switch (type)
			{
			case 131072:
				select_ = value;
				break;
			case 262144:
				scrollValue_ = value;
				break;
			default:
				base.SetTouchValue(type, value);
				break;
			}
		}

		public override bool OnTouchEvent(TouchEvent e)
		{
			TouchComponent component = e.component;
			if (type_ == 2 && component.id == 4)
			{
				int type = e.type;
				if ((type != 1) && (type != 2)) { } else {
					select2_ = component.value;
					if (e.type == 1)
					{
						canvas_.KeyClick(2048);
					}
				}
				return false;
			}
			return true;
		}

		public override bool CheckMainWindow()
		{
			return false;
		}

		public void SetSoftlabel()
		{
			int num = configData_[type_][0];
			int num2 = configData_[type_][1];
			if (threadState_ != 0)
			{
				num = (num2 = 0);
			}
			SetSoftLabel(AppData.softLabels_[num], AppData.softLabels_[num2]);
		}

		public void UpdateSaleList()
		{
			MyBilling instance = MyBilling.GetInstance();
			BillingItem[] sales = instance.GetSales();
			int[] array = new int[sales.Length];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = -1;
			}
			int num = 0;
			for (int j = 0; j < sales.Length; j++)
			{
				if (array[j] != -1)
				{
					continue;
				}
				BillingItem billingItem = sales[j];
				if (billingItem.GetSeriesNumber() != -1)
				{
					for (int k = j + 1; k < sales.Length; k++)
					{
						if (sales[k].GetSeriesNumber() != -1 && sales[j].GetSeriesName().Equals(sales[k].GetSeriesName()))
						{
							array[k] = num;
						}
					}
				}
				array[j] = num;
				num++;
			}
			Vector vector = new Vector();
			for (int l = 0; l < num; l++)
			{
				BillingItem billingItem2 = null;
				BillingItem billingItem3 = null;
				int num2 = int.MaxValue;
				int num3 = int.MinValue;
				for (int m = 0; m < array.Length; m++)
				{
					if (array[m] != l) { continue; }
BillingItem billingItem4 = sales[m];
if (billingItem4.GetState() == 0 && billingItem4.GetSeriesNumber() < num2)
						{
							num2 = billingItem4.GetSeriesNumber();
							billingItem2 = billingItem4;
						}
if (billingItem4.GetState() != 0 && billingItem4.GetSeriesNumber() > num3)
						{
							num3 = billingItem4.GetSeriesNumber();
							billingItem3 = billingItem4;
						}
				}
				if (billingItem2 != null)
				{
					vector.AddElement(billingItem2);
				}
				else if (billingItem3 != null)
				{
					vector.AddElement(billingItem3);
				}
			}
			sales_ = new BillingItem[vector.Size()];
			for (int n = 0; n < sales_.Length; n++)
			{
				sales_[n] = (BillingItem)vector.ElementAt(n);
			}
			if (select_ < 0)
			{
				select_ = 0;
			}
			if (select_ > sales_.Length - 1)
			{
				select_ = sales_.Length - 1;
			}
			ScrollUpdate(sales_.Length, 6, select_);
		}

		public void ThreadStart(int type)
		{
			threadType_ = type;
			threadState_ = 1;
			error_ = null;
			threadFrame_ = 0;
			th_ = new JThread(this);
			th_.Start();
		}

		public void Run()
		{
			threadBeginTime_ = JSystem.CurrentTimeMillis();
			try
			{
				KairoService instance = KairoService.GetInstance();
				MyBilling instance2 = MyBilling.GetInstance();
				if (threadType_ == 0)
				{
					instance2.UpdateBillingItems(true);
					if (type_ == 2)
					{
						try
						{
							instance2.UpdateApplyTask(true);
						}
						catch (Exception)
						{
						}
					}
				}
				else if (threadType_ == 1)
				{
					instance2.LaunchPurchaseFlow(item_);
				}
				else if (threadType_ == 2)
				{
					gift_ = instance.ConnectGift(password_);
				}
				threadState_ = 2;
			}
			catch (Exception ex2)
			{
				error_ = ex2.ToString();
				threadState_ = 3;
				th_ = null;
			}
			finally
			{
				th_ = null;
			}
		}

		public static void DrawLine(Graphics g, int x1, int y1, int x2, int y2, int color)
		{
			g.SetColor(color);
			g.DrawLine(x1, y1, x2, y2);
		}

		public static void DrawDotLine(Graphics g, int x1, int y1, int x2, int y2, int color)
		{
			g.SetColor(color);
			for (int i = x1; i < x2; i += 3)
			{
				g.DrawLine(i, y1, i + 1, y2);
			}
		}

		public static void DrawRect(Graphics g, int x, int y, int width, int height, int color)
		{
			g.SetColor(color);
			g.DrawRect(x, y, width, height);
		}

		public static void DrawRect2(Graphics g, int x, int y, int width, int height, int color1, int color2)
		{
			g.SetColor(color1);
			g.DrawRect(x, y, width, height);
			g.SetColor(color2);
			g.DrawRect(x + 1, y + 1, width - 2, height - 2);
		}

		public static void DrawRect3(Graphics g, int x, int y, int width, int height, int color1, int color2, int color3)
		{
			g.SetColor(color1);
			g.DrawRect(x, y, width, height);
			g.SetColor(color2);
			g.DrawRect(x + 1, y + 1, width - 2, height - 2);
			g.SetColor(color3);
			g.DrawRect(x + 2, y + 2, width - 4, height - 4);
		}

		public static void FillRect(Graphics g, int x, int y, int width, int height, int color)
		{
			g.SetColor(color);
			g.FillRect(x, y, width, height);
		}

		public static void FillRect2(Graphics g, int x, int y, int width, int height, int color1, int color2)
		{
			DrawRect(g, x, y, width - 1, height - 1, color1);
			FillRect(g, x + 1, y + 1, width - 2, height - 2, color2);
		}

		public static void FillRect3(Graphics g, int x, int y, int width, int height, int color1, int color2, int color3)
		{
			DrawRect(g, x, y, width - 1, height - 1, color1);
			DrawRect(g, x + 1, y + 1, width - 3, height - 3, color2);
			FillRect(g, x + 2, y + 2, width - 4, height - 4, color3);
		}

		public static void FillRect4(Graphics g, int x, int y, int width, int height, int color1, int color2, int color3, int color4)
		{
			DrawRect(g, x, y, width - 1, height - 1, color1);
			DrawRect(g, x + 1, y + 1, width - 3, height - 3, color2);
			DrawRect(g, x + 2, y + 2, width - 5, height - 5, color3);
			FillRect(g, x + 3, y + 3, width - 6, height - 6, color4);
		}

		public static void FillRectShadow(Graphics g, int x, int y, int width, int height, int color1, int color2, int color3)
		{
			DrawRect(g, x, y, width - 1, height - 1, color1);
			DrawLine(g, x + 1, y + (height - 2), x + (width - 2), y + (height - 2), color2);
			DrawLine(g, x + (width - 2), y + 1, x + (width - 2), y + (height - 2), color2);
			FillRect(g, x + 1, y + 1, width - 3, height - 3, color3);
		}

		public static void DrawObi(Graphics g, int x, int y, int width, int height, int color1, int color2)
		{
			FillRect(g, x, y, width, 1, color1);
			FillRect(g, x, y + 1, width, height - 2, color2);
			FillRect(g, x, y + height - 1, width, 1, color1);
		}

		public static void DrawGradation(Graphics g, int dx, int dy, int sx, int sy, int w, int h, int sr, int sg, int sb, int dr, int dg, int db)
		{
			if ((w <= 0) || (h <= 0)) { return; }
int dxAlias = dx;
int num = dy;
int num2 = dx + w;
int num3 = dy + h;
double num4 = (double)(dr - sr) / (double)(h / 4 + 1);
double num5 = (double)(dg - sg) / (double)(h / 4 + 1);
double num6 = (double)(db - sb) / (double)(h / 4 + 1);
double num7 = (double)sr + (double)(sy / 4) * num4;
double num8 = (double)sg + (double)(sy / 4) * num5;
double num9 = (double)sb + (double)(sy / 4) * num6;
while (num < num3)
				{
					num7 = (((num7 > 255.0)) ? (255.0) : (num7));
					num7 = (((num7 < 0.0)) ? (0.0) : (num7));
					num8 = (((num8 > 255.0)) ? (255.0) : (num8));
					num8 = (((num8 < 0.0)) ? (0.0) : (num8));
					num9 = (((num9 > 255.0)) ? (255.0) : (num9));
					num9 = (((num9 < 0.0)) ? (0.0) : (num9));
					g.SetColor(Graphics.GetColorOfRGB((int)num7, (int)num8, (int)num9));
					int num10 = 0;
					if ((num == dy && sy % 4 != 0)) { num10 = (sy % 4); } else { if ((num + 4 > num3)) { num10 = ((num3 - num)); } else { num10 = (4); } }
					g.FillRect(dxAlias, num, w, num10);
					num += num10;
					num7 += num4;
					num8 += num5;
					num9 += num6;
				}
		}

		public new static string Comma(long value)
		{
			bool flag = value < 0;
			if (flag)
			{
				value = -value;
			}
			string str = string.Empty + value;
			string text = string.Empty;
			int num = 0;
			int num2 = str.Length() - 1;
			while (num2 >= 0)
			{
				if (num > 0 && num % 3 == 0)
				{
					text = ',' + text;
				}
				text = str.CharAt(num2) + text;
				num2--;
				num++;
			}
			if (flag)
			{
				text = "-" + text;
			}
			return text;
		}

		public static int[] DrawAdOffButtoan(Graphics g, int dx, int dy, int dw, int dh, int frame)
		{
			try
			{
				LoadResource();
				int[] sprite = resBilling_.seb[10].GetSprite(frame, 0);
				g.DrawScaledImage(resBilling_.img[sprite[1]], dx, dy, dw, dh, sprite[2], sprite[3], sprite[4], sprite[5]);
				int[] array = drawAdOffButtoan_result;
				array[0] = dx;
				array[1] = dy;
				array[2] = dw;
				array[3] = dh;
				return array;
			}
			catch (Exception)
			{
				return null;
			}
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}
}
