using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace basicClasses.models.SharedDataContextDrivers
{
    [appliable("GetAnyPartOfOpis Action all")]
    [info("All operations with datastructure, modifying passed parameter (implicit) ")]
   public class BodyValueModificator:ModelBase
    {

        [info("inc;  dec;  set (set b - only body);  add_arr - treat patrition as array, and add value to it (opSpec[uniq (body != val.body || listCou != val.listCou) ]);  rename ;  setmodel(new value place in body);  add_arr_i(add elelments of array from <value> key, only missing keys)  (conc ower) (conc stay)")]
        public static readonly string operation = "operation";

        [info("")]
        public static readonly string operationSpec = "operationSpec";

        [info("support function execution to evaluate before assignment ")]
        public static readonly string value = "value";

        [info("Optionally can contain multilevel path to branch")]
        [model("template")]
        public static readonly string partition = "partition";
      
        [info("")]
        [model("FlagModelSpec")]
        public static readonly string do_not_value_Unwrap = "do_not_value_Unwrap";

        [model("FlagModelSpec")]
        [info("do not run any models from [value]  ")]
        public static readonly string do_not_exec_value = "do_not_exec_value";

        [model("spec_tag")]
        [info("clear value partition kind and all its subitems ")]
        public static readonly string clear_func = "clear_func";

        public override void Process(opis message)
        {
            opis locOpis = modelSpec;
            long integerVal = 0; 

            string oper = modelSpec.V(operation);
            string opSpec = locOpis.V(operationSpec); 

            long lv = 0;

            #region what to process

            opis val = locOpis[value].Duplicate();

            bool unwr = !locOpis.OptionActive(do_not_value_Unwrap);
            if (unwr)
                val = val.W();

            if (!locOpis.OptionActive(do_not_exec_value))
            {
                // obsolete functionality, remove if use fresh context 
                if (string.IsNullOrEmpty(val.PartitionKind) && val.listCou > 0)
                    instanse.ExecActionModelsList(val);

                instanse.ExecActionModel(val, val);
            }
            modelSpec = locOpis;

            if (unwr)
                val = val.W();

            if (modelSpec.isHere(clear_func, false))
            {
                val.PartitionKind = "";
                for (int i = 0; i < val.listCou; i++)
                    val[i].PartitionKind = "";
            }

            opis processThis = message;
            opis templ = modelSpec.getPartitionNotInitOrigName(partition)?.Duplicate();
            if (templ != null)
            {
                if (templ.PartitionKind != "template")
                    instanse.ExecActionModel(templ, templ);
                else if (templ.listCou > 0)
                    instanse.ExecActionModelsList(templ);

                if (templ.listCou > 0)
                    processThis = opis.GetLevelByTemplate(templ[0], processThis, true);
            }

            #endregion

            switch (oper)
            {
              
                case "rename":
                    opis ft = locOpis[operationSpec].Duplicate();
                    instanse.ExecActionModel(ft, ft);
                    processThis.PartitionName = ft.body;
                    break;

                case "setmodel":
                    opis pv = locOpis[value].Duplicate();
                    instanse.ExecActionModel(pv, pv);
                    processThis.PartitionKind = pv.body;
                    break;            

                case "conc ower":
                    for (int i = 0; i < val.listCou; i++)
                        processThis[val[i].PartitionName] = val[i];
                    break;
                 
                case "conc stay":
                    processThis.AddArrMissing(val);
                    break;

                case "dec":
                    lv = StrUtils.LongFromString(processThis.body);
                    integerVal = StrUtils.LongFromString(val.body);
                    processThis.body = (lv - integerVal > 0 ? lv - integerVal : 0).ToString();
                    break;

                case "inc":
                    lv = StrUtils.LongFromString(processThis.body);
                    integerVal = StrUtils.LongFromString(val.body);
                    processThis.body = (lv + (integerVal > 0 ? integerVal : 1)).ToString();
                    break;
                  

                case "add_arr_i":
                    integerVal = opSpec == "all" ? 1 : (opSpec == "new" ? 2 : 0);
                    for (int i = 0; i < val.listCou; i++)
                    {
                        if (integerVal == 1)
                        {
                            processThis.AddArr(val[i]);
                        }
                        else
                        {
                            if (processThis.getPartitionIdx(val[i].PartitionName) == -1)
                            {
                                processThis.AddArr(val[i]);
                            }
                            else
                                if (!(integerVal == 2))
                            {
                                if (processThis[val[i].PartitionName].body != val[i].body
                                     || processThis[val[i].PartitionName].listCou != val[i].listCou)
                                    processThis.AddArr(val[i]);
                            }
                        }
                    }
                    break;             

                case "add_arr":
                    if (!(opSpec == "uniq"))
                    {
                        processThis.AddArr(val);
                    }
                    else
                    {
                        if (processThis.getPartitionIdx(val.PartitionName) == -1
                                || processThis[val.PartitionName].body != val.body
                                     || processThis[val.PartitionName].listCou != val.listCou)
                            processThis.AddArr(val);
                    }
                    break;
                  
                case "set":
                    if (opSpec == "w")
                    {
                        processThis.Wrap(val.W());
                    }
                    else
                    {
                        processThis.body = val.body;
                        processThis.CopyArr(val);
                    }
                    break;
                   
                case "set b":
                    processThis.body = val.body;
                                                                                                                                                                  
                    break;
            }
        }

    }
}
