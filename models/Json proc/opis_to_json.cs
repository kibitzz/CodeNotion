using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace basicClasses.models.WEB_api
{  
    [info("filler")]
    [appliable("Action exe all")]
    class opis_to_json : ModelBase
    {


        public override void Process(opis message)
        {
                      
            message.body = message.ToJson();
                     
            message.CopyArr(new opis());           

        }
    }

}
