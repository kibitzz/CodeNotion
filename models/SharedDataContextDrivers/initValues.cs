using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace basicClasses.models.SharedDataContextDrivers
{
    [info("add all items from spec to shared values context (SVC) or to the function packages partition. can contain fill models or constants.  message is not modified")]
    [appliable("Action creation Builders func")]
   public class initValues:ModelBase
    {


        [model("FlagModelSpec")]
        [info("set body of this partition if you want  this initialization run more than one time (in message validation)")]
        public static readonly string Run_multiple_times = "Run_multiple_times";      

        [model("spec_tag")]
        [info("action models will be placed to separate partition(not in SVC)")]
        public static readonly string put_as_package = "put_as_package";

        [model("Action")]
        [info("new function stub")]
        public static readonly string fun = "fun";

        public override void Process(opis message)
        {
            opis itemsToInit = modelSpec;
            opis svc = sharedVal;
           
            bool asPackage = modelSpec.isHere(put_as_package);
           
            for (int i = 0; i < itemsToInit.listCou; i++)
            {
                bool isModel = false;
                opis itm = itemsToInit[i];
                if (itemsToInit[i].PartitionKind != "Action")
                {
                    itm = itemsToInit[i].Duplicate();

                    instanse.ExecActionModel(itm, itm);

                    if (itm.PartitionKind != "Action")
                        itm.PartitionKind = itm.PartitionKind != "wrapper" ? itm.PartitionKind + "_done" : "wrapper";
                }
                else if (asPackage)
                {
                    isModel = true;
                    thisins[SysInstance.pkgIdx].AddArr(itm);  //TODO: Duplicate item when indexing optimization inmplemented                                
                }
                else
                {
                    itm = itm.Duplicate();
                }               

            // одразу додаємо в контекст, щоб наступна могла взяти це значення
                if (!isModel && itm.PartitionName != "Run_multiple_times"
                    && itm.PartitionName != "_path_")
                {
                    svc[itm.PartitionName] = itm;
                }
            }
           
            if (!modelSpec.OptionActive(Run_multiple_times))
                modelSpec.PartitionKind += "_done";// to prevent further execution
     
        }
    }
}
