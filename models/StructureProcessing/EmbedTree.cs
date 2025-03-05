using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace basicClasses.models.StructureProcessing
{
    [appliable("Action ")]
    [info("modify message to contain all key-value structure of parameter <value>")]
    class EmbedTree : ModelBase
    {
        [info("support function execution to evaluate before assignment ")]
        public static readonly string value = "value";

        [info("put number to the body of each node, the number of how many structures contain this node ")]
        [model("spec_tag")]
        public static readonly string count_hits = "count_hits";

        public override void Process(opis message)
        {           
            opis ms = SpecLocalRunAll();

            build(message, ms[value], ms.isHere(count_hits, false));
        }

        void build(opis host, opis emb, bool set_count_hits)
        {
            for (int i = 0; i < emb.listCou; i++)
            {
                var v = emb[i];
                var hv = host[v.PartitionName];
                if (set_count_hits)
                {
                    hv.body = (++hv.intVal).ToString();
                }
                else
                {
                    hv.body = v.body;
                }
                hv.PartitionKind = v.PartitionKind;

                build(hv, v, set_count_hits);
            }
        }
    }
}
