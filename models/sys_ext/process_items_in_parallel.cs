using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace basicClasses.models.sys_ext
{
   
    [appliable("")]
    [info("")]
    class process_items_in_parallel : ModelBase
    {
        [info("fill it with array of items that will be processed")]
        public static readonly string items = "items";

        [info("")]
        [model("maximum number of concurring tasks (split items by packs of 'task_count' size)")]
        public static readonly string task_count = "task_count";

        [info("actions that will be applied on each element of 'items'")]
        [model("Action")]
        public static readonly string process = "process";

        public override void Process(opis message)
        {
           opis ms = SpecLocalRunAll();

            instanse.MF.use_transient = true;

            int tmax = ms[task_count].intVal > 0 ? ms[task_count].intVal : 24;

            opis itms = ms[items];
            //Task[] tasks = new Task[itms.listCou];
            Task[] tasks = new Task[tmax];


            int tcou = 0;

            for (int i = 0; i < itms.listCou; i++)
            {
                //if (itms[i] != null)
                //{
                //    opis itm = itms[i];
                //    opis proc = ms[process].Duplicate();
                //    tasks[i] = Task.Run(() => instanse.ExecActionResponceModelsList(proc, itm));
                //}

                if (itms[i] != null)
                {                    
                    opis itm = itms[i];
                    opis proc = ms[process].Duplicate();
                    tasks[tcou] = Task.Run(() => instanse.ExecActionResponceModelsList(proc, itm));
                    tcou++;
                }

                if(tcou == tmax)
                {
                    Task.WaitAll(tasks);
                    tcou = 0;
                    tasks = new Task[tmax];
                }

            }

            if (tcou > 0)
            {
                Array.Resize(ref tasks, tcou);
                Task.WaitAll(tasks);
            }

            instanse.MF.use_transient = false;
        }

    }
}
