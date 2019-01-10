using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace basicClasses.models.StructureProcessing
{
   public class join:ModelBase
    {
        [model("")]
        [info("")]
        public static readonly string table = "table";

        [model("spec_tag")]
        [info("")]
        public static readonly string dataJoin = "dataJoin";

        [model("")]
        [info("left right full")]
        public static readonly string join_type = "join_type";

        [model("")]
        [info("")]
        public static readonly string joinTemplate_Left = "joinTemplate_Left";

        [model("")]
        [info("")]
        public static readonly string joinTemplate_Right = "joinTemplate_Right";

        public override void Process(opis message)
        {
           
        }
    }
}
