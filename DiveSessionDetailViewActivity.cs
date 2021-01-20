using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using FreediverApp.DatabaseConnector;

namespace FreediverApp
{

    [Activity(Label = "Activity1")]
    public class DiveSessionDetailViewActivity : Activity
    {

        private Button btnDivesPerSession;
        private TextView tvwSessionName;
        private TextView tvwLocation;
        private TextView tvwDate;
        private TextView tvwWeather;
        private TextView tvwTimeInWater;
        private TextView tvwNotes;
        private FirebaseDataListener diveDataListener;
        private FirebaseDataListener measurepointDataListener;
        private List<Measurepoint> measurepointList = new List<Measurepoint>();

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.DiveSessionDetailViewPage);

            btnDivesPerSession = FindViewById<Button>(Resource.Id.btnDivesPerSession);
            btnDivesPerSession.Click += redirectToDivesPerSessionActivity;
            tvwSessionName = FindViewById<TextView>(Resource.Id.tvwDsdvSessionName);
            tvwLocation = FindViewById<TextView>(Resource.Id.tvwDsdvLocationV);
            //tvwDate = FindViewById<TextView>(Resource.Id.);
            tvwWeather = FindViewById<TextView>(Resource.Id.tvwDsdvWeatherV);
            tvwTimeInWater = FindViewById<TextView>(Resource.Id.tvwDsdvTimeInWaterV);
            tvwNotes = FindViewById<TextView>(Resource.Id.tvwDsdvNotesV);            
            tvwSessionName.Text = User.curUser.curDiveSession.date + " " + User.curUser.curDiveSession.location;
            tvwLocation.Text = User.curUser.curDiveSession.location;
            tvwWeather.Text = User.curUser.curDiveSession.weatherCondition + " | " + User.curUser.curDiveSession.weatherTemperature;
            tvwTimeInWater.Text = User.curUser.curDiveSession.watertime;

            RetrieveDiveData();
        }
        
        private void RetrieveDiveData()
        {
            diveDataListener = new FirebaseDataListener();
            diveDataListener.QueryParameterized("dives", "ref_divesession", User.curUser.curDiveSession.Id);
            diveDataListener.DataRetrieved += DiveDataListener_DataRetrieved;
        }

        private void DiveDataListener_DataRetrieved(object sender, FirebaseDataListener.DataEventArgs e)
        {            
            User.curUser.curDiveSession.dives = e.Dives;
            //GetMeasurepoints();
        }
        int counter = 0;
        private void GetMeasurepoints()
        {
            counter++;
            foreach (var item in User.curUser.curDiveSession.dives)
            {
                RetrieveMeasurepointData(item);
                item.measurepoints = measurepointList;                                
            }
        }

        private void RetrieveMeasurepointData(Dive d)
        {            
            measurepointDataListener = new FirebaseDataListener();
            measurepointDataListener.QueryParameterized("measurepoints", "ref_dive", d.Id);
            measurepointDataListener.DataRetrieved += MeasurepointDataListener_DataRetrieved;
        }        
        private void MeasurepointDataListener_DataRetrieved(object sender, FirebaseDataListener.DataEventArgs e)
        {
            lock (measurepointList)
            {
                measurepointList = e.Measurepoints;
            }                      
        }

        private void redirectToDivesPerSessionActivity(object sender, EventArgs eventArgs)
        {
            var divesPerSessionActivity = new Intent(this, typeof(DivesPerSessionActivity));
            StartActivity(divesPerSessionActivity);
        }
    }
}