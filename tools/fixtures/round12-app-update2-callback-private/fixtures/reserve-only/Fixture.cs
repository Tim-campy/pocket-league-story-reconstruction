public delegate void NameChanged(string text);
public class TeamData{public string name_,name2_;}
public class TitleForm{public string newGameName_;}
public class AppData{public TitleForm frmTitle_;}
public static class JString{public static string New(string value){return value;}public static string ValueOf(char value){return value.ToString();}public static int Length(this string value){return value.Length;}public static char CharAt(this string value,int index){return value[index];}}
public static class MyFormBase{public static bool locale;public static bool Jp(){return locale;}}
public class Fixture{public AppData apdat_;public static int GetStringLength(string text){return text.Length;}
public NameChanged Run(){TeamData t_data=new TeamData();return delegate(string temp_name)
									{
										if (temp_name != null)
										{
											t_data.name_ = JString.New(temp_name);
											apdat_.frmTitle_.newGameName_ = JString.New(temp_name);
											int recoveredUnusedNameWidth;
string text = string.Empty;
											int num38 = 0;
											int num39 = 0;
											for (int num40 = 0; num40 < temp_name.Length(); num40++)
											{
												char c = temp_name.CharAt(num40);
												int stringLength = GetStringLength(JString.ValueOf(c));
												if (num40 == 0)
												{
													num39 = ((!MyFormBase.Jp()) ? 8 : ((stringLength != 1) ? 4 : 5));
												}
												num38 += stringLength;
												if (num38 > num39)
												{
													break;
												}
												text += JString.ValueOf(c);
											}
											t_data.name2_ = text;
										}
									};}
}