using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace basicClasses.models.sys_ext
{
    class thread_lock : ModelBase
    {

        static object lock1 = new object();
        static object lock2 = new object();
        static object lock3 = new object();
        static object lock4 = new object();
        static object lock5 = new object();
        static object lock6 = new object();
        static object lock7 = new object();



        [model("")]
        [info("1..7 separate locks. support fillef for this field  ")]
        public static readonly string lock_idx = "lock_idx";

        [model("Action")]
        [info("")]
        public static readonly string instructions = "instructions";

        int currInstActiveLock = 0;

        public override void Process(opis message)
        {
            opis specl = modelSpec.Duplicate();
            instanse.ExecActionModelsList(specl);

            var instr = specl[instructions];
           
            int ln = specl[lock_idx].intVal;
           
            if (currInstActiveLock > 0)  // lock inside lock is ignored along single instance
            {
#if DEBUG
                var ei = new opis() { PartitionName = "WARN: nested lock -- " + spec.PartitionName + " "+ ln };
                ei.AddArr(instr);
                global_log.log?.AddArr(ei);
#endif
                instanse.ExecActionModelsList(instr);
                return;
            }

            if (ln == 1)
            {
                lock (lock1)
                {
                    currInstActiveLock = ln;
                    instanse.ExecActionModelsList(instr);
                }
            }

            if (ln == 2)
            {
                lock (lock2)
                {
                    currInstActiveLock = ln;
                    instanse.ExecActionModelsList(instr);
                }
            }

            if (ln == 3)
            {
                lock (lock3)
                {
                    currInstActiveLock = ln;
                    instanse.ExecActionModelsList(instr);
                }
            }

            if (ln == 4)
            {
                lock (lock4)
                {
                    currInstActiveLock = ln;
                    instanse.ExecActionModelsList(instr);
                }
            }

            if (ln == 5)
            {
                lock (lock5)
                {
                    currInstActiveLock = ln;
                    instanse.ExecActionModelsList(instr);
                }
            }

            if (ln == 6)
            {
                lock (lock6)
                {
                    currInstActiveLock = ln;
                    instanse.ExecActionModelsList(instr);
                }
            }

            if (ln == 7)
            {
                lock (lock7)
                {
                    currInstActiveLock = ln;
                    instanse.ExecActionModelsList(instr);
                }
            }

            currInstActiveLock = 0;
        }
    }
}
