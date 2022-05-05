// Copyright (C) 2015-2022 Igor Proskochilo

using basicClasses.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
#if NETFRAMEWORK
using System.Windows.Forms;
#endif
using System.Drawing;
using basicClasses.models.sys_ext;
using basicClasses.models.WEB_api;
using basicClasses.Optimizations;

namespace basicClasses
{


    public class opis
    {
        public static bool do_not_build_debug;
        public static opis listOfVisualisedCircularRefs;
        public const int InitialArrSize = 9;
        public const int AccommSize = 4;

        public static Dictionary<string, string> LowerMap = new Dictionary<string, string>(120000);

        public static ulong TotalObjectsCreated;

#if debugCopyOpt

        public static ulong copyCacheHit;
        public static ulong copyCacheIntact;
        public static ulong copyExecTotal;
        public static ulong copyCacheModified;
        public static ulong copyBranchModified;
     
        public static ulong copySourcePresent;
        public static ulong copyCacheHitSourcePresent;
        public static ulong copyCacheHitSourceNotMatched;
        public static ulong copyCacheHitSourceNotMatchedAndThisNotMatchedSource;

        public static ulong copyCacheHitCopyIntact;
        public static ulong copyCacheHitCopyChanged;

        public static ulong duplicatedFladWhileDuplacate;
        public static ulong duplicatedFladCopyIntact;
        public static ulong duplicatedFladCopyChanged;

        public static ulong duplicateFlag13;
        public static ulong duplicateFlag1;
        public static ulong duplicateFlag2;

#endif


#if NETFRAMEWORK

        public TreeNode treeElem;
        public static opis colorChemeForModels;

#endif

        #region  variables  properties

        //  public static ObjectsFactory OF;

        string PartitionName_Lower_;
        public string PartitionName_Lower 
        {
            get
            {
                if (PartitionName_Lower_ == null && PartitionName != null)
                {
                    if (LowerMap.TryGetValue(PartitionName, out PartitionName_Lower_))
                        return PartitionName_Lower_;
                    else
                    {
                        PartitionName_Lower_ = PartitionName.ToLower();
                        LowerMap.Add(PartitionName, PartitionName_Lower_);
                    }

                    //PartitionName_Lower_ = PartitionName == null ? null : PartitionName.ToLower();
                }
                return PartitionName_Lower_;
            }

            set
            {
                PartitionName_Lower_ = value;
            }
        }

      
        public string PartitionName;

        public string PartitionKind;

#if bodyNullOptimization
        string body_;
        public virtual string body { get { return body_ ?? ""; } set { body_ = value; } }
#else
        public virtual string body { get; set; }
#endif

        public opis[] arr;
        public int paramCou;
        bool isDuplicated;
        public bool isDuplicated_
        {
            get
            {
                return isDuplicated;
            }
        }
        public opis copy; 

#if intact_copy_opt
        /// <summary>
        /// copy of this branch is valid to reuse every time duplication is requested
        /// </summary>
        public byte permaCopy;
        public List<WeakReference> allCopies;
        public opis source;
#endif

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
                return (!string.IsNullOrEmpty(body) || paramCou > 0);
            }
        }

        public int intVal
        {
            get
            {
                return StrUtils.PriceFromString(body);
            }

            set
            {
                if (string.IsNullOrEmpty(PartitionKind))
                {
                    PartitionKind = "value";
                }
                body = value.ToString();                
            }
        }
   

        public object bodyObject; //TODO: use as holder for copy; use to store indexes cache
        public object FuncObj;
  
#endregion

        //=======================================

#region partitioning word indexer

        public virtual opis this[string index]
        {
            get
            {
                return getPartition(index);
            }

            set
            {
                if (value == null)
                    return;

                int idx = -1;
                value.PartitionName = index;

                if ((idx = getPartitionIdx(index)) != -1)
                {
                    arr[idx] = value;
                  
                }
                else
                {
                    if (arr.Length <= paramCou)
                    {
                        Array.Resize(ref arr, paramCou + AccommSize);
                    }
                    arr[paramCou] = value;
                  
                    paramCou++;                   
                }
            }
        }

        /// <summary>
        /// use only to access by direct index, setter is just for optimization
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public opis this[int index]
        {
            get
            {
                return arr[index];
            }

            set              
            {
                arr[index] = value;
                if (index >= paramCou)
                    paramCou = index + 1;
            }
        }

        public virtual opis getPartition(string part)
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
                    Array.Resize(ref arr, paramCou + AccommSize);
                }
                arr[paramCou] = rez;
              
                paramCou++;                
            }

            return rez;
        }

        public virtual opis getPartitionNotInitOrigName(string part)
        {
            opis rez = null;
            int idx = -1;

            if ((idx = getPartitionIdx(part, false)) != -1)
            {
                rez = arr[idx];
            }
          
            return rez;
        }

        public virtual int getPartitionIdx(string part, bool searchLower = true)
        {
            if (part == null)
                return -1;

            //if (string.IsNullOrEmpty(part ))
            //    return -1;


            int rez = -1;

           // char pf = part.Length > 0 ? part[0]: ' ';

            int arrl = arr.Length;
            for (int i = 0; i < arrl; i++) //TODO: somehow iterating to paramCou is not finding what is found using arr.Length -- bug in keeping paramCou in sync 
            {
                if (arr[i] != null 
                    && (( arr[i].PartitionName == part)

#if !pidx_second_loop_opt                    
                     || (arr[i].PartitionName != null  && arr[i].PartitionName_Lower == part))
#else
                 )
#endif
                 )
                {
                    rez = i;
                    break;
                }
            }

#if pidx_second_loop_opt
            if (rez == -1 && searchLower)
            {
                for (int i = 0; i < paramCou; i++) 
                {
                    if (( arr[i]?.PartitionName_Lower == part))
                     
                    {
                        rez = i;
                        break;
                    }
                }
            }
#endif

            //if (rez >= paramCou)
            //{
            //    global_log.log.AddArr(new opis() { PartitionName = "fail to keep param coou in sync -> "+ this.PartitionName, body = part});
            //}


            return rez;
        }

        public virtual void RaiseEvents(bool v)
        {
            var rez = new opis() { PartitionName = "RaiseEvents not implemented " };
            rez.AddArr(this); ;
            global_log.log.AddArr(rez);
        }

       public opis getForm(string part)
        {
            opis rez;
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

        public int getPartitionIdxCharPref(string part, char pf)
        {
            if (part == null)
                return -1;
          
            int rez = -1;
                    
            for (int i = 0; i < paramCou; i++) 
            {
                string itmp = arr[i].PartitionName;
                if (!string.IsNullOrEmpty(itmp)
                    && itmp[0] == pf
                    && itmp == part)                 
                {
                    rez = i;
                    break;
                }
            }

            return rez;
        }


        #endregion

        #region constructors

        void init()
        {

#if !bodyNullOptimization

            body = "";
#endif
            TotalObjectsCreated++;
            arr = new opis[InitialArrSize];
        }

        public opis()
        {                    
            init();        
        }

        public opis(int capacity)
        {
            if (capacity > 0)
            {
#if !bodyNullOptimization

            body = "";
#endif
                TotalObjectsCreated++;
                arr = new opis[capacity];            
            } 
        }


        public opis(string PartKind, params string[] values)
        {           
            PartitionKind = PartKind;
            init();

            foreach (string pair in values)
            {
                string v = pair.Trim();
                int idx = v.IndexOf(" ");
                if (idx > 0)
                {
                    string name = v.Substring(0, idx);
                    string val = v.Substring(idx + 1);
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
        }


        public opis(string PartKind)
        {        
            PartitionKind = PartKind;
            init();         
        }

        public opis(string PartKind, object bodyp)
        {
            //bool prev = raiseEvents;
            //raiseEvents = false;

            PartitionKind = PartKind;
            bodyObject = bodyp;
            init();

            //raiseEvents = prev;
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
            //string[] arr = index.Trim().Split(new char[] { '.' });
            //return V(arr, 0); // getPartition(index).body;  

            int pos = -1;
            if ((pos = getPartitionIdx(index)) != -1)
                return this[pos].body;
            else
                return "";

        }

        public string V(string[] index, int id)
        {
            if (id >= index.Length || !this.isInitlze)
            {
                return "";
            }

            string rez = "";
            int pos = -1;
            if ((pos = getPartitionIdx(index[id])) != -1)
            {
                if (id + 1 == index.Length)
                {
                    rez = this[pos].body;
                }
                else
                {
                    rez = this[pos].V(index, id + 1);
                }
            }

            return rez;
        }


        public void Vset(string index, string data)
        {
            //if (raiseEvents) ActionDone(new opis("подія", "вид змінено тіло", "імя " + index, "тип " + getPartition(index).PartitionKind,
            //    "нове " + data, "старе " + getPartition(index).body));
            getPartition(index).body = data;
        }

        public void Vadd(string index, string data)
        {
            //if (raiseEvents) ActionDone(new opis("подія", "вид змінено тіло", "імя " + index, "тип " + getPartition(index).PartitionKind,
            //    "нове " + data, "старе " + getPartition(index).body));
            getPartition(index).body += "," + data;
        }

        //public void InitNameIndexHash()
        //{
        //    UseNameIndexHash = true;
        //    NamesIndexHash = new Dictionary<string, int>(4000);
        //}

        public void CopyArr(opis elem, bool turnOffUniqControl = true)
        {         
            arr = new opis[elem.listCou + AccommSize];
            paramCou = 0;
            AddArrRange(elem, turnOffUniqControl);
        }

        public void SetArr(opis[] narr)
        {
            arr = narr;
            paramCou = narr.Length;
        }

        public void ReinitArr(int size)
        {
            opis[] narr = new opis[size];
            ReinitArr(narr);
        }

        public void ReinitArr(opis[] narr)
        {
            arr = narr;
            paramCou = 0;
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
                        if (!arr[idx].isInitlze)
                            arr[idx] = elem[s];
                        else
                        if (arr[idx].PartitionKind == "Action")
                            arr[idx].AddArrRange(elem[s]);
                    }

                    //arr[idx].PartitionKind += "_copied";
                }
            }

        }

        public IEnumerable<opis> Where(Func<opis, bool> p)
        {
            return arr.Where(x => x != null).Where(p);
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

        public int FindArrIdx(opis instance)
        {
            int rez = -1;

            for (int i = 0; i < paramCou; i++)
            {
                if (arr[i] == instance)
                {
                    rez = i;
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

        public void AddArrRange(opis elem, bool turnOffUniqControl = true)
        {
          
            if (arr.Length < paramCou + elem.listCou + AccommSize)
                Array.Resize(ref arr, paramCou + elem.listCou + AccommSize);

            for (int i = 0; i < elem.listCou; i++)
            {
                if (turnOffUniqControl || !FindArr(elem[i]))
                {
                    arr[paramCou] = elem[i];
                    paramCou++;
                }
            }

        }

        public void AddArrMissing(opis elem)
        {
            //if (arr == null)
            //    arr = new opis[paramCou + elem.listCou + AccommSize];
            //else
            if(arr.Length < paramCou + elem.listCou + AccommSize)
                Array.Resize(ref arr, paramCou + elem.listCou + AccommSize);

            for (int i = 0; i < elem.listCou; i++)
            {
                if (!isHere(elem[i].PartitionName))
                {
                    arr[paramCou] = elem[i];
                    paramCou++;
                }
            }
        }

        public virtual int AddArr(opis elem)
        {
            PartitionKind = (string.IsNullOrEmpty(PartitionKind) ? "" : PartitionKind);

            //if (arr == null)
            //    arr = new opis[InitialArrSize];

            if (arr.Length <= paramCou)
            {
                Array.Resize(ref arr, paramCou + AccommSize);
            }

            int rez = paramCou;
           
            arr[paramCou] = elem;
            paramCou++;

       
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
            //if (raiseEvents) ActionDone(new opis("подія", "вид видалено", "імя " + elem.PartitionName,
            //    "тип " + elem.PartitionKind, "тіло " + elem.body));

            int idxLess = 0;
            for (int i = 0; i < paramCou; i++)
            {
                arr[i - idxLess] = arr[i];

                if (arr[i] == elem)
                {
                    idxLess = 1;
                    arr[i] = null;
                }
            }

            paramCou = paramCou - idxLess;

        }

        public void RemoveArrElem(int idx)
        {           
            int idxLess = 0;
            for (int i = 0; i < paramCou; i++)
            {
                arr[i - idxLess] = arr[i];

                if (i == idx)
                {
                    idxLess = 1;
                    arr[i] = null;
                }
            }

            paramCou = paramCou - idxLess;
        }

        public void RemoveArrElem(string partition)
        {
            RemoveArrElem(getPartitionIdx(partition));
        }

        public void InsertArrElem(opis elem, int pos)
        {
            if (arr.Length <= paramCou)
            {
                Array.Resize(ref arr, paramCou + AccommSize);
            }

            opis[] newArr = new opis[paramCou + 1];

            int idxShift = 0;
            for (int i = 0; i < paramCou; i++)
            {
                if (i == pos)
                {
                    newArr[i] = elem;
                    idxShift = 1;
                }

                newArr[i + idxShift] = arr[i];
            }

            arr = newArr;
            paramCou++;
        }

        public string[] ParamsNames()
        {
            string[] rez = new string[paramCou];
            for (int i = 0; i < paramCou; i++)
            {
                rez[i] = arr[i].PartitionName != null ? arr[i].PartitionName.Trim() : "";
            }

            return rez;
        }

        public string info()
        {
            string rez = PartitionKind != null && PartitionKind.Length > 0 ? PartitionKind + "\n" : "";
            for (int i = 0; i < paramCou; i++)
            {
                string dat = arr[i].body.Trim();
                int partsize = 35;
                int maxlen = partsize;
                if (dat.Length > 0 && dat != "_")
                {
                    string tmp = "";
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
                                p = dat.IndexOf(' ', prevpos + partsize);

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
            this.PartitionKind = "wrapper";
            this.body = o.PartitionName;

            this[this.body] = o; //TODO: optimization - always set this[0] = o;
        }

        public void WrapByName(opis o, string name, string renameO)
        {
            o.PartitionName = renameO;
            WrapByName(o, name);
        }

        public void WrapByName(opis o, string name)
        {
            if (o == null)
                return;
            opis ol = this[name];
            ol.PartitionKind = "wrapper";
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

        public opis W() // optimization - always set this[0] = o;
        {
            if (this.PartitionKind == "wrapper")
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
            opis wordo = new opis();
            if (getPartitionIdx(name) != -1)
            {
                wordo = this[name];
            }

            if (wordo != null && !wordo.isInitlze)// search in forms of neutral words
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

     
        public void InitFuncObj2()
        {
            if (!string.IsNullOrEmpty(PartitionName))
            {
                root r = new root();
                r.nameSpec = PartitionName;
                FuncObj = r;
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
            if (this.V("waiters") == "")
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

            RaiseEvents(autoRaise);
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
                    opis msg = new opis("подія", "вид оповіщення штучне", "імя " + ModelG.allarraycheck,
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

            IOpisFunctionalInstance i = (IOpisFunctionalInstance)FuncObj;

            return i.Process(internl, environment);
        }


#region etc

        /// <summary>
        /// check presence partition with such name (it can be not initialized, just presence is enough)
        /// </summary>
        /// <param name="part"></param>
        /// <returns></returns>
        public bool isHere(string part, bool searchLower = true)
        {
            return getPartitionIdx(part, searchLower) != -1;
        }

        public bool OptionActive(string part, bool searchLower = false)
        {
            bool rez = false;
            int idx = -1;

            if ((idx = getPartitionIdx(part, searchLower)) != -1)
            {
                rez = arr[idx].isInitlze;
            }

            return rez;
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
            sb.Append("\"N\": \"" + (PartitionName != null ? PartitionName.Replace("\"", "[&amp]") : "") + "\",");
            sb.Append("\"K\": \"" + PartitionKind + "\",");
            if (body != null)
                sb.Append("\"B\": \"" + body.Replace("\"", "[&amp]").Replace("\\", "[&bsl]") + "\"");

            if (paramCou > 0)
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

        public string ToJson(bool sub = false)
        {

            if (isDuplicated)
            {
                return "";
            }
            isDuplicated = true;

            string rez = "";
            StringBuilder sb = new StringBuilder(paramCou);

            if(!sub)
            sb.Append("{");

            sb.Append("\""+ PartitionName + "\": " );           
            if (body != null && paramCou == 0)
                sb.Append("\"" + body.Replace("\"", "[&amp]").Replace("\\", "[&bsl]") + "\"");

            if (paramCou > 0)
            {
                sb.Append("{");

                for (int i = 0; i < paramCou; i++)
                {
                    string tmp = arr[i].ToJson(true);
                    sb.Append(tmp);
                    if (i + 1 < paramCou && !string.IsNullOrEmpty(tmp))
                    {
                        sb.Append(",");
                    }
                }

                sb.Append("}");
            }

            if (!sub)
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


                if (data[idx] == '{' && idx > 0)
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
                    currtext = enclosed(data, idx, '"', '"');
                    idxShift = currtext.Length + 2;
                }

                if (data[idx] == ',' || data[idx] == ']' || data[idx] == '}')
                {
                    // it is a string or logical-numeric value 
                    if (currprop.Length > 0)
                    {
                        currtext = currtext.Replace("[&amp]", "\"").Replace("[&bsl]","\\");
                        rawtext = rawtext.Replace("[&amp]", "\"").Replace("[&bsl]", "\\");
                        if (currprop == "PN" || currprop == "N")
                        {
                            PartitionName = currtext;
                        }
                        else
                            if (currprop == "PK" || currprop == "K")
                        {
                            PartitionKind = currtext;
                        }
                        else
                                if (currprop == "B" || currprop == "body")
                        {
                            body = currtext;
                        }
                        else
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

                if (data[idx] != ' ')
                {
                    rawtext += data[idx];
                }

                idx += idxShift;
            }


            return rez;

        }

        public void JsonParce(string data)
        {
            JsonParce(data, 0);
        }

        public int JsonParce(string data, int startpos)
        {
            int idx = startpos;
            int idxShift = 1;

            string currtext = "";
            string currprop = "";
            string rawtext = "";

            //opis root = new opis();

            bool objectIsOpen = false;
            bool thisIsArray = false;

            while (idx < data.Length)
            {
                idxShift = 1;


                if (data[idx] == '{')
                {
                    currprop = "";
                    rawtext = "";
                    currtext = "";

                    if (objectIsOpen)
                    {

                        if (thisIsArray)
                        {
                            opis elem = new opis();
                            elem.PartitionName = "jsonObj";
                            idx = elem.JsonParce(data, idx);
                            this.AddArr(elem);
                            continue;
                        }
                        else
                        {
                            this["parce error"].body = "open bracket for object as for array but this item is not classified as array";
                        }

                    }
                    else
                    {
                        objectIsOpen = true;
                        this.PartitionKind = "jsonObj";
                        this.PartitionName = string.IsNullOrEmpty(PartitionName) ? "jsonObj" : PartitionName;
                    }
                }

                if (data[idx] == '[')
                {
                    thisIsArray = true;
                    objectIsOpen = true;
                    this.PartitionKind = "jsonArray";
                    this.PartitionName = string.IsNullOrEmpty(PartitionName) ? "jsonObj" : PartitionName;

                    rawtext = "";
                    currprop = "";
                    currtext = "";
                    idx += idxShift;
                    continue;
                }


                if (data[idx] == ',')
                {
                    if (thisIsArray && rawtext.Length > 0)
                    {
                        this[paramCou.ToString()].body = (currtext.Length >= rawtext.Length)
                            ? TemplatesMan.UTF8BigEndian_to_Kirill(currtext)
                            : TemplatesMan.UTF8BigEndian_to_Kirill(rawtext);
                        rawtext = "";
                        currprop = "";
                        currtext = "";
                        idx += idxShift;
                        continue;
                    }

                    if (!objectIsOpen)
                    {
                        this.body = currtext.Length > 0 ? currtext : rawtext.Trim('"');
                        this.body = TemplatesMan.UTF8BigEndian_to_Kirill(this.body);
                        return idx + 1;
                    }
                }

                if (data[idx] == '}' || data[idx] == ']')
                {
                    if (!objectIsOpen)
                    {
                        this.body = currtext.Length > 0 ? currtext : rawtext.Trim('"');
                        this.body = TemplatesMan.UTF8BigEndian_to_Kirill(this.body);
                        return idx;
                    }
                    else
                    {
                        if (thisIsArray && rawtext.Length > 0)
                        {
                            this[paramCou.ToString()].body = (currtext.Length >= rawtext.Length)
                            ? TemplatesMan.UTF8BigEndian_to_Kirill(currtext)
                            : TemplatesMan.UTF8BigEndian_to_Kirill(rawtext);
                        }
                    }
                    return idx + 1;
                }

                if (data[idx] == '"')
                {
                    currtext = enclosed(data, idx, '"', '"');
                    idxShift = currtext.Length + 2;
                }

                if (data[idx] == ':')
                {
                    if (objectIsOpen && currtext.Length > 0)
                    {
                        currprop = currtext;
                        rawtext = "";
                        currtext = "";
                        idx = this[currprop].JsonParce(data, idx + 1);
                        continue;
                    }
                    else
                        this["parce error"].body = "object is not opened but encounter colon for property value";

                }

                if (data[idx] != ' ')
                {
                    rawtext += data[idx];
                }

                idx += idxShift;
            }

            return idx;

        }

        public string enclosed(string data, int pos, char open, char close)
        {
            string rez = "";

            int openpos = data.IndexOf(open, pos);
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
                        rez = data.Substring(openpos + 1, closepos - openpos - 1);
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

            while (closepos != -1 && closepos < poslast)
            {
                qcou += data[closepos - 1] == '\\' ? 0 : 1;
                closepos = data.IndexOf('"', closepos + 1);
            }
            rez = (qcou % 2) == 0;

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

#if NETFRAMEWORK

        void RegisterCircularRefs()
        {
            if(PartitionKind !=  ModelNotion.patritionKinda)
                listOfVisualisedCircularRefs.AddArr(this);
        }

        public void RemoveFromClearRefs()
        {
            listOfVisualisedCircularRefs.RemoveArrElem(this);
        }

        public TreeNode GetDebugTree()
        {
            RegisterCircularRefs();
            return GetDebugTreeIL(200000);
        }

        public TreeNode GetDebugTree(int maxDepth)
        {
            TreeNode rez = new TreeNode("-=#Root#=-");

            RegisterCircularRefs();
            if (!do_not_build_debug)
                BuildTree(rez, 0, maxDepth, null);

            return rez;
        }

        public TreeNode GetDebugTreeIL(int maxitemsCount)
        {
            TreeNode rez = new TreeNode("-=#Root#=-");

            if (!do_not_build_debug)
                BuildTree(rez, 0, 70, new opis("", "max " + maxitemsCount.ToString()));

            return rez;
        }

      
        void BuildTree(TreeNode tn, int depth, int maxDepth, opis maxItems)
        {
            if (!isDuplicated && depth < maxDepth
                && (maxItems == null || maxItems["max"].intVal > maxItems["cou"].intVal))
            {
                if (maxItems != null)
                {
                    maxItems["cou"].intVal++;
                }

                isDuplicated = true;
                TreeNode tmp = new TreeNode(PartitionName + "[" + PartitionKind + "]  " + ((body != null && body.Length > 2000) ? "//long data//" : body));
                tmp.Tag = this;                
                treeElem = tmp;
                tmp.ForeColor = getColorOfModel(PartitionKind);

                tn.Nodes.Add(tmp);
               

                for (int i = 0; i < paramCou; i++)
                {
                    if (arr[i] != null && arr[i].PartitionName != "alreadyProcessed")
                    {
                        arr[i].BuildTree(tmp, depth + 1, maxDepth, maxItems);
                    }
                }

                if (PartitionKind == "func")
                    tmp.Nodes.Add(new TreeNode());

                isDuplicated = false;
            }
            else
            {
                TreeNode tmp = new TreeNode(PartitionName + "[ REFERENCE ] : " + body);
                tmp.Tag = this;
                tn.Nodes.Add(tmp);
            }

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

#endif

        private Color getColorOfModel(string partKind)
        {
            Color rez = Color.AntiqueWhite;

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

                default:

                    if(colorChemeForModels != null && colorChemeForModels.isHere(partKind))
                    {
                        opis c = colorChemeForModels[partKind];
                        rez = Color.FromArgb(c["r"].intVal, c["g"].intVal, c["b"].intVal);
                    }
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


       

        public int CheckConformity(opis partition, opis template)
        {
            return CheckConformity(partition, template, null);
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
                        int subtreerez = CheckConformity(partition[template[i].PartitionName], template[i], markForVisualization);

                        if (template[i].listCou != subtreerez)
                            rez--;
                    }
                }
                else
                {
                    if (markForVisualization != null)
                    {
                        if (partition.getPartitionIdx(template[i].PartitionName) == -1)
                            template[i].PartitionKind = markForVisualization;
                        else
                            template[i].PartitionKind = "modified";
                    }
                }


            }


            return rez;
        }

        public void CheckForVersionControl(opis partition, opis template, string markForVisualization)
        {

            bool checkPosition = template.listCou == partition.listCou;

            for (int i = 0; i < template.listCou; i++)
            {
                var elemN = template[i].PartitionName;

                if (partition.isHere(elemN))
                {
                    var elem_opis = partition[elemN];
                    var info = "";

                    if (elem_opis.body != template[i].body)
                        info += "   ~prev  " + elem_opis.body + "";

                    if (elem_opis.PartitionKind != template[i].PartitionKind 
                        && (!string.IsNullOrEmpty(elem_opis.PartitionKind) || !string.IsNullOrEmpty(template[i].PartitionKind))
                     //   && string.IsNullOrEmpty(elem_opis.PartitionKind) != string.IsNullOrEmpty(template[i].PartitionKind)
                        )
                        info += " / " + elem_opis.PartitionKind + " | " + template[i].PartitionKind + " /";

                    if (!string.IsNullOrEmpty(info) && elem_opis.PartitionKind != "modified" && elem_opis.PartitionKind != "moved")
                    {
                        template[i].PartitionKind = "modified";
                        template[i].body += info;
                    }

                    if (checkPosition && info.Length == 0 && elem_opis.PartitionKind != "moved"
                        && partition.getPartitionIdx(elemN) != i
                        )
                    {
                        template[i].body += template[i].PartitionKind?.Length > 0 ? " /" + template[i].PartitionKind + "/" : "";
                        template[i].PartitionKind = "moved";

                    }

                    CheckForVersionControl(elem_opis, template[i], markForVisualization);

                }
                else
                {
                    template[i].body += template[i].PartitionKind?.Length > 0 ? " /" + template[i].PartitionKind + "/" : "";
                    template[i].PartitionKind = markForVisualization;
                }

            }

        }


        public void NormalizeNamesForComparison(opis p)
        {
            if (p == null) return;

#if NETFRAMEWORK
            numArrNames.do_num(p);
#endif

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
        public static opis GetLevelByTemplate(opis templ, opis srs, bool create)
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

        public opis Branch(opis templ, bool create, bool unwrap = true)
        {
            opis rez = null;
            int idx = getPartitionIdx(templ.PartitionName);

            if (idx == -1 && create)
                idx = AddArr(new opis(templ.listCou > 0 ? templ.listCou : 1) { PartitionName = templ.PartitionName, body = templ.body });

            if (idx != -1)
            {
                opis br = unwrap ? this[idx].W() : this[idx];
                rez = br;
                for (int i = 0; i < templ.listCou; i++)
                {
                    rez = br.Branch(templ[i], create, unwrap);                  
                }
            }

            return rez;
        }

        public static bool CopyIntact(opis original, opis usedcopy)
        {
            bool rez = original.paramCou == usedcopy.paramCou;

           
            if (rez)
            {
                rez = original.body == usedcopy.body
                    && usedcopy.PartitionName == original.PartitionName
               && usedcopy.PartitionKind == original.PartitionKind;
      

                for (int i = 0; i < original.paramCou; i++)
                {
                    if (!CopyIntact(original[i], usedcopy[i]))
                    {
                        rez = false;
                        break;
                    }
                }           
            }
          
            return rez;
        }

        /// <summary>
        /// make absolute copy - no copies from cache, no optimizations
        /// </summary>
        /// <returns></returns>
        public opis DuplicateA()
        {
            if (isDuplicated)
                return new opis(1);
            
            opis rez = new opis(-1)
            {
                body = this.body,
                PartitionKind = this.PartitionKind,
                PartitionName_Lower_ = this.PartitionName_Lower_,
                PartitionName = this.PartitionName,
                paramCou = this.paramCou,
            };
           
         
            isDuplicated = true;

            if (this.paramCou > 0)
                rez.arr = new opis[this.paramCou];
            else
                rez.arr = new opis[0];

            for (int i = 0; i < this.paramCou; i++)
            {
                rez.arr[i] = this.arr[i].DuplicateA();
            }

            isDuplicated = false;

            return rez;
        }

       #if intact_copy_opt
        void DoNotTrackCopies(opis p)
        {
            p.allCopies = null;
            p.permaCopy = 3;
        }
      #endif

        public opis Duplicate()
        {

#if intact_copy_opt

#if debugCopyOpt
            copyExecTotal++;

            if ((source != null))
            {
                copySourcePresent++;
            }
          

            if (permaCopy == 1)
                duplicateFlag1++;
            if (permaCopy == 13)
                duplicateFlag13++;
            if (permaCopy == 2)
                duplicateFlag2++;

#endif




#if debugCopyOpt
            if (isDuplicated && copy != null)
            {
                duplicatedFladWhileDuplacate++;              

                if (CopyIntact(this, copy))
                {
                    duplicatedFladCopyIntact++;
                }
                else
                    duplicatedFladCopyChanged++;
            }
#endif


            // actual optimization
            if ((permaCopy == 1) && copy != null)
            {
#if debugCopyOpt
                copyCacheHit++;
                //if (source != null)
                //{
                //    copyCacheHitSourcePresent++;                   
                //}

                //if (source != null && !CopyIntact(source, copy))
                //{
                //    copyCacheHitSourceNotMatched++;
                //    if(!CopyIntact(source, this))
                //        copyCacheHitSourceNotMatchedAndThisNotMatchedSource++;
                //}

                //if (CopyIntact(this, copy))
                //{
                //    copyCacheHitCopyIntact++;                  
                //}
                //else
                //    copyCacheHitCopyChanged++;
#endif
                return copy;
            } 


#else
            if (isDuplicated && copy != null)
            {
                return copy;
            }
#endif

#if intact_copy_opt

            if (permaCopy == 13) // memory optimization - do not collect all copies if one already been modified
            {
                if (copy != null) 
                {
                    // (source == null) - this object is initial source
                    if ((source == null || CopyIntact(source, copy)) && CopyIntact(this, copy))
                    {
#if debugCopyOpt
                        copyCacheIntact++;
#endif
                    }
                    else
                    {
                        permaCopy = 3;  // this instance copy should be recreated all the time                      
#if debugCopyOpt
                        copyCacheModified++;
#endif
                    }
                }
            }
#endif


            opis rez = new opis(-1)
            {
                body = this.body,
                PartitionKind = this.PartitionKind,
                PartitionName_Lower_ = this.PartitionName_Lower_,
                PartitionName = this.PartitionName,
                paramCou = this.paramCou,
                bodyObject = bodyObject
            };

            copy = rez;


#if intact_copy_opt

            if (permaCopy == 13)
            {
                if (source == null)
                    copy.source = this;
                else
                    copy.source = source;

                copy.permaCopy = copy.source.permaCopy;

                if (source == null || source.permaCopy == 13) // if tracking is not finished
                {
                    if (allCopies == null)
                        allCopies = new List<WeakReference>();

                    allCopies.Add(new WeakReference(copy, false));
                }
                else
                    permaCopy = 3; // finish tracking according to source tracking state

            }
#endif

            isDuplicated = true;

            if (this.paramCou > 0)
                rez.arr = new opis[this.paramCou];
            else
                rez.arr = new opis[0];

            for (int i = 0; i < this.paramCou; i++)
            {
                rez.arr[i] = this.arr[i].Duplicate();             
            }

            isDuplicated = false;

#if !intact_copy_opt
            copy = null;
#else
            if (permaCopy == 0)
                copy = null;
            //else
            //    copy = rez;
#endif

            return rez;

        }


        public opis DuplicateInstrOpt(int lvl)
        {
            if (isDuplicated && copy != null)
            {
                return copy;
            }

            //func
            //Action
            //buildTreeVal_sdc_i                       
            //initValues
            //global_log

            if (PartitionKind == "func" ||  (lvl > 0 && PartitionKind == "Action") || PartitionKind == "global_log")                       
                return this;            

            // take function body from packages or SDC to exec on different spec
            if (lvl == 0 && bodyObject == null && PartitionKind == "Action")
            {

                var kch = new ModelSpecIdxPresence();
                kch.Action(this);

                //for (int i = 0; i < listCou; i++)
                //{
                //    var x = arr[i];
                //    if (!string.IsNullOrEmpty(x.PartitionKind) && x.PartitionKind != "Action" && x.bodyObject == null)
                //    {
                //        kch = new ModelSpecIdxPresence();
                //        kch.RandomModel(x);
                //    }
                //}
              
            }


            opis rez = new opis(-1)
            {
                body = body_,
                PartitionKind = this.PartitionKind,
                PartitionName_Lower_ = this.PartitionName_Lower_,
                PartitionName = this.PartitionName,
                paramCou = this.paramCou,
                bodyObject = bodyObject
            };

            copy = rez;


            isDuplicated = true;

            if (this.paramCou > 0)
                rez.arr = new opis[this.paramCou];
            else
                rez.arr = new opis[0];

            if (
              //  (lvl == 0 && PartitionKind == "Action") ||
                PartitionKind == "buildTreeVal_sdc_i" || PartitionKind == "initValues")            
            {
                for (int i = 0; i < this.paramCou; i++)
                {
                    rez.arr[i] = this.arr[i];
                }
            }
            else
            {
                for (int i = 0; i < this.paramCou; i++)
                {
                    rez.arr[i] = this.arr[i].DuplicateInstrOpt(lvl+1);
                }
            }

            isDuplicated = false;
            copy = null;

            return rez;                                                
        }


        public T DuplicateAs<T>() where T : opis, new()
        {
            T rez = new T();
            var tmp = Duplicate();
            rez.body = tmp.body;
            rez.PartitionKind = tmp.PartitionKind;
            rez.PartitionName_Lower_ = tmp.PartitionName_Lower_;
            rez.PartitionName = tmp.PartitionName;
            rez.paramCou = tmp.paramCou;
            rez.arr = tmp.arr;

            return rez;
        }
      

        public void FindTreePartitions(string modelName, string path, opis referers)
        {
            opis templ = new opis();
            templ.PartitionKind = modelName;
            FindTreePartitions(templ, path, referers);
        }

        public void FindTreePartitionsByName(string Name, string path, opis referers)
        {
            opis templ = new opis();
            templ.PartitionName = Name;
            FindTreePartitions(templ, path, referers);
        }

        public void FindTreePartitionsStrictOrFuzzy(opis templ, string path, opis referers, bool fuzzy = false)
        {
            if (fuzzy)
                FindTreePartitionsFuzzy(templ, path, referers);
            else
                FindTreePartitions(templ, path, referers);
        }

        public void FindTreePartitionsFuzzy(opis templ, string path, opis referers)
        {

            for (int i = 0; i < paramCou; i++)
            {
                if ((string.IsNullOrEmpty(templ.PartitionKind) ||
                    (arr[i].PartitionKind != null && arr[i].PartitionKind.Contains(templ.PartitionKind))) &&
                    
                    (string.IsNullOrEmpty(templ.PartitionName) ||
                    arr[i].PartitionName.Contains(templ.PartitionName)) &&
                    
                    (string.IsNullOrEmpty(templ.body) ||
                    arr[i].body.Contains(templ.body))
                    )
                {
                    opis refitem = new opis();
                    refitem.PartitionName = path + "->" + arr[i].PartitionName;
                   
                    refitem.AddArr(arr[i]);                
                    referers.AddArr(refitem);
                }
               
                arr[i].FindTreePartitionsFuzzy(templ, path + "->" + arr[i].PartitionName, referers);
            }

            isDuplicated = false;
        }

        public void FindTreePartitions(opis templ, string path, opis referers, bool recursive = true)
        {

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
                    opis refitem = new opis();
                    refitem.PartitionName = path + "->" + arr[i].PartitionName;
                    //refitem.body = arr[i].PartitionName;
                    refitem.AddArr(arr[i]);

                    referers.AddArr(refitem);

                }

                if (recursive)
                    arr[i].FindTreePartitions(templ, path + "->" + arr[i].PartitionName, referers);
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

            rez = this.isHere(kvp.PartitionName) &&  this.V(kvp.PartitionName) == kvp.body;

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

        public void RunRecursively(Action<opis> act)
        {
            if (isDuplicated)
            {
                return;
            }
            isDuplicated = true;

            act(this);

            for (int i = 0; i < paramCou; i++)
            {
                arr[i].RunRecursively(act);
            }

            isDuplicated = false;
        }

        public void RunOnItems(Action<opis> act)
        {          
            for (int i = 0; i < paramCou; i++)
            {
                act(arr[i]);
            }
           
        }

        void AddTemplData(opis templ, opis data, opis rez)
        {
            if (templ.body != "???")
            {
                var name = templ.body.Remove(0, 3).Trim();
                rez[name].body = data.body;
                rez[name].CopyArr(data, true);
            }
            else
                rez.AddArr(data);
        }

        public int FindByTemplateValue(opis strucTmpl, opis rez, bool exactOnly, bool retdata, bool isTop, bool getdata = false, bool allLevels = true)
        {

            if (isDuplicated) return 0;

            lockThisForDuplication();

            int matchedLvl = 0;
            var match = new int[strucTmpl.listCou];

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
                        (string.IsNullOrEmpty(templ.body)
                        || (templ.body[0] == '?' && templ.body.StartsWith("???"))
                        || arr[i].body == templ.body
                        || (templ.body[0] == '#' && templ.body.StartsWith("###") && arr[i].body.Contains(templ.body.Substring(3)))
                        )
                        )
                    {

                        var r = arr[i].FindByTemplateValue(templ, rez, exactOnly, retdata, false, getdata);
                        match[k] += r;

                        if (getdata && (r > 0 || templ.listCou == 0) && templ.body.StartsWith("???"))
                        {
                            AddTemplData(templ, arr[i], rez);
                        }

                        if (isTop && strucTmpl.listCou == 1 && r > 0)
                        {
                            if (!retdata)
                                rez.AddArr(arr[i]);
                            else
                            {
                                UnlockThisForDuplication();

                                if (templ.body.StartsWith("???"))
                                    AddTemplData(templ, arr[i], rez);

                                arr[i].FindByTemplateValue(templ, rez, exactOnly, retdata, false, true);

                                lockThisForDuplication();
                            }
                        }
                    }
                }
            }

            matchedLvl = match.Where(x => x > 0).Count();

            Func<int, bool> matched;
            if (!exactOnly)
                matched = (x => (x == strucTmpl.listCou));
            else
                matched = x => (x == strucTmpl.listCou && x == paramCou);

            if (isTop)
            {
                if (matched(matchedLvl)
                    && strucTmpl.listCou != 1)
                {
                    if (!retdata)
                        rez.AddArr(this);
                    else
                    {
                        UnlockThisForDuplication();
                        FindByTemplateValue(strucTmpl, rez, exactOnly, retdata, false, true);
                        lockThisForDuplication();
                    }
                }

                if (allLevels)
                {
                    for (int i = 0; i < paramCou; i++)
                        arr[i].FindByTemplateValue(strucTmpl, rez, exactOnly, retdata, true);
                }
            }

            UnlockThisForDuplication();

            return matched(matchedLvl) ? 1 : 0;

        }
       

        public int CountNodeWeight()
        {
            if (isDuplicated) return 0;
            lockThisForDuplication();

            int rez = paramCou;

            for (int i = 0; i < paramCou; i++)
            {
                rez += arr[i].CountNodeWeight();
            }

            UnlockThisForDuplication();

            return rez;
        }

        public int Difference(opis strucTmpl, bool isTop, bool mark_nodes, bool deepIsPrior, bool range_body, int koef = 1, int koefStep = 1)
        {

            if (isDuplicated) return 0;

            lockThisForDuplication();

            int matchedLvl = 0;
            var match = new int[strucTmpl.listCou];
            var matchIndexes = new int[strucTmpl.listCou];
            matchIndexes = matchIndexes.Select(x => -1).ToArray();

            for (int k = 0; k < strucTmpl.listCou; k++)
            {
                var templ = strucTmpl[k];
                var mostAlikeNode = new int[paramCou];


                // range_nodes loop
                for (int i = 0; i < paramCou; i++)
                {
                    if (arr[i].PartitionName == templ.PartitionName)
                    {
                        var r = arr[i].Difference(templ, false, false, deepIsPrior, range_body, deepIsPrior ? koef * koefStep : koef / koefStep,
                            koefStep);
                        mostAlikeNode[i] = r + 1;

                        if (range_body
                          && (arr[i].body == templ.body))
                        {
                            mostAlikeNode[i] += koef + 10;
                        }
                    }
                }


                int maxMatch = mostAlikeNode.Length > 0 ? mostAlikeNode.Max() : 0;
                match[k] += maxMatch;

                if ((isTop || mark_nodes) && mostAlikeNode.Length > 0)
                {
                    int pos = -1;
                    int idx = 0;
                    int minWeight = 1000000000;
                    int templweight = templ.CountNodeWeight();
                    int CountOfMaxRanged = mostAlikeNode.Where(x => x == maxMatch).Count();

                    if (maxMatch > 0)
                        while (idx != -1)
                        {
                            idx = Array.IndexOf(mostAlikeNode, maxMatch, idx);

                            if (idx >= 0)
                            {
                                int w = arr[idx].CountNodeWeight();

                                int used = matchIndexes.Where(x => x == idx).Count();
                                if (used > 0)
                                {
                                    idx++;
                                    continue;
                                }

                                if (minWeight > w

                                     || (CountOfMaxRanged > 1
                                     && templweight > minWeight && w > minWeight && w <= templweight)
                                  )
                                {
                                    minWeight = w;
                                    pos = idx;
                                }

                                idx++;
                            }
                        }

                    if (pos >= 0)
                    {

                        arr[pos].Difference(templ, false, true, deepIsPrior, range_body, deepIsPrior ? koef * koefStep : koef / koefStep,
                            koefStep);

                        if (templweight == minWeight)
                            matchIndexes[k] = pos;

                        string inf = "";

                        if (arr[pos].body != templ.body)
                            inf = "different body";

                        if (arr[pos].listCou != templ.listCou)
                            inf += " different listCou";

                        templ.PartitionKind = templ.PartitionName != "_path_" ? inf.Trim() : "";

                    }
                    else
                        templ.PartitionKind = "node not found";

                }
            }


            matchedLvl = koef * match.Where(x => x > 0).Count() + match.Sum()
                - Math.Abs(strucTmpl.listCou - paramCou);


            UnlockThisForDuplication();

            return matchedLvl;

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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="asc"></param>
        /// <param name="type">1 - as text; 2 - by text length; 3 - by int value  </param>
        public void SortThisArrayBy_items_pname(int type, bool asc = true)
        {
            if (type == 1)
                arr = SortArray(x => x.PartitionName, asc).ToArray();

            if (type == 2)
                arr = SortArray(x => x.PartitionName.Length, asc).ToArray();

            paramCou = paramCou >= arr.Length ? arr.Length : paramCou;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="partname"></param>
        /// <param name="type">1 - as text; 2 - by text length; 3 - by int value </param>
        /// <param name="asc"></param>
        public void SortArrayBy_pname_body(string partname, int type, bool asc = true)
        {
            if (type == 1)
                arr = SortArray(x => x[partname].body, asc).ToArray();

            if (type == 2)
                arr = SortArray(x => x[partname].body.Length, asc).ToArray();

            if (type == 3)
                arr = SortArray(x => x[partname].intVal, asc).ToArray();

            if (type == 4)
                arr = SortArray(x => { long.TryParse(x[partname].body, out long rez); return rez; }, asc).ToArray();

            paramCou = paramCou >= arr.Length ? arr.Length : paramCou;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="partname"></param>
        /// <param name="type">1 - as text; 2 - by text length; 3 - by int value </param>
        /// <param name="asc"></param>
        public void SortArrayBy_items_body(int type, bool asc = true)
        {
            if (type == 1)
                arr = SortArray(x => x.body, asc).ToArray();

            if (type == 2)
                arr = SortArray(x => x.body.Length, asc).ToArray();

            if (type == 3)
                arr = SortArray(x => x.intVal, asc).ToArray();

            if (type == 4)
                arr = SortArray(x => { long.TryParse(x.body, out long rez); return rez; }, asc).ToArray();

            paramCou = arr.Length;
        }

        public List<opis> SortArray(Func<opis, int> key, bool asc = true)
        {
            var l = arr.ToList();

            if (asc)
                return l.Where(x => x != null).OrderBy(key).ToList();
            else
                return l.Where(x => x != null).OrderByDescending(key).ToList();
        }

        public List<opis> SortArray(Func<opis, long> key, bool asc = true)
        {
            var l = arr.ToList();

            if (asc)
                return l.Where(x => x != null).OrderBy(key).ToList();
            else
                return l.Where(x => x != null).OrderByDescending(key).ToList();
        }

        public List<opis> SortArray(Func<opis, string> key, bool asc = true)
        {
            var l = arr.ToList();

            if (asc)
                return l.Where(x => x != null).OrderBy(key).ToList();
            else
                return l.Where(x => x != null).OrderByDescending(key).ToList();
        }

        public List<string> ListValues(string part)
        {
            List<string> idList = new List<string>(listCou);

            for (int i = 0; i < listCou; i++)
                idList.Add(arr[i][part].body);

            return idList;
        }

        public List<string> ListValues()
        {
            List<string> idList = new List<string>(listCou);

            for (int i = 0; i < listCou; i++)
                idList.Add(arr[i].body);

            return idList;
        }       

        public List<int> ListValuesInt(string part)
        {
            var l = ListValues(part);

            return l.Select(x => { int.TryParse(x, out int v); return v; }).ToList();
        }

        public List<string> ListPartitions()
        {
            List<string> idList = new List<string>(listCou);

            for (int i = 0; i < listCou; i++)
                idList.Add(arr[i].PartitionName);

            return idList;
        }

        public void ArrValuesFromList(List<string> l)
        {
            ReinitArr(l.Count);
            for (int i = 0; i < l.Count; i++)
                arr[i] = new opis(0) { body = l[i], PartitionName = i.ToString(), PartitionKind = "" };

            paramCou = l.Count;
        }

    }

    public class opisDictOptimized : opis
    {
        public bool UseNameIndexHash = true;
        Dictionary<string, int> NamesIndexHash = new Dictionary<string, int>(4000);

       // arr can not be null in this type

        public override opis this[string index]
        {
            get
            {
                return getPartition(index);
            }

            set
            {
                if (value == null)
                    return;

                int idx = -1;
                value.PartitionName = index;

                if ((idx = getPartitionIdx(index)) != -1)
                {
                    arr[idx] = value;                  
                }
                else
                {
                    if (arr.Length <= paramCou)
                    {
                        Array.Resize(ref arr, paramCou + 1000);
                    }
                    arr[paramCou] = value;
                   
                    NamesIndexHash.Add(index, paramCou);

                    paramCou++;
                   
                }
            }
        }
       

        public override opis getPartition(string part)
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
                    Array.Resize(ref arr, paramCou + 1000);
                }
                arr[paramCou] = rez;
                
                NamesIndexHash.Add(part, paramCou);

                paramCou++;                
            }

            return rez;
        }

        public override int getPartitionIdx(string part, bool searchLower = true)
        {
            if (part == null)
                return -1;          

            int rez = -1;

            if (UseNameIndexHash)
                rez = NamesIndexHash.TryGetValue(part, out rez) ? rez : -1;

            else
                rez = base.getPartitionIdx(part);

            return rez;
        }

        public override int AddArr(opis elem)
        {
            var rez = base.AddArr(elem);

            if (!NamesIndexHash.ContainsKey(elem.PartitionName))
                NamesIndexHash.Add(elem.PartitionName, rez);
            else
            {
                var ei = new opis() { PartitionName = "ERROR: AddArr UseNameIndexHash not uniq name -" + elem.PartitionName };
                ei.AddArr(elem.DuplicateA());
                global_log.log?.AddArr(ei);
            }

            return rez;

        }

    }

    public class opisDuplicationOptimized : opis
    {
        public int CopyDepth;

        //public override opis Duplicate(int deep = 0)
        //{
        //    return DuplicateLlv(CopyDepth);
        //}
    }

    public class opisEventsSubscription : opis
    {
     

        bool raiseEvents;

        string body_;
        public override string body
        {
            get
            {
                return body_;
            }

            set
            {
                if (raiseEvents)
                {
                    ActionDone(new opis("подія", "вид змінено тіло", "імя " + PartitionName, "тип " + PartitionKind,
   "нове " + value, "старе " + body, "позиція " + paramCou.ToString()));
                }

                body_ = value;
            }

        }

        public override void RaiseEvents(bool v)
        {
            raiseEvents = v;
        }

        public override opis this[string index]
        {
            get
            {
                return getPartition(index);
            }

            set
            {
                if (value == null)
                    return;

                int idx = -1;
                value.PartitionName = index;

                if ((idx = getPartitionIdx(index)) != -1)
                {
                    arr[idx] = value;

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
                        Array.Resize(ref arr, paramCou + AccommSize);
                    }
                    arr[paramCou] = value;
                  

                    paramCou++;

                    if (raiseEvents)
                    {
                        ActionDone(new opis("подія", "вид додано", "імя " + index,
                            "тип " + value.PartitionKind, "тіло " + value.body, "позиція " + (paramCou - 1).ToString()));
                    }
                }
            }
        }

        public override int AddArr(opis elem)
        {
            var rez = base.AddArr(elem);

            if (raiseEvents) ActionDone(new opis("подія", "вид додано", "імя " + elem.PartitionName,
               "тип " + elem.PartitionKind, "тіло " + elem.body, "позиція " + (paramCou - 1).ToString()));

            return rez;

        }

        //public object bodyObject;
        //public object FuncObj;


    }



}
