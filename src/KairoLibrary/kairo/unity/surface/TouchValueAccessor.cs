namespace kairo.unity.surface
{
	public interface TouchValueAccessor
	{
		int GetTouchValue(int type, int index);

		void SetTouchValue(int type, int value);
	}
}
