using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace basicClasses.models.WEB_api
{
    class UseSecurityProtocol : ModelBase
    {
        [info("")]
        [model("spec_tag")]
        public static readonly string use_Tls11 = "use_Tls11";

        [info("")]
        [model("spec_tag")]
        public static readonly string use_Tls12 = "use_Tls12";

        [info("")]
        [model("spec_tag")]
        public static readonly string use_Tls = "use_Tls";

        [info("")]
        [model("spec_tag")]
        public static readonly string use_Ssl3 = "use_Ssl3";

        public override void Process(opis message)
        {
            if(modelSpec.isHere(use_Tls12))
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            if (modelSpec.isHere(use_Tls11))
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls11;

            if (modelSpec.isHere(use_Tls))
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls;

            if (modelSpec.isHere(use_Ssl3))
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3;
        }
    }

}
