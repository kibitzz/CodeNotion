using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace basicClasses.models.StructureProcessing
{
    [appliable("BodyValueModificator")]
    [info("filler.  ")]
    class FindCommonDatastructure : ModelBase
    {
        [info(" any filler.   array of variants to proces")]
        [model("")]
        public static readonly string source = "source";


        [model("spec_tag")]
        [info(" ")]
        public static readonly string add_subnames_to_path = "add_subnames_to_path";

        [info("percentage number as double 0.1   0.3  etc")]
        public static readonly string rarely_found_pecent = "rarely_found_pecent";

        bool addsubnames;

        public override void Process(opis message)
        {
            opis spec = modelSpec.Duplicate();
            instanse.ExecActionModelsList(spec);
            var srs = spec[source];

            var typesCou = srs.listCou;

            if (srs.listCou == 0)
            {
                message.body = "empty array of variants";
                return;
            }

            addsubnames = spec.isHere(add_subnames_to_path);                            

            var allPathes = new List<string>();           

            for (int i = 0; i < srs.listCou; i++)
            {
                List<string> itemPathes = new List<string>();
                GetAllPatches(srs[i], "root", itemPathes);

                allPathes.AddRange(itemPathes);
            }

            var distinct = allPathes.Distinct();

            List<opisStatStruct> uniquePathesStat = new List<opisStatStruct>();
          
            foreach (var p in distinct)
            {
                Predicate<string> fff = delegate (string x) { return x == p; };
                uniquePathesStat.Add(new opisStatStruct() { path = p, countPath = allPathes.FindAll(fff).Count() });              
            }


            var strictlyOneForItem = uniquePathesStat.Where(x => x.countPath == typesCou);
            var pathes_strictlyOneForItem = strictlyOneForItem.Select(x => x.path);

            var fewTimesInitem = uniquePathesStat.Where(x => x.countPath >= typesCou);
            var pathes_fewTimesInitem = fewTimesInitem.Select(x => x.path);

            opis baza = srs[0].Duplicate();
            cutByPathesList(baza, pathes_strictlyOneForItem);
           
            baza.PartitionName = "strictly One For Item";
            message.AddArr(baza);

            //----------------------

            baza = srs[0].Duplicate();
            cutByPathesList(baza, pathes_fewTimesInitem);

            baza.PartitionName = "few Times In Item";
            message.AddArr(baza);

            //----------------------

            message.AddArr(GetPercentageBase(srs.Duplicate(), uniquePathesStat, typesCou, 0.95));
            message.AddArr(GetPercentageBase(srs.Duplicate(), uniquePathesStat, typesCou, 0.9));
            message.AddArr(GetPercentageBase(srs.Duplicate(), uniquePathesStat, typesCou, 0.8));
            message.AddArr(GetPercentageBase(srs.Duplicate(), uniquePathesStat, typesCou, 0.7));
            message.AddArr(GetPercentageBase(srs.Duplicate(), uniquePathesStat, typesCou, 0.6));
            message.AddArr(GetPercentageBase(srs.Duplicate(), uniquePathesStat, typesCou, 0.5));
            message.AddArr(GetPercentageBase(srs.Duplicate(), uniquePathesStat, typesCou, 0.4));

            double rarely = 0.1;
            double.TryParse(spec.V(rarely_found_pecent), out rarely);

            message.AddArr(GetPercentageBase(srs.Duplicate(), uniquePathesStat, typesCou, 0.6, false));
            message.AddArr(GetPercentageBase(srs.Duplicate(), uniquePathesStat, typesCou, 0.5, false));
            message.AddArr(GetPercentageBase(srs.Duplicate(), uniquePathesStat, typesCou, 0.4, false));
            message.AddArr(GetPercentageBase(srs.Duplicate(), uniquePathesStat, typesCou, 0.3, false));
            message.AddArr(GetPercentageBase(srs.Duplicate(), uniquePathesStat, typesCou, 0.2, false));
            message.AddArr(GetPercentageBase(srs.Duplicate(), uniquePathesStat, typesCou, 0.1, false));

            message.AddArr(GetPercentageBase(srs.Duplicate(), uniquePathesStat, typesCou, rarely, false));

        }

        opis GetPercentageBase(opis srs, List<opisStatStruct> uniquePathesStat, int typesCou, double prcentage, bool moreThanPercentage = true)
        {
            opis rez = new opis();
            
            int minimum = (int)(typesCou * prcentage);
            var cut = uniquePathesStat.Where(x => moreThanPercentage? x.countPath >= minimum : x.countPath <= minimum).Select(x => x.path);

            int maxpresent = 0;

            if(moreThanPercentage)
            {
                for (int i = 0; i < srs.listCou; i++)
                {                   
                    var pr = cutByPathesList(srs[i], cut);

                    if (pr > maxpresent)
                    {
                        maxpresent = pr;
                        rez = srs[i];
                    }
                }
            } else
            {
                for (int i = 0; i < srs.listCou; i++)
                {                  
                    var pr = MarkByPathesList(srs[i], cut, "rarely found");

                    if (pr > maxpresent)
                    {
                        maxpresent = pr;
                        rez = srs[i];
                    }
                }
            }

            rez.PartitionName = (moreThanPercentage ? "more " : " less ") + (prcentage*100)+ "%";

            return rez;
        }

        int cutByPathesList(opis baza, IEnumerable<string> pl)
        {
            GetAllPatches(baza, "root", new List<string>());

            int rez = 0;
            List<string> allpathes = new List<string>();

            baza.RunRecursively(x => {

                var ok = new List<opis>();
                for (int i = 0; i < x.listCou; i++)
                {
                    var currp = ((opisStatStruct)x[i].bodyObject).path;

                    if (pl.Contains(currp))
                    {
                        allpathes.Add(currp);
                        ok.Add(x[i]);
                    }
                }

                x.SetArr(ok.ToArray());

            });


            return allpathes.Distinct().Count();
        }

        int MarkByPathesList(opis baza, IEnumerable<string> pl, string marker)
        {
            GetAllPatches(baza, "root", new List<string>());

            int rez = 0;
            List<string> allpathes = new List<string>();

            var searchtmpl = baza.Duplicate();
            searchtmpl.PartitionName = "_search_template";

            baza.RunRecursively(x => {
             
                for (int i = 0; i < x.listCou; i++)
                {
                    var currp = ((opisStatStruct)x[i].bodyObject).path;

                    if (pl.Contains(currp))
                    {
                        allpathes.Add(currp);
                        x[i].PartitionKind = marker;
                        x[i].body = "??? " + currp;
                    }
                }              
            });

            GetAllPatches(searchtmpl, "root", new List<string>());
            searchtmpl.RunRecursively(x => {
               
                for (int i = 0; i < x.listCou; i++)
                {
                    var currp = ((opisStatStruct)x[i].bodyObject).path;

                    if (pl.Contains(currp))
                    {                                       
                        x[i].body = "??? " + currp;
                    }
                }
            });

            baza.AddArr(new opis() { PartitionName = "_marker_", PartitionKind = marker });
            baza.AddArr(searchtmpl);

            return allpathes.Distinct().Count();
        }

        string allItemsNames(opis o)
        {
            var rez = "";
            for (int i = 0; i < o.listCou; i++)
            {
                rez +=  o[i].PartitionName+ ",";
            }

           return rez;
        }

        void GetAllPatches(opis o, string cpath, List<string> rez)
        {
            for (int i = 0; i < o.listCou; i++)
            {
                var pp = cpath + " -> " + o[i].PartitionName + (addsubnames ? " ("+allItemsNames(o[i])+")" : "");
                rez.Add(pp);
                o[i].bodyObject = new opisStatStruct() {path = pp };

                GetAllPatches(o[i], pp, rez);
            }
        }
      
    }


    public class opisStatStruct
    {     
        public int countPath;      
        public string path;
    }
}
