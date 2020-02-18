using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace basicClasses.models.sys_ext
{
    class IntactCopyChecker : ModelBase
    {
        public override void Process(opis message)
        {
            opis surc = message;

            message.RunRecursively(x =>
            {
                if (x.copy != null && opis.CopyIntact(x, x.copy))
                {
                    x["_CopyIntact_"].body = "y";
                }
            });

        }
    }
}
