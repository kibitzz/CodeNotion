using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace basicClasses.models.sys_ext
{


    [info("file operations (write? others not implemented)")]
    [appliable("Action")]
    public class file : ModelBase
    {

        [model("spec_tag")]
        [info("body contain filename")]
        public static readonly string open_write = "open_write";

        [model("")]
        [info("should contain 'data' partition which contain bodyObject of byte[] type")]
        public static readonly string write = "write";

        [model("spec_tag")]
        [info("")]
        public static readonly string close = "close";


        FileStream fs;

        public override void Process(opis message)
        {
            opis ms = SpecLocalRunAll();

            if (ms.isHere(open_write))
            {
                var path = ms.V(open_write); 
                try
                {
                    fs = File.OpenWrite(path);
                } catch (Exception e)
                {
                    global_log.log.AddArr(new opis() { PartitionName = " ERROR file open_write" , body = e.Message});
                }
                
            }

            if (ms.isHere(write) && fs != null)
            {
                var data = (byte[]) ms[write]["data"].bodyObject;
                ms[write]["data"].bodyObject = null;

                if (data != null && data.Length > 0)
                {

                    try
                    {
                        fs.Write(data, 0, data.Length);
                        fs.Flush();
                    }
                    catch (Exception ex)
                    {
                        if (fs != null)
                            fs.Close();
                    }
                  
                }
            }

            if (ms.isHere(close) && fs!= null)
            {
                fs.Close();
                fs.Dispose();
            }
        }
    }
}
