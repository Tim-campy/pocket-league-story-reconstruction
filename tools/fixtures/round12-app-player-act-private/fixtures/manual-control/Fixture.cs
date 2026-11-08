public class Fixture { public int seb_,sink;void Use(int action){sink=action;}
public bool Run(int action){if(action==0){Use(0);}
else if(action==10){Use(10);}
else if(action==5){}
else if(action==6){if(seb_==1){return false;}}
else if(action==11){Use(11);}
return true;}
}