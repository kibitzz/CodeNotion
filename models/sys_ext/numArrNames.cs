using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace basicClasses.models.sys_ext
{
    [info("додає елементам масиву закінчення _№    де № це порядковий номер елемента в масиві.  модель корисна для попередньої обробки до порівняння коли елементи мають однакові імена(partitionName)")]
    [appliable("Action")]
   public class numArrNames:ModelBase
    {
        public override void Process(opis message)
        {

            for (int i = 0; i < message.listCou; i++)
            {
                if(!message[i].PartitionName.EndsWith("_"+i.ToString()))
                {
                    message[i].PartitionName += "_" + i.ToString();
                }
            }

        }
    }
}
