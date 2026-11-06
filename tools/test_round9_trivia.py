"""Protect source literal payloads during generated-line trivia cleanup."""
from round9_clean_added_whitespace import clean_source_lines


def main():
    source='if (true) { \r\n}\n'
    out,lines,blank,code,refused=clean_source_lines(source,[1,2])
    assert out=='if (true) {\r\n}\n' and lines==[1] and code==1 and blank==0 and not refused
    source='string s = @"first\r\n   \r\nlast";  \r\n'
    out,lines,blank,code,refused=clean_source_lines(source,[1,2,3])
    assert out=='string s = @"first\r\n   \r\nlast";\r\n' and lines==[3] and refused==[2]
    source='string s = "value ";  \nchar c = \' \'; \n'
    out,*_=clean_source_lines(source,[1,2])
    assert out=='string s = "value ";\nchar c = \' \';\n'
    source='string s = $"{value}";  \n'
    out,lines,blank,code,refused=clean_source_lines(source,[1])
    assert out==source and not lines and refused==[1]
    print('4 source-trivia payload guards passed; no game compilation')


if __name__=='__main__':main()
