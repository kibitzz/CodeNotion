using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace basicClasses.models
{
    public class ModelAnswer:ModelBase
    {
      
        /// <summary>
        /// answer object to be added in original message 
        /// <para> to trigger instance listener(waiter) and process answer</para>
        /// </summary>
        [ignore]
        public static readonly string answer = "answer";

      
        [info("wrapper opis for direct contact(listener/waiter) with current responder to message")]         
        public static readonly string contact = "contact";

        [info("name of instance that answered")]
        public static readonly string notion = "notion";
      

        [info("context to be handled in receiver instance")]
        public static readonly string context = "context";


        [info("hold all parameters from request, so answer contain all initial information")]
        public static readonly string parameters = "parameters";

        [info("")]
        public static readonly string cancel = "cancel";
       
    
    }
}
