using java.io;

namespace kairo.unity.util
{
	public interface ISerializable
	{
		void Serialize(OutputStream stream);

		void Deserialize(InputStream stream);
	}
}
