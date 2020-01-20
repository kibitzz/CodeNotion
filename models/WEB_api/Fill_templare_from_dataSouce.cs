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

        [model("spec_tag")]
        [info(" do not  UTF_to_UTF25pref(replacement)  => Kirill_to_UTF")]
        public static readonly string no_encoding_conversion = "no_encoding_conversion";

        [model("spec_tag")]
        [info(" space to %20     /  to  %2F   and other non aplha symbols.   kirill convert too")]
        public static readonly string params_full_url_encoding = "params_full_url_encoding";

        [model("spec_tag")]
        [info(" space to +     /  to  %2F   and other non aplha symbols.   kirill convert too.  specially fitted to emulate aspnet form encoding of values")]
        public static readonly string aspnet_form_encoding = "aspnet_form_encoding";

        [model("spec_tag")]
        [info("   %D0%90  to  %25D0%2590")]
        public static readonly string params_UTF_to_UTF25pref_encoding = "params_UTF_to_UTF25pref_encoding";

        [model("spec_tag")]
        [info("  before encoding replace   %  to  %25")]
        public static readonly string percent_symbol_encode = "percent_symbol_encode";


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

                    if (modelSpec.isHere(percent_symbol_encode))
                        replacement = replacement.Replace("%", "%25");

                    if (!modelSpec.isHere(no_slash_conversion))
                        replacement = replacement.Replace("/", "%2F").Replace(" ", "+");

                    if (modelSpec.isHere(params_full_url_encoding))                    
                        replacement = TemplatesMan.UrlEncodeFull(replacement);
                    else
                    if (modelSpec.isHere(aspnet_form_encoding))
                        replacement = TemplatesMan.AspnetUrlEncodeFull(replacement);

                    if (modelSpec.isHere(params_UTF_to_UTF25pref_encoding))
                        replacement = TemplatesMan.UTF_to_UTF25pref(replacement);


                    if (!modelSpec.isHere(no_encoding_conversion))
                    {                       
                        data = data.Replace(torepl, TemplatesMan.Kirill_to_UTF(replacement));
                    }
                    else
                    {
                        data = data.Replace(torepl, replacement);
                    }
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

                    if (modelSpec.isHere(params_full_url_encoding))
                        replacement = TemplatesMan.UrlEncodeFull(replacement);

                    if (modelSpec.isHere(params_UTF_to_UTF25pref_encoding))
                        replacement = TemplatesMan.UTF_to_UTF25pref(replacement);

                    if (!modelSpec.isHere(no_encoding_conversion))
                    {                        
                        data = data.Replace(rl[i].PartitionName, TemplatesMan.Kirill_to_UTF(replacement));
                    }
                    else
                    {
                        data = data.Replace(rl[i].PartitionName, replacement);
                    }
                }
            }
    

            message.body= data;
            message.PartitionKind = "";
            message.CopyArr(new opis());

        }
    }
}
