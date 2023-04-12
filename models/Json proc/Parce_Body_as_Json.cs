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

        [info("if not only values contain cyrillic text u0410, but also names")]
        [model("spec_tag")]
        public static readonly string decode_names = "decode_names";

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

            if (modelSpec.isHere(decode_names))
                trtrt.RunRecursively(x=> x.PartitionName = TemplatesMan.UTF8BigEndian_to_Kirill(x.PartitionName));
                


            message.CopyArr(new opis());
            message.AddArr(trtrt);

        }
    }

}
