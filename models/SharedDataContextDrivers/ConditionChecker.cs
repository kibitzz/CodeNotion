using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace basicClasses.models.SharedDataContextDrivers
{
    [appliable("exe creation Action ")]
    [info("conditional")]
   public class ConditionChecker:ModelBase
    {

        [model("FillerList")]
        [info("you can fill directly this patrition (replace <FillerList> with some filler model)            if you delete this part, message will be taken as object to check ower")]
        public static readonly string Args = "Args";

        [model("ChecksList")]
        [info("")]
        public static readonly string Checks = "Checks";

        [info("argument could pass only one check from list, to be passed to YES ")]
        [model("spec_tag")]
        public static readonly string OneCheckIsEnough = "OneCheckIsEnough";

        [info("body hold one of these  >   <   =   !=   >=  <=   #(run Checks ower each argument)")]
        public static readonly string oprator = "oprator";

        [info("")]
        [model("ConditionResponceModel")]
        public static readonly string responce = "responce";

        [ignore]
        [info("you can cpecify item in context to which set [yess | no] by placing subitem to this partition with the same name ")]
        public static readonly string result = "result";

        [ignore]
        [info("subscribe to SHARED context items, and if changed recheck condition and execute some actions")]
        public static readonly string autoRevalidate = "autoRevalidate";

        public override void Process(opis message)
        {
            opis p = new opis();
           opis locModel = modelSpec;
            if (locModel.isHere(Args))
            {
                p = modelSpec[Args].Duplicate();
                if (p.listCou > 0)
                    instanse.ExecActionModelsList(p);
                else
                    instanse.ExecActionModel(p, p);
            }
            else
                p.AddArr(message);


            bool one_check = locModel.isHere(OneCheckIsEnough);           

            bool rez = false;

            opis left = p.listCou==2? p[0]: new opis();
            opis right = p.listCou == 2 ? p[1] : new opis();

          
            //logopis.AddArr(sharedVal);
            //logopis.AddArr(left);
            //logopis.AddArr(right);


            switch (locModel[oprator].body)
            {
                case "#":
                    opis chekers = locModel[Checks].Duplicate();
                    instanse.ExecActionModel(chekers, chekers);
       
                    bool notEqualRez = false;
                    rez = !one_check;
                    for (int i = 0; i < p.listCou; i++)
                    {
                        opis mmm = new opis();
                        mmm.WrapByName(p[i].W(), "arg");
                        mmm.PartitionName = "mmmmmk";

                        instanse.ExecActionResponceModelsList(chekers, mmm);

                        if (one_check && mmm["passCou"].intVal > 0)
                        {
                            rez = true;
                            notEqualRez = true;//do not stop, run other checkers- they can do some job in this loop, so simply break loop is wrong
                        }
                        else
                             if (mmm["passCou"].intVal != chekers.listCou)                        
                            rez = notEqualRez;                                
                    }
                    //rez = ;
                    break;

                case "<":
                    rez = left.intVal < right.intVal;
                    break;

                case ">":
                    rez = left.intVal > right.intVal;
                    break;

                case "<=":
                    rez = left.intVal <= right.intVal;
                    break;

                case ">=":
                    rez = left.intVal >= right.intVal;
                    break;

                case "!=":
                    rez = left.body != right.body;
                    break;

                case "=":
                    rez = left.body == right.body;
                    break;
                    
            }

            //if (modelSpec.getPartitionIdx(autoRevalidate) != -1 
            //    && !modelSpec.isHere("alreadyBound"))
            //{
            //    opis eventhandler = new opis("eventhandler");

            //    eventhandler["items to listen"].AddArr(left.Duplicate());
            //    eventhandler["items to listen"].AddArr(right.Duplicate());

            //    eventhandler["exec"] = modelSpec;
            //    modelSpec.Vset("alreadyBound", "yep");

            //    //eventhandler.WrapByName(logopis, "log");
               
            //    BindToDataContext(eventhandler);
            //}

            if (rez)
                instanse.ExecActionResponceModelsList(locModel[responce][ConditionResponceModel.yess], new opis());
            else
                instanse.ExecActionResponceModelsList(locModel[responce][ConditionResponceModel.no],  new opis());
           
            //if (modelSpec[result].isInitlze)
            //    sharedVal[modelSpec[result][0].PartitionKind].body = rez ? "yess" : "no";
           
        }

        public override void ProcessWaiter(opis evento, opis sender)
        {
            opis handler = evento.W(ModelOpisEvent.receiver);
            //  sender is sharedVal data storage in current cntext
            sharedVal = sender;

            if (handler.PartitionKind == "eventhandler")
            {
                bool haveChanged = false;
                for (int i = 0; i < handler["items to listen"].listCou; i++)
                {
                    if (sharedVal.V(handler["items to listen"][i].PartitionName) !=
                        handler["items to listen"][i].body)
                        haveChanged = true;
                }

                if (haveChanged)
                {
                    //logopis = handler["log"].W();
                                   
                    modelSpec = handler["exec"];
                    Process(handler["exec"]);
                }
               
            }

        }

    }
}
