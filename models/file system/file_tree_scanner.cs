using basicClasses.Base;
using basicClasses.Interfaces;
using DocumentFormat.OpenXml.Wordprocessing;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace basicClasses.models.file_system
{
    [appliable("func")]
    [info("")]
    internal class file_tree_scanner : ModelBase
    {
        [info("")]
        [model("")]
        public static readonly string root_path = "root_path";

        [info("")]
        [model("spec_tag")]       
        public static readonly string add_file_info = "add_file_info";
     
        [ignore]
        public static readonly string path_separator = @"\";

        bool add_full_info;

        long dirCou;

        public override void Process(opis message)
        {
            opis ms = SpecLocalRunAll();

            add_full_info = ms.isHere(add_file_info);
            dirCou = 0;

            opis rez = GetDirContent(ms.V(root_path), dirCou);

            rez.PartitionName = ms.V(root_path).TrimEnd(path_separator.ToCharArray()[0]);

            message.CopyArr(new opis());
            message.AddArr(rez);
        }

        opis GetDirContent(string path, long parentDirId)
        {
            opis rez = new opis();
            rez.PartitionKind = "dir";

            var dirInf = new DirInfo();
            rez.bodyObject = dirInf;
            dirInf.I = ++dirCou;
            dirInf.D = parentDirId;

            path = path.TrimEnd(path_separator.ToCharArray()[0]);

            FileInfo[] fileinfo = null;
            DirectoryInfo[] dirinfo = null;

            var dirInfo = new DirectoryInfo(path);
            try
            {
                fileinfo = dirInfo.GetFiles();
                dirinfo = dirInfo.GetDirectories();
            }
            catch
            {
                return rez;
            }

            rez.Vset("..", path);
            
            foreach (var dir in dirinfo)
            {
                var d = GetDirContent(path + path_separator + dir.Name, dirInf.I);
                rez[dir.Name] = d;

                var dinf = (d.bodyObject as DirInfo);

                if (dinf != null)
                {
                    dirInf.tSize += dinf.tSize;
                    dirInf.fcTtl += dinf.fcTtl;
                    dirInf.dcTtl += dinf.dcTtl;
                }
            }

            ulong filesDirSize = 0;

            foreach (var file in fileinfo)
            {
                var f = rez[file.Name];
                f.body = BytesToString(file.Length);
                f.PartitionKind = "`";

                var finf = new mFileInfo();
                f.bodyObject = finf;

                finf.D = dirInf.I;
                finf.S = file.Length;
                finf.E = file.Extension;
                finf.C = file.CreationTime.Ticks;
                finf.W = file.LastWriteTime.Ticks;
                finf.A = file.LastAccessTime.Ticks;

                filesDirSize += (ulong)file.Length;

                if (add_full_info)
                {
                    f.Vset("created", file.CreationTime.ToString());
                    f.Vset("ext", file.Extension);

                    f.Vset("size", BytesToString(file.Length));
                    f["size"].Vset("b", file.Length.ToString());

                    f.Vset("last wr", file.LastWriteTime.ToString());
                    f.Vset("last accs", file.LastAccessTime.ToString());

                    f["last wr"].Vset("t", file.LastWriteTime.Ticks.ToString());
                    f["last accs"].Vset("t", file.LastAccessTime.Ticks.ToString());
                    f["created"].Vset("t", file.CreationTime.Ticks.ToString());
                }
            }

            dirInf.flsSize = filesDirSize;
            dirInf.tSize += dirInf.flsSize;
            dirInf.dc = (ulong)dirinfo.Length;
            dirInf.fc = (ulong)fileinfo.Length;

            dirInf.fcTtl += dirInf.fc;
            dirInf.dcTtl += dirInf.dc;

            rez.body = uBytesToString(dirInf.tSize) + "; files: " + dirInf.fc + " (" + uBytesToString(dirInf.flsSize) + "); dirs: " + dirInf.dc + "; tf: " + dirInf.fcTtl;

            return rez;
        }

        static String uBytesToString(ulong byteCount)
        {
            string[] suf = { " B", " KB", " MB", " GB", " TB", " PB", " EB" }; //Longs run out around EB
            if (byteCount == 0)
                return "0" + suf[0];
            ulong bytes = byteCount;
            int place = Convert.ToInt32(Math.Floor(Math.Log(bytes, 1024)));
            double num = Math.Round(bytes / Math.Pow(1024, place), 1);
            return (num).ToString() + suf[place];
        }

        static String BytesToString(long byteCount)
        {
            string[] suf = { " B", " KB", " MB", " GB", " TB", " PB", " EB" }; //Longs run out around EB
            if (byteCount == 0)
                return "0" + suf[0];
            long bytes = Math.Abs(byteCount);
            int place = Convert.ToInt32(Math.Floor(Math.Log(bytes, 1024)));
            double num = Math.Round(bytes / Math.Pow(1024, place), 1);
            return (Math.Sign(byteCount) * num).ToString() + suf[place];
        }

    }

   public class DirInfo: SerializableObj
    {
        /// <summary>
        /// parent directory
        /// </summary>
        public long D;
        /// <summary>
        /// directory ID
        /// </summary>
        public long I;
        public ulong tSize;
        public ulong flsSize;

        public ulong fc;
        public ulong dc;

        public ulong fcTtl;
        public ulong dcTtl;       
    }

    public class mFileInfo : SerializableObj
    {
        /// <summary>
        /// parent directory
        /// </summary>
        public long D;
        /// <summary>
        /// size in bytes
        /// </summary>
        public long S;
        /// <summary>
        /// extention
        /// </summary>
        public string E;
        /// <summary>
        /// creation time
        /// </summary>
        public long C;
        /// <summary>
        /// last write time
        /// </summary>
        public long W;
        /// <summary>
        /// last access time
        /// </summary>
        public long A;      
    }
}
