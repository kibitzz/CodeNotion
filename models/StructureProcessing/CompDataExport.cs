using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace basicClasses.models.StructureProcessing
{
    class CompDataExport : ModelBase
    {
        [info(" any filler")]
        [model("")]
        public static readonly string source = "source";

        //[info("")]
        //[model("")]
        //public static readonly string source = "source";

        [model("spec_tag")]
        [info(" make multiline text - each array elem is at row")]
        public static readonly string as_list = "as_list";

        [model("")]
        [info(" list of names to put in line, if empty - put element partition and body in line ")]
        public static readonly string part_names = "part_names";

        [model("spec_tag")]
        [info("asc    or   desc")]
        public static readonly string order = "order";

        [model("spec_tag")]
        [info(" items part name to sort array, leave empty to sort on item partition name")]
        public static readonly string order_by_partition = "order_by_partition";

        [model("spec_tag")]
        [info(" number:  1 - as text; 2 - by text length; 3 - by int value  ")]
        public static readonly string sort_type = "sort_type";


        public override void Process(opis message)
        {
            opis spec = modelSpec.Duplicate();
            instanse.ExecActionModelsList(spec);

            var srs = spec[source];

            int sortt = (spec.isHere(sort_type)) ? spec[sort_type].intVal : 1;
           
            if (!string.IsNullOrEmpty( spec.V(order)))
            {
                var pnm = spec.V(order_by_partition);
                if (pnm.Length > 0)
                    srs.SortArrayBy_pname_body(pnm, sortt, spec.V(order) != "desc");
                else
                    srs.SortThisArrayBy_items_pname(sortt, spec.V(order) != "desc");
            }


            var rez = "";


            var pn = spec[part_names];

            for (int i = 0; i < srs.listCou; i++)
            {
                var line = "";
                if (pn.listCou > 0)
                {
                    for (int k = 0; k < pn.listCou; k++)
                    {
                        line += srs[i][pn[k].PartitionName] + "\t";
                    }
                }
                else
                {
                    line += srs[i].PartitionName + "\t " + srs[i].body;
                }

                rez += line + " \n";
            }


            message.body = rez;
            message.PartitionKind = "";
            message.CopyArr(new opis());
        }
    }
}
