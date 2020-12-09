using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace FreediverApp
{
    class User
    {
        string username;
        public List<DiveSession> diveSessions = new List<DiveSession>();

        public User(List<DiveSession> _diveSessions)
        {
            diveSessions = _diveSessions;
        }
        
        public static User curUser = new User(new List<DiveSession>() {new DiveSession("22.11.2020"), new DiveSession("23.11.2020"), new DiveSession("25.11.2020"), });
    }
}