using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace basicClasses.models.sys_ext
{
    [appliable("")]
    [info("")]
    class NodesPathInfo : ModelBase
    {
        [info("structure that contain both branches <from> and <to>")]
        public static readonly string tree = "tree";

        [info("")]
        public static readonly string from = "from";

        [info("")]
        public static readonly string to = "to";
       
        [info("if parameters <from> and <to> do not reference to existing branches within tree, but simple key-value to search  ")]
        [model("spec_tag")]
        public static readonly string by_value = "by_value";

        public override void Process(opis message)
        {
            opis ms = SpecLocalRunAll();
            var rez = new opis();

            opis vfrom = ms[from];
            opis vto = ms[to];

            if (vfrom.PartitionKind == "wrapper")
                vfrom = vfrom.W();
            else
                vfrom = vfrom.listCou == 1 ? vfrom[0] : null;

            if (vto.PartitionKind == "wrapper")
                vto = vto.W();
            else
                vto = vto.listCou == 1 ? vto[0] : null;

            if (vfrom == null || vto == null || ms[tree].listCou == 0)
            {
                message.body = "some params are empty";
                return;
            }

            var foundFrom = Search(ms[tree], vfrom, ms.isHere(by_value));
            var foundTo = Search(ms[tree], vto, ms.isHere(by_value));

            if (foundFrom == null || foundTo == null)
            {
                message.body = "branch not found in tree";
                return;
            }

            //FindPath(ms[tree], foundFrom, foundTo);


            message.CopyArr(rez);
        }

        opis Search(opis tree, opis branch, bool byval)
        {
            opis rez = null;

            tree.RunRecursively(x =>
           {
               if (x == branch || (byval && x.PartitionName == branch.PartitionName
                                        && x.body == branch.body))
               {
                   rez = x;
               }

           });

            return rez;
        }

        void FindPath(opis treeBranch, opis from, opis to, opis info, int currLvl, ref int fromLvl, ref int toLvl)
        {
            for (int i = 0; i < treeBranch.paramCou; i++)
            {
                if (fromLvl == -1 && treeBranch[i] == from)
                {
                    fromLvl = currLvl;
                    info[NodesPathInfo.from]["lvl"].intVal = currLvl;
                    opis pathPoint = new opis() { PartitionName = "point" };

                    info[NodesPathInfo.from]["nodes"].AddArr(treeBranch);
                }

                if (toLvl == -1 && treeBranch[i] == to)
                {
                    toLvl = currLvl;
                    info[NodesPathInfo.to]["lvl"].intVal = currLvl;
                    info[NodesPathInfo.to]["nodes"].AddArr(treeBranch);
                }
                
            }

            int lfromLvl = fromLvl;
            int ltoLvl = toLvl;

            for (int i = 0; i < treeBranch.paramCou; i++)
            {                            
                FindPath(treeBranch[i], from, to, info, currLvl + 1, ref lfromLvl, ref ltoLvl);
                if (currLvl < lfromLvl)
                {
                    
                    info[NodesPathInfo.from]["nodes"].AddArr(treeBranch);
                }
            }

           


        }

    }
}
