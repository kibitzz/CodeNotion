using FluentFTP;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace basicClasses.models.WEB_api
{

    [info(" you can run all action types at once, but only in sequence  as appear in model {delete, rename, upload, download} ")]
    class Ftp_client : ModelBase
    {
        [model("ftp_settings")]
        [info("  ")]
        public static readonly string ftp_setting = "ftp_setting";

        [info("NOTE!!!  filename should NOT start with space")]
        public static readonly string local_file = "local_file";

        [info("/htdocs/MyVideo.mp4   filename on ftp side")]
        public static readonly string remote_file = "remote_file";



        [model("spec_tag")]
        [info(" deletes <remote_file> on ftp")]
        public static readonly string delete = "delete";

        [model("spec_tag")]
        [info(" ")]
        public static readonly string rename = "rename";

        [model("spec_tag")]
        [info(" ")]
        public static readonly string upload = "upload";

        [model("spec_tag")]
        [info(" ")]
        public static readonly string compres_upload = "compres_upload";

        [model("spec_tag")]
        [info(" ")]
        public static readonly string delete_local = "delete_local";

        [model("spec_tag")]
        [info(" ")]
        public static readonly string download = "download";

        [model("spec_tag")]
        [info(" get file/dir structure on <remote_file> as catalog.  to recursively scan all subdir set body to <ALL>")]
        public static readonly string scan = "scan";

        [model("")]
        [info("int val 1250 – English + Central Europe  1251 – English + Cyrillic(Russian)  1252 – English + European(accented characters)")]
        public static readonly string encoding = "encoding";

        opis sett;
        int port = 21;

        public override void Process(opis message)
        {
            opis spec = modelSpec.Duplicate();
            instanse.ExecActionModelsList(spec);

            if (spec[ftp_setting].isInitlze)
                sett = spec[ftp_setting];

            int p = sett[ftp_settings.Port].intVal;
            port = p > 0 ? p : port;

            opis rez = new opis();

            try
            {
                var cr = new NetworkCredential(sett.V(ftp_settings.username), sett.V(ftp_settings.password));
                FtpClient client = new FtpClient(sett.V(ftp_settings.FtpServer), port, cr);


                if (spec.isHere(encoding))
                    client.Encoding = Encoding.GetEncoding(spec[encoding].intVal);
                //client.ListingCulture = new CultureInfo("ru-RU");
                client.Connect();


                if (spec.isHere(delete))
                    client.DeleteFile(spec.V(remote_file));

                if (spec.isHere(rename))
                    rez[rename].body = client.MoveFile(spec.V(local_file), spec.V(remote_file)).ToString();

                if (spec.isHere(upload))
                    rez[upload].body = client.UploadFile(spec.V(local_file), spec.V(remote_file), FtpRemoteExists.Overwrite).ToString();

                if (spec.isHere(compres_upload))
                {

                    //using (FileStream originalFileStream = File.OpenRead(spec.V(local_file)))
                    //{                       
                    //        using (FileStream compressedFileStream = File.Create(spec.V(remote_file)))
                    //        {
                    //            using (GZipStream compressionStream = new GZipStream(compressedFileStream,
                    //               CompressionMode.Compress))
                    //            {
                    //                originalFileStream.CopyTo(compressionStream);
                    //            }
                    //        }                                                 
                    //}


                    using (FileStream originalFileStream = File.OpenRead(spec.V(local_file)))
                    {
                        using (MemoryStream mso = new MemoryStream())
                        {
                            using (Stream compressedFTPFileStream = client.OpenWrite(spec.V(remote_file), FtpDataType.Binary))
                            //using (FileStream compressedFileStream = File.OpenWrite(spec.V(remote_file)))
                            using (GZipStream compressionStream = new GZipStream(compressedFTPFileStream, CompressionMode.Compress))                           
                            {
                                originalFileStream.CopyTo(compressionStream);
                                //originalFileStream.Flush();  makes archive broken in the end                               
                            }
                        }
                    }
                }

                if (spec.isHere(delete_local))
                    File.Delete(spec.V(local_file));

                if (spec.isHere(download))
                    rez[download].body = client.DownloadFile(spec.V(local_file), spec.V(remote_file), FtpLocalExists.Overwrite).ToString();

                if (spec.isHere(scan))
                    rez[scan] = spec.V(scan) == "ALL" ? dir(client, spec.V(remote_file)) : dir(client, spec.V(remote_file), false);

                client.Disconnect();

            }
            catch (Exception e)
            {
                rez["Exception"].body = e.Message;
              
            }


            message.CopyArr(rez);
        }


        opis dir(FtpClient client, string path, bool rec = true)
        {
            opis rez = new opis();

            foreach (FtpListItem item in client.GetListing(path))
            {
                opis itm = new opis();

                itm.PartitionName = item.Name;
                var dd = client.GetModifiedTime(item.FullName);
                //  itm.body = dd.ToString();

                if (item.Type == FtpFileSystemObjectType.File)
                {
                    itm.PartitionKind = "file";

                    itm["date"].body = dd.Date.ToString("dd/MM/yyyy");
                    itm["date_ticks"].body = dd.Date.Ticks.ToString();
                    itm["size"].body = client.GetFileSize(item.FullName).ToString();
                }

                if (rec && item.Type == FtpFileSystemObjectType.Directory)
                {
                    itm.CopyArr(dir(client, item.FullName));
                }

                // calculate a hash for the file on the server side (default algorithm)
                //FtpHash hash = client.GetHash(item.FullName);

                rez.AddArr(itm);
            }

            return rez;
        }

        /*          
 // get a list of files and directories in the "/htdocs" folder
foreach (FtpListItem item in client.GetListing("/htdocs")) {
	
	// if this is a file
	if (item.Type == FtpFileSystemObjectType.File){
		
		// get the file size
		long size = client.GetFileSize(item.FullName);
		
	}
	
	// get modified date/time of the file or folder
	DateTime time = client.GetModifiedTime(item.FullName);
	
	// calculate a hash for the file on the server side (default algorithm)
	FtpHash hash = client.GetHash(item.FullName);
	
}

// upload a file
client.UploadFile(@"C:\MyVideo.mp4", "/htdocs/MyVideo.mp4");

// rename the uploaded file
client.Rename("/htdocs/MyVideo.mp4", "/htdocs/MyVideo_2.mp4");

// download the file again
client.DownloadFile(@"C:\MyVideo_2.mp4", "/htdocs/MyVideo_2.mp4");

// delete the file
client.DeleteFile("/htdocs/MyVideo_2.mp4");

// delete a folder recursively
client.DeleteDirectory("/htdocs/extras/");

// check if a file exists
if (client.FileExists("/htdocs/big2.txt")){ }

// check if a folder exists
if (client.DirectoryExists("/htdocs/extras/")){ }

// upload a file and retry 3 times before giving up
client.RetryAttempts = 3;
client.UploadFile(@"C:\MyVideo.mp4", "/htdocs/big.txt", FtpExists.Overwrite, false, FtpVerify.Retry);

         */

    }

    class ftp_settings : ModelBase
    {
        [model("")]
        [info("intVal   ")]
        public static readonly string Port = "Port";

        [model("")]
        [info("ip address")]
        public static readonly string FtpServer = "FtpServer";

        [model("")]
        [info(" ")]
        public static readonly string username = "username";

        [model("")]
        [info(" ")]
        public static readonly string password = "password";

    }
}
