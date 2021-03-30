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
    /**
     *  This activity handles the creation of a new divesession. When the user creates a divesession for a particular day, it is then
     *  directly saved to db without referring any dives or measurepoints. The references will automatically created when dive data is 
     *  transferred from arduino inside the BluetoothFragment. 
     **/
    [Activity(Label = "AddSessionActivity", ScreenOrientation = ScreenOrientation.Portrait)]
    public class AddSessionActivity : Activity
    {
        private Button buttonAddSession;
        private Button buttonCancel;
        private TextView textViewConnectedWith;
        private TextView textViewLocation;
        private TextView textViewWeather;
        private TextView textViewDate;
        private TextView textViewDiveTime;
        private DiveSession diveSession;
        private FirebaseDataListener database;
        private List<SavedSession> savedSessions;

        /**
         *  This function initializes the activity with all of it´s UI components. It also directly creates a new divesession based
         *  on the current location and weather data and sets up the db listener to query if the session already exists for this date
         *  if the user clicks on the create button.
         **/
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.AddSessionPage);

            buttonAddSession = FindViewById<Button>(Resource.Id.button_add_session);
            buttonAddSession.Click += btnAddSession_Click;
            buttonCancel = FindViewById<Button>(Resource.Id.button_cancel);
            buttonCancel.Click += buttonCancel_Click;
            textViewConnectedWith = FindViewById<TextView>(Resource.Id.textview_connected_with);
            textViewLocation = FindViewById<TextView>(Resource.Id.textview_location);
            textViewWeather = FindViewById<TextView>(Resource.Id.textview_weather);
            textViewDate = FindViewById<TextView>(Resource.Id.textview_date);
            textViewDiveTime = FindViewById<TextView>(Resource.Id.textview_divetime);

            diveSession = createDiveSession();
            textViewConnectedWith.Text = TemporaryData.CONNECTED_DIVE_COMPUTER;
            textViewLocation.Text = diveSession.location_lon + " | " + diveSession.location_lat;
            textViewWeather.Text = diveSession.weatherCondition_main + " | " + diveSession.weatherTemperature;
            textViewDate.Text = diveSession.date;
            textViewDiveTime.Text = diveSession.watertime;

            database = new FirebaseDataListener();

            retrieveSavedSessions();
        }

        /**
         *  This function represents the onclick event handler for the add session button. After the button was clicked, 
         *  the created divesession is stored in the TemporaryData class to make the access easier inside other activities instead of
         *  reading it from db everytime we need to access data. After that we save the created session inside our db and also create
         *  a savedsessions object, which is stored inside a extra table to read from that table if we want to check if a session already exists
         *  because we only need to check the date. We do this because this is more efficient than reading all sessions from the current user 
         *  with all their data due to the limitation of our db api that doesn´t let us query on more than one parameter (WHERE clause with AND conditions).
         *  Afterwards the activity is closed with a call of the Finish function.
         **/
        private void btnAddSession_Click(object sender, EventArgs eventArgs)
        {            
            TemporaryData.CURRENT_DIVESESSION = diveSession;
            TemporaryData.CURRENT_USER.diveSessions.Add(diveSession);

            database.saveEntity("divesessions", diveSession);
            SavedSession savedSession = new SavedSession(TemporaryData.CURRENT_USER.id, DateTime.Now.Date.ToString("dd.MM.yyyy"));
            database.saveEntity("savedsessions", savedSession);

            Finish();
        }

        /**
         *  This function closes the activity when the user clicked on the cancel button. 
         **/
        private void buttonCancel_Click(object sender, EventArgs eventArgs)
        {
            Finish();
        }

        /**
         *  This function creates and initializes a new session when the activity is created. In here we read the current location
         *  such as the weather data from OpenWeatherMap API based on the current location in order to initialize the session.
         **/
        private DiveSession createDiveSession()
        {
            DiveSession ds = new DiveSession(TemporaryData.CURRENT_USER.id);
            
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

        /**
         *  This function checks if the user already granted permission to access the location on his phone. We need this 
         *  permission to retrieve weather data from OpenWeatherMap based on the current location of the user or at least the 
         *  last known location if the current one cannot be read.
         **/
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

        /**
         *  This function initializes the db listener and queries for all saved sessions from the current user.
         *  We need this to be sure that a user cannot create more than one session per day because we are only able to
         *  identify a session from arduino side by it´s date.
         **/
        private void retrieveSavedSessions()
        {
            database.QueryParameterized("savedsessions", "ref_user", TemporaryData.CURRENT_USER.id);
            database.DataRetrieved += database_savedSessionsDataRetrieved;
        }

        /**
         *  This function handles the result of the Query above. If there is already a session for this day, we refuse 
         *  to create a new one and prompt a error message instead so that user is notified that he already created a 
         *  divesession for today.
         **/
        private void database_savedSessionsDataRetrieved(object sender, FirebaseDataListener.DataEventArgs args)
        {
            savedSessions = args.SavedSessions;

            if(savedSessions != null)
            {
                foreach(SavedSession session in savedSessions)
                {
                    if(session.sessiondate == DateTime.Now.Date.ToString("dd.MM.yyyy"))
                    {
                        Toast.MakeText(this, "Es existiert bereits eine Session für dieses Datum!", ToastLength.Long).Show();
                        return;
                    }
                }
            }
        }
    }
}