using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using System;
using FreediverApp.DatabaseConnector;

namespace FreediverApp.FragmentActivities
{
    [Obsolete]
    public class MainFragment : Fragment
    {
        private TextView textViewWelcomeMessage;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.content_main, container, false);

            textViewWelcomeMessage = view.FindViewById<TextView>(Resource.Id.titleWelcome);
            textViewWelcomeMessage.Text = Context.GetString(Resource.String.welcome) + " " + TemporaryData.USER_NAME + " !";

            return view;
        }
    }
}