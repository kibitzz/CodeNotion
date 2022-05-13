using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using basicClasses.models;
using System.IO.Compression;

namespace basicClasses
{
    public class Parser
    {
        public string[] dataOut;
        public int datcou;
        public static opis ContextGlobal;

        void tell(string s)
        {
            if (dataOut == null)
            {
                dataOut = new string[10000];
            }
            dataOut[datcou] = s;
            datcou++;
        }

        public static void LoadEnvironment()
        {
            LoadEnvironment("context.zip");
        }

        public static void LoadEnvironment(string path)
        {
            string data = "";

            string zipdata = DataFileUtils.LoadSingleLineZipped(path);
            if (!string.IsNullOrEmpty(zipdata))
            {
                data = zipdata;
            }

            ContextGlobal = new opis();
            ContextGlobal.load(data);

            DeCompressWords();

        }

        public static void SaveEnvironment()
        {
            SaveEnvironment(false);
        }

        public static void SaveEnvironment(bool guiExit)
        {
            if (guiExit)
                CompressWords();

            string fd = ContextGlobal.serialize();          
            DataFileUtils.savefileZipped(fd, "context.zip");
         
            File.Copy("context.zip", "context " + DateTime.Now.Ticks.ToString() + ".zip");          
            System.Media.SystemSounds.Asterisk.Play();
        }

        public static void CompressWords()
        {
          
            var words = ContextGlobal["words"];
            var sentence = words.Find("sentence_context");
           
            if (!sentence.isInitlze)
                return;

            if (sentence.isHere("compress words"))
            {
                for (int i = 0; i < words.listCou; i++)
                {
                    OntologyTreeBuilder.CompressTerm(words[i]);
                }
            }
        }

        public static void DeCompressWords()
        {
            var words = ContextGlobal["words"];
            var sentence = words.Find("sentence_context");

            if (!sentence.isInitlze)
                return;

            if (sentence.isHere("compress words"))
            {
                for (int i = 0; i < words.listCou; i++)
                {
                    OntologyTreeBuilder.DecompressTerm(words[i]);
                }
            }
        }


    }



    class StrUtils
    {
        static char[] digits = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };

        public static int EnumCharOccurrence(string s, char[] c)
        {
            int rez = 0;
            foreach (char item in c)
            {
                rez += EnumCharOccurrence(s, item);
            }
            return rez;
        }

        public static int EnumCharOccurrence(string s, char c)
        {
            int rez = 0;

            for (int i = 0; i < s.Length; i++)
            {
                if (s[i] == c)
                {
                    rez++;
                }
            }
            return rez;
        }

        public static bool IsNumber(string s)
        {
            if (s == null)
            {
                return false;
            }
            bool rez = true;

            for (int i = 0; i < s.Length; i++)
            {
                if (Char.IsNumber(s[i]) == false)
                {
                    rez = false;
                    break;
                }
            }
            return rez;
        }

        public static int PriceFromString(string price)
        {
            int rez = (int)PriceFromStringF(price);
            return rez;
        }

        public static long LongFromString(string alphanum)
        {
            long rez = 0;

            alphanum = alphanum.Replace(" ", "");

            long dis = 1;

            if (alphanum.StartsWith("-"))
                dis = -1;

            for (int i = (alphanum.Length - 1); i >= 0; i--)
            {
                int pos = Array.IndexOf(digits, alphanum[i]);
                if (pos != -1 && dis < 1000000000000000000)
                {
                    rez += dis * pos;
                    dis *= 10;
                }
            }


            return rez;
        }

        public static float PriceFromStringF(string price)
        {
            float rez = 0;

            if (price == null)
            {
                price = "";
            }
            float mult = 1;

            price = price.Replace(" ", "").Replace("\"","").TrimStart('0');

            if (price.StartsWith("-"))
                mult = -1;

            if (EnumCharOccurrence(price, digits) >= (price.Length - 1))
            {
                string[] p = price.Split(new char[] { '.', ',' });
                int dis = 1;
                string integer = p[0];

                for (int i = (integer.Length - 1); i >= 0; i--)
                {
                    int pos = Array.IndexOf(digits, integer[i]);
                    if (pos != -1)
                    {
                        rez += dis * pos;
                        dis *= 10;
                    }
                }

                if (p.Length == 2)
                {
                    float disf = 0.1f;
                    integer = p[1];

                    for (int i = 0; i < integer.Length; i++)
                    {
                        int pos = Array.IndexOf(digits, integer[i]);
                        if (pos != -1)
                        { // 123.235
                            rez += disf * pos;
                            disf *= 0.1f;
                        }
                    }

                }
            }

            return rez * mult;
        }

        public static string NormalizeSpaces(string s)
        {
            char[] arr;
            int newLen = 0;
            char prev = 'a';
            arr = new char[s.Length];
            bool constOpen = false;

            for (int i = 0; i < s.Length; i++)
            {
                if (s[i] == (char)39) { constOpen = !constOpen; }
                if (s[i] == (char)0) { continue; }

                if ((((prev == ' ') && (s[i] == ' ')) != true) || (constOpen))
                {
                    arr[newLen] = s[i];
                    prev = s[i];
                    newLen++;
                }
            }
            Array.Resize(ref arr, newLen);

            string rez = new string(arr);
            return rez;
        }

        public static string GetEnclosedNested(string s, int startPos, string opener, string enclose)
        {
           
            string rez = "";
            for (int i = 0; i < s.Length; i++)
            {
               
            }
            return rez;
        }

       
    }

    class DataFileUtils
    {

        public static byte[] LoadFile(string fname)
        {

            byte[] rez = new byte[64 * 1024];

            int read;

            try
            {
                using (FileStream fs = File.OpenRead(fname))
                {
                    using (var ByteStream = new MemoryStream())
                    {
                        while ((read = fs.Read(rez, 0, rez.Length)) > 0)
                        {
                            ByteStream.Write(rez, 0, read);
                            ByteStream.Flush(); //seems to have no effect
                        }

                        rez = ByteStream.ToArray();
                    }
                }
            } catch(Exception e)
            {
                rez = new byte[0];
            }



            return rez;

        }

        public static void savefile(string data, string path)
        {
            if (!string.IsNullOrEmpty(data))
            {
                try
                {
                    using (StreamWriter sw = new StreamWriter(path))
                    {
                        sw.WriteLine(data);

                    }
                }
                catch (Exception e)
                {

                }
            }
        }

        public static void savefile(string[] data, string path)
        {
            if (data != null && data.Length > 0)
            {
                
                using (StreamWriter sw = new StreamWriter(path,false, Encoding.UTF8))
                {
                    for (int i = 0; i < data.Length; i++)
                    {
                        sw.WriteLine(data[i]);
                    }

                }
            }
        }

        public static void savefileZipped(string data, string path)
        {
           
                using (FileStream fs = File.OpenWrite(path))
                using (GZipStream zs = new GZipStream(fs, CompressionMode.Compress, true))
                using (StreamWriter sw = new StreamWriter(zs))
                {
                    sw.WriteLine(data);               
                }
            
        }
    

        public static string LoadSingleLineZipped(string path)
        {

            string rez = "";

            StringBuilder sb = new StringBuilder();

            try
            {

                using (FileStream fs = File.OpenRead(path))
                using (GZipStream zs = new GZipStream(fs, CompressionMode.Decompress, true))
                using (StreamReader sr = new StreamReader(zs))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        line = line.Trim();
                        if (line == "") { continue; }

                        sb.Append(line);
                       // rez += line + " ";
                    }
                }
             
            }
            catch (Exception e)
            {

            }

            return sb.ToString();
        }


        public static string LoadSingleLine(string path)
        {

            string rez = "";

            try
            {
                using (StreamReader sr = new StreamReader(path))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        line = line.Trim();
                        if (line == "") { continue; }

                        rez += line+" ";

                    }
                }
            }
            catch (Exception e)
            {

            }

            //cache.SetParam(path, rez);

            return rez;
        }

        public static string[] LoadLines(string path)
        {

            string[] rez = new string[70000];
            int i = 0;

            try
            {
                using (StreamReader sr = new StreamReader(path))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        //line = line.Trim();
                        //if (line == "") { continue; }

                        rez[i] = line;
                        i++;
                        if(i >= rez.Length)
                        {
                            Array.Resize(ref rez, i+ 70000);
                        }
                    }
                }
            }
            catch (Exception e)
            {

            }

            Array.Resize(ref rez, i);

            return rez;
        }

    }


}
