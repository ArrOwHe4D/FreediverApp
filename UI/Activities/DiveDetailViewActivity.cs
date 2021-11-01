using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Widget;
using FreediverApp.DatabaseConnector;
using FreediverApp.Utils;
using Microcharts;
using Microcharts.Droid;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace FreediverApp
{
    /**
     *  This Activity displays the detail view of a single dive that belongs to a divesession. 
     *  It also displays all relevant data like max depth, max heart frequency etc. inside a cardview
     *  component. Above the cardview a chart is generated with displays measurepoints form a dive
     *  as datapoints. For the chart generation we use the Microcharts Nuget package.
     **/
    [Activity(Label = "DiveDetailViewActivity", ScreenOrientation = ScreenOrientation.Portrait)]
    public class DiveDetailViewActivity : Activity
    {
        /*Member Variables (UI components from XML)*/
        private TextView textViewDiveSessionTitle;
        private TextView textViewGraphTitle;
        private TextView textViewDepth;
        private TextView textViewDuration;
        private TextView textViewMaxHF;
        private TextView textViewMinHF;
        private TextView textViewMaxOxy;
        private TextView textViewMinOxy;
        private ChartView chartView;

        private FirestoreDataListener measurepointDataListener;
        private List<Measurepoint> measurepointList;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

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

        /**
         *  This function fills all the textfields inside the cardview to display 
         *  all relevant informations from the current dive. It also adds the correct
         *  measure units because the measurepoint data is processed as raw data without 
         *  any specified measure units.
         **/
        private void fillTextView()
        {
            textViewDiveSessionTitle.Text = Resources.GetString(Resource.String.dive) + " #" + Intent.GetStringExtra("index");
            textViewGraphTitle.Text = Resources.GetString(Resource.String.last_dive) + " (" + TemporaryData.CURRENT_DIVE.maxDepth + " m " + Resources.GetString(Resource.String.deep_and) + " " + TemporaryData.CURRENT_DIVE.duration + " sec " + Resources.GetString(Resource.String.str_long) + ")";
            textViewDepth.Text = TemporaryData.CURRENT_DIVE.maxDepth + " m";
            textViewDuration.Text = TemporaryData.CURRENT_DIVE.duration + " sec";
            textViewMaxHF.Text = TemporaryData.CURRENT_DIVE.HeartFreqMax + " bpm";
            textViewMinHF.Text = TemporaryData.CURRENT_DIVE.HeartFreqMin + " bpm";
            textViewMaxOxy.Text = TemporaryData.CURRENT_DIVE.OxygenSaturationMax + " %";
            textViewMinOxy.Text = TemporaryData.CURRENT_DIVE.OxygenSaturationMin + " %";
        }

        /**
         *  This function initializes the db listener and queries for all measurepoints that have 
         *  a reference (ref_dive) to the current dive.
         **/
        private void RetrieveMeasurepointData(Dive dive)
        {
            measurepointDataListener = new FirestoreDataListener();
            measurepointDataListener.QueryParameterizedOrderBy("measurepoints", "ref_dive", dive.id, "duration");
            measurepointDataListener.DataRetrieved += MeasurepointDataListener_DataRetrieved;
        }

        /**
         *  Setup the event for the db listener. Set the datalists to the retrieved data and generate the 
         *  chart based on the retrieved measurepoints for this dive.
         **/
        private void MeasurepointDataListener_DataRetrieved(object sender, FirestoreDataListener.DataEventArgs e)
        {
            TemporaryData.CURRENT_DIVE.measurepoints = e.Measurepoints;
            measurepointList = e.Measurepoints;
            SKColor valueLabelColor = FreediverHelper.darkModeActive(this) ? SKColor.Parse("#8d9094") : SKColor.Parse("#000000");
            SKColor backgroundColor = FreediverHelper.darkModeActive(this) ? SKColor.Parse("#1d1e1f") : SKColor.Parse("#ffffff");
            generateChart(valueLabelColor, backgroundColor);
        }

        /**
         *  This function generates the chart based on the measurepoint data that was retrieved from the db listener. 
         *  The duration in seconds is displayed on the X-axis and the depth on the Y-axis.
         **/
        private void generateChart(SKColor valueLabelColor, SKColor backgroundColor)
        {
            List<ChartEntry> dataList = new List<ChartEntry>();

            int hop = measurepointList.Count / 10;

            List<Measurepoint> scaledMeasurePoints = new List<Measurepoint>();

            for (int i = 0; i < measurepointList.Count; i += hop) 
            {
                scaledMeasurePoints.Add(measurepointList[i]);
            }

            scaledMeasurePoints.Sort((x, y) => long.Parse(x.duration).CompareTo(long.Parse(y.duration)));

            for (int i = 0; i < scaledMeasurePoints.Count; i++)
            {
                SKColor color;
                float depth = 0.0f;

                CultureInfo cultureInfo = (CultureInfo)CultureInfo.CurrentCulture.Clone();

                if (cultureInfo.Name == "de-DE")
                {
                    depth = scaledMeasurePoints[i].depth.Length > 1 ? float.Parse(scaledMeasurePoints[i].depth.Substring(0, 5).Replace(".", ","), cultureInfo)
                                                                    : float.Parse(scaledMeasurePoints[i].depth.Replace(".", ","), cultureInfo);
                }
                else 
                {
                    depth = scaledMeasurePoints[i].depth.Length > 1 ? float.Parse(scaledMeasurePoints[i].depth.Substring(0, 5), cultureInfo)
                                                                    : float.Parse(scaledMeasurePoints[i].depth, cultureInfo);
                }

                //assign the color based on the depth value of the current measurepoint
                if (depth <= 20.0f)
                {
                    color = SKColor.Parse("#5cf739"); //green
                }
                else if (depth <= 60.0f)
                {
                    color = SKColor.Parse("#f7c139"); //yellow
                }
                else 
                {
                    color = SKColor.Parse("#f75939"); //red
                }

                TimeSpan ts = TimeSpan.FromMilliseconds(long.Parse(scaledMeasurePoints[i].duration));

                //Add a new chartEntry to the dataList containing the depth value of the current measurepoint
                dataList.Add(new ChartEntry(depth)
                {
                    Label = ts.ToString(@"mm\:ss"),
                    ValueLabel = depth.ToString() + " m",
                    ValueLabelColor = valueLabelColor,
                    Color = color
                });
            }

            //create a new chart with the data entries and a custom display configuration
            var chart = new LineChart 
            { 
                Entries = dataList, 
                LabelTextSize = 20f, 
                LabelOrientation = Microcharts.Orientation.Horizontal, 
                ValueLabelOrientation = Microcharts.Orientation.Horizontal,
                BackgroundColor = backgroundColor
            };
            chartView.Chart = chart;
        }
    }
}