using kairo.unity.surface;
using kairo.unity.ui;

namespace kairo.unity.form
{
	public class FormBase : TouchValueAccessor
	{
		internal int id_;

		internal string leftSoftLabel_ = "";

		internal string rightSoftLabel_ = "";

		internal string softLabel3_;

		internal string softLabel4_;

		internal int formProcMode_;

		internal FormManagerBase formManager_;

		internal int continuousUpdate_;

		public FormBase(int id)
		{
			id_ = id;
		}

		public virtual bool Init()
		{
			return true;
		}

		public virtual void Draw(Graphics g)
		{
		}

		public virtual bool Update()
		{
			return true;
		}

		public virtual bool Resume()
		{
			return true;
		}

		public virtual int GetTouchValue(int type, int index)
		{
			return 0;
		}

		public virtual void SetTouchValue(int type, int value)
		{
		}

		public virtual void Suspend()
		{
		}

		public virtual bool Finish()
		{
			return true;
		}

		public void SetSoftLabel(string left, string right, string soft3 = null, string soft4 = null)
		{
			if (CheckUsable())
			{
				leftSoftLabel_ = left;
				rightSoftLabel_ = right;
				softLabel3_ = soft3;
				softLabel4_ = soft4;
				Canvas.GetInstance().SetSoftLabel(left, right, soft3, soft4);
			}
		}

		public string GetLeftSoftLabel()
		{
			return leftSoftLabel_;
		}

		public string GetRightSoftLabel()
		{
			return rightSoftLabel_;
		}

		public string GetSoftLabel3()
		{
			return softLabel3_;
		}

		public string GetSoftLabel4()
		{
			return softLabel4_;
		}

		public bool Push(FormBase form)
		{
			if (formManager_ != null)
			{
				formManager_.Push(this, form);
			}
			return true;
		}

		public bool Pop()
		{
			if (formManager_ != null)
			{
				formManager_.Pop(this);
			}
			return true;
		}

		public virtual bool CheckMainWindow()
		{
			return true;
		}

		public bool CheckUsable()
		{
			if (formManager_ == null)
			{
				return false;
			}
			return formManager_.CheckUsable(this);
		}

		public int GetId()
		{
			return id_;
		}

		public int GetProcessMode()
		{
			return formProcMode_;
		}

		public void SetProcessMode(int mode)
		{
			formProcMode_ = mode;
		}

		public bool IsAlive()
		{
			if (formProcMode_ != 3 && formProcMode_ != 1)
			{
				return formProcMode_ == 2;
			}
			return true;
		}

		public virtual bool ProcessEvent(AppEvent evt)
		{
			return false;
		}

		public virtual bool ProcessBackKey()
		{
			return !CheckMainWindow();
		}
	}
}
