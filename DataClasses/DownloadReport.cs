using System.Collections.Generic;
using System.Linq;

namespace FreediverApp.DataClasses
{
    class DownloadReport
    {
        private string directoryPath;
        private List<KeyValuePair<string, List<string>>> sessions;

        public DownloadReport()
        {
            sessions = new List<KeyValuePair<string, List<string>>>();
        }

        public void setDirectoryPath(string directoryPath)
        {
            this.directoryPath = directoryPath;
        }

        public string getDirectoryPath()
        {
            return directoryPath;
        }

        public void addSession(KeyValuePair<string, List<string>> session)
        {
            sessions.Add(session);
        }

        public List<KeyValuePair<string, List<string>>> getFilesToDownload()
        {
            return sessions.ToList();
        }

        public DownloadReport fromJsonObject(object jsonObject) 
        {
            return new DownloadReport();
        }
    }
}