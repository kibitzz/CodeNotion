using basicClasses.models.DataSets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace basicClasses.models.ContextOperators
{
    [info("put parameter of Process() to sharedVal ")]
    [appliable("")]
   public class MoveToSharedVarInstCont:ModelBase
    {
        [model("spec_tag")]
        [info("do not run  instanse.ExecActionModelsList(message); ")]
        public static readonly string do_not_exec = "do_not_exec";
      
        [model("")]
        [info("назначає роль даному опису, і потім мати до нього доступ, пригодиться коли пусте або не знаєш яке PartitionName ")]
        public static readonly string set_role = "set_role";

        public override void Process(opis message)
        {
            if (modelSpec.getPartitionIdx(do_not_exec) == -1)
                instanse.ExecActionModelsList(message);

            opis rol = modelSpec[set_role].Duplicate();
            instanse.ExecActionModel(rol, rol);

            if (modelSpec.getPartitionIdx(set_role) != -1)
                SharedContextRoles.SetRole(message, rol.body, sharedVal);
            else
                sharedVal.AddArr(message);
          
        }
    }


    [info("obsolete ")]
    [appliable("")]
    public class messageTo_Role : ModelBase
    {             
        public override void Process(opis message)
        {           
            if (!string.IsNullOrEmpty(modelSpec.body))
                SharedContextRoles.SetRole(message, modelSpec.body, sharedVal);           
        }
    }

}
