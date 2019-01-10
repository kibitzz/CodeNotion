using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace basicClasses.models.StructureProcessing
{
    [appliable("template BodyValueModificator GetAnyPartOfOpis")]
    [info("будує, а що я ще не придумав")]
   public class template_builder: ModelBase
    {
        [model("")]
        [info("")]
        public static readonly string blueprint = "blueprint";

        [model("")]
        [info("")]
        public static readonly string blueprint_source = "blueprint_source";

        public override void Process(opis message)
        {
            
        }
    }
}
