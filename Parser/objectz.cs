using basicClasses.Factory;
using basicClasses.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace basicClasses
{
   
    class objBases 
    {
     
        public static opis baseOpisNotion()
        {
            opis rez = new opis();

            //var mf = new ModelFactory();
            //rez = mf.GetModel("ModelNotion");
            rez.PartitionKind = "ModelNotion";

            rez.Vset("intellection", "");
            rez.Vset("ontology", "");
            rez.Vset("comments", "");

            rez.body = "";

            return rez;
        }


    }



}
