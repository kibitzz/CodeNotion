using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace basicClasses.models.StructureProcessing
{
    [appliable("Action exe all")]
    public  class TemplateSearch : ModelBase
    {
        [model("")]
        [info("")]
        public static readonly string source = "source";

        [model("")]
        [info("")]
        public static readonly string template = "template";

        [model("spec_tag")]
        [info("search till the end, and form(return) an array of such items")]
        public static readonly string all = "all";

        public override void Process(opis message)
        {
           opis ms=  modelSpec.Duplicate();
            instanse.ExecActionModelsList(ms);
            opis rez = new opis();

            ms[source].FindByTemplateValue(ms[template], rez, ms.isHere(all));

            message.CopyArr(rez);
            if (!ms.isHere(all) && rez.listCou > 0)
                message.CopyArr(rez[0]);
            
        }
    }
}
