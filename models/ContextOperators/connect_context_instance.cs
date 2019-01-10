using basicClasses.models.DataSets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace basicClasses.models.ContextOperators
{
    [appliable("MoveToSharedVarInstCont ")]
   public class connect_context_instance:ModelBase
    {
        public override void Process(opis message)
        {
            string reqWrapper = message.PartitionName;
            string instName = message.body;

            message.PartitionKind = MsgTemplate.MsgTemplate_lit;

            message.Vset(MsgTemplate.msg, reqWrapper);
            message.Vset(MsgTemplate.msg_receiver, instName);
            message.Vset("context_notion", instanse.name);

            opis item=  CTX.GetItemName(instName, true);

            message[ModelAnswer.context] = CTX.GetItemContext(item);

        }

    }


    [appliable("exe ")]
    [info("obsolete")]
    public class share_context_instance : ModelBase
    {
        [model("")]
        [info("name of notion from context")]
        public static readonly string instname = "instname";

        [model("")]
        [info("set role with such name")]
        public static readonly string role = "role";

        public override void Process(opis message)
        {
                 
            opis item = CTX.GetItemName(modelSpec.V(instname), true);

            SharedContextRoles.SetRole(item, modelSpec.V(role), sharedVal);

        }

    }

}
