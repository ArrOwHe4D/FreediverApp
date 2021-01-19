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
        private TextView tvwSessionName;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.DiveSessionDetailViewPage);

            btnDivesPerSession = FindViewById<Button>(Resource.Id.btnDivesPerSession);
            btnDivesPerSession.Click += redirectToDivesPerSessionActivity;
            tvwSessionName = FindViewById<TextView>(Resource.Id.tvwDdvSessionName);

            tvwSessionName.Text = User.curUser.curDiveSession.date + " " + User.curUser.curDiveSession.location;
        }

        private void redirectToDivesPerSessionActivity(object sender, EventArgs eventArgs)
        {
            var divesPerSessionActivity = new Intent(this, typeof(DivesPerSessionActivity));
            StartActivity(divesPerSessionActivity);
        }
    }
}