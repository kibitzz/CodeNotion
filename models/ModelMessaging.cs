using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace basicClasses.models
{
    [info("")]
    [appliable("creation ModelNotion")]
    public  class Messaging:ModelInfo
    {
        [model("ModelListOfMessages")]
        public static readonly string someContextName1 = "someContextName1";
     

        [model("modelReq")]
        public static readonly string someResponceContextName1 = "msg1";

       
    }

    [info("список повідомлень MsgTemplate")]
    [appliable("creation Messaging")]
    public class ModelListOfMessages : ModelInfo
    {
        [model("MsgTemplate")]
        public static readonly string msg1 = "msg1";

        [model("MsgTemplate")]
        public static readonly string msg2 = "msg2";

        
    }


}
