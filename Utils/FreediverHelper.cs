using System;
using System.Globalization;

namespace FreediverApp.Utils
{
    class FreediverHelper
    {
        public static bool validateEmail(string email)
        {
            try
            {
                var emailAdress = new System.Net.Mail.MailAddress(email);
                return emailAdress.Address == email;
            }
            catch
            {
                return false;
            }
        }

        public static bool validateBirthdate(string birthday)
        {
            try
            {
                birthday = birthday.Replace('.', Convert.ToChar(@"-"));
                string pattern = "dd-MM-yyyy";

                DateTime bday;
                DateTime.TryParseExact(birthday, pattern, null, DateTimeStyles.None, out bday);

                return bday.Date <= DateTime.Now.Date;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
    }
}