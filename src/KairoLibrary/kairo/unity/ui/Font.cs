using System.Collections.Generic;
using kairo.common.cfg;
using kairo.unity.util;
using UnityEngine;

namespace kairo.unity.ui
{
	public class Font
	{
		public const int SIZE_SMALL = 0;

		public const int SIZE_MEDIUM = 1;

		public const int SIZE_LARGE = 2;

		public const int STYLE_PLAIN = 0;

		public const int STYLE_BOLD = 1;

		public const int STYLE_ITALIC = 2;

		public const int FACE_SYSTEM = 0;

		public const int FACE_MONOSPACE = 1;

		public const int FACE_PROPORTIONAL = 2;

		private static readonly int[] HEIGHT = new int[3] { 12, 24, 30 };

		private static readonly int[] WIDTH = new int[3] { 6, 12, 15 };

		private int size_;

		private static readonly FontStyle[] STYLE = new FontStyle[3]
		{
			FontStyle.Normal,
			FontStyle.Bold,
			FontStyle.Italic
		};

		private int style_;

		private int face_;

		private int usedSize_;

		private bool usePreset_;

		private GUIStyle guiStyle_;

		private float scale_ = 1f;

		private static UnityEngine.Font defaultGUIFont_;

		private static int defaultGUIFontLanguage_ = -1;

		private static List<string> invalidFontNames_ = new List<string>();

		private static bool localizeSettingLock_;

		private static List<bool> localizeSettingStack_ = new List<bool>();

		private static int localizeSettingCount_;

		internal static int requestFontScale_ = 100;

		internal int fontScale_ = requestFontScale_;

		public static bool LT_DISABLE_TEXTLAYOUT = false;

		private static GUIContent guiContents_ = new GUIContent();

		public Font()
		{
			Init();
		}

		internal void Init()
		{
			if (IApplication.GetCurrentApp().CheckUiThread())
			{
				_init();
				return;
			}
			IApplication.GetCurrentApp().RunOnUiThread(delegate
			{
				_init();
			}, true);
		}

		private void _init()
		{
			if (guiStyle_ == null)
			{
				guiStyle_ = new GUIStyle();
			}
			guiStyle_.font = GetDefaultGUIFont();
			face_ = 2;
			SetSize(0);
			SetStyle(0);
		}

		public Font(Font font)
		{
			guiStyle_ = new GUIStyle();
			Set(font);
		}

		public void Set(Font font)
		{
			guiStyle_.font = font.GetFont();
			SetSize(font.size_, font.usePreset_);
			SetStyle(font.style_);
		}

		public UnityEngine.Font GetFont()
		{
			return UpdateGUIFont();
		}

		public void Draw(Graphics g, string text, float x, float y)
		{
			Font font = g.GetFont();
			g.SetFont(this);
			g.DrawString(text, x, y);
			g.SetFont(font);
		}

		public int GetSize()
		{
			return size_;
		}

		public void SetSize(int size, bool usePreset = true)
		{
			size_ = (usedSize_ = size);
			usePreset_ = usePreset;
			if (size_ < HEIGHT.Length && usePreset)
			{
				usedSize_ = HEIGHT[size];
			}
			usedSize_ = usedSize_ * fontScale_ / 100;
			guiStyle_.fontSize = usedSize_;
		}

		public int GetStyle()
		{
			return style_;
		}

		public FontStyle GetFontStyle()
		{
			return STYLE[style_];
		}

		public void SetStyle(int style)
		{
			style_ = style;
			guiStyle_.fontStyle = STYLE[style_];
		}

		public int GetFace()
		{
			return face_;
		}

		public void SetFace(int face)
		{
			face_ = face;
		}

		public int GetHeight()
		{
			return usedSize_;
		}

		public float GetWidth()
		{
			return (float)usedSize_ / 2f;
		}

		public int StringWidth(string text)
		{
			return StringWidth(null, text);
		}

		public int StringWidth(TextFormat lf, string text)
		{
			if (text == null || guiStyle_ == null)
			{
				return 0;
			}
			text = Language.TranslateText(text);
			if (Graphics.PeekAutoTextLayout() && text.IndexOf('<') != -1)
			{
				return StringWidth2(lf, text);
			}
			return (int)(StringWidthF(lf, text) + 0.01f);
		}

		public float StringWidthF(string text)
		{
			return StringWidthF(null, text);
		}

		public float StringWidthF(TextFormat lf, string text)
		{
			if (text == null || guiStyle_ == null)
			{
				return 0f;
			}
			if (!LT_DISABLE_TEXTLAYOUT)
			{
				text = Language.TranslateText(text);
			}
			if (Graphics.PeekAutoTextLayout() && text.IndexOf('<') != -1)
			{
				return StringWidthF2(lf, text);
			}
			int num = -1;
			if (!Language.Japanese() && lf != null && lf.GetFont() != -1 && lf.GetFont() != GetSize())
			{
				num = GetSize();
				SetSize(lf.GetFont());
			}
			if ((lf == null || lf.GetFont() == -1) && IsNeedLocalizeScale())
			{
				num = GetSize();
				SetSize(GetLocalizeScale());
			}
			UpdateGUIFont();
			int fontSize = guiStyle_.fontSize;
			float scaleRatio = IApplication.GetCurrentApp().GetScaleRatio();
			guiStyle_.fontSize = (int)((float)guiStyle_.fontSize * scaleRatio / 100f + 0.001f);
			if (guiContents_ == null)
			{
				guiContents_ = new GUIContent();
			}
			guiContents_.text = text.Replace(' ', '.');
			float result = guiStyle_.CalcSize(guiContents_).x * 100f / scaleRatio;
			guiStyle_.fontSize = fontSize;
			if (num != -1)
			{
				SetSize(num);
			}
			return result;
		}

		public int StringWidth2(string text)
		{
			return StringWidth2(null, text);
		}

		public int StringWidth2(TextFormat lf, string text)
		{
			if (text == null || guiStyle_ == null)
			{
				return 0;
			}
			return (int)(StringWidthF2(lf, text) + 0.01f);
		}

		public float StringWidthF2(string text)
		{
			return StringWidthF(null, text);
		}

		public float StringWidthF2(TextFormat lf, string text)
		{
			if (text == null || guiStyle_ == null)
			{
				return 0f;
			}
			return TextLayout.StringWidthF(lf, this, text);
		}

		public void Dispose()
		{
			guiStyle_ = null;
		}

		public void Copy(Font font)
		{
			size_ = font.size_;
			style_ = font.style_;
			face_ = font.face_;
			usedSize_ = font.usedSize_;
			scale_ = font.scale_;
			fontScale_ = font.fontScale_;
			guiStyle_.font = font.GetFont();
			guiStyle_.fontSize = font.guiStyle_.fontSize;
			guiStyle_.fontStyle = font.guiStyle_.fontStyle;
			guiStyle_.normal.textColor = font.guiStyle_.normal.textColor;
		}

		public bool Equals(Font font)
		{
			if (face_ == font.face_ && size_ == font.size_)
			{
				return style_ == font.style_;
			}
			return false;
		}

		public void AdjustFontSize(float scaleRatio)
		{
			UpdateFontScale();
			scale_ = scaleRatio / 100f;
			float num = (float)usedSize_ * scale_;
			guiStyle_.fontSize = (int)(num / scale_ + 0.01f);
		}

		internal bool IsNeedLocalizeScale()
		{
			if (((localizeSettingCount_ <= 0) ? Config.LOCALIZE_FONT_SCALE : localizeSettingStack_[localizeSettingCount_ - 1]) && !localizeSettingLock_ && !Language.Japanese())
			{
				return usedSize_ >= 6;
			}
			return false;
		}

		internal int GetLocalizeScale()
		{
			return (int)((float)usedSize_ * 0.83f + 0.1f);
		}

		internal static void LockLocalizeSetting()
		{
			localizeSettingLock_ = true;
		}

		internal static void UnlockLocalizeSetting()
		{
			localizeSettingLock_ = false;
		}

		public static void PushLocalizeSetting(bool scale)
		{
			if (localizeSettingCount_ >= localizeSettingStack_.Count)
			{
				localizeSettingStack_.Add(true);
			}
			localizeSettingStack_[localizeSettingCount_] = scale;
			localizeSettingCount_++;
		}

		public static void PopLocalizeSetting()
		{
			if (localizeSettingCount_ > 0)
			{
				localizeSettingCount_--;
			}
		}

		internal static void ClearLocalizeSettings()
		{
			localizeSettingCount_ = 0;
		}

		internal static void SetScale(int scale)
		{
			requestFontScale_ = scale;
		}

		private void UpdateFontScale()
		{
			if (requestFontScale_ != fontScale_)
			{
				fontScale_ = requestFontScale_;
				SetSize(GetSize());
			}
		}

		private static UnityEngine.Font GetDefaultGUIFont()
		{
			IApplication currentApp = IApplication.GetCurrentApp();
			if (currentApp.CheckUiThread())
			{
				return _getDefaultGUIFont();
			}
			return (UnityEngine.Font)currentApp.RunOnUiThread(() => _getDefaultGUIFont(), true);
		}

		private static UnityEngine.Font _getDefaultGUIFont()
		{
			if (defaultGUIFontLanguage_ != Language.Get())
			{
				if (defaultGUIFontLanguage_ != -1)
				{
					invalidFontNames_.Add(GetFontKey(defaultGUIFont_));
				}
				defaultGUIFont_ = null;
				if (Language.Japanese())
				{
					defaultGUIFont_ = (UnityEngine.Font)Resources.Load("Fonts/default-ja");
				}
				if (defaultGUIFont_ == null)
				{
					defaultGUIFont_ = (UnityEngine.Font)Resources.Load("Fonts/default");
				}
				defaultGUIFontLanguage_ = Language.Get();
				invalidFontNames_.Remove(GetFontKey(defaultGUIFont_));
				TextLayout.ClearCache();
			}
			return defaultGUIFont_;
		}

		private UnityEngine.Font UpdateGUIFont()
		{
			IApplication currentApp = IApplication.GetCurrentApp();
			if (currentApp.CheckUiThread())
			{
				return _updateGUIFont();
			}
			return (UnityEngine.Font)currentApp.RunOnUiThread(() => _updateGUIFont(), true);
		}

		private UnityEngine.Font _updateGUIFont()
		{
			UnityEngine.Font defaultGUIFont = GetDefaultGUIFont();
			string fontKey = GetFontKey(guiStyle_.font);
			for (int i = 0; i < invalidFontNames_.Count; i++)
			{
				if (invalidFontNames_[i].Length == fontKey.Length && invalidFontNames_[i] == fontKey)
				{
					guiStyle_.font = defaultGUIFont;
					break;
				}
			}
			return guiStyle_.font;
		}

		private static string GetFontKey(UnityEngine.Font font)
		{
			if (font == null)
			{
				return "*";
			}
			return font.name;
		}
	}
}
