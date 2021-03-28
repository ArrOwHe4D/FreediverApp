using System;
using System.Collections.Generic;

namespace FreediverApp
{
    public class DiveSession
    {
        public string date;
        public string location_lon;
        public string location_lat;
        public string refUser;
        public string watertime;
        public string weatherCondition_main;
        public string weatherCondition_description;
        public string weatherTemperature;
        public string weatherTemperatureFeelsLike;
        public string weatherPressure;
        public string weatherHumidity;
        public string weatherWindSpeed;
        public string weatherWindGust;
        public string duration;
        private string id;
        public List<Dive> dives = new List<Dive>();

        public string Id
        {
            get { return id; }
            set { id = value; }
        }

        public DiveSession()
        {

        }

        public DiveSession(string _refUser) 
        {
            refUser = _refUser;
            id = refUser + "_" + System.Guid.NewGuid();
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

        public void UpdateAll()
        {
            foreach(Dive d in dives)
            {
                d.UpdateAll();
            }
            UpdateDuration();
        }
    }
}