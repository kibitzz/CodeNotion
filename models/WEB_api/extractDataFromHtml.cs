using SqlBuilderClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace basicClasses.models.WEB_api
{
    [info("fill data that found in souce")]
    [appliable("BodyValueModificator")]
   public class extractDataFromHtml:ModelBase
    {
        [model("")]
        [info("any filler will execute and use value")]
        public static readonly string source = "source";

        [model("")]
        [info("")]
        public static readonly string startBlock = "startBlock";

        [model("")]
        [info("")]
        public static readonly string encloseBlock = "encloseBlock";

        QueryParser qp;

        public override void Process(opis message)
        {

            if (qp == null)
                qp = new QueryParser();

            opis surc = modelSpec[source].Duplicate();
            instanse.ExecActionModel(surc, surc);

            opis stb = modelSpec[startBlock].Duplicate();
            instanse.ExecActionModel(stb, stb);
            opis enb = modelSpec[encloseBlock].Duplicate();
            instanse.ExecActionModel(enb, enb);

            string datasource ="#start777#"+ surc.body + "#fin777#";
            datasource = TemplatesMan.UTF8BigEndian_to_Kirill(datasource);

            string[] bl = new string[2];
            bl[0] = string.IsNullOrEmpty(stb.body)? " " : stb.body;
            bl[1] = string.IsNullOrEmpty(enb.body) ? " " : enb.body;           

            message.body= qp.GetTextBlock(datasource, bl);
            message.CopyArr(new opis());

            if (bl[1]== "#fin777#")
            {
                int spi = datasource.IndexOf(" ");
                if (spi > 0)
                {
                    
                    string val = datasource.Substring(spi + 1).Trim();
                    message.body = val.Replace("#fin777#","");
                }
               
            }

        }
    }
}
