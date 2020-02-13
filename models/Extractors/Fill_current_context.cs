using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace basicClasses.models.Extractors
{
    [info("   message.CopyArr(CTX.curr);    ")]
    [appliable("MsgTemplate creation initValues ")]
    public  class Fill_current_context:ModelBase
    {
        public override void Process(opis message)
        {           
            message.CopyArr(contex.GetHierarchyStub(CTX.curr));
        }
    }

    [info(" message.CopyArr( CTX.GoUp());   ")]
    [appliable("MsgTemplate creation")]
    public class Fill_upper_context : ModelBase
    {
        public override void Process(opis message)
        {                      
            message.CopyArr(contex.GetHierarchyStub(CTX.GoUp()));
        }
    }
}
