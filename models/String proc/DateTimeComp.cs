using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace basicClasses.models.String_proc
{
    [appliable("BodyValueModificator")]
    class DateTimeComp : ModelBase
    {
        [info("number 1..31")]
        [model("")]
        public static readonly string day = "day";

        [info("number 1..12")]
        [model("")]
        public static readonly string month = "month";

        [info("number. can be in short format 19  20 ...  so 2000 added automatically ")]
        [model("")]
        public static readonly string year = "year";

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
        [info(" you can spesify amount of days to add inbody of this partition. default value = 7 ")]
        public static readonly string Get_next_Week_monday = "Get_next_Week_monday";

        [model("spec_tag")]
        [info("  ")]
        public static readonly string Get_This_Week_Sunday = "Get_This_Week_Sunday";        

        [model("spec_tag")]
        [info(" ")]
        public static readonly string add_time_to_date = "add_time_to_date";

        [model("spec_tag")]
        [info(" ")]
        public static readonly string set_curr = "set_curr";

        [model("spec_tag")]
        [info(" ")]
        public static readonly string return_actual_date = "return_actual_date";

        DateTime curr;

        public override void Process(opis message)
        {
            opis mspec = modelSpec.Duplicate();
            instanse.ExecActionModelsList(mspec);

            DateTime r = new DateTime();

          

            if (mspec.isHere(return_actual_date))
            {
                r = DateTime.Now;
                if (mspec.isHere(hour))
                {
                    TimeSpan sp = new TimeSpan(mspec[day].intVal, mspec[hour].intVal, mspec[minute].intVal, 0);

                    r = r.Add(sp);
                }
            }

            if (mspec.isHere(month))
            {
                try
                {
                    var yr = !string.IsNullOrEmpty(mspec.V(year)) ? mspec[year].intVal : DateTime.Now.Year;
                    yr = yr < 100 ? 2000 + yr : yr;
                    r = new DateTime(yr, mspec[month].intVal, mspec[day].intVal, mspec[hour].intVal, mspec[minute].intVal, 0);
                }
                catch
                {
                }
            }

            if (mspec.isHere(set_curr))
                curr = r;


            if (mspec.isHere(Get_next_Week_monday))
            {
                r = Dates.MondayForDate(Dates.MondayForDate(curr).AddDays(mspec[Get_next_Week_monday].intVal > 0 
                                                                         || mspec[Get_next_Week_monday].PartitionKind != "spec_tag"
                                                                         ? mspec[Get_next_Week_monday].intVal : 7));
                if (mspec.isHere(hour))
                {
                    TimeSpan sp = new TimeSpan(mspec[hour].intVal, mspec[minute].intVal, 0);
                    r = r.Add(sp);
                }
            }

            if (mspec.isHere(Get_This_Week_Sunday))
            {
                r = Dates.SundayForDate(curr);              
            }



            message.body = mspec[format].isInitlze ? r.ToString(mspec.V(format)) : r.Ticks.ToString();
            message.CopyArr(new opis());

        }

    }


   public class Dates
    {
        //public static long EpochTime(DateTime d)
        //{
        //    var x = new DateTimeOffset(d);
        //    return x.ToUnixTimeSeconds();
        //}

        //public static DateTime FromEpochTime(string t)
        //{
        //    long lt = 0;
        //    long.TryParse(t, out lt);

        //    return FromEpochTime(lt);
        //}

        //public static DateTime FromEpochTime(long t)
        //{
        //    return DateTimeOffset.FromUnixTimeSeconds(t).DateTime;
        //}

        public static DateTime FromStringTicks(string d)
        {
            DateTime rez = new DateTime();
           
            long ticks = 0;
            if (long.TryParse(d, out ticks))
                rez = new DateTime(ticks);

            return rez;
        }

        public static int WeekNumSince(DateTime st, DateTime to)
        {
            var sp = MondayForDate(st) - MondayForDate(to);

            int weeks = (int)sp.TotalDays / 7;

           

            return weeks;
        }

        public static List<DateTime> DaysInPeriod(DateTime st, DateTime fin)
        {
          
            var rez = new List<DateTime>();
            var sp = fin - st;

            rez.Add(st.Date);
            var tmp = st.Date;
           
            for (int i = 1; i <= sp.TotalDays; i++)
            {
                tmp = tmp.AddDays(1).Date;
                rez.Add(tmp);
            }

            return rez;
        }


        public static int WeekForDate(DateTime modelDate, int modelWeek, DateTime target)
        {
            var rez = modelWeek;

            var sp = MondayForDate(target) - MondayForDate(modelDate);

            if (IsOdd((int)sp.TotalDays / 7))
                rez += rez == 1 ? 1 : -1;

            return rez;
        }

        public static DateTime MondayForDate(DateTime target)
        {
            var d = (int)target.DayOfWeek;
            var m = (int)DayOfWeek.Monday;
            if (d == 0)
            {
                d = 7;
            }

            return target.AddDays(m - d).Date;
        }

        public static DateTime SundayForDate(DateTime target)
        {

            int daysUntilSunday = 7 - (int)target.DayOfWeek;
            DateTime thisWeekSunday = target.AddDays(daysUntilSunday == 7 ? 0 : daysUntilSunday).Date.AddHours(23);

            return thisWeekSunday;
        }

        public static bool IsOdd(int value)
        {
            return value % 2 != 0;
        }

     
      
    }


}
