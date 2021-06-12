using System;
using System.IO;
using System.Net;

namespace FreediverApp.WifiCommunication
{
    class FtpConnector
    {
        public FtpConnector() 
        {

        }

        public void downloadFile(string url, string username, string password, string filename)
        {
            FtpWebRequest ftpRequest = (FtpWebRequest) WebRequest.Create(url + "/" + filename);
            ftpRequest.AuthenticationLevel = System.Net.Security.AuthenticationLevel.None;
            ftpRequest.Credentials = new NetworkCredential(username, password);
            ftpRequest.Method = WebRequestMethods.Ftp.DownloadFile;

            using (Stream streamWriter = ftpRequest.GetResponse().GetResponseStream()) 
            {
                StreamReader streamReader = new StreamReader(streamWriter);
                var output = streamReader.ReadToEnd();
                Console.WriteLine(output);
            }
        }
    }
}