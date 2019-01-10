using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace basicClasses.models
{
    [info("Set of standart messages")]
    [appliable("creation Messaging  ModelNotion")]
    public class modelReq:ModelInfo
    {
      
        [model("Action")]
        [info("System message, send by framework after <GenerateTags> is finished, actual work is starting here")]
        public const string start = "start";

        [model("Action")]
        public const string inject = "inject";

      
        [model("Action")]
        [info("System message, send by framework after <Build> is finished, and before <Start> message")]
        public static readonly string GenerateTags = "GenerateTags";

      

    }
}
