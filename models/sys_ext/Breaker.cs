using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace basicClasses.models.sys_ext
{
    [appliable("Action func")]
    [info("stop further code exec in the same branch level (where placed)  use local data context (LDC) to seek flag <exit>. Usage: put anything to *exit and all breakers at levels that use same LDC activated")]
    class Breaker : ModelBase
    {

        [model("")]
        [info("delete this to use data context flag <exit>.  or set body of this equal <exit>")]
        public static readonly string condition = "condition";
       
        [ignore]
        public static readonly string flag = "break further code exec";

        public override void Process(opis message)
        {
            opis surc = message;
            if (modelSpec.isHere(condition))
            {
                surc = modelSpec[condition].Duplicate();
                instanse.ExecActionModel(surc, surc);
                if (surc.isHere("exit") || surc.body == "exit")
                    setFlag(message);
            }
            else
            {
                if(!string.IsNullOrEmpty(instanse.GetLocalDataContextVal("exit").PartitionName ))
                     setFlag(message);
            }
                    
        }

        void setFlag(opis message)
        {
            message[flag].body = "true";
        }
    }
}
