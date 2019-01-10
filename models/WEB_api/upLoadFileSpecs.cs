using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace basicClasses.models.WEB_api
{
    [info("")]
   public class upLoadFileSpecs:ModelBase
    {
        [info("")]
        [model("")]
        public static readonly string Filename = "Filename";

        [info("якщо заповнено, то передається в message.body =")]
        [model("")]
        public static readonly string url = "url";

        [info("must hawe dot as first char .txt .gif ")]
        [model("")]
        public static readonly string Extention = "Extention";

        [info("")]
        [model("")]
        public static readonly string Directory = "Directory";

        [info("")]
        [model("spec_tag")]
        public static readonly string SaveToCurrDir = "SaveToCurrDir";

        [info("")]
        [model("spec_tag")]
        public static readonly string addTimestampTo_name = "addTimestampTo_name";

        [info("")]
        [model("spec_tag")]
        public static readonly string FillMessageBody = "FillMessageBody";

        [ignore]
        public static readonly string CompiledFilename = "CompiledFilename";


        public override void Process(opis message)
        {
            string defP = Application.StartupPath;
            opis ex = modelSpec.Duplicate();
            instanse.ExecActionModelsList(ex);

            string CompFilename = ex.V(Directory) +@"\"+ ex.V(Filename) + ex.V(Extention);
            if (ex.isHere(SaveToCurrDir))
                CompFilename = defP+@"\" + ex.V(Filename);

            if (ex.isHere(url) && ex[url].isInitlze)
                message.body = ex.V(url);

            if (modelSpec.isHere(FillMessageBody))
                message.body = CompFilename;
            else
            {
                if (!modelSpec.isHere(CompiledFilename))
                    message[CompiledFilename].body = CompFilename;
                else
                    message[CompiledFilename].body = ex.V(CompiledFilename);
            }

            //logopis.AddArr(ex);
        }

    }
}
