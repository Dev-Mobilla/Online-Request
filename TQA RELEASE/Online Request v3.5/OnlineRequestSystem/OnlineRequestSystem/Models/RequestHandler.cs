using System;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Threading;

namespace OnlineRequestSystem.Models
{
    internal class RequestHandler
    {
        private string _Method = string.Empty;
        private Uri _Url = null;
        private string _ContentType = string.Empty;
        private byte[] _jsonData = null;
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(RequestHandler));

        // Post Request Handler
        public RequestHandler(string method, Uri uri, string ContentType, byte[] jsonData)
        {
            this._Method = method;
            this._Url = uri;
            this._ContentType = ContentType;
            this._jsonData = jsonData;
        }

        public RequestHandler(Uri url, string Method, string ContentType)
        {
            _Url = url;
            _Method = Method;
            _ContentType = ContentType;
        }

        public virtual string HttpPostRequest()
        {
            try
            {
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(delegate { return true; });
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(_Url) as HttpWebRequest;

                request.Credentials = request.Credentials = CredentialCache.DefaultCredentials;
                request.Method = _Method;
                request.ContentType = _ContentType;
                request.ContentLength = _jsonData.Length;
                request.Timeout = Timeout.Infinite;
                using (Stream stream = request.GetRequestStream())
                {
                    stream.Write(_jsonData, 0, _jsonData.Length);
                    stream.Close();
                }
                WebResponse webresponse = request.GetResponse();
                String res = null;
                using (Stream response = webresponse.GetResponseStream())
                {
                    if (webresponse != null)
                    {
                        using (StreamReader reader = new StreamReader(response))
                        {
                            res = reader.ReadToEnd();
                            reader.Close();
                            webresponse.Close();
                        }
                    }
                }
                return res;
            }
            catch (Exception ex)
            {
                log.Fatal(ex.Message, ex);
                return "Error : " + ex.Message;
            }
        }

        public virtual string HttpGetRequest()
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(_Url) as HttpWebRequest;
                request.Method = _Method;
                request.ContentType = _ContentType;
                request.Credentials = CredentialCache.DefaultCredentials;
                request.Timeout = Timeout.Infinite;
                WebResponse webresponse = request.GetResponse();
                String res = null;
                using (Stream response = webresponse.GetResponseStream())
                {
                    if (webresponse != null)
                    {
                        using (StreamReader reader = new StreamReader(response))
                        {
                            res = reader.ReadToEnd();
                            reader.Close();
                            webresponse.Close();
                        }
                    }
                }
                return res;
            }
            catch (Exception ex)
            {
                log.Fatal(ex.Message, ex);
                return "Error : " + ex.Message;
            }
        }
    }
}