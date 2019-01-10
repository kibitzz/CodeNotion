using basicClasses.models.DataSets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace basicClasses.models.StructureProcessing
{
    [appliable("Action ")]
    [info("fill context role(valuesContainer) object with blueprint specified by data from source item values")]

   public class TreeDataExtractor:ModelBase
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

        [model("spec_tag")]
        [info(" ")]
        public static readonly string new_template_type = "new_template_type";


        opis roleObject;
        
        
        public override void Process(opis message)
        {
            roleObject = new opis("valuesContainer");
            opis locModSp = modelSpec.Duplicate();
            opis locShVal = sharedVal;
            instanse.ExecActionModelsList(locModSp);

            string rolename = locModSp[containerRole].isInitlze? locModSp.V(containerRole) : "valuesContainer";

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
            //logopis.WrapByName(srs.Duplicate(), "srs_debug");

            modelSpec = locModSp;

            if(modelSpec.isHere(new_template_type))
                buildCool(bp, srs);
            else
            build(bp, srs);

            //logopis["srs"].Wrap( srs);
            //logopis.WrapByName(roleObject, "roleObject");
        }

        public void build(opis template, opis partition)
        {

            for (int i = 0; i < template.listCou; i++)
            {
                string pn = template[i].PartitionName.Replace("$", "").Replace("#", "");
                if (template[i].PartitionName == "^")
                    pn = partition.body;
               

                if (partition.getPartitionIdx(pn) != -1 )
                {

                    // якщо потрібно додати значення PartitionName в контейнер
                    // перед PartitionName ставить $ а в PartitionKind вказати імя по котрому буде доступне значення
                    if (template[i].PartitionName.StartsWith("$"))
                    {
                        //roleObject[template[i].PartitionKind].body = pn;
                        //if (string.IsNullOrEmpty(template[i].body))
                        //{
                            roleObject.Vset(pn, partition.V(pn));
                        //}
                    }

                    // все дерево копіюється
                    if (template[i].PartitionName.StartsWith("#"))
                    {
                        if (modelSpec.isHere(do_not_duplicate_branch))
                            roleObject[pn].Wrap(partition[pn].W());
                        else
                            roleObject[pn] = partition[pn].Duplicate();                 
                    }

                    if (template[i].body.StartsWith("$"))
                    {
                        roleObject[template[i].body.Replace("$", "")].body = partition[pn].body;
                    }


                    if (template[i].listCou > 0)
                    {
                         build(template[i], partition[pn]);                    
                    }
                }
            }
      
        }


        string getChemaVal(string chema, string part)
        {

           string rez = "";
            int pos = chema.IndexOf(part);
            int posend = chema.IndexOfAny(new char[]{ '?','#','*', '$'}, pos+1);

            if(pos >=0)
            {
                rez = chema.Substring(pos + 1, (posend>0 ? posend -pos: chema.Length - pos));
            }

            return rez;
        }

        public void buildCool(opis template, opis partition)
        {

            for (int i = 0; i < template.listCou; i++)
            {
                opis srch = new opis();
                srch.body = getChemaVal(template[i].body, "?");
                srch.PartitionName = getChemaVal(template[i].PartitionName, "?");
                srch.PartitionKind = getChemaVal(template[i].PartitionKind, "?");

                opis found = new opis();

                partition.FindTreePartitions(srch, "", found, false);

                if(found.listCou>0)
                {


                }

                string pn = template[i].PartitionName.Replace("$", "").Replace("#", "");
                if (template[i].PartitionName == "^")
                    pn = partition.body;
                if (template[i].PartitionName == "@")
                    pn = partition.body;

                if (partition.getPartitionIdx(pn) != -1)
                {

                    // якщо потрібно додати значення PartitionName в контейнер
                    // перед PartitionName ставить $ а в PartitionKind вказати імя по котрому буде доступне значення
                    if (template[i].PartitionName.StartsWith("$"))
                    {
                        //roleObject[template[i].PartitionKind].body = pn;
                        //if (string.IsNullOrEmpty(template[i].body))
                        //{
                        roleObject.Vset(pn, partition.V(pn));
                        //}
                    }

                    // все дерево копіюється
                    if (template[i].PartitionName.StartsWith("#"))
                    {
                        if (modelSpec.isHere(do_not_duplicate_branch))
                            roleObject[pn].Wrap(partition[pn].W());
                        else
                            roleObject[pn] = partition[pn].Duplicate();
                    }

                    if (template[i].body.StartsWith("$"))
                    {
                        roleObject[template[i].body.Replace("$", "")].body = partition[pn].body;
                    }


                    if (template[i].listCou > 0)
                    {
                        buildCool(template[i], partition[pn]);
                    }
                }
            }

        }


    }

}
