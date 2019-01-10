using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace basicClasses.models.sys_ext
{
    [appliable("Action ")]
    public class TagArrayItems : ModelBase
    {

        [model("")]
        [info("будь який філлер кортий заповнить масив елементів . якщо видалено з специфікації, береться message")]
        public static readonly string source = "source";

        [model("template")]
        [info("")]
        public static readonly string partition = "partition";

        [model("")]
        [info("")]
        public static readonly string partName = "partName";

        [model("")]
        [info("")]
        public static readonly string value = "value";

        public override void Process(opis message)
        {
            opis surc = message;
            opis ex = modelSpec.Duplicate();
            instanse.ExecActionModelsList(ex);

            if (ex.isHere(source))
            {
                surc = ex[source];            
            }

            opis ptt = modelSpec[partition].Duplicate();
            instanse.ExecActionModelsList(ptt);

            string pn = ex.V(partName);
            string vl = ex.V(value);

            for (int i = 0; i < surc.listCou; i++)
            {
                opis processThis = opis.GetLevelByTemplate(ptt[0], surc[i], true);
                if (processThis != null)
                {
                    processThis.Vset(pn, vl);
                }
            }

        }
    }
}
