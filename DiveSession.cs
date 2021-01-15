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
        public string heartFreqMax;
        public string heartFreqMin;
        public string luminanceMax;
        public string luminanceMin;
        public string maxDepth;
        public string oxygenSaturationMax;
        public string oxygenSaturationMin;
        public string refUser;
        public string timestampBegin;
        public string timestampEnd;
        public string waterTemperatureMax;
        public string waterTemperatureMin;
        
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