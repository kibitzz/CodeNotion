using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace basicClasses.models.String_proc
{
    [appliable("func")]
    [info("filler. modifies message body")]
    class normalize_spaces : ModelBase
    {
        public override void Process(opis message)
        {
            message.body = NormalizeWhiteSpace2(message.body);
        }

        private static string NormalizeWhiteSpace2(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return string.Empty;
            }

        //    StringBuilder output = new StringBuilder();
            bool skipped = false;

            char[] rez = new char[input.Length];
            int pos = 0;

            foreach (char c in input)
            {
                if (char.IsWhiteSpace(c))
                {
                    if (!skipped)
                    {
                        rez[pos] = ' ';
                        pos++;
                        // output.Append(' ');
                        skipped = true;
                    }
                }
                else
                {
                    skipped = false;
                    rez[pos] = c;
                    pos++;
                    // output.Append(c);
                }
            }

            return new string(rez);
        }

        private static string NormalizeWhiteSpace(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return string.Empty;
            }

            StringBuilder output = new StringBuilder();
            bool skipped = false;

            foreach (char c in input)
            {
                if (char.IsWhiteSpace(c))
                {
                    if (!skipped)
                    {
                        output.Append(' ');
                        skipped = true;
                    }
                }
                else
                {
                    skipped = false;
                    output.Append(c);
                }
            }

            return output.ToString();
        }

    }
}
