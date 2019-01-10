using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace basicClasses.models.Extractors
{
    [appliable("MsgTemplate creation FillerList")]
    public class Fill_Spec_Info:ModelBase
    {

        [info("select one option from list to get int value from it")]
        public static readonly string name_of_spec = "name_of_spec";

        public override void Process(opis message)
        {
            if (message.listCou > 0)
            {
                if (message.body == name_of_spec)
                {
                    message.body = spec.PartitionName;
                }
                else
                {
                    message.body = spec[message.body].body;
                }
            }

        }
    }
}
