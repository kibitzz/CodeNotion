using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace basicClasses.Optimizations
{
    public class ModelSpecIdxPresence
    {
        private static readonly char[] flagChars = new char[] { '0', '@', '!', '#', '*', '$', '|' };
        private static readonly string[] specOpt = new string[] { "0", "lp", "rp", "a", "v" };


        public int a_v_lp_rp_fil_iv_vcon_acon;
        
        public int flags;

        public void Action(opis o)
        {
            o.bodyObject = this;

            var modbody = o.body;
            flags = 0;

            if (!string.IsNullOrEmpty(modbody))
            {
                int len = modbody.Length;
                byte i = 1;
                while (len > 0 && i < 7)
                {
                    // i = Array.IndexOf(flagChars, modbody[i - 1]); //which is optimal?
                    if (modbody.Contains(flagChars[i]))
                    {
                        len--;
                        flags = flags | (1 << i);
                    }
                    i++;
                }
            }

        }

        public void RandomModel(opis o)
        {
            o.bodyObject = this;
            a_v_lp_rp_fil_iv_vcon_acon = 0;

            if (o.listCou > 0 || o.PartitionKind == "fc" || o.PartitionKind == "fr" || o.PartitionKind == "fill")
            {
                a_v_lp_rp_fil_iv_vcon_acon = 1;

                if (!o.isHere("v", false))
                    o.Vset("v", o.body);
                if (!o.isHere("a", false))
                    o.Vset("a", o.PartitionName);
            }

            if (o.isHere("lp", false))
                a_v_lp_rp_fil_iv_vcon_acon = a_v_lp_rp_fil_iv_vcon_acon | (1 << 1);

            if (o.isHere("rp", false))
                a_v_lp_rp_fil_iv_vcon_acon = a_v_lp_rp_fil_iv_vcon_acon | (1 << 2);

        }

    }
}
