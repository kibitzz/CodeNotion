using basicClasses.models.sys_ext;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace basicClasses.models.WEB_api
{
    [info("simple http server based on HttpListener, you need to run app as administrator.  to return json responce set value to responce partition of request ")]
    public class HttpListenerModel : ModelBase
    {
        [model("spec_tag")]
        [info("")]
        public static readonly string start = "start";

        [model("spec_tag")]
        [info("")]
        public static readonly string stop = "stop";

        [model("")]
        [info("http://localhost:8081/     http://127.0.0.1:8081/    http://*:8080/")]
        public static readonly string prefixes = "prefixes";

        [model("")]
        [info("code to exec for request processing.  for each url make separate branch (Url.AbsolutePath  with  leading and trailing slashes). for all urls use <all> branch name")]
        public static readonly string func = "func";

        static bool serve;

        opis code;

        public override void Process(opis message)
        {

            opis ms = SpecLocalRunAll();
            var rez = new opis();

            if (ms.isHere(start) && !serve)
            {
                instanse.ExecActionModelsList(ms[start]);
                code = ms[func];
                Run(ms[prefixes].ListValues());
                
            }


            if (ms.isHere(stop))
            {
                serve = false;
                instanse.ExecActionModelsList(ms[stop]);
            }
        }


        public void Run(List<string> prefixes)
        {
            var listener = new HttpListener();

            foreach (var p in prefixes)
            {
                listener.Prefixes.Add(p);
            }
           
            listener.Start();

            serve = true;

            while (serve)
            {
                try
                {
                    var context = listener.GetContext();
                    HandleRequest(context);
                  //  ThreadPool.QueueUserWorkItem(o => HandleRequest(context));
                }
                catch (Exception)
                {
                    // Ignored for this example
                }
             
            }

            listener.Stop();

            opis err = new opis();
            err.PartitionName = "http listener stopped ";           
            global_log.log.AddArr(err);
        }

        private void HandleRequest(object state)
        {
            try
            {
                var context = (HttpListenerContext)state;

                var req = context.Request;
                var resp = context.Response;
                resp.StatusCode = 200;
               

                string body;
                using (StreamReader sr = new StreamReader(req.InputStream))
                    body = sr.ReadToEnd();

                opis reqo = new opis() { PartitionName = "param"};
                reqo.Vset("RemoteEndPoint", req.RemoteEndPoint.ToString());
                reqo["json"].JsonParce(body);
                reqo.Vset("AbsolutePath", req.Url.AbsolutePath);
                reqo.Vset("body", body);
                reqo.Vset("Query", req.Url.Query);
                reqo.Vset("HttpMethod", req.HttpMethod);

                var queryString = HttpUtility.ParseQueryString(req.Url.Query);
                foreach (var key in queryString.AllKeys)
                    reqo["Query"].Vset(key, queryString.Get(key));

                instanse.ExecActionResponceModelsList(code["all"], reqo);
                instanse.ExecActionResponceModelsList(code[req.Url.AbsolutePath], reqo);

                var bytes = Encoding.UTF8.GetBytes(reqo["responce"].ToJson());

                resp.ContentType = "application/json";
                resp.ContentEncoding = Encoding.UTF8;
                resp.ContentLength64 = bytes.Length;
                resp.OutputStream.Write(bytes, 0, bytes.Length);
                resp.OutputStream.Close();

                resp.Close();


            }
            catch (Exception)
            {
                // Client disconnected or some other error - ignored for this example
            }
        }



    }


    
}
