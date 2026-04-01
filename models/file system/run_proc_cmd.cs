using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace basicClasses.models.file_system
{
    public class run_proc_cmd : ModelBase    
    {
        [model("")]
        [info("")]
        public static readonly string file_name = "file_name";

        [model("")]
        [info("")]
        public static readonly string args = "args";


        public override void Process(opis message)
        {
            opis ms = SpecLocalRunAll();

            var startInfo = new ProcessStartInfo
            {
                FileName = ms.V(file_name),
                Arguments = ms.V(args),
                RedirectStandardOutput = true,
                RedirectStandardError = true, 
                UseShellExecute = false,
                CreateNoWindow = true
            };

            string status = "";

            using (Process process = new Process { StartInfo = startInfo })
            {
                process.Start();
                
                string errorLog = process.StandardError.ReadToEnd();

                process.WaitForExit();

                if (process.ExitCode == 0)
                {
                    status ="done!";
                }
                else
                {
                    status = $"error {ms.V(file_name)}: {errorLog}";                   
                }
            }

            message.CopyArr(new opis());
            message.Vset("status", status);         
        }
    }
}
