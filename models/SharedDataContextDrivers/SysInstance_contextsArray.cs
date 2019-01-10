using basicClasses.models.DataSets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace basicClasses.models.SharedDataContextDrivers
{
    [info("надає доступ до масиву контекстів даного обєкта SysInstance")]
    [appliable("Action initValues")]
    public class SysInst_contextsArray:ModelBase
    {
        [model("spec_tag")]
        [info("загружає в SharedDataContext контекст з поточним індексом  ")]
        public static readonly string LoadContext = "LoadContext";

        [model("spec_tag")]
        [info("filler - заповнює значенням кількості доступних контекстів  ")]
        public static readonly string GetCount = "GetCount";

        [info("filler - заповнює  message масивом контекстів (пригодиться для обробки в foreach) ")]
        [model("spec_tag")]
        public static readonly string GetAllArray = "GetAllArray";

        public override void Process(opis message)
        {
            if (modelSpec.getPartitionIdx(LoadContext) != -1)
            {
                int idx = SharedContextRoles.GetRole(SharedContextRoles.ProcessIndex, sharedVal).intVal;
                opis currentContextItem = new opis();

                if (contexts.listCou > idx)
                    currentContextItem = contexts[idx];
                else
                {
                    currentContextItem.PartitionName = "currentContextItem";
                    currentContextItem.body = "ERR: index is out of range";
                }


                SharedContextRoles.SetRole(currentContextItem, "currentContext", sharedVal);
            }

            if (modelSpec.getPartitionIdx(GetCount) != -1)
            {
                message.intVal = contexts.listCou;
            }

            if (modelSpec.isHere(GetAllArray) )
            {
                message.AddArr(contexts);
            }
        }


    }
}
