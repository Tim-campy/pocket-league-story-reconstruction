using kairo.unity.system.lineup.ui;

namespace system.lineup
{
	public class MyLineup : LineupManager
	{
		internal MyLineup()
		{
		}

		public new static MyLineup GetInstance()
		{
			if (LineupManager.instance_ == null)
			{
				LineupManager.instance_ = new MyLineup();
			}
			return (MyLineup)LineupManager.instance_;
		}

		public void Start()
		{
			Start(1);
		}
	}
}
