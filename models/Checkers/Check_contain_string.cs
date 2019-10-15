using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace basicClasses.models.Checkers
{
   
    [appliable("ChecksList RangingList ")]
    [info("")]
    public class Check_contain_string : ModelBase
    {
        [model("template")]
        [info("")]
        public static readonly string template = "template";

        [model("spec_tag")]
        [info("")]
        public static readonly string NOT = "NOT";

        [model("spec_tag")]
        [info("")]
        public static readonly string substring = "substring";

        public override void Process(opis message)
        {
            opis arg = message.W("arg");

            opis ptt = modelSpec[template].Duplicate();
            instanse.ExecActionResponceModelsList(ptt, ptt);

            opis spec = modelSpec.Duplicate();
            instanse.ExecActionModelsList(spec);

            opis processThis = arg;
            if (ptt.isInitlze)
                processThis = opis.GetLevelByTemplate(ptt[0], arg, true);

            if (
                processThis != null &&
                (
                (processThis.body.Contains(spec.V(substring)) && !modelSpec.isHere(NOT))
                ||
                (!processThis.body.Contains(spec.V(substring)) && modelSpec.isHere(NOT))
                  )
                 )
            {
                message["pass"].body = "y";
                message["passCou"].intVal++;
                message["range"].intVal++;
            }



        }

       
    }

}
