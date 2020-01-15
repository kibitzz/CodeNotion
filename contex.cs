// Copyright (C) 2015 Igor Proskochilo

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

        public OntologyTreeBuilder()
        {
            CTX = new contex();
        }

        public opis buildOntologyOnly(opis term)
        {
            opis o = new opis();
            o.PartitionName = term.PartitionName;
            buildTree(o);

            return o;
        }

        public void buildTree(opis b)
        {

            opis term = context.Find(b.PartitionName);

            if (string.IsNullOrEmpty(term.PartitionName)
               || term.isDuplicated_)
            {
                return;
            }
                     
            term.lockThisForDuplication();

            opis intellection = context.Find(term[ModelNotion.intellection].body.Trim());
            if (!intellection.isInitlze)
            {
                return;
            }

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
                        b.AddArr(tmp);

                        buildTree(tmp);
                    }
                }
            }

            term.UnlockThisForDuplication();

        }

        public void buildTree(opis term, opis con)
        {
            if (string.IsNullOrEmpty(term.PartitionName)
               || term.isDuplicated_)       {
                return;          }

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

            //if (!string.IsNullOrWhiteSpace(ontology))
            //{
                ncon = new opis("context");
                           
                ncon.Vset(models.context.Owner, term.PartitionName+", "+ intellection.PartitionName);
                ncon.Vset(models.context.Organizer, intellection.PartitionName);
                // функция [intellection] : системное
                if (intellection.PartitionName.Contains("системн"))
                {
                    ncon.Vset(models.context.Organizer, term.PartitionName);
                }

                CTX.AddContext(ncon);

                string[] ttt = ontology.Split();

                foreach (string n in ttt)
                {
                    buildTree(context.Find(n), ncon);
                }
            //}
           
            CTX.Handle(ncon);// switch back to our local context (to add system elem. to proper context),
                             // because previously there was calls to {CTX.AddLvl()} which can change CTX.curr

           
            #region system elements to global context

            // больше [intellection] : суждение [intellection] : системное
            if (intellection.V(ModelNotion.intellection).Contains("системн"))
            {
                CTX.AddRootElem(intellection);
            }

            // суждение [intellection] : системное
            if (intellection.PartitionName.Contains("системн"))
            {
                CTX.AddRootElem(term);              
            }

            #endregion


        }

        public void initInstances(opis o)
        {
            global_log.log.CopyArr(new opis());
            CTX.Handle(o);
            CTX.AddRootElem(context.Find("sentence_context"));
            SysInstance.Words = context;

            o["globalcomm"].PartitionKind = "communicator";

            for (int i = 0; i < o["sys"].listCou; i++)
            {
                opis curr = o["sys"][i];
                
                opis p = context.Find(curr.PartitionName.Trim()).Duplicate();
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
            contextToIgnite["globalcomm"]["контекстречення"] = new opis("message", "body GenerateTags");
            contextToIgnite["globalcomm"]["all"] = new opis("message", "body GenerateTags");

            if (contextParameterizeMessages != null)
                for (int i = 0; i < contextParameterizeMessages.listCou; i++)
                {
                    contextToIgnite["globalcomm"].AddArr(contextParameterizeMessages[i]);
                }

            contextToIgnite["globalcomm"]["all"] = new opis("message", "body start");

            if (messagesToSend != null)
                for (int i = 0; i < messagesToSend.listCou; i++)
                {
                    contextToIgnite["globalcomm"].AddArr(messagesToSend[i]);
                }

            contextToIgnite["globalcomm"]["контекстречення"] = new opis("message", "body NotifyFinished");
        }
        
    }

    public class ScriptRuntime
    {
        object scriptLocker = new object();

        opis ScriptContext;

        opis CreateMethodMessage(string specName, string method, string param = "")
        {
            var methodRun = new opis();
            var req = "{\"N\": \"api_provider\",\"K\": \"message\",\"B\": \"###msgtype\",\"a\": [{\"N\": \"contTargetModel\",\"K\": \"TargetingChecks\",\"B\": \"\",\"a\": [{\"N\": \"1\",\"K\": \"targetAnyCont\",\"B\": \"\"}]},{\"N\": \"p\",\"K\": \"\",\"B\": \"" + param + "\"}]}";
            methodRun.load(req.Replace("###msgreceiv", specName)
                .Replace("###msgtype", method));

            return methodRun;
        }

        opis CreateMethodMessage(string specName, string method, opis param)
        {
            var t = CreateMethodMessage(specName, method);
            t["p"] = param;

            return t;
        }


        public opis CodenotionScript(string specName, string sysName, string method, opis param)
        {
            var methodRun = CreateMethodMessage(sysName, method, param);
         
            lock (scriptLocker)
            {              
                if (ScriptContext == null)
                {                   
                    OntologyTreeBuilder tpb = new OntologyTreeBuilder();
                    tpb.context = Parser.ContextGlobal["words"];

                    opis o = new opis("context");
                    o.PartitionName = "context";
                    o.Vset("level", "topBranch");
                    o.Vset(context.Higher, "none");
                    o.Vset(context.Organizer, "контекстречення");

                    tpb.buildTree(tpb.context.Find(specName), o);

                    opis currContext = o;
                    currContext["globalcomm"] = new opis();

                    var vmsg = CreateMethodMessage(sysName, "version");

                    tpb.messagesToSend = new opis();
                    tpb.messagesToSend.AddArr(vmsg);

                    tpb.initInstances(currContext);

                    if (SysInstance.Log != null)
                        SysInstance.Log.Clear();

                    tpb.contextToIgnite = currContext;
                    tpb.igniteTree();

                    ScriptContext = currContext;
                    //AddAnalytic(222, "Init new ScriptContext ", specName, vmsg["p"].serialize(), "");
                    ScriptContext["globalcomm"][sysName] = methodRun;
                }
                else
                {
                    try
                    {
                        ScriptContext["globalcomm"][sysName] = methodRun;
                    }
                    catch (Exception e)
                    {
                        //AddAnalytic(222, "Exception running ScriptContext " + e.StackTrace, specName, methodRun.serialize(), e.Message);
                    }                    
                }
            }
          
            return methodRun["p"];
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
                p = p.Duplicate();
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
