using FreediverApp.DatabaseConnector;
using FreediverApp.DataClasses;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace FreediverApp.Utils
{
    class FileParser
    {
        private DownloadReport downloadReport;


        public FileParser(DownloadReport downloadReport) 
        {
            this.downloadReport = downloadReport;
        }

        public async Task<List<DiveSession>> iterateThroughFiles()
        {
            List<DiveSession> diveSessions = new List<DiveSession>();
            foreach(var session in downloadReport.getFilesToDownload())
            {
                string directorySessionPath = downloadReport.getDirectoryPath() + "/" + session.Key + "/";
                DiveSession diveSession = new DiveSession(TemporaryData.CURRENT_USER.id);

                foreach(var dive in session.Value)
                {
                    var directorySessionDivePath = Path.Combine(directorySessionPath, dive);
                    string diveId = dive.Substring(2).Split(".")[0];
                    Dive newDive = new Dive(diveSession.Id, diveId);

                    if (directorySessionDivePath == null || !File.Exists(directorySessionDivePath))
                    {
                        Console.WriteLine("Error reading file: " + directorySessionDivePath);
                        continue;
                    }
                    List<Measurepoint> measurepoints = await parseFile(directorySessionDivePath);
                    newDive.measurepoints = new List<Measurepoint>(measurepoints);
                    newDive.UpdateAll();
                    diveSession.dives.Add(newDive);

                }
                diveSessions.Add(diveSession);
            }
            return diveSessions;
        }



        public async Task<List<Measurepoint>> parseFile(string filePath) 
        {
            if (filePath == null || !File.Exists(filePath))
            {
                Console.WriteLine("Error reading file: " + filePath);
                return null;
            }

            List<string> measurepointJsonList = new List<string>();

            using (var reader = new StreamReader(filePath, true))
            {
                string line;
                while ((line = await reader.ReadLineAsync()) != null)
                {
                    measurepointJsonList.Add(line);
                }
            }

            List<Measurepoint> measurepoints = new List<Measurepoint>();

            foreach(var measurepoint in measurepointJsonList)
            {
                Measurepoint mp = Measurepoint.fromJson(measurepoint);
                measurepoints.Add(mp);
            }

            return measurepoints;
        }

        public bool parseDirectory(string directoryPath) 
        {
            return false;
        }
    }
}