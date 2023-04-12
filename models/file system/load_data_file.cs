using basicClasses.models.DataSets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace basicClasses.models.sys_ext
{
    [appliable("Action ")]
    [info("set role Data_loaded")]
   public class load_data_file:ModelBase
    {
        [model("")]
        [info("")]
        public static readonly string file = "file";

        [model("spec_tag")]
        [info("якщо потрібно завантажити(FILLER) значення напряму в обєкт")]
        public static readonly string FillMessage = "FillMessage";

        [model("")]
        [info("role to set  [Data_loaded] is default(if deleted this partition)")]
        public static readonly string role = "role";

        [model("file_formats")]
        [info("")]
        public static readonly string format_type = "format_type";

        public override void Process(opis message)
        {
            opis data = new opis();
            opis curr_data = data;
            data.PartitionName = "root";

            opis treeStruct = new opis();
            treeStruct["data"].Wrap(curr_data);

            opis curr_value = new opis();

            opis ex = modelSpec.Duplicate();
            instanse.ExecActionModelsList(ex);
            modelSpec = ex;

            string filename = modelSpec.V(file).Replace("<%backslash%>", @"\");
            string[] proc= DataFileUtils.LoadLines(filename);

            string format = "my";
            if (modelSpec[format_type].listCou > 0)
                format = modelSpec[format_type][0].PartitionName;

            if (format != "my")
            {
                opis paramToParser = new opis("stringArray", (object)proc);
              
               instanse.ExecActionResponceModelsList(modelSpec[format_type], paramToParser);

                data = paramToParser["data"];
              
            }

             #region  tree structure from 1c (MY format)

                if (format == "my")
                foreach (string s in proc)
                {
                    string ds = s.Trim();
                    if (string.IsNullOrEmpty(ds))
                        continue;

                    if (ds == "{" || ds == "}")
                    {
                        if (ds == "{")
                        {
                            opis treeStructProc = new opis();
                            treeStructProc["higher"] = treeStruct;
                            curr_data = curr_value;
                            treeStructProc["data"].Wrap(curr_data);
                            treeStruct = treeStructProc;
                        }

                        if (ds == "}")
                        {
                            treeStruct = treeStruct["higher"];
                            //if (!treeStruct.isInitlze)
                            //    logopis.Vset(curr_data.PartitionName, "err: parent object not found");

                            curr_data = treeStruct["data"].W();
                        }
                    }
                    else
                    {

                        //opis treeStructProc = new opis();
                        //treeStructProc["higher"] = treeStruct;

                        opis vvv = new opis();
                        curr_data[ds] = vvv;
                        curr_value = vvv;

                        //treeStructProc["data"].Wrap(curr_data);
                        //treeStruct = treeStructProc;

                    }

                }

            #endregion

            SharedContextRoles.SetRole(data, modelSpec.isHere(role) ? modelSpec[role].body : "Data_loaded", sharedVal);

            if(modelSpec.isHere(FillMessage))
            {
                message.CopyArr(data);
            }

         
        }

    }
}
