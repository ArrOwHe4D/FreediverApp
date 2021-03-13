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
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace FreediverApp.GeoLocationSevice
{
    public class GeoLocationService
    {
        public Location location;

        public GeoLocationService()
        {
            getLocation();
        }
        public async void getLocation()
        {
            location = await Geolocation.GetLastKnownLocationAsync();
        }
    }
}