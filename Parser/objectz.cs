using basicClasses.Factory;
using basicClasses.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace basicClasses
{
    //interface ISushnostj
    //{
    //    opis op { get; set;}
    //    string name{ get; set;}
    //    string kind{ get; set;}

    //    opis baseOpis();
    //}

    class objBases 
    {
     
        public static opis baseOpisNotion()
        {
            opis rez = new opis();

            var mf = new ModelFactory();
            rez = mf.GetModel("ModelNotion");

            rez.body = "";

            return rez;
        }


    }



}
