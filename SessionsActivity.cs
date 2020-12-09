using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace FreediverApp
{
    [Activity(Label = "SessionsActivity")]
    public class SessionsActivity : Activity
    {
        private List<string> dives;
        private ListView lvwDive;
        private Button btnAdd;
        private EditText etSearch;
        private LinearLayout llContainer;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.SessionsPage);

            lvwDive = FindViewById<ListView>(Resource.Id.lvwDiveSessions);


            dives = new List<string> { "23.5.2020" , "24.5.2020" , "25.5.2020" , "26.5.2020" , "27.5.2020" , "28.5.2020"  , "29.5.2020"  , "30.5.2020"  , "1.6.2020",
            "2.7.2020", "12.7.2020", "13.7.2020", "15.7.2020", "16.7.2020", "16.7.2020", "19.7.2020",  "20.7.2020", "23.7.2020", "23.7.2020"};


            ArrayAdapter<string> adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleListItem1, dives);
            lvwDive.Adapter = adapter;

            lvwDive.ItemClick += lvwDive_ItemClick;
            lvwDive.ItemLongClick += lvwDive_ItemLongClick;

            btnAdd = FindViewById<Button>(Resource.Id.btnAdd);
            btnAdd.Click += btnAdd_Click;

            //etSearch = FindViewById<EditText>(Resource.Id.etSuche);
            //etSearch.Alpha = 0;
            //llContainer = FindViewById<LinearLayout>(Resource.Id.llContainer);
        }

        void lvwDive_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            var divesPerSessionActivity = new Intent(this, typeof(DivesPerSessionActivity));
            StartActivity(divesPerSessionActivity);
        }

        void lvwDive_ItemLongClick(object sender, AdapterView.ItemLongClickEventArgs e)
        {

        }

        void btnAdd_Click(object sender, EventArgs eventArgs)
        {
            var addSessionActivity = new Intent(this, typeof(AddSessionActivity));
            StartActivity(addSessionActivity);
        }
    }
}