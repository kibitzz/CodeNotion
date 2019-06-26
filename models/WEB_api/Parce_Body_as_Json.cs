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

        //[info("")]
        //[model("spec_tag")]
        //public static readonly string use_new_parcer = "use_new_parcer";

        public override void Process(opis message)
        {
            opis trtrt = new opis();

            try
            {
                //if (!modelSpec.isHere(use_new_parcer))
                //{
                //    JsonObject jrez = JsonParser.Parse(message.body);

                //    jrez.BuildTreeopis(trtrt);
                //}
                //else
                //{
                    trtrt.JsonParce(message.body);
                //}
            }
            catch(Exception e)
            {
                trtrt["error parce"].body = e.Message;
            }

            message.body = "";
            //  message.CopyArr(trtrt);
            message.CopyArr(new opis());
            message.AddArr(trtrt);

        }
    }
}
