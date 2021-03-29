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
    /**
     *  This activity provides a detailed view for a single divesession, including general 
     *  data like the location, weather conditions and the total time in water. Besides that
     *  it displays a chart that contains all dives of the current divesession and displays the
     *  timestamp where the dive was started on the X-axis. On the Y-axis it displays the max 
     *  depth of that dive.
     **/
    [Activity(Label = "DiveSessionDetailViewActivity", ScreenOrientation = ScreenOrientation.Portrait)]
    public class DiveSessionDetailViewActivity : Activity
    {
        /*Member Variables*/
        private Button btnDivesPerSession;
        private ChartView chartView;
        private TextView tvwSessionName;
        private TextView tvwLocation;
        private TextView tvwWeather;
        private TextView tvwTimeInWater;

        private FirebaseDataListener diveDataListener;
        private FirebaseDataListener measurepointDataListener;
        private List<Measurepoint> measurepointList = new List<Measurepoint>();

        /**
         *  This function initializes all member variables such as UI components
         *  and the db listener to retrieve measurepoint data for the current divesession.
         **/
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.DiveSessionDetailViewPage);

            btnDivesPerSession = FindViewById<Button>(Resource.Id.btnDivesPerSession);
            btnDivesPerSession.Click += redirectToDivesPerSessionActivity;
            tvwSessionName = FindViewById<TextView>(Resource.Id.tvwDsdvSessionName);
            tvwLocation = FindViewById<TextView>(Resource.Id.tvwDsdvLocationV);
            tvwWeather = FindViewById<TextView>(Resource.Id.tvwDsdvWeatherV);
            tvwTimeInWater = FindViewById<TextView>(Resource.Id.tvwDsdvTimeInWaterV);
            chartView = FindViewById<ChartView>(Resource.Id.chartview_divesession_detail);

            //set the textfield values below the chart using the current selected divesession that was stored in the TemporaryData class
            tvwSessionName.Text = TemporaryData.CURRENT_DIVESESSION.date + " " + TemporaryData.CURRENT_DIVESESSION.location_lon + " | " + TemporaryData.CURRENT_DIVESESSION.location_lat;
            tvwLocation.Text = TemporaryData.CURRENT_DIVESESSION.location_lon + " | " + TemporaryData.CURRENT_DIVESESSION.location_lat;
            tvwWeather.Text = TemporaryData.CURRENT_DIVESESSION.weatherCondition_main + " | " + TemporaryData.CURRENT_DIVESESSION.weatherTemperature;
            tvwTimeInWater.Text = TemporaryData.CURRENT_DIVESESSION.watertime + " sec";
            
            RetrieveDiveData();
        }

        /**
         *  This function initializes the db listener and queries for all dives that 
         *  belong to the current selected divesession.
         **/
        private void RetrieveDiveData()
        {
            diveDataListener = new FirebaseDataListener();
            diveDataListener.QueryParameterized("dives", "ref_divesession", TemporaryData.CURRENT_DIVESESSION.Id);
            diveDataListener.DataRetrieved += DiveDataListener_DataRetrieved;
        }

        /**
         *  This function represents the event listener and sets the dive data retrieved from db to the current selected
         *  divesession. After that the chart is generated with that dive data.
         **/
        private void DiveDataListener_DataRetrieved(object sender, FirebaseDataListener.DataEventArgs e)
        {            
            TemporaryData.CURRENT_DIVESESSION.dives = e.Dives;
            generateChart();
        }

        /**
         *  This function redirects the user to the divesPerSessionActivity, where all dives of this session
         *  are presented inside a listview.
         **/
        private void redirectToDivesPerSessionActivity(object sender, EventArgs eventArgs)
        {
            var divesPerSessionActivity = new Intent(this, typeof(DivesPerSessionActivity));
            StartActivity(divesPerSessionActivity);
        }

        /**
         *  This function generates the chart that is populated with the data of all dives 
         *  of the current selected divesession.
         *  NOTE: this is a hardcoded example at the moment and needs to be compeletely implemented in future versions.
         **/
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