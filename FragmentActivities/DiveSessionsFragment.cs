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
    /**
     *  This fragment contains the view with a list of all divesessions from the current user. It also provides 
     *  a add button to create a new divesession inside a other activity. A divesession inside the listview can be clicked
     *  to call the DiveSessionDetailViewActivity with the data of the clicked divesession.
     **/
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

        /**
         *  This function initializes all ui components and returns the view to be displayed in the fragment manager 
         *  of our main activity.
         **/
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

        /**
         *  This function initializes the db listener and queries for all divesessions that belong to the current user.
         *  The query gets all divesessions in which the field "ref_user" is set to the id of the current user.
         **/
        private void RetrieveDiveSessionData()
        {
            diveSessionsDataListener = new FirebaseDataListener();
            diveSessionsDataListener.QueryParameterized("divesessions", "ref_user", TemporaryData.CURRENT_USER.id);
            diveSessionsDataListener.DataRetrieved += DiveSessionsDataListener_DataRetrieved;
        }

        /**
         *  This function sets the divesessionlist to the list of retrieved divesessions from the db listener.
         *  After that the listview is populated with the retrieved divesession data.
         **/
        private void DiveSessionsDataListener_DataRetrieved(object sender, FirebaseDataListener.DataEventArgs e)
        {
            diveSessionList = e.DiveSessions;
            fillDiveSessionData(diveSessionList);
        }

        /**
         *  This function populates the listview with the retrieved divesessions that were queried from the db.
         *  A Listview entry contains the date of the divesession and the coordinates of the location where the 
         *  divesession was created.
         **/
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

        /**
         *  This function handles the onclick event of the listview. If a divesession was selected inside the listview, 
         *  The current divesession in the TemporaryData class is set to the divesession from the listview and then 
         *  a divesessionDetailActivity is started that reads the date from the current divesession in TemporaryData.
         **/
        void lvwDive_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            TemporaryData.CURRENT_DIVESESSION = TemporaryData.CURRENT_USER.diveSessions[e.Position];

            var diveSessionDetailViewActivity = new Intent(Context, typeof(DiveSessionDetailViewActivity));
            StartActivity(diveSessionDetailViewActivity);

            /*
            if (TemporaryData.CURRENT_DIVESESSION.dives.Count > 0)
            {
                
            }
            else 
            {
                Toast.MakeText(Context, Resource.String.no_data_uploaded_yet, ToastLength.Long).Show();
            }
            */
        }

        /**
         *  This function handles the onclick event of the add button. When the button was clicked a new instance of 
         *  addSessionActivity is started to create a new divesession.
         **/
        void buttonAdd_Click(object sender, EventArgs eventArgs)
        {
            var addSessionActivity = new Intent(Context, typeof(AddSessionActivity));
            StartActivity(addSessionActivity);
        }
    }
}