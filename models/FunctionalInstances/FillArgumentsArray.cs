using basicClasses.models.DataSets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace basicClasses.models.FunctionalInstances
{
    [info(" Arguments.GetParams(thisins);   message.CopyArr(args);")]
    [appliable("RangeAndAssign initValues")]
    public class FillArgumentsArray : ModelBase
    {
        public override void Process(opis message)
        {          
            opis args = Arguments.GetParams(thisins);
            message.CopyArr(args);
        }
    }
}
