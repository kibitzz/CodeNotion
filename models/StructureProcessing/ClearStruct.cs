using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace basicClasses.models.StructureProcessing
{
    class ClearStruct : ModelBase
    {
    
        [model("spec_tag")]
        [info(" ")]
        public static readonly string clear_body = "clear_body";

        [model("spec_tag")]
        [info(" ")]
        public static readonly string clear_type = "clear_type";

        [info("partition names that should avoid transformation")]
        [model("")]
        public static readonly string Exceptions = "Exceptions";

        opis exept;

        public override void Process(opis message)
        {
            
           opis  mspec = modelSpec.Duplicate();
            instanse.ExecActionModelsList(mspec);

            exept = mspec[Exceptions];

            Do(message, mspec.isHere(clear_type), mspec.isHere(clear_body));

        }

        bool IsException(opis curr)
        {
            return exept.isHere(curr.PartitionName);
        }

        void Do(opis curr, bool cltype, bool clbody)
        {
            for (int i = 0; i < curr.listCou; i++)
            {

                if (cltype && !IsException(curr[i]))
                    curr[i].PartitionKind = "";

                if (clbody && !IsException(curr[i]))
                    curr[i].body = "";

                Do(curr[i], cltype, clbody);

            }

        }


    }
}
