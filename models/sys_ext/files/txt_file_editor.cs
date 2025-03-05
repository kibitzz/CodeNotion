using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace basicClasses.models.sys_ext.files
{
    [info("")]
    [appliable("Action func")]
    public class txt_file_editor : ModelBase
    {      

        [model("")]
        [info("full filename to file. call once with this param and next calls use the same lines (if remove this pram)")]
        public static readonly string file = "file";

        [model("spec_tag")]
        [info("")]
        public static readonly string replace = "replace";

        [model("")]
        [info("")]
        public static readonly string by = "by";

        [model("spec_tag")]
        [info("body should contain full filename to save to")]
        public static readonly string save = "save";

        [model("spec_tag")]
        [info("body should contain index of the line where to insert, OR text that should be found as substring in existing line - if found it's index will be used to insert  ")]
        public static readonly string insert = "insert";

        private string[] proc = null;

        public override void Process(opis message)
        {
            opis ms = SpecLocalRunAll();

            string filename = "";

            if (ms.isHere(file))
            {
                filename = ms.V(file).Replace("<%backslash%>", @"\");
                proc = DataFileUtils.LoadLines(filename);
            }

            string setV = ms.V(by);
            string repl = ms.V(replace);

            if (!string.IsNullOrEmpty(repl))
            {                                
                for (int i =0; i < proc.Length; i++)
                {
                    proc[i] = proc[i].Replace(repl, setV);
                }
            }

            if (ms.isHere(insert))
            {
                opis oins = ms[insert].W();
                string inst = oins.body;
                int idx = oins.intVal;
                bool multiline = oins.PartitionKind == "all";

                if (!string.IsNullOrEmpty(setV))
                {
                    if (idx.ToString() != inst)
                    {
                        idx = -1;

                        if (multiline)
                        {
                            List<int> indices = new List<int>();

                            for (int i = 0; i < proc.Length; i++)
                            {
                                if (proc[i].Contains(inst))
                                {
                                    indices.Add(i);
                                }
                            }

                            indices.Reverse();
                            var l = proc.ToList();
                            foreach (var i in indices)
                            {
                                l.Insert(i, setV);
                            }
                            proc = l.ToArray();
                        }
                        else
                        {
                            for (int i = 0; i < proc.Length; i++)
                            {
                                if (proc[i].Contains(inst))
                                {
                                    idx = i;
                                    break;
                                }
                            }
                        }
                    }

                    if (idx != -1)
                    {
                        var l = proc.ToList();
                        l.Insert(idx, setV);
                        proc = l.ToArray();
                    }
                }
            }


            if (ms.isHere(save) && proc != null && !string.IsNullOrEmpty(ms.V(save)))
                DataFileUtils.savefile(proc, ms.V(save));

        }
    }
}
