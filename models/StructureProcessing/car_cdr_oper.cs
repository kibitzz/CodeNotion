using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace basicClasses.models.StructureProcessing
{
  
    [appliable("BodyValueModificator func")]
    [info("filler. wrap")]
    class car_cdr_oper : ModelBase
    {

        [info("text")]
        public static readonly string source = "source";
      

        [info("The CAR of a list is, quite simply, the first item in the list")]
        [model("spec_tag")]
        public static readonly string car = "car";

        [info("(not implemented yet) The CDR of a list is the rest of the list, that is, the cdr function returns the part of the list that follows the first item.")]
        [model("spec_tag")]
        public static readonly string cdr = "cdr";



        public override void Process(opis message)
        {
            opis ms = SpecLocalRunAll();
            var rez = new opis();

            var srs = ms[source];

            message.CopyArr(new opis());

            if (ms.isHere(car, false))
            {
               
                message.Wrap(srs.listCou > 0 ? srs[0] : new opis());
            }

            if (ms.isHere(cdr, false)) 
            {
                
            }
           
        }

       


    }


}
