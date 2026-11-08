"""Pure four parameter copies used only by the first border rectangle."""
RULE='r12-draw3dbox-first-rectangle-four-parameter-aliases'
METHOD_ID='main.AppData::Draw3DBox(kairo.unity.ui.Graphics,System.Int32,System.Int32,System.Int32,System.Int32,System.Int32[][])->System.Void'

def transform(source):
 first='g.SetColor(Graphics.GetColorOfRGB(colors[0][0], colors[0][1], colors[0][2]));'
 call='g.DrawRect(offset_x, offset_y, width - 1, height - 1);'
 if source.count(first)!=1 or source.count(call)!=1:raise ValueError('One first border color/rectangle required')
 if source.index(first)>source.index(call):raise ValueError('Color must precede first rectangle')
 copies='int recoveredBorderX = offset_x;\nint recoveredBorderY = offset_y;\nint recoveredBorderWidth = width;\nint recoveredBorderHeight = height;\n'
 source=source.replace(first,copies+first,1)
 return source.replace(call,'g.DrawRect(recoveredBorderX, recoveredBorderY, recoveredBorderWidth - 1, recoveredBorderHeight - 1);',1)
