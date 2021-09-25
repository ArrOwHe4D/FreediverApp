using Android.Net;
using FluentFTP;
using System;
using Android.Content;
using System.Collections.Generic;
using System.IO;
using FreediverApp.DataClasses;
using System.Linq;
using Android.Widget;

namespace FreediverApp.WifiCommunication
{
    class FtpConnector
    {
        private Context context;
        private FtpProfile serverProfile;
        private FtpClient client;
        private string directoryPath;
        private List<string> filenames = new List<string>();
        private string filepath;
        private string sessionsPathOnEsp = "/sessions.log";

        public FtpConnector(Context context)
        {
            this.context = context;
            client = new FtpClient();
        }

        public FtpConnector(Context context, string address_v4, string username, string password) 
        {
            this.context = context;
            buildFilePath();
            client = new FtpClient(address_v4, username, password);
            serverProfile = client.AutoConnect();
            filepath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        }

        public DownloadReport synchronizeData()
        {
            DownloadReport downloadReport = new DownloadReport();
            downloadReport.setDirectoryPath(directoryPath);
            try
            {
                if (File.Exists(filepath + sessionsPathOnEsp))
                {
                    File.Delete(filepath + sessionsPathOnEsp);
                }

                downloadFile(filepath, sessionsPathOnEsp);

                List<string> sessions = new List<string>();
                List<string> results = new List<string>();
                using (StreamReader sr = new StreamReader(File.Open(filepath + sessionsPathOnEsp, FileMode.Open)))
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

                    bool isSuccess = updateSessionFileOnEsp(filepath, sessionsPathOnEsp);

                    if (!isSuccess)
                    {
                        Toast.MakeText(context, Resource.String.no_dives_available, ToastLength.Long).Show();
                        return;
                    }

                    downloadReport.addSession(new KeyValuePair<string, List<string>>(session, new List<string>(filenames)));
                    filenames.Clear();
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
                string fullFilepath = directoryPath + remoteDirectory;

                List<FtpResult> results = client.DownloadDirectory(@fullFilepath, "/logFiles" + remoteDirectory, FtpFolderSyncMode.Update);

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

        private bool updateSessionFileOnEsp(string filePath, string filename)
        {
            string fullFilePath = filePath + filename;
            FtpStatus ftpStatus = FtpStatus.Failed;
            int count = 0;
            while(ftpStatus != FtpStatus.Success)
            {
                ftpStatus = client.DownloadFile(@fullFilePath, filename);
                if(count == 10)
                {
                    return false;
                }
                count++;
            }
            List<string> sessions = File.ReadLines(fullFilePath).ToList();
            sessions.RemoveAt(sessions.Count - 1);
            File.WriteAllLines(fullFilePath, sessions);

            client.DeleteFile(filename);

            ftpStatus = FtpStatus.Failed;
            count = 0;
            while (ftpStatus != FtpStatus.Success)
            {
                ftpStatus = client.UploadFile(fullFilePath, filename);
                if(count == 10)
                {
                    return false;
                }
                count++;
            }
            return true;
        }

        private void buildFilePath()
        {
            directoryPath = "/storage/emulated/0/FreediverApp";
            Directory.CreateDirectory(directoryPath);
        }  
    }
}
