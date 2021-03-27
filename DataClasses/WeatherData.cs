namespace FreediverApp.DataClasses
{
    /**
     *  This dataclass holds weather data that is returned from the OpenWeatherMap API which is called 
     *  inside the OpenWeatherMapConnector.cs class. This data is used to create a new divesession.
     **/
    public class WeatherData
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