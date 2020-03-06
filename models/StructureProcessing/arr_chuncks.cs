using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace basicClasses.models.StructureProcessing
{
    [info("filler")]
    public class arr_chuncks : ModelBase
    {
        [model("")]
        [info("source filler function     or    constant array")]
        public static readonly string source = "source";

        [info("int value")]
        public static readonly string chunck_size = "chunck_size";

        public override void Process(opis message)
        {
            opis spec = modelSpec.Duplicate();
            instanse.ExecActionModelsList(spec);

            int siz = spec[chunck_size].intVal;

            opis srs = spec[source];
            opis rez = new opis(srs.listCou / siz);

            int idx = 0;

            while (idx < (srs.listCou - siz +1))
            {
                var itm = new opis(siz) { PartitionName = "itm"};
                rez.AddArr(itm);
                for (int i = 0; i < siz; i++)
                {                                
                    itm[i.ToString()] = srs[idx];
                    idx++;
                }               
            }


            message.body = "";
            message.CopyArr(rez);

        }
    }
}
