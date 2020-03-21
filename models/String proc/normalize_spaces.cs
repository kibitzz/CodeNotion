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

        public static string NormalizeWhiteSpace2(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return string.Empty;
            }
      
            bool skipped = true;

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
                        skipped = true;
                    }
                }
                else
                {
                    skipped = false;
                    rez[pos] = c;
                    pos++;                  
                }
            }

            if (pos > 0 && char.IsWhiteSpace(rez[pos - 1]))
                pos--;

            if (pos == input.Length)
                return input;

            return new string(rez, 0, pos);
        }

        public static string NormalizeSpRemoveControlsSeparators(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return string.Empty;
            }

            bool skipped = true;

            char[] rez = new char[input.Length];
            int pos = 0;

            foreach (char c in input)
            {
                if (char.IsWhiteSpace(c) || char.IsControl(c) || char.IsSeparator(c))
                {
                    if (!skipped)
                    {
                        rez[pos] = ' ';
                        pos++;
                        skipped = true;
                    }
                }
                else
                {
                    skipped = false;
                    rez[pos] = c;
                    pos++;
                }
            }

            if (pos > 0 && char.IsWhiteSpace(rez[pos - 1]))
                pos--;

            if (pos == input.Length)
                return input;

            return new string(rez, 0, pos);
        }


       
    }
}
