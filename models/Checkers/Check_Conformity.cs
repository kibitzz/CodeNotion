using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace basicClasses.models.Checkers
{
    [info("gets parameters from message, in wrap [arg]    if pass  set message[pass].body ='y'    message[passCou].intVal++;")]
    [appliable("ChecksList RangingList ")]
   public class check_Conformity:ModelBase
    {

        [model("template")]
        [info("")]
        public static readonly string template = "template";

        [model("spec_tag")]
        [info("negation (CheckConformity = 0)")]
        public static readonly string NOT = "NOT";

        public override void Process(opis message)
        {           
            opis arg = message.W("arg");

            opis ptt = modelSpec[template].Duplicate();
            if (ptt.PartitionKind == "template")
                instanse.ExecActionResponceModelsList(ptt, ptt);
            else
                instanse.ExecActionModel(ptt, ptt);

            //logopis["debug_template"] = ptt;
            //logopis.WrapByName( arg, "arg");

            int cf= arg.CheckConformity(arg, ptt);

            if ((ptt.listCou == cf && !modelSpec.isHere(NOT))
                || (cf == 0 && modelSpec.isHere(NOT)))
            {
                //logopis["debug_pass"].body = "y";
                message["pass"].body = "y";
                message["passCou"].intVal++;
                message["range"].intVal++;
            }


        }

    }
}
