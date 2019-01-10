using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace basicClasses.models.SharedDataContextDrivers
{
   public class ConditionResponceModel: ModelBase
    {
        [model("Action")]
        public static readonly string yess = "yess";

        [model("Action")]
        public static readonly string no = "no";

    }
}
