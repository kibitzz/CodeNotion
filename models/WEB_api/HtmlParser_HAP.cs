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

        [info("for optimization big raw html not added in structure, only parsed data.   optional, set constant for this instance.  Set 0 (zero) value to omit InnerHtml generation")]
        public static readonly string InnerHtmlLengthLimit = "InnerHtmlLengthLimit";

        //[model("spec_tag")]
        //[info("")]
        //public static readonly string ReturnNamed = "ReturnNamed";

        [model("spec_tag")]
        [info(" use this to set rules of replacing incorrect html substrings in further parsing. add elements to this partition where PN <bad> is text to replace and <good> is what replace it (only for this instance)")]
        public static readonly string fix_html = "fix_html";

        [model("spec_tag")]
        [info(" do not calculate InnerHtml and InnerText for branch names listed here")]
        public static readonly string ignore_names_attr = "ignore_names_attr";

        int maxHtmlShow = 500;
        opis fixes;
        List<string> namesAtrIgnore = null;

        public override void Process(opis message)
        {
            opis spec = modelSpec.Duplicate();
            instanse.ExecActionModelsList(spec);

            if (spec.isHere(InnerHtmlLengthLimit))
            {
                int.TryParse(spec.V(InnerHtmlLengthLimit), out maxHtmlShow);
            }

            if (spec.isHere(ignore_names_attr))
            {
                namesAtrIgnore = new List<string>();
                for (int i = 0; i < spec[ignore_names_attr].listCou; i++)
                    namesAtrIgnore.Add(spec[ignore_names_attr][i].PartitionName);
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


        void Trace(HtmlNode node, opis rn, bool ignoreattr = false)
        {

            rn.ReinitArr(node.ChildNodes.Count + 2);
          //  rn.ArrResize(node.ChildNodes.Count + 2);

            rn[0] = new opis(node.Attributes.Count + 3)
            {
                PartitionName = "Attributes"
            };

            foreach (var a in node.Attributes)
                rn[0].Vset(a.Name, a.Value);

            if (!ignoreattr && (namesAtrIgnore == null || !namesAtrIgnore.Contains(node.Name)))
            {
                var htmobj = rn[0]["InnerHtml"];

                if (maxHtmlShow != 0)
                {                  
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
                }

                rn[0].Vset("InnerText", node.InnerText.Trim().Replace("\n", " ")
                                                    .Replace("\t", " ").Replace("                ", " ").Replace("     ", " "));
            } else
                ignoreattr = !ignoreattr;
          

            foreach (var n in node.ChildNodes)
            {
                if (n.Name != "#text")
                {
                    opis cn = new opis(-1);
                    cn.PartitionName = n.Name;
                    //cn.PartitionKind = "";
                    //cn.body = "";


                    rn.AddArr(cn);
                    Trace(n, cn, ignoreattr);
                }
            }               
        }


    }
}
