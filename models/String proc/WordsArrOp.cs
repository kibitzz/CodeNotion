using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace basicClasses.models.String_proc
{
    [info("split text to array of words and give you ability to choose which one to return")]
    [appliable("func")]
    class WordsArrOp : ModelBase
    {
        [info("")]
        [model("")]
        public static readonly string source = "source";

        [info("return text composed by last N count of words. Put value of N to this field")]
        [model("")]
        public static readonly string last_N = "last_N";

        [info("return text composed by first N count of words. Put value of N to this field")]
        [model("")]
        public static readonly string first_N = "first_N";


        public override void Process(opis message)
        {
            opis spec = modelSpec.Duplicate();
            instanse.ExecActionModelsList(spec);

            string csource = spec.V(source);

            var arr = csource.Split();

            var rez = "";

            if (spec.isHere(last_N))
            {
                int n = 0;
                int.TryParse(spec.V(last_N), out n);
                rez = String.Join(" ", arr.Skip(arr.Length > n ? arr.Length - n : 0).ToList());
            }

            if (spec.isHere(first_N))
            {
                int n = 0;
                int.TryParse(spec.V(first_N), out n);
                rez = String.Join(" ", arr.Take(arr.Length < n ? arr.Length  : n).ToList());
            }




            message.body = rez.Trim();
            message.PartitionKind = "";
            message.CopyArr(new opis());
        }


    }
}
