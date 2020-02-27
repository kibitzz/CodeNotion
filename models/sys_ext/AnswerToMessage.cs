using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace basicClasses.models.sys_ext
{
    [appliable("Action exec")]
   public class AnswerToMessage:ModelBase
    {
        [model("FillerList")]
        [info("тут список філлерів, котрі заповняться перед відповіддю")]
        public static readonly string answer = "answer";

        [model("")]
        [info("в випадку пересланого повідомлення можна явно вказати на яке повідомлення відповідати (адресувати відправнику)")]
        public static readonly string message_to_answer = "message_to_answer";

        [model("FlagModelSpec")]
        [info(" ")]
        public static readonly string resendToUpper = "resendToUpper";


        public override void Process(opis message)
        {
            opis ans = modelSpec[answer].Duplicate();
            instanse.ExecActionModelsList(ans);

            if (modelSpec[resendToUpper].isInitlze)
            {
                if (modelSpec[message_to_answer].isInitlze)
                {
                    opis msg = modelSpec[message_to_answer].Duplicate();
                    instanse.ExecActionModel(msg, msg);
                    msg = (msg.PartitionKind == "wrapper") ? msg.W() : msg;

                    instanse.ResendToUpper(msg, true);
                }
                else
                    instanse.ResendToUpper();
            }
            else
            {
                if (modelSpec[message_to_answer].isInitlze)
                {
                     opis msg = modelSpec[message_to_answer].DuplicateAs<opisEventsSubscription>();
                  // opis msg = modelSpec[message_to_answer].Duplicate();
                    instanse.ExecActionModel(msg, msg);
                    msg =  msg.W() ;
                    instanse.ModelAnswerMsg(msg, ans);
                }
                else
                    instanse.ModelAnswerMsg(ans);
            }
        }
    }
}
