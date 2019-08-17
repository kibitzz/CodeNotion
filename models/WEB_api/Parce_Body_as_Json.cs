using System;


namespace basicClasses.models.WEB_api
{
    [info("filler")]
    [appliable("Action exe all")]


    class Parce_Body_as_Json : ModelBase
    {

        [info("parce json as opis structure")]
        [model("spec_tag")]
        public static readonly string opis_json = "opis_json";

        public override void Process(opis message)
        {
            opis trtrt = new opis();

            try
            {

                if (modelSpec.isHere(opis_json))
                {
                    trtrt.load(message.body);
                }else
                    trtrt.JsonParce(message.body);
               
            }
            catch(Exception e)
            {
                trtrt["error parce"].body = e.Message;
            }

            message.body = "";
            if (modelSpec.isHere(opis_json))           
                message.body = trtrt.body;
            
            //  message.CopyArr(trtrt);
            message.CopyArr(new opis());
            message.AddArr(trtrt);

        }
    }
}
