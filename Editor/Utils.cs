using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using MoonSharp.Interpreter;
using System.ComponentModel;

namespace Luavit
{
    class Utils
    {
        public HTTPClient NewHTTPClient(string url)
        {
            return new HTTPClient((HttpWebRequest)WebRequest.Create(url));
        }
    }

    class HTTPClient
    {
        public HttpWebRequest request;

        public HTTPClient(HttpWebRequest hwr)
        {
            request = hwr;
        }

        public string Get()
        {
            WebResponse response = request.GetResponse();
            Stream dataStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(dataStream);
            string responseFromServer = reader.ReadToEnd();
            reader.Close();
            response.Close();

            return (responseFromServer);
        }
    }
}
