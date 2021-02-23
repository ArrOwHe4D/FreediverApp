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
        TextView textViewDiveSessionTitle;
        TextView textViewGraphTitle;
        TextView textViewDepth;
        TextView textViewDuration;
        TextView textViewMaxHF;
        TextView textViewMinHF;
        TextView textViewMaxOxy;
        TextView textViewMinOxy;
        private ChartView chartView;
        private FirebaseDataListener measurepointDataListener;
        private List<Measurepoint> measurepointList;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.DiveDetailViewPage);
            RetrieveMeasurepointData(User.curUser.curDive);
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
            textViewGraphTitle.Text = "Last Dive (" + User.curUser.curDive.maxDepth + "m deep and " + User.curUser.curDive.duration + "sec long)";
            textViewDepth.Text = User.curUser.curDive.maxDepth + " m";
            textViewDuration.Text = User.curUser.curDive.duration + " sec";
            textViewMaxHF.Text = User.curUser.curDive.HeartFreqMax + " bpm";
            textViewMinHF.Text = User.curUser.curDive.HeartFreqMin + " bpm";
            textViewMaxOxy.Text = User.curUser.curDive.OxygenSaturationMax + " %";
            textViewMinOxy.Text = User.curUser.curDive.OxygenSaturationMin + " %";
        }

        private void RetrieveMeasurepointData(Dive d)
        {
            measurepointDataListener = new FirebaseDataListener();
            measurepointDataListener.QueryParameterized("measurepoints", "ref_dive", d.Id);
            measurepointDataListener.DataRetrieved += MeasurepointDataListener_DataRetrieved;
        }
        private void MeasurepointDataListener_DataRetrieved(object sender, FirebaseDataListener.DataEventArgs e)
        {
            User.curUser.curDive.measurepoints = e.Measurepoints;
            measurepointList = e.Measurepoints;
            generateChart();
        }

        private void generateChart()
        {
            List<ChartEntry> dataList = new List<ChartEntry>();


            for (int i = 0; i < measurepointList.Count - 6; i += 6)
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