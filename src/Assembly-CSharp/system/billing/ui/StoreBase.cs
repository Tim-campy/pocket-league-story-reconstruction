using System;
using System.Collections.Generic;
using java.io;
using java.lang;
using kairo.unity.io;
using kairo.unity.system;
using kairo.unity.ui;
using kairo.unity.util;
using system.billing.util;
using UnityEngine;
using UnityEngine.Purchasing;

namespace system.billing.ui
{
	public abstract class StoreBase : IStoreListener
	{
		public enum RestoreResult
		{
			Complete,
			Faild
		}

		public const int VERSION = 102;

		private const string RESTORE_KEY = "_restore_transactions";

		private static KairoService ks_ = null;

		private IStoreController storeController_;

		private IExtensionProvider storeExtensionProvider_;

		private List<Receipt> receipts_ = new List<Receipt>();

		internal static string PAYMENT_PRODUCT;

		private static bool isPurchaseProcessed_;

		private static Purchase processPurchaseResult_;

		private RestoreResult restoreResult_;

		private bool isRestoreProcessed_;

		public virtual void Init(string[][] data, string paymentProduct, params string[] subscription)
		{
			ks_ = KairoService.GetInstance();
			PAYMENT_PRODUCT = paymentProduct;
			if (storeController_ == null)
			{
				InitializePurchasing(data, paymentProduct, subscription);
			}
		}

		private void InitializePurchasing(string[][] data, string paymentProduct, string[] subscriptions)
		{
		}

		protected bool IsInitialized()
		{
			return true;
		}

		protected void WaitForInitialized()
		{
			while (!IsInitialized())
			{
				JThread.Yield();
			}
		}

		public Purchase LaunchPurchaseFlow(string productId)
		{
			return null;
		}

		public RestoreResult Restore()
		{
			if (!IsInitialized())
			{
				Log.Info("RestorePurchases FAIL. Not initialized.");
				restoreResult_ = RestoreResult.Faild;
			}
			else
			{
				restoreResult_ = RestoreResult.Complete;
			}
			return restoreResult_;
		}

		public Inventory QueryInventory(bool querySkuDetails, List<string> moreItemSkus, List<string> moreSubsSkus = null)
		{
			List<string> list = new List<string>();
			List<string>[] array = new List<string>[2] { moreItemSkus, moreSubsSkus };
			List<string>[] array2 = array;
			foreach (List<string> list2 in array2)
			{
				if (list2 != null)
				{
					for (int j = 0; j < list2.Count; j++)
					{
						list.Add(list2[j]);
					}
				}
			}
			if (list.Count <= 0)
			{
				return new Inventory();
			}
			return new Inventory();
		}

		public bool ConsumeItem(Purchase purchase)
		{
			Receipt receipt = SearchReceipt(purchase.GetOriginalJson());
			if (receipt == null)
			{
				return false;
			}
			if (receipt.IsConsume())
			{
				return false;
			}
			receipt.Consume();
			return true;
		}

		public Receipt SearchReceipt(string json)
		{
			foreach (Receipt item in receipts_)
			{
				if (item.GetJson() == json)
				{
					return item;
				}
			}
			return null;
		}

		public ProductType GetProductType(string productID)
		{
			throw new Exception("[ 存在しないproductID : " + productID + " ]");
		}

		public List<Receipt> GetIncompletePayments()
		{
			List<Receipt> list = new List<Receipt>();
			foreach (Receipt item in receipts_)
			{
				if (!item.paymentComplete_ && item.IsTicketProduct())
				{
					list.Add(item);
				}
			}
			return list;
		}

		public bool HasIncompletePayments()
		{
			if (isRestoreProcessed_)
			{
				return false;
			}
			foreach (Receipt item in receipts_)
			{
				if (item.paymentComplete_ || !item.IsTicketProduct())
				{
					continue;
				}
				return true;
			}
			return false;
		}

		protected Purchase CompletePayment(Purchase purchase)
		{
			foreach (Receipt item in receipts_)
			{
				if (item.GetOrderId() != purchase.GetOrderId())
				{
					continue;
				}
				item.paymentComplete_ = true;
				if (purchase.GetPurchaseTime() != 0L)
				{
					continue;
				}
				Purchase purchase2 = SetTime(item);
				if (purchase2 == null || purchase2.GetPurchaseTime() <= 0)
				{
					item.paymentComplete_ = false;
					throw new Exception();
				}
				return purchase2;
			}
			return purchase;
		}

		public bool IsCompletePayment(Purchase purchase)
		{
			if (purchase == null)
			{
				return false;
			}
			foreach (Receipt item in receipts_)
			{
				if (item.GetOrderId() != purchase.GetOrderId() || !item.IsTicketProduct())
				{
					continue;
				}
				return item.paymentComplete_;
			}
			return false;
		}

		public void ProcessIncompletePayment(BillingItem item, Purchase purchase)
		{
			if (!item.GetProvisionType().Equals(PAYMENT_PRODUCT))
			{
				return;
			}
			KairoBase instance = KairoBase.GetInstance();
			BillingManager billingManager = (BillingManager)this;
			try
			{
				instance.SetReserveSaveEnable(false);
				item.provision_ = false;
				try
				{
					KairoBase instance2 = KairoBase.GetInstance();
					instance2.AddPaymentMoney(item.GetProductId(), purchase.GetOrderId(), purchase.GetToken(), item.GetProvisionValue(), 1);
					purchase = CompletePayment(purchase);
				}
				catch (Exception ex)
				{
					throw ex;
				}
				instance.AccessCounter(0, "purchase");
				item.SetPurchase(purchase);
				billingManager.Purchase(item);
				billingManager.UpdateApplyTask(true);
			}
			finally
			{
				instance.Save();
				instance.SetReserveSaveEnable(true);
			}
		}

		protected void ProcessIncompletePayments()
		{
			if (!HasIncompletePayments())
			{
				return;
			}
			List<Receipt> incompletePayments = GetIncompletePayments();
			while (incompletePayments.Count > 0)
			{
				BillingItem[] items = GetItems();
				Receipt receipt = incompletePayments[0];
				BillingItem billingItem = null;
				BillingItem[] array = items;
				foreach (BillingItem billingItem2 in array)
				{
					if (billingItem2 != null && billingItem2.GetProductId() == receipt.GetProductID())
					{
						billingItem = billingItem2;
						break;
					}
				}
				if (billingItem != null)
				{
					ProcessIncompletePayment(billingItem, receipt.GetPurchase());
				}
				incompletePayments.RemoveAt(0);
			}
		}

		private Purchase SetTime(Receipt receipt)
		{
			return null;
		}

		private string GetUnityJson(string json, string orderId)
		{
			if (!json.Contains("in_app"))
			{
				return null;
			}
			int num = json.IndexOf("in_app");
			bool flag = false;
			string text = string.Empty;
			for (int i = num; i < json.Length; i++)
			{
				if (json[i] == '[')
				{
					flag = true;
					continue;
				}
				if (json[i] == ']')
				{
					break;
				}
				if (flag)
				{
					text += json[i];
				}
			}
			List<string> list = new List<string>();
			flag = false;
			string text2 = string.Empty;
			for (int j = 0; j < text.Length; j++)
			{
				if (text[j] == '{')
				{
					flag = true;
					text2 = string.Empty;
				}
				else if (text[j] == '}')
				{
					flag = false;
					list.Add(text2);
					text2 = string.Empty;
				}
				else if (flag && (text2.Length != 0 || text[j] != ','))
				{
					text2 += text[j];
				}
			}
			for (int k = 0; k < list.Count; k++)
			{
				string text3 = Json.ReadJSon(list[k], "transaction_id");
				if (!(text3 != orderId))
				{
					return Json.ReadJSon(list[k], "purchase_date_ms");
				}
			}
			return null;
		}

		public bool CheckPaymentMaintenance()
		{
			bool flag = true;
			try
			{
				KairoBase instance = KairoBase.GetInstance();
				int paymentMoney = instance.GetPaymentMoney();
				if (paymentMoney == -1)
				{
					throw new Exception();
				}
				return false;
			}
			catch (Exception ex)
			{
				string text = "通信に失敗しました";
				if (Http.GetErrorCode() != 0)
				{
					text = Http.GetErrorText();
				}
				Dialog dialog = new Dialog(2, "ｴﾗｰ");
				dialog.SetText(text);
				dialog.Show();
				throw ex;
			}
		}

		public void Save(OutputStream stream)
		{
			StreamUtil.WriteInt(stream, 102);
			StreamUtil.WriteInt(stream, receipts_.Count);
			for (int i = 0; i < receipts_.Count; i++)
			{
				receipts_[i].Save(stream);
			}
		}

		public void Load(InputStream stream)
		{
			if (0 >= stream.Available())
			{
				return;
			}
			int num = StreamUtil.ReadInt(stream);
			if (num == 100 && GetRestoreRequest() == -1)
			{
				SetRestoreRequest(1);
			}
			receipts_ = new List<Receipt>();
			int num2 = StreamUtil.ReadInt(stream);
			for (int i = 0; i < num2; i++)
			{
				receipts_.Add(Receipt.Load(stream));
			}
			if (num == 101)
			{
				int num3 = StreamUtil.ReadInt(stream);
				if (GetRestoreRequest() == -1)
				{
					SetRestoreRequest((num3 == 1) ? 1 : 0);
				}
			}
		}

		protected static int GetRestoreRequest()
		{
			return (int)IApplication.GetCurrentApp().RunOnUiThread(() => PlayerPrefs.GetInt("_restore_transactions", -1), true);
		}

		protected static void SetRestoreRequest(int request)
		{
			IApplication.GetCurrentApp().RunOnUiThread(delegate
			{
				PlayerPrefs.SetInt("_restore_transactions", request);
				PlayerPrefs.Save();
			}, true);
		}

		public abstract BillingItem[] GetItems();

		public bool CallRestoreFlow(out string message)
		{
			try
			{
				KairoService instance = KairoService.GetInstance();
				BillingManager instance2 = BillingManager.GetInstance();
				if (instance2.Restore() != RestoreResult.Complete)
				{
					message = ((!Language.Japanese()) ? "Error" : "復元エラー");
					return false;
				}
				instance2.UpdateBillingItems(false);
				instance2.UpdateApplyTask(false);
				message = ((!Language.Japanese()) ? "Purchase info restored." : "課金情報の復元が完了しました。");
				return true;
			}
			catch (Exception t)
			{
				Log.Info(t);
				message = Http.GetErrorText();
				return false;
			}
		}
	}
}
