using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace basicClasses.models.sys_ext
{
    [info("for message add array index to the name of partition")]
    [appliable("Action")]
    public class numArrNames : ModelBase
    {
        public override void Process(opis message)
        {
            string[] a = new string[message.listCou];

            for (int i = 0; i < message.listCou; i++)
            {
                a[i] = message[i].PartitionName ?? "";
                message[i].PartitionName += "_" + a.Where(x => x == (message[i].PartitionName ?? "")).Count();
            }

        }

        public static void do_num(opis message)
        {
            string[] a = new string[message.listCou];

            for (int i = 0; i < message.listCou; i++)
            {
                a[i] = message[i].PartitionName;
                int cou = a.Where(x => x == message[i].PartitionName).Count();
                message[i].PartitionName += cou > 1 ? ("_" + cou) : "";
            }
        }
    }
}
