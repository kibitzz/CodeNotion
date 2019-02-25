using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace basicClasses.models.String_proc
{
    [appliable("func")]
    class GetEnclosedText : ModelBase
    {

        [info("if missing - use previous value")]
        [model("")]
        public static readonly string source = "source";

        [info("from the start put ###")]
        [model("")]
        public static readonly string start = "start";

        [info("to the end put ###")]
        [model("")]
        public static readonly string fin = "fin";

        [info("if missing - use previous value")]
        [model("")]
        public static readonly string pos = "pos";

        string csource;
        int cpos;

        public override void Process(opis message)
        {
            opis spec = modelSpec.Duplicate();
            instanse.ExecActionModelsList(spec);

            if (spec.isHere(source))
                csource = spec.V(source);

            if (spec.isHere(pos))
                cpos = spec[pos].intVal;

            var rez = GetEnclosed("###" + csource + "###", spec.V(start), spec.V(fin), ref cpos);

            message.body = rez;
            message.CopyArr(new opis());          
        }

        public static string GetEnclosed(string srs, string st, string fin, ref int pos)
        {
            if (string.IsNullOrEmpty(srs))
                return "";

            var stp = srs.IndexOf(st, pos);
            stp = stp >= 0 ? stp + st.Length : srs.Length;

            var finp = fin.Length > 0 ? srs.IndexOf(fin, stp) : srs.Length - 1;
            pos = pos > finp ? pos : finp + fin.Length;

            if (finp - stp > 0)
                return srs.Substring(stp, finp - stp);
            else
            {
                return "";
            }
        }
    }
}
