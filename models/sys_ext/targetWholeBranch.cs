﻿using basicClasses.models.OptionsLists;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace basicClasses.models.sys_ext
{
    [info("специфічна поведінка моделей, їм потрібно передавати штучний опис з необхідними даними")]
    public class TargetingChecks : ModelBase
    {

    }

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

    [appliable("TargetingChecks")]
    public class targetID : ModelBase
    {
        public override void Process(opis message)
        {

            bool pass = (modelSpec.V("ID") == o.V("ID") );
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
