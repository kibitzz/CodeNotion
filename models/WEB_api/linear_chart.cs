using basicClasses.models.sys_ext;
using LiveCharts;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace basicClasses.models.WEB_api
{
    class linear_chart : ModelBase
    {       
        [info("")]
        [model("")]
        public static readonly string source = "source";

        [info("")]
        [model("spec_tag")]
        public static readonly string clear = "clear";
        

        [info("one by one you can populate series Collection and then show form")]
        [model("spec_tag")]
        public static readonly string add_line_int = "add_line_int";

        [info("int val. interval size. maximum in interval ")]
        [model("spec_tag")]
        public static readonly string interval = "interval";

        [info("max  avg  sum  min")]
        [model("spec_tag")]
        public static readonly string interval_func = "interval_func";

        [info("partition name to use its value as X axis label (while Y axis by deafault show values)")]
        [model("")]
        public static readonly string x_axis_labels = "x_axis_labels";

        [info("")]
        [model("spec_tag")]
        public static readonly string show_form = "show_form";
        
        List<IEnumerable<string>> axesx;
        List<IEnumerable<double>> seriez;

        public static diagram form;
        public static guiChartGelegate updateDelegate;

        public override void Process(opis message)
        {
            if (form == null || !form.Visible)
                GuiFomsFactory.Open("linear_chart");

            opis spec = modelSpec.Duplicate();
            instanse.ExecActionModelsList(spec);

            if (seriez == null || spec.isHere(clear))
            {
                seriez = new List<IEnumerable<double>>();                
                axesx = new List<IEnumerable<string>>();
            }

            int intrv = spec[interval].intVal;
            string func = spec.V(interval_func);


            if (spec.isHere(add_line_int))
            {
                IEnumerable<double> vl = string.IsNullOrEmpty(spec.V(add_line_int)) ?
                    spec[source].ListValues().Select(x => { double.TryParse(x.Replace('.', ','), out double v); return v; })
                    : spec[source].ListValues(spec.V(add_line_int)).Select(x => { double.TryParse(x.Replace('.', ','), out double v); return v; });

                if (intrv > 0)
                {
                    int pos = 0;
                    List<double> tmp = new List<double>(vl.Count() / intrv);

                    while (pos < vl.Count() - intrv)
                    {
                        switch (func)
                        {
                            case "max":
                                tmp.Add(vl.Skip(pos).Take(intrv).Max());
                                break;

                            case "sum":
                                tmp.Add(vl.Skip(pos).Take(intrv).Sum());
                                break;

                            case "avg":
                                tmp.Add(vl.Skip(pos).Take(intrv).Average());
                                break;

                            case "min":
                                tmp.Add(vl.Skip(pos).Take(intrv).Min());
                                break;
                        }

                        pos += intrv;
                    }

                    vl = tmp;
                }

              
                seriez.Add(vl.ToArray());

            }

            if (spec.isHere(x_axis_labels) && spec[x_axis_labels].isInitlze)
            {
                var lbl = spec[source].ListValues(spec.V(x_axis_labels));

                if (intrv > 0)
                {
                    int pos = 0;
                    List<string> tmp = new List<string>(lbl.Count() / intrv);

                    while (pos < lbl.Count() - intrv)
                    {
                        tmp.Add(lbl.Skip(pos).First());                       
                        pos += intrv;
                    }

                    lbl = tmp;
                }

                axesx.Add(lbl);
            }

            if (spec.isHere(show_form) && form != null)
            {               
                form.Invoke(updateDelegate, seriez, axesx);             
            }

        }
    }
}
