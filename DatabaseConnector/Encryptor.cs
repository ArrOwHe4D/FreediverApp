namespace FreediverApp.DatabaseConnector
{
    public class Encryptor
    {
        public static string Encrypt(string text) 
        {
            byte[] data = System.Text.Encoding.ASCII.GetBytes(text);
            data = new System.Security.Cryptography.SHA256Managed().ComputeHash(data);   
            string hash = System.Text.Encoding.ASCII.GetString(data);
            return hash;
        }
    }
}