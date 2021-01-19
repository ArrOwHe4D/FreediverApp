using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using FreediverApp.DatabaseConnector;

namespace FreediverApp
{
    [Activity(Label = "SessionsActivity")]
    public class SessionsActivity : Activity
    {
        private List<string> dives = new List<string> { "23.5.2020", "24.5.2020", "25.5.2020", "26.5.2020" };
        private ListView lvwDive;
        private Button btnAdd;
        private EditText etSearch;
        private LinearLayout llContainer;
        int counter = 0;
        private FirebaseDataListener diveSessionsDataListener;
        private List<DiveSession> diveSessionList;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.SessionsPage);

            lvwDive = FindViewById<ListView>(Resource.Id.lvwDiveSessions);

            RetrieveDiveSessionData();            

            lvwDive.ItemClick += lvwDive_ItemClick;
            lvwDive.ItemLongClick += lvwDive_ItemLongClick;

            btnAdd = FindViewById<Button>(Resource.Id.btnAdd);
            btnAdd.Click += btnAdd_Click;


            //etSearch = FindViewById<EditText>(Resource.Id.etSuche);
            //etSearch.Alpha = 0;
            //llContainer = FindViewById<LinearLayout>(Resource.Id.llContainer);
        }

        private void RetrieveDiveSessionData()
        {
            diveSessionsDataListener = new FirebaseDataListener();
            diveSessionsDataListener.QueryParameterized("divesessions", "ref_user", User.curUser.id);
            diveSessionsDataListener.DataRetrieved += DiveSessionsDataListener_DataRetrieved;
        }

        private void DiveSessionsDataListener_DataRetrieved(object sender, FirebaseDataListener.DataEventArgs e)
        {
            diveSessionList = e.DiveSessions;
            fillDiveSessionData(diveSessionList);
        }

        private void fillDiveSessionData(List<DiveSession> diveSessions)
        {
            if (diveSessions != null)
            {
                dives = new List<string>();
                foreach (var item in diveSessionList)
                {
                    if (item.date != null)
                    {
                        dives.Add(item.date + " | " + item.location);
                    }
                }

                ArrayAdapter<string> adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleListItem1, dives);
                lvwDive.Adapter = adapter;
                User.curUser.diveSessions = diveSessions;
            }
        }

        protected override void OnResume()
        {
            counter++;
            base.OnResume();
        }

        void lvwDive_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            User.curUser.curDiveSession = User.curUser.diveSessions[e.Position];
            var diveSessionDetailViewActivity = new Intent(this, typeof(DiveSessionDetailViewActivity));
            StartActivity(diveSessionDetailViewActivity);
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