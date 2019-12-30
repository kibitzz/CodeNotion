using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace basicClasses.models.SharedDataContextDrivers
{
 
    [appliable("exe creation Action ")]
    [info("delete all items from sharedVal array. to start entirely new context chain execution")]
    public class ClearSharedContext: ModelBase
    {
        public override void Process(opis message)
        {
            sharedVal.CopyArr(new opis());
        }

    }
}
