using System;
using java.util;
using UnityEngine;

namespace java.lang
{
	public static class JSystem
	{
		public static void Arraycopy(object src, int src_position, object dst, int dst_position, int length)
		{
			Array.Copy((Array)src, src_position, (Array)dst, dst_position, length);
		}

		public static long CurrentTimeMillis()
		{
			return (long)(DateTime.UtcNow - JDate.UNIX_TIME).TotalMilliseconds;
		}

		public static void Exit(int status)
		{
			JRuntime.GetRuntime().Exit(status);
		}

		public static void Gc()
		{
			JRuntime.GetRuntime().Gc();
		}

		public static string GetProperty(string key)
		{
			if (key == "microedition.platform" || key == "Platform")
			{
				if (Application.platform == RuntimePlatform.IPhonePlayer)
				{
					return SystemInfo.deviceName;
				}
				return SystemInfo.deviceModel;
			}
			return null;
		}
	}
}
