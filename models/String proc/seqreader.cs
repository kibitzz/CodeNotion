using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace basicClasses.models.String_proc
{
    public class seqreader : ModelBase
    {
             
        [info("processing context, as it runs ower one text input multiple times (text, pos, service)")]
        public static readonly string pcontext = "pcontext";

        [model("spec_tag")]
        [info("at first set working context (processed text, position etc)")]
        public static readonly string setContext = "setContext";

        [model("spec_tag")]
        [info("read text block enclosed by quotes")]
        public static readonly string read_quoted = "read_quoted";

        [model("spec_tag")]
        [info("read text block till encounter any of service symbol")]
        public static readonly string read_till_service = "read_till_service";

        [model("spec_tag")]
        [info("")]
        public static readonly string check_end = "check_end";

        [model("spec_tag")]
        [info("set body string where to skip ")]
        public static readonly string skipTo = "skipTo";
      


        opis pcont;
        char[] serviceSymb;
        string text;
        string qconst;

        public override void Process(opis message)
        {
            

            opis locModel = modelSpec.Duplicate();
            instanse.ExecActionModelsList(locModel);

            if (locModel.isHere(setContext))
            {
                pcont = locModel[pcontext].W();
                pcont["text"].body = pcont.V("text").Replace("\n","").Replace("\t", "").Replace("\r", "");
                pcont["text_length"].intVal = pcont.V("text").Length;

                text = pcont["text"].body;
                pcont["text"].body = "";

                serviceSymb = new char[pcont["service"].listCou];

                for (int i = 0; i < serviceSymb.Length; i++)
                {
                    string v = pcont["service"][i].body;

                    if (v.Length > 0)
                        serviceSymb[i] = pcont["service"][i].body[0];
                    else serviceSymb[i] = ' ';
                }

                char h = '"';
                qconst = h.ToString();
            }

            if (locModel.isHere(skipTo))
            {
                int pos = pcont["pos"].intVal;
                int found = text.IndexOf(locModel.V(skipTo), pos);
                if (found != -1)
                {
                    string rez = text.Substring(pos, found - pos);

                    pcont["pos"].intVal = found + locModel.V(skipTo).Length ;

                    pcont["quoted"].body = rez;
                }
            }

            if (locModel.isHere(read_quoted))
            {
                string p = text;
                int pos = pcont["pos"].intVal;
                string q = locModel.V(read_quoted);

                int found = p.IndexOf((string.IsNullOrEmpty(q)? qconst : q), pos);
                if (found != -1)
                {
                    string rez = p.Substring(pos, found - pos);

                    pcont["pos"].intVal = found + 1;

                    pcont["quoted"].body = rez;
                }
                else
                {
                    pcont["pos"].intVal = pcont["text_length"].intVal;
                    pcont["symb"].body = "end";
                }

            }


            if (locModel.isHere(read_till_service))
            {
                string p = text;
                int pos = pcont["pos"].intVal;

                int found = p.IndexOfAny(serviceSymb, pos);
                if (found != -1)
                {
                    string rez = p.Substring(pos, found - pos);

                    pcont["pos"].intVal = found + 1;

                   //* pcont["prev tsrv"].body = pcont["tillservice"].body;
                   //if(!string.IsNullOrEmpty(rez))
                    pcont["tillservice"].body = rez;
                    pcont["prev_symb"].body = pcont["symb"].body;
                    pcont["symb"].body = p[found].ToString();
                    if (p[found] == ' ')
                        pcont["symb"].body = "space";
                }
                else
                {
                    pcont["pos"].intVal = pcont["text_length"].intVal;
                    pcont["symb"].body = "end";
                }
            }

        }
    }
}
