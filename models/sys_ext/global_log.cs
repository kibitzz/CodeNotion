using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace basicClasses.models.sys_ext
{
    [appliable("all")]
    [info("shared log for all objects in context. no modification at Process( message).  by default duplicate all values ")]
   public class global_log:ModelBase
    {
        [ignore]
         static opis _log;

        [ignore]
        public static opis log
        {
            get
            {
                if (_log == null)
                {
                    _log = new opis("global logging");
                }

                return _log;
            }
        }

        [info("do not duplicate original values, put references instead")]
        [model("FlagModelSpec")]
        public static readonly string do_not_duplicate = "do_not_duplicate";

        public override void Process(opis message)
        {
            opis logitem = modelSpec.Duplicate();
            instanse.ExecActionModelsList(logitem);

            if(modelSpec.isHere(do_not_duplicate) && modelSpec[do_not_duplicate].isInitlze)
                log.AddArr(logitem);
            else
            log.AddArr(logitem.Duplicate());
           

        }

        public static void ClearAll()
        {
            _log = null;
        }
    }



    [appliable("all")]
    [info("separate local log for current instance .   by default duplicate all values ")]
    public class local_log : ModelBase
    {

        [info("do not duplicate original values, put references instead")]
        [model("FlagModelSpec")]
        public static readonly string do_not_duplicate = "do_not_duplicate";

        public override void Process(opis message)
        {
            opis logitem = modelSpec.Duplicate();
            instanse.ExecActionModelsList(logitem);

            if (modelSpec.isHere(do_not_duplicate) && modelSpec[do_not_duplicate].isInitlze)
                thisins["Models_log"].AddArr(logitem);
            else
                thisins["Models_log"].AddArr(logitem.Duplicate());
          
        }
    }


    [appliable("")]
    [info("get/clear all items of global_log (if overriden or turned off -- system warnings put there anyway)  ")]
    public class sys_log_info : ModelBase
    {

        [model("spec_tag")]
        [info(" ")]
        public static readonly string get_all = "get_all";

        [model("spec_tag")]
        [info(" ")]
        public static readonly string clear_all = "clear_all";


        public override void Process(opis message)
        {

            if (modelSpec.isHere(get_all, false) && global_log.log != null)
                message.CopyArr(global_log.log);

            if (modelSpec.isHere(clear_all,false))
                global_log.ClearAll() ;
          
        }
    }


}
