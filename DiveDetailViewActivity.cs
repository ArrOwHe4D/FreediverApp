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
        TextView tvwDepth;
        TextView tvwDuration;
        TextView tvwMaxHF;
        TextView tvwMinHF;
        TextView tvwMaxOxy;
        TextView tvwMinOxy;
        private ChartView chartView;
        private FirebaseDataListener measurepointDataListener;
        private List<Measurepoint> measurepointList;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.DiveDetailViewPage);
            RetrieveMeasurepointData(User.curUser.curDive);
            tvwDepth = FindViewById<TextView>(Resource.Id.tvwDdvDepthV);
            tvwDuration = FindViewById<TextView>(Resource.Id.tvwDdvDurationV);
            tvwMaxHF = FindViewById<TextView>(Resource.Id.tvwDdvMaxHeartFV);
            tvwMinHF = FindViewById<TextView>(Resource.Id.tvwDdvMinHeartFV);
            tvwMaxOxy = FindViewById<TextView>(Resource.Id.tvwDdvMaxOxyV);
            tvwMinOxy = FindViewById<TextView>(Resource.Id.tvwDdvMinOxyV);            
            chartView = FindViewById<ChartView>(Resource.Id.cvwDdvDiveDia);
            fillTextView();            
        }

        private void fillTextView()
        {
            tvwDepth.Text = User.curUser.curDive.maxDepth;
            tvwDuration.Text = User.curUser.curDive.duration;
            tvwMaxHF.Text = User.curUser.curDive.HeartFreqMax;
            tvwMinHF.Text = User.curUser.curDive.HeartFreqMin;
            tvwMaxOxy.Text = User.curUser.curDive.OxygenSaturationMax;
            tvwMinOxy.Text = User.curUser.curDive.OxygenSaturationMin;
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