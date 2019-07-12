using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace basicClasses.models
{
    [info("base structure of message ")]
    [appliable("creation ModelNotion modelReq ModelListOfMessages ")]
    class MsgTemplate: ModelInfo
    {
        [ignore]
        public static readonly string MsgTemplate_lit = "MsgTemplate";

        public static readonly string msg = "msg";
     
        public static readonly string msg_receiver = "msg_receiver";

        [model("TargetingChecks")]
        [info("list of models that make judgment which context is targeted")]
        public static readonly string contTargetModel = "contTargetModel";

        [info("base parameter name, it analyses in all types of messaging")]
        public static readonly string p = "p";
       

        [info("")]
        public static readonly string Hint = "Hint";

        [ignore]
        [info("for inject messages")]
        public static readonly string sender_tags = "sender_tags";


        [model("Fill_current_context")]
        [info("context to be handled in receiver instance")]
        public static readonly string context = "context";

        [ignore]
        [info("cpecial flag to prevent resending message to higher context") ]
        public static readonly string no_transfer = "no_transfer";

        [model("Action")]
        [info("additional code(inject) to run in receiver if context is matched, also there could be an method that handle such msg, this will extend it.  this models exec AFTER spec responces ")]       
        public static readonly string getAnswerDetails = "getAnswerDetails";

        [model("Action")]
        [info("additional code(inject) to run BEFORE internal responces exec ")]
        public static readonly string preProcess = "preProcess";


        [model("Action")]
        [info("array of actions that will be done (by the instance that received answer) if answer received")]
        public static readonly string responce = "responce";

        [model("Action")]
        [info("array of actions that will check validity of messge and can decide to cancel: resend, answer. модель має доступ до опису відповіді та параметрів повідомлення(inject)")]
        public static readonly string validate = "validate";

        [ignore]
        [info("")]
        public static readonly string cancel = "cancel";
              


    }
}
