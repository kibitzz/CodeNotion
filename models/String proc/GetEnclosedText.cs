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

        [info("alternative fin.  if end element <fin> not found - search for alternative")]
        [model("")]
        public static readonly string alt_fin = "alt_fin";

        [info("alternative fin.  if end element <alt_fin> not found - search for alternative")]
        [model("")]
        public static readonly string alt_fin2 = "alt_fin2";

        [info("if missing - use previous value")]
        [model("")]
        public static readonly string pos = "pos";

        [model("spec_tag")]
        [info(" roll back position by enclosed string length")]
        public static readonly string fin_as_separator = "fin_as_separator";

        [info("if result is not empty - run some code")]
        [model("Action")]
        public static readonly string if_not_empty = "if_not_empty";


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

            var enclfound = spec.V(fin);

            if (string.IsNullOrEmpty(rez) && spec.isHere(alt_fin))
            {
                rez = GetEnclosed("###" + csource + "###", spec.V(start), spec.V(alt_fin), ref cpos);
                enclfound = spec.V(alt_fin);
            }

            if (string.IsNullOrEmpty(rez) && spec.isHere(alt_fin2))
            {
                rez = GetEnclosed("###" + csource + "###", spec.V(start), spec.V(alt_fin2), ref cpos);
                enclfound = spec.V(alt_fin2);
            }

            if (!string.IsNullOrWhiteSpace(rez))
            {
                if (spec.isHere(fin_as_separator))
                    cpos -= enclfound.Length;

                if (spec.isHere(if_not_empty))
                    instanse.ExecActionModelsList(spec[if_not_empty]);
            }

            message.body = rez;
            message.PartitionKind = "";
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
