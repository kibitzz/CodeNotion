using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace basicClasses.models.StructureProcessing
{
    [appliable("BodyValueModificator")]
    [info("")]
    public class proc_recursive : ModelBase
    {

        [info("actions to do with each item")]
        [model("Action")]
        public static readonly string process = "process";

        public override void Process(opis message)
        {
            opis ms = SpecLocalRunAll();

            run(message, ms[process]);          
        }

        private void run(opis o, opis proc)
        {
            instanse.ExecActionResponceModelsList(proc, o);

            for (int i = 0; i < o.listCou; i++)
            {
                run(o[i], proc);
            }
        }

    }
}
