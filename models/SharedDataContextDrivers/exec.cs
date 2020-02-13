using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace basicClasses.models.SharedDataContextDrivers
{
    [info("execute all models in given parameter Process(opis message) or instructions  ower message as parameter")]
    [appliable("Action exe func ")]
   public class exec: ModelBase
    {
        [model("Action")]
        [info("")]
        public static readonly string instructions = "instructions";

        [model("FlagModelSpec")]
        [info("")]
        public static readonly string message_as_parameter_for_instructions = "message_as_parameter_for_instructions";

        [model("")]
        [info("")]
        public static readonly string for_X_times = "for_X_times";

        [model("")]
        [info("")]
        public static readonly string While = "While";

        
        [info("")]
        [ignore]
        public static readonly int SUBJ = 0;
        //public static readonly string SUBJ = "SUBJ";


        public override void Process(opis message)
        {
            opis instLoc = thisins;
            opis datacontext = instLoc[SysInstance.svcIdx][SysInstance.ldcIdx].W();

            if (modelSpec.getPartitionIdx(instructions) != -1)
            {
                opis currSpec = modelSpec;
                opis instr = modelSpec[instructions].Duplicate();
                instanse.ExecActionModel(instr, instr);

                //opis messagerole = instr;

                modelSpec = currSpec;
              
                if (modelSpec[message_as_parameter_for_instructions].isInitlze)
                    instanse.ExecActionResponceModelsList(instr, message);
                else
                {
                    if (modelSpec.isHere(for_X_times) || modelSpec.isHere(While))
                    {
                        if (modelSpec.isHere(While))
                        {
                            opis chk = new opis();
                            chk.PartitionName = "chk";
                            chk.Vset("do", "");
                            opis tim = modelSpec[While].Duplicate();

                            SysInstance locInst = instanse;
                            locInst.ExecActionResponceModelsList(tim, chk);

                            while (chk.V("do") == "y")
                            {
                                locInst.ExecActionModelsList(instr.Duplicate());
                                chk.Vset("do", "");
                                locInst.ExecActionResponceModelsList(tim, chk);
                            }
                        }
                        else
                        {
                            SysInstance locInst = instanse;
                            opis tim = modelSpec[for_X_times].Duplicate();
                            locInst.ExecActionModel(tim, tim);

                            for (int i = 0; i < tim.intVal; i++)
                            {
                                locInst.ExecActionModelsList(instr.Duplicate());
                            }
                        }
                    }
                    else
                        instanse.ExecActionModelsList(instr);
                }
            }
            else
            {
                opis instr = message.Duplicate();
                instanse.ExecActionModelsList(instr);
            }

            instLoc[SysInstance.svcIdx][SysInstance.ldcIdx].Wrap(datacontext);

        }


    }


    [info("execute all models in given parameter Process(opis message) or instructions  ower message as parameter")]
    [appliable("")]
    public class exec_inline : ModelBase
    {
        [model("Action")]
        [info("")]
        public static readonly string instructions = "instructions";
    
        public override void Process(opis message)
        {          
            opis instr = modelSpec[instructions].Duplicate();
            instanse.ExecActionResponceModelsList(instr, message);  
        }


    }

}
