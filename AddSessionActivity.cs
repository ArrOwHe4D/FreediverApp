using System;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Widget;
using FreediverApp.GeoLocationSevice;
using Xamarin.Essentials;
using FreediverApp.DataClasses;
using FreediverApp.OpenWeatherMap;
using Android;
using Android.Support.V4.Content;
using Android.Support.V4.App;
using FreediverApp.DatabaseConnector;
using System.Collections.Generic;

namespace FreediverApp
{
    [Activity(Label = "AddSessionActivity", ScreenOrientation = ScreenOrientation.Portrait)]
    public class AddSessionActivity : Activity
    {
        private Button btnAddSession;
        private Button btnCancel;
        private TextView tvwConnected;
        private TextView tvwLocation;
        private TextView tvwWeather;
        private TextView tvwDate;
        private TextView tvwDiveTime;
        private DiveSession diveSession;
        private FirebaseDataListener database;
        private List<SavedSession> savedSessions;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.AddSessionPage);

            btnAddSession = FindViewById<Button>(Resource.Id.btnAddSession);
            btnAddSession.Click += btnAddSession_Click;
            btnCancel = FindViewById<Button>(Resource.Id.btnCancel);
            btnCancel.Click += btnCancel_Click;
            tvwConnected = FindViewById<TextView>(Resource.Id.tvwConnectedV);
            tvwLocation = FindViewById<TextView>(Resource.Id.tvwLocationV);
            tvwWeather = FindViewById<TextView>(Resource.Id.tvwWeatherV);
            tvwDate = FindViewById<TextView>(Resource.Id.tvwDateV);
            tvwDiveTime = FindViewById<TextView>(Resource.Id.tvwDiveTimeV);

            diveSession = createDiveSession();
            tvwLocation.Text = diveSession.location_lon + " | " + diveSession.location_lat;
            tvwWeather.Text = diveSession.weatherCondition_main + " | " + diveSession.weatherTemperature;
            tvwDate.Text = diveSession.date;
            tvwDiveTime.Text = diveSession.watertime;

            database = new FirebaseDataListener();

            retrieveSavedSessions();
        }


        private void btnAddSession_Click(object sender, EventArgs eventArgs)
        {            
            TemporaryData.CURRENT_DIVESESSION = diveSession;
            TemporaryData.CURRENT_USER.diveSessions.Add(diveSession);

            database.saveEntity("divesessions", diveSession);
            SavedSession savedSession = new SavedSession(TemporaryData.CURRENT_USER.id, DateTime.Now.Date.ToString("dd.MM.yyyy"));
            database.saveEntity("savedsessions", savedSession);

            Finish();
        }

        private void btnCancel_Click(object sender, EventArgs eventArgs)
        {
            Finish();
        }

        private DiveSession createDiveSession()
        {
            DiveSession ds = new DiveSession(TemporaryData.CURRENT_USER.id);
            System.Random rand = new System.Random();
            
            if (checkLocationPermission())
            {
                Location location = new GeoLocationService().location;
                OpenWeatherMapConnector openWeatherMapConnector = new OpenWeatherMapConnector(location.Longitude, location.Latitude);
                WeatherData weatherData = openWeatherMapConnector.downloadWeatherData();

                ds.date = DateTime.Now.ToShortDateString();
                ds.location_lat = location.Latitude.ToString();
                ds.location_lon = location.Longitude.ToString();
                ds.weatherTemperature = weatherData.temp;
                ds.weatherTemperatureFeelsLike = weatherData.tempFeelsLike;
                ds.weatherCondition_main = weatherData.main;
                ds.weatherCondition_description = weatherData.description;
                ds.weatherPressure = weatherData.pressure;
                ds.weatherHumidity = weatherData.humidity;
                ds.weatherWindSpeed = weatherData.windSpeed;
                ds.weatherWindGust = weatherData.windGust;
                ds.watertime = "";
            }
            else 
            {
                Toast.MakeText(this, "Please enable Location services on your device!", ToastLength.Long).Show();
            }

            return ds;
        }

        private bool checkLocationPermission()
        {
            const int locationPermissionsRequestCode = 1000;

            var locationPermissions = new[]
            {
                Manifest.Permission.AccessCoarseLocation,
                Manifest.Permission.AccessFineLocation
            };

            var coarseLocationPermissionGranted = ContextCompat.CheckSelfPermission(this, Manifest.Permission.AccessCoarseLocation);

            var fineLocationPermissionGranted = ContextCompat.CheckSelfPermission(this, Manifest.Permission.AccessFineLocation);

            if (coarseLocationPermissionGranted == Permission.Denied || fineLocationPermissionGranted == Permission.Denied)
                ActivityCompat.RequestPermissions(this, locationPermissions, locationPermissionsRequestCode);

            return coarseLocationPermissionGranted == Permission.Granted && fineLocationPermissionGranted == Permission.Granted;
        }

        private void retrieveSavedSessions()
        {
            database.QueryParameterized("savedsessions", "ref_user", TemporaryData.CURRENT_USER.id);
            database.DataRetrieved += database_savedSessionsDataRetrieved;
        }

        private void database_savedSessionsDataRetrieved(object sender, FirebaseDataListener.DataEventArgs args)
        {
            savedSessions = args.SavedSessions;

            if(savedSessions != null)
            {
                foreach(SavedSession s in savedSessions)
                {
                    if(s.sessiondate == DateTime.Now.Date.ToString("dd.MM.yyyy"))
                    {
                        Toast.MakeText(this, "Es existiert bereits eine Session für dieses Datum!", ToastLength.Long).Show();
                        return;
                    }
                }
            }
        }
    }
}