using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FreediverApp.DataClasses
{
    public class SavedSession
    {
        public string ref_user;
        public string sessiondate;
        public SavedSession() { }

        public SavedSession(string _ref_user, string _sessiondate)
        {
            ref_user = _ref_user;
            sessiondate = _sessiondate;
        }

        
    }
}