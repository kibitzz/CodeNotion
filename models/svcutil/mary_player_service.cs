using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace basicClasses.models.svcutil
{
    [appliable("")]
    [info("  ")]
    public class mary_player_service : ModelBase
    {

        public override void Process(opis message)
        {
            opis ms = SpecLocalRunAll();


            CustomizationsWilliamHillPlayerServiceClient client = new CustomizationsWilliamHillPlayerServiceClient();

            string errorMsg = "";

            string apiver = ms.V("apiver");
            string btmid = ms.V("btmid");
            string request_id = ms.V("request_id");
            string request_system_sub_type = ms.V("request_system_sub_type");
            string request_system_type = ms.V("request_system_type");
            string requester_id = ms.V("requester_id");
           // string requester_type = ms.V("apiver");
           
           

            Dictionary<string, string> addinfo = new Dictionary<string, string>();

            try
            {

                var rez = client.RegulationSelfExclusionImport(
                   ref apiver,
                   ref btmid,
                   ms.V("initiating account id"),
                   ms.V("initiating application"),
                   ms.V("log policy"),
                   ms.V("originator system id"),
                    ref request_id,
                    ref request_system_sub_type,
                    ref request_system_type,
                    requester_id,
                    (char)83, // requester_type
                    0, //sub_system_id 
                    false,
                    ref addinfo,
                    523,
                    null,
                    null,
                    null,
                    null,
                    ms.V("regulation type"),
                    true,
                    out errorMsg


                    );

            }
            catch (Exception e)
            {
                errorMsg = e.Message;
            }
            finally
            {
                client.Close();
            }
            // Use the 'client' variable to call operations on the service.

            // Always close the client.
            

            message.body = errorMsg;

        }

       
    }
}
