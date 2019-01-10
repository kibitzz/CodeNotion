using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace basicClasses.models.DataSets
{
   public class file_formats: ModelBase
    {

        [info("tree structure from 1c (MY format of data transfer)")]
        public static readonly string TreeStructMy = "TreeStructMy";

        [info("")]
        public static readonly string currentContext = "currentContext";

        [info("")]
        public static readonly string currentContextItem = "currentContextItem";

        [info("")]
        public static readonly string currentAnswer = "currentAnswer";

        [info("")]
        public static readonly string currentMessage = "currentMessage";
    }
}
