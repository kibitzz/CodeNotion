// Copyright (C) 2015-2022 Igor Proskochilo

using basicClasses.Factory;
using basicClasses.models;
using basicClasses.models.sys_ext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace basicClasses
{

    public class OntologyTreeBuilder
    {
        [ignore]
        public static readonly string itemContextID = "itemContextID";

        public opis contextToIgnite;
        public opis contextParameterizeMessages;
        /// <summary>
        /// put here methods you want to exec, and get the result in p element 
        /// </summary>
        public opis messagesToSend;

        contex CTX;
        /// <summary>
        /// global context, contain words descriptions to build contexts, process, and understanding
        /// </summary>
        public opis context;
        private opis StartPreparationMessages;

        public const string globalcomm = "globalcomm";

        public OntologyTreeBuilder()
        {
            CTX = new contex();
        }

        #region Visualisation

        public opis buildOntologyOnly(opis term)
        {
            opis o = new opis();
            o.PartitionName = term.PartitionName;
            buildTree(o);

            return o;
        }

        public static bool isMetaTerm(string term)
        {
            return term.Contains("системн");
        }

        public void buildTree(opis b)
        {

            opis term = context.Find(b.PartitionName);

            if (term.isDuplicated_)
            {
                b.PartitionKind = "circular";
                return;
            }

            if (string.IsNullOrEmpty(term.PartitionName) || !term.isInitlze)
            {                         
                return;
            }
                                
            opis intellection = context.Find(term[ModelNotion.intellection].body.Trim());
            if (!intellection.isInitlze)
            {               
                return;
            }

            term.lockThisForDuplication();

            b.PartitionKind = intellection.PartitionName;

            string ontology = term.V(ModelNotion.ontology) + " "
                + intellection.V(ModelNotion.ontology);

            if (!string.IsNullOrWhiteSpace(ontology))
            {
               
                string[] ttt = ontology.Split();

                foreach (string n in ttt)
                {
                    if (!string.IsNullOrWhiteSpace(n))
                    {
                        opis tmp = new opis();
                        tmp.PartitionName = context.Find(n).PartitionName;
                        if (string.IsNullOrEmpty(tmp.PartitionName))
                            tmp.PartitionName = n;

                        b.AddArr(tmp);

                        buildTree(tmp);
                    }
                }
            }

            term.UnlockThisForDuplication();

        }

        public opis buildFullRelativeOntology(opis term)
        {
            opis o = new opis();
            o.PartitionName = term.PartitionName;
            o.body = term.V(ModelNotion.ontology);
            buildTreeAllRelations(o);

            return o;
        }

        public void buildTreeAllRelations(opis b)
        {

            opis term = context.Find(b.PartitionName);

            if (term.isDuplicated_)
            {
                b.PartitionKind = "circular";
                return;
            }

            if (string.IsNullOrEmpty(term.PartitionName) || !term.isInitlze)
            {
                return;
            }

            opis intellection = context.Find(term[ModelNotion.intellection].body.Trim());
            if (!intellection.isInitlze)
            {
                return;
            }

            term.lockThisForDuplication();

            b.PartitionKind = intellection.PartitionName;

            var inh = FindAllInheritedFromOrHaveInOntology(term).rootForms;


            foreach (string n in inh)
            {
                if (!string.IsNullOrWhiteSpace(n))
                {
                    var inhterm = context.Find(n);
                    opis tmp = new opis();

                    tmp.PartitionName = inhterm.PartitionName;
                    tmp.body = inhterm.V(ModelNotion.ontology);
                    b.AddArr(tmp);

                    buildTreeAllRelations(tmp);
                }
            }


            term.UnlockThisForDuplication();

        }

        public static bool ContainNonPrefixedSuffixed(string word, string text)
        {
            if (string.IsNullOrEmpty(text))
                return false;
           return word.ToLower().Split().Contains(text.ToLower());
        }

        public static bool ContainNonPrefixedSuffixed(string onto, List<string> termForms)
        {
            var ontoterms = onto.ToLower().Split().ToList();
        
            return termForms.Intersect(ontoterms).Any(); ;
        }

        public static bool NotionTreeContainsTerm(opis nt, opis term)
        {
            var forms = OntologyTreeBuilder.Forms(term);
            bool found = false;
            nt.RunRecursively(x => { found = found || forms.Contains(x.PartitionName); });

            return found;
        }

        public static bool NotionTreeLinearOntoContainsTerm(opis nt, opis term)
        {
            var forms = OntologyTreeBuilder.Forms(term);
            bool found = false;
            nt.RunRecursively(x => { found = found || ContainNonPrefixedSuffixed(x.body, forms); });

            return found;
        }

        public static List<string> Forms(opis term)
        {
            var forms = term[ModelNotion.formz].ListPartitions();
            forms.Add(term.PartitionName);

            return forms;
        }

        public (List<string> allForms, List<string> rootForms) FindAllInheritedFromOrHaveInOntology(opis term)
        {
            List<string> allForms = new List<string>();
            List<string> rootForms = new List<string>();

            var forms = Forms(term);           

            for (int i = 0; i < context.listCou; i++)
            {
                var w = context[i];
                var ont = w.V(ModelNotion.ontology);
                var intl = w.V(ModelNotion.intellection);

                if (forms.Where(x=> ContainNonPrefixedSuffixed(ont, x) 
                                 || intl == x).Any())
                {
                    rootForms.Add(w.PartitionName);
                    allForms.Add(w.PartitionName);
                    allForms.AddRange(w[ModelNotion.formz].ListPartitions());
                }
            }

            return (allForms, rootForms);
        }

        #endregion


        public void buildTree(opis term, opis con)
        {
            if (string.IsNullOrEmpty(term.PartitionName)
               || term.isDuplicated_)
            {
                return;
            }

            CTX.Handle(con);

            string currItemID = term.PartitionName + DateTime.Now.Ticks.ToString();

            CTX.itemContextID = currItemID;

            opis curr = CTX.curr;
            opis ncon = curr;

            opis intellection = context.Find(term[ModelNotion.intellection].body.Trim());
            if (!intellection.isInitlze)
            {
                return;
            }

            CTX.AddTermAsItemOfCurrent(term);

            string ontology = term.V(ModelNotion.ontology) + " "
                + intellection.V(ModelNotion.ontology);


            ncon = new opis("context");

            ncon.Vset(models.context.Owner, term.PartitionName + ", " + intellection.PartitionName);
            ncon.Vset(models.context.Organizer, intellection.PartitionName);
            // функция [intellection] : системное
            if (isMetaTerm(intellection.PartitionName))
            {
                ncon.Vset(models.context.Organizer, term.PartitionName);
            }

            CTX.AddContext(ncon);

            string[] ttt = ontology.Split();

            foreach (string n in ttt)
            {
                buildTree(context.Find(n), ncon);
            }

            CTX.Handle(ncon);// switch back to our local context (to add system elem. to proper context),
                             // because previously there was calls to {CTX.AddLvl()} which can change CTX.curr

            #region system elements to global context

            // больше [intellection] : суждение [intellection] : системное
            if (isMetaTerm(intellection.V(ModelNotion.intellection)))
            {
                CTX.AddRootElem(intellection);
            }

            // суждение [intellection] : системное
            if (isMetaTerm(intellection.PartitionName))
            {
                CTX.AddRootElem(term);
            }

            #endregion


        }

        public void initInstances(opis o)
        {
            global_log.log.CopyArr(new opis());
            CTX.Handle(o);

            var sentence = context.Find("sentence_context");
            StartPreparationMessages = sentence["preparation messages"].DuplicateA();

            ModelFactory.hotkeys = sentence["hotkeys"].DuplicateA();           

            CTX.AddRootElem(sentence);
            SysInstance.Words = context;

            o[globalcomm] = new opisEventsSubscription();
            o[globalcomm].PartitionKind = "communicator";

            for (int i = 0; i < o["sys"].listCou; i++)
            {
                opis curr = o["sys"][i];
                
                opis p = context.Find(curr.PartitionName.Trim()).DuplicateA();
                if (!p.isInitlze)
                {
                    continue;
                }
               
                p.InitFuncObj2();
                o["sys_instances"].AddArr(p);                
                p.Process("bind", o);            
            }
        }

        
        public void igniteTree()
        {
            for (int i = 0; i < StartPreparationMessages.paramCou; i++)
                contextToIgnite[globalcomm][StartPreparationMessages[i].PartitionName] = StartPreparationMessages[i].DuplicateA();
           

            if (contextParameterizeMessages != null)
                for (int i = 0; i < contextParameterizeMessages.listCou; i++)
                {
                    contextToIgnite[globalcomm].AddArr(contextParameterizeMessages[i]);
                }

            contextToIgnite[globalcomm]["all"] = new opis("message", "body start");

            if (messagesToSend != null)
                for (int i = 0; i < messagesToSend.listCou; i++)
                {
                    contextToIgnite[globalcomm].AddArr(messagesToSend[i]);
                }

            contextToIgnite[globalcomm]["контекстречення"] = new opis("message", "body NotifyFinished");
        }
        
    }

    public class ScriptRuntime
    {
        // тільки в процесі роботи над проектом ми дізнаємося що є що і які відношення між сутностями домену та його реалізаціями в коді
        // тому дуже важливо мати середовще де ці відношення легко можна описати/установити та модифікувати в процесі пізнання (категоріі, відношення, сутності)

        object scriptLocker = new object();

        /// <summary>
        /// name of function defined in used context
        /// </summary>
        public static string TermTargetFuncName = "target term";

        opis ScriptContext;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"> sentence of one or more words that will define context specifications and awailable functions and classes (terms)</param>
        public ScriptRuntime(string command)
        {
            ScriptContext = InitNewScriptContext(command);
        }

        public ScriptRuntime(opis InitializedScriptContext)
        {
            ScriptContext = InitializedScriptContext;
        }


        public opis CreateMethodMessage(string receiverClass, string method, string param = "")
        {
            var methodRun = new opis();
            var req = "{\"N\": \"###msgreceiv\",\"K\": \"message\",\"B\": \"###msgtype\",\"a\": [{\"N\": \"contTargetModel\",\"K\": \"TargetingChecks\",\"B\": \"\",\"a\": [{\"N\": \"1\",\"K\": \"targetAnyCont\",\"B\": \"\"}]},{\"N\": \"p\",\"K\": \"\",\"B\": \"" + param + "\"}]}";
            methodRun.load(req.Replace("###msgreceiv", receiverClass)
                .Replace("###msgtype", method));

            return methodRun;
        }

        public opis CreateMethodMessage(string receiverClass, string method, opis param)
        {
            var t = CreateMethodMessage(receiverClass, method);

            if (param != null)
                t[MsgTemplate.p] = param;

            return t;
        }

        public opis CreateMethodMessage(string receiverClass, string term, string method, opis param)
        {
            var t = CreateMethodMessage(receiverClass, method, param);

            if (receiverClass != term)
            {
                t[MsgTemplate.contTargetModel] = TargetTermName(term);
            }

            return t;
        }



        string[] SplitSentence(string sentence)
        {
            return sentence.Split();
        }

        public opis InitNewScriptContext(string sentence)
        {          
            return InitNewScriptContext(SplitSentence(sentence));
        }

        public opis InitNewScriptContext(string[] sentenceParts)
        {
            opis cont = null;

            OntologyTreeBuilder otb = new OntologyTreeBuilder();
            otb.context = Parser.ContextGlobal["words"];

            cont = new opis("context");
            cont.PartitionName = "context";
            cont.Vset("level", "topBranch");
            cont.Vset(context.Higher, "none");
            cont.Vset(context.Organizer, "контекстречення");

            foreach (string term in sentenceParts)
            {
                otb.buildTree(otb.context.Find(term), cont);               
            }
           
            cont[OntologyTreeBuilder.globalcomm] = new opisEventsSubscription();
           
            otb.initInstances(cont);

            otb.contextToIgnite = cont;
            otb.igniteTree();

                    
            return cont;
        }


        
        public opis TargetTermName(string term)
        {
            opis rez = new opis();
            rez["1"] = new opis(0) { PartitionName = "1", PartitionKind = TermTargetFuncName, body = term };

            return rez;
        }



        public opis SendMsg(string term, string method, opis param = null)
        {
            var t = Parser.ContextGlobal["words"].Find(term);
            string receiverClass = t.V(ModelNotion.intellection);
            receiverClass = OntologyTreeBuilder.isMetaTerm(receiverClass) ? term : receiverClass;

            var message = receiverClass == term ? CreateMethodMessage(receiverClass, method, param)
                                                : CreateMethodMessage(receiverClass, term, method, param);
          
            return SendMsg(receiverClass,  message);
        }

        public opis SendMsg(string receiverClass, opis message)
        {                    
            lock (scriptLocker)
            {              
                if (ScriptContext != null)             
                {
                    try
                    {
                        //var gk = ScriptContext[OntologyTreeBuilder.globalcomm];
                        //gk[receiverClass] = message;
                        ScriptContext[OntologyTreeBuilder.globalcomm][receiverClass] = message;
                    }
                    catch (Exception e)
                    {
                        opis rez = message[MsgTemplate.p];
                        rez.Vset("Exception", e.Message);
                        rez.Vset("InnerException", e.InnerException?.Message);
                        rez.Vset("StackTrace", e.StackTrace);
                        rez.Vset("InnerException StackTrace", e.InnerException?.StackTrace);
                    }                    
                }
            }
          
            return message[MsgTemplate.p]; // receiverClass put responce back via the received message, responce data is placed in the partition 'p' where was parameter of the message
        }




    }


    public class root : SysInstance
    {
        public string nameSpec;

        public override string Name()
        {
            return nameSpec;
        }

#region IOpisFunctionalInstance

        public override opis Process(string internl, opis environment)
        {
            opis rez = base.Process(internl, environment);

            switch (internl)
            {
                case "вказівник":
                    rez.Vset("command", "accepted");
                    break;
                      
            }

            return rez;
        }

#endregion
    
    }


    public class contex
    {
        opis o;

        Random rnd;
        string itemContextID_;
        public string itemContextID
        {
            get { return itemContextID_; }
            set {

                itemContextID_ = value +"_"+ rnd.Next().ToString(); }
        }
        private string canHandleParentRange;

        public opis curr
        {
            get
            {
                return o;
            }
        }

        public string organizer
        {
            get
            {
                return o.V(models.context.Organizer);
            }
        }

        public static string org(opis p)
        {
            return p.V(models.context.Organizer);
        }

        public contex()
        {
            rnd = new Random();

            canHandleParentRange = "";
        }

        public contex(string canHandlParentRange)
        {
            canHandleParentRange = canHandlParentRange;
        }

        /// <summary>
        /// change working context
        /// </summary>
        /// <param name="p"></param>
        public void Handle(opis p)
        {
            o = p;
        }

        public void AddRootElem(opis p)
        {        
            o["sys"][p.PartitionName].body = p.V("система");
            o["sys"][p.PartitionName].AddArr(o);
        }

        public opis AddContext(opis p)
        {
            if (p.PartitionKind == "context")
            {
                p.PartitionName = "context";

                if (!o.V(context.Owner).Contains(p.V(context.Owner)))
                {
                    p["ID"].body = itemContextID;
                    itemContextID = "";
                    p.WrapByName(o, context.Higher);
                    o["subcon"].AddArr(p);

                    p["sys"] = o["sys"]; // global list of system objects

                    Handle(p);// switch to new added context to work with
                }
            }

            return o;
        }

        public void AddTermAsItemOfCurrent(opis p)
        {
            if (
                !o.V(context.Owner).StartsWith(p.PartitionName+",") &&
                    !o.V(context.Owner).EndsWith(p.PartitionName) )
            {
                // same termin in different context is presented by separate copy
                p = p.DuplicateA();
                p.Vset("itemContextID", itemContextID);
                o["items"].AddArr(p);
            
            }
        }

        public bool CheckParentOrder(opis parent, opis child)
        {
            bool rez = false;
            o = child;
            if (o.PartitionKind != "context")
            {
                return rez;
            }

            while (o[context.Higher].isInitlze)
            {
                o = o.W(context.Higher);

                if (o == parent || o.V("ID") == parent.V("ID"))
                {
                    rez = true;
                    break;
                }               
            }           

            return rez;
        }

        public static opis GetHierarchyStub(opis contextp)
        {
            opis loc = contextp;
            opis rez = new opis("context");
            rez.Vset(context.ID, contextp.V(context.ID));

            opis high = rez[context.Higher];

            while (loc[context.Higher].isInitlze)
            {
                loc = loc.W(context.Higher);

                opis t = new opis();
                t.PartitionName = loc.V(context.ID);
                t.Vset(context.ID, loc.V(context.ID));
                high.Wrap(t);

                high = t[context.Higher];
            }

            return rez;
        }

        public opis GetItemType(string type)
        {
            return GetItemType(type, false);
        }

        public void RecurseItems(opis conpar, opis storage)
        {
             string itemz = "items";

             for (int i = 0; i < conpar[itemz].listCou; i++)
             {                
                 storage.AddArr(conpar[itemz][i]);
             }

            for (int i = 0; i < conpar["subcon"].listCou; i++)
            {
                RecurseItems(conpar["subcon"][i], storage);
            }
        }

        public void FormAllItemsArr()
        {         
            if (!o["all_items"].isInitlze)
            {
                RecurseItems(o, o["all_items"]);
            }
        }

        public opis GetItemContext(opis item)
        {                   
            return RecurseFindItem(o, item);           
        }

        public opis RecurseFindItem(opis conpar, opis item)
        {
            opis rez = new opis();
            string itemz = "items";

            if (conpar[itemz].getPartitionIdx(item.PartitionName) != -1)
            {
                return conpar;
            }

            if (conpar["subcon"].listCou == 0)
            {
                return new opis();
            }
            else
            {
                for (int i = 0; i < conpar["subcon"].listCou; i++)
                {
                    if (!rez.isInitlze)
                    {
                        rez = RecurseFindItem(conpar["subcon"][i], item);
                    }
                    else
                        break;
                }
            }

            return rez;

        }

        public opis GetItemType(string type, bool sub)
        {
            opis t = FindItem(type, ModelNotion.intellection, sub);
            if (t.listCou > 0)
                return t[0];
            else
                return t;          
        }

        public opis GetItemTypeList(string type, bool sub)
        {
           return FindItem(type, ModelNotion.intellection, sub);
        }

        public opis GetItemOperationalList(string type, bool sub)
        {
            return FindItem(type, "operational", sub);
        }

        public opis GetItemOperational(string type, bool sub)
        {
            opis t = FindItem(type, "operational", sub);

            //opis t = FindItem(type, ModelNotion.description, sub);
            if (t.listCou > 0)
                return t[0];
            else
                return t;
        }

        public opis FindItem(string type, string pname, bool sub)
        {
            opis rez = new opis("ListOf_Fitted_Items");
            string itemz = "items";

            if (sub)
            {
                itemz = "all_items";
                if (!o[itemz].isInitlze)
                {
                    RecurseItems(o, o[itemz]);
                }
            }

            for (int i = 0; i < o[itemz].listCou; i++)
            {
                if (o[itemz][i].V(pname).Contains(type))
                {
                    rez.AddArr(o[itemz][i]);
                }
            }
            
            return rez;
        }

        public opis GetItemName(string type)
        {
            return GetItemName(type, false);
        }

        public opis GetItemName(string type, bool sub)
        {
            opis rez = new opis();

            string itemz = "items";

            if (sub)
            {
                itemz = "all_items";
                if (!o[itemz].isInitlze)
                {
                    RecurseItems(o, o[itemz]);
                }
            }

            int i = -1;
            if ((i = o[itemz].getPartitionIdx(type)) != -1)
            {
                rez = o[itemz][i];
            }
            
            return rez;
        }

        public opis FindSubContextByID(opis item)
        {
            opis rez = new opis();

            if (item[OntologyTreeBuilder.itemContextID].isInitlze)
            {
                string id = item.V(OntologyTreeBuilder.itemContextID);

                for (int i = 0; i < o["subcon"].listCou; i++)
                {
                    if (o["subcon"][i].V("ID") == id)
                        rez = o["subcon"][i];
                }
            }

            return rez;           
        }

      
        public opis GoUp()
        {
            opis rez = new opis();
            if (o[context.Higher].isInitlze)
            {
                rez = o.W(context.Higher);
            }

            return rez;
        }
        

    }
}
