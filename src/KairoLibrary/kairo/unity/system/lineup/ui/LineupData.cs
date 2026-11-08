using java.lang;
using java.util;
using kairo.common.cfg;
using kairo.unity.system.appli;
using kairo.unity.ui;
using kairo.unity.util;

namespace kairo.unity.system.lineup.ui
{
	public class LineupData
	{
		internal int gameId_;

		internal string explain_;

		internal string url_;

		internal string file_;

		internal int priority_;

		internal int[][] ratio_;

		internal Image image_;

		public void Load(int gameId, string src)
		{
			string[] array = StringUtil.Split(src, "@");
			gameId_ = gameId;
			explain_ = array[0];
			url_ = array[1];
			file_ = array[2];
			priority_ = JInteger.ParseInt(array[3]);
			string[] array2 = StringUtil.Split(array[4], "&");
			ratio_ = JTool.MakeArray<int[][]>(new int[2] { array2.Length, 2 });
			for (int i = 0; i < array2.Length; i++)
			{
				string[] array3 = StringUtil.Split(array2[i], ",");
				ratio_[i] = new int[2]
				{
					JInteger.ParseInt(array3[0]),
					JInteger.ParseInt(array3[1])
				};
			}
		}

		public int GetGameId()
		{
			return gameId_;
		}

		public string GetExplain()
		{
			return explain_;
		}

		public string GetURL()
		{
			return url_;
		}

		public int GetPriority()
		{
			return priority_;
		}

		public string GetFileName()
		{
			if (file_.Length() > 0)
			{
				return "gm" + gameId_ + file_ + ".png";
			}
			return "gm" + gameId_ + ".png";
		}

		public Image GetImage()
		{
			return image_;
		}

		public int GetRatio()
		{
			if (explain_.Length() == 0 || url_.Length() == 0)
			{
				return 0;
			}
			if (gameId_ == Config.PID && !Config.TRIAL_VERSION)
			{
				return 0;
			}
			KairoBase instance = KairoBase.GetInstance();
			bool flag = false;
			int num;
GameAppli[] appList=instance.GetAppList();
for(int i=0;i<appList.Length;i++){
GameAppli recoveredApp=appList[i];
if(recoveredApp.Pid==gameId_){flag=true;}
}
			if (!flag)
			{
				return 0;
			}
			for (int j = 1; j < ratio_.Length; j++)
			{
				if (ratio_[j][0] == Config.PID && ratio_[j][1] != -1)
				{
					return ratio_[j][1];
				}
			}
			num = ratio_[0][1];
			if (num < 0)
			{
				num = 0;
			}
			return num;
		}
	}
}
