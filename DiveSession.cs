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
    class DiveSession
    {
        string location;
        public string date;
        string temperature;
        string weather;
        int timeInWater;
        //List<Dive> dives = new List<Dive>();

        //static DiveSession cur;

        public DiveSession(string _date)
        {
            date = _date;
        }
    }
}