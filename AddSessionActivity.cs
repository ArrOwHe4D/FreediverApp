using System;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Widget;
using Firebase.Database;
using Java.Util;
using DBConnector = FreediverApp.DatabaseConnector.DatabaseConnector;
using FreediverApp.GeoLocationSevice;
using Xamarin.Essentials;
using FreediverApp.DataClasses;
using FreediverApp.OpenWeatherMap;
using Android;
using Android.Support.V4.Content;
using Android.Support.V4.App;

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

            diveSession = SampleData();
            tvwLocation.Text = diveSession.location_lon + " | " + diveSession.location_lat;
            tvwWeather.Text = diveSession.weatherCondition_main + " | " + diveSession.weatherTemperature;
            tvwDate.Text = diveSession.date;
            tvwDiveTime.Text = diveSession.watertime;
        }

        private void btnAddSession_Click(object sender, EventArgs eventArgs)
        {            
            User.curUser.diveSessions.Add(diveSession);
            SaveDiveSession(diveSession);
            SampleData();

            Finish();
        }

        private void btnCancel_Click(object sender, EventArgs eventArgs)
        {
            Finish();
        }

        private DiveSession SampleData()
        {
            DiveSession ds = new DiveSession(User.curUser.id);
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
                    Measurepoint m = new Measurepoint(d.Id)
                    {
                        accelerator_x = "kp",
                        accelerator_y = "kp",
                        accelerator_z = "kp",
                        heart_freq = hf.ToString(),
                        heart_var = hv.ToString(),
                        depth = dep.ToString(),
						duration = dur.ToString(),
                        ref_dive = d.Id,
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

        private void SaveDiveSession(DiveSession ds)
        {
            HashMap diveSessionData = new HashMap();
            diveSessionData.Put("date", ds.date);
            diveSessionData.Put("location_lon", ds.location_lon);
            diveSessionData.Put("location_lat", ds.location_lat);
            diveSessionData.Put("ref_user", ds.refUser);
            diveSessionData.Put("watertime", ds.watertime);
            diveSessionData.Put("weather_condition_main", ds.weatherCondition_main);
            diveSessionData.Put("weather_condition_description", ds.weatherCondition_description);
            diveSessionData.Put("weather_temp", ds.weatherTemperature);
            diveSessionData.Put("weather_temp_feels_like", ds.weatherTemperatureFeelsLike);
            diveSessionData.Put("weather_pressure", ds.weatherPressure);
            diveSessionData.Put("weather_humidity", ds.weatherHumidity);
            diveSessionData.Put("weather_wind_speed", ds.weatherWindSpeed);
            diveSessionData.Put("weather_wind_gust", ds.weatherWindGust);
            diveSessionData.Put("id", ds.Id);


            DatabaseReference newDiveSessionsRef = DBConnector.GetDatabase().GetReference("divesessions").Push();
            newDiveSessionsRef.SetValue(diveSessionData);

            foreach (var item in ds.dives)
            {
                SaveDive(item);
            }
        }

        private void SaveDive(Dive d)
        {
            HashMap diveData = new HashMap();
            diveData.Put("duration", d.GetTotalTime());
            diveData.Put("heart_freq_max", d.HeartFreqMax);
            diveData.Put("heart_freq_min", d.HeartFreqMin);
            diveData.Put("luminance_max", d.LuminanceMax);
            diveData.Put("luminance_min", d.LuminanceMin);
            diveData.Put("max_depth", d.maxDepth);
            diveData.Put("oxygen_saturation_max", d.OxygenSaturationMax);
            diveData.Put("oxygen_saturation_min", d.OxygenSaturationMin);
            diveData.Put("ref_divesession", d.refDivesession);
            diveData.Put("timestamp_begin", d.timestampBegin);
            diveData.Put("timestamp_end", d.timestampEnd);
            diveData.Put("water_temp_max", d.WaterTemperatureMax);
            diveData.Put("water_temp_min", d.WaterTemperatureMin);
            diveData.Put("id", d.Id);


            DatabaseReference newDivesrRef = DBConnector.GetDatabase().GetReference("dives").Push();
            newDivesrRef.SetValue(diveData);

            foreach (var item in d.measurepoints)
            {
                SaveMeasurepoint(item);
            }
        }

        private void SaveMeasurepoint(Measurepoint m)
        {
            HashMap measurepointData = new HashMap();
            measurepointData.Put("accelerator_x", m.accelerator_x);
            measurepointData.Put("accelerator_y", m.accelerator_y);
            measurepointData.Put("accelerator_z", m.accelerator_z);
            measurepointData.Put("depth", m.depth);
            measurepointData.Put("duration", m.duration);
            measurepointData.Put("gyroscope_x", m.gyroscope_x);
            measurepointData.Put("gyroscope_y", m.gyroscope_y);
            measurepointData.Put("gyroscope_z", m.gyroscope_z);
            measurepointData.Put("heart_freq", m.heart_freq);
            measurepointData.Put("heart_var", m.heart_var);
            measurepointData.Put("luminance", m.luminance);
            measurepointData.Put("oxygen_saturation", m.oxygen_saturation);
            measurepointData.Put("ref_dive", m.ref_dive);
            measurepointData.Put("water_temp", m.water_temp);


            DatabaseReference newMeasurepointsRef = DBConnector.GetDatabase().GetReference("measurepoints").Push();
            newMeasurepointsRef.SetValue(measurepointData);
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