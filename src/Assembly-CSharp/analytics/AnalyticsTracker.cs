using kairo.unity.analytics;

namespace analytics
{
	public class AnalyticsTracker
	{
		private const string PROPERTY_ID = "UA-35228877-2";

		public static void TrackPageView(bool packageName, params string[] p)
		{
			TrackPageViewId(packageName, "UA-35228877-2", p);
		}

		public static void TrackPageViewId(bool packageName, string id, params string[] p)
		{
			string text = string.Empty;
			for (int i = 0; i < p.Length; i++)
			{
				if (i != 0)
				{
					text += "/";
				}
				text += p[i];
			}
			kairo.unity.analytics.AnalyticsTracker.TrackPageView(packageName, id, text);
		}

		public static void DispatchPageView()
		{
		}
	}
}
