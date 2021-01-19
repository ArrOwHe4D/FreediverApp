using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using Firebase.Database;
using Java.Util;
using DBConnector = FreediverApp.DatabaseConnector.DatabaseConnector;

namespace FreediverApp
{
    [Activity(Label = "AddSessionActivity")]
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
            // Create your application here
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
            tvwLocation.Text = diveSession.location;
            tvwWeather.Text = diveSession.weatherCondition + " | " + diveSession.weatherTemperature;
            tvwDate.Text = diveSession.date;
            tvwDiveTime.Text = diveSession.watertime;
        }
        private void btnAddSession_Click(object sender, EventArgs eventArgs)
        {            
            User.curUser.diveSessions.Add(diveSession);
            SaveDiveSession(diveSession);
            var sessionsActivity = new Intent(this, typeof(SessionsActivity));
            StartActivity(sessionsActivity);
        }
        private void btnCancel_Click(object sender, EventArgs eventArgs)
        {
            var sessionsActivity = new Intent(this, typeof(SessionsActivity));
            StartActivity(sessionsActivity);
        }

        string[] locations = new string[] { "Koeln", "Leverkusen", "Gummersbach", "Kiel", "Bremerhafen" };
        string[] conditions = new string[] { "sonnig", "regnerisch", "bewölkt" };
        private DiveSession SampleData()
        {
            DiveSession ds = new DiveSession(User.curUser.id);
            System.Random rand = new System.Random();
            ds.date = DateTime.Now.ToShortDateString();
            ds.location = locations[rand.Next(locations.Length)];
            ds.weatherTemperature = rand.Next(5, 26) + "C°";
            ds.weatherCondition = conditions[rand.Next(conditions.Length)];
            ds.weatherWindSpeed = rand.Next(5, 15) + "Km/h";


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
                    if (oxySat >= 85)
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
                        if (dep < 0)
                        {
                            dep -= (float)rand.NextDouble();
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
                        heartFreq = hf.ToString(),
                        heartVar = hv.ToString(),
                        depth = dep.ToString(),
						duration = dur.ToString(),
                        refDive = "kp",
                        gyroscope_x = "kp",
                        gyroscope_y = "kp",
                        gyroscope_z = "kp",
                        luminance = lumi.ToString(),
                        oxygenSaturation = oxySat.ToString(),
                        waterTemperature = watTemp.ToString()                        
                    };
                    d.measurepoints.Add(m);
                }                
                ds.dives.Add(d);                
            }
            ds.UpdateDuration();
            return ds;
        }

        private void SaveDiveSession(DiveSession ds)
        {
            HashMap diveSessionData = new HashMap();
            diveSessionData.Put("date", ds.date);
            diveSessionData.Put("location", ds.location);
            diveSessionData.Put("ref_user", ds.refUser);
            diveSessionData.Put("watertime", ds.watertime);
            diveSessionData.Put("weather_condition", ds.weatherCondition);
            diveSessionData.Put("weather_temp", ds.weatherTemperature);
            diveSessionData.Put("weather_wind_speed", ds.weatherWindSpeed);
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
            measurepointData.Put("heart_freq", m.heartFreq);
            measurepointData.Put("heart_var", m.heartVar);
            measurepointData.Put("luminance", m.luminance);
            measurepointData.Put("oxygen_saturation", m.oxygenSaturation);
            measurepointData.Put("ref_dive", m.refDive);
            measurepointData.Put("water_temp", m.waterTemperature);


            DatabaseReference newMeasurepointsRef = DBConnector.GetDatabase().GetReference("measurepoints").Push();
            newMeasurepointsRef.SetValue(measurepointData);
        }

    }
}