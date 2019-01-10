
using System;
using System.Text;
using System.Windows.Forms;
using System.Data;
using System.Data.OleDb;
using Microsoft.Win32; // work with registry
using System.IO;
using System.Runtime.InteropServices;
using twite;

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
		#region constants
		const byte sqCou = 21;
		
		string[] syntax_session_loggedin =  {"<meta content=\"", "\" name=\"session-loggedin\""};
		string[] syntax_session_loggedinNS =  {"twttr.loggedIn =", ";"};
		string[] syntax_PostAuthToken =      {"\"postAuthenticityToken\":\"", "\""};
		string[] syntax_session_userNS =    {"twttr.currentUserScreenName = '", "';"};
		string[] syntax_session_page_user = {"<meta content=\"", "\" name=\"page-user-screen_name\""};
		string[] syntax_page_title =  {"title id=\"page_title\">", "</title>"};
		string[] syntax_page_next_page =  {"<a href=\"", "\" class=\"section_links\" rel=\"me next\""};
		string[] syntax_authenticity =  {"name=\"authenticity_token\" type=\"hidden\" value=\"",  "\" />"};
		string[] syntax_authenticityNS =  {"value='",  "' name='authenticity_token'"};
		
		string[] syntax_following_count = {"<span id=\"following_count\" class=\"stats_count numeric\">", "</span>"};
		string[] syntax_follower_count = {"<span id=\"follower_count\" class=\"stats_count numeric\">" , "</span>"};
		string[] syntax_userID = {"<tr id=\"user_", "\" class=\"user"};
		string[] syntax_userName = {"title=\"", "\">"};
		
		string[] syntax_postID = {"<span onclick=\"spinit(", ","};
		string[] syntax_postPrice = {"<span class=\"price\">", "</span>"};
		//   "href=\"https://twitter.com/?status="
		//	string[] syntax_postMessage = {"href=\"https://twitter.com/?status=", "\" onclick"};
		string[] syntax_postMessage = {"https://twitter.com/intent/tweet?text=", "\" onclick"};
		
		string[] syntax_postRow = {"<tr>", "</tr>"};
		
		string[] syntax_AdwarePercent = {"Выполненные задания и процент рекламы в твиттере\" onclick=\"get_orders();\" class=\"link\">", "</span>"};
		string[] syntax_MsgLeft = {"Сегодня ост", "из"};
		string[] syntax_moneyBill = {"title=\"Финансовая статистика\" class=\"link\">", "руб."};
		string[] syntax_Followerz = {"Количество фолловеров\">", "</span>"};
//		<a href="http://klout.com/" target="_blank"><img src="http://kcdn3.klout.com/static/images/favicon.png" style="margin-bottom: -2px;" /></a>&nbsp;10.32</td>

		string[] syntax_Clout = {"<a href=\"http://klout.com/\" target=\"_blank\"><img src=\"http://kcdn3.klout.com/static/images/favicon.png\" style=\"margin-bottom: -2px;\" /></a>&nbsp;", "</td>"};
		string[] syntax_MsgId = {"\"id\":", "###"};
		string[] syntax_UsrId = {"<id>", "</id>"};
		string[] syntax_brackets = {"[", "]"};
		string[] syntax_listId = {"<list>", "</id>"};
		string[] syntax_listIdTrunc = {"<id>", "*"};
        string[] syntax_IdStr = { "tweet_id\":\"", "\"" };//"tweet_id":"257817668692889600"
		string[] syntax_IdStrList = {"id\":", ","};
		string[] syntax_OuterBr = {"},", "}"};
		
		string[] syntax_Dev_form = {"name=\"form_build_id\" id=\"", "\" value"};
		string[] syntax_remaining_hits = {"<remaining-hits type=\"integer\">", "</remaining-hits>"};
		//<input id="user_name" maxlength="20" name="user[name]" size="20" type="text" value=
		//<input id="user_name" maxlength="20" type="text" name="user[name]" value="   "\">"
		//	string[] syntax_UsrFullName = {"id=\"user_name\" maxlength=\"20\" name=\"user[name]\" size=\"20\" type=\"text\" value=\"", "\" />"};
		string[] syntax_UsrFullName = {"<input id=\"user_name\" maxlength=\"20\" type=\"text\" name=\"user[name]\" value=\"", "\">"};
		string[] syntax_Js_auth = {"<input type=\"hidden\" name=\"authenticity_token\" value=\"", "\">"};
        string[] syntax_Js_authReverse = { "<input type=\"hidden\" value=\"", "\" name=\"authenticity_token\"/>" };
		string[] syntax_followers_count = {"<followers_count>", "</followers_count>"};
		string[] syntax_post_auth = {"postAuthenticityToken\":\"", "\","};
		string[] syntax_default_profile_image = {"<default_profile_image>", "</default_profile_image>"};
		
		string[] syntax_Multiparam =  {"<#P>",  "<#>"};
		string[] syntax_MsgLimitDate = {"следующий твит Вы сможете отправить", "</div>"};
		
		string[] syntax_PHPSESSID = {"PHPSESSID=", ";"};
		string[] syntax_session_id = {"session_id=", ";"};
		string[] syntax_blogger_id = {"blogger_id=", ";"};
		//	string[] syntax_PHPSESSID = {"PHPSESSID=", ";"};
		
		
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
			"%D1%8E", "%D1%8F","%20", "%3A", "%2F", "%2C", "%21", "%40", "%3F", "%3D", "%29", "%7C", "%22", "%23"};
		
		
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
			"ю", "я", " ", ":", @"/", ",", "!", "@", "?", "=", ")", "|", "\"", "#"};
		
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
		
		string[] syntaxBlocksGlobal = {"### ", "SELECT ", " FROM ", " WHERE ", " GROUP BY ", " HAVING ", " ORDER BY ", " UNION "};
		string[] syntaxUNION = {" UNION "," UNION "};
		string[] syntaxSelect= {"### ", "DISTINCT ", "ALL ", " AS ", ",", " AS ", ","};
		string[] syntaxFrom  = {"### ", " AS ", " ON ", " AND ", " AND ", " LEFT ", " RIGHT ", " FULL ", " INNER ", " OUTER ", " JOIN "};
		
		//  between like in is not null all some any
		string[] AgregateFunctionsReservedW = {"count", "sum", "avg", "max", "min", "between", "like", "in", "is", "not", "null", "all", "some", "any"};
		static string[] ReservedWBlock = {"select", "from", "where", "group", "by", "having", "order", "union"};
		static string[] AgregateFunctions = {"count", "sum", "avg", "max", "min", "stddev", "variance"};
		static string[] ReservedW = {"between", "like", "escape", "in", "is", "not", "null", "all", "some", "any", "and", "or", "as", "asc", "desc"};
		static string[] ReservedJoinsW = {"distinct", "left", "right", "full", "inner", "outer", "join", "on"};
		//    CASE WHEN (resultTotal.hr24v=resultSumm.hr24v) THEN 1 ELSE 0 END
		static string[] caseSyntax = {"case", "when", "then", "else", "end"};
		
		
		static char[] specSymb = {',', '=', ';' , ' '};
		static char[] variousOp= {'A', 'B', 'C', 'O', 'R', 'S', 'J'};// agregate function, query block (select, from ...), case lexeme, operand, reserved words, string const, join reserved word
		static char[] reservedOp= {'A', 'B', 'C', 'R', 'S', 'J'};
		static char[] equalityOperators = {'=', '<', '>'};
		static char[] trimCharz = {'"'};
		
		public static char[] reservedOpGet
		{
			get
			{
				char[] rez = new char[reservedOp.Length];
				Array.Copy (reservedOp, rez, reservedOp.Length);
				return rez;
			}
		}
		
		public static char[] variousOpGet
		{
			get
			{
				char[] rez = new char[variousOp.Length];
				Array.Copy (variousOp, rez, variousOp.Length);
				return rez;
			}
		}
		string[] rowClosedUNION = {" UNION(", ")UNION(", ")UNION ",  " WHERE(", ")WHERE(", ")WHERE ",    " FROM(", ")FROM(", ")FROM ", ")AS ",    ")and ", ")and(", " and(",     ")or ", ")or(",  " or(",   "><", "> <", ")as ", " as ", " all("};
		string[] normClosedUNION= {" UNION (",") UNION (",") UNION "," WHERE (",") WHERE (",") WHERE ", " FROM (", ") FROM (",") FROM ", ") AS ", ") and ",") and (", " and (", ") or ",") or (", " or (",  "<>",  "<>", ") ",    " " ,  " all ("};
		#endregion
		
		#region Data
		
		public string errMessages; // куда ж без них :)
		string inlineText;     // GLOBAL CON_TEXT
		int procTextPos;       // processed part of inlineText (no more parsed for lexems)
		string previousBlock;  // previously processed block (in string form) from current syntaxBlocks array
		string[] inlineTextArr;// all [sub]Queries parsed from initiall text
		string[] syntaxBlocks; // содержит текущий набор лексем который будет обрабатываться функциями сегментации
		
		#endregion
		
		#region arrays of blocks  // public only for debug time
		public string[] selects;
		public string[] fromz;
		public string[] wherez;
		public string[] groupingz;
		public string[] havingz;
		public string[] orderz;
		#endregion
		
		public string logged;
		public string session_user;
		public string page_user;
		public string page_title ;
		public string authenticity;
		public string next_page;
		public string follower_count;
		public string following_count;
		public string expire;
		public string default_profile_image;
		
		public usrInfo[] users;
		public TweetInfo[] tweets;
		
		public string Text
		{
			set
			{
//				StringBuilder SB = new StringBuilder();
//				BlockComment = false;
//
//				string tmp;
//				foreach (string ln in value)
//				{
//					tmp = DelComment(ln);
//					SB.Append(' '+ tmp.Trim());
//				}
//				inlineText = SB.ToString();
				inlineTextArr[0] = value;
				
			}
			
			get
			{
				return inlineTextArr[0] ;
			}
		}
		
		public QueryParser()
		{
			
			errMessages = "";
			next_page = "twitter.com/login.html";
			procTextPos = 0;
			inlineTextArr = new string[sqCou];
			selects  = new string[sqCou];
			fromz    = new string[sqCou];
			wherez   = new string[sqCou];
			groupingz = new string[sqCou];
			havingz  = new string[sqCou];
			orderz   = new string[sqCou];
			
		}
		
		//<<========================= FUNCTIONS declaration =================================>>
		
		// reg key check
		
	
	// -------
		
		
		#region string utils
		
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
		
		public static bool IsValidOperandName(string s)
		{
			return !(EnumCharOccurrence(GetExpresionStruct(s), reservedOp) > 0);
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
		
		void NormalizeLexemSpacing()
		{
			StringBuilder b = new StringBuilder(inlineText);
			
			for(int i = 0; i < rowClosedUNION.Length; i++)
			{
				b.Replace(rowClosedUNION[i], normClosedUNION[i]);
			}
			inlineText = b.ToString();
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
		
		
		void NormalizeSpaces()
		{
			inlineText = NormalizeSpaces(inlineText);
		}
		
		public static string OpenBrackets(string s)
		{
			s = s.Trim();
			string orig = s;
			
			if(string.IsNullOrEmpty(s)) {return "";}
			
			#region path for 0 terminated string
			if ( s[s.Length -1] == (char)0)
			{
				s = s.Substring(0, s.Length - 1 );
			}
			#endregion
			
			if ( (s.StartsWith("(")) && (s.EndsWith(")")) )
			{
				s = s.Substring(1, s.Length - 1 );
				s = s.Trim();
				s = s.Substring(0, s.Length - 1 );
			}
			if(!CheckBracketsClosed(s))
			{
				s = orig;
			}
			
			return s;
		}
		
		public static string InserText(string s, string ins, int pos)
		{
			if(pos > s.Length) {return s;}
			string rez = s;
			string space = "";
			ins = ins.TrimEnd();
			
			segmCoord sc = QueryParser.GetSubstrSegmByCharNum(s, pos);
			if(!sc.isEmpty)
			{
				pos = sc.fin;
				space = " ";
			}
			
			rez = s.Substring(0, pos +((pos == s.Length) ? 0 : 1)) + space + ins + s.Substring(pos +((pos == s.Length) ? 0 : 1));
			
			return rez;
//			operand1.SelectionStart
		}
		
		public static segmCoord GetSubstrSegmByCharNum(string s, int pos)
		{
			segmCoord rez;
			bool spaceFound = false;
			int p = pos;
			rez.start = p;
			rez.fin = p;
			while ((p > 0) && (!spaceFound))
			{
				p -= 1;
				int index = Array.IndexOf( specSymb, s[p]);
				if ((index != -1) && (s[p] != '.'))
				{
					spaceFound = true;
				} else
				{
					rez.start = p;
				}
			}
			
			p = pos-1; // -1 works well
			spaceFound = false;
			while ((p < (s.Length -1)) && (!spaceFound))
			{
				p += 1;
				int index = Array.IndexOf( specSymb, s[p]);
				if ((index != -1) && (s[p] != '.'))
				{
					spaceFound = true;
				} else
				{
					rez.fin = p;
				}
			}
			
			return rez;
		}
		
		string[] SplitBy(string s, string separator)
		{
			string[] rez = new string[255];
			int rezCou = 0;
			byte lastBlock = 0;
			string[] com = {"###", separator, separator};
			
			initNewParse("###" + s, com);
			
			bool repeat = true;
			while (repeat)
			{
				string block = "";
				repeat = false;
				
				block = GetBlockTextByNumberInline(ref lastBlock).Trim();
				
				#region	separating loop flow
				if (syntaxBlocks[lastBlock] == separator )
				{
					repeat = true;
					lastBlock = 1;
				}
				#endregion
				
				#region	add new array item
				if ((block != "") && (block != separator))
				{
					rez[rezCou] = block;
					if (rezCou == 255)
					{
						//err handle
					} else
					{
						rezCou++;
					}
				}
				#endregion
			}
			
			Array.Resize(ref rez, rezCou);
			
			return rez;
		}
		
		public static string OpenUnionParamBrackets(string s)
		{
			s = s.Trim();
			string rez = s;
			
			if((s.StartsWith("all (")) && (s.EndsWith(")")))
			{
				rez = "all " + OpenBrackets(s.Substring(3));
			} else
			{
				rez =  OpenBrackets(s);
			}
			
			return rez;
		}
		
		#endregion string utils
		
		#region parse utils
		
		void initNewParse(string s, string[] blocks)
		{
			inlineText = s;
			procTextPos = 0;
			Array.Resize(ref syntaxBlocks, blocks.Length);
			Array.Copy(blocks, syntaxBlocks, blocks.Length);
			previousBlock = "";
		}
		
		#region обложки для GetEnclosedSegm
		
		string GetEnclosedSegmText(string s, string open, string enclose)
		{
			return GetEnclosedSegmText(s, open, enclose, false);
		}
		
		string GetEnclosedSegmText(string s, string open, string enclose, bool ignoreBrackets)
		{
			string rez = "";
			string TMPinlineText = inlineText;
			inlineText = s;
			segmCoord Region = GetEnclosedSegm(0, open, enclose, true, ignoreBrackets);
			
			if ((Region.start <= Region.fin) && (Region.start > 0))
			{
				rez = inlineText.Substring(Region.start, Region.fin - Region.start +1);
			}
			
			inlineText = TMPinlineText;
			return rez;
		}
		
		segmCoord GetEnclosedSegm(int st, string open, string enclose, bool allowSequental)
		{
			return GetEnclosedSegm(st, open, enclose, allowSequental, false);
		}
		
		#endregion
		
		segmCoord GetEnclosedSegm(int st, string open, string enclose, bool allowSequental, bool ignoreBrackets)
		{
			segmCoord rez;
			byte brackets = 0;
//			byte openedCou = 0;
			int num = st;
			int openLen = open.Length;
			int enLen = enclose.Length;
			string br = "(";
			rez.fin = 0;
			rez.start = 0;
//			open = open.ToLower();
//			enclose = enclose.ToLower();
			int  maxL = inlineText.Length;
			int serclen = openLen;
			
			while ((num >= 0)&&((num - 1 +  Math.Min(enLen,openLen) + (allowSequental ? 0 : 1)) < maxL) )
			{
				
				if ((brackets == 0) || (open.Contains(br)) || ignoreBrackets)  // not searching open and enclose in the internal statements enclosed ( ) , thus there can be local appearance of open and enclose substr that will be omitted
				{
					#region search open
					if (inlineText[num].ToString()== open[0].ToString())
					{
						string sub = allowSequental ? inlineText.Substring(num , ((num + openLen) < maxL) ? openLen : 1 ) : inlineText.Substring(num -1, openLen +1);
						if (sub == open)
						{
							rez.start = num + openLen;
							num = num + openLen;
							continue;
						}
					}
					#endregion
					
					#region search close
					if ((rez.start > 0) && (inlineText[num].ToString() == enclose[0].ToString()))
					{
						string sub = allowSequental ? inlineText.Substring(num ,  ((num + enLen) < maxL) ? enLen : 1 ) : inlineText.Substring(num -1, enLen +1);
						if ((sub== enclose) && (rez.start > 0))
						{
							rez.fin = num - 1;
							num = -3;
						}
					}
					#endregion
				}
				
				#region brackets
//				if ((num >= 0) && (ignoreBrackets != true))
//				{
//					if (inlineText[num] == '(')
//					{
//						brackets++;
				////						MessageBox.Show(brackets.ToString());
//					}
//
//					if (inlineText[num] == ')')
//					{
//						if (brackets==0)
//						{
//							//errhandling
//							errMessages = string.Concat( errMessages, " closed brackets err at:  ", inlineText.Substring(num - 20, 21 ) );
//							num = -3;
//						} else	 {brackets--;}
				////						MessageBox.Show(brackets.ToString());
//					}
//				}
				#endregion
				
				num++;
			}
			
			return rez;
		}
		
		#region обложки для GetBlockTextByNumber
		
		string GetBlockTextByNumberInline(ref byte n)
		{
			return GetBlockTextByNumber(ref n, (byte)syntaxBlocks.Length, true, 1);
		}
		
		string GetBlockTextByNumberInline(ref byte n, byte finReg)
		{
			return GetBlockTextByNumber(ref n, finReg, true, 1);
		}
		
		string GetBlockTextByNumber(ref byte n)
		{
			return GetBlockTextByNumber(ref n, (byte)syntaxBlocks.Length, false, 1);
		}
		
		#endregion
		
		string GetBlockTextByNumber(ref byte n, byte finReg, bool inline, byte offset)
		{
			segmCoord Region;
			byte next = 1;
			Region.start = 0;
			Region.fin = 0;
			string rez ="";
			
			
			while ((Region.fin == 0) && ((n+next) < finReg))
			{
				Region = GetEnclosedSegm(procTextPos, syntaxBlocks[n], syntaxBlocks[n+next], inline);
				next++;
				if (errMessages != "")
				{
//					errMessages = errMessages + " !!!at Block " +syntaxBlocks[n];
//					break;
				}
			}
			
			if (Region.fin != 0)
			{
				if (Region.start <= Region.fin)
				{
					rez = inlineText.Substring(Region.start, Region.fin - Region.start +1);
				} else
				{rez = "";}
				previousBlock = syntaxBlocks[n];
				n = (byte)(n+next -1);
				
				procTextPos = Region.fin + offset;  // wery important and crock
				
			} else
			{
				previousBlock = syntaxBlocks[n];
				rez = "";
				n = 0; // coma (and other substr) repeating issue
			}
			
			return rez;
		}
		
		#region Обложки для GetExpresionStruct
		public static string GetExpresionStruct(string s)
		{
			string[] OpArr;
			return GetExpresionStruct(s, out OpArr, false, true);
		}
		
		public static string GetExpresionStruct(string s, out string[] OpArr)
		{
			return GetExpresionStruct(s, out OpArr, false, true);
		}
		
		public static string GetExpresionStruct(string s, out string[] OpArr, bool opSpacing)
		{
			return GetExpresionStruct(s, out OpArr, opSpacing, true);
		}
		
		public static string GetExpresionStructUntyped(string s, out string[] OpArr)
		{
			return GetExpresionStruct(s, out OpArr, false, false);
		}
		
		public static string GetExpresionStructUntyped(string s)
		{
			string[] OpArr;
			return GetExpresionStruct(s, out OpArr, false, false);
		}
		
		#endregion
		
		/// <summary>
		/// function not trimming input string;
		/// if string ends by space, last item in OpArr will be null !!!
		/// </summary>
		public static string GetExpresionStruct(string s, out string[] OpArr, bool opSpacing , bool useTypification)
		{
			if (s == null)
			{
				OpArr = new string[0];
				return "";
			}
			
//			s = s.ToLower();
			string rez = "";
			bool isPrevSpec = true;
			int OperCou = 0;
			
			string[] OperandArr = new string[(int)(s.Length / 2)+1]; // in incredible case there can be a half-length number of operands, because else part of characters will be a spec symbols to separate it
			bool strConstOpened = false;
			
			#region character-oriented processing for separation on the operands
			for (int i = 0; i < s.Length; i++ )
			{
				int index = Array.IndexOf( specSymb, s[i]);
				
				#region string constants
				if ((strConstOpened == true) && (s[i] != (char)39))
				{
					if (isPrevSpec)
					{
						rez += "S";
						isPrevSpec = false;
					}
					OperandArr[OperCou] += s[i];
					continue;
				}
				
				if (s[i] == (char)39)
				{
					strConstOpened = !strConstOpened;
				}
				#endregion
				
				if (index == -1)
				{
					#region	Operand proc
					if (isPrevSpec)   // current char is a start of new operand, so operand mask added to the struct
					{
						rez += "O";
						isPrevSpec = false;
					}
					OperandArr[OperCou] += s[i];
					OperandArr[OperCou] = OperandArr[OperCou].Trim();
					#endregion
				} else
				{
					#region Spec symbol separating
					
					if ((s[i] != ' ') || (opSpacing)) // space not including to the expresion structure
					{
						rez += s[i];
					}
					
					if (isPrevSpec == false) // when couple spec symbols go inline ( >=, <>, ...) there no (new) operands between
					{
						OperCou++;
					}
					
					isPrevSpec = true;
					#endregion
				}
			}
			#endregion
			
			OpArr = OperandArr;
			Array.Resize(ref OpArr, OperCou+1); // OperCou+1
			
			int opNum = -1;
			string tempRez = ""; // string is read only ( immutable	)
			
			if(useTypification)
			{
				#region search SQL operators among operands
				for (int i = 0; i < rez.Length; i++ )
				{
					char curr = rez[i];
					
					if (curr == 'S') // string constant not separating on operands (substrings divided by spaces, etc) and appear as single operand. example: 'this is a string constant' its wouldn't be a sequence of string operands 'this', 'is', 'a'...
					{
						opNum++;
					}
					
					if (curr == 'O')
					{
						opNum++;
						
						#region Agregate Functions
						int index = Array.IndexOf( AgregateFunctions, OperandArr[opNum]);
						if (index != -1)
						{
							curr = 'A';
						}
						#endregion
						
						#region Reserved Words
						index = Array.IndexOf( ReservedW, OperandArr[opNum]);
						if (index != -1)
						{
							curr = 'R';
						}
						#endregion
						
						#region case Syntax
						index = Array.IndexOf( caseSyntax, OperandArr[opNum]);
						if (index != -1)
						{
							curr = 'C';
						}
						#endregion
						
						#region Blocks Syntax
						index = Array.IndexOf( ReservedWBlock, OperandArr[opNum]);
						if (index != -1)
						{
							curr = 'B';
						}
						#endregion
						
						#region Blocks Joins
						index = Array.IndexOf( ReservedJoinsW, OperandArr[opNum]);
						if (index != -1)
						{
							curr = 'J';
						}
						#endregion
						
					}
					tempRez += curr;
					
				}
				#endregion
				
			} else
			{
				tempRez = rez;
			}
			
			return tempRez;
		}
		
		public static bool ContainsExpr(string s, string sub)
		{
			bool rez = false;
			string[] sArr;
			string[] subArr;
			string sExpr = GetExpresionStruct(s, out sArr);
			string subExpr = GetExpresionStruct(sub, out subArr);
			
			if (sExpr.Contains(subExpr))
			{
				int idx = 0;
				while (idx != -1)
				{
					
					idx = Array.IndexOf(sArr, subArr[0], idx);
					bool opMatch = true;
					if (idx >= 0)
					{
						for (int i = 0; i< subArr.Length; i++)
						{
							if (subArr[i] != null)
							{
								if ((sArr[idx + i] != subArr[i]) )
								{
									opMatch = false;
								}
							}
						}
						
						if (opMatch) {idx = -1; rez = true;} else {idx++;}
					}
					
				}

			}
			return rez;
		}
		
		public static string Rename(string s, string sub, string newn)
		{
			string rez = s;
			string[] sArr;
			string[] subArr;
			string[] newArr;
			string sExpr = GetExpresionStruct(s, out sArr);
			string subExpr = GetExpresionStruct(sub, out subArr);
			GetExpresionStruct(newn, out newArr);
			
			if (sExpr.Contains(subExpr))
			{
				int idx = 0;
				while (idx != -1)
				{
					
					idx = Array.IndexOf(sArr, subArr[0], idx);
					bool opMatch = true;
					if (idx >= 0)
					{
						for (int i = 0; i< subArr.Length; i++)
						{
							if (subArr[i] != null)
							{
								if ((sArr[idx + i] != subArr[i]))
								{
									opMatch = false;
								}

							}
						}
						
						string sttm = sExpr.Substring(GetOperandExprIdx(sExpr, idx ), subExpr.Length);
						if(sttm != subExpr)
						{
							opMatch = false;
						}
						
//						subExpr += "@"; // commented 23-10-10 because cause onlu one renaming in current s; why did it appear here i dont know
						
						if (opMatch)
						{
							for (int i = 0; i< subArr.Length; i++)
							{
								if (newArr[i] != null)
								{
									sArr[idx + i] = newArr[i];
								}
							}
							rez = Merge(sExpr, sArr);
//							idx = -1;
						} else
						{
							idx++;
						}
						
//						idx++;
					}
					
				}
				
			}
			
			
			
			return rez;
		}
		
		public static string Merge(string expr, string[] operands)
		{
			string rez ="";
			
			int opCou = -1;
			
			for (int i = 0; i < expr.Length; i++)
			{
				int index = Array.IndexOf(variousOp, expr[i]);
				if (index != -1)
				{
					opCou++;
					
					rez +=  operands[opCou].Trim() ;
				} else
				{
					
					rez += expr[i] + ((expr[i] == ';')? " " : "");
				}
			}
			
			return rez;
		}
		
		// we have array of operands(lexemes) that appear in expr, and needed to find an lexeme index in expr, that associated with item index in array;  lexeme: 'O', 'R', 'B'...
		public static int GetOperandExprIdx(string expr, int opnum)
		{
			int opCou = -1;
			for (int i = 0; i < expr.Length; i++)
			{
				int index = Array.IndexOf(variousOp, expr[i]);
				if (index != -1) {opCou++;}
				if (opCou == opnum) {return i;}
			}
			return -1;
		}
		
		public static int GetOperandIdxByExprIdx(string expr, int ExprIdx)
		{
			int opCou = -1;
			for (int i = 0; i < expr.Length; i++)
			{
				int index = Array.IndexOf(variousOp, expr[i]);
				if (index != -1) {opCou++;}
				if (i == ExprIdx) {return opCou;}
			}
			return -1;
		}
		
		public static string GetConditionStruct(string expr, string[] OpArr, out string[] EqOpArr, out string ConditionStr)
		{
			EqOpArr = new string[OpArr.Length + 2];
			segmCoord[] opSegmA = new segmCoord[EqOpArr.Length];
			bool openBr = false;
			bool prevSpec = false;
			int opCou = -1;
			int specOpCou = -1;
			int bracketCou = 0;
			string specExprMap = "";
			ConditionStr = "";
			
			for(int i=0; i < expr.Length; i++)
			{
				int index = Array.IndexOf(variousOp, expr[i]);
				if (index != -1) {opCou++;}
				
				#region brackets
				if(expr[i] == '(')
				{
					bracketCou++;
					openBr = true;
				}
				if(expr[i] == ')')
				{
					bracketCou--;
					openBr = (bracketCou > 0);
				}
				#endregion
				
				if (  ((Array.IndexOf(equalityOperators, expr[i]) == -1) && (expr[i] != 'R')) || (openBr)  )
				{
					#region add new operand if needed
					if((prevSpec) || (specExprMap == ""))
					{
						specExprMap += "O";
						specOpCou++;
						opSegmA[specOpCou].start = (index != -1) ? opCou : -1; // case of ' start begin from previous operand, not correct
						prevSpec = false;
					}
					#endregion
					if((opSegmA[specOpCou].start == -1) &&(index != -1))
					{
						opSegmA[specOpCou].start = opCou;
					}
					opSegmA[specOpCou].fin = opCou;
					EqOpArr[specOpCou] += expr[i];
				}
				
				if(!openBr)
				{
					#region check for Condition operands
					
					if ( (expr[i] == 'R') || (Array.IndexOf(equalityOperators, expr[i]) >= 0) )
					{
						if (! prevSpec)
						{
							specExprMap += "E";
						}
						prevSpec = true;
						#region assemble Condition substrings
						if (expr[i] == 'R')
						{
							ConditionStr += " " + OpArr[opCou]+" ";
						} else
						{
							ConditionStr += "" + expr[i].ToString();
						}
						#endregion
					}
					#endregion
				}
			}
			
			string tmp = ConditionStr +" \n"; // for debug only
			#region  Merge operand strings
			string[] mergeOp = new string[OpArr.Length + 2];
			for(int i = 0; i<=specOpCou; i++)
			{
				int len = (opSegmA[i].fin - opSegmA[i].start) + 1;
				Array.Copy(OpArr, opSegmA[i].start, mergeOp, 0, len);
				EqOpArr[i] = Merge(EqOpArr[i], mergeOp);
				tmp += EqOpArr[i]+" \n";
			}
			#endregion
			
			Array.Resize(ref EqOpArr, specOpCou+1);
			return specExprMap;
//			return tmp;
		}
		
		public static string LeftToRightPath(string TreeFullPath)
		{
			string rez = "";
			string[] oparr;
			string expr = QueryParser.GetExpresionStruct(TreeFullPath.Trim(), out oparr);
			int opCou = -1;
			int opCouAdded = opCou;
			bool prevQuery = false;
			string separator = "";
			int dropOpr = -1;
			
			oparr[0] = "Query constructor";
			
			for(int i=0; i < expr.Length; i++)
			{
				int index = Array.IndexOf(variousOp, expr[i]);
				if (index != -1) {opCou++;}
				
				if(opCou == dropOpr)
				{
					prevQuery = false;
				}
				
				if((opCouAdded != opCou) && (!prevQuery))
				{
					string queryNumber = "";
					
					if((oparr[opCou] == "query") && (IsNumber(oparr[opCou+1])) )
					{
						dropOpr = opCou+2;
						prevQuery = true;
						queryNumber = " "+oparr[opCou+1];
					}
					rez = ( !prevQuery ? (opCou ==0 ? oparr[opCou] : oparr[opCou].ToUpper()) : "Query") +queryNumber +separator+ rez;
					opCouAdded = opCou;
					
					separator = prevQuery  ? " /" : " |";
				}
				
				
			}
			
			return rez;
		}
		
		#endregion parse utils
		
		
		public string GetPageAuthenticity(string html)
		{
			inlineTextArr[0] = html;
			return GetPageAuthenticity();
		}
		
		public string GetPageAuthenticity()
		{
			byte lastBlock = 0;
			procTextPos = 0;
			inlineText =  (string)inlineTextArr[0].Clone();
			
			lastBlock = 0;
			syntaxBlocks = syntax_authenticity;
			string authenticity = GetBlockTextByNumberInline(ref lastBlock);
			
			if(authenticity.Trim() == "" || authenticity.Length < 6 )
			{
				procTextPos = 0;
				lastBlock = 0;
				syntaxBlocks = syntax_authenticityNS;
				authenticity = GetBlockTextByNumberInline(ref lastBlock);
			}
			
			return authenticity;
		}
		
		public void ParseTweets()
		{
			timer t = new timer();
			#region init
			byte lastBlock = 0;
			procTextPos = 0;
			inlineText =  (string)inlineTextArr[0].Clone();

			#endregion
			
			#region parse table rows
			
			lastBlock = 0;
			procTextPos = 0;
			syntaxBlocks = syntax_postRow;
			string[] rowz = new string[500];
			
			int reblock = 1;
			int rowcou = 0;
			while(reblock == 1)
			{
				reblock = 0;
				lastBlock = 0;
				rowz[rowcou] = GetBlockTextByNumberInline(ref lastBlock);
				reblock += lastBlock;
				if(!rowz[rowcou].Contains("class=\"hint\">Facebook") && !rowz[rowcou].Contains("class=\"hint\">Копирайтус"))
				{
					rowcou++;
				}
			}
			
			#endregion
			
			tweets = new TweetInfo[500];
			
			int tcou = -1;
			
			for(int i = 0; i < rowcou; i ++)
			{
				tcou++;
				reblock = 0;
				lastBlock = 0;
				procTextPos = 0;
				inlineText = rowz[i];
				
				procTextPos = 0;
				syntaxBlocks = syntax_postID;
				tweets[tcou].id = GetBlockTextByNumberInline(ref lastBlock);
				reblock += lastBlock;
				tweets[tcou].isRetweet = false;
				
				#region secret Code
				string secretCode = "";
				
				int lapkaCou = 0;
				while(lapkaCou < 2 && (procTextPos+1) < inlineText.Length)
				{
					procTextPos++;
					
					if(inlineText[procTextPos] == (char)39)
					{
						lapkaCou++;
					} else
					{
						if(lapkaCou == 1)
						{
							secretCode += inlineText[procTextPos];
						}
					}
				}
				tweets[tcou].secretCode = secretCode;
				
				#endregion
				
				if(inlineText.Contains("рекламный ретвит"))
				{
					tweets[tcou].isRetweet = true;
				}
				
				procTextPos = 0;
				lastBlock = 0;
				syntaxBlocks = syntax_postPrice;
				tweets[tcou].price = GetBlockTextByNumberInline(ref lastBlock);
				tweets[tcou].price  = tweets[tcou].price.Replace("&nbsp;руб.", "");
				reblock += lastBlock;
				
				procTextPos = 0;
				lastBlock = 0;
				syntaxBlocks = syntax_postMessage;
				tweets[tcou].text = GetBlockTextByNumberInline(ref lastBlock);
				reblock += lastBlock;
				
//				if(reblock !=3 ) // this is a retwwet
//				{tcou--;}
				
			}
			
			Array.Resize(ref tweets, tcou);
			
//			MessageBox.Show("logged "+ logged+ " folwer "+ follower_count + "folwing" + following_count + "  session_user  " + session_user+ " page_user " +page_user + " page_title " + page_title  + t.final("     time parse   ") + "\n"+ authenticity +"\n" +next_page);
			
			
		}
		
		
		
		public bool ChechTwitterLogin(string usrName)
		{
			#region init
			byte lastBlock = 0;
			procTextPos = 0;
			inlineText =  (string)inlineTextArr[0].Clone();
			#endregion
			
			logged = "notLogged";
//			if (inlineText.Contains("span id=\"session\" class=\"loggedin\""))
//			{
//				logged = "y";
//			}
//
//
			lastBlock = 0;
			syntaxBlocks = syntax_PostAuthToken;
			authenticity = "";
			authenticity = GetBlockTextByNumberInline(ref lastBlock);
//
            if (authenticity.Length > 42)
            {
                authenticity = "";
            }
               

			inlineTextArr[0] = "";
			
			bool rez = inlineText.Contains(usrName);
			
			return rez; //((session_user.Trim().ToLower() == usrName.ToLower()) && (logged.Trim().ToLower() == "y"));
			
		}
		
		public string GetTwitterMsgID()
		{
			#region init
			byte lastBlock = 0;
			procTextPos = 0;
			inlineText =  (string)inlineTextArr[0].Clone();
			#endregion
			
			lastBlock = 0;
			
			string	MsgID = "";
			syntaxBlocks = syntax_IdStr;
			
			
			MsgID = GetBlockTextByNumberInline(ref lastBlock);
			lastBlock = 0;
			string MsgID2 = GetBlockTextByNumberInline(ref lastBlock);
			lastBlock = 0;
			string MsgID3 = GetBlockTextByNumberInline(ref lastBlock);
			
			if(MsgID.Length < MsgID2.Length)
			{
				MsgID = MsgID2;
			}
			
			if(MsgID.Length < MsgID3.Length)
			{
				MsgID = MsgID3;
			}
			
			
//			if(MsgID.Length >3){
//				MsgID = MsgID.Substring(0, MsgID.Length -3);}
//			MsgID.Trim(',');
			
			inlineTextArr[0] = "";
			return MsgID;

		}
		
		public string GetTwitterUserID()
		{
			#region init
			byte lastBlock = 0;
			procTextPos = 0;
			inlineText =  (string)inlineTextArr[0].Clone();
			#endregion
			
			lastBlock = 0;
			syntaxBlocks = syntax_UsrId;
			string	MsgID = "";
			MsgID = GetBlockTextByNumberInline(ref lastBlock);
			
			lastBlock = 0;
			syntaxBlocks = syntax_default_profile_image;
			
			default_profile_image = GetBlockTextByNumberInline(ref lastBlock);
			
			//inlineTextArr[0] = "";
			return MsgID;

		}
		
		public string GetTwitterJs_auth()
		{
			#region init
			byte lastBlock = 0;
			procTextPos = 0;
			inlineText =  (string)inlineTextArr[0].Clone();
			#endregion
			
			lastBlock = 0;
			syntaxBlocks = syntax_Js_auth;
			string	MsgID = "";
			MsgID = GetBlockTextByNumberInline(ref lastBlock);

            if (string.IsNullOrEmpty(MsgID))
            {
                procTextPos = 0;
                lastBlock = 0;
                MsgID = "";

                syntaxBlocks = syntax_Js_authReverse;
                MsgID = GetBlockTextByNumberInline(ref lastBlock);

            }
			
			//inlineTextArr[0] = "";
			return MsgID;

		}
		
		public string GetTwitter_Post_auth()
		{
			#region init
			byte lastBlock = 0;
			procTextPos = 0;
			inlineText =  (string)inlineTextArr[0].Clone();
			#endregion
			
			lastBlock = 0;
			syntaxBlocks = syntax_post_auth;
			string	MsgID = "";
			MsgID = GetBlockTextByNumberInline(ref lastBlock);
			
			//inlineTextArr[0] = "";
			return MsgID;

		}

        public string GetTextBlock(string srs, string[] blocks)
        {
            #region init
            byte lastBlock = 0;
            procTextPos = 0;
            inlineText = srs;
            #endregion

            lastBlock = 0;
            syntaxBlocks = blocks;
            string MsgID = "";
            MsgID = GetBlockTextByNumberInline(ref lastBlock);

            //inlineTextArr[0] = "";
            return MsgID;

        }

        public string GetUserListText(string input)
		{
			#region init
			byte lastBlock = 0;
			procTextPos = 0;
			inlineText =  input;
			#endregion
			
			lastBlock = 0;
			syntaxBlocks = syntax_brackets;
			string	MsgID = "";
			MsgID = GetBlockTextByNumberInline(ref lastBlock);
			
			//inlineTextArr[0] = "";
			return MsgID;

		}
		
		public string GetUserListIdz(string input)
		{
			#region init
			byte lastBlock = 0;
			procTextPos = 0;
			inlineText =  input;
			#endregion
			
			lastBlock = 0;
			syntaxBlocks = syntax_listId;
			string	MsgID = "";
			MsgID = GetBlockTextByNumberInline(ref lastBlock);
			
			lastBlock = 0;
			procTextPos = 0;
			inlineText =  MsgID + "*";
			
			lastBlock = 0;
			syntaxBlocks = syntax_listIdTrunc;
			MsgID = GetBlockTextByNumberInline(ref lastBlock);
			//inlineTextArr[0] = "";
			return MsgID;

		}
		
		public string GetIdStr(string input)
		{
			#region init
			byte lastBlock = 0;
			procTextPos = 0;
			inlineText =  input;
			#endregion
			
			
			
			string	MsgID = "";
			syntaxBlocks = syntax_OuterBr;
			MsgID = GetBlockTextByNumberInline(ref lastBlock);
			
			
			lastBlock = 0;
			procTextPos = 0;
			inlineText =  MsgID ;
			
			
			
			syntaxBlocks = syntax_IdStrList;
			MsgID = GetBlockTextByNumberInline(ref lastBlock);
			
			
			
			//inlineTextArr[0] = "";
			return MsgID;
		}
		
		//<remaining-hits type="integer">0</remaining-hits>
		public string GetRateLimit()
		{
			#region init
			byte lastBlock = 0;
			procTextPos = 0;
			inlineText =  (string)inlineTextArr[0].Clone();
			#endregion
			
			lastBlock = 0;
			syntaxBlocks = syntax_remaining_hits;
			string	MsgID = "";
			MsgID = GetBlockTextByNumberInline(ref lastBlock);
			
			//inlineTextArr[0] = "";
			return MsgID;

		}
		
		public string GetTwitterUsrFullName()
		{
			#region init
			byte lastBlock = 0;
			procTextPos = 0;
			inlineText =  (string)inlineTextArr[0].Clone();
			#endregion
			
			lastBlock = 0;
			syntaxBlocks = syntax_UsrFullName;
			string	MsgID = "";
			MsgID = GetBlockTextByNumberInline(ref lastBlock);
			
			//inlineTextArr[0] = "";
			return MsgID;

		}
		
		public string GetTwitterUserInfo(ref string folowCou)
		{
			#region init
			byte lastBlock = 0;
			procTextPos = 0;
			inlineText =  (string)inlineTextArr[0].Clone();
			#endregion
			
			lastBlock = 0;
			syntaxBlocks = syntax_followers_count;
			string	MsgID = "";
			MsgID = GetBlockTextByNumberInline(ref lastBlock);
			if(!string.IsNullOrEmpty(MsgID)){folowCou = MsgID;}
			
			inlineTextArr[0] = "";
			return MsgID;

		}
		
		public string GetDevFormID()
		{
			#region init
			byte lastBlock = 0;
			procTextPos = 0;
			inlineText =  (string)inlineTextArr[0].Clone();
			#endregion
			
			lastBlock = 0;
			syntaxBlocks = syntax_Dev_form;
			string	MsgID = "";
			MsgID = GetBlockTextByNumberInline(ref lastBlock);
			
			//inlineTextArr[0] = "";
			return MsgID;

		}
		
		public string ExtractTwiteMsg()
		{
			#region init
			byte lastBlock = 0;
			procTextPos = 0;
			inlineText =  (string)inlineTextArr[0].Clone();
			#endregion
			
			lastBlock = 0;
			syntaxBlocks = syntax_MsgLimitDate;
			string	Msg = "";
			Msg = GetBlockTextByNumberInline(ref lastBlock);
			
			if(Msg == ""){Msg = inlineText;}
			else
			{
				Msg = syntaxBlocks[0] + Msg;
			}
			if(inlineText.Trim() ==""){Msg = "ответ сетвера отсутствует";}
			
			return Msg;
			//следующий твит Вы сможете отправить 06 января в 19:29</div>
		}
		
		public MultiparamString ParseTwiteCookies(string cookie)
		{
			MultiparamString rez = new MultiparamString();
			
			#region init
			byte lastBlock = 0;
			procTextPos = 0;
			inlineText = cookie;
			#endregion
			
//			string[] syntax_PHPSESSID = {"PHPSESSID=", ";"};
//		string[] syntax_session_id = {"session_id=", ";"};
//		string[] syntax_blogger_id = {"blogger_id=", ";"};
			
			lastBlock = 0;
			syntaxBlocks = syntax_PHPSESSID;
			string	part = "";
			part = GetBlockTextByNumberInline(ref lastBlock);
			
			rez.AddParam("PHPSESSID", part);
			
			lastBlock = 0;
			syntaxBlocks = syntax_session_id;
			part = "";
			part = GetBlockTextByNumberInline(ref lastBlock);
			
			rez.AddParam("session_id", part);
			
			lastBlock = 0;
			syntaxBlocks = syntax_blogger_id;
			part = "";
			part = GetBlockTextByNumberInline(ref lastBlock);
			
			rez.AddParam("blogger_id", part);
			
			return rez;
			
		}
		
	}
	
	public class MultiparamString
	{
		int paramCou;
		string[] paramNames;
		string[] paramValues;
		
		public MultiparamString()
		{
			init(30);
		}
		
		void init(int len)
		{
			paramNames = new string[len];
			paramValues = new string[len];
		}
		
		int NewParam(string name)
		{
			int rez = paramCou;
			paramNames[rez] = name;
			paramCou++;
			
			return rez;
		}
		
		/// <summary>
		/// if such param already exist new value will be added to prev separated by coma
		/// </summary>
		public void AddParam(string name, string val)
		{
			int idx = GetParamIdx(name);
			string sep = ",";
			
			if(idx < 0)
			{
				idx = NewParam(name);
				sep = "";
			}
			
			paramValues[idx]+= sep + val;
			
		}
		
		public string GetParam(string name)
		{
			string rez = "";
			
			int idx = GetParamIdx(name);
			
			if(idx >= 0)
			{
				rez = paramValues[idx];
			}
			
			return rez;
		}
		
		int GetParamIdx(string name)
		{
			int rez =  Array.IndexOf(paramNames, name);
			
			return rez;
		}
		
		public string Save()
		{
			string rez = "";
			
			for(int i = 0; i < paramCou; i++)
			{
				rez += paramNames[i] + "|" +  paramValues[i] + "#";
			}
			
			
			return rez;
		}
		
		public void Parse(string text)
		{
			Clear();
			
			string[] spl = text.Split('#');
			
			foreach(string s in spl)
			{
				string[] np = s.Split('|');
				
				if(np.Length == 2)
				{
					paramNames[paramCou] = np[0];
					paramValues[paramCou] = np[1];
					paramCou++;
				}
			}
			
		}
		
		public void Clear()
		{
			if(paramCou > 0)
			{
				Array.Clear(paramNames, 0, paramCou);
				Array.Clear(paramValues, 0, paramCou);
			}
			
			paramCou = 0;
		}
		
		
		
	}
	
	
	public class rass
	{
		public ulong exponenta;
		public ulong modul_n;
		public ulong d_hide;
		
		public rass()
		{
		}
		
		public rass(ulong e, ulong d, ulong n)
		{
			exponenta = e;
			modul_n = n;
			d_hide = d;
		}
		
		public ulong encrypt(ulong msg)
		{
			return encrypt(msg, exponenta, modul_n);
		}
		
		public ulong encrypt(ulong msg, ulong publicE, ulong en)
		{
			return modpow(msg, publicE, en);
		}
		
		public ulong decrypt(ulong msg)
		{
			return modpow( msg, d_hide, modul_n);
		}
		
		
		ulong  modpow(ulong x, ulong e, ulong n)
		{
			ulong r = 1;
			while( e > 0 )
			{
				if( (e%2)==1 ) { r = (r*x) % n;}
				e = e/2;
				x = (x * x) % n;
			}
			return r;
		}
		
	}

}
