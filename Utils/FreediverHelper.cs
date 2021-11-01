using System;
using System.Globalization;
using System.Net.NetworkInformation;

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

        public static bool darkModeActive(Android.Content.Context context)
        {
            return context.Resources.Configuration.UiMode == (Android.Content.Res.UiMode.NightYes | Android.Content.Res.UiMode.TypeNormal);
        }

        public static bool isConnectedToDatabase()
        {
            int timeout = 1000;
            Ping ping = new Ping();
            PingReply reply;
            try
            {
                reply = ping.Send("8.8.8.8", timeout);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }

            if (reply != null && reply.Status == IPStatus.Success)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}