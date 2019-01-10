using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace basicClasses.models
{
  public  class Msg
    {
        /// <summary>
        /// cpecial flag to prevent resending message to higher context
        /// <para>  </para>
        /// </summary>      
        public static readonly string no_transfer = "no_transfer";

        /// <summary>
        /// partition in message object, that is resended hierarchically
        /// <para> contains original message (its initiator/listener) </para>
        /// </summary>
        public static readonly string initiator = "initiator";

        /// <summary>
        /// answer object to be added in original message 
        /// <para> to trigger instance listener(waiter) and process answer</para>
        /// </summary>
        public static readonly string answer = "answer";

        /// <summary>
        /// opis for direct contact(listener/waiter) with responder to message
        /// <para>  </para>
        /// </summary>       
        public static readonly string contact = "contact";

       
        /// <summary>
        /// 
        /// <para>  </para>
        /// </summary>
        public static readonly string stub975 = "98yt8gkgjtrdcjy55";
    }
}
