using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace basicClasses.models.String_proc
{
    [appliable("func")]
    [info("filler. return list of text chunks that matched one of the patterns.  body contain remained original text without matched text chunks")]
    class RegexParcer : ModelBase
    {
        [info("text to process")]
        [model("")]
        public static readonly string text = "text";

        [info("list of REGEXP patterns.   use _ symbol instead space if pattern starts or ends by space")]
        [model("")]
        public static readonly string patterns = "patterns";       
        

        public override void Process(opis message)
        {
            opis ms = SpecLocalRunAll();
            var rez = new opis();
            int rezcou = 0;

            var txt = ms.V(text);
            var ptn = ms[patterns].ListValues(); ;

            foreach (var p in ptn)
            {
                if (string.IsNullOrEmpty(p))
                    continue;
                Regex regex = new Regex(p.Replace("_", " "));
                MatchCollection matches = regex.Matches(txt);
                if (matches.Count > 0)
                {
                    foreach (Match match in matches)
                    {
                        rezcou++;
                        rez.Vset(rezcou.ToString(), match.Value);
                        txt = txt.Replace(match.Value, "");                       


                    }
                }
            }

            message.body = txt;
            message.CopyArr(rez);

          
        }
    }

}
