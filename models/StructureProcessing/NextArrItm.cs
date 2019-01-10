using basicClasses.models.DataSets;
using basicClasses.models.SharedDataContextDrivers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace basicClasses.models.StructureProcessing
{
    [appliable("Action ParamList exe ")]
    public class NextArrItm:ModelBase
    {
        [model("")]
        [info("роль обєкта котрий зберігає позицію")]
        public static readonly string Iterator = "Iterator";

        [model("")]
        [info("")]
        public static readonly string source = "source";

        [model("spec_tag")]
        [info("if all elements was processed - start from first again")]
        public static readonly string loop = "loop";

        [model("")]
        [info("роль елемента котрий обробляється")]
        public static readonly string role = "role";

        [info(" при певному результаті отримав/неотримав елемент виконує різний набір моделей")]
        [model("ConditionResponceModel")]
        public static readonly string responce = "responce";

        public override void Process(opis message)
        {
            opis surc = message;
           

           opis iter = SharedContextRoles.GetRole(modelSpec[Iterator].body, sharedVal);
            if (iter.PartitionKind == "null" || !iter.isInitlze)
            {
                opis ex = modelSpec.Duplicate();
                instanse.ExecActionModelsList(ex);

                if (ex.isHere(source))
                {
                    surc = ex[source];
                }

                iter = new opis("iterator");
                iter[source] = surc;
                iter["pos"].body ="0";

                SharedContextRoles.SetRole(iter, modelSpec[Iterator].body, sharedVal);
            }

            if(modelSpec.isHere(loop) && iter[source].listCou == iter["pos"].intVal)
                iter["pos"].body = "0";

            opis itm = null;
            if (iter[source].listCou > iter["pos"].intVal)
            {
                itm = iter[source][iter["pos"].intVal];
                iter["pos"].intVal++;
                SharedContextRoles.SetRole(itm, modelSpec.V(role), sharedVal);


            }else
            SharedContextRoles.SetRole(new opis(), modelSpec.V(role), sharedVal);


            if (itm != null)
                instanse.ExecActionResponceModelsList(modelSpec[responce][ConditionResponceModel.yess], itm);
            else
                instanse.ExecActionResponceModelsList(modelSpec[responce][ConditionResponceModel.no], new opis());


        }

    }
}
