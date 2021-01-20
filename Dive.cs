using System;
using System.Collections.Generic;
using System.Linq;

namespace FreediverApp
{
    class Dive
    {
        public List<Measurepoint> measurepoints = new List<Measurepoint>();
        public string duration;
        public string refDivesession;
        public string timestampBegin;
        public string timestampEnd;
        private string id;
        public string Id
        {
            get { return id; }
            set { id = value; }
        }

        public Dive(string _refDiveSession, string reihenfolge)
        {
            refDivesession = _refDiveSession;
            id = refDivesession + "_" + reihenfolge;
        }

        public Dive() { } 

        private string heartFreqMax;
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
        private string heartFreqMin;
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
        private string luminanceMax;
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
        private string luminanceMin;
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
        public string maxDepth;

        private string oxygenSaturationMax;
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
        private string oxygenSaturationMin;
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
        private string waterTemperatureMax;
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