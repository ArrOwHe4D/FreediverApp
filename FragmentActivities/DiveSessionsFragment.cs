using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using FreediverApp.DatabaseConnector;

namespace FreediverApp
{
    [Obsolete]
    public class DiveSessionsFragment : Fragment
    {
        private List<string> dives;
        private ListView listViewDives;
        private Button buttonAdd;
        private FirebaseDataListener diveSessionsDataListener;
        private List<DiveSession> diveSessionList;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.SessionsPage, container, false);

            listViewDives = view.FindViewById<ListView>(Resource.Id.lvwDiveSessions);

            RetrieveDiveSessionData();

            listViewDives.ItemClick += lvwDive_ItemClick;

            buttonAdd = view.FindViewById<Button>(Resource.Id.btnAdd);
            buttonAdd.Click += buttonAdd_Click;

            return view;
        }

        private void RetrieveDiveSessionData()
        {
            diveSessionsDataListener = new FirebaseDataListener();
            diveSessionsDataListener.QueryParameterized("divesessions", "ref_user", TemporaryData.CURRENT_USER.id);
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
                        dives.Add(item.date + " | " + item.location_lon + " / " + item.location_lat);
                    }
                }

                ArrayAdapter<string> adapter = new ArrayAdapter<string>(Context, Android.Resource.Layout.SimpleListItem1, dives);
                listViewDives.Adapter = adapter;
                TemporaryData.CURRENT_USER.diveSessions = diveSessions;
            }
        }

        void lvwDive_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            TemporaryData.CURRENT_DIVESESSION = TemporaryData.CURRENT_USER.diveSessions[e.Position];
            var diveSessionDetailViewActivity = new Intent(Context, typeof(DiveSessionDetailViewActivity));
            StartActivity(diveSessionDetailViewActivity);
        }

        void buttonAdd_Click(object sender, EventArgs eventArgs)
        {
            var addSessionActivity = new Intent(Context, typeof(AddSessionActivity));
            StartActivity(addSessionActivity);
        }
    }
}