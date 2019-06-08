using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace basicClasses.models.WEB_api
{
    class HtmlParser_HAP : ModelBase
    {
       
        [info("")]
        public static readonly string HtmlText = "HtmlText";

        [info("for optimization big raw html not added in structure, only parsed data.   optional, set constant for this instance.")]
        public static readonly string InnerHtmlLengthLimit = "InnerHtmlLengthLimit";

        //[model("spec_tag")]
        //[info("")]
        //public static readonly string ReturnNamed = "ReturnNamed";
        int maxHtmlShow = 500;

        public override void Process(opis message)
        {
            opis spec = modelSpec.Duplicate();
            instanse.ExecActionModelsList(spec);

            if (spec.isHere(InnerHtmlLengthLimit))
            {
                int.TryParse(spec.V(InnerHtmlLengthLimit), out maxHtmlShow);
            }

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(spec.V(HtmlText));

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
            var html = node.InnerHtml;
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
