using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace basicClasses.models.Checkers
{
   
    [appliable("ChecksList RangingList ")]
    [info("id odd ")]
    public class Check_is_odd : ModelBase
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

         
            opis processThis = arg;
            if (ptt.isInitlze)
                processThis = opis.GetLevelByTemplate(ptt[0], arg, true);

            if (
                processThis != null &&
                (
                (IsOdd(processThis.intVal) && !modelSpec.isHere(NOT))
                ||
                (!IsOdd(processThis.intVal) && modelSpec.isHere(NOT))
                  )
                 )
            {              
                message["pass"].body = "y";
                message["passCou"].intVal++;
                message["range"].intVal++;
            }

        

        }

        public static bool IsOdd(int value)
        {
            return value % 2 != 0;
        }
    }

}
