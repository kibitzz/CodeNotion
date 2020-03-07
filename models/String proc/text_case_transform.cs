using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace basicClasses.models.String_proc
{
    class text_case_transform : ModelBase
    {
        [info("Higher First Letter")]
        [model("spec_tag")]
        public static readonly string first_higher = "first_higher";

        [info("all to lower and trim")]
        [model("spec_tag")]
        public static readonly string to_lower = "to_lower";

        public override void Process(opis message)
        {
            opis spec = modelSpec.Duplicate();
            instanse.ExecActionModelsList(spec);

            if (spec.isHere(first_higher))
            {
                var arr = message.body.Trim().ToLower().ToCharArray();               

                StringBuilder output = new StringBuilder();
                bool prevsp = true;
                foreach (var c in arr)
                {

                    if (!char.IsWhiteSpace(c) && prevsp)
                    {
                        output.Append(char.ToUpper(c));
                    }
                    else
                    {
                        output.Append(c);
                    }

                    prevsp = char.IsWhiteSpace(c);
                }

                message.body = output.ToString();
            }

            if (spec.isHere(to_lower))
            {
                message.body = message.body.ToLower().Trim();
            }



        }


    }
}
