public class Fixture { public int seb_,sink;void Use(int action){sink=action;}
public bool Run(int act_id){if(act_id==0){Use(0);}
else if ((act_id == 10)) { Use(10);
} else if ((act_id == 5)) {
} else if ((act_id == 6)) {
 if(seb_==1){return false;}
} else if ((act_id == 11)) {Use(11);}
return true;}
}