using basicClasses.models.DataSets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace basicClasses.models.StructureProcessing
{

    [appliable("Array_of_RangingList")]
    [info("")]
    public class RangingList : ModelBase
    {
        [model("spec_tag")]
        [info("do not require all checks in list return OK to set range points to argument")]
        public static readonly string only_range = "only_range";
    }


    [appliable("Action ParamList exe all")]
    [info("")]
    public class RangeAndAssign:ModelBase
    {
        [model("")]
        [info("масив обєктів котрі треба проранжувати")]
        public static readonly string arguments = "arguments";

        [model("Array_of_RangingList")]
        [info("набори шаблонів даних(і деревовидних), котрі дають бали відповідності. також імена цих наборів назначаться обєктам що ім відповідають. якщо в RangingList міститься поле only range то обєкту не обовязково проходити всі перевірки з списку")]
        public static readonly string range_criterias = "range_criterias";

        [model("spec_tag")]
        [info("помітка того, що в range_criterias кожен елемент це набір різноманітних моделей ранжування, а не шаблон для CheckConformity")]
        public static readonly string range_func = "range_func";

        [model("sdc role")]
        [info("form array which contain all arguments that pass criterias.  BODY is sdc_role of created array")]
        public static readonly string form_array_of_positively_ranged = "form_array_of_positively_ranged";


        public override void Process(opis message)
        {
            opis args = modelSpec[arguments].Duplicate();
            opis crit = modelSpec[range_criterias].Duplicate();
            opis arrPassRole = modelSpec[form_array_of_positively_ranged].Duplicate();

            instanse.ExecActionModelsList(crit);
            instanse.ExecActionModel(crit, crit);

            instanse.ExecActionModelsList(args);
            instanse.ExecActionModel(args, args);

            instanse.ExecActionModel(arrPassRole, arrPassRole);

            opis arrPass = null;
            if (arrPassRole.isInitlze)
            {
                arrPass = new opis();
                SharedContextRoles.SetRole(arrPass, arrPassRole.body, sharedVal);
            }


            //logopis["args"].Wrap( args);
            //logopis["crit"] = crit;
           
            //instanse.ExecParamModels(curr);

            opis ranging = new opis();
            ranging.PartitionName = "ranging array";
            //logopis.AddArr(ranging);
            bool do_range = modelSpec.isHere(range_func);

            for (int c = 0; c < crit.listCou; c++)
            {
                //instanse.ExecParamModels(crit[c]);
                instanse.ExecActionModelsList(crit[c]);
                for (int i = 0; i < args.listCou; i++)
                {
                    if (do_range)
                    {
                        int vvv = RangeByModel(crit[c], args[i].W());
                        ranging[args[i].PartitionName + "_" + i.ToString()][crit[c].PartitionName].intVal = vvv;

                        if (arrPass != null && vvv>0)
                        {
                            arrPass.AddArr(args[i].W());
                        }
                    }
                    else
                    ranging[args[i].PartitionName +"_"+i.ToString()][crit[c].PartitionName].intVal = args[i].CheckConformity(args[i], crit[c]);
                }
            }

            opis ranges = ranging;

            thisins["Models_log"].AddArr(ranges);

            for (int c = 0; c < crit.listCou; c++)
            {

                int minDiff = -1;
                int bestmin = -1;
                uint average = 0;

                for (int i = 0; i < ranges.listCou; i++)
                {
                    int minDiffLoc = 10000000;
                    int currDiff = 0;
                    int thisval = ranges[i][crit[c].PartitionName].intVal;
                    for (int k = 0; k < ranges[i].listCou; k++)
                    {
                        if (ranges[i][k].PartitionName != crit[c].PartitionName)
                        {
                            currDiff = thisval - ranges[i][k].intVal;
                            average+= (uint)(ranges[i][k].intVal > 0 ? 3 : 0);
                        }
                        else
                            currDiff = 10000000;

                        if (currDiff < minDiffLoc)
                        {
                            minDiffLoc = currDiff;
                        }
                    }
                    average += (uint)(thisval > 0 ? 3 : 0);

                    // якщо мінімальна перевага даного параметру над іншими позитивна, і найбільна з наявних
                    // то даний параметр береться як найпідходящий
                    if (minDiffLoc >= 0 && minDiffLoc > minDiff)
                    {
                        minDiff = minDiffLoc;
                        bestmin = i;
                    }

                }

                // average > 0 критерій того, що є позитивно ранжовані обєкти на це місце
                // якщо всі нулі, тоді взагалі не встановлюємо значення
                if (bestmin >= 0 && average > 0)
                {
                    char[] numb = new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };

                    string parameterId = crit[c].PartitionName;
                    int puy = parameterId.IndexOfAny(numb);
                    if (puy != 0)
                    {
                        opis featest = args[bestmin].W();
                        SharedContextRoles.SetRole(featest, parameterId, sharedVal);
                    }
                }
                else
                {
                    //logopis.Vset(crit[c].PartitionName+"  значення не встановлено", "average " + average.ToString());
                }
            }
         
        }

        int RangeByModel(opis crit, opis arg)
        {

            int rez = 0;
            opis mmm = new opis();
            mmm.PartitionName = "range arg";
            mmm.WrapByName(arg, "arg");

            instanse.ExecActionResponceModelsList(crit, mmm);
         
            if(mmm["passCou"].intVal == crit.listCou || crit.isHere(RangingList.only_range))          
             rez = mmm["range"].intVal;

            return rez;
        }

    }
}
