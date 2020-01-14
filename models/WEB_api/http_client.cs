
using System.Security.Cryptography.X509Certificates;
using System;
using System.Net;
using System.IO;
using System.IO.Compression; // to decomress receiving data
using System.Text;
using System.Windows.Forms;
using SqlBuilderClasses;
using System.Collections.Specialized;


namespace basicClasses.models.WEB_api
{
    public delegate void EventHandler(int state);

    public class MyPolicy : ICertificatePolicy
    {
        public bool CheckValidationResult(ServicePoint srvPoint,
                                          X509Certificate certificate, WebRequest request,
                                          int certificateProblem)
        {
            //Return True to force the certificate to be accepted.
            return true;
        }
    }

    /// <summary>
    /// Description of httpClient.
    /// </summary>
    public class httpClient
    {
        public int readLimit;
        public string usrResponse
        {
            get
            {
                return dat;
            }
        }
        public string responseData;
        public string cookies;

        opis _proxySettings;
        public opis proxySettings
        {
            get { return (_proxySettings); }

            set
            {
                _proxySettings = value;
                if (_proxySettings["adr"].isInitlze )
                {
                    if (_proxySettings["obj"].bodyObject == null)
                    {
                        myProxy = new WebProxy(_proxySettings.V("adr"),
                            _proxySettings["port"].isInitlze ?
                            _proxySettings["port"].intVal : 8080);

                        if (_proxySettings.isHere("usr"))
                            myProxy.Credentials = new NetworkCredential(_proxySettings.V("usr"), _proxySettings.V("pwd"));

                        _proxySettings["obj"].bodyObject = myProxy;
                    }
                    else
                        myProxy = (WebProxy)_proxySettings["obj"].bodyObject;

                    useProxy = true;
                } else
                    useProxy = false;
            }
        }

        public opis responceHeaders;

        bool useProxy;
     

        public string RedirectLocation;
        public string[] headers;
        public string NewCookies
        {
            get
            {
                string rez = "";
                if (string.IsNullOrEmpty(respCookies)) { return cookies; }
                string[] spl = respCookies.Split(new char[] { ';', ',' });

                foreach (string s in spl)
                {
                    var v = s.Trim();
                    if (v.StartsWith("path=")
                        || v.StartsWith("HttpOnly") || v.StartsWith("expires="))
                        continue;

                    rez += v + "; ";
                }

                    //string[] splKK;
                    //string kkExpr = QueryParser.GetExpresionStructUntyped(cookies, out splKK);

                    //foreach (string s in spl)
                    //{
                    //    string[] lex;
                    //    string expr = QueryParser.GetExpresionStructUntyped(s, out lex);
                    //    if (expr.StartsWith("O=O") && !s.StartsWith(" expires") && !s.StartsWith(" path") && !s.StartsWith(" domain"))
                    //    {
                    //        int idx = Array.IndexOf(splKK, lex[0]);
                    //        if (idx != -1)
                    //        {
                    //            splKK[idx + 1] = lex[1].Trim();
                    //        }
                    //        else
                    //        {
                    //            rez += s + "; ";
                    //        }
                    //    }
                    //}              

                    //rez = rez + QueryParser.Merge(kkExpr, splKK);

                    return rez;

            }
        }
        public static int BytesLoad = 0;
        public static object locker = new object();
        public event EventHandler stateChange;
        int State_;
        public int State
        {
            get { return State_; }

            set
            {
                State_ = value;
                EventHandler handler = this.stateChange;
                if (handler != null) handler(State_);
            }
        }

        Uri urlUri;

        string respCookies;
        string dat;
        string _proxyServer;
        bool _proxyServerOff;
        string _getAccept;
        string _postAccept;
        string _postContentType;
        public string GetAccept
        {
            get
            {
                if (string.IsNullOrEmpty(_getAccept))
                {
                    return "text/html, image/gif, image/x-xbitmap, image/jpeg, image/pjpeg, application/x-shockwave-flash, application/msword, application/x-ms-application, application/x-ms-xbap, application/vnd.ms-xpsdocument, application/xaml+xml, */*";
                }
                else
                {
                    return _getAccept;
                }
            }
            set
            {
                _getAccept = value;
            }
        }
        public string PostAccept
        {
            get
            {
                if (string.IsNullOrEmpty(_postAccept))
                {
                    return "application/json, text/javascript, */*";
                }
                else
                {
                    return _postAccept;
                }
            }
            set
            {
                _postAccept = value;
            }
        }

        public string PostContentType
        {
            get
            {
                if (string.IsNullOrEmpty(_postContentType))
                {
                    return "application/x-www-form-urlencoded; charset=UTF-8";
                }
                else
                {
                    return _postContentType;
                }
            }
            set
            {
                _postContentType = value;
            }
        }
        //		string serverAnswer;
        public string compressType;
        public string contentType;

        public Encoding Encoder;
        public Encoding EncoderOfResponce;
        HttpWebResponse _myHttpWebResponse = null;
        System.IO.Stream _ResponseStream;
        WebProxy myProxy;
        //public static ProxyMan pm;

        private string ResponseString
        {
            get
            {
                string resultData = "";
                if ((_ResponseStream == null) || (!_ResponseStream.CanRead)) { return ""; }

                //if()

                StreamReader streamRead = new StreamReader(_ResponseStream, EncoderOfResponce);
                Char[] readBuffer = new Char[256];
                // Read from buffer
                int ttl = 0;

                int count = -1;

                try
                {
                    count = streamRead.Read(readBuffer, 0, 256);
                }
                catch (Exception) //  bug with server interact, try to fix by handling exception
                { count = -1; }

                while (count > 0) // (count > 0 && (ttl < readLimit || readLimit ==0))
                {
                     //Encoding encoding = Encoding.GetEncoding("windows-1251");
                    ttl += count;
                    resultData += new String(readBuffer, 0, count);
                    try
                    {
                        count = streamRead.Read(readBuffer, 0, 256);
                    }
                    catch (Exception) //  bug with server interact, try to fix by handling exception
                    { count = -1; }
                }

                BytesLoad += ttl;
                // Release the response object resources.

                streamRead.Close();
                _ResponseStream.Close();

                return resultData;
            }
        }

        string refer;
        public  bool allowAutoRedrect;
        public bool AcceptAllCertificates;

        public httpClient()
        {
            Encoder = new UTF8Encoding();
            headers = new string[0];

            _proxyServerOff = false;

        }

        private void SetDecompressRespStream()
        {
            if (compressType == "gzip")
            {
                GZipStream zipStream = new GZipStream(_ResponseStream, CompressionMode.Decompress);
                _ResponseStream = zipStream;
            }

            if (compressType == "deflate")
            {
                DeflateStream defStream = new DeflateStream(_ResponseStream, CompressionMode.Decompress);
                _ResponseStream = defStream;
            }
        }

        public void AddHeader(string headerElement)
        {
            int l = 0;
            if (headers != null)
            {
                l = headers.Length;
            }
            else
            {
                headers = new string[0];
            }
            Array.Resize(ref headers, l + 1);
            headers[l] = headerElement;
        }

        void ClearSpecParams()
        {
            refer = "";
            allowAutoRedrect = false;
        }

        public void ClearAdditionHeaders()
        {
            headers = new string[0];
        }

        public bool Post(string _URL, string dataToPost)
        {
            ClearSpecParams();
            return SendHttpRequest(_URL, dataToPost, "POST", true);
        }

        public bool Post(string _URL, string referHesder, bool redirectAuto, string dataToPost)
        {
            ClearSpecParams();
            refer = referHesder;
            allowAutoRedrect = redirectAuto;
            return SendHttpRequest(_URL, dataToPost, "POST", true);
        }

        public bool Get(string _URL)
        {
            ClearSpecParams();
            allowAutoRedrect = false;
            return SendHttpRequest(_URL.Trim(), "", "GET", true);
        }

        public bool Get(string _URL, string referHesder, bool redirectAuto)
        {
            ClearSpecParams();
            refer = referHesder;
            allowAutoRedrect = redirectAuto;
            return SendHttpRequest(_URL.Trim(), "", "GET", true);
        }

        public bool DownloadData(string StringWebResource, string fileName)
        {
            bool rez = true;
            if (string.IsNullOrEmpty(StringWebResource))
                return false;
           
            WebClient myWebClient = new WebClient();
            //myWebClient.UploadData

            try
            {
                myWebClient.DownloadFile(StringWebResource.Trim('"'), fileName);
            }catch(WebException e)
            {
                rez = false;
            }

            rez = File.Exists(fileName);
            


            return rez;
        }

        public string UploadData(string StringWebResource, string fileName)
        {
            string rez = "ok";
            if (string.IsNullOrEmpty(StringWebResource))
                return "file empty";

            WebClient myWebClient = new WebClient();
            //myWebClient.UploadData

            if (File.Exists(fileName))
            {

                try
                {
                    myWebClient.UploadData(StringWebResource.Trim('"'), DataFileUtils.LoadFile(fileName));
                }
                catch (WebException e)
                {
                    rez = e.Message ;
                }
            }

            return rez;
        }


        // multipart/form-data
        public bool UploadFilesToRemoteUrl(string url, string refer, string[] files, NameValueCollection nvc, NameValueCollection nvcFinal)
        {

            string boundary = "----------------------------" + DateTime.Now.Ticks.ToString("x");


            HttpWebRequest httpWebRequest2 = (HttpWebRequest)WebRequest.Create(url);

            #region headers

            httpWebRequest2.ServicePoint.Expect100Continue = false;
           
            foreach (string header in headers)
            {

                if (header.StartsWith("Referer"))
                    httpWebRequest2.Referer = header.Substring(8).Trim();
                else
                    if (header.StartsWith("Content-Type"))
                    httpWebRequest2.ContentType = header.Substring(13).Trim();
                else
                    if (header.StartsWith("Accept:"))
                    httpWebRequest2.Accept = header.Substring(7).Trim();
                else
                    if (header.StartsWith("Host"))
                    httpWebRequest2.Host = header.Substring(5).Trim();
                else
                if (header.StartsWith("Content-Length"))
                { }
                else
               if (header.StartsWith("User-Agent"))
                    httpWebRequest2.UserAgent = header.Substring(11).Trim();

                else
                    if (header.StartsWith("Connection"))
                {
                    if (header.Contains("keep-alive"))
                        httpWebRequest2.KeepAlive = true;
                    //myHttpWebRequest.Connection = header.Substring(11).Trim();
                }
                else
                    httpWebRequest2.Headers.Add(header);
            }

            httpWebRequest2.ContentType = "multipart/form-data; boundary=" + boundary;
            httpWebRequest2.Method = "POST";
           
            httpWebRequest2.ServicePoint.Expect100Continue = false;
            httpWebRequest2.AllowAutoRedirect = false;

            #endregion

            Stream memStream = new System.IO.MemoryStream();
            byte[] boundarybytesST = System.Text.Encoding.ASCII.GetBytes("--" + boundary + "\r\n");
            byte[] boundarybytes = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");
            byte[] boundarybytesend = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "--\r\n");

            string formdataTemplate = "\r\n--" + boundary + "\r\nContent-Disposition: form-data; name=\"{0}\"\r\n\r\n{1}";
            string formdataTemplatest = "--" + boundary + "\r\nContent-Disposition: form-data; name=\"{0}\"\r\n\r\n{1}";

            int cc = 0;
            foreach (string key in nvc.Keys)
            {
                cc++;
                string formitem = string.Format(cc > 1 ? formdataTemplate : formdataTemplatest, key, nvc[key]);
                byte[] formitembytes = System.Text.Encoding.ASCII.GetBytes(formitem);
                memStream.Write(formitembytes, 0, formitembytes.Length);
            }

            memStream.Write(boundarybytes, 0, boundarybytes.Length);


            string headerTemplate = "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\nContent-Type: image/pjpeg\r\n\r\n";

            for (int i = 0; i < files.Length; i++)
            {
                string header = string.Format(headerTemplate, "profile_image[uploaded_data]", files[i]);
                byte[] headerbytes = System.Text.Encoding.UTF8.GetBytes(header);

                memStream.Write(headerbytes, 0, headerbytes.Length);

                FileStream fileStream = new FileStream(files[i], FileMode.Open, FileAccess.Read);
                byte[] buffer = new byte[1024];
                int bytesRead = 0;
                while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
                {
                    memStream.Write(buffer, 0, bytesRead);
                }

                fileStream.Close();
            }

            foreach (string key in nvcFinal.Keys)
            {
                string formitem = string.Format(formdataTemplate, key, nvcFinal[key]);
                //	byte[] formitembytes = System.Text.Encoding.ASCII.GetBytes(formitem);  //HACK: change ascii encoding to utf because twitter donot recognize kirillik names
                byte[] formitembytes = System.Text.Encoding.UTF8.GetBytes(formitem);
                memStream.Write(formitembytes, 0, formitembytes.Length);
            }

            memStream.Write(boundarybytesend, 0, boundarybytesend.Length);

            httpWebRequest2.ContentLength = memStream.Length;
            Stream requestStream = httpWebRequest2.GetRequestStream();

            memStream.Position = 0;
            byte[] tempBuffer = new byte[memStream.Length];
            memStream.Read(tempBuffer, 0, tempBuffer.Length);
            memStream.Close();

            requestStream.Write(tempBuffer, 0, tempBuffer.Length);
            requestStream.Close();

            Stream stream2;
            HttpWebResponse webResponse2;
            try
            {
                webResponse2 = (HttpWebResponse)httpWebRequest2.GetResponse();
                stream2 = webResponse2.GetResponseStream();
            }
            catch (Exception e)
            {
                //				MessageBox.Show(e.Message);
                return false;
            }

            StreamReader reader2 = new StreamReader(stream2);

            reader2.ReadToEnd();

            bool rez = false;
            if (webResponse2.StatusCode == HttpStatusCode.Redirect || webResponse2.StatusCode == HttpStatusCode.OK)
            {
                rez = true;
            }

            webResponse2.Close();
            httpWebRequest2 = null;
            webResponse2 = null;

            return rez;
        }


        private bool SendHttpRequest(string _URL, string dataToPost, string metod, bool waitResponse)
        {
            if (string.IsNullOrEmpty(_URL))
                return false;

          
            responceHeaders = new opis();
            RedirectLocation = "";
           responseData = "";
            State = 0;

            try
            {
                urlUri = new Uri(_URL);
            }
            catch
            {
                responseData = "url not valid  " + _URL;
                return false;
            }
           
            HttpWebRequest myHttpWebRequest = (HttpWebRequest)WebRequest.Create(urlUri);
            myHttpWebRequest.Method = metod;
            myHttpWebRequest.Timeout = 200000;
            EncoderOfResponce = Encoder;

            if (AcceptAllCertificates)
                myHttpWebRequest.ServerCertificateValidationCallback = UseSecurityProtocol.AcceptAllCertificatePolicy;

            #region  Proxy
            if (useProxy)
            {
                myHttpWebRequest.Proxy = myProxy;                
            }
            #endregion

            State = 1;

            byte[] byte1 = null;
            if (dataToPost != "")
            {
                byte1 = Encoder.GetBytes(dataToPost);
                myHttpWebRequest.ContentLength = byte1.Length;
            }

            //			myHttpWebRequest.Connection = "keep-alive";

            #region Headers
          
            string[] SpecHeaders = new string[] { "UserAgent" ,"Referer", "Content-Type", "Accept" };
            foreach (string header in headers)
            {
                string headerl = header.ToLower();
                 if (headerl.StartsWith("referer"))
                    myHttpWebRequest.Referer = header.Substring(8).Trim();
                else
                     if (headerl.StartsWith("content-type"))
                    myHttpWebRequest.ContentType = header.Substring(13).Trim();
                else
                     if (headerl.StartsWith("accept:") )
                    myHttpWebRequest.Accept = header.Substring(7).Trim();
                else
                     if (headerl.StartsWith("host"))
                    myHttpWebRequest.Host = header.Substring(5).Trim();
                else
                 if (headerl.StartsWith("content-length"))
                { }
                else
                if (headerl.StartsWith("user-agent"))
                    myHttpWebRequest.UserAgent = header.Substring(11).Trim();
                
                else
                     if (headerl.StartsWith("connection"))
                {
                    if (header.Contains("keep-alive"))
                        myHttpWebRequest.KeepAlive = true;
                    //myHttpWebRequest.Connection = header.Substring(11).Trim();
                }
                else
                    myHttpWebRequest.Headers.Add(header);
            }


            myHttpWebRequest.ServicePoint.Expect100Continue = false;          

            #endregion

            #region send Request and get Response
            myHttpWebRequest.AllowAutoRedirect = allowAutoRedrect;
        
            if (readLimit > 0)
            {
                myHttpWebRequest.AddRange(0, readLimit);
            }

            try
            {
                if (dataToPost != "")
                {
                    State = 2;
                    Stream newStream = myHttpWebRequest.GetRequestStream();
                    newStream.Write(byte1, 0, byte1.Length);
                    newStream.Close();
                }

                //				lock(locker)
                //				{
                State = 3;
                if (waitResponse)
                {
                    _myHttpWebResponse = (HttpWebResponse)myHttpWebRequest.GetResponse();
                }
                //				}
            }
            catch (Exception e)
            {
                responseData = e.ToString();

                if (myHttpWebRequest != null && myHttpWebRequest.HaveResponse && ((WebException)e).Response != null)
                {
                    _myHttpWebResponse = (HttpWebResponse)(((WebException)e).Response);
                }
                else
                {
                    //if(!string.IsNullOrEmpty(proxyServer)){
                    //    pm.DeadResp(proxyServer);}
                    return false;
                }
            }
            #endregion

            //			RedirectLocation = "";


            if (_myHttpWebResponse == null)
            {
                responseData = "NO Responce from server";
                return false;
            }

            if (waitResponse)
            {
                bool compessed = false;
                string atata = "";
                //				foreach( Cookie cc in _myHttpWebResponse.Cookies)
                //				{
                //					atata += cc.Name +"="+cc.Value+";";
                //				}

                responceHeaders = new opis();

                #region WebResponse Headers: Cookie, gzip Encoding

                respCookies = "";
                for (int i = 0; i < _myHttpWebResponse.Headers.Count; ++i)
                {
                    responceHeaders.Vset(_myHttpWebResponse.Headers.Keys[i], _myHttpWebResponse.Headers[i]);

                    if (_myHttpWebResponse.Headers.Keys[i].Contains("Set-Cookie"))
                    {
                        respCookies += _myHttpWebResponse.Headers[i];

                        //						cociDoc.SetCookies(urlUri ,  respCookies);
                    }

                    if (_myHttpWebResponse.Headers.Keys[i].Contains("Location"))
                    {
                        RedirectLocation = _myHttpWebResponse.Headers[i];
                    }

                    if (_myHttpWebResponse.Headers.Keys[i] == "Date")
                    {
                        DateTime dd = new DateTime(0);
                        try
                        {
                            dd = Convert.ToDateTime(_myHttpWebResponse.Headers[i]);
                        }
                        catch (Exception e)
                        { }
                        dat = dd.Day.ToString() + ((dd.Month < 10) ? "0" : "") + dd.Month.ToString() + dd.Year.ToString().Replace("20", "");
                    }

                    //					MessageBox.Show(dat);

                    if (_myHttpWebResponse.Headers.Keys[i] == "Content-Encoding")
                    {
                        compressType = _myHttpWebResponse.Headers[i];
                        if ((_myHttpWebResponse.Headers[i] == "gzip") || (_myHttpWebResponse.Headers[i] == "deflate"))
                        {
                            compessed = true;
                        }
                    }

                    if (_myHttpWebResponse.Headers.Keys[i] == "Content-Type")
                    {
                        //Content - Type: text / html; charset = windows - 1251
                        contentType = _myHttpWebResponse.Headers[i];
                        if (contentType.Contains("charset=windows-1251"))
                        {
                            EncoderOfResponce = Encoding.GetEncoding("windows-1251");

                        }
                    }

                   
                }
                #endregion

                State = 4;
                _ResponseStream = _myHttpWebResponse.GetResponseStream();
                State = 5;

                if (compessed)
                {
                    SetDecompressRespStream();
                }

                responseData = ResponseString;
            }
            _myHttpWebResponse.Close();




            #region  Proxy

            //This URLs or Page has been blocked.
            if (responseData.Contains("This URLs or Page has been blocked"))
            {
                //if(!string.IsNullOrEmpty(proxyServer)){
                //    pm.DeadResp(proxyServer);}
                return false;
            }

            //if (!string.IsNullOrEmpty(proxyServer))
            //{
            //    pm.StopPing(_proxyServer, dataToPost);
            //}
            #endregion


            bool rez = false;
            if (_myHttpWebResponse.StatusCode == HttpStatusCode.Redirect || _myHttpWebResponse.StatusCode == HttpStatusCode.OK)
            {
                rez = true;
            }
           
            if(string.IsNullOrEmpty(responseData) && _myHttpWebResponse.StatusCode != HttpStatusCode.Redirect)
            { rez = false; }

            _myHttpWebResponse = null;
            myHttpWebRequest = null;

            State = 0;

            return rez;

        }

    }



}
