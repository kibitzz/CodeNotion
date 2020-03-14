using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace basicClasses.models.String_proc
{
    [info(" filler ")]
    class b_length : ModelBase
    {
        [model("")]
        [info(" fill body to eval ")]
        public static readonly string source = "source";

        public override void Process(opis message)
        {
            opis spec = modelSpec.Duplicate();
            instanse.ExecActionModelsList(spec);

            message.body = spec[source].body.Length.ToString();

        }
    }
}
