using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using System;
using FreediverApp.DatabaseConnector;
using System.Collections.Generic;
using Microcharts;
using SkiaSharp;
using Microcharts.Droid;
using FreediverApp.Utils;
using FreediverApp.UI.Components;

namespace FreediverApp.FragmentActivities
{
    /**
     *  This Fragment represents the main menu that is being displayed after the user has logged in successfully.
     *  It displays a simple XML page that shows the app logo and prints a welcome message for the user.
     **/
    [Obsolete]
    public class MainFragment : Fragment
    {
        private TextView textViewWelcomeMessage;
        private TextView textViewTotalDiveSessionCount;
        private TextView textViewTotalWaterTime;
        private TextView textViewLongestDiveSession;
        private TextView textViewWarmestDiveSession;
        private TextView textViewColdestDiveSession;
        private ChartView chartView;

        private List<DiveSession> diveSessionList;
        private FirebaseDataListener diveSessionDataListener;

        private int longestSessionWaterTime;
        private int totalWaterTime;
        private float coldestWaterTemperature;
        private float warmestWaterTemperature;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        /**
         *  This function instantiates the UI components from XML and builds the welcome message for the user.
         *  In order to do this it utilizes the userdata that was saved to TemporaryData.
         **/
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.LandingPage, container, false);

            textViewWelcomeMessage = view.FindViewById<TextView>(Resource.Id.textview_welcome);
            textViewWelcomeMessage.Text = Context.GetString(Resource.String.welcome) + " " + TemporaryData.CURRENT_USER.username + " !";

            chartView = view.FindViewById<ChartView>(Resource.Id.chartview_divesession_statistic);

            //textViewTotalDiveSessionCount = view.FindViewById<TextView>(Resource.Id.textview_total_divesession_count);
            //textViewTotalWaterTime = view.FindViewById<TextView>(Resource.Id.textview_total_watertime);
            //textViewLongestDiveSession = view.FindViewById<TextView>(Resource.Id.textview_longest_divesession);
            //textViewWarmestDiveSession = view.FindViewById<TextView>(Resource.Id.textview_warmest_divesession);
            //textViewColdestDiveSession = view.FindViewById<TextView>(Resource.Id.textview_coldest_divesession);

            longestSessionWaterTime = 0;
            totalWaterTime = 0;
            coldestWaterTemperature = 1000.0f;
            warmestWaterTemperature = 0.0f;

            RetrieveDiveSessionData();

            return view;
        }

        private void RetrieveDiveSessionData()
        {
            diveSessionDataListener = new FirebaseDataListener();
            diveSessionDataListener.QueryParameterized("divesessions", "ref_user", TemporaryData.CURRENT_USER.id);
            diveSessionDataListener.DataRetrieved += DiveSessionDataListener_DataRetrieved;
        }

        private void DiveSessionDataListener_DataRetrieved(object sender, FirebaseDataListener.DataEventArgs e)
        {
            diveSessionList = e.DiveSessions;
            fillStatisticsView();
        }

        private void fillStatisticsView() 
        {
            if (sessionDataAcquired()) 
            {
                updateDiveSessionStatistics();

                SKColor valueLabelColor = FreediverHelper.darkModeActive(Context) ? SKColor.Parse("#8d9094") : SKColor.Parse("#000000");
                SKColor backgroundColor = FreediverHelper.darkModeActive(Context) ? SKColor.Parse("#1d1e1f") : SKColor.Parse("#ffffff");
                generateChart(valueLabelColor, backgroundColor);

                //textViewTotalDiveSessionCount.Text = "Anzahl bisheriger Tauchsessions: " + diveSessionList.Count;
                //textViewTotalWaterTime.Text        = "Gesamte Tauchzeit: " + totalWaterTime + " sec";
                //textViewLongestDiveSession.Text    = "Längste Tauchsession: " + longestSessionWaterTime + " sec";
                //textViewWarmestDiveSession.Text    = "Höchste Wassertemperatur: " + warmestWaterTemperature + " °C";
                //textViewColdestDiveSession.Text    = "Niedrigste Wassertemperatur: " + coldestWaterTemperature + " °C";
            }
        }

        private bool sessionDataAcquired() 
        {
            return diveSessionList != null;
        }

        private void updateDiveSessionStatistics() 
        {
            foreach (DiveSession session in diveSessionList)
            {
                int sessionWaterTime = int.Parse(session.watertime);
                float sessionTemperature = float.Parse(session.weatherTemperature);
                totalWaterTime += sessionWaterTime;

                //Longest Divesession
                if (sessionWaterTime > longestSessionWaterTime) 
                {
                    longestSessionWaterTime = sessionWaterTime;
                }

                //Warmest Divesession
                if (sessionTemperature > warmestWaterTemperature) 
                {
                    warmestWaterTemperature = sessionTemperature;
                }

                //Coldest Divesession
                if (sessionTemperature < coldestWaterTemperature) 
                {
                    coldestWaterTemperature = sessionTemperature;
                }
            }
        }

        private void generateChart(SKColor valueLabelColor, SKColor backgroundColor) 
        {
            if (sessionDataAcquired()) 
            {
                List<ChartEntry> dataList = new List<ChartEntry>();

                //Add a new chartEntry to the dataList containing statistic values 
                dataList.Add(new ChartEntry(diveSessionList.Count)
                {
                    Label = "Anzahl Sessions",
                    ValueLabel = diveSessionList.Count.ToString(),
                    ValueLabelColor = valueLabelColor,
                    Color = SKColor.Parse("#03f8fc") //aqua blue
                });

                dataList.Add(new ChartEntry(totalWaterTime)
                {
                    Label = "Zeit unter Wasser",
                    ValueLabel = totalWaterTime.ToString() + " sec",
                    ValueLabelColor = valueLabelColor,
                    Color = SKColor.Parse("#032cfc") //blue
                });

                dataList.Add(new ChartEntry(longestSessionWaterTime)
                {
                    Label = "Längste Session",
                    ValueLabel = longestSessionWaterTime.ToString() + " sec",
                    ValueLabelColor = valueLabelColor,
                    Color = SKColor.Parse("#03fc5e") //mint green
                });

                dataList.Add(new ChartEntry(warmestWaterTemperature)
                {
                    Label = "Max Temp",
                    ValueLabel = warmestWaterTemperature.ToString() + " °C",
                    ValueLabelColor = valueLabelColor,
                    Color = SKColor.Parse("#fc6f03") //sunny orange
                });

                dataList.Add(new ChartEntry(coldestWaterTemperature)
                {
                    Label = "Min Temp",
                    ValueLabel = coldestWaterTemperature.ToString() + " °C",
                    ValueLabelColor = valueLabelColor,
                    Color = SKColor.Parse("#69c8ff") //ice blue
                });

                //create a new chart with the data entries and a custom display configuration
                var chart = new BarChart 
                { 
                    Entries = dataList, 
                    LabelTextSize = 20f, 
                    LabelOrientation = 
                    Microcharts.Orientation.Horizontal, 
                    ValueLabelOrientation = Microcharts.Orientation.Horizontal, 
                    BackgroundColor = backgroundColor
                };
                chartView.Chart = chart;
            }
        }
    }
}