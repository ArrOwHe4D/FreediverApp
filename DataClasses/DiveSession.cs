using System;
using System.Collections.Generic;

namespace FreediverApp
{
    /**
     *  This dataclass represents a Divesession that consists of different metadata like the date, location and
     *  weather conditions. It also contains a list of dives that belong to this divesession.
     **/
    public class DiveSession
    {
        public string date;
        public string location_lon;
        public string location_lat;
        public string location_locality;
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
        public string note;
        public string key;
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

        /**
         *  This function calculates the maximum time the diver was inside the water
         *  during this divesession.
         **/
        public void UpdateDuration()
        {
            int dur = 0;

            foreach (var item in dives)
            {
                try
                {
                    dur += Convert.ToInt32(item.GetTotalTime());
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    Console.WriteLine("Failed to convert dive duration!");
                }
            }
            watertime = dur.ToString();
        }

        public void UpdateAll()
        {
            foreach (Dive d in dives)
            {
                d.UpdateAll();
            }
            UpdateDuration();
        }
    }
}