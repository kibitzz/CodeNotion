using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace basicClasses.models.String_proc
{
    class DateTimeComp : ModelBase
    {
        [info("number 1..31")]
        [model("")]
        public static readonly string day = "day";

        [info("number 1..12")]
        [model("")]
        public static readonly string month = "month";

        [info("number 0..23")]
        [model("")]
        public static readonly string hour = "hour";

        [info("number 0..59")]
        [model("")]
        public static readonly string minute = "minute";

        [info(" to use in ToString() method to fill string instead of ticks number")]
        [model("")]
        public static readonly string format = "format";

        [model("spec_tag")]
        [info(" ")]
        public static readonly string Get_next_Week_monday = "Get_next_Week_monday";

        [model("spec_tag")]
        [info(" ")]
        public static readonly string add_time_to_date = "add_time_to_date";

        [model("spec_tag")]
        [info(" ")]
        public static readonly string set_curr = "set_curr";

        DateTime curr;

        public override void Process(opis message)
        {
            opis mspec = modelSpec.Duplicate();
            instanse.ExecActionModelsList(mspec);

            try
            {
                var r = new DateTime(DateTime.Now.Year, mspec[month].intVal, mspec[day].intVal, mspec[hour].intVal, mspec[minute].intVal, 0);

                if (mspec.isHere(set_curr))
                    curr = r;

                message.body = mspec[format].isInitlze ? r.ToString(mspec.V(format)) : r.Ticks.ToString();

                message.CopyArr(new opis());
            }
            catch
            {
            }

        }

    }

}
