using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace basicClasses.models.sys_ext
{
    class garbage_collection : ModelBase
    {

        [model("spec_tag")]
        [info("ulong. fill minimun count of objects to trigger garbage collection (if less -- do nothing)")]
        public static readonly string if_more_than = "if_more_than";

        public override void Process(opis message)
        {
            opis spec = SpecLocalRunAll();

            message.body = "GC DO NOTHING. total obj count (" + opis.TotalObjectsCreated + ")  ";

            if (spec.isHere(if_more_than))
            {
                ulong lim = 0;
                ulong.TryParse(spec.V(if_more_than), out lim);

                if(opis.TotalObjectsCreated > lim)
                    message.body = Collect();
            }
            else
            {              
                message.body = Collect();
            }

        }


        string Collect()
        {
            long before = GC.GetTotalMemory(false);
            GC.Collect();
            long after = GC.GetTotalMemory(true);

            string rez = "GC before   total obj count (" + opis.TotalObjectsCreated + ")    MEM before  " + before + "   after " + after;
            opis.TotalObjectsCreated = 0;

            return rez;
        }
    }
}
