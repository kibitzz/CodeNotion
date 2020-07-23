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


        public override void Process(opis message)
        {           
            opis ms = SpecLocalRunAll();

            build(message, ms[value]);
        }

        void build(opis host, opis emb)
        {
            for (int i = 0; i < emb.listCou; i++)
            {
                var v = emb[i];
                host[v.PartitionName].body = v.body;

                build(host[v.PartitionName], v);
            }
        }
    }
}
