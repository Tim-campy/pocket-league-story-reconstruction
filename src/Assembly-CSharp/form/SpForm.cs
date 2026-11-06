using System;
using System.IO;
using System.Threading;
using java.io;
using java.lang;
using java.util;
using kairo.common.cfg;
using kairo.unity.io;
using kairo.unity.surface;
using kairo.unity.ui;
using kairo.unity.util;
using main;
using surface;

namespace form
{
	public class SpForm : MyFormBase, JRunnable
	{
		private const int STATE_MAIN = 0;

		private const int STATE_SEND = 1;

		private const int STATE_RECV = 2;

		private int frame_;

		private int page_;

		private string text_;

		private string date_;

		private bool succeed_;

		private int state_;

		private static readonly int[][] softLabels_ = new int[3][]
		{
			new int[2] { 0, 2 },
			new int[2],
			new int[2]
		};

		private Vector commands_;

		private int select_;

		private int send_;

		private JThread th_;

		private Vector task_;

		private byte[] recv_;

		private int taskMax_;

		public SpForm()
			: base(3)
		{
		}

		public SpForm(Exception th)
			: base(3)
		{
			text_ = null;
			if (th == null)
			{
				return;
			}
			if (Config.PTW)
			{
				text_ = string.Empty;
			}
			if (Http.GetErrorUrl() != null)
			{
				string text = Http.GetErrorUrl();
				int num = text.LastIndexOf('/');
				if (num != -1)
				{
					text = text.SubstringJ(num + 1);
				}
				num = text.LastIndexOf('.');
				if (num != -1)
				{
					text = text.SubstringJ(0, num);
				}
				string text2 = text_;
				text_ = text2 + text + "," + Http.GetErrorCode() + "\n";
			}
			string text3 = string.Empty + th;
			string text4 = th.GetType().Name;
			int num2 = text4.LastIndexOf('.');
			if (num2 != -1)
			{
				text4 = text4.SubstringJ(num2 + 1);
			}
			num2 = text3.IndexOf(text4);
			if (num2 != -1)
			{
				text3 = text3.SubstringJ(num2);
			}
			if (th is ConnectionException)
			{
				text3 = text3 + "," + ((ConnectionException)th).GetStatus();
			}
			if (th is UIException)
			{
				text3 = text3 + "," + ((UIException)th).GetStatus();
			}
			text_ += text3;
		}

		public override string ToString()
		{
			return "SpForm:" + state_;
		}

		public override bool Init()
		{
			base.Init();
			ChangeState(0);
			bool flag = Config.APPLI_TYPE == 0 || Config.APPLI_TYPE == 1;
			commands_ = new Vector();
			if (!flag)
			{
				AddCommand(10, MyFormBase.LT("中断ﾃﾞｰﾀ<0>", "1"));
			}
			AddCommand(0, MyFormBase.LT("ｾｰﾌﾞﾃﾞｰﾀ<0>", "1"));
			if (!flag)
			{
				AddCommand(11, MyFormBase.LT("中断ﾃﾞｰﾀ<0>", "2"));
			}
			AddCommand(1, MyFormBase.LT("ｾｰﾌﾞﾃﾞｰﾀ<0>", "2"));
			AddCommand(-1, "全てのﾃﾞｰﾀを送信");
			if (Config.APPLI_TYPE == 0)
			{
				AddCommand(-2, "まるごと送信");
			}
			if (text_ != null)
			{
				AddCommand(-3, "エラー内容を表示する");
			}
			if (Config.DEBUG && !Config.PTW)
			{
				AddCommand(-4, "セーブデータを受信する");
			}
			JCalendar instance = JCalendar.GetInstance();
			instance.SetTime(new JDate(JSystem.CurrentTimeMillis()));
			string text = string.Empty + instance.Get(1);
			string text2 = string.Empty + (instance.Get(2) + 1);
			if (text2.Length() == 1)
			{
				text2 = "0" + text2;
			}
			string text3 = string.Empty + instance.Get(5);
			if (text3.Length() == 1)
			{
				text3 = "0" + text3;
			}
			string text4 = string.Empty + instance.Get(11);
			if (text4.Length() == 1)
			{
				text4 = "0" + text4;
			}
			string text5 = string.Empty + instance.Get(12);
			if (text5.Length() == 1)
			{
				text5 = "0" + text5;
			}
			string text6 = string.Empty + instance.Get(13);
			if (text6.Length() == 1)
			{
				text6 = "0" + text6;
			}
			date_ = string.Empty + text + "." + text2 + "." + text3 + "_" + text4 + "." + text5 + "." + text6 + string.Empty;
			return true;
		}

		internal void AddCommand(int id, string str)
		{
			RecordStore recordStore = null;
			int num = 0;
			if (id >= 0)
			{
				bool flag = id / 10 == 1;
				int num2 = id % 10;
				string text = apdat_.sysSave_.strs[((!flag) ? 2 : 6) + num2];
				string text2 = apdat_.sysSave_.strs[((!flag) ? 4 : 8) + num2];
				int num3 = apdat_.sysSave_.ints[((!flag) ? 6 : 11) + num2];
				if (num3 == -1)
				{
					commands_.AddElement(new string[2]
					{
						JString.ValueOf(id),
						str
					});
					return;
				}
				try
				{
					recordStore = RecordStore.OpenRecordStore(0);
					num = recordStore.GetRecordSize(((!flag) ? 1 : 3) + num2);
				}
				catch (Exception)
				{
				}
				finally
				{
					if (recordStore != null)
					{
						try
						{
							recordStore.CloseRecordStore();
						}
						catch (Exception)
						{
						}
					}
				}
				commands_.AddElement(new string[6]
				{
					JString.ValueOf(id),
					str,
					JString.ValueOf(num),
					text,
					JString.ValueOf(num3),
					text2
				});
				return;
			}
			switch (id)
			{
			case -1:
				try
				{
					if (Config.APPLI_TYPE == 0)
					{
						num = ScratchPad.GetSPSize(0);
					}
					else
					{
						recordStore = RecordStore.OpenRecordStore(0);
						int numRecords = recordStore.GetNumRecords();
						for (int i = 0; i < numRecords; i++)
						{
							num += recordStore.GetRecordSize(i);
						}
					}
				}
				catch (Exception)
				{
				}
				finally
				{
					if (recordStore != null)
					{
						try
						{
							recordStore.CloseRecordStore();
						}
						catch (Exception)
						{
						}
					}
				}
				commands_.AddElement(new string[3]
				{
					JString.ValueOf(id),
					str,
					JString.ValueOf(num)
				});
				break;
			case -2:
				try
				{
					num = ScratchPad.GetSPSize(0) + ScratchPad.GetSPSize(1);
				}
				catch (Exception)
				{
				}
				commands_.AddElement(new string[3]
				{
					JString.ValueOf(id),
					str,
					JString.ValueOf(num)
				});
				break;
			default:
				commands_.AddElement(new string[3]
				{
					JString.ValueOf(id),
					str,
					JString.ValueOf(-1)
				});
				break;
			}
		}

		public override bool Update()
		{
			if (dialog_ != null)
			{
				if (state_ == 0 && dialog_.result_ == 0)
				{
					string[] array = (string[])commands_.ElementAt(select_);
					int num = JInteger.ParseInt(array[0]);
					send_ = select_;
					task_ = null;
					taskMax_ = 0;
					if (num == -4)
					{
						ChangeState(2);
					}
					else
					{
						ChangeState(1);
					}
					th_ = new JThread(this);
					th_.Start();
				}
				else if (state_ == 1 || state_ == 2)
				{
					if (dialog_.result_ == 0)
					{
						th_ = new JThread(this);
						th_.Start();
					}
					else
					{
						if (task_ != null)
						{
							task_.RemoveAllElements();
						}
						task_ = null;
						ChangeState(0);
					}
				}
				dialog_ = null;
			}
			frame_ = (frame_ + 1) % int.MaxValue;
			if (state_ == 0)
			{
				if (canvas_.CheckKeyRepeat(131072))
				{
					select_ = (select_ - 1 + commands_.Size()) % commands_.Size();
				}
				else if (canvas_.CheckKeyRepeat(524288))
				{
					select_ = (select_ + 1) % commands_.Size();
				}
				else if (canvas_.CheckKeyPulse(1048576))
				{
					string[] array2 = (string[])commands_.ElementAt(select_);
					if (array2.Length > 2)
					{
						string empty = string.Empty;
						int num2 = JInteger.ParseInt(array2[0]);
						if (num2 != -3)
						{
							switch (num2)
							{
							case -4:
								ShowDialog("セーブデータを受信します", "はい\tいいえ");
								return true;
							case -1:
								ShowDialog("全てのデータを送信します", "はい\tいいえ");
								break;
							case -2:
								ShowDialog("まるごと送信します", "はい\tいいえ");
								break;
							default:
							{
								int i = num2 % 10 + 1;
								if (num2 / 10 == 1)
								{
									ShowDialog(MyFormBase.LT("中断データ<0> を送信します", JString.ValueOf(i)), "はい\tいいえ");
								}
								else
								{
									ShowDialog(MyFormBase.LT("セーブデータ<0> を送信します", JString.ValueOf(i)), "はい\tいいえ");
								}
								break;
							}
							}
							return true;
						}
						canvas_.ImeOnBlock(text_, 0, 1);
					}
				}
				else if (apdat_.IsPushSoftLabel(2))
				{
					return Pop();
				}
			}
			else
			{
				if (state_ == 1)
				{
					if (th_ != null)
					{
						return true;
					}
					if (succeed_)
					{
						apdat_.sysSave_.strs[10] = string.Empty;
						try
						{
							apdat_.SaveSystem();
						}
						catch (Exception)
						{
						}
						Dialog dialog = new Dialog(0, "完了");
						dialog.SetText("送信を完了しました。\nご協力ありがとうございます。\n今後もカイロパークを宜しくお願い致します。");
						dialog.Show();
						return Pop();
					}
					ShowDialog("エラーが発生しました\nリトライしますか？", "はい\tいいえ");
					return true;
				}
				if (state_ == 2)
				{
					if (th_ != null)
					{
						return true;
					}
					if (succeed_)
					{
						try
						{
							if (7 <= recv_.Length && recv_[0] == 115 && recv_[1] == 97 && recv_[2] == 118 && recv_[3] == 101)
							{
								ByteArrayInputStream byteArrayInputStream = new ByteArrayInputStream(recv_);
								byteArrayInputStream.Skip(4L);
								int num3 = StreamUtil.ReadByte(byteArrayInputStream);
								int num4 = StreamUtil.ReadByte(byteArrayInputStream);
								int num5 = StreamUtil.ReadByte(byteArrayInputStream);
								if (num5 >= 0)
								{
									byte[] data = StreamUtil.Read(byteArrayInputStream, true);
									RecordStore.WriteRecord(0, 1, data);
									try
									{
										Property property = new Property(AppData.defSaveP_, AppData.defSaveI_, AppData.defSaveL_, AppData.defSaveS_, AppData.defSaveB_);
										property.Load(data);
										apdat_.sysSave_.ints[6] = property.ints[1] * 100 * 100 + property.ints[2] * 100 + property.ints[3];
										apdat_.sysSave_.strs[4] = JString.ValueOf(property.lngs[0]);
										apdat_.sysSave_.strs[2] = property.strs[0];
										apdat_.SaveSystem();
									}
									catch (Exception)
									{
									}
								}
								if (num5 == -1)
								{
									if (Config.APPLI_TYPE == 0)
									{
										byte[] data = StreamUtil.Read(byteArrayInputStream, true);
										ScratchPad.WriteByteArray(0, 0, data);
									}
									else
									{
										RecordStore recordStore = null;
										try
										{
											recordStore = RecordStore.OpenRecordStore(0);
											int numRecords = recordStore.GetNumRecords();
											for (int j = 0; j < numRecords; j++)
											{
												byte[] data = StreamUtil.Read(byteArrayInputStream, true);
												recordStore.WriteRecord(j, data);
												if (j != 0)
												{
													try
													{
														Property property2 = new Property(AppData.defSaveP_, AppData.defSaveI_, AppData.defSaveL_, AppData.defSaveS_, AppData.defSaveB_);
														property2.Load(data);
														bool flag = j == 3 || j == 4;
														int num6 = ((j != 3 && j != 1) ? 1 : 0);
														apdat_.sysSave_.ints[((!flag) ? 6 : 11) + num6] = property2.ints[1] * 100 * 100 + property2.ints[2] * 100 + property2.ints[3];
														apdat_.sysSave_.strs[((!flag) ? 4 : 8) + num6] = JString.ValueOf(property2.lngs[0]);
														apdat_.sysSave_.strs[((!flag) ? 2 : 6) + num6] = property2.strs[0];
													}
													catch (Exception)
													{
													}
												}
											}
										}
										finally
										{
											if (recordStore != null)
											{
												recordStore.CloseRecordStore();
											}
											recordStore = null;
										}
										try
										{
											apdat_.SaveSystem();
										}
										catch (Exception)
										{
										}
									}
								}
								if (num5 == -2)
								{
									byte[] data = StreamUtil.Read(byteArrayInputStream, true);
									ScratchPad.WriteByteArray(0, 0, data);
									data = StreamUtil.Read(byteArrayInputStream, true);
									ScratchPad.WriteByteArray(1, 0, data);
								}
								byteArrayInputStream.Close();
							}
							else
							{
								if (recv_.Length == 0)
								{
									Dialog dialog2 = new Dialog(0, "失敗");
									dialog2.SetText("ファイルが見つかりません。");
									dialog2.Show();
									ChangeState(0);
									return true;
								}
								RecordStore.WriteRecord(0, 1, recv_);
								try
								{
									Property property3 = new Property(AppData.defSaveP_, AppData.defSaveI_, AppData.defSaveL_, AppData.defSaveS_, AppData.defSaveB_);
									property3.Load(recv_);
									apdat_.sysSave_.ints[6] = property3.ints[1] * 100 * 100 + property3.ints[2] * 100 + property3.ints[3];
									apdat_.sysSave_.strs[4] = JString.ValueOf(property3.lngs[0]);
									apdat_.sysSave_.strs[2] = property3.strs[0];
									apdat_.SaveSystem();
								}
								catch (Exception)
								{
								}
							}
						}
						catch (Exception)
						{
						}
						Dialog dialog3 = new Dialog(0, "完了");
						dialog3.SetText("受信完了しました。");
						dialog3.Show();
						return Pop();
					}
					ShowDialog("エラーが発生しました\nリトライしますか？", "はい\tいいえ");
					return true;
				}
			}
			return true;
		}

		public override void Draw(Graphics g)
		{
			int num = 0;
			int num2 = 0;
			num += (view_.GetGameWidth() - 240) / 2;
			num2 += (view_.GetGameHeight() - 240) / 2;
			g.SetOrigin(num, num2);
			try
			{
				_draw(g);
			}
			finally
			{
				g.SetOrigin(0, 0);
			}
		}

		private void _draw(Graphics g)
		{
			if (!apdat_.IsTopForm(this))
			{
				return;
			}
			int num = 48;
			for (int i = 0; i < commands_.Size(); i++)
			{
				string[] array = (string[])commands_.ElementAt(i);
				int num2 = JInteger.ParseInt(array[0]);
				if (num2 >= 0) { num = (num + 35); } else { num = (num + 19); }
			}
			int[] array2 = null;
			try
			{
				array2 = DrawWindow(g, 220, num, "セーブデータの送信", 6, 6, 21, 80, MyFormBase.WNDFLAG_BOX);
			}
			catch (Exception)
			{
				array2 = new int[MyFormBase.WNDRET_BOX_END];
				array2[MyFormBase.WNDRET_TOP] = 120 - num / 2 + 15;
				array2[MyFormBase.WNDRET_LEFT] = 10;
				array2[MyFormBase.WNDRET_RIGHT] = array2[MyFormBase.WNDRET_LEFT] + 220;
				array2[MyFormBase.WNDRET_BOTTOM] = array2[MyFormBase.WNDRET_TOP] + num - 15;
				array2[MyFormBase.WNDRET_BOX_LEFT] = array2[MyFormBase.WNDRET_LEFT] + 6;
				array2[MyFormBase.WNDRET_BOX_RIGHT] = array2[MyFormBase.WNDRET_RIGHT] - 6;
				array2[MyFormBase.WNDRET_BOX_TOP] = array2[MyFormBase.WNDRET_TOP] + 6;
				array2[MyFormBase.WNDRET_BOX_BOTTOM] = array2[MyFormBase.WNDRET_BOTTOM] - 21;
			}
			int num3 = array2[MyFormBase.WNDRET_BOX_LEFT] + 10;
			int num4 = 0;
			num4 = array2[MyFormBase.WNDRET_BOX_TOP] + 6;
			if (state_ == 0)
			{
				int num5 = 0;
				while (num5 < commands_.Size())
				{
					string[] array3 = (string[])commands_.ElementAt(num5);
					bool flag = array3.Length > 2;
					int num6 = JInteger.ParseInt(array3[0]);
					string str = array3[1];
					int num7 = array2[MyFormBase.WNDRET_BOX_LEFT] + 10;
					int num8 = array2[MyFormBase.WNDRET_BOX_RIGHT] - array2[MyFormBase.WNDRET_BOX_LEFT] - 20;
					int num9 = 0;
					if (select_ == num5)
					{
						g.SetColor(200, 200, 255);
						g.FillRect(num7 - 6, num4 - 2, num8 + 12, num9 + 16);
					}
					GameView.AddTouch(g, 3, num7 - 6, num4 - 2, num8 + 12, 16, 0x20000 | num5, TouchOption.Create().Margin(0, 0, 10, num9 + 10));
					if (flag)
					{
						g.SetColor(30, 30, 30);
					}
					else
					{
						g.SetColor(128, 128, 128);
					}
					if (MyFormBase.En())
					{
						g.DrawString(str, num3 + 3, num4);
					}
					else
					{
						g.DrawString(str, num3 + 20, num4);
					}
					if (flag && JInteger.ParseInt(array3[2]) != -1)
					{
						g.DrawString((JInteger.ParseInt(array3[2]) + 1023) / 1024 + "k", array2[MyFormBase.WNDRET_BOX_RIGHT] - 10, num4, 4);
					}
					if (num6 >= 0)
					{
						int num10 = 0;
						if (MyFormBase.En()) { num10 = (num7 + 22 + 60); } else { num10 = (num7 + 20 + 60); }
						int num11 = array2[MyFormBase.WNDRET_BOX_RIGHT] - 10 - 24;
						g.DrawString((flag) ? (array3[3]) : (string.Empty), num10 + (num11 - num10) / 2, num4, 2);
					}
					int num12 = num4 - 2;
					if (num6 >= 0)
					{
						num12 += 8;
					}
					if (select_ != num5 || Active())
					{
					}
					if (num5 < commands_.Size() - 1)
					{
						g.SetColor(Graphics.GetColorOfRGB(206, 187, 153));
						g.DrawLine(array2[MyFormBase.WNDRET_BOX_LEFT] + 2, num4 + 15, array2[MyFormBase.WNDRET_BOX_RIGHT] - 3, num4 + 15);
						num4 += 3;
					}
					num5++;
					num4 += 16;
				}
				num4 = MyFormBase.GetBoxFooterY(array2[MyFormBase.WNDRET_BOX_BOTTOM]);
				g.SetColor(60, 100, 200);
				g.DrawString("送る内容を選んでください", 120f, num4, 2);
			}
			else if (state_ == 1)
			{
				num4 = array2[MyFormBase.WNDRET_BOX_TOP] + (array2[MyFormBase.WNDRET_BOX_BOTTOM] - array2[MyFormBase.WNDRET_BOX_TOP] - 28) / 2;
				Font font = g.GetFont(null);
				g.SetColor(30, 30, 30);
				string text = MyFormBase.LT("データの送信中");
				string text2 = string.Empty;
				for (int j = 4; j < frame_ % 16; j += 4)
				{
					text2 += ".";
				}
				int num13 = 0;
				if (taskMax_ != 0)
				{
					num13 = (taskMax_ - (((task_ == null)) ? (0) : (task_.Size()))) * 100 / taskMax_;
				}
				g.DrawString(text + text2, 105f - font.StringWidthF(text) / 2f, num4);
				g.DrawString(num13 + "%", 105f + font.StringWidthF(text) / 2f + 19f, num4);
				text = "電源キーを押さないでください。";
				g.DrawString(text, 120f, num4 + 16, 2);
			}
			else if (state_ == 2)
			{
				num4 = array2[MyFormBase.WNDRET_BOX_TOP] + (array2[MyFormBase.WNDRET_BOX_BOTTOM] - array2[MyFormBase.WNDRET_BOX_TOP] - 28) / 2;
				Font font2 = g.GetFont(null);
				g.SetColor(30, 30, 30);
				string text3 = MyFormBase.LT("データの受信中");
				string text4 = string.Empty;
				for (int k = 4; k < frame_ % 16; k += 4)
				{
					text4 += ".";
				}
				int num14 = 0;
				if (taskMax_ != 0)
				{
					num14 = (taskMax_ - (((task_ == null)) ? (0) : (task_.Size()))) * 100 / taskMax_;
				}
				g.DrawString(text3 + text4, 105 - font2.StringWidth(text3) / 2, num4);
				g.DrawString(num14 + "%", 105 + font2.StringWidth(text3) / 2 + 19, num4);
				text3 = "電源キーを押さないでください。";
				g.DrawString(text3, 120f, num4 + 16, 2);
			}
		}

		public override bool Resume()
		{
			return true;
		}

		public override bool Finish()
		{
			return true;
		}

		private void ChangeState(int state)
		{
			state_ = state;
			SetSoftLabel();
		}

		private void SetSoftLabel()
		{
			int[] array = softLabels_[state_];
			if (array == null) { return; }
{
int num = array[0];
int num2 = array[1];
SetSoftLabel(AppData.softLabels_[num], AppData.softLabels_[num2]);
}
		}

		internal void ShowDialog(string text, string buttons)
		{
			if (apdat_.resCom2_ != null)
			{
				Push(text, buttons);
				return;
			}
			int num = 0;
			if (buttons.IndexOf('\t') != -1)
			{
				num = 3;
			}
			Dialog dialog = new Dialog(num, "メッセージ");
			dialog.SetText(text);
			int num2 = dialog.Show();
			if (num == 3)
			{
				dialog_ = new SubForm(1);
				if (num2 == 4)
				{
					dialog_.result_ = 0;
				}
				else
				{
					dialog_.result_ = 1;
				}
			}
		}

		public void Run()
		{
			try
			{
				JThread.Sleep(100L);
			}
			catch (ThreadAbortException)
			{
			}
			try
			{
				if (state_ == 1)
				{
					string text = Config.URL_SP + "&ver=" + 202;
					if (task_ == null)
					{
						MakeSendDataTask();
					}
					while (task_.Size() > 0)
					{
						object[] array = (object[])task_.ElementAt(0);
						Http.Connect(string.Concat(text, "&file=", array[1], "&index=", array[2], "&date=(", date_, ")"), (byte[])array[0], true);
						array[0] = null;
						task_.RemoveElementAt(0);
					}
				}
				else if (state_ == 2)
				{
					string text2 = Config.URL_SP + "&ver=" + 202 + "&load=save.bin";
					int num = 51200;
					if (task_ == null)
					{
						byte[] array2 = Http.Connect(text2 + "&size=1", null, false);
						if (1 < array2.Length && array2[0] == 78 && array2[1] == 71)
						{
							throw new IOException();
						}
						int num2 = JInteger.ParseInt(JString.New(array2));
						task_ = new Vector();
						for (int i = 0; i < num2; i += num)
						{
							int num3 = num;
							if (i + num3 > num2)
							{
								num3 = num2 - i;
							}
							task_.Add(new int[2] { i, num3 });
						}
						taskMax_ = task_.Size();
						recv_ = new byte[num2];
					}
					while (task_.Size() > 0)
					{
						int[] array3 = (int[])task_.ElementAt(0);
						byte[] array4 = Http.Connect(text2 + "&pos=" + array3[0] + "&len=" + array3[1], null, false);
						if (1 < array4.Length && array4[0] == 78 && array4[1] == 71)
						{
							throw new IOException();
						}
						JSystem.Arraycopy(array4, 0, recv_, array3[0], array4.Length);
						task_.RemoveElementAt(0);
					}
				}
				succeed_ = true;
			}
			catch (Exception t)
			{
				succeed_ = false;
				if (Config.PRINT)
				{
					Log.Info(t);
				}
			}
			th_ = null;
		}

		internal void MakeSendDataTask()
		{
			string[] array = (string[])commands_.ElementAt(send_);
			int num = JInteger.ParseInt(array[0]);
			string text = "dev=" + apdat_.platform_ + "\nver" + 202 + "\nerr=" + apdat_.sysSave_.strs[10] + "\n";
			if ((num == -1)) { text += "system"; } else if ((num == -2)) { text += "all"; } else { if (num / 10 == 1) { text = (text + "auto" + (num % 10 + 1)); } else { text = (text + "save" + (num % 10 + 1)); } }
			ByteListOutputStream byteListOutputStream = new ByteListOutputStream();
			StreamUtil.WriteByte(byteListOutputStream, 115);
			StreamUtil.WriteByte(byteListOutputStream, 97);
			StreamUtil.WriteByte(byteListOutputStream, 118);
			StreamUtil.WriteByte(byteListOutputStream, 101);
			StreamUtil.WriteByte(byteListOutputStream, 48);
			StreamUtil.WriteByte(byteListOutputStream, (sbyte)Config.APPLI_TYPE);
			if (num >= 0)
			{
				bool flag = num / 10 == 1;
				int num2 = num % 10;
				try
				{
					byte[] data = RecordStore.ReadRecord(0, ((flag) ? (3) : (1)) + num2);
					StreamUtil.WriteByte(byteListOutputStream, (sbyte)num);
					StreamUtil.Write(byteListOutputStream, data, true);
				}
				catch (Exception ex)
				{
					text = text + "\nfailed-read[" + ex.ToString() + "]";
					num = -1;
				}
			}
			if (num == -1)
			{
				StreamUtil.WriteByte(byteListOutputStream, (sbyte)num);
				if (Config.APPLI_TYPE == 0)
				{
					byte[] data2 = ScratchPad.ReadByteArray(0, 0, ScratchPad.GetSPSize(0));
					StreamUtil.Write(byteListOutputStream, data2, true);
				}
				else
				{
					RecordStore recordStore = null;
					try
					{
						recordStore = RecordStore.OpenRecordStore(0);
						int numRecords = recordStore.GetNumRecords();
						for (int i = 0; i < numRecords; i++)
						{
							StreamUtil.Write(byteListOutputStream, recordStore.ReadRecord(i), true);
						}
					}
					finally
					{
						if (recordStore != null)
						{
							recordStore.CloseRecordStore();
						}
						recordStore = null;
					}
				}
			}
			if (num == -2)
			{
				StreamUtil.WriteByte(byteListOutputStream, (sbyte)num);
				byte[] array2 = null;
				array2 = ScratchPad.ReadByteArray(0, 0, ScratchPad.GetSPSize(0));
				StreamUtil.Write(byteListOutputStream, array2, true);
				array2 = null;
				array2 = ScratchPad.ReadByteArray(1, 0, ScratchPad.GetSPSize(1));
				StreamUtil.Write(byteListOutputStream, array2, true);
			}
			int num3 = 51200;
			task_ = new Vector();
			task_.Add(new object[3]
			{
				text.GetBytes(),
				"info",
				"0"
			});
			int num4 = 0;
			int num5 = 0;
			while (num4 < byteListOutputStream.Size())
			{
				int num6 = byteListOutputStream.Size() - num4;
				if (num6 > num3)
				{
					num6 = num3;
				}
				task_.Add(new object[3]
				{
					byteListOutputStream.ToByteArray(num4, num6),
					"save",
					JString.ValueOf(num5)
				});
				num4 += num3;
				num5++;
			}
			taskMax_ = task_.Size();
		}

		public override int GetTouchValue(int type, int index)
		{
			type = (int)(type & 0xFFFF0000u);
			int num = type;
			if (num != 131072) { } else {
				return select_;
			}
			return base.GetTouchValue(type, index);
		}

		public override void SetTouchValue(int type, int value)
		{
			type = (int)(type & 0xFFFF0000u);
			int num = type;
			if (num != 131072) {
				base.SetTouchValue(type, value);
			} else {
				select_ = value;
			}
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}
}
