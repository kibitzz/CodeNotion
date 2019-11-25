using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace basicClasses.models.WEB_api
{
    [info("fill currently active settings")]
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

        [info("")]
        [model("spec_tag")]
        public static readonly string use_mix = "use_mix";

        [info("")]
        [model("spec_tag")]
        public static readonly string ServerCertificateValidationCallback = "ServerCertificateValidationCallback";

        [ignore]
        static bool callbIsSet;

        public bool AcceptAllCertificate;

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

            if (modelSpec.isHere(ServerCertificateValidationCallback) && !callbIsSet)
            {
                //ServicePointManager.ServerCertificateValidationCallback += AcceptAllCertificatePolicy;
                callbIsSet = true;
                AcceptAllCertificate = true;
            }

            message.Vset("AcceptAllCertificate", AcceptAllCertificate ? "true" : "false");

            if (modelSpec.isHere(use_mix))
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
        }
     

       //просто делегат возращающий всегда true
        public static bool AcceptAllCertificatePolicy(object sender, X509Certificate certificate, X509Chain chain, System.Net.Security.SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }
    }

}
