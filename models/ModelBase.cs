﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace basicClasses.models
{
  public class ModelBase: ModelInfo, IActionProcessor, IOpisFuncInstanceWaiter
    {
        protected opis env;
        protected SysInstance instanse;

        protected opis thisins;
        protected opis spec;
        protected opis o;
        protected opis communicator;
        protected opis contexts
        {
            get { return env.W("contexts"); }
        }
        protected contex CTX;
        protected opis waiter
        {
            get { return env.W("waiter"); }
        }

        protected opis modelSpec;
        protected opis sharedVal;

        protected opis logopis;
        public opis log { get { return logopis; } }

        public virtual opis GetModelSpecTemplate()
        {
            opis rez = new opis("ModelSpecTemplate");
            rez.Vset("", "");
            rez.Vset("", "");
            rez.Vset("", "");
            rez.Vset("", "");
            rez.Vset("", "");
            rez.Vset("", "");

            return rez;
        }

        public void InitAct(SysInstance ins)
        {
            modelSpec = new opis();
            instanse = ins;          
            env = ins.GetEnvirinmentForModel();
          
            thisins = env.W("thisins");
            spec = env.W("spec");
            o = env.W("currCont");
         
            sharedVal = thisins["sharedVariablesContext"];
            modelSpec = null;

            if (CTX ==null)
            CTX = new contex(spec.V(ModelNotion.canHandleParentRange));

            CTX.Handle(o);

            if(logopis==null)
            logopis = new opis(name);          

        }

        public void CpecifyActionModel(opis specification)
        {
            modelSpec = specification;
          
        }

        public void GetLogForThisins()
        {
            logopis = new opis(name);          
            thisins["Models_log"].AddArr(logopis);
            logopis.CopyArr(modelSpec);
            logopis.body = modelSpec.V("_path_");
        }

        public virtual opis GetMessageModel()
        {
            opis rez = new opis("MessageModel"+ name);
            rez.Vset("", "");
            rez.Vset("", "");
            rez.Vset("", "");
            rez.Vset("", "");
            rez.Vset("", "");
            rez.Vset("", "");

            return rez;
        }

        public virtual void Process(opis message)
        {
            //throw new NotImplementedException();
        }

        public void BindToDataContext(opis modelData)
        {

            modelData.FuncObj = this;
            sharedVal.SubscribeForNotification(modelData);
        }

        public virtual void ProcessWaiter(opis evento, opis sender)
        {
            // ModelOpisEvent.receiver це обєкт що підписаний на сповіщення 
            opis message = evento.W(ModelOpisEvent.receiver);
            opis sharedVal = sender;

        }


        public virtual void Phase1(opis item, opis sharedVal)
        {
        }

        public virtual void Phase2(opis item, opis sharedVal)
        {
        }     

    }

    public class ModelInfo 
    {
        string _name;
        public string name
        {
            get
            {
                if (string.IsNullOrEmpty(_name ))
                {
                    GetDataModel();
                }
                return _name;
            }
        }

        private MethodInfo[] allMethodInfo;
       
        opis _dataModel;
        public opis DataModel
        {
            get
            {
                if (_dataModel == null)
                {
                    GetDataModel();
                }

                return _dataModel.Duplicate();
            }
        }

        opis _Info;
        public opis Info
        {
            get
            {
                if (_Info == null)
                {
                    GetDataModel();
                }

                return _Info;
            }
        }

        opis _actionModel;
        public opis ActionModel
        {
            get
            {
                if (_actionModel == null)
                {
                    GetDataModel();
                }

                return _actionModel;
            }
        }

        [ignore]
        public string ContextsApliable;

        /// <summary>
        /// return all constant partition names in class
        /// </summary>
        /// <returns></returns>
        public opis GetDataModel()
        {
            ContextsApliable = "";
            Type type = this.GetType();
            _name = type.Name;
            _dataModel = new opis(type.Name);
            _actionModel = new opis(type.Name);
            _Info = new opis(type.Name);
        
            foreach (object attribute in type.GetCustomAttributes(true))
            {
                if (attribute is appliableAttribute)
                {
                    ContextsApliable = ((appliableAttribute)attribute).v;
                }

                if (attribute is infoAttribute)
                {
                    _Info[_name].body = ((infoAttribute)attribute).v;
                }
            }

                allMethodInfo = type.GetMethods();
            //allPropertyInfo = type.GetProperties();
            FieldInfo[] fin = type.GetFields();

            foreach (var p in fin)
            {
                bool ignore = false;
                               
                foreach (object attribute in p.GetCustomAttributes(true))
                {
                    if (attribute is modelAttribute)
                    {
                        _dataModel[p.Name].PartitionKind = ((modelAttribute)attribute).v;
                    }

                    if (attribute is infoAttribute)
                    {
                        _Info[p.Name].body = ((infoAttribute)attribute).v;
                    }

                    if (attribute is ignoreAttribute)
                    {
                        ignore = true;
                    }

                }

                if(!ignore) _dataModel.Vset(p.Name, "");

            }

            //foreach (var p in allPropertyInfo)
            //{
            //    _dataModel.Vset(p.Name,"");
            //}

            foreach (var p in allMethodInfo)
            {
                _actionModel.Vset(p.Name, "");
            }

            return _dataModel;
        }

                
    }


}
