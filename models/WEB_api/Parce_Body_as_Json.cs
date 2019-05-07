using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using twite;

namespace basicClasses.models.WEB_api
{
    [info("filler")]
    [appliable("Action exe all")]


    class Parce_Body_as_Json : ModelBase
    {

        public override void Process(opis message)
        {
            opis trtrt = new opis();

            try
            {

                JsonObject jrez = JsonParser.Parse(message.body);

                jrez.BuildTreeopis(trtrt);
            }
            catch(Exception e)
            {
                trtrt["error parce"].body = e.Message;
            }

            message.body = "";
            message.CopyArr(trtrt);
        }
    }
}
