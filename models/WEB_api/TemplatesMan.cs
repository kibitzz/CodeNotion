using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using System.IO.Compression;
using Microsoft.Win32;
using System.Collections;
using System.Reflection;

namespace basicClasses.models.WEB_api
{  
    class TemplatesMan
    {
        static char[] digits = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };


        #region  utf8 constants

        static string[] utf8_aspforms = {"%D0%90","%D0%91", "%D0%92","%D0%93",
            "%D0%94","%D0%96", "%D0%97", "%D0%95",
            "%D0%81", "%D0%98","%D0%99","%D0%9A",
            "%D0%9B","%D0%9C", "%D0%9D", "%D0%9E",
            "%D0%9F","%D0%A0", "%D0%A1","%D0%A2",
            "%D0%A3","%D0%A4", "%D0%A5", "%D0%A6",
            "%D0%A7","%D0%A8", "%D0%A9", "%D0%AC",
            "%D0%AB", "%D0%AA", "%D0%AD", "%D0%AE",
            "%D0%AF", "%D0%B0", "%D0%B1","%D0%B2",
            "%D0%B3","%D0%B4","%D0%B6", "%D0%B7",
            "%D0%B5", "%D1%91", "%D0%B8", "%D0%B9",
            "%D0%BA","%D0%BB", "%D0%BC","%D0%BD",
            "%D0%BE","%D0%BF","%D1%80", "%D1%81",
            "%D1%82", "%D1%83","%D1%84","%D1%85",
            "%D1%86","%D1%87","%D1%88", "%D1%89",
            "%D1%8C","%D1%8B","%D1%8A","%D1%8D",
            "%D1%8E", "%D1%8F",
          
              "%D1%96", "%D1%94", "%D0%86", "%D0%84", "%D1%97", "%D0%87",  
            //"%20",
            "%3A", "%2F", "%2C", "%21", "%40", "%3F", "%3D", "%29", "%7C", "%22", "%23", "%09", "%0D%0A", "%D1%96", "%2B", "+", "%28",
            "%27", "%D1%97", "%D1%94", "%D0%86", "%C2%A0", "%E2%84", "%96", "%5B", "%5D", "%3B",
            "%3c", "%3e", "%3C", "%3E", "%7B", "%7D", "%24", "%26", "%5E"     };

        // URL ENCODE
        static string[] utf8 = {"%D0%90","%D0%91", "%D0%92","%D0%93",
			"%D0%94","%D0%96", "%D0%97", "%D0%95",
			"%D0%81", "%D0%98","%D0%99","%D0%9A",
			"%D0%9B","%D0%9C", "%D0%9D", "%D0%9E",
			"%D0%9F","%D0%A0", "%D0%A1","%D0%A2",
			"%D0%A3","%D0%A4", "%D0%A5", "%D0%A6",
			"%D0%A7","%D0%A8", "%D0%A9", "%D0%AC",
			"%D0%AB", "%D0%AA", "%D0%AD", "%D0%AE",
			"%D0%AF", "%D0%B0", "%D0%B1","%D0%B2",
			"%D0%B3","%D0%B4","%D0%B6", "%D0%B7",
			"%D0%B5", "%D1%91", "%D0%B8", "%D0%B9",
			"%D0%BA","%D0%BB", "%D0%BC","%D0%BD",
			"%D0%BE","%D0%BF","%D1%80", "%D1%81",
			"%D1%82", "%D1%83","%D1%84","%D1%85",
			"%D1%86","%D1%87","%D1%88", "%D1%89",
			"%D1%8C","%D1%8B","%D1%8A","%D1%8D",
			"%D1%8E", "%D1%8F",
         
              "%D1%96", "%D1%94", "%D0%86", "%D0%84", "%D1%97", "%D0%87",  
            "%20", "%3A", "%2F", "%2C", "%21", "%40", "%3F", "%3D", "%29", "%7C", "%22", "%23", "%09", "%0D%0A", "%D1%96", "%2B", "+", "%28",
            "%27", "%D1%97", "%D1%94", "%D0%86", "%C2%A0", "%E2%84", "%96", "%5B", "%5D", "%3B",
            "%3c", "%3e", "%3C", "%3E", "%7B", "%7D", "%24"   };



        static string[] utf8Pref25 = { "%25D0%2590","%25D0%2591", "%25D0%2592","%25D0%2593",
            "%25D0%2594","%25D0%2596", "%25D0%2597", "%25D0%2595",
            "%25D0%2581", "%25D0%2598","%25D0%2599","%25D0%259A",
            "%25D0%259B","%25D0%259C", "%25D0%259D", "%25D0%259E",
            "%25D0%259F","%25D0%25A0", "%25D0%25A1","%25D0%25A2",
            "%25D0%25A3","%25D0%25A4", "%25D0%25A5", "%25D0%25A6",
            "%25D0%25A7","%25D0%25A8", "%25D0%25A9", "%25D0%25AC",
            "%25D0%25AB", "%25D0%25AA", "%25D0%25AD", "%25D0%25AE",
            "%25D0%25AF", "%25D0%25B0", "%25D0%25B1","%25D0%25B2",
            "%25D0%25B3","%25D0%25B4","%25D0%25B6", "%25D0%25B7",
            "%25D0%25B5", "%25D1%2591", "%25D0%25B8", "%25D0%25B9",
            "%25D0%25BA","%25D0%25BB", "%25D0%25BC","%25D0%25BD",
            "%25D0%25BE","%25D0%25BF","%25D1%2580", "%25D1%2581",
            "%25D1%2582", "%25D1%2583","%25D1%2584","%25D1%2585",
            "%25D1%2586","%25D1%2587","%25D1%2588", "%25D1%2589",
            "%25D1%258C","%25D1%258B","%25D1%258A","%25D1%258D",
            "%25D1%258E", "%25D1%258F","%20", "%3A", "%2F", "%2C", "%21", "%40", "%3F", "%3D", "%29", "%7C", "%22", "%23", "%09", "%0D%0A", "%D1%96", "%2B", "+", "%28",
            "%27", "%25D1%2597", "%25D1%2594", "%25D0%2586", "%25C2%25A0", "%25E2%2584", "%96", "%5B", "%5D", "%3B",
            "%3c", "%3e", "%3C", "%3E", "%7B", "%7D", "%24"     };


   //     static string[] win1251 = { "А" ,"Б","В" ,"Г" ,
			//"Д" ,"Ж","З" ,"Е",
			//"Ё" ,"И" ,"Й","К" ,
			//"Л" ,"М" ,"Н"  ,"О" ,
			//"П" ,"Р" ,"С"  ,"Т" ,
			//"У" ,"Ф" ,"Х"  ,"Ц" ,
			//"Ч" ,"Ш" ,"Щ"  ,"Ь"  ,
			//"Ы" ,"Ъ","Э"  ,"Ю"  ,
			//"Я","а","б"  ,"в"  ,
			//"г" ,"д","ж","з" ,
			//"е" ,"ё","и"  ,"й",
			//"к", "л" ,"м","н" ,
			//"о", "п","р" ,"с",
			//"т","у" ,"ф","х" ,
			//"ц","ч","ш","щ" ,
			//"ь", "ы" ,"ъ","э" ,
			//"ю", "я", " ", ":", "/", ",", "!", "@", "?", "=", ")", "|", "\"", "#", "\t", "\n", "і", "+", " ", "(",
   //         "'", "ї",  "є", "І", " ", "№", "–", "[", "]", ";",
   //          "<", ">",  "<", ">", "{" , "}", "$"  };

        static string[] win1251 = { "А" ,"Б","В" ,"Г" ,
            "Д" ,"Ж","З" ,"Е",
            "Ё" ,"И" ,"Й","К" ,
            "Л" ,"М" ,"Н"  ,"О" ,
            "П" ,"Р" ,"С"  ,"Т" ,
            "У" ,"Ф" ,"Х"  ,"Ц" ,
            "Ч" ,"Ш" ,"Щ"  ,"Ь"  ,
            "Ы" ,"Ъ","Э"  ,"Ю"  ,
            "Я","а","б"  ,"в"  ,
            "г" ,"д","ж","з" ,
            "е" ,"ё","и"  ,"й",
            "к", "л" ,"м","н" ,
            "о", "п","р" ,"с",
            "т","у" ,"ф","х" ,
            "ц","ч","ш","щ" ,
            "ь", "ы" ,"ъ","э" ,
            "ю", "я",
             "і", "є", "І", "Є", "ї", "Ї",
            " ", ":", "/", ",", "!", "@", "?", "=", ")", "|", "\"", "#", "\t", "\n", "і", "+", " ", "(",
            "'", "ї",  "є", "І", " ", "№", "–", "[", "]", ";",
             "<", ">",  "<", ">", "{" , "}", "$"  };

        static string[] win1251_aspforms = { "А" ,"Б","В" ,"Г" ,
            "Д" ,"Ж","З" ,"Е",
            "Ё" ,"И" ,"Й","К" ,
            "Л" ,"М" ,"Н"  ,"О" ,
            "П" ,"Р" ,"С"  ,"Т" ,
            "У" ,"Ф" ,"Х"  ,"Ц" ,
            "Ч" ,"Ш" ,"Щ"  ,"Ь"  ,
            "Ы" ,"Ъ","Э"  ,"Ю"  ,
            "Я","а","б"  ,"в"  ,
            "г" ,"д","ж","з" ,
            "е" ,"ё","и"  ,"й",
            "к", "л" ,"м","н" ,
            "о", "п","р" ,"с",
            "т","у" ,"ф","х" ,
            "ц","ч","ш","щ" ,
            "ь", "ы" ,"ъ","э" ,
            "ю", "я",
             "і", "є", "І", "Є", "ї", "Ї",
            
            ":", "/", ",", "!", "@", "?", "=", ")", "|", "\"", "#", "\t", "\n", "і", "+", " ", "(",
            "'", "ї",  "є", "І", " ", "№", "–", "[", "]", ";",
             "<", ">",  "<", ">", "{" , "}", "$", "&", "^"  };

        static string[] kirill = {	"А" ,"Б","В" ,"Г" ,
			"Д" ,"Ж","З" ,"Е",
			"Ё" ,"И" ,"Й","К" ,  
			"Л" ,"М" ,"Н"  ,"О" ,
			"П" ,"Р" ,"С"  ,"Т" ,
			"У" ,"Ф" ,"Х"  ,"Ц" ,
			"Ч" ,"Ш" ,"Щ"  ,"Ь"  ,
			"Ы" ,"Ъ","Э"  ,"Ю"  ,
			"Я","а","б"  ,"в"  ,
			"г" ,"д","ж","з" ,
			"е" ,"ё","и"  ,"й",
			"к", "л" ,"м","н" ,
			"о", "п","р" ,"с",
			"т","у" ,"ф","х" ,
			"ц","ч","ш","щ" ,
			"ь", "ы" ,"ъ","э" ,
			"ю", "я",
            "і", "є", "І", "Є", "ї", "Ї", "№", "«", "»"};

        static string[] trans = {	"A" ,"B","V" ,"G" ,
			"D" ,"G","Z" ,"E",
			"E" ,"I" ,"J","K" ,
			"L" ,"M" ,"N"  ,"O" ,
			"P" ,"R" ,"S"  ,"T" ,
			"U" ,"F" ,"H"  ,"C" ,
			"CH" ,"SH" ,"SCH"  ,"J"  ,
			"I" ,"","YE"  ,"YU"  ,
			"YA","a","b"  ,"v"  ,
			"g" ,"d","g","z" ,
			"e" ,"e","i"  ,"j",
			"k", "l" ,"m","n" ,
			"o", "p","r" ,"s",
			"t","u" ,"f","h" ,
			"c","ch","sh","sch" ,
			"", "i" ,"","ye" ,
			"yu", "ya", };

        static string[] utfCodes = { "&#1040;","&#1041;","&#1042;","&#1043;",
		 "&#1044;","&#1046;","&#1047;","&#1045;",
		 "&#1025;","&#1048;","&#1049;","&#1050;",
		 "&#1051;","&#1052;","&#1053;","&#1054;",
		 "&#1055;","&#1056;","&#1057;","&#1058;",
		 "&#1059;","&#1060;","&#1061;","&#1062;",
		 "&#1063;","&#1064;","&#1065;","&#1068;",
		 "&#1067;","&#1066;","&#1069;","&#1070;",
		 "&#1071;","&#1072;","&#1073;","&#1074;",
		 "&#1075;","&#1076;","&#1078;","&#1079;",
		 "&#1077;","&#1105;","&#1080;","&#1081;",
		 "&#1082;","&#1083;","&#1084;","&#1085;",
		 "&#1086;","&#1087;","&#1088;","&#1089;",
		 "&#1090;","&#1091;","&#1092;","&#1093;",
		 "&#1094;","&#1095;","&#1096;","&#1097;",
		 "&#1100;","&#1099;","&#1098;","&#1101;",
		 "&#1102;","&#1103;"};

        static string[] utfCodesVariant = { @"\u0410",@"\u0411",@"\u0412",@"\u0413",
         @"\u0414",@"\u0416",@"\u0417",@"\u0415",
         @"\u0401",@"\u0418",@"\u0419",@"\u041a",
         @"\u041b",@"\u041c",@"\u041d",@"\u041e",
         @"\u041f",@"\u0420",@"\u0421",@"\u0422",
         @"\u0423",@"\u0424",@"\u0425",@"\u0426",
         @"\u0427",@"\u0428",@"\u0429",@"\u042c",
         @"\u042b",@"\u042a",@"\u042d",@"\u042e",
         @"\u042f",@"\u0430",@"\u0431",@"\u0432",
         @"\u0433",@"\u0434",@"\u0436",@"\u0437",
         @"\u0435",@"\u0451",@"\u0438",@"\u0439",
         @"\u043a",@"\u043b",@"\u043c",@"\u043d",
         @"\u043e",@"\u043f",@"\u0440",@"\u0441",
         @"\u0442",@"\u0443",@"\u0444",@"\u0445",
         @"\u0446",@"\u0447",@"\u0448",@"\u0449",
         @"\u044c",@"\u044b",@"\u044a",@"\u044d",
         @"\u044e",@"\u044f",
         @"\u0456", @"\u0454",  @"\u0406", @"\u0404", @"\u0457", @"\u0407", @"\u2116", @"\u00ab", @"\u00bb"};

        static string[] Engl = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "G", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "V", "W", "X", "Y", "Z", "U", "J" };
        static string[] Russ = { "А", "Б", "Ц", "Д", "Е", "Ф", "Ж", "Ш", "И", "Г", "К", "Л", "М", "Н", "О", "П", "Q", "Р", "С", "Т", "В", "В", "Х", "Ю", "З", "Я", "М" };

        #endregion


        #region  utf8 CONVERSION

        /// <summary>
        /// big or not i really can not classify but i know how it looks "\u0410"
        /// </summary>
        /// <param name="s">text in krakozabra</param>
        /// <returns>readable Cyrillic text</returns>
        public static string UTF8BigEndian_to_Kirill(string s)
        {
            StringBuilder b = new StringBuilder(s);

            for (int i = 0; i < utfCodesVariant.Length; i++)
            {
                b.Replace(utfCodesVariant[i], kirill[i]);
            }

            //string rez = b.ToString();
            //if(rez!= s)
            //{
            //    return rez;
            //}

            return b.ToString();
        }

        /// <summary>
        ///  it looks "\u0410"
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string Kirill_to_UTF8BigEndian(string s)
        {
            StringBuilder b = new StringBuilder(s);

            for (int i = 0; i < utfCodesVariant.Length; i++)
            {
                b.Replace(kirill[i], utfCodesVariant[i]);
            }
            return b.ToString();
        }

        /// <summary>
        /// it looks "& #1040;" separated by semicolon so i added it to each char
        /// </summary>
        /// <param name="s">text in krakozabra</param>
        /// <returns>readable Cyrillic text</returns>
        public static string UTF8Codes_to_Kirill(string s)
        {

            StringBuilder b = new StringBuilder(s);

            for (int i = 0; i < utfCodes.Length; i++)
            {
                b.Replace(utfCodes[i], kirill[i]);
            }
            return b.ToString();
        }

        /// <summary>
        /// it looks "& #1040;"
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string Kirill_to_UTF8Codes(string s)
        {

            StringBuilder b = new StringBuilder(s);

            for (int i = 0; i < utfCodes.Length; i++)
            {
                b.Replace(kirill[i], utfCodes[i]);
            }
            return b.ToString();
        }

        public static string UrlEncodeFull(string s)
        {

            //  StringBuilder b = new StringBuilder(s);

            for (int i = 0; i < win1251.Length; i++)
            {
                s = s.Replace(win1251[i], utf8[i]);
            }
            return s;
        }

        public static string AspnetUrlEncodeFull(string s)
        {           

            for (int i = 0; i < win1251_aspforms.Length; i++)
            {
                s = s.Replace(win1251_aspforms[i], utf8_aspforms[i]);
            }
            return s;
        }

        /// <summary>
        /// it looks  "%D0%90","%D0%91", "%D0%92",
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string Kirill_to_UTF(string s)
        {

          //  StringBuilder b = new StringBuilder(s);

            for (int i = 0; i < kirill.Length; i++)
            {
               s= s.Replace(win1251[i], utf8[i]);
            }
            return s;
        }

        public static string Url_encode(string s)
        {

            StringBuilder b = new StringBuilder(s);

            //for (int i = 0; i < utf8.Length; i++)
            //{
                b.Replace("/", "%2F");
            b.Replace("+", "%2B");
            //}
            return b.ToString();
        }

        public static string UTF_to_UTF25pref(string s)
        {

            StringBuilder b = new StringBuilder(s);

            for (int i = 0; i < utf8.Length; i++)
            {
                b.Replace(utf8[i], utf8Pref25[i]);
            }
            return b.ToString();
        }

        public static string UTF_to_win1251(string s)
        {
            StringBuilder b = new StringBuilder(s);

            for (int i = 0; i < win1251.Length; i++)
            {
                b.Replace(utf8[i], win1251[i]);
            }
            return b.ToString();
        }


        #endregion



        #region PriceFromString  IsNumber

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

        public static float PriceFromStringF(string price)
        {
            float rez = 0;

            if (price == null)
            {
                price = "";
            }

            price = price.Replace(" ", "");

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

            return rez;
        }

        public static long LongFromString(string alphanum)
        {
            long rez = 0;

            alphanum = alphanum.Replace(" ", "");



            long dis = 1;


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

        #endregion


        public static byte[] CompressBufferDeflate(byte[] buffer)
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

        public static byte[] CompressBufferGZip(byte[] buffer)
        {
            byte[] rez;

            using (var ByteStream = new MemoryStream())
            {
                //   GZipStream    DeflateStream
                using (var zip = new GZipStream(ByteStream, CompressionMode.Compress, true))
                    zip.Write(buffer, 0, buffer.Length);
                rez = ByteStream.ToArray();

            }

            return rez;
        }

        public static byte[] LoadFile(string fname)
        {

            byte[] rez = new byte[64 * 1024];
            
            int read;

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

           

            return rez;

        }

        public static void savefile(string data, string path)
        {
            if (!string.IsNullOrEmpty(data))
            {

                using (StreamWriter sw = new StreamWriter(path))
                {
                    sw.WriteLine(data);

                }
            }
        }

        public static void savefile(string[] data, string path)
        {
            if (data != null && data.Length>0)
            {

                using (StreamWriter sw = new StreamWriter(path))
                {
                    for (int i = 0; i < data.Length; i++)
                    {
                        sw.WriteLine(data[i]);
                    }

                }
            }
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
                        line = line.Trim();
                        if (line == "") { continue; }

                        rez[i]= line;
                        i++;
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
