using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using basicClasses.models.DataSets;

namespace basicClasses.models.StructureProcessing
{
 

    [appliable("Action func")]
    [info("fill context role(valuesContainer) object with blueprint specified by data from source item values")]
    public class Destructure : ModelBase
    {
        [model("")]
        [info("")]
        public static readonly string blueprint = "blueprint";

        [model("")]
        [info("обєкт з котрого будуть взяті значення (якщо поле видалено, джерелом буде message ), будь який філлер   instanse.ExecActionModel(modelSpec[source], modelSpec[source]);")]
        public static readonly string source = "source";

        [model("")]
        [info("role will be assigned to container, if not cpecified - default role name [valuesContainer]     \n  rolename = modelSpec.V(containerRole)")]
        public static readonly string containerRole = "containerRole";

        [model("spec_tag")]
        [info("додає дані в існуючий набір(роль), а не створює чистий і заповнює з даного джерела.  З цим специфікатором можна формувати набори з кількох джерел")]
        public static readonly string appendContainer = "appendContainer";

        [model("spec_tag")]
        [info("якщо в шаблоні вказано # - копіювання всієї гілки та для даних котрі потрібні в оригіналі (параметри ссилки на обєкти) можна відключити дуплікацію")]
        public static readonly string do_not_duplicate_branch = "do_not_duplicate_branch";

        [model("spec_tag")]
        [info(" ")]
        public static readonly string do_not_add_metadata = "do_not_add_metadata";

       

        opis roleObject;


        public override void Process(opis message)
        {
            roleObject = new opis("valuesContainer");
            opis locModSp = modelSpec.Duplicate();
            opis locShVal = sharedVal;
            instanse.ExecActionModelsList(locModSp);

            string rolename = locModSp[containerRole].isInitlze ? locModSp.V(containerRole) : "valuesContainer";

            if (SharedContextRoles.GetRole(rolename, locShVal).PartitionKind != "null"
                && locModSp.getPartitionIdx(appendContainer) != -1)
                roleObject = SharedContextRoles.GetRole(rolename, locShVal);

            SharedContextRoles.SetRole(roleObject, rolename, locShVal);

            opis srs = message;
            if (locModSp.getPartitionIdx(source) != -1)
            {
                srs = locModSp[source];
                //instanse.ExecActionModel(srs, srs);              
            }

            srs = srs.W();

            opis bp = locModSp[blueprint];

            if (!locModSp.isHere(do_not_add_metadata))
            {
                roleObject.Vset("list_cou", srs.listCou.ToString());
                roleObject.Vset("name", srs.PartitionName);
                roleObject.Vset("val", srs.body);
            }
          

            modelSpec = locModSp;

           
                buildCool(bp, srs);
          
        }

        string getChemaVal(string chema, string part, string defval)
        {

            string rez = defval;
            int pos = chema.IndexOf(part);
            int posend = chema.IndexOfAny(new char[] { '?', '#', '*', '$', '@', '^', '=' }, pos + 1);

            if (pos >= 0)
            {
                rez = chema.Substring(pos + 1, (posend > 0 ? posend - pos : chema.Length - pos));
            }

            return rez.Trim();
        }

        public void buildCool(opis template, opis partition)
        {

            for (int i = 0; i < template.listCou; i++)
            {
                opis tt = template[i];
                opis srch = new opis();

                srch.body = getChemaVal(tt.body, "?", "");
                srch.PartitionName = getChemaVal(tt.PartitionName, "?", "");
                srch.PartitionKind = getChemaVal(tt.PartitionKind, "?", "");

                opis found = new opis();

                partition.FindTreePartitions(srch, "", found, false);

                if (found.listCou > 0)
                {
                    opis fop = found[0][0];

                    // = operate duplicated object                
                    if (tt.PartitionName.Contains("="))
                    {
                        fop = fop.Duplicate();
                    }

                    // ^ operate unwrapped object                
                    if (tt.PartitionName.Contains("^"))
                    {
                        fop = fop.W();
                    }

                    // * fill array
                    if (tt.PartitionName.Contains("*"))
                    {
                        string apn = getChemaVal(tt.PartitionName, "*", srch.PartitionName);
                        opis itm = new opis();
                        itm.PartitionName = apn;
                        itm.CopyArr(fop);
                        roleObject.AddArr(itm);
                    }

                    // @ wrap partition                    
                    if (tt.PartitionName.Contains("@"))
                    {
                        string apn = getChemaVal(tt.PartitionName, "@", srch.PartitionName);
                        opis itm = new opis();
                        itm.PartitionName = apn;
                        itm.Wrap(fop);
                        roleObject.AddArr(itm);
                    }

                    // # put as is                   
                    if (tt.PartitionName.Contains("#"))
                    {
                        roleObject.AddArr(fop);
                    }

                    // $ put body value as (usable in body )                   
                    if (tt.body.Contains("$"))
                    {
                        string apn = getChemaVal(tt.body, "$", srch.PartitionName);
                        opis itm = new opis();
                        itm.PartitionName = apn;
                        itm.body = fop.body;
                        roleObject.AddArr(itm);
                    }

                    //recurce
                    if (tt.listCou > 0)
                    {
                        buildCool(tt, fop);
                    }

                }

              
            }
        }

        


    }


}
