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
        }

        private void btnAddSession_Click(object sender, EventArgs eventArgs)
        {            
            diveSession = new DiveSession(TemporaryData.CURRENT_USER.id);
            TemporaryData.CURRENT_DIVESESSION = diveSession;
            TemporaryData.CURRENT_USER.diveSessions.Add(diveSession);

            database.saveEntity("divesessions", diveSession);

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
            }
            else 
            {
                Toast.MakeText(this, "Please enable Location services on your device!", ToastLength.Long).Show();
            }

            for (int i = 0; i < 3; i++)
            {
                Dive d = new Dive(ds.Id, i.ToString());                
                #region Measurepoints start values
                int hf = rand.Next(90, 130);
                int hv = rand.Next(750, 800);
                float dep = 0;
                float dur = 0;
                int lumi = rand.Next(40, 60);
                int oxySat = rand.Next(90, 98);
                float watTemp = rand.Next(5, 18);
                #endregion

                int measurep = rand.Next(80, 200);
                for (int u = 0; u < measurep; u++)
                {
                    #region Measurepoints changeings
                    hf += rand.Next(-1, 2);
                    hv += rand.Next(-3, 4);
                    oxySat += rand.Next(-1, 2);
                    if (oxySat <= 85)
                    {
                        oxySat++;
                    }
                    else if (oxySat > 100)
                    {
                        oxySat--;
                    }
                    dur += 0.2f;
                    if (u < (measurep / 2))
                    {
                        dep += (float)rand.NextDouble();
                        watTemp += (float)rand.Next(0, 3) / 10;
                        if (lumi > 2)
                        {
                            lumi -= rand.Next(0, 2);
                        }                        
                    }
                    else
                    {
                        if (dep > 0)
                        {
                            dep -= (float)rand.NextDouble();
                            if (dep < 0) { dep = 0; }
                            watTemp -= (float)rand.Next(0, 3) / 10;
                            if (lumi < 70)
                            {
                                lumi += rand.Next(0, 2);
                            }                            
                        }                        
                    }
                    #endregion
                    Measurepoint m = new Measurepoint(d.id)
                    {
                        accelerator_x = "kp",
                        accelerator_y = "kp",
                        accelerator_z = "kp",
                        heart_freq = hf.ToString(),
                        heart_var = hv.ToString(),
                        depth = dep.ToString(),
						duration = dur.ToString(),
                        ref_dive = d.id,
                        gyroscope_x = "kp",
                        gyroscope_y = "kp",
                        gyroscope_z = "kp",
                        luminance = lumi.ToString(),
                        oxygen_saturation = oxySat.ToString(),
                        water_temp = watTemp.ToString()                        
                    };
                    d.measurepoints.Add(m);
                }
                d.timestampBegin = "kp";
                d.timestampEnd = "kp";                
                d.UpdateAll();
                ds.dives.Add(d);                
            }
            ds.UpdateDuration();
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
    }
}