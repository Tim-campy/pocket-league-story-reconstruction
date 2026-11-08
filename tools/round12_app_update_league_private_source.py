"""Pure role-based declaration repair for the two shared league scores."""
import re
RULE='r12-league-root-score-locals-and-loop-local-team-ids'
METHOD_ID='game.UserData::Update_league()->System.Void'

def replace_one(source,before,after):
 if source.count(before)!=1:raise ValueError('Unique source anchor required: '+before)
 return source.replace(before,after,1)

def transform(source):
 pattern=re.compile(r'int num;\s*int num2;(?P<gap>\s*)(?=for \(int i = 0; i < league_fightCombi_\.Length; i\+\+\))')
 matches=list(pattern.finditer(source))
 if len(matches)!=1:raise ValueError('Exactly two existing root ID declarations required')
 m=matches[0];source=source[:m.start()]+'int num5;\nint num6;'+m['gap']+source[m.end():]
 source=replace_one(source,'num = league_fightCombi_[i][0];','int num = league_fightCombi_[i][0];')
 source=replace_one(source,'num2 = league_fightCombi_[i][1];','int num2 = league_fightCombi_[i][1];')
 source=replace_one(source,'int num5 = GetTeamScore(teamData.war_offence_, teamData2.war_deffence_);','num5 = GetTeamScore(teamData.war_offence_, teamData2.war_deffence_);')
 source=replace_one(source,'int num6 = GetTeamScore(teamData2.war_offence_, teamData.war_deffence_);','num6 = GetTeamScore(teamData2.war_offence_, teamData.war_deffence_);')
 return source
