using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace basicClasses.models.Builders
{
    [info("")]
    [appliable("Builders creation TreeDataExtractor")] 
    public class BuildTermBasicValuesContainer : ModelBase
    {      
        public override void Process(opis message)
        {

            message.Vset("spec_name", spec.PartitionName);
            message.Vset("waiter_name", waiter.PartitionName);
            message.Vset("spec_name", "");
            message.Vset("spec_name", "");
            message.Vset("spec_name", "");
            message.Vset("spec_name", "");
            message.Vset("spec_name", "");
            message.Vset("spec_name", "");
            message.Vset("spec_name", "");

        }
    }
}
