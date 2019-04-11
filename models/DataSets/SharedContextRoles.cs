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
         
            return context[role].W();
        }

        public static bool addRole(opis item, opis context)
        {
            bool rez = false;
        

            return rez;
        }

        public static void SetRole(opis item, string roleName, opis context)
        {
            if (!string.IsNullOrEmpty(roleName))
            {
                context[roleName] = new opis(); // clean previous item wrapper            
                context.WrapByName(item, roleName);                
            }
        }

    }
}
