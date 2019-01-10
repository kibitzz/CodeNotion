using basicClasses.models.SharedDataContextDrivers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace basicClasses.models.DataSets
{
   public class SharedContextRoles: ModelBase
    {
        [ignore]
        [info("ForEach  set this automatically ")]
        public static readonly string ProcessIndex = "ProcessIndex";
      

        [ignore]
        [info(" ForEach  set this automatically")]
        public static readonly string procItem = "procItem";
       

        [ignore]
        [info("")]
        public static readonly string roles = "roles";

        public static opis GetRole(string role, opis context)
        {
            opis rez = new opis("null");

            string s= context[roles].V(role);
            if (!string.IsNullOrEmpty(s))
                rez= context[s];

            if (rez.PartitionKind == "wrapper")
                rez = context.W(s);

            return rez;
        }

        public static bool addRole(opis item, opis context)
        {
            bool rez = false;

            for (int i = 0; i < item.listCou; i++)
            {
                if (item[i].PartitionKind == "contextRole")
                {
                    opis l = item[i][contextRole.list];
                    for (int k = 0; k < l.listCou; k++)
                    {
                        rez = true;
                        context[roles].Vset(l[k].PartitionName, item.PartitionName);
                    }                    
                }

            }

            return rez;
        }

        public static void SetRole(opis item, string roleName, opis context)
        {
            if (!string.IsNullOrEmpty(roleName))
            {
                context[roleName] = new opis(); // clean previous item wrapper            
                context.WrapByName(item, roleName);
                context[roles].Vset(roleName, roleName);
            }
        }

    }
}
