using basicClasses.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace basicClasses
{
 

   public class opis
    {
        public static bool do_not_build_debug;
        public static opis listOfVisualisedCircularRefs;
        public bool raiseEvents;
        public TreeNode treeElem;

        #region  variables  properties

        public static ObjectsFactory OF;
     
        string PartitionName_Lower;
        string PartitionName_;
        
        public string PartitionName  
        {
            get
            {
                return PartitionName_;
            }
            set
            {
                if ( raiseEvents && !string.IsNullOrEmpty(PartitionName_))
                {
                    ActionDone(new opis("подія", "вид змінено імя", "імя " + PartitionName, "тип " + PartitionKind,
                       "нове " + value, "старе " + PartitionName_));
                }

                PartitionName_ = value;
                PartitionName_Lower = value ==null ? null: value.ToLower();
            }
        }

       
        string PartitionKind_;      
        public string PartitionKind 
        {
             get{
                 return PartitionKind_;
            }

            set
            {
                if (raiseEvents) ActionDone(new opis("подія", "попередній " + PartitionKind_, "новий " + value, "вид змінено тип частини", "імя " + PartitionName, "тіло " + body, ""));
                PartitionKind_ = value;
               
            }
        }


        opis[] arr;
        int paramCou;
        bool isDuplicated;
        public bool isDuplicated_
        {
            get
            {
                return isDuplicated;
            }
        }
        opis copy;
        public int listCou
        {
            get
            {
                return paramCou;
            }
        }

        public bool isInitlze
        {
            get
            {
                return (!string.IsNullOrEmpty(body) || paramCou>0);
            }
        }
        public int intVal
        {
            get{
                return StrUtils.PriceFromString(body);
            }

            set
            {
                if (string.IsNullOrEmpty(PartitionKind))
                {
                    PartitionKind = "value";
                }
                body = value.ToString();
                if (raiseEvents) ActionDone("числове значення присвоєно", PartitionName, PartitionKind);
            }
        }

        string body_;
        public string body 
        {
            get
            {
                return body_;
            }

            set
            {
                if (raiseEvents){ ActionDone(new opis("подія", "вид змінено тіло", "імя " + PartitionName, "тип " + PartitionKind,
                 "нове " + value, "старе " + body, "позиція " + paramCou.ToString())); }

                body_ = value;
            }

        }

        public object bodyObject; 
        public object FuncObj;

     //   Dictionary<string, int> itemsNamesHash;

        #endregion

        //=======================================

        #region partitioning word indexer

        public opis this[string index]
        {
            get
            {
                return getPartition(index);
            }

            set
            {
                int idx = -1;
                value.PartitionName = index;

                if ((idx = getPartitionIdx(index)) != -1)
                {
                    arr[idx] = value;
                    //ActionDone("замінено", index, value.PartitionKind);
                    if (raiseEvents)
                    {
                        ActionDone(new opis("подія", "вид замінено", "імя " + index,
                            "тип " + value.PartitionKind, "тіло " + value.body, "позиція " + idx.ToString()));
                    }
                }
                else
                {                               
                    if (arr.Length <= paramCou)
                    {
                        Array.Resize(ref arr, paramCou + 3);
                    }
                    arr[paramCou] = value;
                    paramCou++;

                    if (raiseEvents)
                    {                       
                        ActionDone(new opis("подія", "вид додано", "імя " + index,
                            "тип " + value.PartitionKind, "тіло " + value.body, "позиція " + (paramCou-1).ToString()));
                    }
                }
            }
        }

        public opis this[int index]
        {
            get
            {
                return arr[index];
            }
        }

        opis getPartition(string part)
        {
            opis rez = null;
            int idx = -1;

            if ((idx = getPartitionIdx(part)) != -1)
            {
                rez = arr[idx];
            }
            else
            {
                rez = new opis();
                rez.PartitionName = part;

                if (arr.Length <= paramCou)
                {
                    Array.Resize(ref arr, paramCou + 3);
                }
                arr[paramCou] = rez;
                paramCou++;
                //throw new ArgumentOutOfRangeException(PartitionName +" не знайдено сегмент опису " + part);
            }

            return rez;
        }

      public int getPartitionIdx(string part)
        {
            if (part == null)
                return -1;

            int rez = -1;          

            if (rez == -1 )
            {
                part = part.ToLower();
                for (int i = 0; i < arr.Length; i++)
                {                   
                    if (arr[i] != null &&
                        arr[i].PartitionName != null &&                        
                     arr[i].PartitionName_Lower == part)
                    {                        
                        rez = i;
                        break;
                    }
                }
            }

            return rez;
        }

        public opis getForm(string part)
        {
            opis rez ;
            int p = getPartitionIdx(part);
            p = (p == -1) ? getPartitionForm(part) : p;

            if (p == -1)
            {
                rez = new opis();
            }
            else
            {
                rez = arr[p];
            }

            return rez;
        }

        public int getPartitionForm(string part)
        {

            if (string.IsNullOrEmpty(part)) return -1;

            int idx = -1;


            if (idx == -1)
            {
                for (int i = 0; i < arr.Length; i++)
                {
                    if (arr[i] != null && arr[i].PartitionName != null)
                    {
                        if (arr[i]["formz"].isHere(part))
                        {
                            idx = i;
                            break;
                        }
                    }
                }
            }

            return idx;
        }

        #endregion

        #region constructors

        void init()
        {
            body = "";
            paramCou = 0;
            int len = 7;

            if (!string.IsNullOrEmpty(PartitionKind))
            {
                switch (PartitionKind)
                {
                    case "list":
                        len = 20;
                        break;
                    case "object":
                        len = 0;
                        break;
                    case "value":
                        len = 0;
                        break;


                    case "medium":
                        break;
                   
                    case "large":
                       
                        //goto case "1";
                    default:
                        len =3;
                        break;
                }

            }

            arr = new opis[len];
        }

        public opis()
        {
            bool prev = raiseEvents;
            raiseEvents = false;

            PartitionKind = "";     
            init();

            raiseEvents = prev;
        }

     
        public opis(string PartKind, params string[] values)
        {
            bool prev = raiseEvents;
            raiseEvents = false;
            PartitionKind = PartKind;
            init();

            foreach (string pair in values)
            {
                string v = pair.Trim();
                int idx = v.IndexOf(" ");
                if (idx >0)
                {
                   string name = v.Substring(0,idx);
                    string val = v.Substring(idx+1);
                    if (name != "body")
                    {
                        this[name].body = val;
                    }
                    else
                    {
                        this.body = val;
                    }
                }

            }

            raiseEvents = prev;
        }

       
        /// <summary>
        /// <para>list  object  value  knovledge</para>
        /// якими прийомами обробляти даний опис 
        /// <para>list -список, в циклі по значенню listCou </para>
        /// <para>object - використовувати bodyObject </para>
        /// <para>value - використовувати body як значення детального опису (вага, довж...) </para>
        /// <para>knovledge - використовувати body в понятійному механізмі, розбір лексем</para> 
        /// </summary>
        /// <param name="PartKind">list  object  value  knovledge</param>
        public opis(string PartKind)
        {
            bool prev = raiseEvents;
            raiseEvents = false;
                  

            PartitionKind = PartKind;
            init();

            raiseEvents = prev;
        }
        
        public opis(string PartKind, object bodyp)
        {
            bool prev = raiseEvents;
            raiseEvents = false;
        
            PartitionKind = PartKind;
            bodyObject = bodyp;
            init();

            raiseEvents = prev;
        }

        #endregion

        #region get values and ARR +/- elements

        /// <summary>
        /// do not create absent object as access by index
        /// </summary>
        /// <param name="index">full path separated by dots</param>
        /// <returns>value of body</returns>
        public string V(string index)
        {
            string[] arr = index.Trim().Split(new char[] { '.' });


            return V(arr, 0); // getPartition(index).body;            
        }

        public string V(string[] index, int id)
        {
            if (id >= index.Length || !this.isInitlze)
            {
                return "";
            }

            string rez = "";

            if (getPartitionIdx(index[id]) != -1)
            {
                if (id + 1 == index.Length)
                {
                    rez = getPartition(index[id]).body;
                }
                else
                {
                    rez = getPartition(index[id]).V(index, id + 1);
                }
            }
           
            return rez;
        }


        public void Vset(string index, string data)
        {
            if (raiseEvents) ActionDone(new opis("подія", "вид змінено тіло", "імя " + index, "тип " + getPartition(index).PartitionKind,
                "нове " + data, "старе " + getPartition(index).body)   );           
            getPartition(index).body = data;            
        }

        public void Vadd(string index, string data)
        {
            if (raiseEvents) ActionDone(new opis("подія", "вид змінено тіло", "імя " + index, "тип " + getPartition(index).PartitionKind,
                "нове " + data, "старе " + getPartition(index).body));
            getPartition(index).body +=","+ data;
        }

        public void CopyArr(opis elem)
        {
            arr = elem.arr;
            paramCou = elem.paramCou;
        }
       
        public void CopyParams(opis elem)
        {
            //arr = elem.arr;
            string[] tmp = elem.ParamsNames();
            foreach (string s in tmp)
            {
                if (s != "waiters" && s != "context")
                {
                    int idx = getPartitionIdx(s);
                    if (idx == -1)
                        AddArr(elem[s]);
                    else
                        arr[idx] = elem[s];
                }
            }
                    
        }

        /// <summary>
        /// Do NOT copy MsgTemplate elements accept {p}
        /// </summary>
        /// <param name="elem"></param>
        public void CopyParamsMsg(opis elem)
        {
            //arr = elem.arr;
            string[] tmp = elem.ParamsNames();
            foreach (string s in tmp)
            {
                if (s != "waiters"  
                    && s != MsgTemplate.msg
                    && s != MsgTemplate.context
                    && s != MsgTemplate.msg_receiver
                    && s != MsgTemplate.no_transfer
                    && s != MsgTemplate.responce
                     && s != MsgTemplate.getAnswerDetails
                      && s != MsgTemplate.responce
                     //&& s != MsgTemplate.times_repeat
                     
                    )
                {
                    int idx = getPartitionIdx(s);
                    if (idx == -1)
                        idx = AddArr(elem[s]);
                    else
                    {
                        if(!arr[idx].isInitlze)
                        arr[idx] = elem[s];
                        else
                        if (arr[idx].PartitionKind == "Action")
                            arr[idx].AddArrRange(elem[s]);
                    }

                    //arr[idx].PartitionKind += "_copied";
                }
            }

        }

        public bool FindArr(opis instance)
        {
            bool rez = false;

            for (int i = 0; i < paramCou; i++)
            {            
                if (arr[i] == instance)
                {
                    rez = true;
                    return rez;
                }
            }
            return rez;
        }

        public void ForAllArrSetV(string part, string val)
        {
            for (int i = 0; i < listCou; i++)
            {
                arr[i].Vset(part, val);
            }
        }

        public void AddArrRange(opis elem)
        {
            //if (raiseEvents) ActionDone(new opis("подія", "вид додано масив", "імя " + elem.PartitionName,
            //    "тип " + elem.PartitionKind, "тіло " + elem.body, "позиція " + paramCou.ToString()));
            //PartitionKind_ = (string.IsNullOrEmpty(PartitionKind_) ? "" : PartitionKind_);
            
            Array.Resize(ref arr, paramCou + elem.listCou+ 3);

            for (int i = 0; i < elem.listCou; i++)
            {
                if (!FindArr(elem[i]))
                {
                    arr[paramCou] = elem[i];
                    paramCou++;
                }
            }
                     
        }

        public void AddArrMissing(opis elem)
        {           
            Array.Resize(ref arr, paramCou + elem.listCou + 3);

            for (int i = 0; i < elem.listCou; i++)
            {
                if (!isHere(elem[i].PartitionName))
                {
                    arr[paramCou] = elem[i];
                    paramCou++;
                }
            }
        }

        public int AddArr(opis elem)
        {           
            PartitionKind_ = (string.IsNullOrEmpty(PartitionKind_) ? "" : PartitionKind_);
            if (arr.Length <= paramCou)
            {
                Array.Resize(ref arr, paramCou + 3);
            }

            int rez = paramCou;
            arr[paramCou] = elem;
            paramCou++;

            if (raiseEvents) ActionDone(new opis("подія", "вид додано", "імя " + elem.PartitionName,
               "тип " + elem.PartitionKind, "тіло " + elem.body, "позиція " + (paramCou-1).ToString()));

            return rez;

        }

        public void ArrResize(int len)
        {
            if (arr.Length < len)
            {
                Array.Resize(ref arr, len);
            }
            else
                if (paramCou > len)
                paramCou = len;
        }


        public void RemoveArrElem(opis elem)
        {
            if (raiseEvents) ActionDone(new opis("подія", "вид видалено", "імя " + elem.PartitionName,
                "тип " + elem.PartitionKind, "тіло " + elem.body ));

            int idxLess = 0;
            for (int i = 0; i < paramCou; i++)
            {
                arr[i - idxLess] = arr[i];

                if (arr[i] == elem)
                {
                    idxLess++;
                    arr[i] = null;

                }
            }
           
            paramCou = paramCou - idxLess;    

        }

        public string[] ParamsNames()
        {
            string[] rez = new string[paramCou];
            for (int i = 0; i < paramCou; i++)
            {
                rez[i] = arr[i].PartitionName != null ? arr[i].PartitionName.Trim(): "";
            }

            return rez;
        }

        public string info()
        {
            string rez = PartitionKind.Length >0 ? PartitionKind+"\n" :"";
            for (int i = 0; i < paramCou; i++)
            {
                 string dat = arr[i].body.Trim();
                int partsize = 35;
                int maxlen = partsize;
                if (dat.Length > 0 && dat!="_")
                {
                    string tmp="";
                    string shift = "       ";
                    shift = shift.PadLeft(arr[i].PartitionName.Length, ' ');

                    if (dat.Length > partsize)
                    {                      
                        int prevpos = 0;
                        int p = dat.IndexOf(' ', partsize);
                        while (dat.Length > prevpos && p > 0)
                        {
                            tmp += shift + dat.Substring(prevpos, p - prevpos) + "  \n";
                            prevpos = p;
                            if (prevpos + partsize >= dat.Length)
                            {
                                tmp += shift + dat.Substring(p, dat.Length - prevpos);
                                p = -1;
                            }
                            else
                                p =  dat.IndexOf(' ', prevpos + partsize);
                         
                        }
                        if (prevpos + partsize < dat.Length)
                        {
                            tmp += shift + dat.Substring(prevpos, dat.Length - prevpos);
                        }

                        dat = tmp.Length > 0 ? tmp.Trim() : dat;
                        
                    }
                    rez += arr[i].PartitionName + ": " + dat + " \n";
                }
            }

            return rez;
        }


        #endregion


        #region Wraps

        public void Wrap(opis o)
        {
            this.PartitionKind_ = "wrapper";
            this.body_ = o.PartitionName;
            
            this[this.body_] = o;
        }

        public void WrapByName(opis o, string name, string renameO)
        {
            o.PartitionName = renameO;
            WrapByName( o,  name);
        }

        public void WrapByName(opis o, string name)
        {
            if (o == null)
                return;
            opis ol = this[name];
            ol.PartitionKind_ = "wrapper";
            ol.body = string.IsNullOrEmpty(o.PartitionName) ? name : o.PartitionName;
            ol[ol.body] = o;
        }

        /// <summary>
        /// return wrapped partition opis this[name][ this[name].body  ]
        /// </summary>
        /// <param name="name">name of wrapper</param>
        /// <returns></returns>
        public opis W(string name)
        {
            opis o = this[name];
            return o[o.body];         
        }

        public opis W()
        {      
            if(this.PartitionKind_ == "wrapper")      
            return this[this.body];
            else
                return this;
        }

        /// <summary>
        /// splitted body text by comma 
        /// <para>this.V(name).Split(new char[] { ',' })</para>
        /// </summary>
        /// <param name="name"></param>
        /// <returns> this.V(name).Split(new char[] { ',' })</returns>
        public string[] S(string name)
        {
            string[] rez = this.V(name).Split(new char[] { ',' });

            return rez;
        }

        #endregion

         #region LinkMethods

        /// <summary>
        /// get word or its form, useful in contexts
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public opis Find(string name)
        {
            opis wordo =new opis();
            if (getPartitionIdx(name) !=-1)
            {
                wordo = this[name];
            }

            if (wordo !=null && !wordo.isInitlze)// search in forms of neutral words
            {
                int iii = getPartitionForm(name);
                if (iii != -1 && this[iii].isInitlze)
                {
                    wordo = this[iii];
                }
                else
                {
                                       
                }
            }

            return wordo;
        }
                       
         #endregion

        public void InitFuncObj()
        {           
            if (!string.IsNullOrEmpty(PartitionName_))
            {
                FuncObj = ObjectsFactory.GetInstance(string.IsNullOrEmpty(V(ModelNotion.InitFuncObj)) ? PartitionName_ : V(ModelNotion.InitFuncObj));

                if (V(ModelNotion.InitFuncObj)=="root")
                {
                    root r = (root)FuncObj;
                    r.nameSpec = PartitionName_;
                }
               
            }
        }

        public void PrepareFuncObj()
        {
            if (FuncObj == null)
            {
                InitFuncObj();
                //FuncObj = OF.GetInstance(PartitionKind);
            }
        }

        #region Action registering and set handlers

        /// <summary>
        /// викликати всюди при зміні
        /// </summary>
        /// <param name="type">опис події що трапилась</param>
        /// <param name="partName">імя частини</param>
        /// <param name="partKind">тип частини</param>
        public void ActionDone(string type, string partName, string partKind)
        {
            if (this.V("waiters") =="")
            {
                return;
            }
            else
            {
                ActionDone(new opis("подія", "вид " + type, "імя " + partName, "тип " + partKind));
            }
        }

        public void ActionDone(string type, string partName, string partKind, string bod)
        {
            if (this.V("waiters") == "")
            {
                return;
            }
            else
            {
                ActionDone(new opis("подія", "вид " + type, "імя " + partName, "тип " + partKind, "тіло " + bod));
            }
        }

        /// <summary>
        /// send description of event to all waiters subscribed by [WaitAction] 
        /// </summary>
        /// <param name="o">description of event</param>
        public void ActionDone(opis o)
        {
            if (this.V("waiters") == "")
            {
                return;
            }
            o["count"].intVal = paramCou;
            
            opis w = this["waiters"];
            for (int i = 0; i < w.paramCou; i++)
            {
                if (w[i] != null)
                {
                    w[i].ReceiveAction(o, this);
                }
            }
        }

        // в обробчику події присвоювати свойство "опрацьовано" і так як усім передається один 
        // екземпляр опису події то всі можуть побачити його  та не опрацьовувати у себе

        /// <summary>
        /// add object that wait action from this object
        /// </summary>
        /// <param name="waiter">object with type [waiter]</param>
        public void WaitAction(opis waiter)
        {
            WaitAction(waiter, false);
        }

        public void WaitAction(opis waiter, bool uniqueInstansesOnly)
        {
            WaitAction(waiter, uniqueInstansesOnly, true);
        }

        public void WaitAction(opis waiter, bool uniqueInstansesOnly, bool autoRaise)
        {
            if (uniqueInstansesOnly && this["waiters"].FindArr(waiter))
            {
                return;
            }
                
            raiseEvents = autoRaise;
            this["waiters"].body = "present";
            this["waiters"].AddArr(waiter);
        }

        /// <summary>
        /// WARNING: turn [ raiseEvents = false ]
        /// </summary>
        /// <param name="waiter"></param>
        public void SubscribeForNotification(opis waiter)
        {
            WaitAction(waiter, false, false);
        }

        /// <summary>
        /// do not change raise events flag, for some cases use NotifyReceivers()
        /// </summary>
        /// <param name="o"></param>
        /// <param name="sendAll"></param>
        public void CopyWaiters(opis o, bool sendAll)
        {
            //raiseEvents = false;
            opis wp = o["waiters"];
            for (int i = 0; i < wp.listCou; i++)
            {
                this["waiters"].AddArr(wp[i]);

                if (sendAll)
                {              
                      opis msg=  new opis("подія", "вид оповіщення штучне", "імя "+ ModelG.allarraycheck,
                         "тип " + ModelG.allarraycheck);
                      wp[i].ReceiveAction(msg, this);
                   
                }

            }       
           
        }

        public void NotifyReceivers()
        {
            opis msg = new opis("подія", "вид оповіщення штучне", "імя " + ModelG.allarraycheck,
                        "тип " + ModelG.allarraycheck);
            ActionDone(msg);
        }

        public void StopReceive(opis waiter)
        {
            this["waiters"].RemoveArrElem(waiter);           
        }

        public void ReceiveAction(opis action, opis sender)
        {
            if (FuncObj == null)
            {
                return;
            }
            
            if (FuncObj is IOpisFuncInstanceWaiter)
            {
                IOpisFuncInstanceWaiter i = (IOpisFuncInstanceWaiter)FuncObj;
                opis tmp = action.Duplicate();
                tmp.WrapByName(this, "receiver");
                i.ProcessWaiter(tmp, sender);
            }

            if (FuncObj is IOpisFunctionalInstance)
            {
                IOpisFunctionalInstance i = (IOpisFunctionalInstance)FuncObj;
                i.ProcessWaiter(action, sender);
            }            
        }

        #endregion


        /// <summary>
        /// passes execution to functional object (FuncObj prop) that represents curr PartitionKind 
        /// </summary>
        /// <param name="internl">function name to execute</param>
        /// <param name="environment">all the parameters in one opis</param>
        public opis Process(string internl, opis environment)
        {
            if (FuncObj == null)
            {
                return new opis("process_accompl_status", "status no functional object assigned");
            }

            PrepareFuncObj();
            IOpisFunctionalInstance i = (IOpisFunctionalInstance)FuncObj;

            return i.Process(internl, environment);
        }


        #region etc

        /// <summary>
        /// check presence partition with such name (it can be not initialized, just presence is enough)
        /// </summary>
        /// <param name="part"></param>
        /// <returns></returns>
        public bool isHere(string part)
        {

            return getPartitionIdx(part) != -1;
        }

        #endregion


        #region hierarchy algorithms

        public string serialize()
        {

            if (isDuplicated)
            {
                return "";
            }
            isDuplicated = true;         

            string rez = "";
            StringBuilder sb = new StringBuilder(paramCou);

            sb.Append("{");
            sb.Append("\"N\": \"" + PartitionName + "\",");
            sb.Append("\"K\": \"" + PartitionKind + "\",");
            if(body !=null)
            sb.Append("\"B\": \"" + body.Replace("\"", "[&amp]") + "\"");           

            if ( paramCou >0)
            {
                sb.Append(",\"a\": [");

                for (int i = 0; i < paramCou; i++)
                {
                    string tmp = arr[i].serialize();
                    sb.Append(tmp);
                    if (i + 1 < paramCou && !string.IsNullOrEmpty(tmp))
                    {
                        sb.Append(",");
                    }
                }

                sb.Append("]");
            }
   
            sb.Append("}");            

            rez = sb.ToString();

            isDuplicated = false;

            return rez;
        }

        public void load(string data)
        {
            load(data, 0);
        }

        public int load(string data, int startpos)
        {
            int rez = 0;

            int idx = startpos;
            int idxShift = 1;

            string currtext = "";
            string currprop = "";
            string rawtext = "";

            //opis root = new opis();

            while (idx < data.Length)
            {
                idxShift = 1;


                if (data[idx] == '{' && idx>0)
                {                
                    if (currprop.Length > 0)
                    {
                        idx = this[currprop].load(data, idx + 1);
                    }
                    else
                    {
                        opis elem = new opis();
                        idx = elem.load(data, idx + 1);
                        this.AddArr(elem);
                    }

                    currprop = "";
                    rawtext = "";
                    currtext = "";
                }

                if (data[idx] == '[')
                {
               
                    if (currprop.Length > 0)
                    {
                        if (currprop == "array" || currprop == "a")
                        {
                            idx = this.load(data, idx + 1);
                        }
                        else
                        {
                            idx = this[currprop].load(data, idx + 1);
                        }
                    }
                    else
                    {
                        opis elem = new opis();
                        idx = elem.load(data, idx + 1);
                        this.AddArr(elem);
                    }

                    rawtext = "";
                    currprop = "";
                    currtext = "";
                }

                if (data[idx] == '"')
                {
                    currtext = enclosed( data, idx, '"', '"');
                    idxShift = currtext.Length + 2;
                }

                if (data[idx] == ',' || data[idx] == ']' || data[idx] == '}')
                {
                    // it is a string or logical-numeric value 
                    if (currprop.Length > 0)
                    {
                        currtext = currtext.Replace("[&amp]", "\"");
                        rawtext = rawtext.Replace("[&amp]", "\"");
                        if ( currprop == "PN" || currprop == "N")
                        {
                            PartitionName = currtext;
                        }else
                            if ( currprop == "PK" || currprop == "K")
                            {
                                PartitionKind = currtext;
                            }
                            else
                                if (currprop == "B" || currprop == "body")
                                {
                                    body = currtext;
                                }else
                            {
                                this[currprop].body = currtext.Length > 0 ? currtext : rawtext;
                            }
                    }
                    else
                    {
                        if (rawtext.Length > 0 || currtext.Length > 0)
                        {
                            // add value to array
                        }
                    }
                }

                if (data[idx] == '}' || data[idx] == ']')
                {
                    return idx + 1;
                }
                

                if (data[idx] == ':')
                {
                    currprop = currtext;
                    rawtext = "";
                }

                if (data[idx] !=' ')
                {
                    rawtext +=data[idx];
                }

                idx += idxShift;
            }


            return rez;

        }

        public string enclosed(string data, int pos, char open, char close)
        {
            string rez ="";

           int openpos=   data.IndexOf(open, pos);
           int closepos = -1;

           if (open == '"')
           {

               if (openpos != -1)
               {
                   closepos = data.IndexOf(close, openpos + 1);
                   while (closepos != -1 && data[closepos - 1] == '\\')
                   {
                       closepos = data.IndexOf(close, closepos + 1);
                   }

                   if (closepos != -1)
                   {
                       rez = data.Substring(openpos + 1, closepos - openpos-1);
                   }
               }
           }
           else
           {
               if (openpos != -1)
               {
                  
                   if (closepos != -1)
                   {
                       rez = data.Substring(openpos + 1, closepos - openpos);
                   }
               }

           }

            return rez;
        }

        bool checkPairedQuotes(string data, int pos, int poslast)
        {
            bool rez = false;
            int qcou = 0;

           int closepos = data.IndexOf('"', pos + 1);
           
           while (closepos != -1 && closepos< poslast)
            {
                qcou += data[closepos - 1] == '\\' ? 0 : 1;
                closepos = data.IndexOf('"', closepos + 1);
            }
            rez = (qcou %2) ==0;

            return rez;
        }

        public TreeNode GetDebugTree()
        {
            listOfVisualisedCircularRefs.AddArr(this);
            return GetDebugTreeIL(160000);
        }

        public TreeNode GetDebugTree( int maxDepth)
        {
            TreeNode rez = new TreeNode("-=#Root#=-");

            listOfVisualisedCircularRefs.AddArr(this);
            if (! do_not_build_debug)
            BuildTree(rez, 0, maxDepth, null);

            return rez;
        }

        public TreeNode GetDebugTreeIL(int maxitemsCount)
        {
            TreeNode rez = new TreeNode("-=#Root#=-");

            if (!do_not_build_debug)
                BuildTree(rez, 0, 70, new opis("", "max "+ maxitemsCount.ToString()));

            return rez;
        }

        public void lockThisForDuplication()
        {
            isDuplicated = true;
        }

       public void UnlockThisForDuplication()
        {
            isDuplicated = false;
        }

        void BuildTree(TreeNode tn, int depth, int maxDepth, opis maxItems)
        {
            if (!isDuplicated && depth< maxDepth 
                && (maxItems==null || maxItems["max"].intVal > maxItems["cou"].intVal))
            {
                if (maxItems != null)
                {
                    maxItems["cou"].intVal++;
                }

                isDuplicated = true;
                TreeNode tmp = new TreeNode(PartitionName + "[" + PartitionKind + "]  " + ((body!=null && body.Length> 2000)? "//long data//": body));
                tmp.Tag = this;
                //if (treeElem != null) treeElem.Tag = null;
                treeElem = tmp;
                tmp.ForeColor = getColorOfModel(PartitionKind);
               
                tn.Nodes.Add(tmp);
               
                for (int i = 0; i < paramCou ; i++)
                {
                    if (arr[i]!=null && arr[i].PartitionName != "alreadyProcessed")
                    {
                        arr[i].BuildTree(tmp, depth+1, maxDepth, maxItems);
                    }
                }

                isDuplicated = false;
            }
            else
            {
                TreeNode tmp = new TreeNode(PartitionName + "[ REFERENCE ] : " + body);
                tmp.Tag = this;
                tn.Nodes.Add(tmp);
            }
   
        }

        private Color getColorOfModel(string partKind)
        {
            Color rez = Color.AntiqueWhite;

            //if (partitionKind.StartsWith("fill_"))
            //    partitionKind = "filler";

            switch (partKind)
            {
               
                 case "filler":
                    rez = Color.Cornsilk;
                    break;

                case "ConditionChecker":
                case "ChecksList":
                    rez = Color.Coral;
                    break;
                case "exec":
                case "Action":
                    rez = Color.Chocolate;
                    break;

                case "func":              
                  //  rez = Color.FromArgb(225,199,117);
                    //rez = Color.FromArgb(217, 188, 92);
                    rez = Color.FromArgb(226, 205, 133);
                    break;

                case "initValues":
                    rez = Color.DarkSalmon;
                    break;

                case "ForEach":
                    rez = Color.Aqua;
                    break;

                case "MsgTemplate":
                    rez = Color.ForestGreen;
                    break;

                case "BodyValueModificator":
                    
                case "GetAnyPartOfOpis":
                    rez = Color.FromArgb(113, 204, 182);
                    break;

                //case "ConditionChecker":
                //    rez = Color.FromArgb(179, 138, 0);
                //    break;

                case "template":
                case "buildTreeVal_sdc_i":
                case "TreeDataExtractor":
                    rez = Color.FromArgb(179, 179, 255);
                    break;

                case "RangingList":                    
                case "RangeAndAssign":
                    rez = Color.FromArgb(255, 151, 185);
                    break;


                case "":
                    break;

            }


            return rez;

        }

        public void CleanPartitions(string pt)
        {
            string[] arrp = pt.ToLower().Split(new char[] { ',' });
            for (int p = 0; p < arrp.Length; p++)
            {
                arrp[p] = arrp[p].Trim();
            }

            CleanPartitions(arrp);
        }

        void CleanPartitions(string[] arrp)
        {
            if (isDuplicated)
            {
                return;
            }
            isDuplicated = true;

                int idxLess = 0;
                for (int i = 0; i < paramCou; i++)
                {
                    arr[i - idxLess] = arr[i];
                    for (int p = 0; p < arrp.Length; p++)
                    {
                        if (arr[i].PartitionName.ToLower() == arrp[p])
                        {
                            idxLess++;
                            arr[i] = null;
                            break;
                        }
                    }
                    if (i - idxLess >= 0)
                    {
                        arr[i - idxLess].CleanPartitions(arrp);
                    }

                    //tn.Nodes.Add(tmp);
                }

                isDuplicated = false;
                paramCou = paramCou - idxLess;          
        }


        public void CleanNodeRef()
        {
            if (isDuplicated)
            {
                return;
            }
            isDuplicated = true;

            if (treeElem != null)
            {
                treeElem.Tag = null;
                treeElem = null;
                for (int i = 0; i < paramCou; i++)
                {
                    arr[i].CleanNodeRef();
                }
            }

            isDuplicated = false;            
        }

        public void ClearNodesRef()
        {
            listOfVisualisedCircularRefs.treeElem = new TreeNode();
            listOfVisualisedCircularRefs.CleanNodeRef();
            listOfVisualisedCircularRefs.CopyArr(new opis());
        }

        public int CheckConformity(opis partition, opis template)
        {
            return CheckConformity( partition,  template,  null);
        }

        public int CheckConformity(opis partition, opis template, string markForVisualization)
        {
            int rez = 0;

            for (int i = 0; i < template.listCou; i++)
            {
                if (partition.getPartitionIdx(template[i].PartitionName) != -1
                    && (template[i].body == ""
                    || partition.V(template[i].PartitionName) == template[i].body))
                {
                    rez++;
                    if (template[i].PartitionKind != "Fill_Spec_Info"
                        && template[i].listCou > 0)
                    {
                        int subtreerez = CheckConformity(partition[template[i].PartitionName], template[i] , markForVisualization);

                        if (template[i].listCou != subtreerez)
                            rez--;
                    }
                }
                else
                {
                    if (markForVisualization != null)
                    {
                        if(partition.getPartitionIdx(template[i].PartitionName) == -1)
                        template[i].PartitionKind = markForVisualization;
                        else
                            template[i].PartitionKind = "modified";
                    }
                }


            }


            return rez;
        }

        public void NormalizeNamesForComparison(opis p)
        {
            if (p == null) return;
            char[] numb = new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };

            opis presenceCou = new opis();

            for (int i = 0; i < p.listCou; i++)
            {
                int puy = p[i].PartitionName.IndexOfAny(numb);
                if (puy > 0)
                    p[i].PartitionName = p[i].PartitionName.Substring(0, puy);

                if (presenceCou[p[i].PartitionName].isInitlze)
                    p[i].PartitionName += presenceCou[p[i].PartitionName].intVal;

                presenceCou[p[i].PartitionName].intVal++;
            }

            for (int i = 0; i < p.listCou; i++)
            {
                NormalizeNamesForComparison(p[i]);
            }
        }

        /// <summary>
        ///  
        /// </summary>
        /// <param name="templ"></param>
        /// <param name="srs"></param>
        /// <returns></returns>
        public static  opis GetLevelByTemplate(opis templ, opis srs, bool create)
        {
            opis rez = null;
            if (srs.getPartitionIdx(templ.PartitionName) != -1
                || create)
            {
                rez = srs[templ.PartitionName];
                for (int i = 0; i < templ.listCou; i++)
                {
                    opis tmp = GetLevelByTemplate(templ[i], rez, create);
                    if (tmp != null && (tmp.isInitlze || create))
                    {
                        rez = tmp;
                        break;
                    }
                }
            }

            return rez;

        }

        public opis Duplicate()
        {
            if (isDuplicated && copy!=null)
            {
                return copy;
            }

            if(PartitionKind =="dataref" && this.paramCou >0)
            {
                return this;
            }

            opis rez = new opis();
            copy = rez;

            rez.body = this.body;
            rez.PartitionKind = this.PartitionKind;
            rez.PartitionName_Lower = this.PartitionName_Lower;
            rez.PartitionName_ = this.PartitionName;
            rez.paramCou = this.paramCou;
            isDuplicated = true;

            rez.arr = new opis[this.paramCou];
             

            for (int i = 0; i < this.paramCou; i++)
            {
                rez.arr[i] = this.arr[i].Duplicate();
                //rez.arr[i].bodyObject = this.arr[i].bodyObject;
            }

            isDuplicated = false;
            copy = null;

            return rez;
        }

        public void FindTreePartitions(string modelName, string path, opis referers)
        {
            opis templ = new opis();
            templ.PartitionKind = modelName;
            FindTreePartitions(templ, path, referers);
        }

        public void FindTreePartitions(opis templ, string path, opis referers)
        {
            FindTreePartitions(templ, path, referers, true);
        }

       public void FindTreePartitions(opis templ, string path, opis referers, bool recursive)
        {

            for (int i = 0; i < paramCou; i++)
            {
                if ((string.IsNullOrEmpty(templ.PartitionKind)|| 
                    arr[i].PartitionKind == templ.PartitionKind) &&
                    (string.IsNullOrEmpty(templ.PartitionName) ||
                    arr[i].PartitionName == templ.PartitionName) &&
                    (string.IsNullOrEmpty(templ.body) ||
                    arr[i].body == templ.body)
                    )                 
                {
                    opis refitem = new opis();
                    refitem.PartitionName = path + "->" + arr[i].PartitionName;
                    //refitem.body = arr[i].PartitionName;
                    refitem.AddArr(arr[i]);

                    referers.AddArr(refitem);
                   
                }

                if(recursive)
                arr[i].FindTreePartitions(templ, path+"->"+ arr[i].PartitionName, referers);
            }

            isDuplicated = false;
           
        }

        public opis GetItemByModel(string modelName)
        {
            opis rez = null;
           
                for (int i = 0; i < listCou; i++)
                {                   
                    if (arr[i].PartitionKind == modelName)
                    {
                        rez = arr[i];
                        break;
                    }
                }
            
            return rez;
        }

        public bool CheckKVPresence(opis kvp)
        {
            bool rez = false;

            rez =  this.V(kvp.PartitionName) == kvp.body;

            if (!rez)
            {
                for (int i = 0; i < listCou; i++)
                {
                    if (arr[i].CheckKVPresence(kvp))
                        return true;
                }
            }

            return rez;
        }

        public int FindByTemplateValue(opis strucTmpl, opis rez, bool isTop)
        {
           
            if (isDuplicated) return 0;

            lockThisForDuplication();

            int matchedLvl = 0;
            var match = new int[strucTmpl.listCou];

            int idxFound = -1;

            for (int k = 0; k < strucTmpl.listCou; k++)
            {
                var templ = strucTmpl[k];

                if (k > 0 && match[k - 1] == 0)
                    break;

                for (int i = 0; i < paramCou; i++)
                {

                    if ((string.IsNullOrEmpty(templ.PartitionKind) ||
                        arr[i].PartitionKind == templ.PartitionKind) &&
                        (string.IsNullOrEmpty(templ.PartitionName) ||
                        arr[i].PartitionName == templ.PartitionName) &&
                        (string.IsNullOrEmpty(templ.body) ||
                        arr[i].body == templ.body)
                        )
                    {

                        var r = arr[i].FindByTemplateValue(templ, rez, false);
                        match[k] += r;
                        
                        if(isTop && strucTmpl.listCou == 1 && r>0)
                            rez.AddArr(arr[i]);

                    }
                }
            }

            matchedLvl = match.Where(x => x > 0).Count();

            if (isTop)
            {
                if (matchedLvl == strucTmpl.listCou
                    && strucTmpl.listCou != 1)
                    rez.AddArr(this);

                for (int i = 0; i < paramCou; i++)
                    arr[i].FindByTemplateValue(strucTmpl, rez, true);
            }

            UnlockThisForDuplication();

            return matchedLvl == strucTmpl.listCou ? 1 : 0;

        }

        public void TransformLoadedRaw()
        {
            if (isDuplicated)
            {
                return;
            }
            isDuplicated = true;

        
            for (int i = 0; i < paramCou; i++)
            {

                if (arr[i].PartitionName == "PartitionName")
                {
                    this.PartitionName = arr[i].body;
                    arr[i].PartitionName = "PartitionName";
                    continue;
                }

                if (arr[i].PartitionName == "array")
                {                  
                    arr[i].PartitionName = "#array";
                    //arr[i].TransformLoadedRaw();
                    for (int k = 0; k < arr[i].listCou; k++)
                    {
                        this["transform_array_" + k.ToString()] = arr[i].arr[k];
                        //arr[i].arr[k].TransformLoadedRaw();
                    }

                    continue;
                }

                //arr[i].TransformLoadedRaw();
            }

            for (int i = 0; i < paramCou; i++)
            {

                if (arr[i].PartitionName == "PartitionName")
                {
                    this.PartitionName = arr[i].body.Trim();
                    arr[i].PartitionName = "#PartitionName";
                    continue;
                }

                arr[i].TransformLoadedRaw();
            }

            isDuplicated = false;
        }

        #endregion hierarchy algorithms

    }


    public class ObjectsFactory
    {
        public object GetInstance(string kind, opis o)
        {

            return null;
        }

        public static object GetInstance(string kind)
        {
            object rez = null;

            rez = new root();
            if (kind != "root")
                ((root)rez).nameSpec = kind;

            return rez;

        }

        public void init()
        {

        }


    }
}
