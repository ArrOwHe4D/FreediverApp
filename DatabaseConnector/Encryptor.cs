namespace FreediverApp.DatabaseConnector
{
    /**
     *  This is a helper class that holds different functions for security purposes.
     *  For now there is only one function that hashes a inpuit string with SHA256, to save passwords
     *  to db for example. This class should be extended in future versions with salting or 
     *  other algorithms and security functions if needed.
     **/
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