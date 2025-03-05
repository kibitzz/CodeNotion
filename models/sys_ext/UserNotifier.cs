using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace basicClasses.models.sys_ext
{      
    [info("show notification banner (exactly where you see this text)")]
    [appliable("Action func")]
    public class UserNotifier : ModelBase
    {

        [model("")]
        [info("text to show in notification banner")]
        public static readonly string message = "message";

        public override void Process(opis message)
        {
            opis ms = SpecLocalRunAll();

            if (ms["message"].isInitlze)
            {
                SysInstance.messageBannertext = ms["message"].body;
                instanse.updateGui();
            }          
        }
    }
   
}
