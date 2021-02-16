using basicClasses.models.DataSets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace basicClasses.models.StructureProcessing
{
    [info("реалізовує цикл вказаної послідовності моделей, для кожного елемента опису котрий передається в  Process(opis message).  заповнює ролі ProcessIndex та procItem")]
    [appliable("Action ParamList exe ")]
    public class ForEach : ModelBase
    {

        [model("Action")]
        [info("послідовності моделей котра буде тілом циклу")]
        public static readonly string sequence = "sequence";

        [model("")]
        [info("роль елемента котрий обробляється")]
        public static readonly string role = "role";

        [model("")]
        [info("any filler, object that contain array of items to iterate by")]
        public static readonly string source = "source";

        [model("FlagModelSpec")]
        [info("кожен елемент буде передаватись моделям як параметр для обробки    process(message)")]
        public static readonly string models_process_item = "models_process_item";

        [model("FlagModelSpec")]
        [info("перебір йтиме з останнього елементу до першого")]
        public static readonly string move_backward = "move_backward";


        public override void Process(opis message)
        {
            opis locSeq = modelSpec[sequence].Duplicate();
            opis locModel = modelSpec;

            message = message.PartitionKind == "wrapper" ? message.W() : message;


            opis srs = message;
            if (locModel[source].isInitlze || !string.IsNullOrEmpty(locModel[source].PartitionKind))
            {
                srs = locModel[source].Duplicate();
                instanse.ExecActionModel(srs, srs);
            }

            opis idx = new opis();
            SharedContextRoles.SetRole(idx, SharedContextRoles.ProcessIndex, sharedVal);
         
            string rl = SharedContextRoles.procItem;
            if (locModel[role].isInitlze)
                rl = locModel[role].body;

            bool backward = locModel.OptionActive(move_backward);
            bool procItm = locModel.OptionActive(models_process_item);

            for (int i = 0; i < srs.listCou; i++)
            {
                int pos = i;
                if (backward)
                    pos = srs.listCou -i -1;            

                if (procItm)
                    instanse.ExecActionResponceModelsList(locSeq.Duplicate(), srs[pos].W());
                else
                {
                    SharedContextRoles.GetRole(SharedContextRoles.ProcessIndex, sharedVal).intVal = pos;
                    SharedContextRoles.SetRole(srs[pos].W(), rl, sharedVal);

                    instanse.ExecActionModelsList(locSeq.Duplicate());
                }

                // повертаємо інстанс в поточний контекст
                instanse.Handle(o); //на випадок коли інстанс може отримувати повідомлення
                // чи відповіді в процесі виконання попереднього циклу
                // отримавши повідомлення змінюється поточний контекст інстанса
                // і через це наступний цикл не має доступу до SDC - виникає збій
        
            }

        }
    }
}
