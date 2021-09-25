using System;
using System.Text;

namespace FreediverApp.DatabaseConnector
{
    /**
     *  This is a helper class that holds different functions for security purposes.
     *  For now there is only one function that hashes a inpuit string with SHA256, to save passwords
     *  to db for example. This class should be extended in future versions with salting or 
     *  other algorithms and security functions if needed.
     **/
    public class CryptoService
    {
        static string SALT = "SU2j6Kmf93DW85fg98mWD3";

        public static string Encrypt(string text)
        {
            byte[] data = Encoding.ASCII.GetBytes(text);
            data = new System.Security.Cryptography.SHA256Managed().ComputeHash(data);
            string hash = Encoding.ASCII.GetString(data);
            return hash;
        }

        public static string SaltClearText(string clearText)
        {
            return clearText += SALT;
        }

        public static string GeneratePassword(int characterCount, int asciiStart, int asciiEnd)
        {
            Random random = new Random(DateTime.Now.Millisecond);
            StringBuilder passwordBuilder = new StringBuilder();

            for (int i = 0; i < characterCount; i++)
            {
                passwordBuilder.Append((char)(random.Next(asciiStart, asciiEnd + 1) % 255));
            }
            return passwordBuilder.ToString();
        }
    }             
}