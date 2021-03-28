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
    [Activity(Label = "DivesPerSessionActivity", ScreenOrientation = ScreenOrientation.Portrait)]
    public class DivesPerSessionActivity : Activity
    {
        private ListView lvwDive;
        private TextView tvwDive;
        List<string> dives;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.DivesPerSessionPage);

            lvwDive = FindViewById<ListView>(Resource.Id.lvwDpsDives);
            tvwDive = FindViewById<TextView>(Resource.Id.tvwDpsDives);

            lvwDive.ItemClick += lvwDive_ItemClick;

            fillLvw();
        }

        void fillLvw()
        {
            dives = new List<string>();
            int count = 0;
            try
            {
                foreach (var item in TemporaryData.CURRENT_DIVESESSION.dives)
                {
                    if (item.duration != null)
                    {
                        count++;
                        dives.Add("Tauchgang " + count + " | " + Convert.ToDouble(item.duration) + "sec. | " + item.maxDepth + "m");
                    }
                }
                ArrayAdapter<string> adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleListItem1, dives);
                lvwDive.Adapter = adapter;
            }
            catch (Exception e)
            {
                Toast.MakeText(this, Resource.String.no_dives_available, ToastLength.Long).Show();
            }
        }

        private void lvwDive_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            try
            {
                TemporaryData.CURRENT_DIVE = TemporaryData.CURRENT_DIVESESSION.dives[e.Position];
                var addDiveDetailViewActivity = new Intent(this, typeof(DiveDetailViewActivity));

                int index = e.Position;
                index++;

                addDiveDetailViewActivity.PutExtra("index", index.ToString());
                StartActivity(addDiveDetailViewActivity);
            }
            catch (Exception)
            {
                
            }
        }    
    }
}