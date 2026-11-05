using System;
using System.Collections.Generic;
using System.Text;
using kairo.common.cfg;
using kairo.unity.io;
using kairo.unity.ui;
using kairo.unity.util;
using UnityEngine;

namespace kairo.unity.graphics
{
	public class ShaderBuilder
	{
		private class Node
		{
			public Node Parent;

			public int[] Flags = new int[1];

			public List<Node> Nodes = new List<Node>();

			public Node(params int[] flags)
			{
				if (flags != null && flags.Length != 0)
				{
					Flags = new int[flags.Length];
					Array.Copy(flags, Flags, Flags.Length);
				}
			}

			public Node Add(params Node[] nodes)
			{
				Nodes.Clear();
				if (nodes == null)
				{
					return this;
				}
				for (int i = 0; i < nodes.Length; i++)
				{
					Nodes.Add(nodes[i].Clone());
				}
				return this;
			}

			public Node Add(Node node)
			{
				node = node.Clone();
				node.Parent = this;
				Nodes.Add(node);
				return node;
			}

			public Node Add(params int[] flags)
			{
				Node node = New(flags);
				Nodes.Add(node);
				node.Parent = this;
				return node;
			}

			public static Node New(params int[] flags)
			{
				return new Node(flags);
			}

			public static Node New(Node copy)
			{
				Node node = new Node(copy.Flags);
				node.Parent = copy.Parent;
				for (int i = 0; i < copy.Nodes.Count; i++)
				{
					node.Nodes.Add(copy.Nodes[i].Clone());
				}
				return node;
			}

			public Node Clone()
			{
				return New(this);
			}

			public Node Top()
			{
				Node node = this;
				while (node.Parent != null)
				{
					node = node.Parent;
				}
				return node;
			}
		}

		public bool setClip;

		public bool useClip;

		public bool maskMode;

		public bool useTexture;

		public bool useBlendColor;

		public int renderOperator;

		public int renderSrcAlpha;

		public int renderDstAlpha;

		public bool isGUI;

		public int alphaType;

		public int opaTransOrder;

		public bool untiColor;

		public bool clearDepth;

		public bool clearColor;

		public bool clearStencil;

		private const int NAMEBIT_GUI = 1;

		private const int NAMEBIT_SETCLIP = 2;

		private const int NAMEBIT_USECLIP = 4;

		private const int NAMEBIT_MASKMODE = 8;

		private const int NAMEBIT_TEXTURE = 16;

		private const int NAMEBIT_BLEND = 32;

		private const int NAMEBIT_ADD = 64;

		private const int NAMEBIT_SUB = 128;

		private const int NAMEBIT_OPAQUE = 256;

		private const int NAMEBIT_TRANSPARENT = 512;

		private const int NAMEBIT_ALPHATEST = 1024;

		private const int NAMEBIT_OPATRANS1 = 2048;

		private const int NAMEBIT_OPATRANS2 = 4096;

		private const int NAMEBIT_INDEX0 = 8192;

		private const int NAMEBIT_INDEX1 = 16384;

		private const int NAMEBIT_INDEX2 = 32768;

		private const int NAMEBIT_UNTICOLOR = 65536;

		private const int NAMEBIT_CLEAR_COLOR = 131072;

		private const int NAMEBIT_CLEAR_DEPTH = 262144;

		private const int NAMEBIT_CLEAR_STENCIL = 524288;

		private const int NAMEBIT_CLEAR = 917504;

		private static Dictionary<int, string> nameCache_ = new Dictionary<int, string>();

		public void Init(kairo.unity.ui.Graphics g, Image img = null, float alpha = 1f, bool clipRegion = false, int opaTransOrder = -1, bool clearDepth = false, bool clearColor = false, bool clearStencil = false)
		{
			Init(g, img, null, alpha, clipRegion, opaTransOrder, clearDepth, clearColor, clearStencil);
		}

		public void Init(kairo.unity.ui.Graphics g, Image img, Color[] colors, float alpha = 1f, bool clipRegion = false, int opaTransOrder = -1, bool clearDepth = false, bool clearColor = false, bool clearStencil = false)
		{
			if (g.setFigureClip_)
			{
				clipRegion = true;
			}
			useTexture = img != null;
			renderOperator = g.renderOperator_;
			renderSrcAlpha = g.renderSrcRatio_;
			renderDstAlpha = g.renderDstRatio_;
			isGUI = g.IsRenderGUIMode();
			if (renderOperator == 1 && renderSrcAlpha >= 255 && renderDstAlpha == 0)
			{
				renderOperator = 0;
			}
			useBlendColor = !clipRegion && g.blendMode_ == 1;
			setClip = clipRegion;
			useClip = g.clipEnable_;
			maskMode = setClip && g.setFigureClip_ && g.isFigureClipMask_;
			untiColor = !setClip && img != null && alpha >= 0.99f && renderOperator == 0 && renderSrcAlpha >= 255 && g.blendMode_ == 0;
			if (untiColor && colors != null)
			{
				for (int i = 0; i < colors.Length; i++)
				{
					Color color = colors[i];
					if (color.r < 1f || color.g < 1f || color.b < 1f)
					{
						untiColor = false;
						break;
					}
				}
			}
			this.clearDepth = clearDepth;
			this.clearColor = clearColor;
			this.clearStencil = clearStencil;
			alphaType = -1;
			if (img != null)
			{
				alphaType = img.GetAlphaType();
			}
			this.opaTransOrder = opaTransOrder;
			switch (opaTransOrder)
			{
			case 1:
				alphaType = 1;
				break;
			case 2:
				alphaType = 2;
				break;
			}
			if (renderOperator != 0)
			{
				alphaType = 2;
			}
			if (alpha < 0.01f)
			{
				if (alphaType == 0)
				{
					alphaType = 1;
				}
			}
			else if (alpha <= 0.99f && alphaType != 2)
			{
				alphaType = 2;
			}
		}

		public Shader ToShader()
		{
			if (Config.PRINT)
			{
				Log.Info("ｼｪｰﾀﾞｰのﾗﾝﾀｲﾑ生成:" + GetName());
			}
			UnityEngine.Material material = new UnityEngine.Material(GetCode());
			Shader shader = material.shader;
			UnityEngine.Object.Destroy(material);
			return shader;
		}

		public string GetCode()
		{
			return GetCode(GetNameId());
		}

		private static string GetCode(int nameId)
		{
			string text = null;
			string name = GetName(nameId);
			string text2 = "";
			string text3 = "";
			string text4 = "";
			string text5 = "";
			string text6 = "ZWrite Off";
			string text7 = "";
			string text8 = "Geometry";
			string text9 = "Opaque";
			string text10 = "";
			string text11 = "";
			int num = 0;
			if ((nameId & 0xE0000) != 0)
			{
				string newValue = "";
				string newValue2 = "";
				if ((nameId & 0x20000) == 0)
				{
					newValue = "ColorMask 0";
				}
				if ((nameId & 0x40000) != 0)
				{
					text6 = "ZWrite On";
				}
				if ((nameId & 0x80000) != 0)
				{
					newValue2 = "Stencil { Ref 0 Comp always Pass replace }";
				}
				text5 = "\r\n                BindChannels {\r\n                    Bind 'vertex', vertex\r\n                    Bind 'color', color\r\n                }";
				text = "\r\n                Shader '<name>' {\r\n\t\t\t        SubShader {\r\n\t\t\t            Tags {'Queue'='Geometry' 'IgnoreProjector'='True' 'RenderType'='Opaque'}\r\n                        Pass {\r\n                            Lighting Off <depth> Cull Off ZTest Always\r\n\t\t\t                <colorMask>\r\n                            <bind>\r\n\t\t\t                <stencil>\r\n                        }\r\n\t\t\t        }\r\n\t\t\t    }";
				text = text.Replace("<colorMask>", newValue);
				text = text.Replace("<stencil>", newValue2);
			}
			else if ((nameId & 2) != 0)
			{
				string newValue3 = "1";
				if ((nameId & 8) != 0)
				{
					newValue3 = "0";
				}
				text = "\r\n                Shader '<name>' {\r\n\t\t\t        Properties {\r\n\t\t\t            _MainTex ('Base (RGB)', 2D)  = 'white' {}\r\n\t\t\t        }\r\n\t\t\t        SubShader {\r\n\t\t\t            Tags {'Queue'='Transparent' 'IgnoreProjector'='True' 'RenderType'='Transparent'}\r\n                        Pass {\r\n                            Lighting Off ZWrite Off Cull Off\r\n                            Blend Zero One\r\n                            AlphaTest GEqual 0.01\r\n                            Stencil { Ref <stencil> Comp always Pass replace }\r\n                            <tex>\r\n                        }\r\n\t\t\t        }\r\n\t\t\t    }";
				if ((nameId & 0x10) != 0)
				{
					text4 = "SetTexture [_MainTex] { combine texture }";
				}
				text = text.Replace("<stencil>", newValue3);
			}
			else
			{
				if ((nameId & 4) != 0)
				{
					text2 = "Stencil { Ref 0 Comp NotEqual }";
				}
				if ((nameId & 0x10000) == 0)
				{
					text5 = "\r\n                    BindChannels {\r\n                        Bind 'vertex', vertex\r\n                        Bind 'texcoord', texcoord\r\n                        Bind 'color', color\r\n                    }";
				}
				if ((nameId & 1) == 0)
				{
					if ((nameId & 0x200) != 0)
					{
						text6 = "ZWrite Off";
						text8 = "Transparent";
						text9 = "Transparent";
					}
					else if ((nameId & 0x400) != 0)
					{
						text6 = "ZWrite On";
						text7 = "AlphaTest GEqual 0.01";
						text8 = "AlphaTest";
						text9 = "TransparentCutout";
					}
					else
					{
						text6 = "ZWrite On";
						text8 = "Geometry";
						text9 = "Opaque";
					}
					if ((nameId & 0x8000) != 0)
					{
						num--;
					}
					if ((nameId & 0x800) != 0)
					{
						text7 = "AlphaTest Greater 0.99";
					}
					else if ((nameId & 0x1000) != 0)
					{
						text7 = "AlphaTest LEqual  0.99";
					}
				}
				if ((nameId & 0x10) != 0)
				{
					if ((nameId & 0x20) == 0)
					{
						text4 = (((nameId & 0x10000) != 0) ? "SetTexture [_MainTex] { combine texture }" : "SetTexture [_MainTex] { combine texture * primary }");
					}
					else
					{
						text10 = "\r\n                        _BlendColor ('BlendColor', Color) = (1,1,1,1)\r\n                        _BlendAlpha ('BlendAlpha', Range(0,1)) = 1\r\n                        ";
						text4 = "\r\n                        SetTexture [_MainTex] {\r\n                            ConstantColor [_BlendColor]\r\n                            combine constant * texture alpha\r\n                        }\r\n\r\n                        SetTexture [_MainTex] {\r\n                            ConstantColor (1,1,1,[_BlendAlpha])\r\n                            combine previous lerp(constant) texture\r\n                        }\r\n\r\n                        SetTexture [_MainTex] {\r\n                            combine previous * primary\r\n                        }\r\n                        ";
					}
				}
				if ((nameId & 0x40) != 0)
				{
					if ((nameId & 0x2000) != 0)
					{
						text3 = "Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha";
					}
					else if ((nameId & 0x4000) != 0)
					{
						text3 = "Blend SrcAlpha One";
					}
					else if ((nameId & 0x8000) != 0)
					{
						text3 = "Blend Zero OneMinusSrcAlpha, One OneMinusSrcAlpha";
					}
				}
				else if ((nameId & 0x80) == 0)
				{
					text3 = (((nameId & 0x100) == 0) ? "Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha" : "Blend Off");
				}
				else if ((nameId & 0x2000) != 0)
				{
					text3 = "Blend SrcAlpha One, One OneMinusSrcAlpha BlendOp RevSub";
				}
				else if ((nameId & 0x4000) != 0)
				{
					text3 = "Blend SrcAlpha One BlendOp RevSub";
				}
				else if ((nameId & 0x8000) != 0)
				{
					text3 = "Blend Zero OneMinusSrcAlpha, One OneMinusSrcAlpha";
				}
				if (num != 0)
				{
					text8 = text8 + ((num > 0) ? "+" : "") + num;
				}
				text = "\r\n                Shader '<name>' {\r\n\t\t\t        Properties {\r\n\t\t\t            _MainTex ('Base (RGB)', 2D)  = 'white' {}\r\n                        <prop_color>\r\n                        <prop_blend>\r\n\t\t\t        }\r\n\t\t\t        SubShader {\r\n\t\t\t            Tags {'Queue'='<tag_queue>' 'IgnoreProjector'='True' 'RenderType'='<tag_render>'}\r\n                        Pass {\r\n                            Lighting Off <depth> Cull Off\r\n                            <alpha>\r\n                            <blend>\r\n                            <clip>\r\n                            <bind>\r\n                            <tex>\r\n                        }\r\n\t\t\t        }\r\n\t\t\t    }";
			}
			text = text.Replace("<name>", name.TrimStart('\r', '\n'));
			text = text.Replace("<prop_blend>", text10.TrimStart('\r', '\n'));
			text = text.Replace("<prop_color>", text11.TrimStart('\r', '\n'));
			text = text.Replace("<tag_queue>", text8.TrimStart('\r', '\n'));
			text = text.Replace("<tag_render>", text9.TrimStart('\r', '\n'));
			text = text.Replace("<depth>", text6.TrimStart('\r', '\n'));
			text = text.Replace("<alpha>", text7.TrimStart('\r', '\n'));
			text = text.Replace("<blend>", text3.TrimStart('\r', '\n'));
			text = text.Replace("<clip>", text2.TrimStart('\r', '\n'));
			text = text.Replace("<bind>", text5.TrimStart('\r', '\n'));
			text = text.Replace("<tex>", text4.TrimStart('\r', '\n'));
			return text.Replace("'", "\"");
		}

		public string GetName()
		{
			return GetName(GetNameId());
		}

		private static string GetName(int nameBits)
		{
			string value;
			if (nameCache_.TryGetValue(nameBits, out value))
			{
				return value;
			}
			value = "Kairo/";
			if ((nameBits & 0xE0000) != 0)
			{
				value += "Clear/";
				if ((nameBits & 0x20000) != 0)
				{
					value += "Color";
				}
				if ((nameBits & 0x40000) != 0)
				{
					value += "Depth";
				}
				if ((nameBits & 0x80000) != 0)
				{
					value += "Stencil";
				}
			}
			else
			{
				if ((nameBits & 1) != 0)
				{
					value += "GUI/";
				}
				if ((nameBits & 2) != 0)
				{
					value = (((nameBits & 8) == 0) ? (value + "Clip/") : (value + "Mask/"));
				}
				if ((nameBits & 0x10000) != 0)
				{
					value += "Unc/";
				}
				if ((nameBits & 4) != 0)
				{
					value += "Clipped";
				}
				value = (((nameBits & 0x10) == 0) ? (value + "Shape") : (value + "Texture"));
				if ((nameBits & 0x20) != 0)
				{
					value += "Blend";
				}
				if ((nameBits & 0x40) != 0)
				{
					value += "-Add";
				}
				if ((nameBits & 0x80) != 0)
				{
					value += "-Sub";
				}
				if ((nameBits & 0x2000) != 0)
				{
					value += "0";
				}
				if ((nameBits & 0x4000) != 0)
				{
					value += "1";
				}
				if ((nameBits & 0x8000) != 0)
				{
					value += "2";
				}
				if ((nameBits & 0x100) != 0)
				{
					value += "-Opaque";
				}
				if ((nameBits & 0x200) != 0)
				{
					value += "-Trans";
				}
				if ((nameBits & 0x400) != 0)
				{
					value += "-Alpha";
				}
				if ((nameBits & 0x800) != 0)
				{
					value += "OT1";
				}
				if ((nameBits & 0x1000) != 0)
				{
					value += "OT2";
				}
			}
			nameCache_[nameBits] = value;
			return value;
		}

		public int GetNameId()
		{
			int num = 0;
			if (clearDepth)
			{
				num |= 0x40000;
			}
			if (clearColor)
			{
				num |= 0x20000;
			}
			if (clearStencil)
			{
				num |= 0x80000;
			}
			if (num != 0)
			{
				return num;
			}
			if (isGUI)
			{
				num |= 1;
			}
			if (useTexture)
			{
				num |= 0x10;
			}
			if (setClip)
			{
				num |= 2;
				if (maskMode)
				{
					num |= 8;
				}
			}
			else
			{
				if (useClip)
				{
					num |= 4;
				}
				if (useBlendColor)
				{
					num |= 0x20;
				}
				if (untiColor)
				{
					num |= 0x10000;
				}
				if (renderOperator == 1)
				{
					num |= 0x40;
					num = ((renderSrcAlpha + renderDstAlpha == 255) ? (num | 0x2000) : ((renderDstAlpha != 255) ? (num | 0x8000) : (num | 0x4000)));
				}
				else if (renderOperator == 2)
				{
					num |= 0x80;
					num = ((renderSrcAlpha + renderDstAlpha == 255) ? (num | 0x2000) : ((renderSrcAlpha <= 0) ? (num | 0x8000) : (num | 0x4000)));
				}
				else if (alphaType == 0)
				{
					num |= 0x100;
				}
				if (!isGUI)
				{
					if (alphaType == 2)
					{
						num |= 0x200;
					}
					else if (alphaType == 1)
					{
						num |= 0x400;
					}
					if (opaTransOrder == 1)
					{
						num |= 0x800;
					}
					if (opaTransOrder == 2)
					{
						num |= 0x1000;
					}
				}
			}
			return num;
		}

		public override bool Equals(object obj)
		{
			string name = GetName();
			if (obj != null)
			{
				if (obj is Shader)
				{
					return name.Equals(((Shader)obj).name);
				}
				if (obj is string)
				{
					return name.Equals((string)obj);
				}
			}
			return base.Equals(obj);
		}

		public override int GetHashCode()
		{
			return GetName().GetHashCode();
		}

		public static void OutputShaders()
		{
			Node copy = Node.New(2).Add(0, 8);
			Node copy2 = Node.New(0, 4);
			Node node = Node.New(8192, 16384, 32768);
			Node node2 = Node.New(0, 512, 1024);
			Node node3 = Node.New(0, 2048, 4096);
			Node node4 = Node.New(0, 32, 65536);
			Node node5 = new Node();
			node5.Add(Node.New(131072, 262144, 524288).Add(131072, 262144, 524288).Add(131072, 262144, 524288)
				.Top(), Node.New(1).Add(0, 16).Add(Node.New(copy).Top(), Node.New(copy2).Add(node4).Add(Node.New(64, 128).Add(node).Top(), Node.New(256, 0).Top())
				.Top())
				.Top(), Node.New(default(int)).Add(0, 16).Add(Node.New(copy).Top(), Node.New(copy2).Add(node4).Add(Node.New(64, 128).Add(node).Add(node2)
				.Add(node3)
				.Top(), Node.New(256, 0).Add(node2).Add(node2)
				.Add(node3)
				.Top())
				.Top())
				.Top());
			OutputShaders(node5, 0);
		}

		private static void OutputShaders(Node node, int flag)
		{
			for (int i = 0; i < node.Flags.Length; i++)
			{
				int num = flag | node.Flags[i];
				if (node.Nodes.Count == 0)
				{
					string name = GetName(num);
					string code = GetCode(num);
					string filename = "shader/" + name + ".shader";
					string[] array = code.Replace("\t", "    ").Split('\n');
					string text = "";
					int num2 = 0;
					for (int j = 0; j < array.Length; j++)
					{
						if (array[j].Trim().Length == 0)
						{
							continue;
						}
						string text2 = (array[j] = array[j].Trim());
						if (text2.Length > 0 && text2[0] == '}')
						{
							num2--;
						}
						for (int k = 0; k < num2; k++)
						{
							array[j] = "\t" + array[j];
						}
						for (int l = 0; l < text2.Length; l++)
						{
							if (text2[l] == '{')
							{
								num2++;
							}
							if (text2[l] == '}' && l > 0)
							{
								num2--;
							}
						}
						if (text.Length > 0)
						{
							text += "\n";
						}
						text += array[j];
					}
					Storage.Write(1, false, filename, Encoding.UTF8.GetBytes(text));
					continue;
				}
				foreach (Node node2 in node.Nodes)
				{
					OutputShaders(node2, num);
				}
			}
		}
	}
}
