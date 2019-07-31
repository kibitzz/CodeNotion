using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace basicClasses.models
{
    [info("all functional and ontological parts of Notion")]
    public class ModelNotion : ModelBase
    {
        [ignore]
        public static readonly string patritionKinda = "ModelNotion";

        [ignore]
        public static readonly string operational = "operational";

        /// <summary>
        /// 
        /// <para>  </para>
        /// </summary>
        [ignore]
        public static readonly string description = "description";

        [info("one word! metalanguage elements only")]
        public static readonly string intellection = "intellection";

        [info("several words")]
        public static readonly string ontology = "ontology";
     

        /// <summary>
        /// obsolete
        /// </summary>
        [ignore]
        public static readonly string pointer = "pointer";

        [info("run after tags initialized, at this point every instance is entirely initialized and we can run AST compilation. contain branches only for each context. may contain raw MsgTemplate in context branch ")]
        [model("Messaging")]
        public static readonly string Start = "Start";

        [info("contain branches for each context and seperate branch for message type within context. to access current message and its parameter system generate local data context (val. acccess by * prefix) contain <currMsg> and <msg p>.    Base methods runs first when handling message, and then spec methods")]
        [model("Messaging")]
        public static readonly string Responces = "Responces";

        [ignore]
        [info("obsolete, relation <has> is established by using <class> notion ")]
        public static readonly string has = "has";
      
        [info("list of notions whose specs should be implemented in term that inherits from it  ")]
        public static readonly string consist = "consist";

        [ignore]
        public static readonly string as_arg = "as_arg";
        
       
        public static readonly string comments = "comments";          

        [ignore]
        public static readonly string canHandleParentRange = "canHandleParentRange";
      
        [ignore]
        public static readonly string root = "root";
      
        [ignore]
        public static readonly string imply = "imply";
      
        [info("class name of instance to use as functional object")]
        [ignore]
        public static readonly string InitFuncObj = "InitFuncObj";
   
        [ignore]        
        public static readonly string sys = "sys";

        [info("executed before any message received, initialization purposes mainly. base model is exec first, then spec code run - that allows you to owerride constants and functions of base modelNotion")]
        [model("Builders")]
        public static readonly string Build = "Build";

        public static readonly string formz = "formz";

        [ignore]       
        public static readonly string ParamTypesConversion = "ParamTypesConversion";


        public override void Process(opis message)
        {
            
        }

        public void CompileNotion(opis n, opis context)
        {
            modelSpec = context;
            if (modelSpec["words"].isInitlze)
            {
                opis no =n;
                string descr = no.V(ModelNotion.intellection);

                if (n.getPartitionIdx(ModelNotion.intellection) != -1)
                {
                    no = modelSpec["words"].Find(n.PartitionName);
                     descr = no.V(ModelNotion.intellection);
                    ConsistPart(no[descr]);
                }
                else
                    ConsistPart(no);               
            }
             
        }


        void  ConsistPart(opis part)
        {
            opis n = modelSpec["words"].Find(part.PartitionName);
            if (n.isInitlze && n.getPartitionIdx(ModelNotion.consist) != -1)
            {
                opis consistArr = n[ModelNotion.consist];
                for (int i = 0; i < consistArr.listCou; i++)
                {
                    ConsistPart(part[consistArr[i].PartitionName]);
                }
            }
        }

    }
}
