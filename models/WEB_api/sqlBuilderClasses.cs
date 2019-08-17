
using System;
using System.Text;


namespace SqlBuilderClasses
{
	
	public class timer
	{
		public static bool showTime = true;
		private DateTime start;
		private DateTime finish;

		public timer()
		{
			start = DateTime.Now;
		}
		public void Start()
		{
			start = DateTime.Now;
		}
		public void final()
		{
			if (showTime)
			{
				finish = DateTime.Now;
				TimeSpan workTime;
				workTime = finish - start;
//				Console.WriteLine("       *** Time => {0}", workTime);
			}
		}
		public string final(string mess)
		{
			finish = DateTime.Now;
			TimeSpan workTime;
			workTime = finish - start;
			return ("    ***" + mess + "  time => " + workTime.ToString());
		}
		
	}
	
	public class PrevValGetter
	{
		int prev;
		int curr;
		int prevAlreadyOut;
		int cou;
		
		public PrevValGetter()
		{
			prev = 0;
			curr = 0;
		}
		
		public int Current
		{
			get
			{
				return curr;
			}
			set
			{
				if ((value != curr))
				{
					prev = curr;
					curr = value;
				}
				prevAlreadyOut = -1;
				cou = 0;
			}
		}
		
		public int Previous
		{
			get
			{
				prevAlreadyOut = prev;
				return prev;
			}
		}
		
		public bool isBothEqual
		{
			get
			{
				bool rez = (prev == prevAlreadyOut);
				if (rez) {cou ++;}
				return (cou > 0);
			}
		}
		
		void SetPrevious(int p)
		{
			prev = p;
			prevAlreadyOut = -1;
		}
		
	}
	
	public struct segmCoord
	{
		public int start;
		public int fin;
		
		public bool isEmpty
		{
			get
			{
				return (start == fin);
			}
		}
	}

    public struct segmCoordSeq
    {
        public uint start;
        public uint fin;

        public uint times;
       
    }
	
	public struct usrInfo
	{
		public string id;
		public string name;
		public bool folowing;
	}
	
	
	public struct TweetInfo
	{
		public string id;
		public string text;
		public string price;
		public bool isRetweet;
		public bool useScroll;
		public string secretCode;
		public int err;
	}
	
	
	public class QueryParser
	{
		
													
		static readonly string[] utf8 = {"%D0%90","%D0%91", "%D0%92","%D0%93",
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
			"%D1%8E", "%D1%8F","%20", "%3A", "%2F", "%2C", "%21", "%40", "%3F", "%3D", "%29", "%7C", "%22", "%23",
            "%24"
        };
		
		
		static string[] win1251 = {	"А" ,"Б","В" ,"Г" ,
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
			"ю", "я", " ", ":", @"/", ",", "!", "@", "?", "=", ")", "|", "\"", "#",
            "$",
        };
		
		static string[] kirillTwitter = {"Рђ","Р‘","Р’","Р“",
			"Р”","Р–","Р—","Р•",
			"РЃ","Р","Р™","Рљ",
			"Р›","Рњ","Рќ","Рћ",
			"Рџ","Р ","РЎ","Рў",
			"РЈ","Р¤","РҐ","Р¦",
			"Р§","РЁ","Р©","Р¬",
			"Р«","РЄ","Р­","Р®",
			"РЇ","Р°","Р±","РІ",
			"Рі","Рґ","Р¶","Р·",
			"Рµ","С‘","Рё","Р№",
			"Рє","Р»","Рј","РЅ",
			"Рѕ","Рї","СЂ","СЃ",
			"С‚","Сѓ","С„","С…",
			"С†","С‡","С€","С‰",
			"СЊ","С‹","СЉ","СЌ",
			"СЋ","СЏ"
		};
		
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
			"ю", "я", };
		
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
	 
	 static  string[] utfCodesVariant = { "\u0410","\u0411","\u0412","\u0413",
		 "\u0414","\u0416","\u0417","\u0415",
		 "\u0401","\u0418","\u0419","\u041a",
		 "\u041b","\u041c","\u041d","\u041e",
		 "\u041f","\u0420","\u0421","\u0422",
		 "\u0423","\u0424","\u0425","\u0426",
		 "\u0427","\u0428","\u0429","\u042c",
		 "\u042b","\u042a","\u042d","\u042e",
		 "\u042f","\u0430","\u0431","\u0432",
		 "\u0433","\u0434","\u0436","\u0437",
		 "\u0435","\u0451","\u0438","\u0439",
		 "\u043a","\u043b","\u043c","\u043d",
		 "\u043e","\u043f","\u0440","\u0441",
		 "\u0442","\u0443","\u0444","\u0445",
		 "\u0446","\u0447","\u0448","\u0449",
		 "\u044c","\u044b","\u044a","\u044d",
		 "\u044e","\u044f"}; 
		
		static string[] Engl = {"A", "B", "C", "D", "E", "F", "G", "H", "I", "G", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "V", "W", "X", "Y", "Z", "U", "J"};
		static string[] Russ = {"А", "Б", "Ц", "Д", "Е", "Ф", "Ж", "Ш", "И", "Г", "К", "Л", "М", "Н", "О", "П", "Q", "Р", "С", "Т", "В", "В", "Х", "Ю", "З", "Я", "М"};
		
		
		
							
		
		/// <summary>
		/// big or not i really can not classify but i know how it looks "\u0410"
		/// </summary>
		/// <param name="s">text in krakozabra</param>
		/// <returns>readable Cyrillic text</returns>
		public static string UTF8BigEndian_to_Kirill(string s)
		{
			StringBuilder b = new StringBuilder(s);
			
			for(int i = 0; i < utfCodesVariant.Length; i++)
			{
				b.Replace(utfCodesVariant[i], kirill[i]);
			}
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
			
			for(int i = 0; i < utfCodesVariant.Length; i++)
			{
				b.Replace(kirill[i], utfCodesVariant[i]);
			}
			return b.ToString();
		}	
		
		public static string Kirill_to_TwitterKrakoziabry(string s)
		{
			StringBuilder b = new StringBuilder(s);
			
			for(int i = 0; i < kirill.Length; i++)
			{
				b.Replace(kirill[i], kirillTwitter[i]);
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
			
			for(int i = 0; i < utfCodes.Length; i++)
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
			
			for(int i = 0; i < utfCodes.Length; i++)
			{
				b.Replace(kirill[i], utfCodes[i]);
			}
			return b.ToString();
		}
		
		/// <summary>
		/// it looks  "%D0%90","%D0%91", "%D0%92",
		/// </summary>
		/// <param name="s"></param>
		/// <returns></returns>
		public static string Kirill_to_UTF(string s)
		{
			
			StringBuilder b = new StringBuilder(s);
			
			for(int i = 0; i < kirill.Length; i++)
			{
				b.Replace(kirill[i], utf8[i]);
			}
			return b.ToString();
		}
		
		public static string UTF_to_win1251(string s)
		{
			StringBuilder b = new StringBuilder(s);
			
			for(int i = 0; i < win1251.Length; i++)
			{
				b.Replace(utf8[i], win1251[i]);
			}
			return b.ToString();
		}
		
		public static string Translit(string s)
		{
			StringBuilder b = new StringBuilder(s);
			
			for(int i = 0; i < kirill.Length; i++)
			{
				b.Replace(kirill[i], trans[i]);
			}
			return b.ToString();
		}
		
		public static string ToKirillSet(string s)
		{
			s = s.ToUpper();
			StringBuilder b = new StringBuilder(s);
			
			for(int i = 0; i < Engl.Length; i++)
			{
				b.Replace(Engl[i], Russ[i]);
			}
			return b.ToString();
		}
		
		
		
		public static bool CheckBracketsClosed(string s)
		{
			int tmp = 0;
			bool rez = true;
			
			for (int i = 0; i < s.Length; i++ )
			{
				if (s[i] == '(')
				{
					tmp++;
				}
				if (s[i] == ')')
				{
					tmp--;
					if(tmp < 0){rez = false;}
				}
			}
			return ((tmp == 0) && rez);
		}
		
		public static int EnumCharOccurrence(string s, char[] c)
		{
			int rez = 0;
			foreach(char item in c)
			{
				rez += EnumCharOccurrence(s, item);
			}
			return rez;
		}
		
		public static int EnumCharOccurrence(string s, char c)
		{
			int rez = 0;
			
			for (int i = 0; i < s.Length; i++ )
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
			
			for (int i = 0; i < s.Length; i++ )
			{
				if (Char.IsNumber(s[i]) == false)
				{
					rez = false;
					break;
				}
			}
			return rez;
		}
		
		public static string TrimBrackets(string s)
		{
			s = s.Trim();
			char[] br = {'(',')'};
			
			s = s.Trim(br);
			
			return s;
		}
		
		public static string TruncOrderMdf(string s)
		{
			string rez = s;
			if (IsSortExplicit(s))
			{
				rez = s.TrimEnd().Substring(0, s.Length - 4).TrimEnd();
			}
			
			return rez;
		}
		
		public static bool IsSortExplicit(string s)
		{
			bool rez = false;
			string tmp = s.ToLower().TrimEnd();
			if ((tmp.EndsWith(" asc")) || (tmp.EndsWith(" desc")) || (tmp.EndsWith(" all")) || (tmp.EndsWith(" ___")) )
			{
//				tmp = tmp.Substring(0, tmp.Length - 4);
				rez = true;
			}
			
			return rez;
		}
		
		public static string Recycle(string s)
		{
			string rez = "";
			byte p = 0;
			int le = s.Length;
			
		wvvwuwvvwu:
			switch (p)
			{
				case 0:
					goto lponbc;
					
				case 1:
					goto kjiuhgf;
					
				case 2:
					goto ghtrnm;
					
				case 3:
					goto hiuhgyftrd;
					
				case 4:
					goto kjnbvftrd;
					
				case 5:
					goto jhjkhuytgf;
					
				case 6:
					goto komnbvcfdttr;
					
				case 7:
					goto njkhvcxr;
					
				case 8:
					goto knmhgvfe;
					
				case 9:
					goto kihbvcfs;
					
				default:
					goto  fuff;
					
			}

			
		ghtrnm: // 2
//			rez +=
//			p = le
			goto wvvwuwvvwu;
			
		hiuhgyftrd:  // 3
			goto wvvwuwvvwu;
			
		kihbvcfs: // 9
			goto wvvwuwvvwu;
			
		njkhvcxr: // 7
			goto wvvwuwvvwu;
			
		knmhgvfe: // 8
			goto wvvwuwvvwu;
			
		lponbc: // 0
			
			goto wvvwuwvvwu;
			
		komnbvcfdttr: // 6
			
			goto wvvwuwvvwu;
			
		kjnbvftrd: // 4
			goto wvvwuwvvwu;
			
		jhjkhuytgf: // 5
			goto wvvwuwvvwu;
			
		kjiuhgf: // 1
			goto wvvwuwvvwu;
			
		fuff:
			return rez;
		}
		
	
		
		public static string NormalizeSpaces(string s)
		{
			char[] arr;
			int newLen = 0;
			char prev = 'a';
			arr = new char[s.Length];
			bool constOpen = false;
			
			for (int i = 0; i < s.Length; i++ )
			{
				if (s[i] == (char)39) {constOpen = !constOpen;}
				if (s[i] == (char)0) {continue;}
				
				if ( (((prev == ' ') && (s[i] == ' ') ) != true) || (constOpen) )
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
								
		
		
													
	}

}
