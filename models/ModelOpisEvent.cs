using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace basicClasses.models
{
    /// <summary>
    /// opis class can raise events to objects in [waiters] array
    /// <para>each event is described by set of parameters  </para>
    /// </summary>
    class ModelOpisEvent
    {
        /// <summary>
        /// 
        /// <para>  </para>
        /// </summary>
        public static readonly string partKind = "подія";

        /// <summary>
        /// 
        /// <para>  </para>
        /// </summary>
        public static readonly string initiatorPartName = "імя";

        /// <summary>
        /// 
        /// <para>  </para>
        /// </summary>
        public static readonly string initiatorPartKind = "тип";

        /// <summary>
        /// 
        /// <para>  </para>
        /// </summary>
        public static readonly string valueNew = "нове";

        /// <summary>
        /// 
        /// <para>  </para>
        /// </summary>
        public static readonly string valuePrev = "старе";

        /// <summary>
        /// 
        /// <para>  </para>
        /// </summary>
        [info("опис що сповіщується. тільки для IOpisFuncInstanceWaiter")]
        public static readonly string receiver = "receiver";


        ///// <summary>
        ///// 
        ///// <para>  </para>
        ///// </summary>
        //public static readonly string stub9075 = "98yt8gkgjtrdcjy55";



        ///// <summary>
        ///// 
        ///// <para>  </para>
        ///// </summary>
        //public static readonly string stub9705 = "98yt8gkgjtrdcjy55";
    }
}
