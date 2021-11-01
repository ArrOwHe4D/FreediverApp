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
using System.Globalization;

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
        private ChartView chartView;

        private List<DiveSession> diveSessionList;
        private FirestoreDataListener diveSessionDataListener;

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
            textViewWelcomeMessage.Text = Context.GetString(Resource.String.welcome) + " " + TemporaryData.CURRENT_USER.firstname + " !";

            chartView = view.FindViewById<ChartView>(Resource.Id.chartview_divesession_statistic);

            longestSessionWaterTime = 0;
            totalWaterTime = 0;
            coldestWaterTemperature = 1000.0f;
            warmestWaterTemperature = 0.0f;

            RetrieveDiveSessionData();
            return view;
        }

        private void RetrieveDiveSessionData()
        {
            diveSessionDataListener = new FirestoreDataListener();
            diveSessionDataListener.QueryParameterized("divesessions", "ref_user", TemporaryData.CURRENT_USER.id);
            diveSessionDataListener.DataRetrieved += DiveSessionDataListener_DataRetrieved;
        }

        private void DiveSessionDataListener_DataRetrieved(object sender, FirestoreDataListener.DataEventArgs e)
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
                try
                {
                    float sessionTemperature;
                    CultureInfo cultureInfo = (CultureInfo)CultureInfo.CurrentCulture.Clone();

                    if (cultureInfo.Name == "de-DE")
                    {
                        sessionTemperature = string.IsNullOrEmpty(session.weatherTemperature) ? 0.0f : float.Parse(session.weatherTemperature.Replace(".", ","), cultureInfo);
                    }
                    else
                    {
                        sessionTemperature = string.IsNullOrEmpty(session.weatherTemperature) ? 0.0f : float.Parse(session.weatherTemperature, cultureInfo);
                    }

                    int sessionWaterTime = int.Parse(session.watertime);
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
                        if (!string.IsNullOrEmpty(session.weatherTemperature)) 
                        {
                            coldestWaterTemperature = sessionTemperature;
                        }
                    }
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex);
                    coldestWaterTemperature = 0.0f;
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
                    Label = Context.Resources.GetString(Resource.String.session_count),
                    ValueLabel = diveSessionList.Count.ToString(),
                    ValueLabelColor = valueLabelColor,
                    Color = SKColor.Parse("#03f8fc") //aqua blue
                });

                int min = totalWaterTime / 60;
                int sec = totalWaterTime % 60;
                float twt = min + (float)sec / 100;

                dataList.Add(new ChartEntry(twt)
                {
                    Label = Context.Resources.GetString(Resource.String.time_under_water),
                    ValueLabel = twt.ToString() + " min",
                    ValueLabelColor = valueLabelColor,
                    Color = SKColor.Parse("#032cfc") //blue
                });

                min = longestSessionWaterTime / 60;
                sec = longestSessionWaterTime % 60;
                float lswt = min + (float)sec / 100;

                dataList.Add(new ChartEntry(lswt)
                {
                    Label = Context.Resources.GetString(Resource.String.longest_session),
                    ValueLabel = lswt.ToString() + " min",
                    ValueLabelColor = valueLabelColor,
                    Color = SKColor.Parse("#03fc5e") //mint green
                });

                dataList.Add(new ChartEntry(warmestWaterTemperature)
                {
                    Label = Context.Resources.GetString(Resource.String.max_temperature),
                    ValueLabel = warmestWaterTemperature.ToString() + " °C",
                    ValueLabelColor = valueLabelColor,
                    Color = SKColor.Parse("#fc6f03") //sunny orange
                });

                dataList.Add(new ChartEntry(coldestWaterTemperature)
                {
                    Label = Context.Resources.GetString(Resource.String.min_temperature),
                    ValueLabel = coldestWaterTemperature.ToString() + " °C",
                    ValueLabelColor = valueLabelColor,
                    Color = SKColor.Parse("#69c8ff") //ice blue
                });

                //create a new chart with the data entries and a custom display configuration
                var chart = new RadarChart 
                { 
                    Entries = dataList, 
                    LabelTextSize = 20f, 
                    BackgroundColor = backgroundColor
                };
                chartView.Chart = chart;
                chartView.Chart.Margin = 140;
            }
        }
    }
}