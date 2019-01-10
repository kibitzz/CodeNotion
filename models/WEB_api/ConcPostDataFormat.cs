using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace basicClasses.models.WEB_api
{
    [appliable("MsgTemplate webActorModel creation FillerList BodyValueModificator")]
    [info("(Filler)  compile opis to required format of srting data, and set it to message body  [ message.body = data;]")]
   public class ConcPostDataFormat:ModelBase
    {

        [model("")]
        [info("будь який філлер кортий заповнить масив елементів для формування даних")]
        public static readonly string source = "source";

        [model("spec_tag")]
        [info("")]
        public static readonly string Make_Json = "Make_Json";

        [model("spec_tag")]
        [info("")]
        public static readonly string Make_Cookie = "Make_Cookie";

        [model("spec_tag")]
        [info("")]
        public static readonly string Concatenate = "Concatenate";

        [model("spec_tag")]
        [info("appliable in combination with Concatenate")]
        public static readonly string Trim_Element_Quotes = "Trim_Element_Quotes";

        //[model("spec_tag")]
        //[info("")]
        //public static readonly string DoNotRunSourceList = "DoNotRunSourceList";

        [model("FlagModelSpec")]
        [info("")]
        public static readonly string TopLevelNotJson = "TopLevelNotJson";

        public override void Process(opis message)
        {
            opis surc = modelSpec[source].Duplicate();
            instanse.ExecActionModel(surc, surc);
            //if(!modelSpec.isHere(DoNotRunSourceList))
            //instanse.ExecActionModelsList(surc);

            //logopis.AddArr(surc);

            string data = "";
            bool mcCook = modelSpec.isHere(Make_Cookie);
            bool mcJson = modelSpec.isHere(Make_Json);
            bool mcConcat = modelSpec.isHere(Concatenate);

            if (mcCook)
            {
                string sep = "";
                for (int i = 0; i < surc.listCou; i++)
                {
                    if (surc[i].PartitionName.ToLower() != "cookie"
                        && surc[i].PartitionName.ToLower() != "path"
                        && surc[i].PartitionName.ToLower() != "domain"
                        && surc[i].PartitionName.ToLower() != "max-age"
                         && surc[i].PartitionName.ToLower() != "target"
                        )
                    {
                        
                        data += sep + surc[i].PartitionName + "=" + surc[i].body;
                         sep = "; " ;
                    }                   
                }
            }

            if (mcConcat)
            {
                bool trim = modelSpec.isHere(Trim_Element_Quotes);
                for (int i = 0; i < surc.listCou; i++)
                {                  
                    data += trim? surc[i].body.Trim('"'): surc[i].body;
                }
            }

            if (mcJson)
            {
                if(modelSpec[TopLevelNotJson].isInitlze)
                {
                    for (int i = 0; i < surc.listCou; i++)
                    {
                        string sep = i > 0 ? "&" : "";
                        data += sep + surc[i].PartitionName + "=" 
                            + (surc[i].listCou>0? surc[i].body : MakeJsonTree(surc[i])   );
                    }
                }
                else
                data = MakeJsonTree(surc);
            }

            message.body = data;
            message.CopyArr(new opis());

        }

        public string MakeJsonTree(opis p)
        {
            string rez = "{";
            for (int i = 0; i < p.listCou; i++)
            {

                string sep = i > 0 ? "," : "";
                if (p[i].listCou > 0)
                {
                    rez += sep + MakeJsonTree(p[i]);
                }
                else
                    rez += sep + p[i].PartitionName + ":" + p[i].body;

            }

            rez += "}";

            return rez;
        }

    }
}
