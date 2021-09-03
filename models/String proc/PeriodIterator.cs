using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace basicClasses.models.String_proc
{
    [info("filler  - return array of date info in specified period")]
    class PeriodIterator: ModelBase
    {
        [info("date in ticks (long number)")]
        [model("")]
        public static readonly string start = "start";

        [info("date in ticks (long number)")]
        [model("")]
        public static readonly string fin = "fin";

        [info("date in ticks (long number)")]
        [model("")]
        public static readonly string dateForFirstWeek = "dateForFirstWeek";

        [info("")]
        [model("")]
        public static readonly string dateFormat = "dateFormat";

        [info("hour to add to generated date")]
        [model("")]
        public static readonly string hour = "hour";
        [info("minute to add to generated date")]
        [model("")]
        public static readonly string minute = "minute";


        public override void Process(opis message)
        {
            opis spec = modelSpec.Duplicate();
            instanse.ExecActionModelsList(spec);

        
            var dayz = Dates.DaysInPeriod(Dates.FromStringTicks(spec.V(start)), Dates.FromStringTicks(spec.V(fin)));

            var firstW = Dates.FromStringTicks(spec.V(dateForFirstWeek));

            var h = spec[hour].intVal;
            var m = spec[minute].intVal;

            var format = spec[dateFormat].body;

            opis rez = new opis();

            foreach (var date in dayz)
            {
                var d = date.AddHours(h).AddMinutes(m);
                opis dayInfo = new opis();
                dayInfo.PartitionName = "d";
                dayInfo.Vset("DayOfYear", d.DayOfYear.ToString());
                dayInfo.Vset("Year", d.Year.ToString());
                dayInfo.Vset("Month", d.Month.ToString());
                dayInfo.Vset("Day", d.Day.ToString());
                dayInfo.Vset("DayOfWeek", ((int)d.DayOfWeek).ToString());
                dayInfo.Vset("Ticks", d.Ticks.ToString());
                dayInfo.Vset("Formatted", d.ToString(format));

                int wnum = (Dates.WeekNumSince(firstW, d) + 1);
                int week_odd_even = Dates.WeekForDate(firstW, 1, d);
                int quattro_odd_even = 0;
                dayInfo.Vset("week_num", wnum.ToString());
                dayInfo.Vset("week_1_2", week_odd_even.ToString());
               
                quattro_odd_even = (wnum + 3) % 4;
                dayInfo.Vset("week_1_2_3_4", quattro_odd_even.ToString());


                rez.AddArr(dayInfo);
            }

            message.CopyArr(rez);

        }


    }
}
