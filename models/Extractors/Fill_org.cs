using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace basicClasses.models.Extractors
{

    [info("заповняє дане поле значенням CTX.organizer;")]
    [appliable("MsgTemplate creation FillerList")]
   public class Fill_org:ModelBase
    {
        public override void Process(opis message)
        {
            message.body = CTX.organizer;
            //*  m.Vset("пко*/", thisins.W(ModelG.spec).isInitlze ? thisins.V(ModelG.spec) : name);
        }
    }

    [info("заповняє дане поле значенням  CTX.Handle(CTX.GoUp());  body = CTX.organizer; ")]
    [appliable("MsgTemplate creation FillerList")]
    public class Fill_org_upper : ModelBase
    {
        public override void Process(opis message)
        {
            //CTX.Handle(CTX.GoUp());          
            //message.body = CTX.organizer;

            message.body = CTX.GoUp().V(context.Organizer);

        }
    }
}
