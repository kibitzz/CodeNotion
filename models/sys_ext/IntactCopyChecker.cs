using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace basicClasses.models.sys_ext
{
    [info("Optimization of functions and constants reuse. !!! incompatible with DuplicateInstrOpt")]
    class IntactCopyChecker : ModelBase
    {
        [model("spec_tag")]
        [info("")]
        public static readonly string activate_copy_tracking = "activate_copy_tracking";

        [model("spec_tag")]
        [info("")]
        public static readonly string permaCopy_mark = "permaCopy_mark";

        [model("spec_tag")]
        [info("")]
        public static readonly string stata = "stata";

        [model("spec_tag")]
        [info("")]
        public static readonly string visualize = "visualize";

        public override void Process(opis message)
        {
            opis surc = message;

#if intact_copy_opt
            void mark(opis x)
            {
                if (x == null) return;
              

                if (x.copy != null)
                {
                    if (x.allCopies != null && opis.CopyIntact(x, x.copy) && (x.source == null || opis.CopyIntact(x.source, x.copy)))
                    {
                        x.permaCopy = (byte)(x.allCopies.Count > 1 ? 1 : 2);

                        foreach (var c in x.allCopies)
                        {
                            opis copy = c.Target as opis;
                            if (copy!= null && !opis.CopyIntact(x, copy))
                            {
                                x.permaCopy = 2;
                                x.copy = null;
                                break;
                            }
                        }
                       
                        foreach (var c in x.allCopies)
                            mark(c.Target as opis);

                    }
                    else
                    {
                        x.copy = null;
                        x.permaCopy = 2;
                    }

                    x.allCopies = null;
                }
                else
                {                  
                    x.permaCopy = 2;
                }
            }
#endif

            if (modelSpec.isHere(permaCopy_mark))
            {
#if intact_copy_opt
                message.RunRecursively(mark);
#endif
            }


            if (modelSpec.isHere(stata))
            {
#if intact_copy_opt
#if debugCopyOpt
                message.Vset("copyExecTotal", opis.copyExecTotal.ToString());
                message.Vset("copyCacheHit", opis.copyCacheHit.ToString());
                message.Vset("copyCacheIntact", opis.copyCacheIntact.ToString());
                message.Vset("copyCacheModified", opis.copyCacheModified.ToString());
                message.Vset("copySourcePresent", opis.copySourcePresent.ToString());
                message.Vset("copyCacheHitSourcePresent", opis.copyCacheHitSourcePresent.ToString());
                message.Vset("copyCacheHitSourceNotMatched", opis.copyCacheHitSourceNotMatched.ToString());
                message.Vset("copyCacheHitSourceNotMatchedAndThisNotMatchedSource", opis.copyCacheHitSourceNotMatchedAndThisNotMatchedSource.ToString());


                message.Vset("copyCacheHitCopyIntact", opis.copyCacheHitCopyIntact.ToString());
                message.Vset("copyCacheHitCopyChanged", opis.copyCacheHitCopyChanged.ToString());

                message.Vset("duplicatedFladWhileDuplacate", opis.duplicatedFladWhileDuplacate.ToString());
                message.Vset("duplicatedFladCopyIntact", opis.duplicatedFladCopyIntact.ToString());
                message.Vset("duplicatedFladCopyChanged", opis.duplicatedFladCopyChanged.ToString());

                message.Vset("duplicateFlag13", opis.duplicateFlag13.ToString());
                message.Vset("duplicateFlag1", opis.duplicateFlag1.ToString());
                message.Vset("duplicateFlag2", opis.duplicateFlag2.ToString());

                //          public static ulong copyCacheHitCopyIntact;
                //public static ulong copyCacheHitCopyChanged;

                //public static ulong duplicatedFladWhileDuplacate;
                //public static ulong duplicatedFladCopyIntact;
                //public static ulong duplicatedFladCopyChanged;

                //      public static ulong duplicateFlag13;
                //public static ulong duplicateFlag1;
                //public static ulong duplicateFlag2;
#endif
#endif
            }

#if intact_copy_opt
            if (modelSpec.isHere(visualize))
            {
                message.RunRecursively(x =>
                {
                    if (x.permaCopy == 1)
                    {
                        x["_CopyIntact_"].body = "y";
                    }
                });
            }

            if (modelSpec.isHere(activate_copy_tracking))
            {
                message.RunRecursively(x =>
                {
                    x.source = null;
                    x.permaCopy = 13;                  
                });
            }
#endif
            

        }
    }
}
