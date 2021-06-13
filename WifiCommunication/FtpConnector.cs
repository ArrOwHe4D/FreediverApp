using Android.Net;
using Android.Net.Wifi;
using FluentFTP;
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Android.Content;

namespace FreediverApp.WifiCommunication
{
    class FtpConnector
    {
        Context context;

        public FtpConnector(Context context)
        {
            this.context = context;
        }

        

        //public void downloadFile()
        //{
        //    //FtpClient ftpClient = new FtpClient(url);

        //    //ftpClient.Credentials = new NetworkCredential(username, password);

        //    //ftpClient.Connect();

        //    //FtpStatus status = ftpClient.DownloadFile("/" + filename, "/" + filename);

        //    //Console.WriteLine("FTP STATUS: " + status);

        //    Uri uri = new Uri("ftp://192.168.4.1/divelog25.txt");
        //    FtpWebRequest ftpRequest = (FtpWebRequest)WebRequest.Create(uri);

        //    ftpRequest.AuthenticationLevel = System.Net.Security.AuthenticationLevel.None;
        //    ftpRequest.Timeout = System.Threading.Timeout.Infinite;
        //    ftpRequest.KeepAlive = false;
        //    ftpRequest.Credentials = new NetworkCredential("diver", "diverpass");
        //    ftpRequest.Method = WebRequestMethods.Ftp.DownloadFile;

        //    try
        //    {
        //        using (FtpWebResponse ftpResponse = (FtpWebResponse)ftpRequest.GetResponse()) //Stream streamWriter = ftpRequest.GetResponse().GetResponseStream()
        //        {
        //            Stream stream = ftpResponse.GetResponseStream();
        //            StreamReader streamReader = new StreamReader(stream);
        //            var output = streamReader.ReadToEnd();
        //            Console.Write("Stream output: ------>");
        //            Console.WriteLine(output);
        //        }
        //    }
        //    catch (WebException webexp)
        //    {
        //        Console.WriteLine("------------------------------------------------------");
        //        Console.WriteLine(webexp);
        //    }

        //}

        public async Task downloadFile_v2(string url, string username, string password, string filename)
        {


            //using (WebClient client = new WebClient())
            //{
            //    client.DownloadFile(url, filename);
            //}



            try
            {
                string fileUrl = String.Format("{0}/{1}", url, filename);
                FtpWebRequest req = (FtpWebRequest)FtpWebRequest.Create(fileUrl);
                req.Proxy = null;
                req.Method = WebRequestMethods.Ftp.DownloadFile;
                req.Credentials = new NetworkCredential(username, password);
                req.UseBinary = true;
                req.UsePassive = true;
                WebResponse res = await req.GetResponseAsync();
                FtpWebResponse webRes = (FtpWebResponse) res;
                using(StreamReader reader = new StreamReader(res.GetResponseStream()))
                {
                    string dataString = reader.ReadToEnd();
                    Console.WriteLine(dataString);
                    //return dataString;
                }
            } catch(Exception exp)
            {
                Console.WriteLine("------ ERROR ------ ERROR ------ ERROR ------");
                Console.WriteLine(exp);
                Console.WriteLine("------ ERROR ------ ERROR ------ ERROR ------");
                //return "ERROR";
            }
        }

        public async void downloadFile_v3()
        {
            try
            {
                ConnectivityManager connectivityManager = (ConnectivityManager)context.GetSystemService(Context.ConnectivityService);
                if (connectivityManager.ActiveNetworkInfo.IsConnectedOrConnecting)
                {
                    Console.WriteLine("------------- IS CONNECTED ---------------");
                }

                FtpClient client = new FtpClient("ftp://192.168.4.1");
                client.Credentials = new NetworkCredential("diver", "diverpass");
                await client.ConnectAsync();
                client.Rename("/divelog25.txt", "/divelog30.txt");
                client.Disconnect();
            } catch(Exception e)
            {
                Console.WriteLine("------------------ ERROR START --------------------");
                Console.WriteLine(e);
                Console.WriteLine("------------------ ERROR END --------------------");
            }
        }
    }
}
