﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace basicClasses.models.String_proc
{
    [info("filler")]
    [appliable("func BodyValueModificator")]
    class StrReplace : ModelBase
    {
        [info("")]
        [model("")]
        public static readonly string to_replace = "to_replace";

        [info("")]
        [model("")]
        public static readonly string by_this = "by_this";

        public override void Process(opis message)
        {
            opis spec = modelSpec.Duplicate();
            instanse.ExecActionModelsList(spec);
                   
            message.body = message.body.Replace(spec.V(to_replace), spec.V(by_this));          
        }
    }
}