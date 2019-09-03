﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace basicClasses.models.SharedDataContextDrivers
{
    [appliable("GetAnyPartOfOpis Action all")]
    [info("All operations with datastructure, modifying passed parameter (implicit) ")]
   public class BodyValueModificator:ModelBase
    {

        [info("inc  dec  set (set b - only body)  add_arr   rename   setmodel(new value place in body)  add_arr_i(add elelments of array from <value> key, only missing keys)  (conc ower) (conc stay)")]
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
            long integerVal = StrUtils.LongFromString( modelSpec[value].body);
            string oper = modelSpec.V(operation);

            opis ft = locOpis[operationSpec].Duplicate();
            instanse.ExecActionModel(ft, ft);
            string opSpec = ft.body ;

            long lv = 0;

            switch (oper)
            {
              
                case "rename":                                   
                    message.PartitionName = opSpec ;
                    break;

                case "setmodel":                  
                    message.PartitionKind = modelSpec[value].body;
                    break;            

                case "conc ower":
                case "conc stay":
                case "dec":
                case "inc":
                case "add_arr_i":
                case "add_arr":
                case "set":
                case "set b":
                    opis ttt = locOpis[value].Duplicate();
                    if (ttt.PartitionKind == "wrapper" && !locOpis[do_not_value_Unwrap].isInitlze)
                        ttt = ttt.W();

                    opis csp = locOpis;

                    if (!locOpis[do_not_exec_value].isInitlze)
                    {
                        if(string.IsNullOrEmpty(ttt.PartitionKind))
                        instanse.ExecActionModelsList(ttt);

                        instanse.ExecActionModel(ttt, ttt);
                    }
                    modelSpec= csp;

                    if (ttt.PartitionKind == "wrapper" && !modelSpec[do_not_value_Unwrap].isInitlze)
                        ttt = ttt.W();

                    if (modelSpec.isHere(clear_func))
                    {
                        ttt.PartitionKind = "";
                        for (int i = 0; i < ttt.listCou; i++)
                            ttt[i].PartitionKind = "";
                    }


                    if (modelSpec[partition].isInitlze || modelSpec[partition].PartitionKind != "template")
                    {
                        opis ptt = modelSpec[partition].Duplicate();
                       // instanse.ExecActionModelsList(ptt);

                        bool processMessage = false;
                        if (ptt.PartitionKind != "template")
                        {
                            instanse.ExecActionModel(ptt, ptt);
                            if (ptt.listCou == 0)
                            {
                                ptt.AddArr(new opis("", "body jkjk"));
                                processMessage = true;
                            }
                        }else
                            instanse.ExecActionModelsList(ptt);



                        opis processThis=  message;
                         processThis = processMessage? processThis:  opis.GetLevelByTemplate(ptt[0], message, true);

                        if (processThis != null)
                        {

                            if (oper == "conc ower")
                            {
                                for (int i = 0; i < ttt.listCou; i++)                                
                                    processThis[ttt[i].PartitionName] = (ttt[i]);                                                                   
                            }

                            if (oper == "conc stay")
                            {                               
                                  processThis.AddArrMissing(ttt);
                            }
                            

                            if (oper == "add_arr_i")
                            {
                                for (int i = 0; i < ttt.listCou; i++)
                                {
                                    if (opSpec == "all")
                                    {
                                        processThis.AddArr(ttt[i]);
                                    }
                                    else
                                    {
                                        if (processThis.getPartitionIdx(ttt[i].PartitionName) == -1)
                                        {
                                            processThis.AddArr(ttt[i]);
                                        }
                                        else
                                            if (!(opSpec == "new"))
                                        {
                                            if (processThis[ttt[i].PartitionName].body != ttt[i].body
                                                 || processThis[ttt[i].PartitionName].listCou != ttt[i].listCou)
                                                processThis.AddArr(ttt[i]);
                                        }
                                    }
                                   
                                }
                            }
                            
                            if (oper == "add_arr")
                            {
                                if (!(opSpec == "uniq"))
                                {
                                    processThis.AddArr(ttt);
                                } else
                                {
                                    if (processThis.getPartitionIdx(ttt.PartitionName) == -1 
                                            || processThis[ttt.PartitionName].body != ttt.body
                                                 || processThis[ttt.PartitionName].listCou != ttt.listCou)
                                        processThis.AddArr(ttt);
                                }
                            }
                            if (oper == "set")
                            {
                                if (opSpec == "w")
                                {
                                    processThis.Wrap(ttt.W());
                                }
                                else
                                {
                                    processThis.body = ttt.body;
                                    processThis.CopyArr(ttt);
                                }
                            }
                            if (oper == "set b")
                            {
                                processThis.body = ttt.body;                               
                            }

                            if (oper == "inc")
                            {
                                lv = StrUtils.LongFromString(processThis.body);
                                integerVal = StrUtils.LongFromString(ttt.body); 
                                processThis.body = (lv+ (integerVal > 0 ? integerVal : 1)).ToString();
                            }

                            if (oper == "dec")
                            {
                                lv = StrUtils.LongFromString(processThis.body);
                                integerVal = StrUtils.LongFromString(ttt.body);
                                processThis.body = (lv - integerVal > 0 ? lv - integerVal : 0).ToString();
                            }

                        }
                    }
                    else
                    {
                        if (oper == "conc ower")
                        {
                            for (int i = 0; i < ttt.listCou; i++)
                                message[ttt[i].PartitionName] = ttt[i];
                        }

                        if (oper == "conc stay")
                        {
                            message.AddArrMissing(ttt);
                        }


                        if (oper == "add_arr_i")
                        {
                            for (int i = 0; i < ttt.listCou; i++)
                            {
                                if (opSpec == "all")
                                {
                                    message.AddArr(ttt[i]);
                                }
                                else
                                {
                                    if (message.getPartitionIdx(ttt[i].PartitionName) == -1)
                                    {
                                        message.AddArr(ttt[i]);
                                    }
                                    else                                  

                                    if (!(opSpec == "new"))
                                    {
                                        if (message[ttt[i].PartitionName].body != ttt[i].body
                                             || message[ttt[i].PartitionName].listCou != ttt[i].listCou)
                                            message.AddArr(ttt[i]);
                                    }
                                }
                                                              
                            }
                        }
                       
                       if (oper == "add_arr")
                        {
                          //  message.AddArr(ttt);

                            if (!(opSpec == "uniq"))
                            {
                                message.AddArr(ttt);
                            }
                            else
                            {
                                if (message.getPartitionIdx(ttt.PartitionName) == -1
                                        || message[ttt.PartitionName].body != ttt.body
                                             || message[ttt.PartitionName].listCou != ttt.listCou)
                                    message.AddArr(ttt);
                            }

                        }
                        if (oper == "set")
                        {
                            if (opSpec == "w")
                            {
                                message.Wrap(ttt.W());
                            }
                            else
                            {
                                message.body = ttt.body;
                                message.CopyArr(ttt);
                            }                          
                        }

                        if (oper == "set b")
                        {
                            message.body = ttt.body;
                        }

                        if (oper == "inc")
                        {
                            lv = StrUtils.LongFromString(message.body);
                            integerVal = StrUtils.LongFromString(ttt.body);
                            message.body = (lv +( integerVal > 0 ? integerVal : 1)).ToString();
                            //message.intVal += integerVal > 0 ? integerVal : 1;
                        }

                        if (oper == "dec")
                        {
                            lv = StrUtils.LongFromString(message.body);
                            integerVal = StrUtils.LongFromString(ttt.body);
                            message.body = (lv - integerVal > 0 ? lv - integerVal : 0).ToString();
                        }
                    }

                    break;
            }
        }

    }
}
