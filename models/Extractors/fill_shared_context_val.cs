using basicClasses.models.DataSets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace basicClasses.models.Extractors
{
    [info("DO NOT use  ")]
    [appliable("MsgTemplate creation FillerList BodyValueModificator")]
    public class fill_shared_context_val:ModelBase
    {

        public override void Process(opis message)
        {
            if (message.listCou > 0)
            {
                string name = message[0].PartitionName;
                message.body = sharedVal[name].body;

                message.CopyArr(sharedVal[name]);

            }
            else
            {

            }
        }

    }


    [info("береться обєкт з Роллю вказаною в BODY та копіюється його масив і body")]
    [appliable("  creation    ")]
    public class fill_shared_context_Role : ModelBase
    {
        [info("message  wrapping object with such role    message.Wrap(rez);")]
        [model("")]
        public static readonly string Wrap = "Wrap";

        public override void Process(opis message)
        {
            if (message.PartitionKind == name)
                message.PartitionKind += "_done";

            if (message.listCou > 0 && message.getPartitionIdx(Wrap) == -1)
            {
                string name = message[0].PartitionName;
                opis tmp = SharedContextRoles.GetRole(name, sharedVal);

                    message.body = tmp.body;
                message.CopyArr(tmp);

                if (modelSpec.getPartitionIdx(Wrap) != -1)
                {
                    message.Wrap(tmp);
                }
                else
                {
                    message.body = tmp.body;
                    message.CopyArr(tmp);
                }

            }
            else
            {
                string name = message.body;
                if (!string.IsNullOrEmpty(name))
                {
                    opis tmp = SharedContextRoles.GetRole(name, sharedVal);

                    if (modelSpec.getPartitionIdx(Wrap) != -1)
                    {
                        message.Wrap(tmp);
                    }
                    else
                    {
                        message.body = tmp.body;
                        message.CopyArr(tmp);
                    }
                }
            }
        }

    }


    [info("береться обєкт з Роллю вказаною в BODY та копіюється його масив і body")]
    [appliable("MsgTemplate all creation FillerList BodyValueModificator TreeDataExtractor ")]
    public class fill_Role : ModelBase
    {
      
        public override void Process(opis message)
        {
            if (message.PartitionKind == name)
                message.PartitionKind += "_done";

            string nm = message.body;
            if (!string.IsNullOrEmpty(nm))
            {
                opis tmp = sharedVal.isHere(nm) ? sharedVal[nm].W() : new opis();// SharedContextRoles.GetRole(nm, sharedVal);

                if (modelSpec.getPartitionIdx("W") != -1)
                {
                    message.Wrap(tmp);
                }
                else
                {
                    message.body = tmp.body;
                    message.CopyArr(tmp);
                }
            }
        }

    }

}
