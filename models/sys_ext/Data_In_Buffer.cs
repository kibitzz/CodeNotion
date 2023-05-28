using basicClasses.models.WEB_api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace basicClasses.models.sys_ext
{
    [info("filler. get data parced after command <import>. use: to avoid passing data as constant arrays to methods")]
    class Data_In_Buffer : ModelBase
    {
        [model("spec_tag")]
        [info(" ")]
        public static readonly string clipboard = "clipboard";

        public static string rawData;

        public static string clipboard_data;

        public override void Process(opis message)
        {
            opis data = new opis();

            if (!string.IsNullOrEmpty(rawData))
            {

                if (rawData.Trim().StartsWith("{"))
                    data.load(rawData);
                else
                {
                    data.load(Compress.DeComprez(rawData));
                }
            }

            if(modelSpec.isHere(clipboard))
            {
                message.ReinitArr(0);
                message.body = clipboard_data.Trim(); 
                return;
            }
           
            message.CopyArr(data);
        }
       
    }
}
