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
        [info("delete this to use local data context(LDC) flag <exit>.  or set body of this equal <exit>")]
        public static readonly string condition = "condition";

        [model("FlagModelSpec")]
        [info("if condition to exit is matched, make it propagate to other Breaker's that use LDC flag <exit>")]
        public static readonly string setLdcExitOnCondition = "setLdcExitOnCondition";

        [ignore]
        public static readonly string flag = "break further code exec";  //TODO: optimize by index

        public override void Process(opis message)
        {
            opis surc = message;
            if (modelSpec.isHere(condition))
            {
                bool prop = modelSpec[setLdcExitOnCondition].isInitlze;

                surc = modelSpec[condition].Duplicate();
                instanse.ExecActionModel(surc, surc);
                if (surc.body == "exit" || surc.isHere("exit") )
                {
                    SetFlag(message);

                    if (prop)
                        instanse.GetLocalDataContextVal("exit", true);
                }
            }
            else
            {
                if(!string.IsNullOrEmpty(instanse.GetLocalDataContextVal("exit").PartitionName ))
                     SetFlag(message);
            }
                    
        }

        void SetFlag(opis message)
        {
            message[flag].body = "true";
        }
    }
}
