using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace basicClasses.models.Checkers
{
    [appliable("ChecksList RangingList ")]
    [info("")]
    public class Check_Key_value_prsnce:ModelBase
    {
              
        [info("key to search in whole branch")]
        public static readonly string key = "key";

        [info("value of key to search in whole branch")]
        public static readonly string val = "val";

        [model("template")]
        [info("")]
        public static readonly string template = "template";

        [model("spec_tag")]
        [info("")]
        public static readonly string NOT = "NOT";

        public override void Process(opis message)
        {
            opis arg = message.W("arg");

            opis ptt = modelSpec[template].Duplicate();
            instanse.ExecActionResponceModelsList(ptt, ptt);

            //logopis["debug_template"] = ptt;
            //logopis.WrapByName(arg, "arg");

            opis processThis = arg;
            if(modelSpec[template].isInitlze)
                processThis = opis.GetLevelByTemplate(ptt[0], arg, true);

            opis t = modelSpec[key].Duplicate();
            instanse.ExecActionModel(t, t);

            opis kvpobj = new opis();
            kvpobj.PartitionName = t.body;

            t = modelSpec[val].Duplicate();
            instanse.ExecActionModel(t, t);
            kvpobj.body = t.body;

            bool pass = (processThis != null &&
                 processThis.CheckKVPresence(kvpobj));

            if (pass || (!pass && modelSpec.isHere(NOT)))
            {
                //logopis["debug_pass"].body = "y";
                message["pass"].body = "y";
                message["passCou"].intVal++;
                message["range"].intVal++;
            }

            //if (processThis == null)
            //    logopis["partition not found"].body = "";

        }

        public bool FindKVP(opis area, opis kvp)
        {
            bool rez = false;
            if (kvp == null) return false;



            return rez;
        }

    }
}
