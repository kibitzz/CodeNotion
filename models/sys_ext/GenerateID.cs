using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace basicClasses.models.sys_ext
{
    [appliable("Action")]
    [info("filler ")]
  public  class GenerateID: ModelBase
    {
        public override void Process(opis message)
        {
            Random r = new Random();
            message.body += DateTime.Now.Ticks.ToString()+ r.Next().ToString();
        }
    }

    [appliable("Action MsgTemplate creation FillerList ")]
    [info("filler ")]
    public class PutTimestamp : ModelBase
    {
        public override void Process(opis message)
        {           
            message.body += DateTime.Now.Ticks.ToString();
            message["readable"].body = DateTime.Now.ToString();
        }
    }
}
