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
        [info("1..7 separate locks")]
        public static readonly string lock_idx = "lock_idx";

        [model("Action")]
        [info("")]
        public static readonly string instructions = "instructions";

        public override void Process(opis message)
        {
            var instr = modelSpec[instructions].Duplicate();


            if (modelSpec[lock_idx].intVal == 1)
            {
                lock (lock1)
                {
                    instanse.ExecActionModelsList(instr);
                }
            }

            if (modelSpec[lock_idx].intVal == 2)
            {
                lock (lock2)
                {
                    instanse.ExecActionModelsList(instr);
                }
            }

            if (modelSpec[lock_idx].intVal == 3)
            {
                lock (lock3)
                {
                    instanse.ExecActionModelsList(instr);
                }
            }

            if (modelSpec[lock_idx].intVal == 4)
            {
                lock (lock4)
                {
                    instanse.ExecActionModelsList(instr);
                }
            }

            if (modelSpec[lock_idx].intVal == 5)
            {
                lock (lock5)
                {
                    instanse.ExecActionModelsList(instr);
                }
            }

            if (modelSpec[lock_idx].intVal == 6)
            {
                lock (lock6)
                {
                    instanse.ExecActionModelsList(instr);
                }
            }

            if (modelSpec[lock_idx].intVal == 7)
            {
                lock (lock7)
                {
                    instanse.ExecActionModelsList(instr);
                }
            }
        }
    }
}
