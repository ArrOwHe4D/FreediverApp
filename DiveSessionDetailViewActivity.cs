using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Widget;
using FreediverApp.DatabaseConnector;
using Microcharts;
using Microcharts.Droid;
using SkiaSharp;

namespace FreediverApp
{

    [Activity(Label = "Activity1", ScreenOrientation = ScreenOrientation.Portrait)]
    public class DiveSessionDetailViewActivity : Activity
    {

        private Button btnDivesPerSession;
        private ChartView chartView;
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
            tvwSessionName.Text = User.curUser.curDiveSession.date + " " + User.curUser.curDiveSession.location_lon + " | " + User.curUser.curDiveSession.location_lat;
            tvwLocation.Text = User.curUser.curDiveSession.location_lon + " | " + User.curUser.curDiveSession.location_lat;
            tvwWeather.Text = User.curUser.curDiveSession.weatherCondition_main + " | " + User.curUser.curDiveSession.weatherTemperature;
            tvwTimeInWater.Text = User.curUser.curDiveSession.watertime + " sec";
            chartView = FindViewById<ChartView>(Resource.Id.chartview_divesession_detail);

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
            generateChart();
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

        private void generateChart()
        {
            List<ChartEntry> dataList = new List<ChartEntry>();

            dataList.Add(new ChartEntry(0)
            {
                Label = "0:05",
                ValueLabel = "0",
                Color = SKColor.Parse("#5cf739")
            });

            dataList.Add(new ChartEntry(5)
            {
                Label = "0:10",
                ValueLabel = "5",
                Color = SKColor.Parse("#f7c139")
            });

            dataList.Add(new ChartEntry(8)
            {
                Label = "0:15",
                ValueLabel = "8",
                Color = SKColor.Parse("#f75939")
            });

            dataList.Add(new ChartEntry(8)
            {
                Label = "0:20",
                ValueLabel = "8",
                Color = SKColor.Parse("#f75939")
            });

            dataList.Add(new ChartEntry(9)
            {
                Label = "0:25",
                ValueLabel = "9",
                Color = SKColor.Parse("#f75939")
            });

            dataList.Add(new ChartEntry(8)
            {
                Label = "0:30",
                ValueLabel = "8",
                Color = SKColor.Parse("#f75939")
            });

            dataList.Add(new ChartEntry(4)
            {
                Label = "0:35",
                ValueLabel = "4",
                Color = SKColor.Parse("f7c139")
            });

            dataList.Add(new ChartEntry(1)
            {
                Label = "0:40",
                ValueLabel = "4",
                Color = SKColor.Parse("#5cf739")
            });

            var chart = new LineChart { Entries = dataList, LabelTextSize = 30f };
            chartView.Chart = chart;
        }
    }
}