using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace basicClasses.models.String_proc
{
    [info("filler")]
    [appliable("func")]
    class GetEnclosedText : ModelBase
    {

        [info("if missing - use previous value")]
        [model("")]
        public static readonly string source = "source";

        [info("from the start put ###")]
        [model("")]
        public static readonly string start = "start";

        [info("from the start put ###")]
        [model("")]
        public static readonly string alt_start = "alt_start";

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

        [model("spec_tag")]
        [info("seach last contained item of <start> substring")]
        public static readonly string start_closer_to_end = "start_closer_to_end";

        [model("spec_tag")]
        [info("seach last contained item of <fin> substring")]
        public static readonly string fin_closer_to_end = "fin_closer_to_end";

        [model("spec_tag")]
        [info("result should contain start text or/and fin text  {sf, s, f}")]
        public static readonly string add_closures = "add_closures";

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

            bool startCloser_to_end = spec.isHere(start_closer_to_end);
            bool endingCloser_to_end = spec.isHere(fin_closer_to_end);

            string st = string.IsNullOrEmpty(spec.V(start)) ? spec.V(alt_start) : spec.V(start);

            var rez = GetEnclosed("###" + csource + "###", st, spec.V(fin), ref cpos, startCloser_to_end, endingCloser_to_end);

            var enclfound = spec.V(fin);

            if (string.IsNullOrEmpty(rez) && spec.isHere(alt_fin))
            {
                rez = GetEnclosed("###" + csource + "###", st, spec.V(alt_fin), ref cpos, startCloser_to_end, endingCloser_to_end);
                enclfound = spec.V(alt_fin);
            }

            if (string.IsNullOrEmpty(rez) && spec.isHere(alt_fin2))
            {
                rez = GetEnclosed("###" + csource + "###", st, spec.V(alt_fin2), ref cpos, startCloser_to_end, endingCloser_to_end);
                enclfound = spec.V(alt_fin2);
            }

            if (!string.IsNullOrWhiteSpace(rez))
            {
                if (spec.isHere(fin_as_separator))
                    cpos -= enclfound.Length;

                if (spec.isHere(add_closures))
                {
                    switch (spec.V(add_closures))
                    {
                        case "sf":
                            rez = st + rez + enclfound;
                            break;
                        case "s":
                            rez = st + rez;
                            break;
                        case "f":
                            rez = rez + enclfound;
                            break;
                    }
                }

                if (spec.isHere(if_not_empty))
                {
                    var par = new opis() {body = rez, PartitionName = "rez" };
                    instanse.ExecActionResponceModelsList(spec[if_not_empty], par);
                }
            }
           

            message.body = rez.Trim();
            message.PartitionKind = "";
            message.CopyArr(new opis());
        }

        public static string GetEnclosed(string srs, string st, string fin, ref int pos, bool lastSt, bool lastFin = false)
        {
            if (string.IsNullOrEmpty(srs))
                return "";

            st = st == "_" ? " " : st;
            fin = fin == "_" ? " " : fin;

            var stp = lastSt? srs.LastIndexOf(st) : srs.IndexOf(st, pos);
            stp = stp >= 0 ? stp + st.Length : srs.Length;

            var finp = lastFin ? srs.LastIndexOf(fin) : srs.IndexOf(fin, stp);

            finp = fin.Length > 0 ? finp : srs.Length - 1;
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
