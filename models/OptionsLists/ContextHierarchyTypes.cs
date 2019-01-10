using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace basicClasses.models.OptionsLists
{
   public class ContextHierarchyTypes:ModelBase
    {

        [info("")]      
        public static readonly string SubTree = "SubTree";

        [info("")]
        public static readonly string UpperTree = "UpperTree";

        //[info("")]
        public static readonly string SameCont = "SameCont";

        [info("")]
        public static readonly string ThisBranch = "ThisBranch";

        [info("")]
        public static readonly string NextBranch = "NextBranch";

        [info("")]
        public static readonly string PrevBranch = "PrevBranch";

    }
}
