﻿using basicClasses.models.DataSets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace basicClasses.models.SharedDataContextDrivers
{
    [info("Base function to work with local variables and object reflection.  Automatically unwrap name [ rez = rez.W(); ] ")]
    [appliable("all")]
   public class GetAnyPartOfOpis:ModelBase
    {

        [info("variable from SharedDataContext   source = sharedVal[modelSpec[sdc_item].body];")]
        public static readonly string sdc_item = "sdc_item";
       
        [info("thisins reflection (metalanguage base object)   ")]
        public static readonly string thisins_data = "thisins_data";

        [info("spec reflection (inherited object info)  ")]
        public static readonly string spec_data = "spec_data";

        [info("separator")]
        public static readonly string l_____________ = "l_____________";

        [info("variable name to put in SharedDataContext   name should be placed as value [.body]")]
        [model("")]
        public static readonly string sdc_Role = "sdc_Role";

        [info("create object with such role or sdc_item if not exist")]
        [model("FlagModelSpec")]
        public static readonly string create = "create";

        [info("template of branch to access some partition of the source. if not required - delete or leave empty")]
        [model("template")]
        public static readonly string template = "template";

        [info("actions to do with original item")]
        [model("Action")]
        public static readonly string process = "process";

        [info("set body of this partition if you want processed opis to be duplicated(by default its references copied) into [message] parameter of  Process(opis message) ")]
        [model("FlagModelSpec")]
        public static readonly string duplicate = "duplicate";

        [info("set body of this partition if you want  not make copy  message.body = rez.body; message.CopyArr(rez);  BUT   message.Wrap(rez); ")]
        [model("FlagModelSpec")]
        public static readonly string do_wrap = "do_wrap";

        [info("set body of this partition if you want    message.PartitionName = rez.body;")]
        [model("FlagModelSpec")]
        public static readonly string body_as_PartitionName = "body_as_PartitionName";

        [info("when retrieve partition on [template]  unwrap each node   rez = rez.W();")]
        [model("FlagModelSpec")]
        public static readonly string templateUnwrap = "templateUnwrap";
        

        [info("set body of this partition if original data should be unchanged ")]
        [model("FlagModelSpec")]
        public static readonly string do_not_modify = "do_not_modify";

        public override void Process(opis message)
        {
            opis source = null;

            opis currSpec = modelSpec;

            if (modelSpec.isHere(sdc_item))
            {
                opis p = modelSpec[sdc_item].Duplicate();
                instanse.ExecActionModel(p, p);                
                source = sharedVal.isHere(p.body)? sharedVal[p.body] : null;

                if (currSpec[create].isInitlze)
                    source = sharedVal[p.body];
            }

            if (modelSpec.isHere(sdc_Role))
            {
                opis p = modelSpec[sdc_Role].Duplicate();
                instanse.ExecActionModel(p, p);

                source = SharedContextRoles.GetRole(p.body, sharedVal);
                if (source.PartitionKind == "null" && currSpec[create].isInitlze)
                {
                    source = new opis();
                    SharedContextRoles.SetRole(source, p.body, sharedVal);
                }

                if (source.PartitionKind == "null" )
                {
                    source =null;                   
                }
            }

            if (modelSpec.getPartitionIdx(thisins_data) != -1)
                source = thisins;

            if (modelSpec.getPartitionIdx(spec_data) != -1)
                source = spec;

            modelSpec = currSpec;

            if ( source!=null)
            {
                opis rez = source;

                if (modelSpec.isHere(template))
                {
                    opis ptt = modelSpec[template].Duplicate();
                    if (ptt.PartitionKind != template)
                        instanse.ExecActionModel(ptt, ptt);

                    if (ptt.listCou > 0)
                    {
                        instanse.ExecActionModelsList(ptt);
                        modelSpec = currSpec;
                        rez = GetLevelCheck(ptt[0], source.W());
                    }
                }
               

                modelSpec = currSpec;

                if (rez != null)
                {
                    // на випадок коли modelSpec і message це одна ссилка
                    opis procSpec = modelSpec[process].Duplicate();
                   
                    rez = rez.W();                 

                    if (modelSpec[duplicate].isInitlze)
                        rez = rez.Duplicate();

                   
                    // якщо в якості параметру для заповнення передається якийсь обєкт
                    // наприклад дана модель в списку validate повідомлення, то ми не змінюємо його
                    if (message.PartitionKind != "answer" && !modelSpec[do_not_modify].isInitlze )
                    {
                        if (modelSpec[do_wrap].isInitlze)
                        {
                            message.CopyArr(new opis());
                            message.Wrap(rez.W());
                        }
                        else
                        {
                            if (modelSpec[body_as_PartitionName].isInitlze)
                                message.PartitionName = rez.body;
                            message.body = rez.body;
                            message.CopyArr(rez);
                        }

                        if (message.PartitionKind == name)
                        {
                            message.PartitionKind += "_done";
                        }
                    }

                    instanse.ExecActionResponceModelsList(procSpec, rez);
                }
            } else
            {
                if (message.PartitionName == "value")
                    message.CopyArr(new opis());
            }                

        }


        opis GetLevelCheck(opis templ, opis srs)
        {
            opis rez = null;
            if (srs.getPartitionIdx(templ.PartitionName) != -1)
            {
                rez = srs[templ.PartitionName];

                if (  modelSpec[templateUnwrap].isInitlze && rez.PartitionKind == "wrapper")
                    rez = rez.W();

                bool haveSubitems = templ.listCou >0;
                bool foundSubitems = false;
                for (int i = 0; i < templ.listCou; i++)
                {
                   opis tmp = GetLevelCheck(templ[i], rez);
                    if (tmp!=null && tmp.isInitlze)
                    {
                        foundSubitems = true;
                        rez = tmp;
                        break;
                    }
                }

                if (haveSubitems && !foundSubitems)
                    rez = null;
            }

            return rez;

        }
    }
}
