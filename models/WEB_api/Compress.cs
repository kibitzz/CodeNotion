using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;



namespace basicClasses.models.WEB_api
{
    [info(" message.body = Comprez_Zlib(message.body);")]
   public class Compress:ModelBase
    {

        [model("spec_tag")]
        [info("")]
        public static readonly string generateSessionId = "generateSessionId";

        [model("spec_tag")]
        [info("set length of generated string (aliquot 4)")]
        public static readonly string generateRandomHex = "generateRandomHex";

        [model("spec_tag")]
        [info("")]
        public static readonly string makeTrackingtoken = "makeTrackingtoken";

        [model("spec_tag")]
        [info("")]
        public static readonly string decodeBase64 = "decodeBase64";

        [model("spec_tag")]
        [info("")]
        public static readonly string ENcodeBase64 = "ENcodeBase64";

        [model("spec_tag")]
        [info("")]
        public static readonly string arhivateBranch = "arhivateBranch";

        [model("spec_tag")]
        [info("")]
        public static readonly string DeArhivateBranch = "DeArhivateBranch";

        [model("spec_tag")]
        [info("you canspecify correction to past, if set body some number of seconds ")]
        public static readonly string epochTime = "epochTime";

        [model("spec_tag")]
        [info("you canspecify correction to past, if set body some number of seconds ")]
        public static readonly string epochTime_ms = "epochTime_ms";
        

        public override void Process(opis message)
        {
            //byte[] buffer = Encoding.UTF8.GetBytes(message.body);

            //string rez = Convert.ToBase64String(buffer);

            // rez = Convert.ToBase64String(CompressBufferDeflate(buffer));

            //message.body = DecompressString(CompressString(message.body));
            //rez = Convert.ToBase64String(DeflateStream.CompressString(message.body));

            if(modelSpec.isHere(generateSessionId))
            {
                string rez = "";
                var r = new Random();

                for (int i = 1; i < 9;i++)
                {
                    int A = r.Next(16000, 65000);
                    string hexValue1 = A.ToString("X");

                    rez += hexValue1;

                    if( i >1 && i< 6)
                    {
                        rez += "-";
                    }
                }

               // 4A7E 3C04 - F684 - 4D60 - AC9E - E66B F1E4 870F

                //byte[] bbb = Encoding.UTF8.GetBytes(message.body);
                //string rez = BitConverter.ToString(bbb).Replace("-", string.Empty);
              
                message.body = rez;
                return;
            }

            if (modelSpec.isHere(generateRandomHex))
            {
                int len = modelSpec[generateRandomHex].intVal;
                len = len / 4;

                string rez = "";
                var r = new Random();

                for (int i = 0; i < len; i++)
                {
                    int A = r.Next(16000, 65000);
                    string hexValue1 = A.ToString("x");

                    rez += hexValue1;
                }

                message.body = rez;
                return;
            }

            if (modelSpec.isHere(epochTime))
            {
                double ts = (DateTime.Now - new DateTime(1970, 1, 1).ToLocalTime()).TotalSeconds;
                double correction = StrUtils.LongFromString(modelSpec.V(epochTime));
                message.body = (ts - correction).ToString().Substring(0, 10);
                return;
            }

            if (modelSpec.isHere(epochTime_ms))
            {
                double ts = (DateTime.Now - new DateTime(1970, 1, 1).ToLocalTime()).TotalSeconds;
                double correction = StrUtils.LongFromString(modelSpec.V(epochTime));
                string ssdf = (ts - correction).ToString().Replace(",", "");

                message.body = ssdf.Length<14 ? ssdf.PadRight(13, '0') : ssdf.Substring(0, 13);
                //message.body =  (ts - correction).ToString().Replace(",", "").Substring(0, 13);
                return;
            }


            if (modelSpec.isHere(decodeBase64))
            {
                byte[] data = Convert.FromBase64String(message.body);
                message.body = Encoding.UTF8.GetString(data);
                return;
            }
       
            if (modelSpec.isHere(ENcodeBase64))
            {
                //logopis["message_body"].body = message.body;
                //logopis["message duplicated"] = message.Duplicate();
                byte[] buffer = Encoding.UTF8.GetBytes(message.body);
                message.body = Convert.ToBase64String(buffer);
                //logopis["Result  message_body"].body = message.body;
                return;
            }

            if (modelSpec.isHere(arhivateBranch))
            {

                message.body = Comprez(message.serialize());
                message.CopyArr(new opis());

                return;
            }

            if (modelSpec.isHere(DeArhivateBranch))
            {
                opis parsed = new opis();
                parsed.load(DeComprez(message.body));
                message.CopyArr(parsed);
                message.body = parsed.body;
                return;
            }

            if (modelSpec.isHere(makeTrackingtoken))
            {
                // here we build payload {"payload":{"server_token":"1484138152791|1421652562016702438|4272561644|76eb81169b957ceeda455393f54d8dde2257da69d47856c0f1c55a9482052be8","is_analytics_tracked":true,"uuid":"2b200c6cdc3841f5a7b58d65fbecb6aa1421652562016702438"},"signature":"","version":5}



                //byte[] buffer = Encoding.UTF8.GetBytes(message.body);
                //string rez = Convert.ToBase64String(buffer);

                //modelSpec[makeTrackingtoken]["userid"]
                //modelSpec[makeTrackingtoken]["imageId"]
                //Convert.FromBase64String
                //message.body = (ts - correction).ToString().Substring(0, 10);
                return;
            }


            message.body = Comprez_Zlib(message.body);     
            
        }


        public void CopyStream(System.IO.Stream input, System.IO.Stream output)
        {
            byte[] buffer = new byte[2000];
            int len;
            while ((len = input.Read(buffer, 0, 2000)) > 0)
            {
                output.Write(buffer, 0, len);
            }
            output.Flush();
        }

        public string Comprez_Zlib(string s)
        {
            var bytes = Encoding.UTF8.GetBytes(s);
      
            string rez = "";

            //MemoryStream outFileStream = new MemoryStream();
            //zlib.ZOutputStream outZStream = new zlib.ZOutputStream(outFileStream, zlib.zlibConst.Z_DEFAULT_COMPRESSION);
            //System.IO.MemoryStream inFileStream = new MemoryStream(bytes);
            //try
            //{
            //    CopyStream(inFileStream, outZStream);
            //}
            //finally
            //{
            //    outZStream.Close();
            //    rez= Convert.ToBase64String(outFileStream.ToArray());
               
            //    outFileStream.Close();
            //    inFileStream.Close();
            //}

            return rez;
        }



        public string Comprez(string s)
        {
            var bytes = Encoding.UTF8.GetBytes(s);
            using (var msi = new MemoryStream(bytes))
            using (var mso = new MemoryStream())
            {
                using (var gs = new DeflateStream(mso, CompressionMode.Compress))
                {
                    msi.CopyTo(gs);
                }
                return Convert.ToBase64String(mso.ToArray());
            }
        }

        public string DeComprez(string s)
        {

            var bytes = new byte[0];
            try
            {

                 bytes = Convert.FromBase64String(s);
            }
            catch
            {
                return s;
            }

            try
            {

                using (var msi = new MemoryStream(bytes))
                using (var mso = new MemoryStream())
                {
                    using (var gs = new DeflateStream(msi, CompressionMode.Decompress))
                    {
                        gs.CopyTo(mso);
                    }
                    return Encoding.UTF8.GetString(mso.ToArray());
                }
            }
            catch
            {
                return "";
            }
           
        }

        public  byte[] CompressBufferDeflate(byte[] buffer)
        {
            byte[] rez;

            using (var ByteStream = new MemoryStream())
            {
                //   GZipStream    DeflateStream
                using (var zip = new DeflateStream(ByteStream, CompressionMode.Compress, true))
                    zip.Write(buffer, 0, buffer.Length);

                rez = ByteStream.ToArray();

            }

            return rez;
        }



    }
}
