using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using twite;

namespace basicClasses.models.WEB_api
{
    [appliable("file_formats")]
   public class jsonParserModel:ModelBase
    {

        public override void Process(opis message)
        {
            //if (message.PartitionKind != "stringArray")
            //    logopis["err:"].body = "message is not a stringArray ";


            string[] proc=(string[]) message.bodyObject;

            for (int i = 0; i < proc.Length; i++)
            {
                if (proc[i].Length > 4)
                {
                    JsonObject jrez = JsonParser.Parse(proc[i]);
                    opis d = new opis();
                    jrez.BuildTreeopis(d);
                    message["data"] = d;

                    break;
                }
            }

        }

    }


    [appliable("file_formats")]
    public class headersParserModel : ModelBase
    {

        public override void Process(opis message)
        {
            if (message.PartitionKind != "stringArray")
                logopis["err:"].body = "message is not a stringArray ";


            string[] proc = (string[])message.bodyObject;

            opis d = new opis();

            for (int i = 0; i < proc.Length; i++)
            {
                if (string.IsNullOrEmpty(proc[i]))
                    continue;

                int spi = proc[i].IndexOf(':');
                if (spi > 0)
                {
                    string name = proc[i].Substring(0, spi);
                    string val = proc[i].Substring(spi + 1);

                    d.Vset(name.Trim(), val.Trim());
                }  else
                {
                    d.Vset("not header format "+i.ToString(), proc[i]);
                }            
                                              
            }

            message["data"] = d;

        }

    }


    [info("розділені табами дві колонки логін - пароль")]
    [appliable("file_formats")]
    public class AccountsParserModel : ModelBase
    {

        public override void Process(opis message)
        {
            if (message.PartitionKind != "stringArray")
                logopis["err:"].body = "message is not a stringArray ";


            string[] proc = (string[])message.bodyObject;

            opis d = new opis();

            for (int i = 0; i < proc.Length; i++)
            {
                if (string.IsNullOrEmpty(proc[i]))
                    continue;

                int spi = proc[i].IndexOf("\t");
                if (spi > 0)
                {
                    string name = proc[i].Substring(0, spi);
                    string val = proc[i].Substring(spi + 1);

                    d.Vset(name, val);
                }
                else
                {
                    d.Vset("not header format " + i.ToString(), proc[i]);
                }

            }

            message["data"] = d;

        }

    }


    [appliable("file_formats")]
    public class Opis_ParserModel : ModelBase
    {

        public override void Process(opis message)
        {
            if (message.PartitionKind != "stringArray")
                logopis["err:"].body = "message is not a stringArray ";


            string[] proc = (string[])message.bodyObject;

            for (int i = 0; i < proc.Length; i++)
            {
                if (proc[i].Length > 4)
                {
                    //JsonObject jrez = JsonParser.Parse(proc[i]);
                    opis d = new opis();
                    d.load(proc[i]);
                    //jrez.BuildTreeopis(d);
                    message["data"] = d;

                    break;
                }
            }

        }

    }


    [appliable("file_formats")]
    public class boardsset_ParserModel : ModelBase
    {

        public override void Process(opis message)
        {
            if (message.PartitionKind != "stringArray")
                logopis["err:"].body = "message is not a stringArray ";


            string[] proc = (string[])message.bodyObject;

            opis rez = new opis();
            opis currSet= new opis();

            for (int i = 0; i < proc.Length; i++)
            {
                if (proc[i].Trim().Length > 2)
                {
                    //JsonObject jrez = JsonParser.Parse(proc[i]);
                    currSet[proc[i]].body = "";
                   
                 
                }else
                {
                    if(currSet.listCou>1)
                    rez.AddArr(currSet);

                    currSet = new opis();
                    currSet.PartitionName = DateTime.Now.Ticks.ToString();
                }
            }

            if (currSet.listCou > 1)
                rez.AddArr(currSet);

            message["data"] = rez;

        }

    }


    [appliable("file_formats")]
    public class universal_line_pareser : ModelBase
    {
        [info("list of splitters, in actual sequence(as fillows in line)")]
        public static readonly string splitters = "splitters";


        public override void Process(opis message)
        {
            if (message.PartitionKind != "stringArray")
                logopis["err:"].body = "message is not a stringArray ";


            string[] proc = (string[])message.bodyObject;

            opis d = new opis();

            opis spl= modelSpec[splitters].Duplicate();


            for (int i = 0; i < proc.Length; i++)
            {
                if (string.IsNullOrEmpty(proc[i]))
                    continue;

                opis el = new opis();
                string s = proc[i]+"-";
                int pos = 0;

                for (int sp = 0; sp < spl.listCou; sp++)
                {
                    int fpos = s.IndexOf(spl[sp].body[0], pos);

                    el[sp.ToString()].body = s.Substring(pos, fpos - pos);

                    pos = fpos + 1;
                }
                d.AddArr(el);

            }

            message["data"] = d;

        }

    }


}
