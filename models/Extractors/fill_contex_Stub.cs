using basicClasses.models.DataSets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace basicClasses.models.Extractors
{

    [info("contex.GetHierarchyStub")]
    [appliable("FillerList MsgTemplate BodyValueModificator")]
   public class fill_contex_Stub:ModelBase
    {
        [info("role SharedDataContext   name should be placed as <.body>")]
        [model("SharedContextRoles")]
        public static readonly string sdc_Role = "sdc_Role";
        public override void Process(opis message)
        {
            opis source =o;
            if (modelSpec[sdc_Role].isInitlze)
                source = SharedContextRoles.GetRole(modelSpec[sdc_Role].body, sharedVal);
            if (source != null)
            {
                opis t = contex.GetHierarchyStub(source);
                message.body = t.body;
                message.CopyArr(t);
            }
        }
    }
}
