using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;

namespace ConsoleGrabit
{
    public class WebPostRequest
    {
        private WebRequest _request;
        private HttpWebResponse _response;
        private Collection<string> _querydata;

        public WebPostRequest(string url)
        {
            _request = WebRequest.Create(url);
            _request.Method = "POST";
            _querydata = new Collection<string>();
        }

        public void Add(string key, string value)
        {
            _querydata.Add(String.Format("{0}={1}",key,HttpUtility.UrlEncode(value)));
        }

        public string GetResponse()
        {
            _request.ContentType = "application/x-www-form-urlencoded";
            var parameters = String.Join("&",  _querydata.ToArray());
            _request.ContentLength = parameters.Length;

            var sw = new StreamWriter(_request.GetRequestStream());
            sw.Write(parameters);
            sw.Close();

            _response = (HttpWebResponse) _request.GetResponse();
            var sr = new StreamReader(_response.GetResponseStream());
            return sr.ReadToEnd();

        }

    }
}
