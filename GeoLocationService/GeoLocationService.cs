using System.Collections.Generic;
using Xamarin.Essentials;
using System.Linq;
using System;
using System.Threading.Tasks;

namespace FreediverApp.GeoLocationServiceNamespace
{
    public class GeoLocationService
    {
        /**
         *  This class handles the retrieval of the geo location data from the users phone.
         *  To do this we use Xamarin.Essentials that calls native Android APIs to provide different
         *  types of functionality like reading the location, network state and so on.
         **/
        public Location location;
        public string location_country;
        public string location_locality;

        /**
         *  When a GeoLocationService object was created, it directly calls the getLocation function to return
         *  the last known geo location.
         */
        public GeoLocationService()
        {
            
        }

        /**
         *  This function consists of a asynchronous API call that returns the last known location of the phone. 
         **/
        public async void getLocation()
        {
            location = await Geolocation.GetLastKnownLocationAsync();
        }

        public async Task getLocation_name(double location_lat, double location_long)
        {
            try
            {
                var placemarks = await Geocoding.GetPlacemarksAsync(location_lat, location_long);

                var placemark = placemarks?.FirstOrDefault();
                if (placemark != null)
                {
                    location_country = placemark.CountryName;
                    location_locality = placemark.Locality;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}