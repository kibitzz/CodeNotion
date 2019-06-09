using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SqlBuilderClasses;
using basicClasses;
using basicClasses.models.WEB_api;

namespace twite
{
    //public struct Prop
    //{
    //    bool isObject;
    //    public bool ReferToObj
    //    {
    //        get
    //        {
    //            return isObject;
    //        }
    //    }

    //    public string propName;
    //    public string propVal;
    //    int ObjIdx;
    //    public int propObjIdx
    //    {
    //        get
    //        {
    //            return ObjIdx;
    //        }

    //        set
    //        {
    //            isObject = true;
    //            ObjIdx = value;
    //        }
    //    }
    //}

    //public class JsonObject
    //{
    //    // set to true when it was started by [
    //    bool isArray;

    //    int propCou;
    //    int currPropIdx;
    //    bool propOpened;
    //    bool stringOpened;
    //    byte openedQuotes;
    //    public string PageText;

    //    public bool isStringOpened
    //    {
    //        get
    //        {
    //            return stringOpened;// (openedQuotes >0);
    //        }
    //    }


    //    Prop[] props;
    //    JsonObject Parent;
    //    List<JsonObject> subobjects;

    //    public JsonObject[] objectsArray
    //    {
    //        get
    //        {
    //            return subobjects.ToArray();
    //        }
    //    }

    //    public JsonObject()
    //    {
    //        init();
    //    }

    //    public JsonObject(bool isArray_)
    //    {
    //        init();
    //        isArray = isArray_;
    //        props = new Prop[1200];
    //    }

    //    void init()
    //    {
    //        Logger.init();

    //        isArray = false;
    //        propOpened = false;
    //        stringOpened = false;
    //        currPropIdx = -1;
    //        openedQuotes = 0;

    //        subobjects = new List<JsonObject>();
    //        props = new Prop[100];

    //    }

    //    #region  Parsing Methods

    //    /// <summary>
    //    /// use in case when : raise
    //    /// </summary>
    //    /// <param name="pname"></param>
    //    public void AddProp(string pname)
    //    {
    //        if (!propOpened)
    //        {
    //            pname = pname.Trim('"');

    //            currPropIdx = propCou;
    //            props[currPropIdx].propName = pname;
    //            props[currPropIdx].propVal = "";

    //            propOpened = true;
    //            openedQuotes = 0;
    //            stringOpened = false;

    //            propCou++;
    //        }

    //    }

    //    public bool StartStringVal(char nextChar)
    //    {
    //        if (propOpened)
    //        {
    //            if (openedQuotes == 0)
    //            {
    //                stringOpened = true;
    //            }

    //            openedQuotes++;

    //            if ((nextChar == ',' || nextChar == '}' || nextChar == ']') && (openedQuotes % 2) == 0)
    //            {
    //                stringOpened = !stringOpened;
    //            }



    //            //openedQuotes++;

    //        }

    //        return propOpened;
    //    }

    //    /// <summary>
    //    /// use in case when , raise
    //    /// </summary>
    //    /// <param name="val"></param>
    //    public void Setval(string val)
    //    {
    //        if (propOpened)
    //        {
    //            props[currPropIdx].propVal += val;

    //            if (!stringOpened)
    //            {
    //                propOpened = false;
    //            }
    //        }
    //        else if (isArray && !string.IsNullOrEmpty(val))
    //        {
    //            props[propCou].propVal = val;
    //            propCou++;
    //        }

    //    }

    //    /// <summary>
    //    /// use in case when some of brackets raise in text
    //    /// </summary>
    //    /// <param name="val"></param>
    //    public JsonObject AddObject(char bracket)
    //    {
    //        JsonObject newSub = null;

    //        if (bracket == '[' || bracket == '{')
    //        {

    //            newSub = new JsonObject((bracket == '['));
    //            newSub.Parent = this;
    //            subobjects.Add(newSub);
    //            if (propOpened)
    //            {
    //                props[currPropIdx].propObjIdx = subobjects.Count - 1;
    //            }
    //            else
    //            {
    //                isArray = true;
    //            }

    //        }
    //        else if (bracket == '}' || bracket == ']')
    //        {
    //            propOpened = false;
    //            newSub = Parent;
    //        }

    //        return newSub;
    //    }

    //    #endregion


    //    public void ConcatenateObjects(JsonObject[] arr)
    //    {
    //        if (isArray)
    //        {
    //            subobjects.AddRange(arr);
    //        }
    //    }

    //    public JsonObject GetObject(string path)
    //    {
    //        JsonObject rez;
    //        string[] nameArr = path.Split('.');
    //        rez = GetObject(nameArr, 0);

    //        return rez;
    //    }

    //    public JsonObject GetObject(string[] propNames, int idx)
    //    {
    //        JsonObject rez = new JsonObject();

    //        if (propCou == 0 && subobjects.Count == 1)
    //        {
    //            rez = subobjects[0].GetObject(propNames, idx);
    //        }

    //        for (int i = 0; i < propCou; i++)
    //        {
    //            if (props[i].propName == propNames[idx])
    //            {

    //                if (props[i].ReferToObj)
    //                {
    //                    if ((idx + 1) < propNames.Length)
    //                    {
    //                        rez = subobjects[props[i].propObjIdx].GetObject(propNames, idx + 1);
    //                    }
    //                    else
    //                    {
    //                        rez = subobjects[props[i].propObjIdx];
    //                    }
    //                }

    //                break;
    //            }
    //        }

    //        return rez;
    //    }


    //    public string GetPropVal(string[] propNames, int idx)
    //    {
    //        string rez = "";

    //        for (int i = 0; i < propCou; i++)
    //        {
    //            if (props[i].propName == propNames[idx])
    //            {

    //                if (props[i].ReferToObj)
    //                {
    //                    if ((idx + 1) < propNames.Length)
    //                    {
    //                        rez = subobjects[props[i].propObjIdx].GetPropVal(propNames, idx + 1);
    //                    }

    //                }
    //                else
    //                {
    //                    rez = props[i].propVal;
    //                }



    //                break;
    //            }
    //        }

    //        return rez;
    //    }

    //    public string GetPropVal(string propName)
    //    {
    //        string rez = "";

    //        string[] nameArr = propName.Split('.');

    //        rez = JsonParser.TrimQuotes(GetPropVal(nameArr, 0));

    //        return rez;
    //    }


    //    #region debug / visualization

    //    void BuildTree(TreeNode tn)
    //    {
    //        if (!isArray)
    //        {

    //            for (int i = 0; i < propCou; i++)
    //            {

    //                TreeNode tmp = new TreeNode(props[i].propName + ((props[i].ReferToObj) ? "" : (" : " + props[i].propVal)));

    //                if (props[i].ReferToObj)
    //                {
    //                    subobjects[props[i].propObjIdx].BuildTree(tmp);
    //                }

    //                //subobjects[props[i].propObjIdx].BuildTree 
    //                tn.Nodes.Add(tmp);
    //            }
    //        }
    //        else
    //        {
    //            tn.Text += "  " + subobjects.Count.ToString();
    //            foreach (JsonObject jso in subobjects)
    //            {
    //                TreeNode tmp = new TreeNode("*");
    //                jso.BuildTree(tmp);
    //                tn.Nodes.Add(tmp);
    //            }

    //            for (int i = 0; i < propCou; i++)
    //            {
    //                TreeNode tmp = new TreeNode(props[i].propVal);

    //                tn.Nodes.Add(tmp);
    //            }
    //        }
    //    }

    //    public TreeNode GetDebugTree()
    //    {
    //        TreeNode rez = new TreeNode("-=#Root#=-");

    //        BuildTree(rez);

    //        return rez;
    //    }

    //    #endregion

    //   public void BuildTreeopis(opis thi)
    //    {                   
    //        if (!isArray)
    //        {

    //            for (int i = 0; i < propCou; i++)
    //            {
    //                thi.Vset(props[i].propName, props[i].propVal);

    //                 //TreeNode tmp = new TreeNode(props[i].propName + ((props[i].ReferToObj) ? "" : (" : " + props[i].propVal)));

    //                if (props[i].ReferToObj)
    //                {
    //                    opis rez = new opis("jsonObj");
    //                    rez.body = props[i].propVal;
    //                    subobjects[props[i].propObjIdx].BuildTreeopis(rez);

    //                    thi[props[i].propName] = rez;
    //                }                                  
    //            }
    //        }
    //        else
    //        {
    //            thi.PartitionKind = "jsonArray";
    //            foreach (JsonObject jso in subobjects)
    //            {
    //                opis rez = new opis("jsonObj");
    //                jso.BuildTreeopis(rez);
    //                thi.AddArr(rez);
    //            }

    //            for (int i = 0; i < propCou; i++)
    //            {

    //                thi.Vset("item_"+i.ToString(), props[i].propVal);                 
    //            }
    //        }

    //        //return rez;
    //    }


    //    #region data arrays retrieving

    //    public string[] GetPropFromArr(string propName)
    //    {
    //        string[] rez = new string[0];

    //        if (isArray)
    //        {
    //            rez = new string[subobjects.Count];
    //            int rezCou = 0;
    //            string[] nameArr = propName.Split('.');

    //            foreach (JsonObject jso in subobjects)
    //            {

    //                rez[rezCou] = jso.GetPropVal(nameArr, 0);
    //                rezCou++;
    //            }

    //            if (subobjects.Count == 0 && propCou > 0)
    //            {
    //                rez = new string[propCou];

    //                for (int i = 0; i < propCou; i++)
    //                {
    //                    rez[rezCou] = props[i].propVal;
    //                    rezCou++;
    //                }
    //            }

    //        }


    //        return rez;

    //    }

    //    /// <summary>
    //    /// return values list if it is an array object
    //    /// </summary>
    //    /// <returns></returns>
    //    public string[] GetArrValues()
    //    {
    //        string[] rez = new string[propCou];

    //        for (int i = 0; i < propCou; i++)
    //        {
    //            rez[i] = props[i].propVal;
    //        }

    //        return rez;
    //    }

    //    #endregion

    //}

    //public class CharBuilder
    //{
    //    int inputlen;
    //    char[] cacheArr;

    //    public CharBuilder()
    //    {
    //        inputlen = 0;
    //        cacheArr = new char[2000];
    //    }


    //    public void AddChar(char c)
    //    {
    //        cacheArr[inputlen] = c;


    //        inputlen++;

    //        if (inputlen >= cacheArr.Length)
    //        {
    //            Array.Resize(ref cacheArr, inputlen + 2000);
    //        }
    //    }

    //    public string GetString()
    //    {

    //        return new string(cacheArr, 0, inputlen);
    //    }

    //    public void Reinit()
    //    {
    //        inputlen = 0;
    //    }

    //}

    //public class JsonParser
    //{
    //    static char[] digits = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };

    //    public JsonParser()
    //    {
    //        init();
    //    }

    //    void init()
    //    {

    //    }

    //    public static JsonObject Parse(string text)
    //    {
    //        //  char[] brackets = {'[',']','{','}'};

    //        JsonObject rez = new JsonObject();
    //        JsonObject curr = rez;


    //        CharBuilder cacheArr = new CharBuilder();


    //        for (int i = 0; i < text.Length; i++)
    //        {

    //            if (text[i] == '"' && text[i - 1].ToString() != @"\")
    //            {
    //                curr.StartStringVal(text[i + 1]);
    //            }

    //            if (!curr.isStringOpened)
    //            {

    //                if ((text[i] == '}' || text[i] == ']' || text[i] == ','))
    //                {
    //                    curr.Setval(cacheArr.GetString());
    //                    cacheArr.Reinit();
    //                }

    //                if (text[i] == ':')
    //                {
    //                    curr.AddProp(cacheArr.GetString());
    //                    cacheArr.Reinit();
    //                }

    //            }

    //            if (curr.isStringOpened || (text[i] != ':' && text[i] != ','))
    //            {
    //                cacheArr.AddChar(text[i]);
    //            }

    //            if (!curr.isStringOpened && (text[i] == '}' || text[i] == ']' || text[i] == '{' || text[i] == '['))
    //            {
    //                curr = curr.AddObject(text[i]);
    //                cacheArr.Reinit();
    //            }

    //        }


    //        return rez;
    //    }


    //    public static int PriceFromString(string price)
    //    {
    //        int rez = (int)PriceFromStringF(price);
    //        return rez;
    //    }

    //    public static float PriceFromStringF(string price)
    //    {
    //        float rez = 0;

    //        price = price.Replace(" ", "");

    //        if (QueryParser.EnumCharOccurrence(price, digits) >= (price.Length - 1))
    //        {
    //            string[] p = price.Split(new char[] { '.', ',' });
    //            int dis = 1;
    //            string integer = p[0];

    //            for (int i = (integer.Length - 1); i >= 0; i--)
    //            {
    //                int pos = Array.IndexOf(digits, integer[i]);
    //                if (pos != -1)
    //                {
    //                    rez += dis * pos;
    //                    dis *= 10;
    //                }
    //            }

    //            if (p.Length == 2)
    //            {
    //                float disf = 0.1f;
    //                integer = p[1];

    //                for (int i = 0; i < integer.Length; i++)
    //                {
    //                    int pos = Array.IndexOf(digits, integer[i]);
    //                    if (pos != -1)
    //                    { // 123.235
    //                        rez += disf * pos;
    //                        disf *= 0.1f;
    //                    }
    //                }

    //            }
    //        }

    //        return rez;
    //    }

    //    public static long LongFromString(string alphanum)
    //    {
    //        long rez = 0;

    //        alphanum = alphanum.Replace(" ", "");



    //        long dis = 1;


    //        for (int i = (alphanum.Length - 1); i >= 0; i--)
    //        {
    //            int pos = Array.IndexOf(digits, alphanum[i]);
    //            if (pos != -1 && dis < 1000000000000000000)
    //            {
    //                rez += dis * pos;
    //                dis *= 10;
    //            }
    //        }


    //        return rez;
    //    }

    //    public static DateTime UnixTimeStampToDateTime(string unixTimeStamp)
    //    {
    //        return UnixTimeStampToDateTime(LongFromString(unixTimeStamp));
    //    }

    //    public static DateTime UnixTimeStampToDateTime(long unixTimeStamp)
    //    {
    //        // Unix timestamp is seconds past epoch
    //        System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);
    //        dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
    //        return dtDateTime;
    //    }

    //    public static double DateTimeToUnixTimestamp(DateTime dateTime)
    //    {
    //        return (dateTime - new DateTime(1970, 1, 1).ToLocalTime()).TotalSeconds;
    //    }

    //    public static bool UnixPeriodReached(string[] arrdates, DateTime boundary)
    //    {
    //        bool rez = false;

    //        foreach (string s in arrdates)
    //        {
    //            if (LongFromString(s) > 999999 && UnixTimeStampToDateTime(LongFromString(s)) < boundary)
    //            {
    //                rez = true;
    //                break;
    //            }
    //        }

    //        return rez;
    //    }

    //    public static bool UnixPeriodSettled(string date, DateTime start, DateTime fin)
    //    {
    //        bool rez = false;

    //        DateTime param = UnixTimeStampToDateTime(LongFromString(date));

    //        rez = (LongFromString(date) > 999999 && param > start && param < fin);


    //        return rez;
    //    }

    //    public static string GetNumber(string s)
    //    {
    //        if (s == null)
    //        {
    //            return "";
    //        }
    //        string rez = "";

    //        for (int i = 0; i < s.Length; i++)
    //        {
    //            if (Char.IsNumber(s[i]))
    //            {
    //                rez += s[i];

    //            }
    //        }
    //        return rez;
    //    }

    //    public static string TrimQuotes(string s)
    //    {
    //        s = s.Trim();
    //        char[] br = { '"' };

    //        s = s.Trim(br);

    //        return s;
    //    }

    //}

    //public class Logger
    //{
    //    public static object locker = new object();

    //    static string[] messages;
    //    static int msgCou;



    //    public Logger()
    //    {
    //    }

    //    public static void init()
    //    {
    //        if (messages == null)
    //        {
    //            messages = new string[200000];
    //            msgCou = 0;
    //        }
    //    }


    //    public static void AddLog(string msg)
    //    {
    //        lock (locker)
    //        {
    //            messages[msgCou] = DateTime.Now.ToString() + " =>  " + msg;
    //            msgCou++;
    //        }
    //    }

    //    public static string[] GetText()
    //    {
    //        string[] rez = new string[msgCou];

    //        Array.Copy(messages, rez, msgCou);

    //        return rez;
    //    }

    //}

    //====================================================

   
    public struct Prop
    {
        bool isObject;
        public bool ReferToObj
        {
            get
            {
                return isObject;
            }
        }

        public string propName;
        public string propVal;
        int ObjIdx;
        public int propObjIdx
        {
            get
            {
                return ObjIdx;
            }

            set
            {
                isObject = true;
                ObjIdx = value;
            }
        }
    }


    public class JsonObject
    {
        // set to true when it was started by [
        bool isArray;

        int propCou;
        int currPropIdx;
        bool propOpened;
        bool stringOpened;
        byte openedQuotes;
        public string PageText;

        public bool isStringOpened
        {
            get
            {
                return stringOpened;// (openedQuotes >0);
            }
        }


        Prop[] props;
        JsonObject Parent;
        List<JsonObject> subobjects;

        public JsonObject[] objectsArray
        {
            get
            {
                return subobjects.ToArray();
            }
        }

        public JsonObject()
        {
            init();
        }

        public JsonObject(bool isArray_)
        {
            init();
            isArray = isArray_;

            props = isArray ? new Prop[5000] : new Prop[500];
        }

        void init()
        {
            Logger.init();

            isArray = false;
            propOpened = false;
            stringOpened = false;
            currPropIdx = -1;
            openedQuotes = 0;

            subobjects = new List<JsonObject>();
            props = new Prop[100];

        }

        #region  Parsing Methods

        /// <summary>
        /// use in case when : raise
        /// </summary>
        /// <param name="pname"></param>
        public void AddProp(string pname)
        {
            if (!propOpened)
            {
                pname = pname.Trim('"');

                currPropIdx = propCou;
                props[currPropIdx].propName = pname;
                props[currPropIdx].propVal = "";

                propOpened = true;
                openedQuotes = 0;
                stringOpened = false;

                propCou++;
            }

        }

        public bool StartStringVal(char nextChar)
        {
            if (propOpened)
            {
                if (openedQuotes == 0)
                {
                    stringOpened = true;
                }

                openedQuotes++;

                if ((nextChar == ',' || nextChar == '}' || nextChar == ']') && (openedQuotes % 2) == 0)
                {
                    stringOpened = !stringOpened;
                }



                //openedQuotes++;

            }

            return propOpened;
        }

        /// <summary>
        /// use in case when , raise
        /// </summary>
        /// <param name="val"></param>
        public void Setval(string val)
        {
            if (propOpened)
            {
                props[currPropIdx].propVal += val;

                if (!stringOpened)
                {
                    propOpened = false;
                }
            }
            else if (isArray && !string.IsNullOrEmpty(val))
            {
                props[propCou].propVal = val;
                propCou++;
            }

        }

        /// <summary>
        /// use in case when some of brackets raise in text
        /// </summary>
        /// <param name="val"></param>
        public JsonObject AddObject(char bracket)
        {
            JsonObject newSub = null;

            if (bracket == '[' || bracket == '{')
            {

                newSub = new JsonObject((bracket == '['));
                newSub.Parent = this;
                subobjects.Add(newSub);
                if (propOpened)
                {
                    props[currPropIdx].propObjIdx = subobjects.Count - 1;
                }
                else
                {
                    isArray = true;
                }

            }
            else if (bracket == '}' || bracket == ']')
            {
                propOpened = false;
                newSub = Parent;
            }

            return newSub;
        }

        #endregion


        public void ConcatenateObjects(JsonObject[] arr)
        {
            if (isArray)
            {
                subobjects.AddRange(arr);
            }
        }

        public JsonObject GetObject(string path)
        {
            JsonObject rez;
            string[] nameArr = path.Split('.');
            rez = GetObject(nameArr, 0);

            return rez;
        }

        public JsonObject GetObject(string[] propNames, int idx)
        {
            JsonObject rez = new JsonObject();

            if (propCou == 0 && subobjects.Count == 1)
            {
                rez = subobjects[0].GetObject(propNames, idx);
            }

            for (int i = 0; i < propCou; i++)
            {
                if (props[i].propName == propNames[idx])
                {

                    if (props[i].ReferToObj)
                    {
                        if ((idx + 1) < propNames.Length)
                        {
                            rez = subobjects[props[i].propObjIdx].GetObject(propNames, idx + 1);
                        }
                        else
                        {
                            rez = subobjects[props[i].propObjIdx];
                        }
                    }

                    break;
                }
            }

            return rez;
        }


        public string GetPropVal(string[] propNames, int idx)
        {
            string rez = "";

            for (int i = 0; i < propCou; i++)
            {
                if (props[i].propName == propNames[idx])
                {

                    if (props[i].ReferToObj)
                    {
                        if ((idx + 1) < propNames.Length)
                        {
                            rez = subobjects[props[i].propObjIdx].GetPropVal(propNames, idx + 1);
                        }

                    }
                    else
                    {
                        rez = props[i].propVal;
                    }



                    break;
                }
            }

            return rez;
        }

        public string GetPropVal(string propName)
        {
            string rez = "";

            string[] nameArr = propName.Split('.');

            rez = GetPropVal(nameArr, 0);

            return rez;
        }


        #region debug / visualization

        void BuildTree(TreeNode tn)
        {
            if (!isArray)
            {

                for (int i = 0; i < propCou; i++)
                {

                    TreeNode tmp = new TreeNode(props[i].propName + ((props[i].ReferToObj) ? "" : (" : " + props[i].propVal)));

                    if (props[i].ReferToObj)
                    {
                        subobjects[props[i].propObjIdx].BuildTree(tmp);
                    }

                    //subobjects[props[i].propObjIdx].BuildTree 
                    tn.Nodes.Add(tmp);
                }
            }
            else
            {
                tn.Text += "  " + subobjects.Count.ToString();
                foreach (JsonObject jso in subobjects)
                {
                    TreeNode tmp = new TreeNode("*");
                    jso.BuildTree(tmp);
                    tn.Nodes.Add(tmp);
                }

                for (int i = 0; i < propCou; i++)
                {
                    TreeNode tmp = new TreeNode(props[i].propVal);

                    tn.Nodes.Add(tmp);
                }
            }
        }

        public TreeNode GetDebugTree()
        {
            TreeNode rez = new TreeNode("-=#Root#=-");

            BuildTree(rez);

            return rez;
        }

        #endregion

        public void BuildTreeopis(opis thi)
        {
            if (!isArray)
            {

                for (int i = 0; i < propCou; i++)
                {
                    props[i].propName = props[i].propName.Replace("\"", string.Empty).Trim();
                    props[i].propVal = props[i].propVal.Replace("\"", string.Empty).Trim();

                    thi.Vset(props[i].propName, TemplatesMan.UTF8BigEndian_to_Kirill( props[i].propVal));

                    //TreeNode tmp = new TreeNode(props[i].propName + ((props[i].ReferToObj) ? "" : (" : " + props[i].propVal)));

                    if (props[i].ReferToObj)
                    {
                        opis rez = new opis("jsonObj");
                        rez.body = TemplatesMan.UTF8BigEndian_to_Kirill(props[i].propVal);
                        subobjects[props[i].propObjIdx].BuildTreeopis(rez);

                        thi[props[i].propName] = rez;
                    }
                }
            }
            else
            {
                thi.PartitionKind = "jsonArray";
                foreach (JsonObject jso in subobjects)
                {
                    opis rez = new opis("jsonObj");
                    jso.BuildTreeopis(rez);
                    thi.AddArr(rez);
                    if(string.IsNullOrEmpty(rez.PartitionName))
                    {
                        rez.PartitionName = "jsonObj";
                    }
                }

                for (int i = 0; i < propCou; i++)
                {

                    thi.Vset("item_" + i.ToString(),
                        TemplatesMan.UTF8BigEndian_to_Kirill(props[i].propVal.Replace("\"", string.Empty).Trim()));
                }
            }

            //return rez;
        }


        #region data arrays retrieving

        public string[] GetPropFromArr(string propName)
        {
            string[] rez = new string[0];

            if (isArray)
            {
                rez = new string[subobjects.Count];
                int rezCou = 0;
                string[] nameArr = propName.Split('.');

                foreach (JsonObject jso in subobjects)
                {

                    rez[rezCou] = jso.GetPropVal(nameArr, 0);
                    rezCou++;
                }

                if (subobjects.Count == 0 && propCou > 0)
                {
                    rez = new string[propCou];

                    for (int i = 0; i < propCou; i++)
                    {
                        rez[rezCou] = props[i].propVal;
                        rezCou++;
                    }
                }

            }


            return rez;

        }

        /// <summary>
        /// return values list if it is an array object
        /// </summary>
        /// <returns></returns>
        public string[] GetArrValues()
        {
            string[] rez = new string[propCou];

            for (int i = 0; i < propCou; i++)
            {
                rez[i] = props[i].propVal;
            }

            return rez;
        }

        #endregion

    }

    public class CharBuilder
    {
        int inputlen;
        char[] cacheArr;

        public CharBuilder()
        {
            inputlen = 0;
            cacheArr = new char[2000];
        }


        public void AddChar(char c)
        {
            cacheArr[inputlen] = c;


            inputlen++;

            if (inputlen >= cacheArr.Length)
            {
                Array.Resize(ref cacheArr, inputlen + 2000);
            }
        }

        public string GetString()
        {

            return new string(cacheArr, 0, inputlen);
        }

        public void Reinit()
        {
            inputlen = 0;
        }

    }

    public class JsonParser
    {
        static char[] digits = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };

        public JsonParser()
        {
            init();
        }

        void init()
        {

        }

        public static JsonObject Parse(string text)
        {
            //  char[] brackets = {'[',']','{','}'};

            JsonObject rez = new JsonObject();
            JsonObject curr = rez;


            CharBuilder cacheArr = new CharBuilder();

            int quotesCou = 0;

            for (int i = 0; i < text.Length; i++)
            {

                if (text[i] == '"' && text[i - 1].ToString() != @"\")
                {
                    quotesCou++;
                    if(i + 1 < text.Length)
                    curr.StartStringVal(text[i + 1]);
                }

                if (!curr.isStringOpened)
                {

                    if ((text[i] == '}' || text[i] == ']' || text[i] == ','))
                    {
                        curr.Setval(cacheArr.GetString());
                        cacheArr.Reinit();
                    }

                    if (text[i] == ':')
                    {
                        if ((quotesCou % 2) == 0)
                        {
                            curr.AddProp(cacheArr.GetString());
                            cacheArr.Reinit();
                        }else
                            cacheArr.AddChar(text[i]);

                    }

                }

                if (curr.isStringOpened || (text[i] != ':' && text[i] != ','))
                {
                    cacheArr.AddChar(text[i]);
                }

                if (!curr.isStringOpened && (text[i] == '}' || text[i] == ']' || text[i] == '{' || text[i] == '['))
                {
                    curr = curr.AddObject(text[i]);
                    cacheArr.Reinit();
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

            price = price.Replace(" ", "");

            if (QueryParser.EnumCharOccurrence(price, digits) >= (price.Length - 1))
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

        public static DateTime UnixTimeStampToDateTime(string unixTimeStamp)
        {
            return UnixTimeStampToDateTime(LongFromString(unixTimeStamp));
        }

        public static DateTime UnixTimeStampToDateTime(long unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dtDateTime;
        }

        public static double DateTimeToUnixTimestamp(DateTime dateTime)
        {
            return (dateTime - new DateTime(1970, 1, 1).ToLocalTime()).TotalSeconds;
        }

        public static bool UnixPeriodReached(string[] arrdates, DateTime boundary)
        {
            bool rez = false;

            foreach (string s in arrdates)
            {
                if (LongFromString(s) > 999999 && UnixTimeStampToDateTime(LongFromString(s)) < boundary)
                {
                    rez = true;
                    break;
                }
            }

            return rez;
        }

        public static bool UnixPeriodSettled(string date, DateTime start, DateTime fin)
        {
            bool rez = false;

            DateTime param = UnixTimeStampToDateTime(LongFromString(date));

            rez = (LongFromString(date) > 999999 && param > start && param < fin);


            return rez;
        }

        public static string GetNumber(string s)
        {
            if (s == null)
            {
                return "";
            }
            string rez = "";

            for (int i = 0; i < s.Length; i++)
            {
                if (Char.IsNumber(s[i]))
                {
                    rez += s[i];

                }
            }
            return rez;
        }


    }


    public class Logger
    {
        public static object locker = new object();

        static string[] messages;
        static int msgCou;



        public Logger()
        {
        }

        public static void init()
        {
            if (messages == null)
            {
                messages = new string[200000];
                msgCou = 0;
            }
            else
            {
                msgCou = 0;
            }

        }


        public static void AddLog(string msg)
        {
            lock (locker)
            {
                messages[msgCou] = DateTime.Now.ToString() + " =>  " + msg;
                msgCou++;
            }
        }

        public static string[] GetText()
        {
            string[] rez = new string[msgCou];

            Array.Copy(messages, rez, msgCou);

            return rez;
        }

    }





}
