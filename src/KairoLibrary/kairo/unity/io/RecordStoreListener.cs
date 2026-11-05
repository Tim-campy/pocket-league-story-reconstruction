using System;
using java.io;

namespace kairo.unity.io
{
	public interface RecordStoreListener
	{
		bool OnWriteRecordError(DataOutputStream dos, Exception err);
	}
}
