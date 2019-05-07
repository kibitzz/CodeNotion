using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace basicClasses.models.sys_ext
{
    [info(" ")]
    [appliable("global_log")]
    class CheckOverrides : ModelBase
    {
        public override void Process(opis message)
        {
            SysInstance.showOverrideWarnings = true;
        }
    }
}
