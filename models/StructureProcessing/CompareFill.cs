using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace basicClasses.models.StructureProcessing
{
    [info(" check second struct is contained inside first, return duplicated second with comments in branches that is differ or not found")]
    [appliable("Action exe func global_log")]
    class CompareFill : ModelBase
    {
        [model("")]
        [info("")]
        public static readonly string first = "first";

        [model("")]
        [info("")]
        public static readonly string second = "second";

        [model("spec_tag")]
        [info("")]
        public static readonly string deep_is_prior = "deep_is_prior";

        [model("spec_tag")]
        [info("")]
        public static readonly string range_body = "range_body";

        [model("")]
        [info("int")]
        public static readonly string koef = "koef";

        [model("")]
        [info("int")]
        public static readonly string koef_mult = "koef_mult";

        [model("spec_tag")]
        [info("use strict comparison algorithm applied for version control. numerate partitions with same name, do not alanyse moved elements ")]
        public static readonly string VerControlAlgorithm = "VerControlAlgorithm";


        public override void Process(opis message)
        {
            opis ms = modelSpec.Duplicate();
            instanse.ExecActionModelsList(ms);
            opis rez = new opis();

            if (ms.isHere(VerControlAlgorithm))
            {
               opis r = VerControlCompare(ms[first].Duplicate(), ms[second].Duplicate());
                message.CopyArr(new opis());
                message.AddArr(r);
                return;
            }

            var sec = ms[second].Duplicate();

            ms[first].Difference(sec, true, false, 
                ms.isHere(deep_is_prior),
                ms.isHere(range_body),

                ms.isHere(koef) ? ms[koef].intVal : 1,
                ms.isHere(koef_mult) ? ms[koef_mult].intVal : 1
                );


            var fir = ms[first].Duplicate();

            ms[second].Difference(fir, true, false,
                ms.isHere(deep_is_prior),
                ms.isHere(range_body),

                ms.isHere(koef) ? ms[koef].intVal : 1,
                ms.isHere(koef_mult) ? ms[koef_mult].intVal : 1
             );



            message.CopyArr(new opis());

            message["sec"] = sec;
            message["fir"] = fir;

            message["compare_b"].PartitionKind = "different body";
            message["compare_not_f"].PartitionKind = "node not found";
            message["compare_listCou"].PartitionKind = "different listCou";
            message["compare_b+listCou"].PartitionKind = "different body different listCou";

        }

        public static opis VerControlCompare(opis now, opis then)
        {
            then.NormalizeNamesForComparison(now);
            then.NormalizeNamesForComparison(then);

            then.CheckForVersionControl(then, now, "added");
            then.CheckForVersionControl(now, then, "deleted");

            then.PartitionKind = "previous";
            now.PartitionKind = "current";
            opis comp = new opis("comparison result");
            comp.AddArr(now);
            comp.AddArr(then);

            return comp;
        }

    }
}
