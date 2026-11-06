using game;
using java.io;
using java.lang;
using java.util;
using kairo.unity.util;
using main;

namespace data
{
	public class TeamData : BaseData
	{
		public const int FLAG_INIT = 1;

		public const int FLAG_MATCH = 2;

		public const int FTYPE_NORMAL = 0;

		public const int FTYPE_OFFENCE = 1;

		public const int FTYPE_DEFFENCE = 2;

		public static AppData apdat_;

		public string name_;

		public string name2_;

		public int nationalFlag_;

		public int league_;

		private string homeTownName_;

		public int stadium_;

		public int formation_;

		public int formationType_;

		public int warPotential_;

		public int[] warP_distribute_;

		public int[] warP_paramPhysical_;

		public int war_offence_;

		public int war_deffence_;

		public int amefto_pz_;

		public int fan_;

		public int uniform_;

		public int bodyImg_;

		public int keeperImg_;

		public string[] goalGetter_;

		public int[] memberFaceId_;

		public int[] goalScore_;

		public int[] goalScoreRate_;

		public TeamData()
		{
			apdat_ = AppData.GetInstance();
		}

		public TeamData(string src)
		{
			apdat_ = AppData.GetInstance();
			Init();
			Load(src);
		}

		public void Init()
		{
			if (goalScore_ != null)
			{
				for (int i = 0; i < goalScore_.Length; i++)
				{
					goalScore_[i] = 0;
				}
			}
			if (goalScoreRate_ != null)
			{
				for (int j = 0; j < goalScoreRate_.Length; j++)
				{
					goalScoreRate_[j] = 0;
				}
			}
		}

		public void Load(string src)
		{
			int num = 0;
			string[] array = StringUtil.Split(src, "\t");
			id_ = JInteger.ParseInt(array[num++]);
			name_ = array[num++];
			name2_ = array[num++];
			nationalFlag_ = JInteger.ParseInt(array[num++]);
			league_ = JInteger.ParseInt(array[num++]);
			homeTownName_ = array[num++];
			stadium_ = JInteger.ParseInt(array[num++]);
			formation_ = JInteger.ParseInt(array[num++]);
			formationType_ = JInteger.ParseInt(array[num++]);
			warPotential_ = JInteger.ParseInt(array[num++]);
			warP_distribute_ = StringUtil.SplitIntegers(array[num++], "&");
			war_offence_ = JInteger.ParseInt(array[num++]);
			war_deffence_ = JInteger.ParseInt(array[num++]);
			amefto_pz_ = JInteger.ParseInt(array[num++]);
			fan_ = JInteger.ParseInt(array[num++]);
			uniform_ = JInteger.ParseInt(array[num++]);
			bodyImg_ = JInteger.ParseInt(array[num++]);
			keeperImg_ = JInteger.ParseInt(array[num++]);
			goalGetter_ = new string[2];
			goalGetter_[0] = array[num++];
			goalGetter_[1] = array[num++];
			memberFaceId_ = StringUtil.SplitIntegers(array[num++], "&");
			SetWarParamPhysical();
			goalScore_ = new int[goalGetter_.Length];
			goalScoreRate_ = new int[goalGetter_.Length];
			int num2 = 0;
			for (int i = 0; i < goalGetter_.Length; i++)
			{
				if (!goalGetter_[i].Equals(string.Empty))
				{
					num2++;
				}
			}
			if ((num2 == 1)) { goalScoreRate_[0] = 100;
goalScoreRate_[1] = goalScoreRate_[0]; } else if ((num2 == 2)) { goalScoreRate_[0] = 80;
goalScoreRate_[1] = goalScoreRate_[0] + 20; }
		}

		public void SetWarParamPhysical()
		{
			warP_paramPhysical_ = new int[warP_distribute_.Length];
			int num = 0;
			for (int i = 0; i < warP_distribute_.Length; i++)
			{
				num += warP_distribute_[i];
			}
			for (int j = 0; j < warP_distribute_.Length; j++)
			{
				warP_paramPhysical_[j] = warPotential_ * warP_distribute_.Length * warP_distribute_[j] / num;
			}
		}

		public void SetScore(int getScore, bool isUserTeam)
		{
			UserData instance = UserData.GetInstance();
			if (isUserTeam)
			{
				return;
			}
			for (int i = 0; i < getScore; i++)
			{
				int num = 0;
				int num2 = GameUtil.Random(100);
				for (int j = 0; j < goalScoreRate_.Length; j++)
				{
					if (num2 < goalScoreRate_[j])
					{
						num = j;
						break;
					}
				}
				goalScore_[num]++;
			}
		}

		public void NewGame()
		{
		}

		public void Serialize(OutputStream os)
		{
			StreamUtil.WriteString(os, name_);
			StreamUtil.WriteString(os, name2_);
			StreamUtil.WriteShort(os, (short)stadium_);
			StreamUtil.WriteShort(os, (short)warPotential_);
			StreamUtil.WriteShort(os, (short)uniform_);
			StreamUtil.WriteShort(os, (short)bodyImg_);
			StreamUtil.WriteShort(os, (short)keeperImg_);
			StreamUtil.WriteInt(os, flag_);
			for (int i = 0; i < goalGetter_.Length; i++)
			{
				StreamUtil.WriteString(os, goalGetter_[i]);
			}
			for (int j = 0; j < goalScore_.Length; j++)
			{
				StreamUtil.WriteInt(os, goalScore_[j]);
			}
		}

		public void Deserialize(InputStream stream)
		{
			name_ = StreamUtil.ReadString(stream);
			name2_ = StreamUtil.ReadString(stream);
			stadium_ = StreamUtil.ReadShort(stream);
			warPotential_ = StreamUtil.ReadShort(stream);
			uniform_ = StreamUtil.ReadShort(stream);
			bodyImg_ = StreamUtil.ReadShort(stream);
			keeperImg_ = StreamUtil.ReadShort(stream);
			flag_ = StreamUtil.ReadInt(stream);
			for (int i = 0; i < goalGetter_.Length; i++)
			{
				goalGetter_[i] = StreamUtil.ReadString(stream);
			}
			for (int j = 0; j < goalScore_.Length; j++)
			{
				goalScore_[j] = StreamUtil.ReadInt(stream);
			}
		}

		public string GetHomeStadiumName()
		{
			if (id_ == 0)
			{
				if (Language.Japanese())
				{
					return name2_ + homeTownName_;
				}
				return name2_ + " " + homeTownName_;
			}
			return homeTownName_;
		}

		public void AddTeamEva(int addEva)
		{
			warPotential_ = AppData.Clamp(warPotential_ + addEva, 0, 1000);
			UserData instance = UserData.GetInstance();
			int rankIndex = instance.GetRankIndex(warPotential_);
			if (rankIndex > instance.teamEvaRank_)
			{
				instance.teamEvaRank_ = rankIndex;
			}
		}

		public static void SetScoreRanking(LeagueData league_data, Vector out_ary1, Vector out_ary2)
		{
			UserData instance = UserData.GetInstance();
			for (int i = 0; i < league_data.matchEnemy_.Length; i++)
			{
				int num = league_data.matchEnemy_[i];
				if (num == 0)
				{
					continue;
				}
				TeamData teamData = apdat_.tData_[num];
				for (int j = 0; j < teamData.goalScore_.Length; j++)
				{
					if (teamData.goalScore_[j] > 0)
					{
						out_ary1.AddElement(teamData.goalGetter_[j]);
						out_ary2.AddElement(new int[3]
						{
							teamData.goalScore_[j],
							num,
							0
						});
					}
				}
			}
			for (int k = 0; k < out_ary2.Size() - 1; k++)
			{
				for (int num2 = out_ary2.Size() - 1; num2 > k; num2--)
				{
					int[] array = (int[])out_ary2.ElementAt(k);
					int[] array2 = (int[])out_ary2.ElementAt(num2);
					if (array2[0] > array[0])
					{
						out_ary2.SetElementAt(array2, k);
						out_ary2.SetElementAt(array, num2);
						string obj = (string)out_ary1.ElementAt(k);
						string obj2 = (string)out_ary1.ElementAt(num2);
						out_ary1.SetElementAt(obj2, k);
						out_ary1.SetElementAt(obj, num2);
					}
				}
			}
			for (int l = 0; l < instance.memberPDataist_.Size(); l++)
			{
				PlayerData playerData = (PlayerData)instance.memberPDataist_.ElementAt(l);
				if (playerData.game_goalInLeague_ <= 0)
				{
					continue;
				}
				int num3 = 0;
				for (num3 = 0; num3 < out_ary2.Size(); num3++)
				{
					int[] array3 = (int[])out_ary2.ElementAt(num3);
					if (playerData.game_goalInLeague_ < array3[0]) { return; }
break;
				}
				if (num3 >= 0 && num3 <= out_ary1.Size() && num3 <= out_ary2.Size())
				{
					out_ary1.InsertElementAt(playerData.name_, num3);
					out_ary2.InsertElementAt(new int[3] { playerData.game_goalInLeague_, 0, playerData.id_ }, num3);
				}
			}
		}
	}
}
