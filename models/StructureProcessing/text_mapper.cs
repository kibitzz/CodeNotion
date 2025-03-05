using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace basicClasses.models.StructureProcessing
{    
    [info("")]
    public class text_mapper : ModelBase
    {

        [info("int value")]
        public static readonly string capacity = "capacity";

        static Dictionary<string, string> text = new Dictionary<string, string>();

        public override void Process(opis message)
        {
            opis ms = SpecLocalRunAll();

            if(text == null)
                text = new Dictionary<string, string>(ms[capacity].intVal);

            message.RunRecursively(o =>
            {
                o.PartitionKind = Get(o.PartitionKind);
                o.PartitionName = Get(o.PartitionName);
                o.body = Get(o.body);
            });
          
        }

        string Get(string t)
        {            
            if (text.TryGetValue(t, out var name))
            {
                return name;
            }
            else
            {
                try
                {
                    text.Add(t, t);
                }catch
                {

                }
                return t;
            }
        }
    }

}
