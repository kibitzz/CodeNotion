using basicClasses.models.DataSets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace basicClasses.models.SharedDataContextDrivers
{
    [info(" handle CTX data structure")]
    [appliable("Action ")]
    public  class SubContextHandler:ModelBase
    {
      
        [model("")]
        [info("")]
        public static readonly string Fill_itemsArray = "Fill_itemsArray";

        [model("")]
        [info("")]
        public static readonly string Fill_ContextsArray = "Fill_ContextsArray";

        //[model("")]
        //[info("")]
        //public static readonly string SomeFiloler = "Filler";

        public override void Process(opis message)
        {
           
            if(modelSpec.isHere(Fill_itemsArray))
            {
                message.CopyArr(o[context.items]);
            }

            if (modelSpec.isHere(Fill_ContextsArray))
            {
                message.CopyArr(o[context.subcon]);
            }

        }

    }
}
