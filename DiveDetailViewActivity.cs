using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Widget;
using FreediverApp.DatabaseConnector;
using Microcharts;
using Microcharts.Droid;
using SkiaSharp;
using System.Collections.Generic;

namespace FreediverApp
{
    [Activity(Label = "DiveDetailViewActivity", ScreenOrientation = ScreenOrientation.Portrait)]
    public class DiveDetailViewActivity : Activity
    {
        private TextView textViewDiveSessionTitle;
        private TextView textViewGraphTitle;
        private TextView textViewDepth;
        private TextView textViewDuration;
        private TextView textViewMaxHF;
        private TextView textViewMinHF;
        private TextView textViewMaxOxy;
        private TextView textViewMinOxy;
        private ChartView chartView;
        private FirebaseDataListener measurepointDataListener;
        private List<Measurepoint> measurepointList;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.DiveDetailViewPage);
            RetrieveMeasurepointData(TemporaryData.CURRENT_DIVE);
            textViewDiveSessionTitle = FindViewById<TextView>(Resource.Id.diveDetailViewSessionName);
            textViewGraphTitle = FindViewById<TextView>(Resource.Id.textViewGraphTitle);
            textViewDepth = FindViewById<TextView>(Resource.Id.tvwDdvDepthV);
            textViewDuration = FindViewById<TextView>(Resource.Id.tvwDdvDurationV);
            textViewMaxHF = FindViewById<TextView>(Resource.Id.tvwDdvMaxHeartFV);
            textViewMinHF = FindViewById<TextView>(Resource.Id.tvwDdvMinHeartFV);
            textViewMaxOxy = FindViewById<TextView>(Resource.Id.tvwDdvMaxOxyV);
            textViewMinOxy = FindViewById<TextView>(Resource.Id.tvwDdvMinOxyV);            
            chartView = FindViewById<ChartView>(Resource.Id.cvwDdvDiveDia);
            fillTextView();            
        }

        private void fillTextView()
        {
            textViewDiveSessionTitle.Text = "Tauchgang #" + Intent.GetStringExtra("index");
            textViewGraphTitle.Text = "Last Dive (" + TemporaryData.CURRENT_DIVE.maxDepth + " m deep and " + TemporaryData.CURRENT_DIVE.duration + " sec long)";
            textViewDepth.Text = TemporaryData.CURRENT_DIVE.maxDepth + " m";
            textViewDuration.Text = TemporaryData.CURRENT_DIVE.duration + " sec";
            textViewMaxHF.Text = TemporaryData.CURRENT_DIVE.HeartFreqMax + " bpm";
            textViewMinHF.Text = TemporaryData.CURRENT_DIVE.HeartFreqMin + " bpm";
            textViewMaxOxy.Text = TemporaryData.CURRENT_DIVE.OxygenSaturationMax + " %";
            textViewMinOxy.Text = TemporaryData.CURRENT_DIVE.OxygenSaturationMin + " %";
        }

        private void RetrieveMeasurepointData(Dive dive)
        {
            measurepointDataListener = new FirebaseDataListener();
            measurepointDataListener.QueryParameterized("measurepoints", "ref_dive", dive.id);
            measurepointDataListener.DataRetrieved += MeasurepointDataListener_DataRetrieved;
        }

        private void MeasurepointDataListener_DataRetrieved(object sender, FirebaseDataListener.DataEventArgs e)
        {
            TemporaryData.CURRENT_DIVE.measurepoints = e.Measurepoints;
            measurepointList = e.Measurepoints;
            generateChart();
        }

        private void generateChart()
        {
            List<ChartEntry> dataList = new List<ChartEntry>();

            int hop = measurepointList.Count / 10;

            for (int i = 0; i < measurepointList.Count; i += hop)
            {
                SKColor color;

                if (float.Parse(measurepointList[i].depth) <= 8.0f)
                {
                    color = SKColor.Parse("#5cf739");
                }
                else if (float.Parse(measurepointList[i].depth) <= 18.0f)
                {
                    color = SKColor.Parse("#f7c139");
                }
                else 
                {
                    color = SKColor.Parse("#f75939");
                }

                dataList.Add(new ChartEntry(float.Parse(measurepointList[i].depth))
                {
                    Label = int.Parse(measurepointList[i].duration.Split(",")[0]) < 10 ? "0:0" + measurepointList[i].duration.Split(",")[0] : "0:" + measurepointList[i].duration.Split(",")[0],
                    ValueLabel = measurepointList[i].depth.Split(",")[0] + "m",
                    Color = color
                });
            }                  

            var chart = new LineChart { Entries = dataList, LabelTextSize = 20f, LabelOrientation = Microcharts.Orientation.Horizontal, ValueLabelOrientation = Microcharts.Orientation.Horizontal };
            chartView.Chart = chart;
        }
    }
}