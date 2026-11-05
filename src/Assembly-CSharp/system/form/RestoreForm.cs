using form;
using java.lang;
using kairo.unity.form;
using kairo.unity.ui;
using system.billing.ui;

namespace system.form
{
	public class RestoreForm : MyFormBase
	{
		public const int FORMID_RESTORE = 906;

		private FormBase next_;

		private string message_;

		public RestoreForm(FormBase next = null)
			: base(906)
		{
			next_ = next;
		}

		public override string ToString()
		{
			return "RestoreForm";
		}

		public override bool Init()
		{
			base.Init();
			SetSoftLabel(string.Empty, string.Empty);
			result_ = -1;
			new JThread(delegate
			{
				BillingManager instance = BillingManager.GetInstance();
				result_ = (instance.CallRestoreFlow(out message_) ? 1 : 0);
			}).Start();
			return true;
		}

		public override void Draw(Graphics g)
		{
			base.Draw(g);
			if (result_ == -1)
			{
				ConnectForm.DrawConnect(g);
			}
		}

		public override bool Update()
		{
			base.Update();
			if (result_ != -1)
			{
				Pop();
				if (next_ != null)
				{
					Push(next_);
				}
				if (message_ != null)
				{
					Push(message_);
				}
			}
			return true;
		}

		public override bool CheckMainWindow()
		{
			return false;
		}
	}
}
