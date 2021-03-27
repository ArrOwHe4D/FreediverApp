using System;
using FreediverApp.DataClasses;
using Newtonsoft.Json.Linq;
using System.Net;

namespace FreediverApp.OpenWeatherMap
{
    /**
     *  This class commuinicates with OpenWeatherMap API to get weather data
     *  based on a specific location. The coordinates of that location
     *  are passed as constructor arguments to build the url for the API call.
     **/
    class OpenWeatherMapConnector
    {
        private string API_KEY;
        private string lon;
        private string lat;
        private string jsonString;
        private string currentURL;

        /**
         *  Constructor that sets the API key and builds the URL based on the location coordinates. 
         **/
        public OpenWeatherMapConnector(double lon, double lat)
        {
            API_KEY = "c20852e07b7189c61e6372f6c676234a";
            this.lon = lon.ToString();
            this.lat = lat.ToString();
            currentURL = "http://api.openweathermap.org/data/2.5/weather?" +
            "lat=" + lat + "&lon=" + lon + "&appid=" + API_KEY + "&units=metric&lang=de";
        }

        /**
         *  This function builds a jsonobject based on the answer we get from the API call.
         *  With that jsonobject another object of our own WeatherData class is created which can
         *  then be used to read the needed data inside a activity or fragment for example.
         **/
        private WeatherData extractWeatherData(string jsonString)
        {
            JObject jsonDataObject = JObject.Parse(jsonString);
            WeatherData dataObject = new WeatherData();

            dataObject.temp = jsonDataObject["main"]["temp"].ToString();
            dataObject.tempFeelsLike = jsonDataObject["main"]["feels_like"].ToString();
            dataObject.main = jsonDataObject["weather"][0]["main"].ToString();
            dataObject.description = jsonDataObject["weather"][0]["description"].ToString();
            dataObject.pressure = jsonDataObject["main"]["pressure"].ToString();
            dataObject.humidity = jsonDataObject["main"]["humidity"].ToString();
            dataObject.windSpeed = jsonDataObject["wind"]["speed"].ToString();
            dataObject.windGust = jsonDataObject["wind"]["gust"].ToString();

            return dataObject;
        }

        /**
         *  This function realizes the actual API call from OpenWeaterMap. At the end a 
         *  WeatherData object based on the JSON result string is returned that can easily be used
         *  to read weatherdata inside another context in our application.
         **/
        public WeatherData downloadWeatherData()
        {
            using (WebClient client = new WebClient())
            {
                try
                {
                    jsonString = client.DownloadString(currentURL);
                }
                catch (WebException ex)
                {
                    Console.WriteLine(ex);
                }
            }
            return extractWeatherData(jsonString);
        }
    }
}