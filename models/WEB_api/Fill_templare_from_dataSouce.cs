using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace basicClasses.models.WEB_api
{
    [appliable("webActorModel ")]
   public class Fill_templare_from_dataSouce:ModelBase
    {
        [model("")]
        [info("")]
        public static readonly string ReplaceList = "ReplaceList";

        [model("")]
        [info("")]
        public static readonly string String_templ = "String_templ";

        [model("")]
        [info("any filler, it will be used as values set for template filling")]
        public static readonly string values_container = "values_container";

        [model("spec_tag")]
        [info("")]
        public static readonly string use_container_names = "use_container_names";

        [model("spec_tag")]
        [info(" do not  Replace(\" / \", \" % 2F\");")]
        public static readonly string no_slash_conversion = "no_slash_conversion";

        public override void Process(opis message)
        {
            opis rl = modelSpec[ReplaceList].Duplicate();
            instanse.ExecActionModel(rl, rl);
            //string data = message.body;

            opis dd = modelSpec[String_templ].Duplicate();
            instanse.ExecActionModel(dd, dd);
            string data = dd.body;

            opis t = modelSpec[values_container].Duplicate();
            instanse.ExecActionModel(t, t);

            //logopis.AddArr(t);

            if (modelSpec.isHere(use_container_names))
            {
                for (int i = 0; i < t.listCou; i++)
                {
                    string torepl = "#" + t[i].PartitionName.Trim() + "#";
                    string replacement = t[i].body;

                    replacement = replacement.Trim(new char[] { '"' });
                    if(!modelSpec.isHere(no_slash_conversion))
                    replacement = replacement.Replace("/", "%2F").Replace(" ", "+");
                    replacement = TemplatesMan.UTF_to_UTF25pref(replacement);

                    data = data.Replace(torepl, TemplatesMan.Kirill_to_UTF(replacement));
                }

            }
            else
            {

                for (int i = 0; i < rl.listCou; i++)
                {
                    string replacement = t.V(string.IsNullOrEmpty(rl[i].body) ? rl[i].PartitionName.Trim(new char[] { '#' }) : rl[i].body);
                    replacement = replacement.Trim(new char[] { '"' });
                    if (!modelSpec.isHere(no_slash_conversion))
                        replacement = replacement.Replace("/", "%2F").Replace(" ", "+");
                    replacement = TemplatesMan.UTF_to_UTF25pref(replacement);
                    //logopis.Vset(i.ToString(), replacement);
                    //logopis.Vset("utf "+i.ToString(), TemplatesMan.Kirill_to_UTF(replacement));
                    data = data.Replace(rl[i].PartitionName, TemplatesMan.Kirill_to_UTF(replacement));
                }
            }
    

            message.body= data;

        }
    }
}
