using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace basicClasses.models.OptionsLists
{
    [info("")]
    public  class TagsTypes:ModelBase
    {
        [info(" ")]
        public static readonly string branch_idx = "branch_idx";

        [info("ID of root context for current branch ")]
        public static readonly string msg_branch = "msg_branch";

        [info(" term that owns context of root element ")]
        public static readonly string branch_notion_cont = "branch_notion_cont";

     
        public override void Process(opis message)
        {
            if (message.listCou > 0)
            {
                message.body = message[0].PartitionName;
            }
        }

    }
}
