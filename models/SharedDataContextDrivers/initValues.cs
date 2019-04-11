using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace basicClasses.models.SharedDataContextDrivers
{
    [info("один раз виконується для даного інстанса (instanse.ExecActionModelsList(message)), заповняє та додає набір даних(усі поля даного опису), переважно це будуть fill моделі або константи. спочатку виконуються моделі ініціалізації контекстного обєкта до заповнення(реєстрація ролі, очистка якихось параметрів)    message НЕ змінює взвагалі")]
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

            opis modelsPart = thisins["packages"];
            Dictionary<string, int> cash= (Dictionary<string, int>)thisins["packages"].bodyObject;
            bool asPackage = modelSpec.isHere(put_as_package);

            // не використовуємо ExecActionModelsList(modelSpec) бо моделі повині мати доступ
            // до даних отриманих попередніми моделями
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
                    int poz = modelsPart.AddArr(itm);
                    if (!cash.ContainsKey(itm.PartitionName))
                        cash.Add(itm.PartitionName, poz);                  
                }
                else
                {
                    itm = itm.Duplicate();
                }
          

            // одразу додаємо в контекст, щоб наступна могла взяти це значення
            if (!isModel && itm.PartitionName != "Run_multiple_times"
                    && itm.PartitionName != "_path_")
                {
                    sharedVal[itm.PartitionName] = itm;
                }
            }


            if (!modelSpec[Run_multiple_times].isInitlze) 
            modelSpec.PartitionKind += "_done";// to prevent further execution
     
        }
    }
}
