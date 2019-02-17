using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace basicClasses.models
{
    [info("Set of standart messages")]
    [appliable("Messaging")]
    public class modelReq:ModelInfo
    {

        [model("Action")]
        [info("System message, send by framework after <GenerateTags> is finished, actual work is starting here")]
        public const string start = "start";



        [model("Action")]
        [info("System message, send by framework after <Build> is finished, and before <Start> message")]
        public static readonly string GenerateTags = "GenerateTags";

        [model("Action")]
        [info("")]
        public const string Hook = "Hook";

        [model("Action")]
        [info("")]
        public const string Aspect_before = "Aspect_before";

        [model("Action")]
        [info("")]
        public const string Aspect_after = "Aspect_after";

        [model("Action")]
        [info("")]
        public const string Aspect_Msg_type_Before_send = "Aspect_Msg_type_Before_send";

        [model("Action")]
        [info("")]
        public const string Ready = "Ready";

        [model("Action")]
        [info("")]
        public const string Search_Param = "Search_Param";

        [model("Action")]
        [info("")]
        public const string Before_Param_Search = "Before_Param_Search";

        [model("Action")]
        [info("")]
        public const string Enumerate = "Enumerate";

        [model("Action")]
        [info("")]
        public const string Immune_Deactivate = "Immune_Deactivate";

        [model("Action")]
        [info("")]
        public const string Deactivate = "Deactivate";

        [model("Action")]
        [info("")]
        public const string Deact_Method = "Deact_Method";

        [model("Action")]
        [info("")]
        public const string Deact_Msg_type_send = "Deact_Msg_type_send";




    }
}
