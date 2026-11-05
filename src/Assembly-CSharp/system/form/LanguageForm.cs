using System.Collections.Generic;
using System.Threading;
using form;
using kairo.common.cfg;
using kairo.unity.io;
using kairo.unity.native;
using kairo.unity.surface;
using kairo.unity.system;
using kairo.unity.ui;
using kairo.unity.util;
using surface;

namespace system.form
{
	public class LanguageForm : MyFormBase
	{
		public const int FORMID_LANGUAGE = 908;

		private static ThreadStart callback_;

		private string[][] languages_;

		private string langPackFile_;

		private bool langPackJapanese_;

		private static string langDownloadUrl_;

		private byte[] langDownloadData_;

		private bool start_;

		private int mode_;

		private static bool warningTextLayout_;

		public LanguageForm()
			: base(908)
		{
		}

		public override string ToString()
		{
			return "LanguageForm";
		}

		public override bool Init()
		{
			base.Init();
			return true;
		}

		public override bool Update()
		{
			base.Update();
			if (!start_)
			{
				start_ = true;
				List<string[]> list = new List<string[]>();
				string[][] systemLanguagePackFiles = Language.GetSystemLanguagePackFiles();
				foreach (string[] item in systemLanguagePackFiles)
				{
					list.Add(item);
				}
				string customLanguagePackLocation = Language.GetCustomLanguagePackLocation();
				if (customLanguagePackLocation != null)
				{
					try
					{
						if (Storage.Exists(1, "_custom_lang_pack.csv"))
						{
							byte[] data = Storage.Read(1, false, "_custom_lang_pack.csv");
							Language.LanguagePackInfo languagePackInfo = Language.GetLanguagePackInfo(data);
							string text = string.Empty;
							if (languagePackInfo.Title != null)
							{
								text += languagePackInfo.Title;
							}
							if (languagePackInfo.Author != null)
							{
								text = text + "@" + languagePackInfo.Author;
							}
							list.Add(new string[2] { text, customLanguagePackLocation });
						}
					}
					catch
					{
					}
				}
				languages_ = list.ToArray();
				int index = 0;
				string[] array = new string[languages_.Length];
				for (int j = 0; j < languages_.Length; j++)
				{
					array[j] = languages_[j][0];
					if (languages_[j][1] == Language.GetLanguagePackFile())
					{
						index = j;
					}
				}
				if (langDownloadUrl_ == null)
				{
					langDownloadUrl_ = Language.GetCustomLanguagePackLocation();
				}
				KairoPlugin.StartLanguagePanel(array, index, langDownloadUrl_, mode_);
				return true;
			}
			if (!KairoPlugin.IsLanguagePanelFinish())
			{
				return true;
			}
			string[] languagePanelResult = KairoPlugin.GetLanguagePanelResult();
			if (languagePanelResult[0] != null)
			{
				int num = int.Parse(languagePanelResult[0]);
				switch (num)
				{
				case -3:
					Canvas.OpenBrowser("http://kairopark.jp/language_pack_en.html");
					break;
				case -2:
				{
					langPackJapanese_ = false;
					if (languagePanelResult[1] == "ja")
					{
						langPackJapanese_ = true;
					}
					HttpForm form = new HttpForm(delegate
					{
						KairoBase instance = KairoBase.GetInstance();
						string data2;
						Language.MakeLangPackTemplate(langPackJapanese_, 202, out langPackFile_, out data2);
						int hashCode = data2.GetHashCode();
						if (instance.ConnectLangPackCheck(langPackFile_, 202, hashCode))
						{
							instance.ConnectLangPackSet(langPackFile_, 202, hashCode, data2);
						}
					}, delegate
					{
						KairoBase instance = KairoBase.GetInstance();
						instance.ConnectLangPackWeb(langPackFile_);
					});
					Push(form);
					break;
				}
				case -1:
				{
					langDownloadUrl_ = languagePanelResult[1];
					langDownloadData_ = null;
					HttpForm httpForm = new HttpForm(delegate
					{
						langDownloadData_ = Http.Connect(langDownloadUrl_, null, false);
					}, delegate(HttpForm httpForm2, HttpForm.ResultData resultData)
					{
						if (resultData.Succeed)
						{
							if (Language.SetLanguagePack(langDownloadUrl_, langDownloadData_) && callback_ != null)
							{
								callback_();
							}
						}
						else
						{
							FormManager instance = FormManager.GetInstance();
							instance.Push(new LanguageForm
							{
								mode_ = 1
							});
							instance.Push(SubForm.MakeDialog(resultData.Error));
						}
					});
					Push(httpForm);
					httpForm.ProcessError = false;
					break;
				}
				default:
				{
					string text2 = languagePanelResult[1];
					if (num != -1)
					{
						text2 = languages_[num][1];
					}
					if (Language.GetLanguagePackFile() != text2 && Language.SetLanguagePack(text2) && callback_ != null)
					{
						callback_();
					}
					break;
				}
				}
			}
			return Pop();
		}

		public override bool Finish()
		{
			base.Finish();
			return true;
		}

		public override bool CheckMainWindow()
		{
			return false;
		}

		public static void DrawButton(Graphics g, ThreadStart callback, int anchor = 20)
		{
			GameView instance = GameView.GetInstance();
			g.PushOrigin(0, 0);
			int x = 20;
			int y = 20;
			if (BitUtil.Check(anchor, 2))
			{
				x = instance.GetGameWidth() / 2;
			}
			else if (BitUtil.Check(anchor, 4))
			{
				x = instance.GetGameWidth() - 20;
			}
			if (BitUtil.Check(anchor, 32))
			{
				y = instance.GetGameHeight() / 2;
			}
			else if (BitUtil.Check(anchor, 64))
			{
				y = instance.GetGameHeight() - 20;
			}
			DrawButton(g, x, y, callback);
			g.PopOrigin();
		}

		public static void DrawButton(Graphics g, int x, int y, ThreadStart callback)
		{
			callback_ = callback;
			GameView instance = GameView.GetInstance();
			g.PushOrigin(0, 0);
			int value = 20160614;
			bool flag = instance.CheckTouch(3, value);
			ResourceManager resource = IApplication.GetCurrentApp().GetResource();
			int[] sprite = resource.seb[0].GetSprite(0, 0);
			if (flag)
			{
				g.PushBlendMode(1, 255, 255, 255, 128);
				g.PushMatrix(Matrix.ScaleTemporary(1.1f, 1.1f));
			}
			g.PushMatrix(Matrix.ScaleTemporary(0.5f, 0.5f));
			resource.DrawSeb(g, x, y, 0);
			g.PopMatrix();
			if (flag)
			{
				g.PopMatrix();
				g.PopBlendMode();
			}
			GameView.AddTouch(g, 3, x + sprite[6] / 2, y + sprite[7] / 2, sprite[4] / 2, sprite[5] / 2, value);
			g.PopOrigin();
			if (Config.DEBUG && !warningTextLayout_)
			{
				if (!Graphics.IsAutoTextLayout())
				{
					KairoPlugin.ShowToast("自動テキストレイアウトが無効になっています。\nGraphics.SetAutoTextLayout() を使って有効にする事を推奨します。");
				}
				warningTextLayout_ = true;
			}
		}

		public new static bool OnTouchEvent(TouchEvent e)
		{
			TouchComponent component = e.component;
			FormManager instance = FormManager.GetInstance();
			if (component.id == 3 && component.value == 20160614)
			{
				int type = e.type;
				if (type == 1)
				{
					instance.Push(new LanguageForm());
				}
				return false;
			}
			return true;
		}
	}
}
