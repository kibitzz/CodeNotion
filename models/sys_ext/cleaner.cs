using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace basicClasses.models.sys_ext
{
    [appliable("global_log ")]
    [info("thisins[Models_log].CopyArr(new opis());")]
   public class cleaner:ModelBase
    {

        public override void Process(opis message)
        {
            thisins["Models_log"].CopyArr(new opis());
        }

    }
}
