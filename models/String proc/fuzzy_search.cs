using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace basicClasses.models.String_proc
{
    class fuzzy_search : ModelBase
    {
        [info("")]
        [model("")]
        public static readonly string word = "word";

        [info("")]
        [model("fuzziness settings")]
        public static readonly string fuzz = "fuzz";

        [info("fill options here")]
        [model("")]
        public static readonly string options = "options";

        public override void Process(opis message)
        {
            opis ms = SpecLocalRunAll();

            var fs = new fuzzinesSettings(ms[fuzz]);
           
            message.PartitionKind = "";
            message.CopyArr(Search(ms[options].ListPartitions().Select(x => new TermMatch() { term = x }).ToList(),
                                   fs, ms.V(word)));          
        }

        public static opis Search(List<TermMatch> options, fuzzinesSettings fs, string word)
        {
            var rez = SearchLogic.Find(options,
                                      word, fs, true);

            rez = SearchLogic.SortByRelevance(rez, word);

            if (rez.Count > fs.optimization_boundary)
            {
                var lim = rez.First().MatchingPoints - fs.remove_points_offset;
                rez = rez.Where(x => x.MatchingPoints > lim).ToList();
            }

            opis found = new opis();
            rez.ForEach(x => found.Vset(x.term, ""));

            return found;
        }
    }

    public class fuzzinesSettings
    {
        public int times_coef = 20;
        public int length_diff_coef = 5;
        public double matched_to_search_length_pass = 0.59;
        public double substring_coef = 1.2;

        public int optimization_boundary = 50;
        public int remove_points_offset = 200;
        public int badRangeBoundary = 600;

        public  fuzzinesSettings()
        {

        }

        public  fuzzinesSettings(opis fuzz)
        {

            if (int.TryParse(fuzz.V("optimization_boundary"), out int optimization_boundary))
                this.optimization_boundary = optimization_boundary;

            if (int.TryParse(fuzz.V("remove_points_offset"), out int remove_points_offset))
                this.remove_points_offset = remove_points_offset;

            if (int.TryParse(fuzz.V("times_coef"), out int times_coef))
                this.times_coef = times_coef;

            if (int.TryParse(fuzz.V("length_diff_coef"), out int length_diff_coef))
                this.length_diff_coef = length_diff_coef;

            if (double.TryParse(fuzz.V("matched_to_search_length_pass"), out double matched_to_search_length_pass))
                this.matched_to_search_length_pass = matched_to_search_length_pass;

            if (double.TryParse(fuzz.V("substring_coef"), out double substring_coef))
                this.substring_coef = substring_coef;

            if (int.TryParse(fuzz.V("bad_range_boundary_if_one"), out int badRangeBoundary))
                this.badRangeBoundary = badRangeBoundary;
            // GlobalOnlineLog.Add("fuzzinesSettings  " + ((BaseExtApiHelper)api).UniversityName+ "  " + times_coef +"  "+ length_diff_coef + "  " + matched_to_search_length_pass);
        }
    }

    public class SearchLogic
    {

        public static List<TermMatch> Find(List<TermMatch> options,
            string name, fuzzinesSettings f, bool all = false, bool ignoreEmpty = true)
        {
            if (string.IsNullOrEmpty(name))
                return new List<TermMatch>();


            name = name.ToLower().Trim();

            var r2 = FindStartAndContain(options, name);

            var rez = new List<TermMatch>();
            var theOne = new List<TermMatch>();

            FilterByAllComponArrange(name, r2, rez, theOne, f, ignoreEmpty);


            if (theOne.Count == 1 && !all)
                rez = theOne;

            return rez;
        }

        static List<TermMatch> FindStartAndContain(List<TermMatch> options, string name)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();

            var arr = NamingTransformations.SplitSearchGroupName(name);
            var first = name;
            string second = null;

            if (arr.Length > 1)
            {
                first = arr[0];
                second = arr.LastOrDefault();
            }
          
            List<TermMatch> rez;

            if (string.IsNullOrEmpty(second))

                rez = options
                    .Where(x =>
                  x.term.ToLower().Contains(first)).ToList();
            else
                rez = options
                   .Where(x =>
                        x.term.ToLower().Contains(first)
                      || x.term.ToLower().Contains(second)).ToList();

            watch.Stop();
            var elapsedMs2 = watch.ElapsedMilliseconds;
          
            return rez;
        }

      
        static void FilterByAllComponArrange(string name, List<TermMatch> foundRange,
            List<TermMatch> rez, List<TermMatch> theOne, fuzzinesSettings f = null, bool ignoreEmpty = true)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();

            var arr = NamingTransformations.SplitSearchGroupName(name);

            double substr_coef = f == null ? 1.5 : f.substring_coef;

            if (foundRange != null && foundRange.Count > 0)
            {
                foreach (var itm in foundRange)
                {
                    int okcou = 0;
                    var itmArr = NamingTransformations.SplitSearchGroupName(itm.term);


                    okcou = NamingTransformations.Alike(arr, itm.term);
                    double mathchPerc;

                    if ((mathchPerc = isMatched(okcou, arr.Length, itmArr.Length, f)) > 0
                        && (!ignoreEmpty || itm.NotValid == 0))
                    {
                        if (f != null)
                            itm.MatchingPoints = (int)(mathchPerc + (itm.term.ToLower().Contains(name) ? mathchPerc * substr_coef : 0));

                        rez.Add(itm);
                        if (ArraysAreEqual(itmArr, arr))
                            theOne.Add(itm);
                    }
                }
            }

            watch.Stop();
            var elapsedMs2 = watch.ElapsedMilliseconds;
          
        }

        public static double isMatched(int matched, int searchLen, int nameLen, fuzzinesSettings f = null)
        {
            if (f == null)
            {
                return matched == searchLen ? 1000 : 0;
            }

            double rez = 0;
            rez = nameLen > searchLen ?
                      1000 - (f.times_coef * (nameLen / searchLen)) - (f.length_diff_coef * (nameLen - searchLen))
                      : 1000 - (f.times_coef * (searchLen / nameLen)) - (f.length_diff_coef * (searchLen - nameLen));


            if (matched >= searchLen * f.matched_to_search_length_pass)
            {
                rez = rez * ((matched * 1.0) / (searchLen * 1.0));
            }
            else
                rez = 0;

            return rez;
        }

        public static bool ArraysAreEqual(string[] f, string[] s)
        {
            bool rez = false;
            if (f.Length == s.Length)
            {
                rez = true;
                for (int i = 0; i < f.Length; i++)
                {
                    if (f[i] != s[i])
                    {
                        rez = false;
                        break;
                    }
                }
            }
            return rez;
        }


        public static List<TermMatch> SortByRelevance(List<TermMatch> lst, string name)
        {
            if (string.IsNullOrEmpty(name) || lst == null || lst.Count() == 0)
                return lst;
           

            var rez = lst.OrderByDescending(x => x.MatchingPoints).ToList();

            return rez;
        }

    }


    public class NamingTransformations
    {

        public static string ToBase64(string s)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(s);
            return Convert.ToBase64String(buffer);
        }

        public static string FromBase64(string s)
        {
            byte[] data = Convert.FromBase64String(s);
            return Encoding.UTF8.GetString(data);
        }

        public static string MakeUnifiedName(string name)
        {
            return name.ToLower().Replace(" ", "")
                .Replace("_", "")
                .Replace("-", "")
                .Replace("(", "")
                .Replace(")", "")
                .Replace("/", "")
                ;
        }

        public static string[] SplitSearchGroupName(string name)
        {
            //  сп-зг61-1  спзг61 1
            //  рі-г61-1

            List<string> rez = new List<string>();

            name = MakeUnifiedName(name);

            foreach (var ch in name)
            {
                rez.Add(ch.ToString());
            }

            return rez.ToArray();
        }

        public static List<string> SplitMaxLength(string name, int l)
        {
            List<string> rez = new List<string>();

            var nl = name;
            int iter = 0;
            while (nl.Length > 0 && iter < 100)
            {
                iter++;
                var t = nl.Length > l ? nl.Substring(0, l) : nl;
                rez.Add(t);

                l = nl.Length >= l ? l : nl.Length;
                nl = nl.Length >= l ? nl.Remove(0, l) : nl;

            }
            return rez;
        }

        public static int Alike(string[] arr, string name, bool sec = false)
        {
            int pos = -1;
            int okcou = 0;
            int subIdx = 0;
            int foundSubIdx = 0;

            foreach (var sub in arr) // check order
            {
                int postmp = name.IndexOf(sub, pos + 1);

                if (postmp > pos && (postmp - 1 == pos && foundSubIdx + 1 == subIdx || pos < 0 && subIdx == 0))
                {
                    foundSubIdx = subIdx;
                    okcou++;
                    pos = postmp + sub.Length - 1;
                }

                subIdx++;
            }

            return okcou;
        }

        public static int Alike(string[] arr, string name)
        {
            int okcou = 0;

            int[] ra = new int[arr.Length];
            name = MakeUnifiedName(name);
            for (int i = 0; i < arr.Length; i++)
            {
                int first = name.IndexOf(arr[i]);
                int last = name.LastIndexOf(arr[i]);

                int r = 0;

                if (first != last)
                {
                    string[] currarr = arr.TakeLast(arr.Length - i).ToArray();
                    while (first >= 0)
                    {
                        int mr = Alike(currarr, name.Substring(first), true);
                        r = r < mr ? mr : r;

                        first = name.IndexOf(arr[i], first + 1);
                    }
                }
                else
                {
                    r = Alike(arr.TakeLast(arr.Length - i).ToArray(), name, true);
                }

                ra[i] = r;
                okcou += r;
            }

            return okcou;
        }
    
        public static string GetEnclosed(string srs, string st, string fin)
        {
            int pos = 0;
            return GetEnclosed(srs, st, fin, ref pos);
        }

        public static string GetEnclosed(string srs, string st, string fin, ref int pos)
        {
            if (string.IsNullOrEmpty(srs))
                return "";

            var stp = srs.IndexOf(st, pos);
            stp = stp >= 0 ? stp + st.Length : srs.Length;

            var finp = fin.Length > 0 ? srs.IndexOf(fin, stp) : srs.Length - 1;
            pos = pos > finp ? pos : finp + fin.Length;

            if (finp - stp > 0)
                return srs.Substring(stp, finp - stp);
            else
            {
                return "";
            }
        }
       
    }

    public class TermMatch
    {
        public string term;
        public int MatchingPoints;
        public byte NotValid;
    }

    public static class MiscExtensions
    {
        
        public static IEnumerable<T> TakeLast<T>(this IEnumerable<T> source, int N)
        {
            return source.Skip(Math.Max(0, source.Count() - N));
        }
    }

}
