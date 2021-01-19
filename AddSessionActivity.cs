using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace FreediverApp
{
    [Activity(Label = "AddSessionActivity")]
    public class AddSessionActivity : Activity
    {
        private Button btnAddSession;
        private Button btnCancel;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.AddSessionPage);
            // Create your application here
            btnAddSession = FindViewById<Button>(Resource.Id.btnAddSession);
            btnAddSession.Click += btnAddSession_Click;
            btnCancel = FindViewById<Button>(Resource.Id.btnCancel);
            btnCancel.Click += btnCancel_Click;
        }
        private void btnAddSession_Click(object sender, EventArgs eventArgs)
        {
            //SessionsActivity.dives
            User.curUser.diveSessions.Add(new DiveSession("9.12.2020"));
            var sessionsActivity = new Intent(this, typeof(SessionsActivity));
            StartActivity(sessionsActivity);
        }
        private void btnCancel_Click(object sender, EventArgs eventArgs)
        {
            var sessionsActivity = new Intent(this, typeof(SessionsActivity));
            StartActivity(sessionsActivity);
        }

        string[] locations = new string[] { "Koeln", "Leverkusen", "Gummersbach", "Kiel", "Bremerhafen" };
        private DiveSession SampleData()
        {
            DiveSession ds = new DiveSession();
            Random rand = new Random();
            ds.date = DateTime.Now.ToShortDateString();
            ds.location = locations[rand.Next(locations.Length)];
            

            for (int i = 0; i < 3; i++)
            {
                Dive d = new Dive();
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
                    Measurepoint m = new Measurepoint()
                    {
                        acceleration = "kp",
                        heartFreq = hf.ToString(),
                        heartVar = hv.ToString(),
                        depth = dep.ToString(),
                        duration = dur.ToString(),
                        magnetSensorData = "kp",
                        refDive = "kp",
                        gyroscope = "kp",
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

    }
}