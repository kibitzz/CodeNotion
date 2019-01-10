using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace basicClasses.models.DataSets
{
   public class Arguments
    {
      
        public static readonly string arguments = "arguments";
        public static readonly string parameter = "parameter";

        public static void AddArg(opis thisins, string paramType, opis arg)
        {

            thisins[arguments]["ready"].intVal += thisins[arguments][paramType].intVal == 0 ? 1 : 0;
            thisins[arguments][paramType].intVal++;
            thisins[arguments][paramType].AddArr(arg);
        }

        public static void AddParam(opis thisins, string paramType)
        {
            thisins[arguments][paramType].body = "0";
            thisins[arguments][paramType].PartitionKind = "parameter";
            thisins[arguments]["count"].intVal++;
       
        }

        public static opis GetParams(opis thisins)
        {
            opis rez = new opis();
            opis a = thisins[arguments];

            for (int i = 0; i < a.listCou; i++)
            {
                if (a[i].PartitionKind == parameter)
                {
                    rez.AddArrRange(a[i]);
                }
            }
           
            return rez;

        }

    }
}
