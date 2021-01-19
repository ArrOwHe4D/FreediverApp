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
        public string date;
        public string location;
        public string refUser;
        public string watertime;
        public string weatherCondition;
        public string weatherTemperature;
        public string weatherWindSpeed;
        public string duration;
        private string id;
        public string Id
        {
            get { return id; }
        }
        
        public List<Dive> dives = new List<Dive>();

        public DiveSession() 
        {
            id = refUser + "_" + System.Guid.NewGuid();
        }

        public DiveSession(string _id)
        {
            id = _id;
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

            watertime = dur.ToString();
        }
    }
}