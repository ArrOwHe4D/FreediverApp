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
    class Measurepoint
    {
        public string acceleration; // string
        public string depth; // float
        public string duration; // float
        public string gyroscope; // string
        public string heartFreq; // int
        public string heartVar; // int
        public string luminance; // int
        public string magnetSensorData; //
        public string oxygenSaturation; // int
        public string refDivesession; //
        public string waterTemperature; // float

        public Measurepoint() { }

    }
}