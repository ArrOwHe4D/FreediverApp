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

namespace FreediverApp
{
    class Dive
    {
        public List<Measurepoint> measurepoints = new List<Measurepoint>();

        public string GetHeartFreqMax()
        {
            try
            {
                int maxheartfreq = Convert.ToInt32(measurepoints.First().heartFreq);
                foreach (var item in measurepoints)
                {
                    if (Convert.ToInt32(item.heartFreq) > maxheartfreq)
                    {
                        maxheartfreq = Convert.ToInt32(item.heartFreq);
                    }
                }
                return maxheartfreq.ToString();
            }
            catch (Exception)
            {
                return "error";   
            }
        }

        public string GetHeartFreqMin()
        {
            try
            {
                int minheartfreq = Convert.ToInt32(measurepoints.First().heartFreq);
                foreach (var item in measurepoints)
                {
                    if (Convert.ToInt32(item.heartFreq) < minheartfreq)
                    {
                        minheartfreq = Convert.ToInt32(item.heartFreq);
                    }
                }
                return minheartfreq.ToString();
            }
            catch (Exception)
            {
                return "error";
            }
        }

        public string GetLuminanceMin()
        {
            try
            {
                int luminanceMin = Convert.ToInt32(measurepoints.First().luminance);
                foreach (var item in measurepoints)
                {
                    if (Convert.ToInt32(item.heartFreq) < luminanceMin)
                    {
                        luminanceMin = Convert.ToInt32(item.heartFreq);
                    }
                }
                return luminanceMin.ToString();
            }
            catch (Exception)
            {
                return "error";
            }
        }

        public string GetLuminanceMax()
        {
            try
            {
                int luminanceMax = Convert.ToInt32(measurepoints.First().luminance);
                foreach (var item in measurepoints)
                {
                    if (Convert.ToInt32(item.heartFreq) > luminanceMax)
                    {
                        luminanceMax = Convert.ToInt32(item.heartFreq);
                    }
                }
                return luminanceMax.ToString();
            }
            catch (Exception)
            {
                return "error";
            }
        }

        public string GetOxygenSaturationMax()
        {
            try
            {
                int oxygenSaturationMax = Convert.ToInt32(measurepoints.First().luminance);
                foreach (var item in measurepoints)
                {
                    if (Convert.ToInt32(item.heartFreq) > oxygenSaturationMax)
                    {
                        oxygenSaturationMax = Convert.ToInt32(item.heartFreq);
                    }
                }
                return oxygenSaturationMax.ToString();
            }
            catch (Exception)
            {
                return "error";
            }
        }

        public string GetOxygenSaturationMin()
        {
            try
            {
                int oxygenSaturationMin = Convert.ToInt32(measurepoints.First().luminance);
                foreach (var item in measurepoints)
                {
                    if (Convert.ToInt32(item.heartFreq) < oxygenSaturationMin)
                    {
                        oxygenSaturationMin = Convert.ToInt32(item.heartFreq);
                    }
                }
                return oxygenSaturationMin.ToString();
            }
            catch (Exception)
            {
                return "error";
            }
        }

        public string GetWaterTemperatureMax()
        {
            try
            {
                int waterTemperatureMax = Convert.ToInt32(measurepoints.First().luminance);
                foreach (var item in measurepoints)
                {
                    if (Convert.ToInt32(item.heartFreq) > waterTemperatureMax)
                    {
                        waterTemperatureMax = Convert.ToInt32(item.heartFreq);
                    }
                }
                return waterTemperatureMax.ToString();
            }
            catch (Exception)
            {
                return "error";
            }
        }

        public string GetWaterTemperatureMin()
        {
            try
            {
                int waterTemperatureMin = Convert.ToInt32(measurepoints.First().luminance);
                foreach (var item in measurepoints)
                {
                    if (Convert.ToInt32(item.heartFreq) < waterTemperatureMin)
                    {
                        waterTemperatureMin = Convert.ToInt32(item.heartFreq);
                    }
                }
                return waterTemperatureMin.ToString();
            }
            catch (Exception)
            {
                return "error";
            }
        }

        public string GetTotalTime()
        {
            float time = 0;
            foreach (var item in measurepoints)
            {
                try
                {
                    time += (float)Convert.ToDouble(item.duration);
                }
                catch (Exception)
                {
                    
                }
            }
            return time.ToString();            
        }
    }
}