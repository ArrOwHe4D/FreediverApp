using System;
using System.Collections.Generic;
using System.Linq;

namespace FreediverApp
{
    /**
     *  This dataclass represents a dive within a divesession. The dive itself contains a list of measurepoints
     *  that are transmitted from the arduino dive computer. The class also handles the calculation of max and min
     *  values of different attributes that can then be displayed inside a dive detail view.
     **/
    public class Dive
    {
        public List<Measurepoint> measurepoints = new List<Measurepoint>();
        public string duration;
        public string refDivesession;
        public string timestampBegin;
        public string timestampEnd;
        public string id;

        private string heartFreqMax;
        private string heartFreqMin;
        private string luminanceMax;
        private string luminanceMin;
        public string maxDepth;
        private string oxygenSaturationMax;
        private string oxygenSaturationMin;
        private string waterTemperatureMax;

        public Dive(string _refDiveSession, string reihenfolge)
        {
            refDivesession = _refDiveSession;
            id = refDivesession + "_" + reihenfolge;

            timestampBegin = "";
            timestampEnd = "";
            heartFreqMax = "";
            heartFreqMin = "";
            luminanceMax = "";
            luminanceMin = "";
            maxDepth = "";
            oxygenSaturationMax = "";
            oxygenSaturationMin = "";
            waterTemperatureMax = "";
            waterTemperatureMin = "";
            duration = "";
        }

        public Dive() { } 

        public string HeartFreqMax
        {
            get
            {
                if (heartFreqMax != null)
                {
                    return heartFreqMax;
                }
                else
                {
                    heartFreqMax = GetHeartFreqMax();
                    return heartFreqMax;
                }
            }
            set { heartFreqMax = value; }
        }


        public string HeartFreqMin
        {
            get
            {
                if (heartFreqMin != null)
                {
                    return heartFreqMin;
                }
                else
                {
                    heartFreqMin = GetHeartFreqMin();
                    return heartFreqMin;
                }
            }
            set { heartFreqMin = value; }
        }


        public string LuminanceMax
        {
            get
            {
                if (luminanceMax != null)
                {
                    return luminanceMax;
                }
                else
                {
                    luminanceMax = GetLuminanceMax();
                    return luminanceMax;
                }
            }
            set { luminanceMax = value; }
        }


        public string LuminanceMin
        {
            get
            {
                if (luminanceMin != null)
                {
                    return luminanceMin;
                }
                else
                {
                    luminanceMin = GetLuminanceMin();
                    return luminanceMin;
                }
            }
            set { luminanceMin = value; }
        }

       

        public string OxygenSaturationMax
        {
            get
            {
                if (oxygenSaturationMax != null)
                {
                    return oxygenSaturationMax;
                }
                else
                {
                    oxygenSaturationMax = GetOxygenSaturationMax();
                    return oxygenSaturationMax;
                }
            }
            set { oxygenSaturationMax = value; }
        }


        public string OxygenSaturationMin
        {
            get
            {
                if (oxygenSaturationMin != null)
                {
                    return oxygenSaturationMin;
                }
                else
                {
                    oxygenSaturationMin = GetOxygenSaturationMin();
                    return oxygenSaturationMin;
                }
            }
            set { oxygenSaturationMin = value; }
        }                


        public string WaterTemperatureMax
        {
            get
            {
                if (waterTemperatureMax != null)
                {
                    return waterTemperatureMax;
                }
                else
                {
                    waterTemperatureMax = GetWaterTemperatureMax();
                    return waterTemperatureMax;
                }
            }
            set { waterTemperatureMax = value; }
        }

        private string waterTemperatureMin;

        public string WaterTemperatureMin
        {
            get
            {
                if (waterTemperatureMin != null)
                {
                    return waterTemperatureMin;
                }
                else
                {
                    waterTemperatureMin = GetWaterTemperatureMin();
                    return waterTemperatureMin;
                }
            }
            set { waterTemperatureMin = value; }
        }

        public string GetHeartFreqMax()
        {
            try
            {
                int maxheartfreq = Convert.ToInt32(measurepoints.First().heart_freq);
                foreach (var item in measurepoints)
                {
                    if (Convert.ToInt32(item.heart_freq) > maxheartfreq)
                    {
                        maxheartfreq = Convert.ToInt32(item.heart_freq);
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
                int minheartfreq = Convert.ToInt32(measurepoints.First().heart_freq);
                foreach (var item in measurepoints)
                {
                    if (Convert.ToInt32(item.heart_freq) < minheartfreq)
                    {
                        minheartfreq = Convert.ToInt32(item.heart_freq);
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
                    if (Convert.ToInt32(item.luminance) < luminanceMin)
                    {
                        luminanceMin = Convert.ToInt32(item.luminance);
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
                    if (Convert.ToInt32(item.luminance) > luminanceMax)
                    {
                        luminanceMax = Convert.ToInt32(item.luminance);
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
                int oxygenSaturationMax = Convert.ToInt32(measurepoints.First().oxygen_saturation);
                foreach (var item in measurepoints)
                {
                    if (Convert.ToInt32(item.oxygen_saturation) > oxygenSaturationMax)
                    {
                        oxygenSaturationMax = Convert.ToInt32(item.oxygen_saturation);
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
                int oxygenSaturationMin = Convert.ToInt32(measurepoints.First().oxygen_saturation);
                foreach (var item in measurepoints)
                {
                    if (Convert.ToInt32(item.oxygen_saturation) < oxygenSaturationMin)
                    {
                        oxygenSaturationMin = Convert.ToInt32(item.oxygen_saturation);
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
                int waterTemperatureMax = Convert.ToInt32(measurepoints.First().water_temp);
                foreach (var item in measurepoints)
                {
                    if (Convert.ToInt32(item.water_temp) > waterTemperatureMax)
                    {
                        waterTemperatureMax = Convert.ToInt32(item.water_temp);
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
                int waterTemperatureMin = Convert.ToInt32(measurepoints.First().water_temp);
                foreach (var item in measurepoints)
                {
                    if (Convert.ToInt32(item.water_temp) < waterTemperatureMin)
                    {
                        waterTemperatureMin = Convert.ToInt32(item.water_temp);
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
            try
            {
                return (Convert.ToInt32(measurepoints.Last().duration) / 1000).ToString();
            }
            catch (Exception)
            {
                return "error";
            }                        
        }

        public string GetMaxDepth()
        {
            try
            {
                double maxDepth = Convert.ToDouble(measurepoints.First().depth);
                foreach (var item in measurepoints)
                {
                    if (Convert.ToDouble(item.depth) > maxDepth)
                    {
                        maxDepth = Convert.ToDouble(item.depth);
                    }
                }
                return Math.Round(maxDepth, 2).ToString();
            }
            catch (Exception)
            {
                return "error";
            }            
        }

        public void UpdateAll()
        {
            heartFreqMax = GetHeartFreqMax();
            heartFreqMin = GetHeartFreqMin();
            luminanceMax = GetLuminanceMax();
            luminanceMin = GetLuminanceMin();
            oxygenSaturationMax = GetOxygenSaturationMax();
            oxygenSaturationMin = GetOxygenSaturationMin();
            waterTemperatureMax = GetWaterTemperatureMax();
            waterTemperatureMin = GetWaterTemperatureMin();
            maxDepth = GetMaxDepth();
            duration = GetTotalTime();
        }
    }
}