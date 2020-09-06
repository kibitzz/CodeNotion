using LiveCharts;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace basicClasses
{
    public delegate void guiChartGelegate(List<IEnumerable<double>> sc, List<IEnumerable<string>> axesx);

    public partial class diagram : Form
    {

        public diagram()
        {
            InitializeComponent();
        }

        public void setLines(List<IEnumerable<double>> sc, List<IEnumerable<string>> axesx)
        {

            cartesianChart1.Series = new SeriesCollection();

            foreach (var s in sc)
                cartesianChart1.Series.Add(new LineSeries
                {
                    Values = new ChartValues<double>(s),
                    PointGeometry = null,
                    LineSmoothness = 0,
                    Title = "",
                });


            cartesianChart1.AxisY.Clear();
            cartesianChart1.AxisY.Add(new Axis
            {                
                LabelFormatter = value => value.ToString("C")
            });

            cartesianChart1.AxisX.Clear();

            foreach (var al in axesx)

                cartesianChart1.AxisX.Add(new Axis
                {
                    Title = "",
                    Labels = (IList<string>) al
                });

            cartesianChart1.Zoom = ZoomingOptions.X;
            cartesianChart1.Pan = PanningOptions.X;
            cartesianChart1.ScrollMode = ScrollMode.X;
            cartesianChart1.DisableAnimations = true;

        }

      
    }


    public enum PropagationType
    {
        forward,
        backward,
        dead,
        rewind,
        stepBack,

    }

    public class World
    {
        public int ActiveGeneration;

        public List<Quardant> content;

        public List<Tribe> tribes;
    }

    public class Tribe
    {
        public int id;
        public int groupsCount;

        public World world;

        public List<CellsGroup> groups;

        public void Propagate(int generation)
        {
            var youthAreal = world.content.Where(x => x.ContainYouthOf(this, generation));

            foreach (Quardant q in youthAreal)
            {
                foreach (CellsGroup g in q.YouthOf(this, generation))
                {
                    var ways = YouthWaysOn(q, g);
                    int splitBy = ways.Count();

                    foreach (Quardant pq in ways)
                    {
                        pq.content.Add(g.Propagate());
                    }

                    g.Spread();

                }
            }
        }

        IEnumerable<Quardant> YouthWaysOn(Quardant quardant, CellsGroup group)
        {
            return quardant.neighbours.Where(x => !x.ContainYouthOf(this, group.generation)
                                           && !x.ContainParentsOf(group)
                                           && !x.ContainPredecessorOf(group)
                                           );
          
        }

        public int GenId()
        {
            groupsCount++;
            return groupsCount;
        }

        void Seed(IEnumerable<Quardant> Quardants)
        {

        }

    }

    public class Quardant
    {
        public List<CellsGroup> content;

        public List<Quardant> neighbours;  // Quardants that do not contain tribes of generation 1

        public double AmountOf(Func<CellsGroup, bool> predicate)
        {
            return content.Where(x => predicate(x)).Sum(x => x.amount);
        }


        public bool ContainYouthOf(Tribe tribe, int generation)
        {
            return YouthOf(tribe, generation).Any();
        }

        public IEnumerable<CellsGroup> YouthOf(Tribe tribe, int generation)
        {
            return content.Where(x => x.tribe == tribe && x.generation == generation);
        }

        public bool ContainParentsOf(CellsGroup group)
        {
            return content.Exists(x => x.IsParentOf(group) );
        }

        public bool ContainOlderThan(CellsGroup group)
        {
            return content.Exists(x => x.IsOlderThan(group));
        }

        public bool ContainPredecessorOf(CellsGroup group)
        {
            return content.Exists(x => x.IsPredecessorOf(group));
        }
    }

    public class CellsGroup
    {
        public int id;
        public int precursorId;
        public CellsGroup precursor;
        public List<CellsGroup> propagations;
        public Tribe tribe;
        public double amount;
        public double quadrantFree;
        public int generation;
        public int pole;
        public PropagationType propagation;

        public CellsGroup Propagate()
        {
            
            var n = new CellsGroup()
            {
                id = tribe.GenId(),
                generation = generation + 1,
                precursorId = id,
                precursor = this,
                pole = pole,
                tribe = tribe,
                propagation = propagation,
                amount = 0,
                quadrantFree =0,
            };

            propagations.Add(n);
            tribe.groups.Add(n);

            return n;
        }

        public void Spread()
        {            
            var part = amount / propagations.Count;
            foreach (var group in propagations)
            {
                group.amount = part;
            }
        }

        public void FittedRecalc()
        {
            int limited = 0;
            double redistribute = 0;
            foreach (var group in propagations)
            {
                if (group.amount > group.quadrantFree)
                {
                    redistribute += group.amount - group.quadrantFree;
                    group.amount = group.quadrantFree;
                    limited++;
                }               
            }

            foreach (var group in propagations)
            {
                if (group.amount < group.quadrantFree)
                {
                    var addon = (redistribute /( propagations.Count - limited))  ;
                    //(group.quadrantFree - group.amount)
                    group.amount = group.quadrantFree;
                    limited++;
                }
            }

        }

        public bool SameDimension(CellsGroup x)
        {
            return x.tribe == this.tribe
                   && x.pole == this.pole ;
        }

        public bool IsParentOf(CellsGroup x)
        {
            return SameDimension(x)
                   && x.precursorId == this.id;
        }

        public bool IsPredecessorOf(CellsGroup x)
        {
            return SameDimension(x)
                   && this.generation == x.generation -1;
        }

        public bool IsOlderThan(CellsGroup x)
        {
            return SameDimension(x)
                   && x.generation > this.generation;
        }
    }

  


}
