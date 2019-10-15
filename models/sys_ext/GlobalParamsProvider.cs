using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace basicClasses.models.sys_ext
{
    [info(" filler")]
    public class GlobalParamsProvider : ModelBase
    {
        [info("")]
        [model("")]
        public static readonly string pname = "pname";

        [info("value to put for the name.  delete this partition to get data from storage (instead of putting value to it) ")]
        [model("")]
        public static readonly string val = "val";

        static opis storage = new opis();

        public override void Process(opis message)
        {
            opis spec = modelSpec.Duplicate();
            instanse.ExecActionModelsList(spec);


            if (spec.isHere(val))
                Set(spec.V(pname), spec[val]);
            else
            {
                message.CopyArr(storage[pname]);
                message.body = storage[pname].body;
            }

        }

        public static void Set(string n, opis v)
        {
            if (storage == null)
                storage = new opis();

            storage[n] = v;

        }

    }
}
