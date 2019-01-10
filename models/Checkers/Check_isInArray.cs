using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace basicClasses.models.Checkers
{
    [appliable("ChecksList RangingList ")]
    public class Check_isInArray:ModelBase
    {
        [model("TreeDataExtractor")]
        [info("get values set from anlysed item ")]
        public static readonly string template = "template";

        [model("buildTreeVal_sdc_i")]
        [info("for array elements, path where stored structure that match analysed arg")]
        public static readonly string template_ForArray = "template_ForArray";

        [model("")]
        [info("fill this partition to use as criteria array")]
        public static readonly string ArrayStorage = "ArrayStorage";

        [model("spec_tag")]
        [info("")]
        public static readonly string NOT = "NOT";

        public override void Process(opis message)
        {
            opis arg = message.W("arg");

            opis ptt = modelSpec[template];
            instanse.ExecActionModel(ptt, arg);
            opis arrstruct = modelSpec[template_ForArray];
            instanse.ExecActionModel(arrstruct, arrstruct);


            opis arrstor = modelSpec[ArrayStorage].Duplicate();
            instanse.ExecActionModel(arrstor, arrstor);


            opis processThis = arg;

            bool pass = false;
            if (processThis != null)
            {
                for (int i = 0; i < arrstor.listCou; i++)
                {
                    int cf = arg.CheckConformity(arrstor[i], arrstruct);

                    pass = ptt.listCou == cf;
                    if (pass)
                        break;
                }
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

            //if (processThis == null)
            //    logopis["partition not found"].body = "";

        }
    }
}
