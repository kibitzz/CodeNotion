﻿// Copyright (C) 2015-2022 Igor Proskochilo

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
using basicClasses.Optimizations;

namespace basicClasses
{
#if NETFRAMEWORK
        
       
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
                OntologyTreeBuilder.isMetaTerm(clicked.PartitionKind)))
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

#endif

    public delegate void ProcessContextDelegate(opis param);
    public delegate void ProcessContextDelegateParam(opis context, opis param);

    public delegate void guiGelegate();



    public  class SysInstance : IOpisFunctionalInstance
    {
#if NETFRAMEWORK
        public static List<MsgLogItem> Log;
        public static Form1 callbackFrm;
        public static guiGelegate updateform;
#endif

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


        public static string messageBannertext;

        /// <summary>
        /// sharedVariablesContext
        /// </summary>
        public const int svcIdx = 2;
        /// <summary>
        /// packages
        /// </summary>
        public const int pkgIdx = 3;
        public const int envIdx = 4;

        /// <summary>
        /// SUBJ
        /// </summary>
        public const int subjIdx = 0;
        /// <summary>
        /// modelSpec
        /// </summary>
        public const int modelSpecIdx = 1;
        /// <summary>
        ///  SYS_use_container_do_not_owerlap
        /// </summary>
        public const int ldcIdx = 2;

        protected opis thisins;
        protected opis spec;
        protected opis o;
        protected opis communicator;
        protected opis contexts;
        protected contex CTX;
        protected opis waiter;
        protected opis curr_msgProc;
        protected bool cacheAnwers;
        
        /// <summary>
        /// local data context for optimization
        /// </summary>
        public opis LDC;

        /// <summary>
        /// lookup optimization
        /// </summary>
        public opis procesParam;


        public static bool debugNext;
        public static bool showOverrideWarnings;

        public static object oddLocker = new object();
        public static object evenLocker = new object();
        public static object modelLocker = new object();

        Stack<string> tempSDCstack;   
        Random rnd;
        /// <summary>
        ///  { '0', '@', '!', '#', '*', '$', '|' }
        /// </summary>
        private static readonly char[] flagChars = new char[] { '0', '@', '!', '#', '*', '$', '|' };

#if NETFRAMEWORK
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

        public void updateGui()
        {
            if (updateform != null)
            {
                callbackFrm.Invoke(updateform);
            }
        }

#endif

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
            opis rez = thisins[envIdx];
            if (rez.listCou == 0)
            {
                rez = ContEnvironment(); //TODO: call in initInstanceSpec environment produce not valid setting
                thisins[envIdx] = rez;
            }
           
            return rez;
        }

        opis ContEnvironment()
        {
            opis rez = new opis("EnvirinmentForModel");
            rez.WrapByName(thisins, "thisins");
            rez.WrapByName(spec, "spec");
            //rez.WrapByName(communicator, "communicator");
            rez.WrapByName(o, "currCont");
            rez.WrapByName(waiter, "waiter");
            rez.WrapByName(contexts, "contexts");

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

        #region IOpisFunctionalInstance 


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

        #endregion  IOpisFunctionalInstance 

        // ==========================================================================

     
        // ==========================================================================

        #region INITIALIZATION

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

        opis SetupConstantIndexes(opis thisinsP)
        {
            opis tmp = new opis();
            tmp.CopyArr(thisinsP);

            thisinsP.CopyArr(new opis());
            thisinsP.ArrResize(30);

            opis ee = new opis();

            if (tmp["sharedVariablesContext"].paramCou > 0)
            {
               throw  new IndexOutOfRangeException();
            }
            tmp["sharedVariablesContext"].AddArr( ee["SUBJ"]);
            tmp["sharedVariablesContext"].AddArr ( ee["modelSpec"]);
            tmp["sharedVariablesContext"].AddArr (ee["SYS_use_container_do_not_owerlap"]);
            //tmp["sharedVariablesContext"].paramCou = 3;



            thisinsP[0] = tmp["spec"];
            thisinsP[1] = tmp["waiter"];
            thisinsP[svcIdx] = tmp["sharedVariablesContext"];
            thisinsP[pkgIdx] = tmp["packages"];
            //thisinsP[pkgIdx].bodyObject = tmp["packages"].bodyObject;
            thisinsP[envIdx] = tmp["environment"];
            thisinsP.paramCou = 5;

            thisinsP.AddArrMissing(tmp);

            return thisinsP;
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

           
            //thisins["sharedVariablesContext"] = new opis(); //TODO: not working with basic implementation of opis
            thisins["sharedVariablesContext"] = new opisDictOptimized();
            thisins["sharedVariablesContext"].ArrResize(4000);

            thisins["packages"] = new opisDictOptimized();
            thisins["packages"].ArrResize(4000);
            //thisins["packages"].bodyObject = new Dictionary<string, int>(4000);
           

            SetupConstantIndexes(thisins);
          //  thisins["environment"] = ContEnvironment();
            spec = thisins.W(ModelG.spec);

            ExecActionResponceModelsList(waiter[ModelNotion.Build], rez);
            ExecActionResponceModelsList(rez[ModelNotion.Build], rez);
        }

        public opis GetWordForm(string f)
        {
            opis rez = Words.Find(f);

            return rez;
        }


        #endregion initialization

        // ==========================================================================

        #region MESSAGE receive check run

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
            var SVC = thisins[svcIdx][ldcIdx];
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
            var SVC = thisins[svcIdx][ldcIdx];
   
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


#if NETFRAMEWORK
                            if (!msg.isHere("hide"))
                            AddInstLog("in", msg.V("msg"), msg);
#endif

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

#if NETFRAMEWORK
                    if (!msg.isHere("hide"))
                        AddInstLog("in", msg.V("msg"), msg);
#endif

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

        
        protected void Start(opis msg)
        {
            for (int i = 0; i < contexts.listCou; i++)
            {
                Handle(contexts[i]);                

                ProcessStartPartition(msg);
            }          
        }



        #endregion MESSAGE receive check run

        // ==========================================================================


        #region ACTOR methods run to proc message | ASPECTS | HOOKS

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
       
            if (spec.PartitionName != waiter.PartitionName)            
                RunNotionReaction(waiter, ModelNotion.Responces, organizer, msg);

            RunNotionReaction(spec, ModelNotion.Responces, organizer, msg);

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


        #endregion ACTOR methods run

        // ==========================================================================      

        #region Message template exec

        public void ExecMessageModelsAndSend(opis requests)
        {
            for (int i = 0; i < requests.listCou; i++)
            {
                opisEventsSubscription req = requests[i].DuplicateAs<opisEventsSubscription>();
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
            opis ms = thisins[svcIdx][modelSpecIdx];//.Duplicate();

            opis elemToChange = req;
            req = GetLocalDataModel(req); // local models

            object obj = MF.GetModelObject(req.PartitionKind);
            if (obj is IActionProcessor)
            {
                IActionProcessor processor = (IActionProcessor)obj;
                processor.InitAct(this, req);
                
                processor.Process(elemToChange);

                if (req.PartitionName != MsgTemplate.context)
                    processor.log.AddArr(req);
                processor.log.body = req.PartitionName;
            }

            thisins[svcIdx][modelSpecIdx] = ms;
        }

        #endregion

        // ==========================================================================

        #region EXEC ACTION | MAIN language implementation


        /// <summary>
        ///  switch exec.SUBJ
        /// </summary>
        /// <param name="req">list of instructions</param>        
        public void ExecActionModelsList(opis req, bool parallel = false)
        {            
           opis marg = thisins[svcIdx][exec.SUBJ];
          
                opis wr = new opis(1);
                wr.Wrap(req);
                thisins[svcIdx][exec.SUBJ] = wr;


            if (!parallel)
            {
                opis activeCont = o;
                for (int i = 0; i < req.listCou; i++)
                {
                    ExecActionModel(req[i], req[i]);

                    if (activeCont != o)
                        Handle(activeCont);

                    if (req[i].PartitionKind == "Breaker" && req[i].isHere(Breaker.flag))  //TODO: optimize comparison and flag by index
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

            thisins[svcIdx][exec.SUBJ] = marg;
        }

        /// <summary>
        /// switch exec.SUBJ
        /// </summary>
        /// <param name="req">list of instructions</param>
        /// <param name="message">process as subject of instructions</param>
        public void ExecActionResponceModelsList(opis req, opis message)
        {
            opis marg = thisins[svcIdx][exec.SUBJ];
           
                opis wr = new opis(1);
                wr.Wrap(message);
                thisins[svcIdx][exec.SUBJ] = wr;

            //на випадок коли інстанс може отримувати повідомлення
            // чи відповіді в процесі виконання попереднього циклу
            // отримавши повідомлення змінюється поточний контекст інстанса
            // і через це наступний цикл не має доступу до SDC - виникає збій
            opis activeCont = o;
            for (int i = 0; i < req.listCou; i++)
            {               
                ExecActionModel(req[i], message);

                if (activeCont != o)
                    Handle(activeCont);

                if (!string.IsNullOrEmpty(req[i].PartitionKind) && req[i].PartitionKind[0] == 'B' && req[i].PartitionKind == "Breaker" && req[i].isHere(Breaker.flag))
                    break;
            }                 

            thisins[svcIdx][exec.SUBJ] = marg;
        }     


        // -----------------------------------------------

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
                        processor.InitAct(this, req);                       

                        processor.Process(processParameter);
                    }
                }
            }
        }

        //  -----------------------------------------------

        bool IsLocalDataModel(opis functionCall, opis processParameter)
        {
            bool rezb = false;         
            opis SVC = thisins[svcIdx];
            int inSvc =-1;// local func override          
            int poz = -1; // package func

            inSvc = SVC.getPartitionIdx(functionCall.PartitionKind);
            inSvc = inSvc >=0 && SVC[inSvc].PartitionKind == "Action" ? inSvc : -1;
           
#if NETFRAMEWORK

            if (showOverrideWarnings && inSvc != -1 &&
              thisins[pkgIdx].isHere(functionCall.PartitionKind))
                {
                    opis err = new opis();
                    err.PartitionName = "WARN: local model override package func: " + functionCall.PartitionKind;
                    err.AddArr(functionCall.Duplicate());
                    global_log.log.AddArr(err);
                }            
#endif 

            opis packages = thisins[pkgIdx];

            if (inSvc >= 0 || // local models should override package funcs
               (poz = packages.getPartitionIdx(functionCall.PartitionKind)) >=0)  // if inSvc -- poz never evaluated, and stay -1
            {
                rezb = true;
                List<string> tempNames = new List<string>();// remove this list use -- very costly             

                opis callerFuncParams = SVC[modelSpecIdx]; // caller function spec -- _sys_subscript can be specified there
                
                opis mod = inSvc >= 0 ? SVC[inSvc] : packages[poz]; // function body (code that process modelspec and params)             
             
                
                int flags = 0; // 2-producer 4-parentspec(!) 8-noRunSpec(#) 16-addcontext(*) 32-duplic($) 64-inline(|)

                if (mod.bodyObject != null && mod.bodyObject is ModelSpecIdxPresence m)
                {
                    flags = m.flags;
                }
                else
                {
                    var modbody = mod.body;

                    if (!string.IsNullOrEmpty(modbody))
                    {
                        int len = modbody.Length;
                        byte i = 1;
                        while (len > 0 && i < 7)
                        {
                            // i = Array.IndexOf(flagChars, modbody[i - 1]); //which is optimal?
                            if (modbody.Contains(flagChars[i]))
                            {
                                len--;
                                flags = flags | (1 << i);
                            }
                            i++;
                        }
                    }
                }

                bool modelIsProducer = (flags & 2) == 2;

                // // 2-lp 4-rp 8-a 16-v       32-duplic($) 64-inline(|) 128- 256-
                int a_v_lp_rp_fil_iv_vcon_acon = 0;
                int lppos = -1;
                int rppos = -1;

                if (functionCall.bodyObject != null && functionCall.bodyObject is ModelSpecIdxPresence reqm)
                {
                    a_v_lp_rp_fil_iv_vcon_acon = reqm.a_v_lp_rp_fil_iv_vcon_acon;
                }
                else
                {
                    // if (req.isHere("lp", false))    // we can cache founded index to use next time                
                    if ((lppos = functionCall.getPartitionIdx("lp", false)) != -1)
                        a_v_lp_rp_fil_iv_vcon_acon = a_v_lp_rp_fil_iv_vcon_acon | 2;

                    //   if (req.isHere("rp", false))     
                    if ((rppos = functionCall.getPartitionIdx("rp", false)) != -1)
                        a_v_lp_rp_fil_iv_vcon_acon = a_v_lp_rp_fil_iv_vcon_acon | 4;
                                       
                }


                opis modelSpec = new opis(-1);
                opis speca = null;
                opis specv = null;
               
                void setA(string val)
                {
                    if(speca == null)
                        speca = modelSpec["a"];
                    speca.body = val;
                }
                void setV(string val)
                {
                    if (specv == null)
                        specv = modelSpec["v"];
                    specv.body = val;
                }
               
                #region create modelSpec

                if ((flags & 4) == 0)//   (!modbody.Contains("!"))
                {
                   
                    // if modelspec not exec (modbody.Contains("#")), so no changes are made to its items 
                    //(example: func # its modelSpec is just data-code to be exec later, and when actually exec it will be copied)
                    modelSpec.CopyArr((flags & 8) == 8 ? functionCall : functionCall.Duplicate());

                        //modelSpec.bodyObject = req.bodyObject; // to check if explicit ^ subscription is present in parent modelspec                   

                    if ((a_v_lp_rp_fil_iv_vcon_acon & 1) == 0)
                    {
                        if (!functionCall.isHere("v", false))//TODO: optimize exec                                                   
                            setV(functionCall.body);
                         
                        if (!functionCall.isHere("a", false))
                            setA(functionCall.PartitionName);                                                 
                    }


                    if ((flags & 8) == 0)// not all items should run param functions      (!modbody.Contains("#"))                                          
                        ExecActionModelsList(modelSpec);                   

                    if ((flags & 16) == 16)// add context items as missed parameters(ms)  (modbody.Contains("*"))
                        modelSpec.AddArrMissing(getSYSContainetP(SVC, ""));

                    if ((flags & 32) == 32)// duplicate all input data       (modbody.Contains("$"))
                        modelSpec = modelSpec.Duplicate();

                    opis mspos;
                    if ((mspos = functionCall.getPartitionNotInitOrigName("ms")) != null)                       
                    {
                        if (string.IsNullOrEmpty(mspos.PartitionKind))
                        {
                            modelSpec.AddArrMissing(callerFuncParams);
                            setV(callerFuncParams.V("v"));
                            setA(callerFuncParams.V("a"));
                        }
                        else if (mspos.PartitionKind.Length > 0 && mspos.PartitionKind[0] == '~')
                        {
                            modelSpec.AddArrMissing(callerFuncParams);
                        }

                        if (mspos.body.Length > 0 && mspos.body[0] == '*')
                            modelSpec.AddArrMissing(getSYSContainetP(SVC, mspos.body.Length > 1 ? mspos.body.Remove(0, 1) : ""));
                    }

                    SVC[modelSpecIdx] = modelSpec;
                }
                else
                {
                  //  modelSpec = new opis(-1);                  

                    if ((flags & 64) == 64)
                    {
                        callerFuncParams["vvv"].body = functionCall.body;
                        callerFuncParams["aaa"].body = functionCall.PartitionName;

                        if (functionCall.isHere("lp", false))
                        {
                            var pex = functionCall["lp"].Duplicate();
                            ExecActionModel(pex, pex);

                            var elpName = GetTempValName(SVC, tempNames);
                            SVC[elpName].Wrap(pex.W());
                            callerFuncParams.Vset("aaa", elpName);
                        }                                 
                    }
                }
           
                #endregion

                            
                string b = functionCall.body ?? "";
                opis processObj = processParameter;
                string nameOfSubj = "";
            
                if (b == "<")
                {
                    functionCall.body = "";
                    processObj = functionCall;//filler
                   
                    if (modelIsProducer)
                    {
                        nameOfSubj = GetTempValName(SVC, tempNames);                        
                        SVC[nameOfSubj].Wrap(functionCall);
                        setV(nameOfSubj);    
                    }                             
                }

                                       
                #region hook input object

                if (functionCall.PartitionName.Length > 0 && functionCall.PartitionName[functionCall.PartitionName.Length -1] == '>')
                {
                    processObj = SVC[exec.SUBJ].W();

                    string pn = functionCall.PartitionName;

                    #region  input in form of <filler model>
                    // when input in form of <filler model> this exec filler and put result in tmp arg
                    if(pn.Length > 0 && pn[0] == '<')
                    {
                        opis paramRez = new opis();
                        paramRez.PartitionName = "stub inp";

                        pn = pn.Trim('>', '<');
                        if (pn.Length > 0)
                        {                          
                            SVC[modelSpecIdx] = callerFuncParams;
                          
                            paramRez.PartitionKind = pn;
                            ExecActionModel(paramRez, paramRez);
                            processObj = paramRez.W();
                         
                            SVC[modelSpecIdx] = modelSpec;
                        }
                    }

                    #endregion

                    nameOfSubj = GetTempValName(SVC, tempNames) ;

                    SVC[nameOfSubj].Wrap(processObj);
                    setA(nameOfSubj); 

                }
            
                if (functionCall.PartitionName.Length > 0 && functionCall.PartitionName[0] == '*')
                {
                    nameOfSubj = GetTempValName(SVC, tempNames);

                    //  processObj = getSYSContainetP(SVC, req.PartitionName.TrimStart('*')); // is it useful to use implicitly ldc values as processObj ? is it used at prod?
                    // for this reason function "return" can not fill proper object when its parameter is ldc value *data[return]
                    //  SVC[nameOfSubj].Wrap(processObj);
                    SVC[nameOfSubj].Wrap(getSYSContainetP(SVC, functionCall.PartitionName.TrimStart('*')));
                    setA(nameOfSubj); 
                }

                #endregion

                bool pipeline = b.Length > 0 && b[0] == '>';

                #region overrides lp rp
              
                if (pipeline)
                {
                    nameOfSubj = GetTempValName(SVC, tempNames);
                    // if we want not to just push new data in pipeline but change the object currently in pipeline
                    if (!modelIsProducer) // why!?!  because if function create new data it rewrite previous subject - this can lead to implicit side effect modification
                        SVC[nameOfSubj].Wrap(SVC[exec.SUBJ].W()); // if function is modifying rp or as readonly datasource
                    else if (b == ">>") // explicit subject modification
                                        // (structure that is in pipeline now is refilled with product @)
                    {
                        SVC[nameOfSubj].Wrap(SVC[exec.SUBJ].W());
                    }

                    setV(nameOfSubj);
                }

               
                if ((a_v_lp_rp_fil_iv_vcon_acon & 4) == 4 && (flags & 4) == 0)
                {
                    nameOfSubj = GetTempValName(SVC, tempNames);
                    if (rppos != -1)
                        SVC[nameOfSubj].Wrap(modelSpec[rppos].W());
                    else
                        SVC[nameOfSubj].Wrap(modelSpec["rp"].W());

                    setV(nameOfSubj);
                }

                if ((a_v_lp_rp_fil_iv_vcon_acon & 2) == 2 && (flags & 4) == 0)
                {
                    nameOfSubj = GetTempValName(SVC, tempNames);
                    if (lppos != -1)
                        SVC[nameOfSubj].Wrap(modelSpec[lppos].W());
                    else
                        SVC[nameOfSubj].Wrap(modelSpec["lp"].W());

                    setA(nameOfSubj); 
                }

                #endregion


                #region context variables []*name
                bool subscribeProduce = false;
                int subidxMs = -1;
                int subidxLdc = -1;

                // working sequence *~itm not ~*itm
                if (b.Length > 0 && b[0]=='*' && functionCall.PartitionKind !="func")                           
                {
                    //  symbols '>', '<' not accepted in combination with *
                    string pn = b.Length > 1 ? b.Remove(0, 1) : ""; 
                    if (pn.Length > 0)
                    {
                        nameOfSubj = GetTempValName(SVC, tempNames);

                        SVC[nameOfSubj].Wrap(getSYSContainetP(SVC, pn, modelIsProducer));
                        setV(nameOfSubj);
                        

                        pn = pn[0] == '~' ? pn.Remove(0, 1) : pn;  //after getSYSContainetP because pn should contain '~' to get without unwrapping from LDC                   
                        if (modelIsProducer)
                        {
                            subidxMs = callerFuncParams.getPartitionIdxCharPref("^" + pn, '^');
                            subidxLdc = subidxMs == -1 ? SVC[ldcIdx].W().getPartitionIdxCharPref("^" + pn, '^') : -1;
                            subscribeProduce = subidxMs != -1 || subidxLdc != -1;
                        }                                           
                    }
                    else
                    {
                        nameOfSubj = GetTempValName(SVC, tempNames);
                        setV(nameOfSubj);
                    }
                }

                #endregion
                
              
                ExecLocalModelCode(SVC, mod, processObj); // M A I N


                if (pipeline)
                {
                    SVC[exec.SUBJ].Wrap(SVC[specv.body].W());
                }

                //context switch []*
                if (b == "*" && functionCall.PartitionKind != "func")
                {
                    SVC[ldcIdx].ArrResize(0);
                    SVC[ldcIdx].Wrap(SVC[nameOfSubj].W());
                }

                SVC[modelSpecIdx] = callerFuncParams; // before subscribeProduce to be able access
                                        // the model spec of caller func from subscription
                                        // (not params of irrelevant producer func) 

                if (subscribeProduce) 
                {                   
                    string pn = "^" + b.Trim('>', '<', ' ', '*', '~');                  

                    if (subidxMs != -1)  // priority on explicit method extention in model spec
                    {
                        var subscription = callerFuncParams[subidxMs].Duplicate();
                        ExecActionModel(GenExecInstr(subscription), SVC[nameOfSubj].W());
                    }
                    else
                    if (subidxLdc != -1) 
                    {
                        var subscription = SVC[ldcIdx].W()[subidxLdc].Duplicate();
                        ExecActionModel(GenExecInstr(subscription), SVC[nameOfSubj].W());
                    }         
                }

                foreach (string tn in tempNames)
                {
                    //SVC[tn] = new opis(1);
                    tempSDCstack.Push(tn);                    
                }
                
            }

            return rezb;
        }

        public void ExecLocalModelCode(opis svc, opis instructions, opis processObj)
        {           
            opis datacontext = svc[ldcIdx].W();
          
            ExecActionResponceModelsList(instructions.DuplicateInstrOpt(0), processObj); 

            svc[ldcIdx].Wrap(datacontext);
        }

        // -----------------------------------------------

        opis GenExecInstr(opis code)
        {
            var rez = new opis(3);
            rez.PartitionKind = "exec";

            rez[exec.message_as_parameter_for_instructions].body = "y";
            rez[exec.instructions].CopyArr(code);

            return rez;
        }

        string GetTempValName(opis SVC, List<string> tempNames)
        {
            string rez = "tmp_val_do_not_override";

            if (tempSDCstack.Count > 0)
            {
                rez = tempSDCstack.Pop();

                SVC[rez] = new opis(1); //TODO: optimize access by caching integer index together with string index key
            }
            else
                rez = DateTime.Now.Ticks.ToString() + rnd.Next().ToString() + rnd.Next().ToString();

            tempNames.Add(rez);

            return rez;
        }

        #region LDC variables

        opis getSYSContainetP(opis SVC, string pn)
        {
           return getSYSContainetP(SVC, pn, false);            
        }

        public opis GetLocalDataContextVal(string pn, bool create = false)
        {
            opis SVC = thisins[svcIdx];
            return getSYSContainetP(SVC, pn, create, false);
        }

        opis getSYSContainetP(opis SVC, string pn, bool create, bool logerror = true)
        {
            opis rez = null;
            opis t = SVC[ldcIdx].W();

            if (string.IsNullOrEmpty(pn))
                return t;

            bool isref = pn.Length > 0 && pn[0] == '~';

            string pnl = isref ? pn.Remove(0, 1) : pn;
            int pos = t.getPartitionIdx(pnl);

            if (pos != -1)
                rez = isref ? t[pos] : t[pos].W();
            else
            {                   
                if (create)
                {
                    rez = isref ? t[pnl] : t[pn].W();
                }
                else
                {
                    rez = new opis(1);
                    if (logerror)
                    {
                        opis err = new opis();
                        err.PartitionName = "ERR no such partition: " + pn;
#if NETFRAMEWORK
                        err.AddArr(t.Duplicate());
                        err.AddArr(SVC[modelSpecIdx].Duplicate());
#endif
                        global_log.log.AddArr(err);
                    }
                }
                
            }

            return rez;
        }

        #endregion ldc variables

        opis GetLocalDataModel(opis req)
        {
           
            opis rez = req;

            opis SVC = thisins[svcIdx];
            bool inSvc = (SVC.isHere(req.PartitionKind)
               && SVC[req.PartitionKind].PartitionKind == "Action");

          //  Dictionary<string, int> cash = (Dictionary<string, int>)thisins[pkgIdx].bodyObject;
            int poz = -1;
           
            if (inSvc ||
                (poz = thisins[pkgIdx].getPartitionIdx(req.PartitionKind)) >=0)
            {

                opis mod = inSvc ? SVC[req.PartitionKind] : thisins[pkgIdx][poz];
                rez = new opis();
                rez.PartitionKind = "exec";

                rez[exec.message_as_parameter_for_instructions].body = "y";
                rez[exec.instructions].CopyArr(mod);

                if (thisins[svcIdx][req.PartitionKind].body != "!")
                {
                    //копируем на случай если модель должна выступать в роли филлера
                    //ведь тогда мы изменим processParameter который будет являться и modelSpec
                    SVC[modelSpecIdx].CopyArr(req.Duplicate());
                    SVC[modelSpecIdx].Vset("v", req.body);
                    SVC[modelSpecIdx].Vset("a", req.PartitionName);
                }

            }

        
            return rez;
        }

        opis GetPartialRec_LocalDataModel(opis req, opis modelspec)
        {
            opis rez = null;

           
            if (req!= null && req.PartitionKind != null 
                && thisins[svcIdx].isHere(req.PartitionKind)
                && thisins[svcIdx][req.PartitionKind].PartitionKind == "Action")
            {
                opis mod = thisins[svcIdx][req.PartitionKind];

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


        #endregion Exec Action


        // ==========================================================================

        #region BuildActionPath

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
            //BuildActionPath(rez, "http get", true);
            //BuildActionPath(rez, "http post", true);

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

        #endregion BuildActionPath

       

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
#if NETFRAMEWORK
                    AddInstLog("gotAnswer", "", answer);
#endif
                    ExecActionResponceModelsList(sender[MsgTemplate.responce], answer);
                    //AddInstLog("After gotAnswer", "", answer);
                }
                else
                {
                    #if NETFRAMEWORK
                    AddInstLog("gotAnswer but not handled", "", answer);
                    #endif
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

     #region MESSAGING 

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
            opis resendmsg = new opisEventsSubscription();
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
            opis message = new opisEventsSubscription();
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
            opisEventsSubscription message = (opisEventsSubscription) new opis("", values);
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

            message.RaiseEvents(true);

            if (answer.V(ModelAnswer.cancel).Length > 0)
            {
                return false;
            }
            else
            {
#if NETFRAMEWORK
                AddInstLog("ans", message.V("msg"), ppp);
#endif
            }

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

 #if NETFRAMEWORK
            AddInstLog("msg", "", message);
 #endif

            message.PartitionKind = "message";

            // if one message sending in cycle, there is no need to add each time one more listener
            message.PartitionName = receiver;

            message.WaitAction(waiter, true);
            communicator.AddArr(message);
        }


    #endregion  messaging 

        public string[] GetTempSDCstackNames()
        {
           return tempSDCstack.ToArray();
        }

    }

}
