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
                    UpdateHeartFreqMax();
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
                    UpdateHeartFreqMin();
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
                    UpdateLuminanceMax();
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
                    UpdateLuminanceMin();
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
                    UpdateOxygenSaturationMax();
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
                    UpdateOxygenSaturationMin();
                    return oxygenSaturationMin;
                }
            }
            set { oxygenSaturationMin = value; }
        }
        public string refUser;
        public string timestampBegin;
        public string timestampEnd;
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
                    UpdateWaterTemperatureMax();
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
                    UpdateWaterTemperatureMin();
                    return waterTemperatureMin;
                }
            }
            set { waterTemperatureMin = value; }
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

        public void UpdateMaxMinValues()
        {
            UpdateHeartFreqMax();
            UpdateHeartFreqMin();
            UpdateLuminanceMax();
            UpdateLuminanceMin();
            UpdateOxygenSaturationMax();
            UpdateOxygenSaturationMin();
            UpdateWaterTemperatureMax();
            UpdateWaterTemperatureMin();
        }

        private void UpdateWaterTemperatureMin()
        {
            try
            {
                string _waterTemperatureMin = dives.First().GetWaterTemperatureMin();
                foreach (var item in dives)
                {
                    if (Convert.ToInt32(item.GetWaterTemperatureMin()) > Convert.ToInt32(_waterTemperatureMin))
                    {
                        _waterTemperatureMin = item.GetWaterTemperatureMin();
                    }
                }
                waterTemperatureMin = _waterTemperatureMin;
            }
            catch (Exception)
            {
               waterTemperatureMin = "error";
            }
        }

        private void UpdateWaterTemperatureMax()
        {
            try
            {
                string _waterTemperatureMax = dives.First().GetWaterTemperatureMax();
                foreach (var item in dives)
                {
                    if (Convert.ToInt32(item.GetWaterTemperatureMax()) > Convert.ToInt32(_waterTemperatureMax))
                    {
                        _waterTemperatureMax = item.GetWaterTemperatureMax();
                    }
                }
                waterTemperatureMax = _waterTemperatureMax;
            }
            catch (Exception)
            {
                waterTemperatureMax = "error";
            }
        }

        private void UpdateOxygenSaturationMin()
        {
            try
            {
                string _oxygenSaturationMin = dives.First().GetOxygenSaturationMin();
                foreach (var item in dives)
                {
                    if (Convert.ToInt32(item.GetOxygenSaturationMin()) > Convert.ToInt32(_oxygenSaturationMin))
                    {
                        _oxygenSaturationMin = item.GetOxygenSaturationMin();
                    }
                }
                oxygenSaturationMin = _oxygenSaturationMin;
            }
            catch (Exception)
            {
                oxygenSaturationMin = "error";
            }
        }

        private void UpdateOxygenSaturationMax()
        {
            try
            {
                string _oxygenSaturationMax = dives.First().GetOxygenSaturationMax();
                foreach (var item in dives)
                {
                    if (Convert.ToInt32(item.GetOxygenSaturationMax()) > Convert.ToInt32(_oxygenSaturationMax))
                    {
                        _oxygenSaturationMax = item.GetOxygenSaturationMax();
                    }
                }
                oxygenSaturationMax = _oxygenSaturationMax;
            }
            catch (Exception)
            {
                oxygenSaturationMax = "error";
            }
        }

        private void UpdateLuminanceMin()
        {
            try
            {
                string _luminanceMin = dives.First().GetLuminanceMin();
                foreach (var item in dives)
                {
                    if (Convert.ToInt32(item.GetLuminanceMin()) > Convert.ToInt32(_luminanceMin))
                    {
                        _luminanceMin = item.GetLuminanceMin();
                    }
                }
                luminanceMin = _luminanceMin;
            }
            catch (Exception)
            {
                luminanceMin = "error";
            }
        }

        private void UpdateLuminanceMax()
        {
            try
            {
                string _luminanceMax = dives.First().GetLuminanceMax();
                foreach (var item in dives)
                {
                    if (Convert.ToInt32(item.GetLuminanceMax()) > Convert.ToInt32(_luminanceMax))
                    {
                        _luminanceMax = item.GetLuminanceMax();
                    }
                }
                luminanceMax = _luminanceMax;
            }
            catch (Exception)
            {
                luminanceMax = "error";
            }
        }

        private void UpdateHeartFreqMin()
        {
            try
            {
                string _heartFreqMin = dives.First().GetHeartFreqMin();
                foreach (var item in dives)
                {
                    if (Convert.ToInt32(item.GetHeartFreqMin()) > Convert.ToInt32(_heartFreqMin))
                    {
                        _heartFreqMin = item.GetHeartFreqMin();
                    }
                }
                heartFreqMin = _heartFreqMin;
            }
            catch (Exception)
            {
                heartFreqMin = "error";
            }
        }

        private void UpdateHeartFreqMax()
        {
            try
            {
                string _heartFreqMax = dives.First().GetHeartFreqMax();
                foreach (var item in dives)
                {
                    if (Convert.ToInt32(item.GetHeartFreqMax()) > Convert.ToInt32(_heartFreqMax))
                    {
                        _heartFreqMax = item.GetHeartFreqMax();
                    }
                }
                heartFreqMax = _heartFreqMax;
            }
            catch (Exception)
            {
                heartFreqMax = "error";
            }
        }
    }
}