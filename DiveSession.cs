using System;
using System.Collections.Generic;

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
        
        public List<Dive> dives = new List<Dive>();

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