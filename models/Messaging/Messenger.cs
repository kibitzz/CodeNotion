using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace basicClasses.models.Actions
{
    [info("Use mechanism of messaging between objects in context. Compose and send message")]
    [appliable("Action ")]
   public class Messenger:ModelBase
    {
        [info("template of message to send")]
        [model("MsgTemplate")]
        public static readonly string Send = "Send";

        [info("list of preprocessors of message before sending, all of them receive <Send> structure as parameter ")]
        [model("Action")]
        public static readonly string Features = "Features";

        public override void Process(opis message)
        {
            if (modelSpec.getPartitionIdx(Send) != -1)
            {
                SendMsg(modelSpec[Send]);
            }
        }

        public void SendMsg(opis msg)
        {
            if (!string.IsNullOrWhiteSpace(msg.V(MsgTemplate.msg)))
            {
                opis newmsg = msg.Duplicate();

                thisins["Models_log"]["instance_msg_aggr"] = (new opis("fill message params", "body ___"));

                instanse.ExecParamModels(newmsg[MsgTemplate.p]);
                instanse.ExecParamModels(newmsg);
                //instanse.ExecParamModels(newmsg[MsgTemplate.p]);

                instanse.ExecActionResponceModelsList(modelSpec[Features], newmsg);

                thisins["Models_log"]["instance_msg_aggr"].PartitionName = "msg_composion " + newmsg.V(MsgTemplate.msg);
             
                instanse.ThisRequest(newmsg.V(MsgTemplate.msg_receiver), newmsg.V(MsgTemplate.msg), newmsg);
            }
        }
    }
}
