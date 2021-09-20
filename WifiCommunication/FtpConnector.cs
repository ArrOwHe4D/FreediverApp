using Android.Net;
using FluentFTP;
using System;
using Android.Content;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using FreediverApp.DataClasses;

namespace FreediverApp.WifiCommunication
{
    class FtpConnector
    {
        private Context context;
        private string address_v4;
        private string username;
        private string password;
        private FtpProfile serverProfile;
        private FtpClient client;
        private string directoryPath;
        private List<string> filenames = new List<string>();

        public FtpConnector(Context context)
        {
            this.context = context;
            client = new FtpClient();
        }

        public FtpConnector(Context context, string address_v4, string username, string password) 
        {
            this.context = context;
            this.address_v4 = address_v4;
            this.username = username;
            this.password = password;
            buildFilePath();
            client = new FtpClient(address_v4, username, password);
            serverProfile = client.AutoConnect();
        }

        public DownloadReport synchronizeData()
        {
            DownloadReport downloadReport = new DownloadReport();
            downloadReport.setDirectoryPath(directoryPath);
            try
            {
                string filepath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

                if (File.Exists(filepath + "/sessions.log"))
                {
                    File.Delete(filepath + "/sessions.log");
                }

                downloadFile(filepath, "/sessions.log");

                List<string> sessions = new List<string>();
                List<string> results = new List<string>();
                using (StreamReader sr = new StreamReader(File.Open(filepath + "/sessions.log", FileMode.Open)))
                {
                    while (!sr.EndOfStream)
                    {
                        sessions.Add(sr.ReadLine());
                    }
                }
                sessions.ForEach((string session) =>
                {
                    string absoluteDirectoryPath = directoryPath + "/" + session;
                    Directory.CreateDirectory(absoluteDirectoryPath);
                    downloadDirectory(directoryPath, "/" + session);
                    downloadReport.addSession(new KeyValuePair<string, List<string>>(session, new List<string>(filenames)));
                    filenames.Clear();

                    //using (StreamReader sr = new StreamReader(File.Open(filepath + "/" + session, FileMode.Open)))
                    //{
                    //    while (!sr.EndOfStream)
                    //    {
                    //        results.Add(sr.ReadLine());
                    //    }
                    //}
                });
            } 
            catch(Exception e)
            {
                Console.WriteLine(e);
            }
            return downloadReport;
        }

        public void downloadFile(string filepath, string filename)
        {
            try
            {
                string fullFilepath = filepath + filename;

                FtpStatus successful = client.DownloadFile(@fullFilepath, filename);



                var res = File.ReadAllText(@fullFilepath);


                if (successful == FtpStatus.Success)
                    Console.WriteLine("------ SUCCESS -------");
                else
                    Console.WriteLine("------ ERROR NO SUCCESS -------");

                string content;
                using (StreamReader sr = new StreamReader(File.Open(fullFilepath, FileMode.Open)))
                {
                    content = sr.ReadToEnd();
                }
                Console.WriteLine("------------ SESSION.LOG-----------");
                Console.WriteLine(content);
                Console.WriteLine("------------ SESSION.LOG-----------");
            }
            catch (Exception e)
            {
                Console.WriteLine("------------------ ERROR START --------------------");
                Console.WriteLine(e);
                Console.WriteLine("------------------ ERROR END --------------------");
            }
        }

        public void downloadDirectory(string directoryPath, string remoteDirectory)
        {
            try
            {
                //var fullFilepath = Path.Combine(filepath, filename);
                string fullFilepath = directoryPath + remoteDirectory;

                List<FtpResult> results = client.DownloadDirectory(@fullFilepath, "/logFiles" + remoteDirectory, FtpFolderSyncMode.Update);



                //var res = File.ReadAllText(@fullFilepath);



                if (results.Count > 0)
                    Console.WriteLine("------ SUCCESS -------");
                else
                    Console.WriteLine("------ ERROR NO SUCCESS -------");

                foreach(FtpResult result in results)
                {
                    filenames.Add(result.Name);
                }

                string content;

                foreach(string name in filenames)
                {
                    using (StreamReader sr = new StreamReader(File.Open(fullFilepath + "/" + name, FileMode.Open)))
                    {
                        content = sr.ReadToEnd();
                        Console.WriteLine(content);
                    }
                }
                
            }
            catch (Exception e)
            {
                Console.WriteLine("------------------ ERROR START --------------------");
                Console.WriteLine(e);
                Console.WriteLine("------------------ ERROR END --------------------");
            }
        }

        

        public async void disconnect() 
        {
            await client.DisconnectAsync();
        }

        public bool isConnected() 
        {
            try
            {
                ConnectivityManager connectivityManager = (ConnectivityManager)context.GetSystemService(Context.ConnectivityService);
                return connectivityManager.ActiveNetworkInfo.IsConnectedOrConnecting;
            }
            catch (Exception ex) 
            {
                Console.WriteLine(ex.Message);
                return false;
            }     
        }

        private void handleError(FtpStatus errorcode) 
        {
            switch (errorcode) 
            {
                case FtpStatus.Success: 
                {
                    Console.WriteLine("Ftp Operation successful!");
                    break;
                }
                case FtpStatus.Failed: 
                case FtpStatus.Skipped: 
                {
                    Console.WriteLine("Error while processing requested Ftp Operation!");
                    break;
                }
                default: { break; }
            }
        }

        //public async Task<bool> synchronizeData_old()
        //{
        //    FtpClient client = new FtpClient("192.168.4.1", "diver", "diverpass");
        //    client.AutoConnect();

        //    bool success = false;

        //    if (File.Exists(localDirectory + "/sessions.log"))
        //    {
        //        File.Delete(localDirectory + "/sessions.log");
        //    }

        //    success = await downloadFile(localDirectory, "/", "sessions.log");

        //    List<string> sessions = new List<string>();
        //    List<string> results = new List<string>();
        //    using (StreamReader sr = new StreamReader(File.Open(localDirectory + "/sessions.log", FileMode.Open)))
        //    {
        //        while (!sr.EndOfStream)
        //        {
        //            sessions.Add(sr.ReadLine());
        //        }
        //    }
        //    sessions.ForEach((Action<string>)(async (string session) =>
        //    {
        //        List<FtpResult> result;
        //        result = await this.downloadDirectory((string)localDirectory, (string)("/logFiles/" + session));
        //        success = result.Count > 0;
        //        using (StreamReader sr = new StreamReader(File.Open(localDirectory + session, FileMode.Open)))
        //        {
        //            while (!sr.EndOfStream)
        //            {
        //                results.Add(sr.ReadLine());
        //            }
        //        }
        //    }));
        //    return success;
        //}

        private void buildFilePath()
        {
            directoryPath = "/storage/emulated/0/FreediverApp";
            Directory.CreateDirectory(directoryPath);
        }


        





        
    }
}
