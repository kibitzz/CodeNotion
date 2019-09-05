using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace basicClasses.models.Rangers
{
    [appliable("RangingList ")]
   public  class RangeMax:ModelBase
    {
        
        [model("template")]
        [info("")]
        public static readonly string template = "template";

        public override void Process(opis message)
        {
            opis arg = message.W("arg");

            opis ptt = modelSpec[template].Duplicate();
          
          
            instanse.ExecActionModelsList(ptt);
            opis processThis = opis.GetLevelByTemplate(ptt[0], arg, false);
            if (processThis != null)
            {
                message["pass"].body = "y";
                message["passCou"].intVal++;
                message["range"].intVal +=  processThis.intVal;
            }

            //logopis["debug_template"] = ptt;
            //logopis.WrapByName(arg, "arg");        
        } 
    }


    [appliable("RangingList ")]
    public class RangeMin : ModelBase
    {

        [model("template")]
        [info("integer value should be in this path ")]
        public static readonly string template = "template";

        public override void Process(opis message)
        {
            opis arg = message.W("arg");

            opis ptt = modelSpec[template].Duplicate();


            instanse.ExecActionModelsList(ptt);
            opis processThis = opis.GetLevelByTemplate(ptt[0], arg, false);
            if (processThis != null)
            {
                message["pass"].body = "y";
                message["passCou"].intVal++;
                message["range"].intVal += 10000000- processThis.intVal;
            }

            //logopis["debug_template"] = ptt;
            //logopis.WrapByName(arg, "arg");
        }
    }
}
