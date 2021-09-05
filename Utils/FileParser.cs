using System;
using System.IO;
using System.Threading.Tasks;

namespace FreediverApp.Utils
{
    class FileParser
    {
        //private string folderPath = System.Environment.GetFolderPath();

        public FileParser() 
        {

        }

        public async Task<bool> parseFile(string filePath, string fileName) 
        {
            var absoluteFilePath = Path.Combine(filePath, fileName);

            if (absoluteFilePath == null || !File.Exists(absoluteFilePath))
            {
                Console.WriteLine("Error reading file: " + absoluteFilePath);
                return false;
            }

            using (var reader = new StreamReader(absoluteFilePath, true))
            {
                string line;
                while ((line = await reader.ReadLineAsync()) != null)
                {
                    //PARSING LOGIC FOR EACH LINE
                }
            }

            return true;
        }

        public bool parseDirectory(string directoryPath) 
        {
            return false;
        }
    }
}