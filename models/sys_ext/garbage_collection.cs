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
        [info("long. fill minimun count of objects to trigger garbage collection (if less -- do nothing). ! delete this option to run GC unconditionally")]
        public static readonly string if_more_than = "if_more_than";

        [model("spec_tag")]
        [info("long. fill Megabytes of heap to trigger garbage collection (if less -- do nothing). ! heap size check run after minimun count of objects created")]
        public static readonly string if_heap_size_more_than = "if_heap_size_more_than";

        public override void Process(opis message)
        {
            opis spec = SpecLocalRunAll();

            message.body = "GC DO NOTHING. total obj count (" + opis.TotalObjectsCreated + ")  ";

            if (spec.isHere(if_more_than))
            {
                ulong lim = 0;
                ulong.TryParse(spec.V(if_more_than), out lim);
              
                if (opis.TotalObjectsCreated > lim)
                {
                    if (spec.isHere(if_heap_size_more_than))
                    {
                        long heapsize = GC.GetTotalMemory(false);
                        long limmb = 0;
                        long.TryParse(spec.V(if_heap_size_more_than), out limmb);                        

                        if(heapsize / 1048576 > limmb)
                            message.body = Collect();
                    }
                    else
                      message.body = Collect();

                }
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

            string rez = "GC before   total obj count (" + opis.TotalObjectsCreated + ")    MEM before  " + before/1048576 + "MB   after " + after / 1048576 + "MB ";
            opis.TotalObjectsCreated = 0;

            return rez;
        }
    }
}
