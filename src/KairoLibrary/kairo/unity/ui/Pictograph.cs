namespace kairo.unity.ui
{
	public class Pictograph
	{
		private string key_;

		public string Key => key_;

		public Seb Seb { get; set; }

		public int Frame { get; set; }

		public int Wait { get; set; }

		public int X { get; set; }

		public int Y { get; set; }

		public int Margin { get; set; }

		public Matrix Transform { get; set; }

		internal Pictograph(string key, Seb seb, int frame = -1, int wait = 1, int x = 0, int y = 0, int margin = 0)
		{
			key_ = key;
			Seb = seb;
			Frame = frame;
			Wait = wait;
			X = x;
			Y = y;
			Margin = margin;
			if (Wait < 1)
			{
				Wait = 1;
			}
			Transform = new Matrix();
		}
	}
}
