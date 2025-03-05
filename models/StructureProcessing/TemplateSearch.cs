using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace basicClasses.models.StructureProcessing
{
    [info("search in source (on all levels) data structures that match given template and return found array. FUNC 2: Extract data fields on  given template ")]
    [appliable("Action exe all")]
    public  class TemplateSearch : ModelBase
    {
        [model("")]
        [info("")]
        public static readonly string source = "source";

        [model("")]
        [info("put ??? to body of any template node to get matched item in results.   You can name extracted branches by putting some text after ??? in body (NOTE !!! in this case only one item of each named branch kind will be returned).  put ### to search body as substring template ")]
        public static readonly string template = "template";

        [model("spec_tag")]
        [info("search till the end, and form(return) an array of such items")]
        public static readonly string all = "all";

        [model("spec_tag")]
        [info("if template contains ??? put to results those items, not the branches that match whole template")]
        public static readonly string returnItemsNotBranches = "returnItemsNotBranches";

        [model("spec_tag")]
        [info("should be no additional fields in found structure. if additional fields present value become not valid and be ignored")]
        public static readonly string exact_structure = "exact_structure";

        [model("spec_tag")]
        [info("check only this array items for conformity, do not search in subitems ")]
        public static readonly string only_top_level = "only_top_level";

        [model("spec_tag")]
        [info("pure function mode - do not modify instanse state, so it can be run in parallel. modelSpec is not run, 'message' is processed as source, and result will be returned in 'message', so message should be composed as opis that contain original source as item and then be filled with search results  ")]
        public static readonly string run_as_pure_function = "run_as_pure_function";

        public override void Process(opis message)
        {
            opis to_process = message;
            opis ms = modelSpec;

            if (!ms.isHere(run_as_pure_function))
            {
                ms = SpecLocalRunAll();               
                to_process = ms[source];
            }
            
            opis rez = new opis();

            bool retdata = ms.isHere(returnItemsNotBranches);
            bool exactOnly = ms.isHere(exact_structure);
            bool onlyItemsLevel = ms.isHere(only_top_level, false);

            to_process.FindByTemplateValue(ms[template], rez, exactOnly, retdata, true, false, !onlyItemsLevel);

            message.CopyArr(rez, true);
            if (!ms.isHere(all) && rez.listCou > 0)
            {
                message.CopyArr(rez[0], true);
                message.body = rez[0].body;
            }

        }
    }
}
