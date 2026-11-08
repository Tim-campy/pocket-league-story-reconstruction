"""Pure loop-condition and continue-owner repair for Camera.Collision."""
import re
RULE='r12-camera-collision-loop-flag-conditions-and-two-continue-guards'
METHOD_ID='game.Camera::Collision(System.Int32,System.Int32)->System.Void'

def once(source,before,after):
 if source.count(before)!=1:raise ValueError('One source anchor required: '+before)
 return source.replace(before,after,1)

def transform(source):
 for variable,bound in [('i','udat_.tAreas_.Length'),('j','udat_.tAreas_[i].Length'),('k','tArea.tGroupAreas_.Length')]:
  header='for (int '+variable+' = 0; '+variable+' < '+bound+'; '+variable+'++)'
  pattern=re.compile(re.escape(header)+r'(?P<gap>\s*)\{\s*if \(flag\)\s*\{\s*break;\s*\}')
  matches=list(pattern.finditer(source))
  if len(matches)!=1:raise ValueError('One first-statement flag guard required for '+variable)
  m=matches[0];new_header=header.replace('; '+variable+'++)',' && !flag; '+variable+'++)')
  source=source[:m.start()]+new_header+m['gap']+'{'+source[m.end():]
 source=once(source,'if (!BitUtil.Check(tPlayer.flag_, 1) && GameUtil.ChRectHitRect(', 'if (BitUtil.Check(tPlayer.flag_, 1)) { continue; }\nif (GameUtil.ChRectHitRect(')
 source=once(source,'if ((tenant.root_ != 1 || udat_.tAreaRootNum_ != 1) && ', 'if (tenant.root_ == 1 && udat_.tAreaRootNum_ == 1) { continue; }\nif (')
 return source
