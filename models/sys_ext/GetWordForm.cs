using basicClasses.models.DataSets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace basicClasses.models.sys_ext
{
    [info("automatically set founded opis to role  [NotionWord].   and fill message with word opis")]
    [appliable("Action exec exe")]
    public class GetWordForm : ModelBase
    {
        [model("")]
        [info("what to search")]
        public static readonly string word = "word";

        [model("")]
        [info("role to set  [NotionWord] is default")]
        public static readonly string role = "role";

        [model("FlagModelSpec")]
        [info("set any to body, and new instance will be created(functional separated to object)")]
        public static readonly string Create_instance = "Create_instance";

        public override void Process(opis message)
        {
            opis args = modelSpec[word].Duplicate();              
            instanse.ExecActionModel(args, args);

            //logopis.WrapByName(args, "word");

            if (args.isInitlze)
            {
                if (modelSpec[Create_instance].isInitlze)
                {
                    instanse.CreateInstance(args.body);
                }


                opis t = instanse.GetWordForm(args.body).Duplicate();
            
                SharedContextRoles.SetRole(t, modelSpec.isHere(role) ? modelSpec[role].body : "NotionWord", sharedVal);
                message.body = t.body;
                message.CopyArr(t);            
            }
        }
    }
}
