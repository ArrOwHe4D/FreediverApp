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
        private string id;
        public string Id
        {
            get { return id; }
            set { id = value; }
        }
        
        public List<Dive> dives = new List<Dive>();

        public DiveSession(string _refUser) 
        {
            refUser = _refUser;
            id = refUser + "_" + System.Guid.NewGuid();
        }

        public DiveSession()
        {            
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