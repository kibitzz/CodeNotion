using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace basicClasses.models.sys_ext
{
    public class GuiFomsFactory
    {
        public static guiFactoryGelegate deleg;
        public static Form1 callbackFrm;

        public static void Open(string name)
        {
            if (deleg != null && callbackFrm != null)
            {
                callbackFrm.Invoke(deleg, name);
            }
        }
    }
}
