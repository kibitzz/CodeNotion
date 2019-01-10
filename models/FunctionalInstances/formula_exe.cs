using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace basicClasses.models.FunctionalInstances
{
    [info("OBSOLETE ")]
    [appliable("exe creation")]
    public class formula_exe : ModelBase
    {
        [model("ParamList")]
        [info("")]
        public static readonly string параметри = "параметри";

        [model("")]
        [info("")]
        public static readonly string модель = "модель";

        public override void Process(opis message)
        {
            opis p = message["параметри"];
          
            instanse.ExecActionModelsList(p);

            sharedVal.NotifyReceivers();

            BindToDataContext(message);
            //message.FuncObj = this;
            //sharedVal.SubscribeForNotification(message);

        }

        public override void Phase1(opis item, opis sharedVal)
        {


            // запускаємо модель

            string instructions = item.V(модель);
            //execution
            opis formula = instanse.GetWordForm("формула");
            opis relationSender= formula["execution"]["sentence_set_relation"];
            opis objectSender = formula["execution"]["sentence_add_object"];

            #region  formatting instructions
            instructions = instructions.Replace("(", " ( ");// functional relation
            instructions = instructions.Replace(",", " , ");// list
            instructions = instructions.Replace(".", " . ");// property relation
            instructions = instructions.Replace("^", " ^ ");// direction of action  властивість^предмет
            instructions = instructions.Replace("!", " ! ");// negation
            instructions = instructions.Replace(">", " > ");// jujment type
            instructions = instructions.Replace("<", " < ");// jujment type
            instructions = instructions.Replace("=", " = ");// jujment type
            instructions = instructions.Replace("&", " & ");// jujment type

            instructions = instructions.Replace("#", " # ");
            instructions = instructions.Replace("%", " % ");
            instructions = instructions.Replace(")", " ) ");
            instructions = instructions.Replace("]", " ] ");// контекст [розуміння]
            instructions = instructions.Replace("[", " [ ");



            instructions = instructions.Replace("  ", " ");
            #endregion

            string[] modelRelations = new string[]{ ".", "[", ">", "<", "!", "=", "(", ")", "]","%", "&" };
            string[] arr = instructions.Split();

            opis currObj = new opis();
            opis currProc = new opis();
            string currRelation = "";
            string symbolicRelations = "";

            opis whatWasPreviously;
            opis higherAbstraction = new opis("abstraction", "lvl 1", "");

            foreach (string s in arr)
            {              
                if (s.Length > 1)// це обєкт з контексту
                {
                    currObj = sharedVal[s].W();                
                }

                if (currObj != null && currObj.isInitlze)
                {
                    if (objectSender.isInitlze
                        && objectSender.PartitionKind == MsgTemplate.MsgTemplate_lit)
                    {
                        opis t = objectSender.Duplicate();
                        t[MsgTemplate.p].Wrap(currObj);
                        instanse.ThisRequest(t);
                    }
                    currObj = null;
                }

                #region run functional relation
                //if (s == ")")// 
                //{
                //    currRelation = "";
                //    if (currProc.PartitionKind == MsgTemplate.MsgTemplate_lit)
                //    {                     
                //         instanse.ThisRequest(currProc.V(MsgTemplate.msg_receiver), currProc.V(MsgTemplate.msg), currProc);
                //    }

                //    higherAbstraction.AddArr(new opis("", "type result", "source function", "function "+ MsgTemplate.msg_receiver, ""));
                //    currObj = null;
                //}
                #endregion 

                #region  prepare functional relation
                //if (s == "(")// 
                //{
                //    currRelation = "as_function";
                //    if(currObj[currRelation].isInitlze)
                //    currProc = currObj[currRelation];
                //    else
                //        currProc = currObj.W(currRelation);

                //    currObj = null;
                //}
                #endregion

                #region  prepare functional relation
                if (modelRelations.Contains(s))// 
                {
                    if (relationSender.isInitlze
                         && relationSender.PartitionKind == MsgTemplate.MsgTemplate_lit)
                    {
                        opis t = relationSender.Duplicate();
                        t[MsgTemplate.p].body = s;
                        instanse.ThisRequest(t);
                    }
                }
                #endregion

                #region  numeric constants
                if ((currObj == null || !currObj.isInitlze) 
                    && StrUtils.IsNumber(s) && !string.IsNullOrEmpty(s))// 
                {
                    if (objectSender.isInitlze
                         && objectSender.PartitionKind == MsgTemplate.MsgTemplate_lit)
                    {
                        opis t = objectSender.Duplicate();
                        opis cccc = new opis("constant object");
                        cccc.body = s;
                        cccc.PartitionName = "constant";
                        cccc["Tags"]["isConstant"].body = s;
                        t[MsgTemplate.p].Wrap(cccc);
                        t["param_type"].body = "number_constant";
                       
                        instanse.ThisRequest(t);
                    }
                }
                #endregion

                //if (currProc.isInitlze && currObj!=null
                //    && !string.IsNullOrEmpty(currRelation))// робим щось з обєктом
                //{

                //    if (currProc.PartitionKind == MsgTemplate.MsgTemplate_lit)
                //    {
                //        if(!currProc["arguments"].FindArr(currObj))
                //        currProc["arguments"].AddArr(currObj);
                //    }                    
                //}

            }

        }

        public override void Phase2(opis item, opis sharedVal)
        {

        }

    }
}
