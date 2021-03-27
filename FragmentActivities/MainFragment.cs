using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using System;
using FreediverApp.DatabaseConnector;

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

            return view;
        }
    }
}