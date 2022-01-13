using basicClasses.models.DataSets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

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
    public class webActorModel : ModelBase
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
        [model("")]
        public static readonly string DownloadBytes = "DownloadBytes";

        [info("body contain url ")]
        [model("upLoadFileSpecs")]
        public static readonly string UP_loadResource = "UP_loadResource";

        [info("body contain url ")]
        [model("upLoadFileSpecs")]
        public static readonly string UP_loadResource_put = "UP_loadResource_put";

        [info(" ")]
        [model("")]
        public static readonly string Headers = "Headers";

        [model("")]
        [info("WEB request result role to set  [responce] is default")]
        public static readonly string role = "role";

        [info(" ")]
        [model("")]
        public static readonly string ProxySettings = "ProxySettings";

        [info("to activate this feature -- add SUB partition AcceptAllCertificate : true")]
        public static readonly string AcceptAllCertificate = "AcceptAllCertificate";

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

            hc.AcceptAllCertificates = ex[AcceptAllCertificate][AcceptAllCertificate].isInitlze;
            hc.allowAutoRedrect = ex.isHere(allowAutoRedrect);

            bool rez = false;

            instanse.ExecActionModel(ex[dataToPost], ex[dataToPost]);

            string data = ex.V(dataToPost);

            if (ex.V(post_data_encoding) == "Kirill_to_UTF")
                data = TemplatesMan.Kirill_to_UTF(data);

            if (ex.V(post_data_encoding) == "url_encode")
                data = TemplatesMan.Url_encode(data);

            //if (ex[ProxySettings].isInitlze)// PROXY
            hc.proxySettings = ex[ProxySettings].W();

            opis t = new opis();
            t["request"] = ex;

            if ((ex.isHere(POST) && string.IsNullOrEmpty(ex.V(POST)))
                || (ex.isHere(GET) && string.IsNullOrEmpty(ex.V(GET))))
            {
                t[webResponceModel.Status].body = "fail";
                t.Vset("Error", "you pass empty url");
                SharedContextRoles.SetRole(t, ex.isHere(role) ? ex[role].body : "responce", sharedVal);
                return;
            }

            try
            {

                if (ex.isHere(POST))
                    rez = hc.Post(ex.V(POST), data);

                if (ex.isHere(GET))
                    rez = hc.Get(ex.V(GET));


                if (ex.isHere(DownloadResource))
                {
                    var fi =  hc.DownloadData(ex.V(DownloadResource), ex[DownloadResource].V(upLoadFileSpecs.CompiledFilename));
                    rez = fi == null ? false : fi.Exists && fi.Length > 100;
                }

                if (ex.isHere(DownloadBytes))
                {
                    var fi = hc.DownloadBytes(ex.V(DownloadBytes));
                    rez = fi == null ? false : fi.Length > 0;
                    t[webResponceModel.responseData]["data"].bodyObject = fi;
                    t[webResponceModel.responseData].Vset("length", (fi == null ? 0 : fi.Length).ToString() );
                }
                

                if (ex.isHere(UP_loadResource))
                {
                    t[webResponceModel.Status].body = hc.UploadData(ex.V(UP_loadResource), ex[UP_loadResource].V(upLoadFileSpecs.CompiledFilename));
                    SharedContextRoles.SetRole(t, ex.isHere(role) ? ex[role].body : "responce", sharedVal);
                    return;
                }

                if (ex.isHere(UP_loadResource_put))
                {
                    t[webResponceModel.Status].body = hc.UploadFilePut(ex.V(UP_loadResource_put), ex[UP_loadResource_put].V(upLoadFileSpecs.CompiledFilename));
                    SharedContextRoles.SetRole(t, ex.isHere(role) ? ex[role].body : "responce", sharedVal);
                    return;
                }
            }
            catch (NullReferenceException e)
            {
                t.Vset("internal error", e.Message);
            }


            t[webResponceModel.Status].body = rez.ToString();

            if (!string.IsNullOrEmpty(hc.RedirectLocation))
                t.Vset("RedirectLocation", hc.RedirectLocation);

            if (ex.isHere(DownloadResource))
                t["filename"].body = ex[DownloadResource].V(upLoadFileSpecs.CompiledFilename);
            //t.Vset(webResponceModel.responseData, hc.responseData);

            if (!ex.isHere(DownloadBytes) && !ex.isHere(DownloadResource))
            {
                t[webResponceModel.responseHeaders] = hc.responceHeaders;

                t.Vset(webResponceModel.responseData, hc.responseData.Replace("\n", " "));
                var NewCookies = hc.NewCookies;
                t.Vset(webResponceModel.NewCookies, NewCookies);

                #region NewCookies

                if (NewCookies != null)
                {
                    string[] cookarr = NewCookies.Split(';');
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

                if (hc.responseData != null &&
                    !hc.responseData.StartsWith("<!DOCTYPE html") &&
                    !hc.responseData.StartsWith("<") &&
                    hc.contentType != null &&
                    hc.contentType.Contains("application/json"))
                {
                    if (hc.responseData.Contains("\n"))
                    {
                        hc.responseData = hc.responseData.Replace('\n', ' ');
                    }
                   
                    opis trtrt = new opis();                  
                    trtrt.JsonParce(hc.responseData);                  
                    t[webResponceModel.responseDataParsed]["jsonObj"] = trtrt;
                }
            }

            hc.responseData = null;


            SharedContextRoles.SetRole(t, ex.isHere(role) ? ex[role].body : "responce", sharedVal);

        }



    }
}
