using basicClasses.models.DataSets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace basicClasses.models.SharedDataContextDrivers
{
    [info("Base function to work with global variables and object reflection.  Automatically unwrap name [ rez = rez.W(); ] ")]
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

        [info("")]
        [model("spec_tag")]
        public static readonly string debug = "debug";

        public override void Process(opis message)
        {
            opis source = null;

            opis currSpec = modelSpec;

            int pnIdx = modelSpec.getPartitionIdx(sdc_item);

            if (pnIdx != -1)
            {
                opis svc = sharedVal;
                opis p = modelSpec[pnIdx].Duplicate();
                instanse.ExecActionModel(p, p);

                int pos = svc.getPartitionIdx(p.body);
                source = pos != -1 ? svc[pos] : null;

                if (source == null && currSpec[create].isInitlze)
                    source = svc[p.body];
            }
            else

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

                if (source.PartitionKind == "null")
                {
                    source = null;
                }
            }
            else

            if (modelSpec.getPartitionIdx(thisins_data) != -1)
                source = thisins;
            else
            if (modelSpec.getPartitionIdx(spec_data) != -1)
                source = spec;

            modelSpec = currSpec;

            if (source != null)
            {
                opis rez = source;

                opis templ = modelSpec.getPartitionNotInitOrigName(template)?.Duplicate();
                if (templ != null)
                {                  
                    instanse.ExecActionModel(templ, templ);

                    if (templ.listCou > 0)
                    {                        
                        rez = GetLevelCheck(templ[0], source.W());
                    }
                }


                modelSpec = currSpec;

                if (rez != null)
                {
                    // in case modelSpec and message referencing the same data
                  //  opis procSpec = modelSpec[process].Duplicate();
                    opis procSpec = modelSpec.getPartitionNotInitOrigName(process)?.Duplicate();

                    rez = rez.W();

                    if (modelSpec[duplicate].isInitlze)
                        rez = rez.Duplicate();


                    //if (message.PartitionKind != "answer" && !modelSpec[do_not_modify].isInitlze)
                    if (!modelSpec[do_not_modify].isInitlze)
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

                    if (procSpec != null)
                        instanse.ExecActionResponceModelsList(procSpec, rez);
                }
                else
                {
                    message.body = "";
                    message.CopyArr(new opis());
                }

            }
            else
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

                if (modelSpec[templateUnwrap].isInitlze && rez.PartitionKind == "wrapper")
                    rez = rez.W();

                bool haveSubitems = templ.listCou > 0;
                bool foundSubitems = false;
                for (int i = 0; i < templ.listCou; i++)
                {
                    opis tmp = GetLevelCheck(templ[i], rez);
                    if (tmp != null && tmp.isInitlze)
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
