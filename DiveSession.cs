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
        public string HeartFreqMax
        {
            get{
                try
                {
                    string heartFreqMax = dives.First().GetHeartFreqMax();
                    foreach (var item in dives)
                    {
                        if (Convert.ToInt32(item.GetHeartFreqMax()) > Convert.ToInt32(heartFreqMax))
                        {
                            heartFreqMax = item.GetHeartFreqMax();
                        }
                    }
                    return heartFreqMax;
                }
                catch (Exception)
                {
                    return "error";
                }                
            }
        }        
        public string HeartFreqMin
        {
            get
            {
                try
                {
                    string heartFreqMin = dives.First().GetHeartFreqMin();
                    foreach (var item in dives)
                    {
                        if (Convert.ToInt32(item.GetHeartFreqMin()) > Convert.ToInt32(heartFreqMin))
                        {
                            heartFreqMin = item.GetHeartFreqMin();
                        }
                    }
                    return heartFreqMin;
                }
                catch (Exception)
                {
                    return "error";
                }                
            }
        }        
        public string LuminanceMax
        {
            get
            {
                try
                {
                    string luminanceMax = dives.First().GetLuminanceMax();
                    foreach (var item in dives)
                    {
                        if (Convert.ToInt32(item.GetLuminanceMax()) > Convert.ToInt32(luminanceMax))
                        {
                            luminanceMax = item.GetLuminanceMax();
                        }
                    }
                    return luminanceMax;
                }
                catch (Exception)
                {
                    return "error";
                }                
            }
        }       
        public string LuminanceMin
        {
            get
            {
                try
                {
                    string luminanceMin = dives.First().GetLuminanceMin();
                    foreach (var item in dives)
                    {
                        if (Convert.ToInt32(item.GetLuminanceMin()) > Convert.ToInt32(luminanceMin))
                        {
                            luminanceMin = item.GetLuminanceMin();
                        }
                    }
                    return luminanceMin;
                }
                catch (Exception)
                {
                    return "error";
                }                
            }
        }
        public string maxDepth;        
        public string OxygenSaturationMax
        {
            get
            {
                try
                {
                    string oxygenSaturationMax = dives.First().GetOxygenSaturationMax();
                    foreach (var item in dives)
                    {
                        if (Convert.ToInt32(item.GetOxygenSaturationMax()) > Convert.ToInt32(oxygenSaturationMax))
                        {
                            oxygenSaturationMax = item.GetOxygenSaturationMax();
                        }
                    }
                    return oxygenSaturationMax;
                }
                catch (Exception)
                {
                    return "error";
                }                
            }
        }        
        public string OxygenSaturationMin
        {
            get
            {
                try
                {
                    string oxygenSaturationMin = dives.First().GetOxygenSaturationMin();
                    foreach (var item in dives)
                    {
                        if (Convert.ToInt32(item.GetOxygenSaturationMin()) > Convert.ToInt32(oxygenSaturationMin))
                        {
                            oxygenSaturationMin = item.GetOxygenSaturationMin();
                        }
                    }
                    return oxygenSaturationMin;
                }
                catch (Exception)
                {
                    return "error";
                }                
            }
        }
        public string refUser;
        public string timestampBegin;
        public string timestampEnd;        
        public string WaterTemperatureMax
        {
            get
            {
                try
                {
                    string waterTemperatureMax = dives.First().GetWaterTemperatureMax();
                    foreach (var item in dives)
                    {
                        if (Convert.ToInt32(item.GetWaterTemperatureMax()) > Convert.ToInt32(waterTemperatureMax))
                        {
                            waterTemperatureMax = item.GetWaterTemperatureMax();
                        }
                    }
                    return waterTemperatureMax;
                }
                catch (Exception)
                {
                    return "error";
                }                
            }
        }        
        public string WaterTemperatureMin
        {
            get
            {
                try
                {
                    string waterTemperatureMin = dives.First().GetWaterTemperatureMin();
                    foreach (var item in dives)
                    {
                        if (Convert.ToInt32(item.GetWaterTemperatureMin()) > Convert.ToInt32(waterTemperatureMin))
                        {
                            waterTemperatureMin = item.GetWaterTemperatureMin();
                        }
                    }
                    return waterTemperatureMin;
                }
                catch (Exception)
                {
                    return "error";
                }                
            }
        }
        public List<Dive> dives = new List<Dive>();
        //needed?
        string temperature;
        string weather;
        int timeInWater;
        //List<Dive> dives = new List<Dive>();

        //static DiveSession cur;

        public DiveSession() { }

        public DiveSession(string _date)
        {
            date = _date;
        }
    }
}