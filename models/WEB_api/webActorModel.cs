using basicClasses.models.DataSets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using twite;

namespace basicClasses.models.WEB_api
{

    public class webResponceModel : ModelBase
    {
        [info("read only")]
        [model("")]
        public static readonly string RedirectLocation = "RedirectLocation";

        [info("read only")]
        [model("")]
        public static readonly string responseData = "responseData";

        [info("")]
        [model("")]
        public static readonly string responseDataParsed = "responseDataParsed";

        [info("returned status of request")]
        [model("")]
        public static readonly string Status = "Status";

        [info("read only")]
        [model("")]
        public static readonly string NewCookies = "NewCookies";

        [info("")]
        [model("")]
        public static readonly string responseHeaders = "responseHeaders";

    }

    [info("do not change message. implement http request functionality")]
    [appliable("Action exec")]
     public  class webActorModel: ModelBase
    {
        [ignore]
        httpClient hc;

        [info("string representation(or opis of json structure) of data to post")]
        [model("")]
        public static readonly string dataToPost = "dataToPost";

        [info("  Kirill_to_UTF  | url_encode  | ")]
        [model("")]
        public static readonly string post_data_encoding = "post_data_encoding";

        [info("")]
        [model("spec_tag")]
        public static readonly string allowAutoRedrect = "allowAutoRedrect";

        [info("body contain url ")]
        [model("")]
        public static readonly string POST = "POST";

        [info("body contain url ")]
        [model("")]
        public static readonly string GET = "GET";

        [info("body contain url ")]
        [model("upLoadFileSpecs")]
        public static readonly string DownloadResource = "DownloadResource";

        [info("body contain url ")]
        [model("upLoadFileSpecs")]
        public static readonly string UP_loadResource = "UP_loadResource";

        [info(" ")]
        [model("")]
        public static readonly string Headers = "Headers";

        [model("")]
        [info("WEB request result role to set  [responce] is default")]
        public static readonly string role = "role";

        [info(" ")]
        [model("")]
        public static readonly string ProxySettings = "ProxySettings";

        

        public override void Process(opis message)
        {
            if (hc == null)
                hc = new httpClient();

            hc.ClearAdditionHeaders();

            opis ex = modelSpec.Duplicate();
            instanse.ExecActionModelsList(ex);

            opis headerz = ex[Headers];

            for (int i = 0; i < headerz.listCou; i++)
            {
                if (!headerz[i].PartitionName.StartsWith("not header format"))             
                    hc.AddHeader(headerz[i].PartitionName + ":" + headerz[i].body);                
            }

            hc.allowAutoRedrect = ex.isHere(allowAutoRedrect);

            bool rez = false;

            instanse.ExecActionModel(ex[dataToPost], ex[dataToPost]);

            string data = ex.V(dataToPost);

          if  (ex.V(post_data_encoding )== "Kirill_to_UTF")
            data = TemplatesMan.Kirill_to_UTF(data);

            if (ex.V(post_data_encoding) == "url_encode")
                data = TemplatesMan.Url_encode(data);

            //if (ex[ProxySettings].isInitlze)// PROXY
                hc.proxySettings = ex[ProxySettings].W();
            


             if (ex.isHere(POST))
             rez = hc.Post(ex.V(POST), data);        

            if (ex.isHere(GET))
                rez = hc.Get(ex.V(GET));
            

            if (ex.isHere(DownloadResource))
            {            
                rez = hc.DownloadData(ex.V(DownloadResource), ex[DownloadResource].V(upLoadFileSpecs.CompiledFilename));
            }

            opis t = new opis();
            t[webResponceModel.Status].body = rez.ToString();

            if(!string.IsNullOrEmpty(hc.RedirectLocation))
            t.Vset("RedirectLocation", hc.RedirectLocation);

            if (ex.isHere(DownloadResource))
                t["filename"].body = ex[DownloadResource].V(upLoadFileSpecs.CompiledFilename);
            //t.Vset(webResponceModel.responseData, hc.responseData);

            if (true && !ex.isHere(DownloadResource))
            {
                t[webResponceModel.responseHeaders] = hc.responceHeaders;

                t.Vset(webResponceModel.responseData, hc.responseData.Replace("\n"," "));              
                t.Vset(webResponceModel.NewCookies, hc.NewCookies);

                #region NewCookies

                if (hc.NewCookies != null)
                {
                    string[] cookarr = hc.NewCookies.Split(';');
                    foreach (string s in cookarr)
                    {
                        //opis o = new opis();
                        int spi = s.IndexOf('=');
                        if (spi > 0)
                        {
                            string name = s.Substring(0, spi);
                            string val = s.Substring(spi + 1);

                            //o.Vset(name, val);
                            t[webResponceModel.NewCookies].Vset(name.Trim(), val); ;
                        }
                    }
                }

                #endregion

                if (!hc.responseData.StartsWith("<!DOCTYPE html") &&
                    !hc.responseData.StartsWith("<") &&
                    hc.contentType != null &&
                    hc.contentType.Contains("application/json"))
                {                   
                    JsonObject jrez = JsonParser.Parse(hc.responseData);
                    opis trtrt = new opis();
                    jrez.BuildTreeopis(trtrt);
                    t[webResponceModel.responseDataParsed] = trtrt;
                }
            }

            t["request"]= ex;

            SharedContextRoles.SetRole(t, ex.isHere(role) ? ex[role].body : "responce", sharedVal);

        }

    }
}
