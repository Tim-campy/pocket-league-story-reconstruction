"""Copy coordinates before DrawImage, use copies only for _addTouch."""
import re
RULE='r12-smcs-addtouch-coordinate-aliases-before-draw-used-only-by-registration'
METHOD_ID='surface.GameView::AddTouch(kairo.unity.ui.Graphics,System.Int32,kairo.unity.ui.Image,System.Int32,System.Int32,System.Int32,System.Int32,System.Int32,System.Int32,System.Int32,kairo.unity.surface.TouchOption)->kairo.unity.surface.TouchComponent'
def transform(method):
 anchor='g.DrawImage(img, x, y, u, v, w, h);';old='instance_._addTouch(g, id, x, y, w, h, value, opt)'
 if method.count(anchor)!=1 or method.count(old)!=1:raise ValueError('Unique original draw and registration required')
 method=method.replace(anchor,'int savedX = x;\nint savedY = y;\nint savedWidth = w;\nint savedHeight = h;\n'+anchor,1)
 return method.replace(old,'instance_._addTouch(g, id, savedX, savedY, savedWidth, savedHeight, value, opt)',1)
