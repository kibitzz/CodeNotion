using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace basicClasses.models.StructureProcessing
{
    
    [appliable("BodyValueModificator func")]
    [info("filler. wrap")]
    class reverse_order : ModelBase
    {

        public override void Process(opis message)
        {
            //  opis ms = SpecLocalRunAll();

            Array.Resize(ref message.arr, message.paramCou);
            Array.Reverse(message.arr);
        }

    }



}
