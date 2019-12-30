using basicClasses.models.DataSets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace basicClasses.models.StructureProcessing
{
    [appliable("initValues all BodyValueModificator template FillerList MsgTemplate")]
    [info("fill message with blueprint specified by shared context(or cpecified in values_container object) items body values.   $some[]$some - will be replaced by body value of <some> .  some_new_part_name[]*some - will copy array of some to partition.   some_new_part_name[]@some - wrap some object to defined partition.  #some_part[] - replaced by some_part object or its duplication.  &b  &p  for getting context object body and partname ")]
    public class buildTreeVal_sdc_i:ModelBase
    {
        [ignore]
        Random rnd = new Random();

        [model("")]
        [info("# in PartitionName means that whole value tree must be assigned.   $ means that body value must be placed instead $template")]
        public static readonly string blueprint = "blueprint";

        [model("")]
        [info("заповни це поле любим філлером, і він буде використовуватись як джерело значень для шаблону.  Якщо потрібно використовувати sharedVal масив - видали це поле з специфікації. якщо використовувати контейнер за замовчуванням(valuesContainer) тоді залиши поле з незаповненим body")]
        public static readonly string values_container = "values_container";

        [model("spec_tag")]
        [info("якщо в шаблоні вказано # - копіювання всієї гілки та для даних котрі потрібні в оригіналі (параметри ссилки на обєкти) можна відключити дуплікацію")]
        public static readonly string do_not_duplicate_branch = "do_not_duplicate_branch";

        [model("spec_tag")]
        [info("якщо в шаблоні вказано # - копіювання всієї гілки та нам не потрібна ссилка на обєкт(його масив може змінюватися і буде побічний ефект) ми копіюємо тільки масив")]
        public static readonly string if_not_duplicate_only_Copy_array = "if_not_duplicate_only_Copy_array";


        [model("spec_tag")]
        [info("")]
        public static readonly string only_value_body = "only_value_body";

        [model("spec_tag")]
        [info("заповнювати при bp.PartitionKind == buildTreeVal_sdc_i")]
        public static readonly string recurce_all = "recurce_all";

        [model("spec_tag")]
        [info("bug fix when ")]
        public static readonly string putWholeResult = "putWholeResult";


        public override void Process(opis message)
        {
            opis bp = modelSpec[blueprint].Duplicate();
            var locmod = modelSpec;
            instanse.ExecActionModel(bp, bp);
            modelSpec = locmod;

            opis vk = sharedVal;

            if (bp.listCou > 0)
            {
                if (modelSpec.isHere(values_container))
                {
                    opis t = null;
                    if (modelSpec[values_container].isInitlze
                        || !string.IsNullOrEmpty(modelSpec[values_container].PartitionKind))
                    {
                        t = modelSpec[values_container].Duplicate();

                        locmod = modelSpec;
                        instanse.ExecActionModel(t, t);
                        modelSpec = locmod;
                    }
                    else
                        t = SharedContextRoles.GetRole("valuesContainer", sharedVal);

                    vk = t.W();
                }

                build(bp, null, vk);
              
              
                if (bp.listCou == 1 && message.PartitionKind != "template")
                {
                    if (locmod.isHere(putWholeResult))
                    {
                        message.PartitionKind = "";
                        message.CopyArr(bp);
                        return;
                    }

                    if (message.PartitionName == "value")
                    {                      
                        if (modelSpec.isHere(only_value_body))
                        {
                            message.CopyArr(new opis());
                            message.body = bp[0].body;
                        }
                        else
                            message.Wrap(bp[0]);
                    }
                    else
                    {
                        message.body = bp[0].body;
                        message.PartitionName = string.IsNullOrEmpty(bp[0].PartitionName) ? message.PartitionName : bp[0].PartitionName;
                        message.PartitionKind = bp[0].PartitionKind;
                        if (modelSpec.isHere(only_value_body))
                            message.CopyArr(new opis());
                        else
                            message.CopyArr(bp[0]);
                    }
                }
                else
                {                  
                    message.PartitionKind = "";
                    message.CopyArr(bp);
                }

               
            }

        }

        public void build(opis bp, opis holder, opis valContainer)
        {
            bool donotrecurce = !modelSpec.isHere(recurce_all) && bp.PartitionKind == "buildTreeVal_sdc_i";
          
            if (bp.body.StartsWith("$"))           
                bp.body = valContainer[bp.body.Replace("$", "")].body;

            if (bp.PartitionName.StartsWith("$"))
                bp.PartitionName = valContainer[bp.PartitionName.Replace("$", "")].body;

            if (bp.body.StartsWith("&b"))
                bp.body = valContainer.body;
            if (bp.body.StartsWith("&p"))
                bp.body = valContainer.PartitionName;

            if (bp.PartitionName.StartsWith("&b"))
                bp.PartitionName = valContainer.body;
            if (bp.PartitionName.StartsWith("&p"))
                bp.PartitionName = valContainer.PartitionName;

            if (bp.PartitionKind =="&b")
                bp.PartitionKind = valContainer.body;
            if (bp.PartitionKind == "&p")
                bp.PartitionKind = valContainer.PartitionName;

            if (bp.PartitionKind.StartsWith("$"))
                bp.PartitionKind = valContainer[bp.PartitionKind.Replace("$", "")].body;

            if (bp.body.StartsWith("*"))
            {
                donotrecurce = true;

                if (bp.body.Length > 1)
                    bp.CopyArr(valContainer[bp.body.Replace("*", "")].W());
                else
                    bp.CopyArr(valContainer);

                bp.body = "";
            }

            if (bp.body.StartsWith("+"))
            {
                donotrecurce = true;

                if (bp.body.Length > 1)
                    bp.AddArrRange(valContainer[bp.body.Replace("+", "")].W());
                else
                    bp.AddArrRange(valContainer);

                bp.body = "";
            }


            if (bp.body.StartsWith("@"))
            {
                donotrecurce = true;

                if (bp.body.Length > 1)
                    bp.Wrap(valContainer[bp.body.Replace("@", "")].W());
                else
                    bp.Wrap(valContainer.W());
            }

            if (bp.body.StartsWith("%"))
            {
                bp.body = "random body "+ rnd.Next().ToString() + rnd.Next().ToString(); ;
            }

            // # in PartitionName means that whole tree must be assigned
            if (bp.PartitionName.StartsWith("#"))
            {
                donotrecurce = true;

                opis branchVal = valContainer[bp.PartitionName.Replace("#", "")].W();
                if (!modelSpec.isHere(do_not_duplicate_branch) 
                    || bp.PartitionName.StartsWith("##"))
                {
                    branchVal = branchVal.Duplicate();

                    bp.PartitionName = branchVal.PartitionName;
                    bp.body = branchVal.body;
                    if(string.IsNullOrEmpty(bp.PartitionKind))
                    bp.PartitionKind = branchVal.PartitionKind;
                    bp.CopyArr(branchVal);
                }
                else
                {
                    if (holder != null)
                    {
                        if (modelSpec.isHere(if_not_duplicate_only_Copy_array))
                        {
                            bp.PartitionName = branchVal.PartitionName;
                            bp.body = branchVal.body;
                            if (string.IsNullOrEmpty(bp.PartitionKind))
                                bp.PartitionKind = branchVal.PartitionKind;
                            bp.CopyArr(branchVal);
                        }
                        else
                        {
                            bp.PartitionName = branchVal.PartitionName;
                            holder[bp.PartitionName] = branchVal;

                            if (!string.IsNullOrEmpty(bp.PartitionKind))
                                branchVal.PartitionKind = bp.PartitionKind;
                            //holder.RemoveArrElem(bp);
                            //holder.AddArr(branchVal);
                        }
                    }
                    //bp.Wrap(branchVal);
                }
            }

            bp.body = bp.body.Replace("{star}", "*");
            //if (bp.PartitionName.StartsWith("$"))           
            //    bp.PartitionName = sharedVal[bp.PartitionName.Replace("$", "")].body;

            if (!donotrecurce)
            for (int i = 0; i < bp.listCou; i++)
                build(bp[i], bp, valContainer);

        }
    }
}
