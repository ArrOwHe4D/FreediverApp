using Xamarin.Essentials;

namespace FreediverApp.GeoLocationSevice
{
    /**
     *  This class handles the retrieval of the geo location data from the users phone.
     *  To do this we use Xamarin.Essentials that calls native Android APIs to provide different
     *  types of functionality like reading the location, network state and so on.
     **/
    public class GeoLocationService
    {
        public Location location;

        /**
         *  When a GeoLocationService object was created, it directly calls the getLocation function to return
         *  the last known geo location.
         */
        public GeoLocationService()
        {
            getLocation();
        }

        /**
         *  This function consists of a asynchronous API call that returns the last known location of the phone. 
         **/
        public async void getLocation()
        {
            location = await Geolocation.GetLastKnownLocationAsync();
        }
    }
}