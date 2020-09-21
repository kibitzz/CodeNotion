using basicClasses.models.WEB_api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace basicClasses.models.String_proc
{
    [appliable("")]
    [info("filler. put result to message.body")]
    class TextCodec : ModelBase
    {

        [info("text")]
        public static readonly string text = "text";

        [info("text snippets to substitute ")]
        public static readonly string blocks = "blocks";

        [info("text snippets equivalent to each block in blocks list")]
        public static readonly string substitution = "substitution";

        [info("")]
        [model("spec_tag")]
        public static readonly string underline_in_blocks_to_space = "underline_in_blocks_to_space";

        [info("use presets of snippets.    blocks is like \u0430  to be replaced by russian alpha")]
        [model("spec_tag")]
        public static readonly string UTF8BigEndian_to_Kirill = "UTF8BigEndian_to_Kirill";

        [info("use presets of snippets.    blocks is like &#1040; to be replaced by russian alpha")]
        [model("spec_tag")]
        public static readonly string UTF8Codes_to_Kirill = "UTF8Codes_to_Kirill";



        public override void Process(opis message)
        {
            opis ms = SpecLocalRunAll();
            var rez = new opis();

            if (ms.isHere(UTF8BigEndian_to_Kirill, false))
            {
                message.body = TemplatesMan.UTF8BigEndian_to_Kirill(ms.V(text));
            }

            if (ms.isHere(UTF8Codes_to_Kirill, false)) //&#1040;
            {
                message.body = TemplatesMan.UTF8Codes_to_Kirill(ms.V(text));
            }


            if (ms.isHere(blocks, false) && ms.isHere(substitution, false) && ms[blocks].listCou == ms[substitution].listCou)
            {

                var bl = ms[blocks].ListValues();
                var subst = ms[substitution].ListValues();
                if (ms.isHere(underline_in_blocks_to_space, false))
                {
                    bl.ForEach(x => x = x.Replace("_", " "));
                    subst.ForEach(x => x = x.Replace("_", " "));
                }

                message.body = Replace(ms.V(text), bl.ToArray(), subst.ToArray());
            }

        }

        public static string Replace(string s, string[] repl, string[] by)
        {

            StringBuilder b = new StringBuilder(s);

            for (int i = 0; i < repl.Length; i++)
            {
                b.Replace(repl[i], by[i]);
            }
            return b.ToString();
        }


    }


}
