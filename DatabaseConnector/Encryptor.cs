using System;
using System.IO;
using System.Security.Cryptography;

namespace FreediverApp.DatabaseConnector
{
    public class Encryptor
    {
        private static byte[] key = { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16 };

        public static string Encrypt(string text) 
        {
            byte[] data = System.Text.Encoding.ASCII.GetBytes(text);
            data = new System.Security.Cryptography.SHA256Managed().ComputeHash(data);   
            string hash = System.Text.Encoding.ASCII.GetString(data);

            return hash;
        }
    }
}