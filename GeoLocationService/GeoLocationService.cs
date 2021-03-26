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