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


            var p = spec.V(pname);

            if (spec.isHere(val))
                Set(p, spec[val]);
            else
            {
                message.PartitionKind = "";
                message.CopyArr(storage[p]);
                message.body = storage[p].body;
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
