using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace basicClasses.models.sys_ext
{
    [info("Optimization of reuse functions and constants")]
    class IntactCopyChecker : ModelBase
    {

        [model("spec_tag")]
        [info("")]
        public static readonly string permaCopy_mark = "permaCopy_mark";

        public override void Process(opis message)
        {
            opis surc = message;

            if (modelSpec.isHere(permaCopy_mark))
            {
                message.RunRecursively(x =>
                {
                    if (x.copy != null && opis.CopyIntact(x, x.copy))
                    {
                        x.permaCopy = true;
                    }
                });
            }
            else

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
