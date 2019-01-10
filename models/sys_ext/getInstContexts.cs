using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace basicClasses.models.sys_ext
{
    [info("message.CopyArr(contexts);")]
    [appliable("initValues ")]
   public class getInstContexts: ModelBase
    {
        public override void Process(opis message)
        {
           message.CopyArr(contexts);
        }

    }
}
