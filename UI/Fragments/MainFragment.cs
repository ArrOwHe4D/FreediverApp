using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using System;
using FreediverApp.DatabaseConnector;
using System.Collections.Generic;

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
        private TextView textViewDeepestDive;
        private TextView textViewLongestDive;
        private List<DiveSession> diveSessionList;

        private FirebaseDataListener diveSessionDataListener;


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

            textViewWelcomeMessage = view.FindViewById<TextView>(Resource.Id.titleWelcome);
            textViewWelcomeMessage.Text = Context.GetString(Resource.String.welcome) + " " + TemporaryData.CURRENT_USER.username + " !";

            textViewDeepestDive = view.FindViewById<TextView>(Resource.Id.textview_deepest_dive);
            textViewLongestDive = view.FindViewById<TextView>(Resource.Id.textview_longest_dive);

            textViewDeepestDive.Text += " 94 m";
            textViewLongestDive.Text += " 9 sec";

            //RetrieveDiveSessionData();

            return view;
        }

        private void RetrieveDiveSessionData()
        {
            diveSessionDataListener = new FirebaseDataListener();
            diveSessionDataListener.QueryParameterized("dives", "ref_user", TemporaryData.CURRENT_USER.id);
            diveSessionDataListener.DataRetrieved += DiveSessionDataListener_DataRetrieved;
        }

        private void DiveSessionDataListener_DataRetrieved(object sender, FirebaseDataListener.DataEventArgs e)
        {
            diveSessionList = e.DiveSessions;
            fillStatisticsView();
        }

        private void fillStatisticsView() 
        {
            if (diveSessionList != null) 
            {
                Dive deepestDive = getDeepestDive();
                Dive longestDive = getLongestDive();

                textViewDeepestDive.Text += deepestDive.maxDepth + " m";
                textViewLongestDive.Text += longestDive.GetTotalTime() + " sec";
            }
        }

        Dive getDeepestDive() 
        {
            int maxDepth = 0;
            foreach (DiveSession session in diveSessionList) 
            {
                foreach (Dive dive in session.dives) 
                {
                    int currentDiveMaxDepth = int.Parse(dive.maxDepth);

                    if (maxDepth < currentDiveMaxDepth) 
                    {
                        maxDepth = currentDiveMaxDepth;
                    }
                }

                foreach (Dive dive in session.dives)
                {
                    if (int.Parse(dive.maxDepth) == maxDepth)
                    {
                        return dive;
                    }
                }
            }

            return null;
        }

        Dive getLongestDive() 
        {
            int maxDuration = 0;
            foreach (DiveSession session in diveSessionList)
            {
                foreach (Dive dive in session.dives)
                {
                    int currentDiveMaxDuration = int.Parse(dive.GetTotalTime());

                    if (maxDuration < currentDiveMaxDuration)
                    {
                        maxDuration = currentDiveMaxDuration;
                    }
                }

                foreach (Dive dive in session.dives)
                {
                    if (int.Parse(dive.maxDepth) == maxDuration)
                    {
                        return dive;
                    }
                }
            }

            return null;
        }
    }
}