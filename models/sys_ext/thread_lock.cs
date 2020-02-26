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

        public override void Process(opis message)
        {
            opis spec = modelSpec.Duplicate();
            instanse.ExecActionModelsList(spec);

            var instr = spec[instructions];
           
            int ln = spec[lock_idx].intVal;

            if (ln == 1)
            {
                lock (lock1)
                {
                    instanse.ExecActionModelsList(instr);
                }
            }

            if (ln == 2)
            {
                lock (lock2)
                {
                    instanse.ExecActionModelsList(instr);
                }
            }

            if (ln == 3)
            {
                lock (lock3)
                {
                    instanse.ExecActionModelsList(instr);
                }
            }

            if (ln == 4)
            {
                lock (lock4)
                {
                    instanse.ExecActionModelsList(instr);
                }
            }

            if (ln == 5)
            {
                lock (lock5)
                {
                    instanse.ExecActionModelsList(instr);
                }
            }

            if (ln == 6)
            {
                lock (lock6)
                {
                    instanse.ExecActionModelsList(instr);
                }
            }

            if (ln == 7)
            {
                lock (lock7)
                {
                    instanse.ExecActionModelsList(instr);
                }
            }
        }
    }
}
