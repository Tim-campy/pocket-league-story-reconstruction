using System;
using System.Collections.Generic;
using System.IO;
using java.io;
using java.lang;
using java.util;
using kairo.common.cfg;
using kairo.unity.io;
using kairo.unity.system;
using kairo.unity.ui;
using kairo.unity.util;
using system.billing.util;

namespace system.billing.ui
{
	public abstract class BillingManager : StoreBase
	{
		internal static BillingManager instance_;

		internal static SecureString BASE64_PUBLIC_KEY = new SecureString(string.Empty);

		private BillingItem consumeItem_;

		private bool consumeResult_;

		private BillingItem[] items_;

		private Vector itemList_;

		private BillingItem[][] itemGroup_;

		private bool confirm_;

		private bool prepareGame_;

		private Vector applyTaskList_;

		private Vector purchaseLog_;

		private bool recoveryCheck_;

		private bool bootCheckRestore_;

		private object RegistApplyTask_lock = new object();

		private object UpdateApplyTask_lock = new object();

		internal BillingManager()
		{
		}

		public static BillingManager GetInstance()
		{
			return instance_;
		}

		public void Init(string[][] data)
		{
			Init<BillingItem>(data, "payment", new string[0]);
		}

		public void Init<T>(string[][] data) where T : BillingItem, new()
		{
			Init<T>(data, "payment", new string[0]);
		}

		public override void Init(string[][] data, string paymentProduct, params string[] subscription)
		{
			Init<BillingItem>(data, paymentProduct, subscription);
		}

		public virtual void Init<T>(string[][] data, string paymentProduct, params string[] subscription) where T : BillingItem, new()
		{
			base.Init(data, paymentProduct, subscription);
			applyTaskList_ = new Vector();
			purchaseLog_ = new Vector();
			items_ = new BillingItem[1000];
			itemList_ = new Vector();
			for (int spriteIndex = 0; spriteIndex < data.Length; spriteIndex++) {
string[] array = data[spriteIndex];
int id = JInteger.ParseInt(array[0]);
int num = 0;
for (int j = 0; j < array[1].Length(); j++)
				{
					char c = array[1].CharAt(j);
					if (c == '1')
					{
						num |= 1 << (j & 0x1F);
					}
				}
BillingItem billingItem = new T();
billingItem.Init(id, num, array[2], array[3], array[4], JInteger.ParseInt(array[5]), array[6]);
items_[billingItem.GetId()] = billingItem;
itemList_.AddElement(billingItem);
}
			Vector vector = new Vector();
			bool[] array2 = new bool[itemList_.Size()];
			for (int k = 0; k < itemList_.Size(); k++)
			{
				if (array2[k])
				{
					continue;
				}
				array2[k] = true;
				BillingItem billingItem2 = (BillingItem)itemList_.ElementAt(k);
				string seriesName = billingItem2.GetSeriesName();
				if (seriesName == null)
				{
					vector.AddElement(new BillingItem[1] { billingItem2 });
					continue;
				}
				Vector vector2 = new Vector();
				for (int l = k; l < itemList_.Size(); l++)
				{
					BillingItem billingItem3 = (BillingItem)itemList_.ElementAt(l);
					if (seriesName.Equals(billingItem3.GetSeriesName()))
					{
						vector2.AddElement(billingItem3);
						array2[l] = true;
					}
				}
				BillingItem[] array3 = new BillingItem[vector2.Size()];
				for (int m = 0; m < vector2.Size(); m++)
				{
					array3[m] = (BillingItem)vector2.ElementAt(m);
				}
				for (int n = 0; n < array3.Length - 1; n++)
				{
					for (int num2 = array3.Length - 1; num2 > n; num2--)
					{
						if (array3[num2 - 1].GetSeriesNumber() > array3[num2].GetSeriesNumber())
						{
							BillingItem billingItem4 = array3[num2];
							array3[num2] = array3[num2 - 1];
							array3[num2 - 1] = billingItem4;
						}
					}
				}
				vector.AddElement(array3);
			}
			itemGroup_ = new BillingItem[vector.Size()][];
			for (int num3 = 0; num3 < vector.Size(); num3++)
			{
				itemGroup_[num3] = (BillingItem[])vector.ElementAt(num3);
			}
		}

		public virtual void Load(byte[] data)
		{
			Load<BillingItem>(data);
		}

		public virtual void Load<T>(byte[] data) where T : BillingItem, new()
		{
			bool[] array = new bool[items_.Length];
			ByteArrayInputStream byteArrayInputStream = null;
			try
			{
				byteArrayInputStream = new ByteArrayInputStream(data);
				int num = StreamUtil.ReadShort(byteArrayInputStream);
				for (int i = 0; i < num; i++)
				{
					int num2 = StreamUtil.ReadShort(byteArrayInputStream);
					BillingItem billingItem = items_[num2];
					if (billingItem == null)
					{
						billingItem = new T();
						billingItem.Init(i, 0, string.Empty, string.Empty, string.Empty, 0, string.Empty);
					}
					billingItem.Deserialize(byteArrayInputStream);
					array[num2] = true;
				}
				applyTaskList_ = new Vector();
				num = StreamUtil.ReadShort(byteArrayInputStream);
				for (int j = 0; j < num; j++)
				{
					ApplyTask applyTask = new ApplyTask();
					applyTask.Deserialize(byteArrayInputStream);
					if (applyTask.Enable())
					{
						applyTaskList_.AddElement(applyTask);
					}
				}
				purchaseLog_ = new Vector();
				num = StreamUtil.ReadShort(byteArrayInputStream);
				for (int k = 0; k < num; k++)
				{
					PurchaseRecord purchaseRecord = new PurchaseRecord();
					purchaseRecord.Deserialize(byteArrayInputStream);
					purchaseLog_.AddElement(purchaseRecord);
				}
				Load(byteArrayInputStream);
			}
			finally
			{
				if (byteArrayInputStream != null)
				{
					byteArrayInputStream.Close();
				}
				byteArrayInputStream = null;
				for (int l = 0; l < itemList_.Size(); l++)
				{
					BillingItem billingItem2 = (BillingItem)itemList_.ElementAt(l);
					if (!array[billingItem2.id_])
					{
						billingItem2.ChangeState(billingItem2.GetState());
					}
				}
			}
		}

		public virtual byte[] Write()
		{
			ByteArrayOutputStream byteArrayOutputStream = null;
			try
			{
				byteArrayOutputStream = new ByteArrayOutputStream();
				StreamUtil.WriteShort(byteArrayOutputStream, (short)itemList_.Size());
				for (int i = 0; i < itemList_.Size(); i++)
				{
					BillingItem billingItem = (BillingItem)itemList_.ElementAt(i);
					StreamUtil.WriteShort(byteArrayOutputStream, (short)billingItem.id_);
					billingItem.Serialize(byteArrayOutputStream);
				}
				Vector applyTaskList = GetApplyTaskList(false);
				StreamUtil.WriteShort(byteArrayOutputStream, (short)applyTaskList.Size());
				for (int j = 0; j < applyTaskList.Size(); j++)
				{
					ApplyTask applyTask = (ApplyTask)applyTaskList.ElementAt(j);
					applyTask.Serialize(byteArrayOutputStream);
				}
				StreamUtil.WriteShort(byteArrayOutputStream, (short)purchaseLog_.Size());
				for (int k = 0; k < purchaseLog_.Size(); k++)
				{
					PurchaseRecord purchaseRecord = (PurchaseRecord)purchaseLog_.ElementAt(k);
					purchaseRecord.Serialize(byteArrayOutputStream);
				}
				Save(byteArrayOutputStream);
				return byteArrayOutputStream.ToByteArray();
			}
			finally
			{
				if (byteArrayOutputStream != null)
				{
					byteArrayOutputStream.Close();
				}
				byteArrayOutputStream = null;
			}
		}

		public virtual void NewGame(bool take)
		{
			prepareGame_ = true;
			try
			{
				confirm_ = false;
				for (int num = applyTaskList_.Size() - 1; num >= 0; num--)
				{
					ApplyTask applyTask = (ApplyTask)applyTaskList_.ElementAt(num);
					if (applyTask.IsGameApply())
					{
						applyTaskList_.RemoveElementAt(num);
					}
				}
				for (int i = 0; i < itemList_.Size(); i++)
				{
					BillingItem billingItem = (BillingItem)itemList_.ElementAt(i);
					if (!(billingItem.Check(2))) { continue; }
billingItem.provision_ = false;
if (billingItem.state_.Get() == 1 && take)
						{
							billingItem.provision_ = true;
						}
				}
				UpdateApplyTask(false);
			}
			finally
			{
				prepareGame_ = false;
			}
		}

		public virtual void LoadGame(byte[] data)
		{
			ByteArrayInputStream byteArrayInputStream = null;
			try
			{
				byteArrayInputStream = new ByteArrayInputStream(data);
				confirm_ = StreamUtil.ReadBoolean(byteArrayInputStream);
				for (int num = applyTaskList_.Size() - 1; num >= 0; num--)
				{
					ApplyTask applyTask = (ApplyTask)applyTaskList_.ElementAt(num);
					if (applyTask.IsGameApply())
					{
						applyTaskList_.RemoveElementAt(num);
					}
				}
				int num2 = StreamUtil.ReadInt(byteArrayInputStream);
				for (int i = 0; i < num2; i++)
				{
					int num3 = StreamUtil.ReadInt(byteArrayInputStream);
					bool provision_ = StreamUtil.ReadBoolean(byteArrayInputStream);
					if (num3 < items_.Length && items_[num3] != null)
					{
						items_[num3].provision_ = provision_;
					}
				}
				num2 = StreamUtil.ReadShort(byteArrayInputStream);
				for (int j = 0; j < num2; j++)
				{
					ApplyTask applyTask2 = new ApplyTask();
					applyTask2.Deserialize(byteArrayInputStream);
					if (applyTask2.Enable())
					{
						applyTaskList_.AddElement(applyTask2);
					}
				}
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

		public virtual byte[] WriteGame()
		{
			ByteArrayOutputStream byteArrayOutputStream = null;
			try
			{
				byteArrayOutputStream = new ByteArrayOutputStream();
				StreamUtil.WriteBoolean(byteArrayOutputStream, confirm_);
				Vector itemList = GetItemList(true);
				StreamUtil.WriteInt(byteArrayOutputStream, itemList.Size());
				for (int i = 0; i < itemList.Size(); i++)
				{
					BillingItem billingItem = (BillingItem)itemList.ElementAt(i);
					StreamUtil.WriteInt(byteArrayOutputStream, billingItem.id_);
					StreamUtil.WriteBoolean(byteArrayOutputStream, billingItem.provision_);
				}
				itemList = GetApplyTaskList(true);
				StreamUtil.WriteShort(byteArrayOutputStream, (short)itemList.Size());
				for (int j = 0; j < itemList.Size(); j++)
				{
					ApplyTask applyTask = (ApplyTask)itemList.ElementAt(j);
					applyTask.Serialize(byteArrayOutputStream);
				}
				return byteArrayOutputStream.ToByteArray();
			}
			finally
			{
				if (byteArrayOutputStream != null)
				{
					byteArrayOutputStream.Close();
				}
				byteArrayOutputStream = null;
			}
		}

		public virtual void UpdateBillingItems(bool detail)
		{
			if (!bootCheckRestore_)
			{
				bootCheckRestore_ = true;
				if (StoreBase.GetRestoreRequest() == 1)
				{
					WaitForInitialized();
					Restore();
				}
			}
			if (HasIncompletePayments())
			{
				detail = true;
			}
			List<string> list = new List<string>();
			for (int i = 0; i < itemList_.Size(); i++)
			{
				BillingItem billingItem = (BillingItem)itemList_.ElementAt(i);
				list.Add(billingItem.GetProductId());
			}
			Inventory inventory = QueryInventory(false, list);
			string[][] array = null;
			try
			{
				if (!recoveryCheck_)
				{
					array = ConnectGetMarketPurchase();
				}
			}
			catch (IOException ex)
			{
				if (!ex.Message.StartsWith("404 Not Found"))
				{
					throw ex;
				}
			}
			bool flag = false;
			for (int j = 0; j < list.Count; j++)
			{
				BillingItem billingItem2 = (BillingItem)itemList_.ElementAt(j);
				string productId = billingItem2.GetProductId();
				Purchase purchase = inventory.GetPurchase(productId);
				if (inventory.HasPurchase(productId))
				{
					billingItem2.recovery_ = false;
					billingItem2.SetPurchase(purchase);
					if (billingItem2.GetState() != 1)
					{
						billingItem2.ChangeState(1);
						RemoveApplyTask(billingItem2, 1);
						AddPurchaseLog(billingItem2.GetProductId(), 0, billingItem2.GetPurchase());
						flag = true;
					}
					continue;
				}
				if (array != null && billingItem2.Check(1))
				{

					foreach (string[] array3 in array)
					{
						if ((array3[0] != productId)) { continue; }
Purchase purchase2 = new Purchase();
purchase2.mOrderId = array3[1];
billingItem2.recovery_ = true;
billingItem2.SetPurchase(purchase2);
billingItem2.ChangeState(1);
flag = true;
					}
				}
				if (billingItem2.GetState() == 1 && !billingItem2.recovery_)
				{
					billingItem2.ChangeState(0);
					RemoveApplyTask(billingItem2, 0);
				}
			}
			if (detail)
			{
				int num = 0;
				int num2 = 20;
				Vector vector = new Vector();
				string text = null;
				while (num < itemGroup_.Length && num2 > 0)
				{
					int num3 = num2;
					if (num + num3 > itemGroup_.Length)
					{
						num3 = itemGroup_.Length - num;
					}
					list = new List<string>();
					for (int l = 0; l < num3; l++)
					{
						list.Add(itemGroup_[num + l][0].GetProductId());
					}
					try
					{
						inventory = QueryInventory(true, list);
					}
					catch (Exception)
					{
						num2 /= 2;
						continue;
					}
					for (int m = 0; m < num3; m++)
					{
						SkuDetails skuDetails = inventory.GetSkuDetails(list[m]);
						BillingItem billingItem3 = itemGroup_[num + m][0];
						if (skuDetails == null)
						{
							if (!billingItem3.Check(4))
							{
								vector.AddElement(billingItem3);
							}
							continue;
						}
						billingItem3.price_ = skuDetails.GetPrice();
						billingItem3.type_ = ((skuDetails.GetTypeA().Equals("inapp")) ? (0) : (1));
						billingItem3.explain_ = skuDetails.GetDescription();
						if (billingItem3.explain_ != null)
						{
							billingItem3.explain_ = billingItem3.explain_.Replace("\\n", "\n");
						}
					}
					num += num3;
				}
				for (int n = 0; n < vector.Size(); n++)
				{
					BillingItem billingItem4 = (BillingItem)vector.ElementAt(n);
					list = new List<string>();
					list.Add(billingItem4.GetProductId());
					try
					{
						inventory = QueryInventory(true, list);
					}
					catch (Exception)
					{
						continue;
					}
					SkuDetails skuDetails2 = inventory.GetSkuDetails(list[0]);
					if (skuDetails2 == null)
					{
						if (text == null)
						{
							text = list[0];
						}
						continue;
					}
					billingItem4.price_ = skuDetails2.GetPrice();
					billingItem4.type_ = ((skuDetails2.GetTypeA().Equals("inapp")) ? (0) : (1));
					billingItem4.explain_ = skuDetails2.GetDescription();
					if (billingItem4.explain_ != null)
					{
						billingItem4.explain_ = billingItem4.explain_.Replace("\\n", "\n");
					}
				}
				for (int num4 = 0; num4 < itemGroup_.Length; num4++)
				{
					for (int num5 = 1; num5 < itemGroup_[num4].Length; num5++)
					{
						itemGroup_[num4][num5].price_ = itemGroup_[num4][0].price_;
						itemGroup_[num4][num5].type_ = itemGroup_[num4][0].type_;
						itemGroup_[num4][num5].explain_ = itemGroup_[num4][0].explain_;
					}
				}
				if (text != null && !Config.UNITY_EDITOR)
				{
					throw new IOException(Language.LT("存在しないサービスID:<0>", list[0]));
				}
			}
			recoveryCheck_ = true;
			if (flag)
			{
				KairoBase instance = KairoBase.GetInstance();
				try
				{
					instance.Save();
				}
				catch (Exception)
				{
				}
			}
			if (HasIncompletePayments())
			{
				ProcessIncompletePayments();
			}
		}

		public void Finish()
		{
		}

		public override BillingItem[] GetItems()
		{
			return items_;
		}

		public BillingItem GetItem(int id)
		{
			if (0 <= id && id < items_.Length)
			{
				return items_[id];
			}
			return null;
		}

		public BillingItem GetItem(string productId)
		{
			for (int i = 0; i < itemList_.Size(); i++)
			{
				BillingItem billingItem = (BillingItem)itemList_.ElementAt(i);
				if (productId.Equals(billingItem.GetProductId()))
				{
					return billingItem;
				}
			}
			return null;
		}

		public BillingItem[] GetSales()
		{
			bool flag = CheckPlayingGame();
			Vector vector = new Vector();
			for (int i = 0; i < itemList_.Size(); i++)
			{
				BillingItem billingItem = (BillingItem)itemList_.ElementAt(i);
				if (billingItem.Check(4)) { } else if ((billingItem.Check(2)) && (!(flag))) { } else if ((billingItem.CheckTestProduct()) && (!(Config.DEBUG))) { } else {
					vector.Add(billingItem);
				}
			}
			BillingItem[] array = new BillingItem[vector.Size()];
			for (int j = 0; j < vector.Size(); j++)
			{
				array[j] = (BillingItem)vector.ElementAt(j);
			}
			return array;
		}

		public bool LaunchPurchaseFlow(BillingItem item)
		{
			KairoBase instance = KairoBase.GetInstance();
			CheckPaymentMaintenance();
			try
			{
				instance.SetReserveSaveEnable(false);
				instance.AccessCounter(0, "shop");
				item.provision_ = false;
				Purchase purchase = LaunchPurchaseFlow(item.GetProductId());
				if (purchase == null)
				{
					return false;
				}
				if (item.GetProvisionType().Equals(StoreBase.PAYMENT_PRODUCT))
				{
					try
					{
						KairoBase instance2 = KairoBase.GetInstance();
						instance2.AddPaymentMoney(item.GetProductId(), purchase.GetOrderId(), purchase.GetToken(), item.GetProvisionValue(), 1);
						purchase = CompletePayment(purchase);
					}
					catch (Exception ex)
					{
						string text = "購入が正常に完了しませんでした。";
						if (Http.GetErrorCode() != 0)
						{
							text = Http.GetErrorText();
							if (text.IndexOf("サーバーメンテナンス中です。") != -1)
							{
								text = "サーバーメンテナンス中です。\n\n時間を置いてから\n以下の方法で商品が届きます。\n\n・購入履歴から復元\n・アプリ再起動";
							}
							else if (text.IndexOf("Server is under maintenance.") != -1)
							{
								text = "The server is currently undergoing maintenance.\n\nPlease try again later and follow the steps below to receive your products.\n・Restore your purchases from the Purchase History menu.\n・Restart the app.";
							}
						}
						Dialog dialog = new Dialog(2, "ｴﾗｰ");
						dialog.SetText(text);
						dialog.Show();
						throw ex;
					}
				}
				instance.AccessCounter(0, "purchase");
				if (item.id_ == 0)
				{
					instance.AccessCounter(0, "adoff");
				}
				item.SetPurchase(purchase);
				Purchase(item);
				UpdateApplyTask(true);
			}
			finally
			{
				instance.Save();
				instance.SetReserveSaveEnable(true);
			}
			return true;
		}

		internal void Purchase(BillingItem item)
		{
			item.ChangeState(1);
			AddPurchaseLog(item.GetProductId(), 0, item.GetPurchase());
			try
			{
				ConnectRegistMarketPurchase();
			}
			catch (Exception)
			{
			}
			item.purchaseNow_ = true;
		}

		private bool Consume(BillingItem item)
		{
			if (item.GetPurchase() == null)
			{
				if (item.consuming_)
				{
					return true;
				}
				return false;
			}
			return ConsumeItem(item.GetPurchase());
		}

		private bool CheckRegistApply(BillingItem item, int type)
		{
			if (item == null)
			{
				return false;
			}
			if (SearchApplyTask(item, type) != -1)
			{
				return false;
			}
			if (item.Check(2) && !CheckPlayingGame())
			{
				return false;
			}
			if (type == 0)
			{
				if (item.state_.Get() == 0 && !item.consuming_)
				{
					return false;
				}
				if (!item.Check(1) && item.provision_)
				{
					return false;
				}
			}
			else
			{
				if (item.Check(1))
				{
					return false;
				}
				if (item.state_.Get() == 1)
				{
					return false;
				}
				if (!item.provision_)
				{
					return false;
				}
			}
			return true;
		}

		public bool CheckNextConsume()
		{
			for (int i = 0; i < itemList_.Size(); i++)
			{
				BillingItem billingItem = (BillingItem)itemList_.ElementAt(i);
				if (CheckRegistApply(billingItem, 0) && billingItem.Check(1))
				{
					return true;
				}
			}
			return false;
		}

		public bool RegistApplyTask(bool notice)
		{
			lock (RegistApplyTask_lock)
			{
				int num = applyTaskList_.Size();
				bool flag = false;
				bool flag2 = false;
				if (notice)
				{
					for (int i = 0; i < itemList_.Size(); i++)
					{
						BillingItem billingItem = (BillingItem)itemList_.ElementAt(i);
						if (CheckRegistApply(billingItem, 0) && billingItem.Check(1) && billingItem.Check(2) && !billingItem.consuming_)
						{
							flag2 = true;
							break;
						}
					}
				}
				bool flag3 = false;
				if (flag2)
				{
					try
					{
						KairoService instance = KairoService.GetInstance();
						instance.SaveAll();
					}
					catch (Exception t)
					{
						flag3 = true;
						if (Config.DEBUG)
						{
							Log.Info(t);
						}
					}
				}
				for (int j = 0; j < itemList_.Size(); j++)
				{
					BillingItem billingItem2 = (BillingItem)itemList_.ElementAt(j);
					if (CheckRegistApply(billingItem2, 0))
					{
						if (billingItem2.Check(1))
						{
							if (!notice)
							{
								continue;
							}
							if (!billingItem2.consuming_)
							{
								billingItem2.consuming_ = true;
								try
								{
									KairoBase instance2 = KairoBase.GetInstance();
									instance2.SetReserveSave(false);
								}
								catch (Exception)
								{
									billingItem2.consuming_ = false;
									continue;
								}
							}
							if (flag3) { continue; }
if (!billingItem2.recovery_) {
bool recoveredConsumeResult = Consume(billingItem2);
if (!recoveredConsumeResult) { continue; }
}
							billingItem2.recovery_ = false;
							billingItem2.ChangeState(0);
							AddPurchaseLog(billingItem2.GetProductId(), 1, billingItem2.GetPurchase());
							billingItem2.consuming_ = false;
							flag = true;
							KairoBase instance3 = KairoBase.GetInstance();
							instance3.SetReserveSave(false);
						}
						AddApplyTask(new ApplyTask(billingItem2, 0, notice));
					}
					else if (CheckRegistApply(billingItem2, 1))
					{
						AddApplyTask(new ApplyTask(billingItem2, 1, notice));
					}
				}
				if (flag)
				{
					try
					{
						ConnectRegistMarketPurchase();
					}
					catch (Exception)
					{
					}
				}
				if (flag2 && !flag3)
				{
					try
					{
						KairoBase instance4 = KairoBase.GetInstance();
						instance4.SaveAll();
					}
					catch (Exception t2)
					{
						if (Config.DEBUG)
						{
							Log.Info(t2);
						}
					}
				}
				return num != applyTaskList_.Size();
			}
		}

		public void UpdateApplyTask(bool notice)
		{
			lock (UpdateApplyTask_lock)
			{
				bool flag = CheckPlayingGame();
				RegistApplyTask(notice);
				for (int i = 0; i < applyTaskList_.Size(); i++)
				{
					ApplyTask applyTask = (ApplyTask)applyTaskList_.ElementAt(i);
					if (applyTask.lock_) { continue; }
if ((applyTask.IsGameApply()) && (!(flag))) { continue; }
applyTask.lock_ = true;
bool flag2 = ExecuteApplyTask(applyTask);
if (!flag2)
						{
							applyTask.lock_ = false;
						}
if (flag2)
						{
							i = -1;
						}
				}
			}
		}

		public virtual bool ExecuteApplyTask(ApplyTask task)
		{
			if (task.type_ == 0)
			{
				Provisioning(task);
			}
			else if (task.type_ == 1)
			{
				Cancel(task);
			}
			return true;
		}

		public virtual void Provisioning(ApplyTask task)
		{
			RemoveApplyTask(task);
			if (task.IsItem())
			{
				BillingItem item_ = task.item_;
				item_.SetProvision(true);
				AddPurchaseLog(item_.GetProductId(), 2, item_.GetPurchase());
				if (item_.purchaseNow_)
				{
					if (IApplication.GetCurrentApp().CheckUiThread())
					{
						new JThread(delegate
						{
							try
							{
								ConnectRegistMarketPurchase();
							}
							catch (Exception)
							{
							}
						}).Start();
					}
					else
					{
						try
						{
							ConnectRegistMarketPurchase();
						}
						catch (Exception)
						{
						}
					}
					item_.purchaseNow_ = false;
				}
			}
			KairoBase instance = KairoBase.GetInstance();
			instance.SetReserveSave(task.IsGameApply());
		}

		public virtual void Cancel(ApplyTask task)
		{
			RemoveApplyTask(task);
			if (task.IsItem())
			{
				BillingItem item_ = task.item_;
				item_.SetProvision(false);
				if (item_.Check(1))
				{
					AddPurchaseLog(item_.GetProductId(), 1, item_.GetPurchase());
				}
			}
			KairoBase instance = KairoBase.GetInstance();
			instance.SetReserveSave(task.IsGameApply());
		}

		public Vector GetPurchaseLog()
		{
			return purchaseLog_;
		}

		public void ClearPurchaseLog()
		{
			purchaseLog_.RemoveAllElements();
		}

		public virtual bool CheckPlayingGame()
		{
			if (prepareGame_)
			{
				return true;
			}
			return false;
		}

		public void AddPurchaseLog(string productId, int state, Purchase purchase)
		{
			PurchaseRecord purchaseRecord = null;
			string text = null;
			if (purchase != null)
			{
				text = purchase.GetOrderId();
			}
			for (int num = purchaseLog_.Size() - 1; num >= 0; num--)
			{
				PurchaseRecord purchaseRecord2 = (PurchaseRecord)purchaseLog_.ElementAt(num);
				if ((!(purchaseRecord2.productId_.Equals(productId))) || ((text != null) && (!(purchaseRecord2.orderId_.Equals(text))))) { continue; }
purchaseRecord = purchaseRecord2;
break;
			}
			if (purchaseRecord == null && text != null)
			{
				purchaseRecord = new PurchaseRecord(productId, text);
				purchaseLog_.AddElement(purchaseRecord);
			}
			if (purchaseRecord != null)
			{
				purchaseRecord.ChangeState(state, purchase);
			}
		}

		public void ConnectRegistMarketPurchase()
		{
			KairoService instance = KairoService.GetInstance();
			if (!instance.CheckUserId())
			{
				return;
			}
			string text = string.Empty;
			for (int i = 0; i < purchaseLog_.Size(); i++)
			{
				PurchaseRecord purchaseRecord = (PurchaseRecord)purchaseLog_.ElementAt(i);
				if (!(purchaseRecord.send_)) { continue; }
if (text.Length() > 0)
					{
						text += "\t";
					}
string text2 = text;
text = text2 + purchaseRecord.productId_ + "," + purchaseRecord.orderId_ + "," + purchaseRecord.state_;
for (int j = 0; j < purchaseRecord.timestamp_.Length; j++)
					{
						text = text + "," + purchaseRecord.timestamp_[j];
					}
			}
			if (text.Length() == 0)
			{
				return;
			}
			byte[] bytes = text.GetBytes();
			string cgi = "regist_market_purchase.php";
			try
			{
				if (!instance.IsIllegalPackage() && !instance.IsIllegalCountry())
				{
					Http.Connect(instance.MakeUrl(cgi, bytes), bytes, true);
				}
			}
			catch (Exception ex)
			{
				if (Config.DEBUG)
				{
					Log.Info(ex);
				}
				throw ex;
			}
			for (int k = 0; k < purchaseLog_.Size(); k++)
			{
				PurchaseRecord purchaseRecord2 = (PurchaseRecord)purchaseLog_.ElementAt(k);
				purchaseRecord2.send_ = false;
			}
		}

		public string[][] ConnectGetMarketPurchase()
		{
			KairoService instance = KairoService.GetInstance();
			if (!instance.CheckUserId())
			{
				return null;
			}
			ConnectRegistMarketPurchase();
			string text = KairoBase.MakeQuery("state", "0");
			string cgi = "get_market_purchase.php" + text;
			try
			{
				if (instance.IsIllegalPackage() || instance.IsIllegalCountry())
				{
					return null;
				}
				byte[] array = Http.Connect(instance.MakeUrl(cgi), null, true);
				if (array.Length == 0)
				{
					return null;
				}
				string[] array2 = JString.New(array).Split('\n');
				string[][] array3 = new string[array2.Length][];
				for (int i = 0; i < array3.Length; i++)
				{
					array3[i] = array2[i].Split(',');
				}
				return array3;
			}
			catch (Exception ex)
			{
				if (Config.DEBUG)
				{
					Log.Info(ex);
				}
				throw ex;
			}
		}

		public bool AddApplyTask(ApplyTask task)
		{
			for (int i = 0; i < applyTaskList_.Size(); i++)
			{
				ApplyTask applyTask = (ApplyTask)applyTaskList_.ElementAt(i);
				if (applyTask.Equals(task))
				{
					return false;
				}
			}
			applyTaskList_.AddElement(task);
			return true;
		}

		internal int SearchApplyTask(BillingItem item, int type)
		{
			for (int i = 0; i < applyTaskList_.Size(); i++)
			{
				ApplyTask applyTask = (ApplyTask)applyTaskList_.ElementAt(i);
				if (applyTask.item_ == item && applyTask.type_ == type)
				{
					return i;
				}
			}
			return -1;
		}

		internal int SearchApplyTask(ApplyTask task)
		{
			for (int i = 0; i < applyTaskList_.Size(); i++)
			{
				ApplyTask applyTask = (ApplyTask)applyTaskList_.ElementAt(i);
				if (applyTask.Equals(task))
				{
					return i;
				}
			}
			return -1;
		}

		internal bool RemoveApplyTask(BillingItem item, int type)
		{
			int num = SearchApplyTask(item, type);
			if (num == -1)
			{
				return false;
			}
			applyTaskList_.RemoveElementAt(num);
			return true;
		}

		internal bool RemoveApplyTask(ApplyTask task)
		{
			int num = SearchApplyTask(task);
			if (num == -1)
			{
				return false;
			}
			applyTaskList_.RemoveElementAt(num);
			return true;
		}

		internal Vector GetApplyTaskList(bool game)
		{
			Vector vector = new Vector();
			for (int i = 0; i < applyTaskList_.Size(); i++)
			{
				ApplyTask applyTask = (ApplyTask)applyTaskList_.ElementAt(i);
				if (applyTask.IsGameApply() == game)
				{
					vector.AddElement(applyTask);
				}
			}
			return vector;
		}

		internal Vector GetItemList(bool game)
		{
			Vector vector = new Vector();
			for (int i = 0; i < itemList_.Size(); i++)
			{
				BillingItem billingItem = (BillingItem)itemList_.ElementAt(i);
				if (billingItem.Check(2) == game)
				{
					vector.AddElement(billingItem);
				}
			}
			return vector;
		}

		public BillingItem[][] GetItemGroup()
		{
			return itemGroup_;
		}
	}
}
