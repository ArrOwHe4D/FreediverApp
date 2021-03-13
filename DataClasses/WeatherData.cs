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

namespace FreediverApp.DataClasses
{
    class WeatherData
    {
        public WeatherData() { }

        public string temp;
        public string tempFeelsLike;
        public string main;
        public string description;
        public string pressure;
        public string humidity;
        public string windSpeed;
        public string windGust;
    }
}