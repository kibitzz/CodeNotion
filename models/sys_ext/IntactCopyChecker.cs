using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace basicClasses.models.sys_ext
{
    [info("Optimization of functions and constants reuse")]
    class IntactCopyChecker : ModelBase
    {

        [model("spec_tag")]
        [info("")]
        public static readonly string permaCopy_mark = "permaCopy_mark";

        [model("spec_tag")]
        [info("")]
        public static readonly string stata = "stata";

        public override void Process(opis message)
        {
            opis surc = message;

            void mark(opis x)
            {
                if (x.copy != null)
                {
                    if (opis.CopyIntact(x, x.copy))
                    {
                        x.permaCopy = 1;
                        mark(x.copy);
                    }
                    else x.permaCopy = 2;
                }
            }

            if (modelSpec.isHere(permaCopy_mark))
            {
                message.RunRecursively(mark);
            }
            else
            {

                if (modelSpec.isHere(stata))
                {
#if DEBUG                    
                    message.Vset("copyExecTotal", opis.copyExecTotal.ToString());
                    message.Vset("copyCacheHit", opis.copyCacheHit.ToString());
                    message.Vset("copyCacheIntact", opis.copyCacheIntact.ToString());
                    message.Vset("copyCacheModified", opis.copyCacheModified.ToString());
#endif
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
}
