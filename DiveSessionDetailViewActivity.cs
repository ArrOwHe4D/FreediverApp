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
            tvwSessionName.Text = TemporaryData.CURRENT_DIVESESSION.date + "\n" + TemporaryData.CURRENT_DIVESESSION.location_lon + " | " + TemporaryData.CURRENT_DIVESESSION.location_lat;
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
         *  This function generates a bar chart that is populated with the data of all dives 
         *  of the current selected divesession. It displays the timestamp where the dive was started on the 
         *  bottom and the max depth of the dive on the top.
         **/
        private void generateChart()
        {
            List<ChartEntry> dataList = new List<ChartEntry>();
            if (TemporaryData.CURRENT_DIVESESSION.dives != null)
            {
                foreach (Dive dive in TemporaryData.CURRENT_DIVESESSION.dives)
                {
                    SKColor color = SKColor.Parse("#038cfc"); //aqua blue

                    //Add a new chartEntry to the dataList containing the depth value of the current measurepoint
                    dataList.Add(new ChartEntry(float.Parse(dive.maxDepth))
                    {
                        Label = dive.timestampBegin,
                        ValueLabel = dive.maxDepth.Split(",")[0] + " m",
                        Color = color
                    });
                }
            }
            

            //create a new chart with the data entries and a custom display configuration
            var chart = new BarChart { Entries = dataList, LabelTextSize = 20f, LabelOrientation = Microcharts.Orientation.Horizontal, ValueLabelOrientation = Microcharts.Orientation.Horizontal };
            chartView.Chart = chart;
        }
    }
}