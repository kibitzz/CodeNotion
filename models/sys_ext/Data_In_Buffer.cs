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
        public static string rawData;       

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

            message.CopyArr(data);
        }
    }
}
