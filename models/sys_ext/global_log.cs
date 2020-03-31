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



}
