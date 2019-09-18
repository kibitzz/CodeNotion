using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace basicClasses.models.FunctionalInstances
{
    [info(" instanse.PauseThread();")]
    [appliable("all Action func")]
   public class Pause_To_debug:ModelBase
    {

        [model("")]
        [info("")]
        public static readonly string messageBanner = "messageBanner";

        [model("FlagModelSpec")]
        [info("")]
        public static readonly string clear_models_log = "clear_models_log";

        [ignore]
        [model("FlagModelSpec")]
        [info("turn off building GUI tree elements for visualization structures.  [y] for off;  [n] to visualize")]
        public static readonly string do_not_build_debug = "do_not_build_debug";


        public override void Process(opis message)
        {
            if (modelSpec[clear_models_log].isInitlze)
            {
                thisins["Models_log"].CopyArr(new opis());
            }
            
            if (modelSpec[messageBanner].isInitlze)
            {
                SysInstance.messageBannertext = "paused to debug : "+ modelSpec[messageBanner].body;
                instanse.updateGui();
            }else
            {
                SysInstance.messageBannertext = "paused to debug";
                instanse.updateGui();
            }

            if (modelSpec.V(do_not_build_debug)=="y")                
            opis.do_not_build_debug = true;
            if (modelSpec.V(do_not_build_debug) == "n")
                opis.do_not_build_debug = false;

            instanse.PauseThread();
        }

    }

    [info("  Thread.Sleep(milliseconds.intVal); ")]
    [appliable("Action ")]
    public class Make_pause : ModelBase
    {       
        [model("integer")]
        [info("")]
        public static readonly string milliseconds = "milliseconds";

     
        public override void Process(opis message)
        {
            opis mils = modelSpec[milliseconds];
            instanse.ExecActionModel(mils, mils);

            Thread.Sleep(mils.intVal);                   
         
        }

    }
}
