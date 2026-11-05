namespace kairo.unity.system.appli
{
	public struct GameAppli
	{
		public int Pid;

		public string Language;

		public string Name;

		public string AndPkg;

		public int AndPrice;

		public int AndDate;

		public string IOSPkg;

		public string IOSId;

		public int IOSPrice;

		public int IOSDate;

		public bool IsReleaseAndroid()
		{
			return AndDate != -1;
		}

		public bool IsReleaseIOS()
		{
			return IOSDate != -1;
		}
	}
}
