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
            textViewDepth.Text = User.curUser.curDive.maxDepth;
            textViewDuration.Text = User.curUser.curDive.duration;
            textViewMaxHF.Text = User.curUser.curDive.HeartFreqMax;
            textViewMinHF.Text = User.curUser.curDive.HeartFreqMin;
            textViewMaxOxy.Text = User.curUser.curDive.OxygenSaturationMax;
            textViewMinOxy.Text = User.curUser.curDive.OxygenSaturationMin;
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


            for (int i = 0; i < measurepointList.Count - 10; i += 10)
            {
                dataList.Add(new ChartEntry(i)
                {
                    Label = measurepointList[i].duration,
                    ValueLabel = measurepointList[i].depth,
                    Color = SKColor.Parse("#5cf739")
                });
            }            

            

            var chart = new LineChart { Entries = dataList, LabelTextSize = 30f };
            chartView.Chart = chart;
        }
    }
}