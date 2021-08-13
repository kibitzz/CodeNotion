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

        [model("Action")]
        [info("run this if condition is matched and garbage need to be released")]
        public static readonly string on_clear = "on_clear";

        [model("Action")]
        [info("")]
        public static readonly string on_lock_timeout = "on_lock_timeout";

        static object lockobj = new object();

        public override void Process(opis message)
        {
            opis spec = SpecLocalRunAll();

            message.body = "GC DO NOTHING. total obj count (" + opis.TotalObjectsCreated + ")  ";

            var watch = System.Diagnostics.Stopwatch.StartNew();                                     

            lock (lockobj)
            {

                if (spec.isHere(if_more_than, false))
                {
                    ulong lim = 0;
                    ulong.TryParse(spec.V(if_more_than), out lim);

                    if (opis.TotalObjectsCreated > lim)
                    {
                        opis run = modelSpec.getPartitionNotInitOrigName(on_clear)?.Duplicate();

                        if (spec.isHere(if_heap_size_more_than, false))
                        {
                            long heapsize = GC.GetTotalMemory(false);
                            long limmb = 0;
                            long.TryParse(spec.V(if_heap_size_more_than), out limmb);

                            if (heapsize / 1048576 > limmb)
                                message.body = Collect(run);
                            else
                                opis.TotalObjectsCreated = 0;

                        }
                        else
                            message.body = Collect(run);

                    }
                }
                else
                {
                    opis run = modelSpec.getPartitionNotInitOrigName(on_clear)?.Duplicate();
                    message.body = Collect(run);
                }

            }

            watch.Stop();
            var ela = watch.ElapsedMilliseconds;

            if (ela > 1000)
            {
                opis run = modelSpec.getPartitionNotInitOrigName(on_lock_timeout)?.Duplicate();
                if (run != null)
                {
                    opis p = new opis() { body = " "+ ela.ToString(), PartitionName = "rez" };
                    instanse.ExecActionResponceModelsList(run, p);
                }
            }

        }


        string Collect(opis run = null)
        {
            long before = GC.GetTotalMemory(false);
            GC.Collect();
            long after = GC.GetTotalMemory(true);

            string rez = "GC before   total obj count (" + opis.TotalObjectsCreated + ")    MEM before  " + before/1048576 + "MB   after " + after / 1048576 + "MB ";
            opis.TotalObjectsCreated = 0;

            if (run != null)
            {
                opis p = new opis() { body = rez, PartitionName = "rez"};
                instanse.ExecActionResponceModelsList(run, p);
            }

            return rez;
        }
    }
}
