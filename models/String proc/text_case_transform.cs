using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace basicClasses.models.String_proc
{
    class text_case_transform : ModelBase
    {
        [info("all to lower and trim")]
        [model("spec_tag")]
        public static readonly string to_lower = "to_lower";

        [info("every word First Letter to upper")]
        [model("spec_tag")]
        public static readonly string first_higher = "first_higher";

        [info("First letter afrer each dot")]
        [model("spec_tag")]
        public static readonly string sentence = "sentence";

        
        public override void Process(opis message)
        {
            opis spec = modelSpec.Duplicate();
            instanse.ExecActionModelsList(spec);

            if (spec.isHere(to_lower))
            {
                message.body = message.body.ToLower().Trim();
            }

            if (spec.isHere(first_higher))
            {
                message.body = upper(message.body, (x, prev) => char.IsWhiteSpace(x));                            
            }
                       
            if (spec.isHere(sentence))
            {
                message.body = upper(message.body, (x, prev) => (x == '.' || (char.IsWhiteSpace(x) && prev)));
            }

            //if (spec.isHere(sentence))
            //{
            //    message.body = upper(message.body, (x, prev) => (x == '.' || (char.IsWhiteSpace(x) && prev)));
            //}



        }

        string upper(string t, Func<char, bool, bool> f)
        {
            var arr = t.Trim().ToCharArray();

            StringBuilder output = new StringBuilder();
            bool prevsp = true;
            foreach (var c in arr)
            {

                if (!f(c, prevsp) && prevsp)
                {
                    output.Append(char.ToUpper(c));
                }
                else
                {
                    output.Append(c);
                }

                prevsp = f(c, prevsp);
            }

            return output.ToString();
        }


    }
}
