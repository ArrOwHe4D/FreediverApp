using System;
using FreediverApp.DataClasses;
using Newtonsoft.Json.Linq;
using System.Net;

namespace FreediverApp.OpenWeatherMap
{
    /**
     *  This class commuinicates with OpenWeatherMap API to get weather data
     *  for a specific location that are passed as constructor arguments.
     **/
    class OpenWeatherMapConnector
    {
        private string API_KEY;
        private string lon;
        private string lat;
        private string jsonString;
        private string currentURL;

        public OpenWeatherMapConnector(double lon, double lat)
        {
            API_KEY = "c20852e07b7189c61e6372f6c676234a";
            this.lon = lon.ToString();
            this.lat = lat.ToString();
            currentURL = "http://api.openweathermap.org/data/2.5/weather?" +
            "lat=" + lat + "&lon=" + lon + "&appid=" + API_KEY + "&units=metric&lang=de";
        }

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