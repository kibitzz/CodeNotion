using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace basicClasses.models.StructureProcessing
{
   

    [info(" filler ")]
    class arr_length : ModelBase
    {
        [model("")]
        [info(" fill array to eval ")]
        public static readonly string source = "source";

        public override void Process(opis message)
        {
            opis ms = SpecLocalRunAll();
            
            message.body = ms[source].W().listCou.ToString();

        }
    }
}
