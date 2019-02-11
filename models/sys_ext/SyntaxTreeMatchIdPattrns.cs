using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace basicClasses.models.sys_ext
{
    [appliable("")]
    [info("")]
    class SyntaxTreeMatchIdPatterns : ModelBase
    {
        public override void Process(opis message)
        {
            var Root = new opis();
            BuildLevel(o, Root);
            message.CopyArr(Root);
        }

        void BuildLevel(opis cont, opis lvl)
        {
            var arr = cont[context.subcon];
            var t =  new opis() { PartitionKind = cont[context.Organizer].body };
            lvl.AddArr(t);
            var id = cont.V(context.ID);
            t.Vset("id", id);

            if (id.Length > 27)
            {
                var idx = id.IndexOfAny(new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' });
                t.PartitionName = id.Substring(0, idx);
            }
            else
                t.PartitionName = t.PartitionKind;

          //  потуши636854239097818000_153121557

            for (int i = 0; i < arr.listCou; i++)
            {
                BuildLevel(arr[i], t);
            }
        }
    }

    
}
