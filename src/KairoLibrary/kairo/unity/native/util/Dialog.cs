using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using kairo.common.cfg;
using kairo.unity.ui;
using UnityEngine;

namespace kairo.unity.native.util
{
	public static class Dialog
	{
		public static string[] ShowUnityDialog(params object[] args)
		{
			return (string[])IApplication.GetCurrentApp().RunOnUiThread(delegate
			{
				if (Config.UNITY_EDITOR)
				{
					Type type = Type.GetType("UnityEditor.EditorWindow, UnityEditor.dll");
					if (type == null)
					{
						return (object)null;
					}
					Assembly assembly = type.Assembly;
					Type type2 = assembly.GetType("UnityEditor.EditorWindow");
					Type type3 = assembly.GetType("UnityEditor.GameView");
					object obj = type2.GetMethod("GetWindow", new Type[1] { typeof(Type) }).Invoke(null, new object[1] { type3 });
					Rect rect = (Rect)type2.GetProperty("position").GetValue(obj, null);
					string text = (int)rect.center.x + "," + (int)(rect.center.y + 80f);
					for (int i = 0; i < args.Length; i++)
					{
						text += " ";
						text = text + "\"" + args[i].ToString() + "\"";
					}
					Process process = new Process();
					process.StartInfo.FileName = "UnityDialogs.exe";
					process.StartInfo.UseShellExecute = false;
					process.StartInfo.RedirectStandardOutput = true;
					process.StartInfo.RedirectStandardInput = false;
					process.StartInfo.Arguments = text;
					process.Start();
					Stream baseStream = process.StandardOutput.BaseStream;
					MemoryStream memoryStream = new MemoryStream();
					while (true)
					{
						int num = baseStream.ReadByte();
						if (num == -1)
						{
							break;
						}
						memoryStream.WriteByte((byte)num);
					}
					byte[] array = memoryStream.ToArray();
					baseStream.Read(array, 0, array.Length);
					process.WaitForExit();
					process.Close();
					string text2 = Encoding.GetEncoding("shift_jis").GetString(array);
					if (text2 == "\0")
					{
						return (object)null;
					}
					string[] array2 = text2.Split(' ');
					for (int j = 0; j < array2.Length; j++)
					{
						array2[j] = array2[j].Trim('"');
						if (array2[j] == "\0")
						{
							array2[j] = null;
						}
						else
						{
							array2[j] = Uri.UnescapeDataString(array2[j]);
						}
					}
					return array2;
				}
				return (object)null;
			}, true);
		}
	}
}
