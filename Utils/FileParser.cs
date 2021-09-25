using FreediverApp.DatabaseConnector;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FreediverApp.Utils
{
    static class FileParser
    {
        public static DiveSession parseSession(KeyValuePair<string, List<string>> session)
        {
            string directoryPath = "/storage/emulated/0/FreediverApp";
            Directory.CreateDirectory(directoryPath);

            string directorySessionPath = directoryPath + "/" + session.Key + "/";
            DiveSession diveSession = new DiveSession(TemporaryData.CURRENT_USER.id);
            diveSession.date = session.Key.Replace("_", ".");

            string tempDateWithoutYear = diveSession.date.Substring(0, 6);
            string fullYear = "20" + diveSession.date.Split(".")[2]; //restore full year string (2021 instead of 21)

            diveSession.date = tempDateWithoutYear + fullYear;

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

                //read the first line of a dive file to set the timestamp when the dive was started
                newDive.timestampBegin = File.ReadLines(directorySessionDivePath).First();

                List<Measurepoint> measurepoints = parseFile(newDive.id, directorySessionDivePath);
                newDive.measurepoints = new List<Measurepoint>(measurepoints);
                diveSession.dives.Add(newDive);
            }
            diveSession.UpdateAll();

            return diveSession;
        }

        public static List<Measurepoint> parseFile(string diveID, string filePath)
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
                while ((line = reader.ReadLine()) != null)
                {
                    measurepointJsonList.Add(line);
                }
            }

            //First line has to be removed. It contains the timestamp.
            measurepointJsonList.RemoveAt(0);

            List<Measurepoint> measurepoints = new List<Measurepoint>();

            foreach (var measurepoint in measurepointJsonList)
            {
                Measurepoint mp = Measurepoint.fromJson(measurepoint);

                //We have to reference the dive to which this measurepoint belongs.
                mp.ref_dive = diveID;
                measurepoints.Add(mp);
            }

            return measurepoints;
        }

        public static bool parseDirectory(string directoryPath)
        {
            return false;
        }
    }
}