using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Widget;
using FreediverApp.DatabaseConnector;
using System;
using System.Collections.Generic;

namespace FreediverApp
{
    /**
     *  This activity holds the list of dives that belong to a selected divesession.
     *  Dives are displayed inside a listview and can be selected to open the detailView
     *  of a single dive.
     **/
    [Activity(Label = "DivesPerSessionActivity", ScreenOrientation = ScreenOrientation.Portrait)]
    public class DivesPerSessionActivity : Activity
    {
        private ListView lvwDive;
        private TextView tvwDive;
        List<string> dives;

        /**
         *  This function initializes the listview and sets the eventlistener for a clicked event inside the listview. 
         **/
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.DivesPerSessionPage);

            lvwDive = FindViewById<ListView>(Resource.Id.lvwDpsDives);
            tvwDive = FindViewById<TextView>(Resource.Id.tvwDpsDives);

            lvwDive.ItemClick += lvwDive_ItemClick;

            fillListView();
        }

        /**
         *  Fill the listview with the data of the dives inside the current divesession
         *  that was stored in TemporaryData when a divesession was clicked inside the divesession listview.
         **/
        void fillListView()
        {
            dives = new List<string>();
            int count = 0;
            try
            {
                //iterate over all dives and build a title string that is used to populate the listview.
                foreach (var item in TemporaryData.CURRENT_DIVESESSION.dives)
                {
                    if (item.duration != null)
                    {
                        count++;
                        dives.Add(Resources.GetString(Resource.String.dive) + " " + count + " | " + Convert.ToDouble(item.duration) + " sec. | " + item.maxDepth + " m");
                    }
                }
                ArrayAdapter<string> adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleListItem1, dives);
                lvwDive.Adapter = adapter;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Toast.MakeText(this, Resource.String.no_dives_available, ToastLength.Long).Show();
            }
        }

        /**
         *  This function handles the logic when a item inside the listview was clicked by the user. 
         **/
        private void lvwDive_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            try
            {
                //set the current selected dive to the dive at the clicked position inside the listview.
                TemporaryData.CURRENT_DIVE = TemporaryData.CURRENT_DIVESESSION.dives[e.Position];
                var addDiveDetailViewActivity = new Intent(this, typeof(DiveDetailViewActivity));

                int index = e.Position;
                index++;

                //pass the incremented index to the next activity that will be shown to enumerate the dive correctly ("Dive #1" insted of "Dive #0")
                addDiveDetailViewActivity.PutExtra("index", index.ToString());
                StartActivity(addDiveDetailViewActivity);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }    
    }
}