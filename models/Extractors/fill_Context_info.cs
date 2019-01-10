using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace basicClasses.models.Extractors
{
    [info("  message.body = o[message[0].PartitionName].body;")]
    [appliable("MsgTemplate creation FillerList initValues")]
    public class fill_Context_info: ModelBase
    {
        [info(" o[items].listCou;   ")]
        public static readonly string itemsCount = "itemsCount";

        public override void Process(opis message)
        {
            if (message.listCou > 0)
            {
                if (contexts.listCou == 1)
                {
                    instanse.Handle(contexts[0]);                   
                }

                if (message[0].PartitionName == itemsCount)
                {
                    message.intVal = o["items"].listCou;                   
                }
                else
                {
                    message.body = o[message[0].PartitionName].body;
                }


                //message.CopyArr(new opis());
            }

        }
    }
}
