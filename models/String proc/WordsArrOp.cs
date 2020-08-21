using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace basicClasses.models.String_proc
{
    [info("filler.  split text to array of words and give you ability to choose which one to return")]
    [appliable("func")]
    class WordsArrOp : ModelBase
    {
        [info("")]
        [model("")]
        public static readonly string source = "source";

        [info("")]
        [model("")]
        public static readonly string array_source = "array_source";

        [info("return text composed by last N count of words. Put value of N to this field")]
        [model("")]
        public static readonly string last_N = "last_N";

        [info("return text composed by first N count of words. Put value of N to this field")]
        [model("")]
        public static readonly string first_N = "first_N";

        [info("")]
        [model("")]
        public static readonly string separator = "separator";

        [info("if use last_N or first_N  -- then use this value as separator in join operation")]
        [model("")]
        public static readonly string glue_with = "glue_with";


        public override void Process(opis message)
        {
            opis spec = modelSpec.Duplicate();
            instanse.ExecActionModelsList(spec);

            string csource = spec.V(source);
           

            var arr = string.IsNullOrEmpty(spec.V(separator)) ? csource.Split() : csource.Split(spec.V(separator)[0]);

            if (spec.isHere(array_source))
            {
                var srs = spec[array_source];
                arr = new string[srs.listCou];
                for (int i = 0; i < srs.listCou; i++)
                {
                    arr[i] = srs[i].body;
                }
            }

            opis word_arr = new opis(arr.Length +1);
           
            for(int i =0; i < arr.Length; i++)
            {
                opis itm = new opis(0) { PartitionName = i.ToString(),  body = arr[i].Trim() };
                word_arr.AddArr(itm);
               // word_arr.Vset(i.ToString(), arr[i].Trim());
            }

            var rez = "";

            string join_sep = spec[glue_with].isInitlze ? spec.V(glue_with).Replace('_', ' ') : " ";

            if (spec.isHere(last_N))
            {
                int n = 0;
                int.TryParse(spec.V(last_N), out n);
                rez = String.Join(join_sep, arr.Skip(arr.Length > n ? arr.Length - n : 0).ToList());
            }

            if (spec.isHere(first_N))
            {
                int n = 0;
                int.TryParse(spec.V(first_N), out n);
                rez = String.Join(join_sep, arr.Take(arr.Length < n ? arr.Length  : n).ToList());
            }




            message.body = rez.Trim();
            message.PartitionKind = "";
            message.CopyArr(word_arr);
        }


    }
}
