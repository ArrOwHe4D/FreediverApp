using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;

namespace FreediverApp
{

    [Activity(Label = "Activity1")]
    public class DiveSessionDetailViewActivity : Activity
    {

        private Button btnDivesPerSession;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.DiveSessionDetailViewPage);

            btnDivesPerSession = FindViewById<Button>(Resource.Id.btnDivesPerSession);
            btnDivesPerSession.Click += redirectToDivesPerSessionActivity;
        }

        private void redirectToDivesPerSessionActivity(object sender, EventArgs eventArgs)
        {
            var divesPerSessionActivity = new Intent(this, typeof(DivesPerSessionActivity));
            StartActivity(divesPerSessionActivity);
        }
    }
}