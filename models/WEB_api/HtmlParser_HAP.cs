using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace basicClasses.models.WEB_api
{
    [appliable("BodyValueModificator")]
    class HtmlParser_HAP : ModelBase
    {
       
        [info("")]
        public static readonly string HtmlText = "HtmlText";

        [info("for optimization big raw html not added in structure, only parsed data.   optional, set constant for this instance.")]
        public static readonly string InnerHtmlLengthLimit = "InnerHtmlLengthLimit";

        //[model("spec_tag")]
        //[info("")]
        //public static readonly string ReturnNamed = "ReturnNamed";

        [model("spec_tag")]
        [info(" use this to set rules of replacing incorrect html substrings in further parsing. add elements to this partition where PN <bad> is text to replace and <good> is what replace it (only for this instance)")]
        public static readonly string fix_html = "fix_html";
       
        int maxHtmlShow = 500;
        opis fixes;
        bool numeratePartitions = false;

        public override void Process(opis message)
        {
            opis spec = modelSpec.Duplicate();
            instanse.ExecActionModelsList(spec);

            if (spec.isHere(InnerHtmlLengthLimit))
            {
                int.TryParse(spec.V(InnerHtmlLengthLimit), out maxHtmlShow);
            }
         
            var htmltext = spec.V(HtmlText);
            if (spec.isHere(fix_html))
            {
                fixes = spec[fix_html];
                return;
            }

            if (fixes != null)
            {               
                for (int i = 0; i < fixes.listCou; i++)
                {
                    htmltext = htmltext.Replace(fixes[i]["bad"].body, fixes[i]["good"].body);
                }
            }


            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(htmltext);

            var rez = new opis();

            var docRoot = rez["DocumentNode"];

            Trace(doc.DocumentNode, docRoot);

            message.CopyArr( docRoot);         
        }


        void Trace(HtmlNode node, opis rn)
        {
            foreach (var a in node.Attributes)
                rn["Attributes"].Vset(a.Name, a.Value);

            var htmobj = rn["Attributes"]["InnerHtml"];
         //   var html = node.InnerHtml;

            var html = "";

            try
            {
                html = node.InnerHtml;
            }
            catch (ArgumentOutOfRangeException e)
            {
                html = "";
            }

            if (html.Length < maxHtmlShow)
                htmobj.body = html.Trim()
                                                .Replace('\n', ' ')
                                                .Replace('\t', ' ');
                                                
            rn["Attributes"].Vset("InnerText", node.InnerText.Trim()    );            

            foreach (var n in node.ChildNodes)
            {
                if (n.Name != "#text")
                {
                    opis cn = new opis();
                    cn.PartitionName = n.Name;
                   
                    rn.AddArr(cn);
                    Trace(n, cn);
                }
            }               
        }


    }
}
