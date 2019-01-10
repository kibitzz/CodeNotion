using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace basicClasses.models.Checkers
{
    [appliable("ChecksList RangingList ")]
    [info("listCou>1")]
   public class Check_list_more_1:ModelBase
    {
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
            if (ptt.isInitlze)
                processThis = opis.GetLevelByTemplate(ptt[0], arg, true);

            if (processThis != null 
                && (processThis.listCou>1 
                || (processThis.listCou <2 && modelSpec.isHere(NOT))))
            {
                //logopis["debug_pass"].body = "y";
                message["pass"].body = "y";
                message["passCou"].intVal++;
                message["range"].intVal++;
            }

            //if (processThis == null)
            //    logopis["partition not found"].body = "";

        }
    }


    [appliable("ChecksList RangingList ")]
    [info("listCou > 0")]
    public class Check_list_more_0 : ModelBase
    {
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
            if (ptt.isInitlze)
                processThis = opis.GetLevelByTemplate(ptt[0], arg, true);

            if (processThis != null
                && (processThis.listCou > 0
                || (processThis.listCou == 0 && modelSpec.isHere(NOT))))
            {
                //logopis["debug_pass"].body = "y";
                message["pass"].body = "y";
                message["passCou"].intVal++;
                message["range"].intVal++;
            }

            //if (processThis == null)
            //    logopis["partition not found"].body = "";

        }
    }



}
