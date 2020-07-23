using basicClasses.models.OptionsLists;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace basicClasses.models.sys_ext
{
    [info("")]
    public class TargetingChecks : ModelBase
    {

    }

    [info("  context.Higher not just any of the top     ")]
    [appliable("TargetingChecks")]
    public class targetParentCont : ModelBase
    {
        public override void Process(opis message)
        {

            bool pass = message["original_msg_context"].W(context.Higher).V("ID") == o.V("ID");

            if (pass)
                message["run_on_this_context"].body = "yepp";
        }

    }

    [appliable("TargetingChecks")]
   public class targetWholeBranch:ModelBase
    {
        public override void Process(opis message)
        {
            bool pass = thisins[TagsTypes.branch_idx].body ==
                message[MsgTemplate.sender_tags][TagsTypes.branch_idx].body;

            if (pass)
                message["run_on_this_context"].body = "yepp";
        }

    }

    [info("CheckParentOrder  -- anyone who is placed upper in tree   ")]
    [appliable("TargetingChecks")]
    public class targetUpper : ModelBase
    {
        public override void Process(opis message)
        {

          bool pass = (message["original_msg_context"].V("ID") == o.V("ID") ||
                   CTX.CheckParentOrder(o, message["original_msg_context"]));
            if (pass)
                message["run_on_this_context"].body = "yepp";
        }

    }

    [info("targer concrete context ID, one or more.   pass = (modelSpec.V(ID) == o.V(ID) )       ")]
    [appliable("TargetingChecks")]
    public class targetID : ModelBase
    {
        [info(" array if filler used. body value if structure is composed ")]       
        public static readonly string ID = "ID";

        public override void Process(opis message)
        {
            bool pass = (modelSpec.V("ID") == o.V("ID") && !string.IsNullOrEmpty(modelSpec.V("ID")) );           

            var ids = modelSpec["ID"].Duplicate();
            instanse.ExecActionModel(ids, ids);

            for (int i = 0; i < ids.listCou; i++)
                pass = pass ? pass : ids[i].body == o.V("ID");

            if (pass)
                message["run_on_this_context"].body = "yepp";
        }
    }

    [appliable("TargetingChecks")]
    public class targetSubcontextsOfItems : ModelBase
    {
        public override void Process(opis message)
        {

            bool pass = (message["original_msg_context"].V("ID") == o.W(context.Higher).V("ID"));
            if (pass)
                message["run_on_this_context"].body = "yepp";
        }

    }

    [info("pass if ID is equal (same context too)")]
    [appliable("TargetingChecks")]
    public class targetSubTree : ModelBase
    {
        public override void Process(opis message)
        {

           bool pass = (message["original_msg_context"].V("ID") == o.V("ID") ||
                      CTX.CheckParentOrder(message["original_msg_context"], o));

            if (pass)
                message["run_on_this_context"].body = "yepp";
        }

    }

    [info(" do not pass if ID is equal, ONLY subcontexts")]
    [appliable("TargetingChecks")]
    public class targetSubTreeOnly : ModelBase
    {
        public override void Process(opis message)
        {
            //  !string.IsNullOrEmpty(message["original_msg_context"].V("ID"));
            //  is wery important, because it remove situations when sentence context stub is empty
            //  then ewery context accept inject msg, this is wrong 

            bool pass = (CTX.CheckParentOrder(message["original_msg_context"], o)) 
                && !string.IsNullOrEmpty( message["original_msg_context"].V("ID"));

            if (pass)
                message["run_on_this_context"].body = "yepp";
        }

    }

    [appliable("TargetingChecks")]
    public class targetSameCont : ModelBase
    {
        public override void Process(opis message)
        {

            bool pass = message["original_msg_context"].V("ID") == o.V("ID");

            if (pass)
                message["run_on_this_context"].body = "yepp";
        }

    }

    [appliable("TargetingChecks")]
    public class targetAnyCont : ModelBase
    {
        public override void Process(opis message)
        {

            //bool pass = message["original_msg_context"].V("ID") == o.V("ID");

            //if (pass)
                message["run_on_this_context"].body = "yepp";
        }

    }
}
