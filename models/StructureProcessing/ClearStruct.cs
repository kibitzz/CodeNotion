using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace basicClasses.models.StructureProcessing
{
    [appliable("BodyValueModificator")]
    class ClearStruct : ModelBase
    {
    
        [model("spec_tag")]
        [info(" ")]
        public static readonly string clear_body = "clear_body";

        [model("spec_tag")]
        [info(" ")]
        public static readonly string clear_type = "clear_type";

        [model("spec_tag")]
        [info(" ")]
        public static readonly string clear_name = "clear_name";

        [model("spec_tag")]
        [info(" ")]
        public static readonly string do_not_recurse = "do_not_recurse";

        [info("partition names that should avoid transformation")]
        [model("")]
        public static readonly string Exceptions = "Exceptions";

        opis exept;

        public override void Process(opis message)
        {
            
           opis  mspec = modelSpec.Duplicate();
            instanse.ExecActionModelsList(mspec);

            exept = mspec[Exceptions];

            Do(message,
                mspec.isHere(clear_type), 
                mspec.isHere(clear_body), 
                mspec.isHere(clear_name),
                !mspec.isHere(do_not_recurse)
                );

        }

        bool IsException(opis curr)
        {
            return exept.isHere(curr.PartitionName);
        }

        void Do(opis curr, bool cltype, bool clbody, bool clpartn, bool recurse = true)
        {
            for (int i = 0; i < curr.listCou; i++)
            {

                if (cltype && !IsException(curr[i]))
                    curr[i].PartitionKind = String.Empty;

                if (clbody && !IsException(curr[i]))
                    curr[i].body = String.Empty;

                if (clpartn && !IsException(curr[i]))
                    curr[i].PartitionName = String.Empty;

                if (recurse)
                    Do(curr[i], cltype, clbody, clpartn);

            }

        }


    }
}
