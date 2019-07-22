using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using basicClasses.models;
using basicClasses.Factory;
using System.Threading;
using basicClasses.models.sys_ext;

using basicClasses.models.SharedDataContextDrivers;
using basicClasses.models.Actions;
using System.Threading.Tasks;

namespace basicClasses
{
    public struct MsgLogItem
    {
       public string info;
       public opis o;
        public override string ToString()
        {
            return info;
        }

        public bool haveResponderOrSender(opis clicked)
        {
            bool rez = false;

            opis j = o["THIS_INS_final"].W();

            if ((j["spec"].body == clicked.PartitionName ||
                j["spec"].body == clicked.PartitionKind)
                && (j["waiter"].body == clicked.PartitionKind ||
                clicked.PartitionKind =="системное"))
            {
                rez = true;
            }

            opis branch = j["Tags"]["Tags"]["contextBranch"].Duplicate();

            opis bb = clicked;

            if (branch["ID"].body.Contains(clicked.PartitionName) )         
            {
                while (bb.treeElem.Parent != null)
                {
                    bb = ((opis)bb.treeElem.Parent.Tag);

                    branch = branch.W(context.Higher);
                    if(!branch["ID"].body.Contains(bb.PartitionName)
                        && bb.PartitionName != "notion_tree")
                    {
                        rez = false;
                    }
                }
            }else
            {
                if(bb.treeElem.Parent != null)
                bb = ((opis)bb.treeElem.Parent.Tag);
               
                while (bb.treeElem.Parent != null)
                {
                    bb = ((opis)bb.treeElem.Parent.Tag);

                    branch = branch.W(context.Higher);
                    if (!branch["ID"].body.Contains(bb.PartitionName)
                        && bb.PartitionName != "notion_tree")
                    {
                        rez = false;
                    }
                }

            }

            return rez;
        }
    }

    public delegate void ProcessContextDelegate(opis param);
    public delegate void ProcessContextDelegateParam(opis context, opis param);

    public delegate void guiGelegate();

    public  class SysInstance : IOpisFunctionalInstance
    {
        public static List<MsgLogItem> Log;
        public static opis Words;
         ModelFactory _mf;
        public  ModelFactory MF
        {
            get
            {
                if (_mf == null)
                {
                    _mf = new ModelFactory();
                }

                return _mf;
            }
        }

        public static Form1 callbackFrm;
        public static guiGelegate updateform;
        public static string messageBannertext;

      
        protected opis thisins;
        protected opis spec;
        protected opis o;
        protected opis communicator;
        protected opis contexts;
        protected contex CTX;
        protected opis waiter;
        protected opis curr_msgProc;
        protected bool cacheAnwers;

        public static bool debugNext;
        public static bool showOverrideWarnings;

        public static object oddLocker = new object();
        public static object evenLocker = new object();
        public static object modelLocker = new object();

        Stack<string> tempSDCstack;   
        Random rnd;

        public static void AddLog(MsgLogItem item)
        {
            if (Log == null)
            {
                Log = new List<MsgLogItem>(1000);
            }

            Log.Add(item);
        }

        public void AddInstLog(string method, string message, opis p)
        {
            opis bbb = new opis();

            bbb.WrapByName(p, "inf");
            //bbb.AddArr(p);
            //bbb.WrapByName(thisins.Duplicate(), "THIS_INS");
            bbb.WrapByName(thisins, "THIS_INS_final");
            //bbb.AddArr(thisins["Models_log"].Duplicate());

            string composed = "";
            if (thisins.W("spec").isInitlze)
            {
                composed += thisins.W("spec").PartitionName;
            } else
            {
                composed += thisins.PartitionName;
            }

            composed = composed.PadRight(13);

            composed += " "+ method +"  " ;

            if (method.Length > 5)
                composed = composed.PadRight(37);

            MsgLogItem li = new MsgLogItem();

            if (method == "msg")
            {
                composed +=  p.body;              
            }

            if (method == "ans" )
            {
                composed += message+"  " + p.body;            
            }

            if (method == "in")
            {
                if(p["waiters"][0] != null)
                composed = p["rec"].body.PadRight(13)
                  + " " + method + "   " + message ;
            }

            composed = composed.PadRight(40);

            composed += " (" + o.V(models.context.Organizer) + ")";

            bbb.AddArr(global_log.log);
            li.info = composed;
            li.o = bbb;

            AddLog(li);


            if (Log.Count % 2 == 0)
            {
                lock (oddLocker)
                {                   
                }
            }
            else
            {
                lock (evenLocker)
                {
                }               
            }
        }

        public static void IterateLockThread()
        {
            while (true)
            {
                debugNext = false;

                lock (evenLocker)
                {
                    while (!debugNext)
                    {
                        Thread.Sleep(200);
                    }
                }
                debugNext = false;

                lock (oddLocker)
                {
                    while (!debugNext)
                    {
                        Thread.Sleep(200);
                    }
                }
            }
                                              
        }

        public void PauseThread()
        {
            lock (modelLocker)
            {
            }

            Thread.Sleep(700);
        }

        public static void ModelStopLockThread()
        {
            while (true)
            {
                debugNext = false;

                lock (modelLocker)
                {
                    while (!debugNext)
                    {
                        Thread.Sleep(200);
                    }
                }
                debugNext = false;
                //Thread.Sleep(100);

            }

        }

        public void updateGui()
        {
            if (updateform != null)
            {
                callbackFrm.Invoke(updateform);
            }
        }

        #region stuff

        bool useLocalName;
        string LocalName;

        public string name { get
            {
                if(!useLocalName)
                return Name();
                else return LocalName;
            }
        }

        public opis curr
        {
            get
            {
                return o;
            }
        }

       /// <summary>
        /// change working context and return specifications of instance in given context
       /// </summary>
       /// <param name="p"></param>
       /// <returns> specifications of instance of given context </returns>
        public opis Handle(opis p)
        {
            if (p.PartitionKind == "context")
            {
                o = p;
                CTX.Handle(o);
            }
            thisins = o[ModelG.instance][name];
            spec = thisins.W(ModelG.spec);

            return thisins;
        }

        /// <summary>
        /// check if such context is present in contexts array and if so switch to this context
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public bool CanHandle(opis p)
        {
            bool rez = false;

            for (int i = 0; i < contexts.listCou; i++)
            {
                if (contexts[i].V("ID") == p.V("ID"))
                {
                    Handle(contexts[i]);
                    rez = true;
                    break;
                }           
            }

            return rez;
        }

        public virtual string Name()
        {
            return "SysInstance";
        }

        public opis GetEnvirinmentForModel()
        {
            opis rez = null;
            if (thisins.isHere("environment"))
            {
                rez = thisins["environment"];
            }
            else
            {
                rez = new opis("EnvirinmentForModel");
                rez.WrapByName(thisins, "thisins");
                rez.WrapByName(spec, "spec");
                //rez.WrapByName(communicator, "communicator");
                rez.WrapByName(o, "currCont");
                rez.WrapByName(waiter, "waiter");
                rez.WrapByName(contexts, "contexts");

                thisins["environment"] = rez;
            }      

            return rez;
        }

        #endregion

 // ==========================================================================

        #region VIRTUAL methods

        // A derived class has access to the public, protected, internal, and protected internal members 
        // of a base class. Even though a derived class inherits the private members of a base class, 
        // it cannot access those members.


        // for reading answers to own broadcast messages or requests
        public virtual void ReceiveSubscriptions(opis evento, opis sender)
        {

            #region A N S W E R

            if (evento.V("імя") == Msg.answer
               && evento.V("тип") == Msg.answer)
            {
                opis answer = sender[Msg.answer];
                // sender is the original message where you can find type of request and others
                // this is answer for previously sended message, it will contain detailed info about
                // instance which decided to answer 
                /* answer["context"] specific context of instance
                   answer["notion"].body  name of instance
                   answer.WrapByName(waiter, "contact")  contain wrapper for instanse direct contact object 
                 */

            }

            #endregion

        }

        // broadcast messages or requests
        public virtual void CheckMessage(opis message)
        {
           
        }

       
        #endregion 

 // ==========================================================================

        #region IOpisFunctionalInstance {


        public virtual opis Process(string internl, opis environment)
        {
            opis rez = new opis("process_accompl_status");

            switch (internl)
            {
                case "bind":
                    rez.Vset("command", "accepted");
                    bind(environment);
                    break;

                //case "scanEnv":
                //    rez.Vset("command", "accepted");
                //    SendRequestFromAllContexts("all", "present_here");                             
                //    break;        
            }

            return rez;
        }

        // communicate between instances etc
        public void ProcessWaiter(opis evento, opis sender)
        {
           
            if (!WaiterAsMessage(evento, sender) )
            {
                ReceiveSubscriptionsBase(evento, sender);                       
            }
            else
            {
                opis message = sender[evento["позиція"].intVal];
                //CheckCommonMessage(message);
                if (!CheckCommonMessage(message))
                {
                    CheckMessage(message);
                }
            }

        }

        protected bool WaiterAsMessage(opis evento, opis sender)
        {
            bool rez = false;

            string receiverName = evento.V(ModelOpisEvent.initiatorPartName);
           
            if (sender.PartitionKind == "communicator"
                && (IsOneOfTheForm(receiverName) && name.Length > 1 || receiverName == "all")
                && evento.V(ModelOpisEvent.initiatorPartKind) == "message")
            {
                rez = true;
            }

            return rez;
        }

        public bool IsOneOfTheForm(string f)
        {

            return f.Contains(name);
        }

        #endregion  IOpisFunctionalInstance }

 // ==========================================================================

        #region internl commands that accepted by base class


        protected void bind(opis context)
        {
             rnd = new Random();
            tempSDCstack = new Stack<string>();          

            communicator = context["globalcomm"];
            contexts = context["sys"][name];
            waiter = context["sys_instances"][name];
            CTX = new contex(waiter[ModelNotion.canHandleParentRange].body);
            // subscribe this instance to context globalcomm messaging
            context["globalcomm"].WaitAction(waiter);

            waiter.Vset("binded", "sucsses");
            BuildActionPath(waiter);

            ToAll(initInstanceSpec, null);
           
        }

        public void CreateInstance(string instName)
        {
            SysInstance lbi = new SysInstance();
            lbi.useLocalName =true;
            lbi.LocalName = instName;
           
            lbi.laterbind(instName, communicator);
        }

        protected void laterbind(string instName, opis communicatorParam)
        {
            CTX = new contex();
          
            spec = GetWordForm(instName).Duplicate();
            waiter = spec.Duplicate();
            BuildActionPath(waiter);
            BuildActionPath(spec);

            contexts = new opis();
            contexts.AddArr(new opis("context"));
           
         
            waiter.FuncObj = this;

            communicator = communicatorParam;
            communicator.WaitAction(waiter);

            thisins = contexts[0][ModelG.instance][name];
            thisins.WrapByName(spec, "spec");
            thisins.WrapByName(waiter, "waiter");

            Handle(contexts[0]);
           

            ExecActionResponceModelsList(waiter[ModelNotion.Build], new opis());
            
        }

        public void initInstanceSpec(opis msg)
        {        
            if (CTX.organizer.Contains(name))
            {
                CTX.Handle(CTX.GoUp());
            }

           
            opis rez = new opis();
            
            opis t = CTX.GetItemTypeList(name, false);
            if (t.listCou > 0)
            {
                for(int i=0; i < t.listCou; i++)
                {
                    if(!t[i].isHere("binded"))
                    {
                        rez = t[i];
                        t[i].Vset("binded", "yes");
                        break;
                    }
                }
            }

            if (!rez.isInitlze)
                rez = CTX.GetItemName(name);

            BuildActionPath(rez);
            if(rez.PartitionName != waiter.PartitionName)
            BuildActionPath(waiter);

            thisins.WrapByName(rez, "spec");
            rez["spec_name"].body = rez.PartitionName;
            thisins.WrapByName(waiter, "waiter");

            // optimize - at start set enough size for this big and very active array
            thisins["sharedVariablesContext"].InitNameIndexHash(); //TODO: check for bugs caused by this optimization
            thisins["sharedVariablesContext"].ArrResize(400);
       
            thisins["packages"].ArrResize(400);
            thisins["packages"].bodyObject = new Dictionary<string, int>(200);

            spec = thisins.W(ModelG.spec);

            ExecActionResponceModelsList(waiter[ModelNotion.Build], rez);
            ExecActionResponceModelsList(rez[ModelNotion.Build], rez);
        }

        protected bool CanRunOnThisContext(opis msg)
        {
            bool rez = false;
            if (msg[MsgTemplate.contTargetModel].isInitlze)
            {
                opis tmp = new opis();
                tmp.PartitionName = "crit";
                tmp["original_msg_context"] = contex.GetHierarchyStub(msg[ModelAnswer.context]);
                tmp[MsgTemplate.sender_tags] = msg[MsgTemplate.sender_tags];

                //Handle(contexts[i]);
                ExecActionResponceModelsList(msg[MsgTemplate.contTargetModel], tmp);
                if (tmp["run_on_this_context"].isInitlze)
                    rez = true;

            }

            return rez;
        }

        protected bool CheckMessageTarget(opis msg)
        {
            bool rez = false;
            if (msg[MsgTemplate.contTargetModel].isInitlze)
            {

                for (int i = 0; i < contexts.listCou; i++)
                {                   
                    Handle(contexts[i]);             
                    if (CanRunOnThisContext( msg))
                        rez = true;
                }
            }

            return rez;
        }

        public void ModelAnswerMsg(opis ans)
        {
            if (curr_msgProc != null)
            {           
                AnswerToMessage(curr_msgProc, ans);
            }
        }

        public void ModelAnswerMsg(opis msg, opis ans)
        {
            AnswerToMessage(msg, ans);
        }

        opis InitMsgStackDatCon(opis msg)
        {
            var SVC = thisins["sharedVariablesContext"]["SYS_use_container_do_not_owerlap"];
            var prev = SVC.W();

            var contl = new opis() { PartitionName = "msgCont" };
          
            var wr = new opis() { PartitionName = "cmsg" }; 
            wr.CopyArr(msg);
            contl.AddArr(wr);

            wr = new opis() { PartitionName = "msg p" }; 
            wr.Wrap(msg["p"]);

            contl["cancel"].body = "";

            contl.AddArr(wr);

            SVC.Wrap(contl);

            return prev;
        }

        void SetStackDatCon(opis data)
        {
            var SVC = thisins["sharedVariablesContext"]["SYS_use_container_do_not_owerlap"];
   
            SVC.Wrap(data);
        }

        protected bool CheckCommonMessage(opis msg)
        {
            bool rez = false;
            //bool msgHandled = false;           

            if (  CanHandle(msg["context"]) || CheckMessageTarget(msg)  ) 
            {
                string mb = msg.body;
                if (msg[MsgTemplate.contTargetModel].isInitlze)
                {
                    for (int i = 0; i < contexts.listCou; i++)
                    {
                        Handle(contexts[i]);
                        if (CanRunOnThisContext(msg))
                        {                          
                           var prevDat = InitMsgStackDatCon(msg);

                            thisins["active_message"] = new opis();
                            thisins.WrapByName(msg, "active_message");
                            curr_msgProc = msg;

                            if(!msg.isHere("hide"))
                            AddInstLog("in", msg.V("msg"), msg);
                           
                            ProcessResponcesPartition(msg);                         
                          
                            SetStackDatCon(prevDat);
                        }
                    }
                }
                else
                {
                    var prevDat = InitMsgStackDatCon(msg);

                    thisins["active_message"] = new opis();
                    thisins.WrapByName(msg, "active_message");
                    curr_msgProc = msg;

                    if (!msg.isHere("hide"))
                        AddInstLog("in", msg.V("msg"), msg);

                    ProcessResponcesPartition(msg);

                    SetStackDatCon(prevDat);
                }
            }

            switch (msg.body)
            {                            
                case "start":
                    Start(msg);                  
                    break;

                case "GenerateTags":
                    for (int i = 0; i < contexts.listCou; i++)
                    {
                        Handle(contexts[i]);

                        var prevDat = InitMsgStackDatCon(msg);
                        ProcessResponcesPartition(msg);
                        SetStackDatCon(prevDat);
                    }

                    break;

            }

            return rez;
        }

       // process broadcast request and answer if needed
        protected void MsgRequestPresentHere(opis msg)
        {
            for(int i =0; i < contexts.listCou;i++)
            {              
                // is there some context of this notion that lies under given context
                if (CTX.CheckParentOrder(msg["context"], contexts[i]))
                {
                    Handle(contexts[i]);
                    AnswerToMessage(msg, "rez ischild");                                
                }
            }
        }

        // process broadcast request and answer if needed
        protected void MsgRequestExistAsOwner(opis msg)
        {
            for (int i = 0; i < contexts.listCou; i++)
            {               
                if (CTX.CheckParentOrder(contexts[i], msg["context"]))
                {
                    Handle(contexts[i]);
                    AnswerToMessage(msg, "rez isparent");
                }
            }
        }
    

        protected void Start(opis msg)
        {
            for (int i = 0; i < contexts.listCou; i++)
            {
                Handle(contexts[i]);                

                ProcessStartPartition(msg);
            }          
        }


        #region Executing models 

        string GetContextOrganizerName()
        {
            string rez = CTX.organizer;
            if (CTX.organizer.Contains(name))
            {
                rez = contex.org(CTX.GoUp());
            }

            return rez;
        }

        protected void ProcessStartPartition(opis msg)
        {
            string organizer = GetContextOrganizerName();          

            // сначала запускаем базовую а потом на основе полученного стартуем спецификацией
            if (spec.PartitionName != waiter.PartitionName)
                RunNotionReactionStart(waiter, ModelNotion.Start, organizer, msg);
            RunNotionReactionStart(spec, ModelNotion.Start, organizer, msg);
          
        }

        void RunMethodSubscribersCode(opis msg, opis container, string systemName)
        {
            var rootpath = RootPathMsg(systemName, msg.body);

            LocalizePathDecorateAspectAndRun(
               container["hooks"][msg.body],
               rootpath + "Hooks", false);

        }

        opis GetBreakerForMessageInjection()
        {
            opis rez = new opis();

            opis compl = new opis() { PartitionKind = "Breaker" };
            compl.AddArr(new opis()
            {
                body = "*cancel",
                PartitionName = "condition",
                PartitionKind = "fill"
            });

            compl.AddArr(new opis()
            {
                body = "yes",
                PartitionName = "setLdcExitOnCondition"
            });

            rez.AddArr(compl);

            rez.AddArr(new opis() { PartitionKind = "Breaker" });

            return rez;
        }

        string RootPathMsg(string systemName, string msg)
        {
            var rootpath = systemName + "->Responces->" + GetContextOrganizerName() + "->" + msg + "->aspects->";

            return rootpath;
        }

        void RunMethodAspectBeforeCode(opis msg, opis container, string systemName)
        {
            var rootpath = RootPathMsg(systemName, msg.body);
             
            // can disable whole object
            ExecActionModelsList(container["aspects"]["All"]["before"]);

            LocalizePathDecorateAspectAndRun(
                container["aspects"][msg.body]["before"], 
                rootpath + "before");
          
            LocalizePathDecorateAspectAndRun(msg[MsgTemplate.preProcess], rootpath + "Msg_before");          
        }

        void LocalizePathDecorateAspectAndRun(opis code, string rootpath, bool decor= true)
        {
            code = code.Duplicate();
            code.PartitionName = rootpath;
            BuildActionPathByName(code, "_path_");
            if (decor)
                code = DecorateCodeBy(code, GetBreakerForMessageInjection());

            ExecActionModelsList(code);

        }

        void RunMethodAspectAfterCode(opis msg, opis container, string systemName)
        {
            var rootpath = RootPathMsg(systemName, msg.body);
          
            LocalizePathDecorateAspectAndRun(
                container["aspects"][msg.body]["after"],
                rootpath+ "after");

            if(msg.V(MsgTemplate.msg) == "put function packages") //optimization
                ExecActionModelsList(msg[MsgTemplate.getAnswerDetails]);
            else
            LocalizePathDecorateAspectAndRun(msg[MsgTemplate.getAnswerDetails], 
               rootpath + "Msg_After");
         

        }

        opis DecorateCodeBy(opis code, opis start, opis end = null)
        {
            var rez = new opis();
          
            if (start != null)
            {
                if (start.listCou > 0)
                    rez.AddArrRange(start);
                else
                    rez.AddArr(start);
            }

            rez.AddArrRange(code);

            if (end != null)
            {
                if (end.listCou > 0)
                    rez.AddArrRange(end);
                else
                    rez.AddArr(end);
            }

            return rez;
        }

        opis DecorateMethodByAspectCode(opis code, opis container, opis msg)
        {                     
            return DecorateCodeBy(code, new opis() { PartitionKind = "Breaker" }, null);           
        }

        /// <summary>
        /// execute action models from spec & waiter 
        /// <para>spec[ModelNotion.Responces][CTX.organizer];</para>
        /// <para> opis responses = rrr[msg.body];</para>
        /// </summary>
        /// <param name="msg"></param>
        protected void ProcessResponcesPartition(opis msg)
        {
            RunMethodAspectBeforeCode(msg, spec, waiter.PartitionName);

            string organizer = GetContextOrganizerName();        
            RunNotionReaction(spec, ModelNotion.Responces, organizer, msg);
       
            if (spec.PartitionName != waiter.PartitionName)            
                RunNotionReaction(waiter, ModelNotion.Responces, organizer, msg);


            RunMethodAspectAfterCode(msg, spec, waiter.PartitionName);
            RunMethodSubscribersCode(msg, spec, waiter.PartitionName);
        }

        public void RunNotionReaction(opis notion, string partition, string organizer, opis msg)
        {
            opis responses = notion[partition][organizer][msg.body];
            responses = DecorateMethodByAspectCode(responses, spec, msg);
            ExecActionResponceModelsList(responses, msg);

            responses = notion[partition]["all"][msg.body];
            responses = DecorateMethodByAspectCode(responses, spec, msg);
            ExecActionResponceModelsList(responses, msg);
        }

        public void RunNotionReactionStart(opis notion, string partition, string organizer, opis msg)
        {
            opis responses = notion[partition]["all"];
            ExecMessageModelsAndSend(responses); // for compatibility
            ExecActionResponceModelsList(responses, msg);

            responses = notion[partition][organizer];
            ExecMessageModelsAndSend(responses);
            ExecActionResponceModelsList(responses, msg);          
        }

        public opis GetWordForm(string f )
        {
            opis rez = Words.Find(f);
      
            return rez;
        }

        #region Message template exec

        public void ExecMessageModelsAndSend(opis requests)
        {
            for (int i = 0; i < requests.listCou; i++)
            {
                var req = requests[i].Duplicate();
                FillMessagePartsAndSend(req);
            }
        }

       public void FillMessagePartsAndSend(opis req)
        {
            if (req.PartitionKind == "MsgTemplate")
            {
                thisins["Models_log"]["instance_msg_aggr"] = (new opis("fill message params", "body ___"));
                ExecParamModels(req[MsgTemplate.p]);// do not run P models if p itself has a filler
                ExecParamModels(req);
                //ExecParamModels(req[MsgTemplate.p]);
                thisins["Models_log"]["instance_msg_aggr"].PartitionName = "msg_composion " + req.V(MsgTemplate.msg);

               ExecActionResponceModelsList(req[Messenger.Features], req);

                ThisRequest(req.V(MsgTemplate.msg_receiver), req.V(MsgTemplate.msg), req);             
            }
        }

        public void ExecParamModels(opis req)
        {
            //here will be processed all partitions in opis
            //retrieved corresponding model from part and 
            // result of action model be saved in part body
            opis activeCont = o;

            for (int i = 0; i < req.listCou; i++)
            {
                Handle(activeCont);
                if (!string.IsNullOrEmpty(req[i].PartitionKind))
                {
                    ExecGivenParamModel(req[i]);                   
                }
            }
        }

        public void ExecGivenParamModel(opis req)
        {
            opis ms = thisins["sharedVariablesContext"]["modelSpec"];//.Duplicate();

            opis elemToChange = req;
            req = GetLocalDataModel(req); // local models

            object obj = MF.GetModelObject(req.PartitionKind);
            if (obj is IActionProcessor)
            {
                IActionProcessor processor = (IActionProcessor)obj;
                processor.InitAct(this);
                processor.CpecifyActionModel(req);
                processor.Process(elemToChange);
                if (req.PartitionName != MsgTemplate.context)
                    processor.log.AddArr(req);
                processor.log.body = req.PartitionName;
            }

            thisins["sharedVariablesContext"]["modelSpec"] = ms;
        }

        #endregion


        public void ExecActionModelsList(opis req, bool parallel = false)
        {
           opis marg = thisins["sharedVariablesContext"][exec.SUBJ];
          
                opis wr = new opis();
                wr.Wrap(req);
                thisins["sharedVariablesContext"][exec.SUBJ] = wr;


            if (!parallel)
            {
                opis activeCont = o;
                for (int i = 0; i < req.listCou; i++)
                {
                    ExecActionModel(req[i], req[i]);
                    Handle(activeCont);

                    if (req[i].PartitionKind == "Breaker" && req[i].isHere(Breaker.flag))
                        break;
                }
            }else
            {              
                Handle(o);
                Task[] tasks = new Task[req.listCou];
                for (int i = 0; i < req.listCou; i++)
                {
                    tasks[i] = Task.Run(() => ExecActionModel(req[i], req[i]));
                }
                //{
                //     new Task(() => Console.WriteLine("First Task")),              
                //};
                //foreach (var t in tasks)
                //    t.Start();
                Task.WaitAll(tasks);
            }

            thisins["sharedVariablesContext"][exec.SUBJ] = marg;
        }


        public void ExecActionResponceModelsList(opis req, opis message)
        {
            opis marg = thisins["sharedVariablesContext"][exec.SUBJ];
           
                opis wr = new opis();
                wr.Wrap(message);
                thisins["sharedVariablesContext"][exec.SUBJ] = wr;

            //на випадок коли інстанс може отримувати повідомлення
            // чи відповіді в процесі виконання попереднього циклу
            // отримавши повідомлення змінюється поточний контекст інстанса
            // і через це наступний цикл не має доступу до SDC - виникає збій
            opis activeCont = o;
            for (int i = 0; i < req.listCou; i++)
            {               
                ExecActionModel(req[i], message);
                Handle(activeCont);

                if (req[i].PartitionKind == "Breaker" && message.isHere(Breaker.flag))
                    break;
            }                 

            thisins["sharedVariablesContext"][exec.SUBJ] = marg;
        }     

        public void ExecActionModel(opis req, opis processParameter)
        {        
            if (req !=null && !string.IsNullOrEmpty(req.PartitionKind))
            {
                if (!IsLocalDataModel(req, processParameter))
                {
                    object obj = MF.GetModelObject(req.PartitionKind);
                    if (obj is IActionProcessor)
                    {
                       
                        IActionProcessor processor = (IActionProcessor)obj;
                        processor.InitAct(this);
                        processor.CpecifyActionModel(req);

                        processor.Process(processParameter);
                    }
                }
            }
        }
      
        bool IsLocalDataModel(opis req, opis processParameter)
        {
            bool rezb = false;
            opis rez = req;
            opis SVC = thisins["sharedVariablesContext"];
            bool inSvc =false;

            Dictionary<string, int> cash = (Dictionary<string, int>)thisins["packages"].bodyObject;
            int poz = -1;
          
            inSvc = (SVC.isHere(req.PartitionKind)
                && SVC[req.PartitionKind].PartitionKind == "Action");


            // ---------  only for dev time in repl
            int tmppos = -1;
            if (showOverrideWarnings && inSvc &&
              cash.TryGetValue(req.PartitionKind, out tmppos))
                {
                    opis err = new opis();
                    err.PartitionName = "WARN: local model override package func: " + req.PartitionKind;
                    err.AddArr(req.Duplicate());
                    global_log.log.AddArr(err);
                }
            // --------- 


            if (inSvc ||
                cash.TryGetValue(req.PartitionKind, out poz) )
            {

                List<string> tempNames = new List<string>();

                opis ms = SVC["modelSpec"];
                rezb = true;
                opis mod = inSvc ? SVC[req.PartitionKind] : thisins["packages"][poz];

                mod = poz >=0 ? thisins["packages"][poz] : mod;

                opis modelSpec = new opis();
                bool modelIsProducer = (mod.body != null && mod.body.Contains("@"));
                

               #region create container for model execution

                rez = new opis();
                rez.PartitionKind = "exec";

                rez[exec.message_as_parameter_for_instructions].body = "y";
                rez[exec.instructions].CopyArr(mod);

                #endregion

                #region create modelSpec

                if (!mod.body.Contains("!"))
                {
                    //копируем на случай если модель должна выступать в роли филлера
                    //ведь тогда мы изменим processParameter который будет являться и modelSpec
                    modelSpec.CopyArr(req.Duplicate());

                    if (!modelSpec.isHere("v"))
                        modelSpec.Vset("v", req.body);
                    if (!modelSpec.isHere("a"))
                        modelSpec.Vset("a", req.PartitionName);


                    if (!mod.body.Contains("#"))// not all items should run param functions        
                        ExecActionModelsList(modelSpec);                    

                    if (mod.body.Contains("*"))// add context items as missed parameters(ms) 
                        modelSpec.AddArrMissing(getSYSContainetP(SVC, ""));

                    if (mod.body.Contains("$"))// duplicate all input data 
                        modelSpec = modelSpec.Duplicate();

                    SVC["modelSpec"] = modelSpec;
                }
                else
                {
                    if (mod.body.Contains("|"))
                    {
                        rez.PartitionKind = "exec_inline";
                    }
                }
           
                #endregion

                #region partial application

                //if (mod.body == "%")
                //{
                //    modelSpec.CopyArr(req.Duplicate());

                //    opis r = GetPartialRec_LocalDataModel(mod[0], modelSpec);
                //    mod = r;
                //    rez[exec.instructions].CopyArr(r);

                //    modelSpec.Vset("v", req.body);
                //    modelSpec.Vset("a", req.PartitionName);
                //}

                #endregion

              
                string b = req.body !=null ? req.body:"";
                opis processObj = processParameter;
                string nameOfSubj = "";
                string nameOfSubjForFiller = "";

                if (b.Contains("<"))
                {
                    req.body = "";
                    processObj = req;//filler
                   
                    if (modelIsProducer)
                    {
                        nameOfSubj = GetTempValName(SVC, tempNames);                        
                        SVC[nameOfSubj].Wrap(req);
                        modelSpec.Vset("v", nameOfSubj);
                        nameOfSubjForFiller = nameOfSubj;
                    }                             
                }

                                       
                #region hook input object

                if (req.PartitionName.Contains(">"))
                {
                    processObj = SVC[exec.SUBJ].W();//composition

                    string pn = req.PartitionName;

                    #region  input in form of <filler model>
                    if (pn.Contains("<")) // when input in form of <filler model> its exec filler and put result in tmp arg
                    {
                        opis paramRez = new opis();
                        paramRez.PartitionName = "stub inp";

                        pn = pn.Trim('>', '<');
                        if (pn.Length > 0)
                        {                          
                            SVC["modelSpec"] = ms;
                          
                            paramRez.PartitionKind = pn;
                            ExecActionModel(paramRez, paramRez);
                            processObj = paramRez.W();
                         
                            SVC["modelSpec"] = modelSpec;
                        }
                    }

                    #endregion

                    nameOfSubj = GetTempValName(SVC, tempNames) ;

                    SVC[nameOfSubj].Wrap(processObj);
                    modelSpec.Vset("a", nameOfSubj);
                                       
                }

                if (req.PartitionName.Contains("*"))
                {
                    nameOfSubj = GetTempValName(SVC, tempNames);

                    processObj = getSYSContainetP(SVC, req.PartitionName.Trim('*'));
                    SVC[nameOfSubj].Wrap(processObj);
                    modelSpec.Vset("a", nameOfSubj);
                }

                #endregion

                #region subject change

                if (b.Contains(">") && !b.Contains("<"))
                {
                    nameOfSubj = GetTempValName(SVC, tempNames);// last added here
                    if (mod.body == null || !mod.body.Contains("@"))
                        SVC[nameOfSubj].Wrap(SVC[exec.SUBJ].W());
                    modelSpec.Vset("v", nameOfSubj);
                }

                if (req.isHere("rp"))
                {
                    nameOfSubj = GetTempValName(SVC, tempNames);
                    SVC[nameOfSubj].Wrap(modelSpec["rp"].W());
                    modelSpec.Vset("v", nameOfSubj);
                }

                if (req.isHere("lp"))
                {
                    nameOfSubj = GetTempValName(SVC, tempNames);
                    SVC[nameOfSubj].Wrap(modelSpec["lp"].W());
                    modelSpec.Vset("a", nameOfSubj);
                }

                #endregion

                #region context switching []*

                bool contextByName = false;
                if (b.Contains("*") && req.PartitionKind !="func")
                {
                  string pn = b.Trim('>', '<',' ', '*');
                    if (pn.Length > 0)
                    {                      
                        nameOfSubj = GetTempValName(SVC, tempNames);

                        b = b.Replace("*", "");
                        SVC[nameOfSubj].Wrap(getSYSContainetP(SVC, pn, modelIsProducer));
                        modelSpec.Vset("v", nameOfSubj);
                    }
                    else
                    {
                        nameOfSubj = GetTempValName(SVC, tempNames) ;

                        modelSpec.Vset("v", nameOfSubj);
                        contextByName = true;
                    }
                }

                #endregion

                

                ExecActionModel(rez, processObj); // M A I N

              
                if (b.Contains(">"))  // pipeline operator
                {                    
                    if (modelIsProducer)    // modelSpec["v"]
                        SVC[exec.SUBJ].Wrap(SVC[nameOfSubj].W());
                    else
                        SVC[exec.SUBJ].Wrap(processObj);
                }

                //if (b.Contains("<"))// filler modifier  this break existing codebase, do not uncomment
                //{
                //    if (modelIsProducer)
                //        processObj.Wrap(SVC[nameOfSubjForFiller].W());
                //}


                if (b.Contains("*") && req.PartitionKind != "func")
                {
                    SVC["SYS_use_container_do_not_owerlap"].ArrResize(0);

                    if (contextByName)
                        SVC["SYS_use_container_do_not_owerlap"].Wrap(SVC[nameOfSubj].W());
                    else
                        SVC["SYS_use_container_do_not_owerlap"].Wrap(processObj);
                }

           
                foreach(string tn in tempNames)
                tempSDCstack.Push(tn);

                SVC["modelSpec"] = ms;
            }

            return rezb;
        }

        string GetTempValName(opis SVC, List<string> tempNames)
        {
            string rez = "";

            if (tempSDCstack.Count > 0)
            {
                rez = tempSDCstack.Pop();
                SVC[rez] = new opis();
            }
            else
                rez = DateTime.Now.Ticks.ToString() + rnd.Next().ToString() + rnd.Next().ToString();
           
            tempNames.Add(rez);

            return rez;
        }

        opis getSYSContainetP(opis SVC, string pn)
        {
           return getSYSContainetP(SVC, pn, false);            
        }

        public opis GetLocalDataContextVal(string pn, bool create = false)
        {
            opis SVC = thisins["sharedVariablesContext"];
            return getSYSContainetP(SVC, pn, create, false);
        }

        opis getSYSContainetP(opis SVC, string pn, bool create, bool logerror = true)
        {
            opis rez = new opis();
            opis t = SVC["SYS_use_container_do_not_owerlap"].W();

            string pnl = pn.Contains("~")? pn.Replace("~", "") : pn;

            if (t.isHere(pnl))
                rez = pn.Contains("~") ? t[pnl] : t[pn].W();
            else
            {
                if (string.IsNullOrWhiteSpace(pn))
                    rez = t;
                else
                {
                    if (create)
                    {
                        //rez = t[pn];
                        rez = pn.Contains("~") ? t[pnl] : t[pn].W();
                    }
                    else
                    if(logerror)
                    {
                        opis err = new opis();
                        err.PartitionName = "ERR no such patrition: " + pn;
                        err.AddArr(t.Duplicate());
                        err.AddArr(SVC["modelSpec"].Duplicate());
                        global_log.log.AddArr(err);
                    }
                }
            }

            return rez;
        }

        opis GetLocalDataModel(opis req)
        {
           
            opis rez = req;

            opis SVC = thisins["sharedVariablesContext"];
            bool inSvc = (SVC.isHere(req.PartitionKind)
               && SVC[req.PartitionKind].PartitionKind == "Action");

            Dictionary<string, int> cash = (Dictionary<string, int>)thisins["packages"].bodyObject;
            int poz = -1;
           
            if (inSvc ||
                (cash.TryGetValue(req.PartitionKind, out poz)))
            {

                opis mod = inSvc ? SVC[req.PartitionKind] : thisins["packages"][poz];
                rez = new opis();
                rez.PartitionKind = "exec";

                rez[exec.message_as_parameter_for_instructions].body = "y";
                rez[exec.instructions].CopyArr(mod);

                if (thisins["sharedVariablesContext"][req.PartitionKind].body != "!")
                {
                    //копируем на случай если модель должна выступать в роли филлера
                    //ведь тогда мы изменим processParameter который будет являться и modelSpec
                    SVC["modelSpec"].CopyArr(req.Duplicate());
                    SVC["modelSpec"].Vset("v", req.body);
                    SVC["modelSpec"].Vset("a", req.PartitionName);
                }

            }

        
            return rez;
        }

        opis GetPartialRec_LocalDataModel(opis req, opis modelspec)
        {
            opis rez = null;

           
            if (req!= null && req.PartitionKind != null 
                && thisins["sharedVariablesContext"].isHere(req.PartitionKind)
                && thisins["sharedVariablesContext"][req.PartitionKind].PartitionKind == "Action")
            {
                opis mod = thisins["sharedVariablesContext"][req.PartitionKind];

                if (mod.body == "%")
                {
                    modelspec.AddArrRange(mod[0]);// первый елемент будет моделью которую частично специфицируют

                    rez = GetPartialRec_LocalDataModel(mod[0], modelspec);
                }
                else
                {
                    rez = mod;
                }     
            }

            return rez;
        }


        #endregion Executing models 

        void BuildActionPath(opis rez)
        {
            opis reflisttmp = new opis();
            rez.FindTreePartitions("Action", rez.PartitionName, reflisttmp);
            for (int i = 0; i < reflisttmp.listCou; i++)
            {
                opis tmp = reflisttmp[i];
                for (int j = 0; j < tmp.listCou; j++)
                {
                    if (tmp[j].PartitionKind == "Action")
                    {
                        for (int k = 0; k < tmp[j].listCou; k++)
                        {
                            tmp[j][k].Vset("_path_", tmp.PartitionName);
                        }
                    }else
                    tmp[j].Vset("_path_", tmp.PartitionName);
                }
            }

            BuildActionPath(rez, "MsgTemplate");
            BuildActionPath(rez, "global_log");
            BuildActionPath(rez, "local_log");
            BuildActionPath(rez, "func", true);
            BuildActionPath(rez, "code_point");
            BuildActionPath(rez, "fill");
            BuildActionPath(rez, "fc");

        }

        void BuildActionPath(opis rez, string modelName, bool eachSubitem = false)
        {
            BuildActionPathFunc(rez,
                (x, p) => {
                    if (!x.isHere("_path_"))
                        x.Vset("_path_", p + "->" + x.PartitionName);
                        },
                x=> x.PartitionKind = modelName,
                
                eachSubitem);           
        }

        void BuildActionPathByName(opis rez, string Name, bool eachSubitem = false)
        {
            BuildActionPathFunc(rez,
                (x, p) => {
                    if (!x.isHere("_path_"))
                        x.Vset("_path_", p + "->" + x.PartitionName);
                },
                x => x.PartitionName = Name,
               
                eachSubitem);
        }

        void BuildActionPathFunc(opis rez, 
            Action<opis, string> setter,
            Action<opis> templMaker,
            bool eachSubitem)
        {
            opis reflisttmp = new opis();

            opis template = new opis();
            templMaker(template);

            rez.FindTreePartitions(template, rez.PartitionName, reflisttmp);
        
            for (int i = 0; i < reflisttmp.listCou; i++)
            {
                opis tmp = reflisttmp[i];
                for (int j = 0; j < tmp.listCou; j++)
                {                   
                    if (eachSubitem)
                        for (int k = 0; k < tmp[j].listCou; k++)
                            setter(tmp[j][k], tmp.PartitionName);

                    setter(tmp[j], tmp.PartitionName);
                }
            }
        }


        #endregion  internl commands that accepted by base class

        // answers base
        public bool ReceiveSubscriptionsBase(opis evento, opis sender)
        {

            bool rez = false;
            #region A N S W E R

            if (evento.V("імя") == Msg.answer
               && evento.V("тип") == Msg.answer)
            {
                rez = true;
                opis answer = sender[Msg.answer];
                #region hint
                // sender is the original message where you can find type of request and others
                // this is answer for previously sended message, it will contain detailed info about
                // instance which decided to answer 
                /* answer["context"] specific context of instance
                   answer["notion"].body  name of instance
                   answer.WrapByName(waiter, "contact")  contain wrapper for instanse direct contact object 
                 */
                #endregion 

                if (CanHandle(sender["context"]))
                {
                    AddInstLog("gotAnswer", "", answer);
                    ExecActionResponceModelsList(sender[MsgTemplate.responce], answer);
                    //AddInstLog("After gotAnswer", "", answer);
                }
                else
                {
                    AddInstLog("gotAnswer but not handled", "", answer);
                }
               
            }

            #endregion
            return rez;

        }

      /// <summary>
      /// switch instance context to each context in array, and perfom [dl] in each
      /// </summary>
      /// <param name="dl">procedure to run in each context</param>
      /// <param name="param"></param>
        public void ToAll(ProcessContextDelegate dl, opis param)
        {
            for (int i = 0; i < contexts.listCou; i++)
            {
                Handle(contexts[i]);
                dl(param);
            }      
        }

        public opis GetInstanceLog()
        {
            return thisins["Models_log"];
        }

        // ==========================================================================

        #region messaging {

        public void ResendToUpper()
        {
            if (curr_msgProc != null)
            {
                ResendToUpper(curr_msgProc, true);
            }          
        }

        public void ResendToUpper(opis message)
        {
            ResendToUpper(message, false);
        }

        public void ResendToUpper(opis message, bool doNotValidate)
        {
            // cpecial flag to prevent resending message to higher context
            if (!string.IsNullOrEmpty(message.V(Msg.no_transfer)))
                return;

            //AddInstLog("rsnd", "", message);

            opis upper = CTX.GoUp();
            opis resendmsg = new opis();
            resendmsg.CopyParams(message);

            if (!message[Msg.initiator].isInitlze)
            {
                resendmsg.WrapByName(message, Msg.initiator);
            }

            if(!doNotValidate)
            ExecActionResponceModelsList(message[MsgTemplate.validate], message);

            if (message.V(MsgTemplate.cancel).Length ==0)
            {
                SendRequest(contex.org(upper), message["msg"].body, upper, resendmsg);
            }
        }

        public void SendRequestFromAllContexts(string receiver, string request)
        {
            for (int i = 0; i < contexts.listCou; i++)
            {
                SendRequest(receiver, request, contexts[i]);
            }       
        }

        #region  SendRequest 

        public void SendRequest(string receiver, string request, opis context)
        {
            opis message = new opis();
            message.body = request;
            message["context"] = contex.GetHierarchyStub( context);

            SendMessage(receiver, message);
        }

        public void SendRequest(string receiver, string request, opis context, opis message)
        {         
            message.body = request;
            message["context"] = contex.GetHierarchyStub(context); 

            SendMessage(receiver, message);
        }

        public void SendRequest(string receiver, string request, opis context, params string[] values)
        {
            opis message = new opis("", values);
            SendRequest(receiver, request, context, message);
        }

        #endregion  SendRequest

        public void ThisRequest(string receiver, string request, params string[] values)
        {
            opis message =  new opis("", values);
            ThisRequest(receiver, request, message);
        }

        public void ThisRequest(string receiver, string request, opis msg)
        {
            opis message = msg;
            message.body = request;
            if (!message["context"].isInitlze)
            message["context"] = contex.GetHierarchyStub(o);

            SendMessage(receiver, message);
        }

        public void ThisRequest(opis msg)
        {
            ThisRequest(msg.V(MsgTemplate.msg_receiver), msg.V(MsgTemplate.msg), msg);
        }

        public bool AnswerToMessage(opis message, params string[] values)
        {
            opis answer = new opis("", values);
          return  AnswerToMessage(message, answer);
        }

        public bool AnswerToMessage(opis message, opis answer)
        {
            bool rez = true;

            opis ppp = new opis();
            ppp.AddArr(message);
            ppp.AddArr(answer);
            ppp.PartitionName = "inf";

            //AddInstLog("ans", "", ppp);

            answer.PartitionKind = ModelAnswer.answer;
            answer[ModelAnswer.parameters].CopyParams(message);
            //answer["original_msg_context"] = message[ModelAnswer.context];

            answer["original_msg_context"] = contex.GetHierarchyStub(message[ModelAnswer.context]);          
            answer[ModelAnswer.context] = contex.GetHierarchyStub(curr);// curr;
            answer[ModelAnswer.notion].body = Name();            
            answer.WrapByName(waiter, ModelAnswer.contact);

            // retrieve all additional parameters from this instance          
            ExecActionResponceModelsList(message[MsgTemplate.validate], answer);

            message.raiseEvents = true;

            if (answer.V(ModelAnswer.cancel).Length > 0)
            {
                return false;
            }  else
                AddInstLog("ans", message.V("msg"), ppp);

            // if this is cascade resended message and original sender is not the instance who create ...
            if (message[Msg.initiator].isInitlze)
            {
                message.W(Msg.initiator)[Msg.answer] = answer;
            }
            else
            {
                // this assigment will raise event directly to instance which send message        
                message[Msg.answer] = answer;
            }

            return rez;
        }

        /// <summary>
        /// utilize global communication in shared context to broadcast message 
        /// </summary>
        /// <param name="receiver">type of sys object</param>
        /// <param name="message">all parameters and specs of message or request</param>
        public void SendMessage(string receiver,  opis message)
        {           
            CanHandle(message["context"]);
          
            opis code;

            if (spec["aspects"]["All"].isHere("before_Send"))
            {
                code = spec["aspects"]["All"]["before_Send"];
                code = code.Duplicate();
                ExecActionResponceModelsList(code, message);
            }

            code = spec["aspects"][message.body]["before_Send"];

            code = code.Duplicate();
            code.PartitionName = spec.PartitionName+ "->aspects->"+message.body+"->before_Send";
            BuildActionPathByName(code, "_path_");

            ExecActionResponceModelsList(code, message);

            if(!message.isHere("cancel"))
            SendMessageNoAspect(receiver, message);
        }

        public void SendMessageNoAspect(string receiver, opis message)
        {
            message["rec"].body = thisins.W("spec").PartitionName;

            AddInstLog("msg", "", message);

            message.PartitionKind = "message";

            // if one message sending in cycle, there is no need to add each time one more listener
            message.PartitionName = receiver;

            message.WaitAction(waiter, true);
            communicator.AddArr(message);
        }

        #endregion  messaging }

    }

}
