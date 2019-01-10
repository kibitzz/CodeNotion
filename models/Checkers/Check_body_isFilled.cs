using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace basicClasses.models.Checkers
{
    [appliable("ChecksList RangingList ")]
    [info(" pass = !string.IsNullOrEmpty(arg.V(modelSpec.PartitionName));")]
    public class Check_body_isFilled:ModelBase
    {

        [model("template")]
        [info("")]
        public static readonly string template = "template";

        [model("")]
        [info("")]
        public static readonly string listOfVerification = "listOfVerification";

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
            processThis = opis.GetLevelByTemplate(ptt[0], arg, false);

            bool pass = false;
            if (processThis != null)
            {              
                if  (modelSpec[listOfVerification].isInitlze)
                {
                    pass = true;
                    opis lis = modelSpec[listOfVerification];
                    for (int i =0; i< lis.listCou;i++)
                    {
                        pass = !string.IsNullOrEmpty(processThis.V(lis[i].PartitionName));
                        if (!pass)
                            break;
                    }
                }else
                    pass = !string.IsNullOrEmpty(processThis.V(modelSpec.PartitionName));
            }

            if (processThis != null
                && (pass
                || (!pass && modelSpec.isHere(NOT))))
            {
                //logopis["debug_pass"].body = "y";
                message["pass"].body = "y";
                message["passCou"].intVal++;
                message["range"].intVal++;
            }
          

        }
    }
}
