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

        [info("remove closest parent branch from trace list")]
        [model("spec_tag")]
        public static readonly string remove_1st = "remove_1st";

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

            opis tracef = new opis();
            TracePath(ms[tree], foundFrom, tracef);

            opis tracet = new opis();
            TracePath(ms[tree], foundTo, tracet);

            if (ms.isHere(remove_1st))
            {
                tracef.RemoveArrElem(tracef[0]);
                tracet.RemoveArrElem(tracet[0]);

                tracef.arr[tracef.listCou] = null;
                tracet.arr[tracet.listCou] = null;
            }


            rez["path_points_from"] = PathPoints(tracef);
            rez["path_points_to"] = PathPoints(tracet);

            rez["last_common_parent"] = LastCommonItem(tracef, tracet);

            related_as(rez);
            CommonBranchPthPoints(rez);


            message.CopyArr(rez);
        }

        void CommonBranchPthPoints(opis rez)
        {
            opis ci = rez["last_common_parent"]["branch_ref"].W();

            opis PathPoints_from = rez["path_points_from"];

            opis from = new opis();

            for (int i = 0; i < PathPoints_from.listCou; i++)
            {
                if (PathPoints_from[i].W("branch_ref") == ci)
                    break;

                from.AddArr(PathPoints_from[i]);
            }

            //===============
            opis PathPoints_to = rez["path_points_to"];

            opis to = new opis();

            for (int i = 0; i < PathPoints_to.listCou; i++)
            {
                if (PathPoints_to[i].W("branch_ref") == ci)
                    break;

                to.AddArr(PathPoints_to[i]);
            }

            rez["common_parent_path_points_from"] = from;
            rez["common_parent_path_points_to"] = to;

        }

        void related_as(opis rez)
        {
            int depthF = 0;
            int depthT = 0;
            int commonDepth = 0;

            if (rez["last_common_parent"].isInitlze)
            {
                commonDepth = rez["last_common_parent"]["level_split"].intVal;
            }

            if (rez["path_points_from"].isInitlze)
            {
                rez["parent_from"].Wrap(rez["path_points_from"][0]["branch_ref"].W());
                depthF = rez["path_points_from"][0]["total_depth"].intVal;
            }

            if (rez["path_points_to"].isInitlze)
            {
                rez["parent_to"].Wrap(rez["path_points_to"][0]["branch_ref"].W());
                depthT = rez["path_points_to"][0]["total_depth"].intVal;
            }


            if (depthF < depthT)
                rez.Vset("related_as", "lower_to");

            if (depthF > depthT)
                rez.Vset("related_as", "lower_from");

            if (depthF == depthT)
                rez.Vset("related_as", "equal");

            rez["related_as"].Vset("lower_to", (depthT - depthF).ToString());
            rez["related_as"].Vset("lower_from", (depthF - depthT).ToString());
            rez["related_as"].Vset("level_from", (depthF).ToString());
            rez["related_as"].Vset("level_to", (depthT).ToString());

            rez["related_as"].Vset("common_level_from", (depthF - commonDepth).ToString());
            rez["related_as"].Vset("common_level_to", (depthT - commonDepth).ToString());


            rez["related_as"].Vset("common_depth", commonDepth.ToString());
        }

        opis LastCommonItem(opis trace1, opis trace2)
        {
            opis rez = new opis();

            var re2 = trace2.arr.Reverse().Where(x=> x!= null);
            int pos = 0;
            opis b = new opis() { PartitionName = "no data"};

            foreach (var ti1 in trace1.arr.Reverse().Where(x => x != null))
            {
                if (re2.ElementAt(pos) == ti1)
                {
                    b = ti1;                    
                    rez.Vset("level_split", (pos + 1).ToString());
                }

                pos++;
            }

            rez.Vset("branch_kind", b.PartitionKind);
            rez.Vset("branch_name", b.PartitionName);
            rez.WrapByName(b, "branch_ref");

            return rez;
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

        bool TracePath(opis treeBranch, opis item, opis trace)
        {
            bool rez = false;
            bool childFound = false;
            for (int i = 0; i < treeBranch.paramCou; i++)
            {
                if (treeBranch[i] == item)
                {
                    rez = true;
                }
                else
                    childFound = TracePath(treeBranch[i], item, trace);

                if (childFound)
                {
                    trace.AddArr(treeBranch[i]);
                    rez = true;
                    break;
                }

            }

            return rez;
        }

        opis PathPoints(opis trace)
        {
            opis rez = new opis();

            for (int i = 0; i < trace.paramCou; i++)
            {
                var itm = new opis()
                {
                    PartitionName = trace[i].PartitionName,
                    body = trace[i].body,
                    PartitionKind = trace[i].PartitionKind,
                };
                itm.Vset("level_up", i.ToString());
                itm.Vset("level_down", (trace.paramCou - i).ToString());
                itm.Vset("total_depth", (trace.paramCou).ToString());
                itm.WrapByName(trace[i],"branch_ref");               

                rez.AddArr(itm);
            }

            return rez;
        }

        
    }
}
