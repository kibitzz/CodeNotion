using basicClasses.models.DataSets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace basicClasses.models.SharedDataContextDrivers
{
    [info("")]
    [appliable(" GetAnyPartOfOpis") ]
   public class contextRole:ModelBase
    {

        [model("SharedContextRoles")]
        public static readonly string list = "list";

        public override void Process(opis message)
        {
            //for (int i = 0; i < message.listCou; i++)
            //{
            if (SharedContextRoles.addRole(message, sharedVal))
            {
                if (sharedVal.getPartitionIdx(message.PartitionName) != -1)
                {
                    sharedVal.RemoveArrElem(sharedVal[message.PartitionName]);
                }
            }
                
            //}
        }
    }
}
