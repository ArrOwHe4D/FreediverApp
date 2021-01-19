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
        public string location;
        public string date;
        public string duration;
        
        public List<Dive> dives = new List<Dive>();
        //needed?
        string temperature;
        string weather;
        int timeInWater;
        //List<Dive> dives = new List<Dive>();

        //static DiveSession cur;
        public string refUser;
        public DiveSession() { }

        public DiveSession(string _date)
        {
            date = _date;
        }

        public void UpdateDuration()
        {
            float dur = 0;

            foreach (var item in dives)
            {
                try
                {
                    dur += (float)Convert.ToDouble(item.GetTotalTime());
                }
                catch (Exception)
                {

                }
            }
        }
    }
}