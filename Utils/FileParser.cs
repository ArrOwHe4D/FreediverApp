using FreediverApp.DatabaseConnector;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace FreediverApp.Utils
{
    static class FileParser
    {
        public async Task<DiveSession> createDiveSessionFromFile(object session)
        {
            string directoryPath = "/storage/emulated/0/FreediverApp";
            Directory.CreateDirectory(directoryPath);

            string directorySessionPath = directoryPath + "/" + session.Key + "/";
            DiveSession diveSession = new DiveSession(TemporaryData.CURRENT_USER.id);
            diveSession.date = session.Key.Replace("_", ".");
            diveSession.date.Split(".")[2] = "20" + diveSession.date.Split(".")[2]; //restore full year string (2021 instead of 21)

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

                using (var reader = new StreamReader(directorySessionDivePath, true))
                {
                    string time;
                    time = await reader.ReadLineAsync();
                    newDive.timestampBegin = time;
                }

                List<Measurepoint> measurepoints = await parseFile(newDive.id, directorySessionDivePath);
                newDive.measurepoints = new List<Measurepoint>(measurepoints);
                diveSession.dives.Add(newDive);

            }
            diveSession.UpdateAll();

            return diveSession;
        }

        public async Task<List<Measurepoint>> parseFile(string diveID, string filePath) 
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

            //First line has to be removed. It contains the timestamp.
            measurepointJsonList.RemoveAt(0);

            List<Measurepoint> measurepoints = new List<Measurepoint>();

            foreach(var measurepoint in measurepointJsonList)
            {
                Measurepoint mp = Measurepoint.fromJson(measurepoint);

                //We have to reference the dive to which this measurepoint belongs.
                mp.ref_dive = diveID;
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