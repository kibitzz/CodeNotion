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

        //[model("spec_tag")]
        //[info("")]
        //public static readonly string ReturnNamed = "ReturnNamed";


        public override void Process(opis message)
        {
            opis spec = modelSpec.Duplicate();
            instanse.ExecActionModelsList(spec);

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

            rn["Attributes"].Vset("InnerHtml", node.InnerHtml);
            rn["Attributes"].Vset("InnerText", node.InnerText);            

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
