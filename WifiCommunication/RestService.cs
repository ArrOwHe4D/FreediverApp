﻿using System;
using System.Net;
using System.IO;

namespace FreediverApp.WifiCommunication
{
    class HttpConnection
    {
        public HttpConnection()
        {

        }

        public void httpRequest()
        {
            var rxcui = "198440";
            var request = HttpWebRequest.Create(string.Format(@"192.168.4.1", rxcui));
            request.ContentType = "text/plain";
            //request.ContentLength = 
            request.Method = "GET";

            using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
            {
                if (response.StatusCode != HttpStatusCode.OK)
                    Console.Out.WriteLine("Error fetching data. Server returned status code: {0}", response.StatusCode);
                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    var content = reader.ReadToEnd();
                    if (string.IsNullOrWhiteSpace(content))
                    {
                        Console.Out.WriteLine("Response contained empty body...");
                    }
                    else
                    {
                        Console.Out.WriteLine("Response Body: \r\n {0}", content);
                    }
                }
            }
        }
    }
}