using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace basicClasses.models.sys_ext
{
   
    [appliable("")]
    [info("  ")]
    class clear_stack_itms : ModelBase
    {
        public override void Process(opis message)
        {
           var names= instanse.GetTempSDCstackNames();

            opis svc = sharedVal;

            foreach (var name in names)
            {
                svc[name].CopyArr(new opis());
            }
        }
    }
}
