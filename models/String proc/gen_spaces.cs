using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace basicClasses.models.String_proc
{
    public class gen_spaces : ModelBase
    {
        [model("")]
        [info(" filler ")]
        public static readonly string length = "length";

        public override void Process(opis message)
        {
            opis spec = modelSpec.Duplicate();
            instanse.ExecActionModelsList(spec);
            string cons = "                                              ";

            message.body = cons.Substring(0, spec[length].intVal);

        }
    }
}
