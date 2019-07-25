using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace basicClasses.models.sys_ext
{
    [info(" source.serialize()")]
    [appliable("Action ")]
    public  class save_toFile: ModelBase
    {
        [model("")]
        [info("будь який філлер кортий заповнить масив елементів для формування даних")]
        public static readonly string source = "source";

        [model("")]
        [info("вкажіть імя файлу")]
        public static readonly string file = "file";

        [model("spec_tag")]
        [info("")]
        public static readonly string only_body = "only_body";

        public override void Process(opis message)
        {
            opis surc = message;
            if (modelSpec.isHere(source))
            {
                 surc = modelSpec[source].Duplicate();
                instanse.ExecActionModel(surc, surc);
            }

            opis f = modelSpec[file].Duplicate();
            instanse.ExecActionModel(f, f);


            DataFileUtils.savefile(modelSpec.isHere(only_body)? surc.body : surc.serialize(), f.body);
        }
    }
}
