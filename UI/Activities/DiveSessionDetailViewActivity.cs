using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using Android.Widget;
using FreediverApp.DatabaseConnector;
using FreediverApp.Utils;
using Microcharts;
using Microcharts.Droid;
using SkiaSharp;
using SupportV7 = Android.Support.V7.App;

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
        private TextView textviewSessionName;
        private TextView textviewLocation;
        private TextView textviewWeather;
        private TextView textviewWaterTime;
        private TextView textviewNotes;

        private ImageView buttonEditNotes;

        private FirestoreDataListener diveDataListener;

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
            textviewSessionName = FindViewById<TextView>(Resource.Id.tvwDsdvSessionName);
            textviewLocation = FindViewById<TextView>(Resource.Id.tvwDsdvLocationV);
            textviewWeather = FindViewById<TextView>(Resource.Id.tvwDsdvWeatherV);
            textviewWaterTime = FindViewById<TextView>(Resource.Id.tvwDsdvTimeInWaterV);
            textviewNotes = FindViewById<TextView>(Resource.Id.textview_notes);

            buttonEditNotes = FindViewById<ImageView>(Resource.Id.button_edit_notes);
            buttonEditNotes.Click += buttonEditNotesOnClick;

            chartView = FindViewById<ChartView>(Resource.Id.chartview_divesession_detail);

            //set the textfield values below the chart using the current selected divesession that was stored in the TemporaryData class
            textviewSessionName.Text = TemporaryData.CURRENT_DIVESESSION.date + "\n" + (TemporaryData.CURRENT_DIVESESSION.location_locality == null ? 
                TemporaryData.CURRENT_DIVESESSION.location_lat + " | " + TemporaryData.CURRENT_DIVESESSION.location_lon : TemporaryData.CURRENT_DIVESESSION.location_locality);
           
            textviewLocation.Text = TemporaryData.CURRENT_DIVESESSION.location_lon + " | " + TemporaryData.CURRENT_DIVESESSION.location_lat + "\n\n" + (TemporaryData.CURRENT_DIVESESSION.location_locality == null ?
                TemporaryData.CURRENT_DIVESESSION.location_lat + " | " + TemporaryData.CURRENT_DIVESESSION.location_lon : TemporaryData.CURRENT_DIVESESSION.location_locality);
            
            textviewWeather.Text = TemporaryData.CURRENT_DIVESESSION.weatherCondition_main + " | " + TemporaryData.CURRENT_DIVESESSION.weatherTemperature + " °C";
            
            textviewWaterTime.Text = TemporaryData.CURRENT_DIVESESSION.watertime + " sec";

            textviewNotes.Text = TemporaryData.CURRENT_DIVESESSION.note;
            
            RetrieveDiveData();
        }

        /**
         *  This function initializes the db listener and queries for all dives that 
         *  belong to the current selected divesession.
         **/
        private void RetrieveDiveData()
        {
            diveDataListener = new FirestoreDataListener();
            diveDataListener.QueryParameterizedOrderBy("dives", "ref_divesession", TemporaryData.CURRENT_DIVESESSION.Id, "timestamp_begin");
            diveDataListener.DataRetrieved += DiveDataListener_DataRetrieved;
        }

        /**
         *  This function represents the event listener and sets the dive data retrieved from db to the current selected
         *  divesession. After that the chart is generated with that dive data.
         **/
        private void DiveDataListener_DataRetrieved(object sender, FirestoreDataListener.DataEventArgs e)
        {            
            TemporaryData.CURRENT_DIVESESSION.dives = e.Dives;
            SKColor valueLabelColor = FreediverHelper.darkModeActive(this) ? SKColor.Parse("#8d9094") : SKColor.Parse("#000000");
            SKColor backgroundColor = FreediverHelper.darkModeActive(this) ? SKColor.Parse("#1d1e1f") : SKColor.Parse("#ffffff");
            generateChart(valueLabelColor, backgroundColor);
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
        private void generateChart(SKColor valueLabelColor, SKColor backgroundColor)
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
                        ValueLabelColor = valueLabelColor,
                        Color = color
                    });
                }
            } 

            //create a new chart with the data entries and a custom display configuration
            var chart = new BarChart 
            { 
                Entries = dataList, 
                LabelTextSize = 20f, 
                LabelOrientation = Microcharts.Orientation.Horizontal, 
                ValueLabelOrientation = Microcharts.Orientation.Horizontal,
                BackgroundColor = backgroundColor
            };
            chartView.Chart = chart;
        }

        private void buttonEditNotesOnClick(object sender, EventArgs eventArgs) 
        {
            LayoutInflater layoutInflater = LayoutInflater.From(this);
            View dialogView = layoutInflater.Inflate(Resource.Layout.UserInputDialog, null);

            SupportV7.AlertDialog.Builder dialogBuilder = createEditDialog("Notiz hinzufügen", "Notiz", Resource.Drawable.icon_pencil, dialogView);

            var editValueField = dialogView.FindViewById<EditText>(Resource.Id.textfield_input);

            dialogBuilder.SetCancelable(false)
                .SetPositiveButton(Resource.String.dialog_save, delegate
                {
                    diveDataListener.updateEntity("divesessions", TemporaryData.CURRENT_DIVESESSION.key, "note", editValueField.Text);
                    textviewNotes.Text = editValueField.Text;
                    TemporaryData.CURRENT_DIVESESSION.note = editValueField.Text;
                    Toast.MakeText(this, Resource.String.saving_successful, ToastLength.Long).Show();
                    dialogBuilder.Dispose();
                })
                .SetNegativeButton(Resource.String.dialog_cancel, delegate
                {
                    dialogBuilder.Dispose();
                });

            SupportV7.AlertDialog dialog = dialogBuilder.Create();
            dialog.Show();
        }

        private SupportV7.AlertDialog.Builder createEditDialog(string title, string placeholder, int iconId, View parentView)
        {
            SupportV7.AlertDialog.Builder dialogBuilder = new SupportV7.AlertDialog.Builder(this);
            dialogBuilder.SetView(parentView);

            dialogBuilder.SetTitle(title);
            dialogBuilder.SetIcon(iconId);

            var editValueField = parentView.FindViewById<EditText>(Resource.Id.textfield_input);
            editValueField.Text = textviewNotes.Text;
            editValueField.Hint = placeholder;

            return dialogBuilder;
        }
    }
}