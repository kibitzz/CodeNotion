using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace basicClasses.models.StructureProcessing
{
    [info("filler.  ")]
    class FindCommonDatastructure : ModelBase
    {
        [info(" any filler.   array of variants to proces")]
        [model("")]
        public static readonly string source = "source";


        [model("spec_tag")]
        [info(" ")]
        public static readonly string add_subnames_to_path = "add_subnames_to_path";

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
           
        }

        opis GetPercentageBase(opis srs, List<opisStatStruct> uniquePathesStat, int typesCou, double prcentage)
        {
            opis rez = new opis();
            
            int minimum = (int)(typesCou * prcentage);
            var cut = uniquePathesStat.Where(x => x.countPath >= minimum).Select(x => x.path);

            int maxpresent = 0;

            for (int i = 0; i < srs.listCou; i++)
            {               
                GetAllPatches(srs[i], "root", new List<string>());
                var pr = cutByPathesList(srs[i], cut);

                if (pr > maxpresent)
                {
                    maxpresent = pr;
                    rez = srs[i];
                }
            }

            rez.PartitionName = (prcentage*100)+ "%";

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

        string allItemsNames(opis o)
        {
            var rez = "";
            for (int i = 0; i < o.listCou; i++)
            {
                rez += o[i].PartitionName;
            }

           return rez;
        }

        void GetAllPatches(opis o, string cpath, List<string> rez)
        {
            for (int i = 0; i < o.listCou; i++)
            {
                var pp = cpath + " -> " + o[i].PartitionName + (addsubnames ? allItemsNames(o[i]) : "");
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
