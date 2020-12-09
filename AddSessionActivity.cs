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
    [Activity(Label = "AddSessionActivity")]
    public class AddSessionActivity : Activity
    {
        private Button btnAddSession;
        private Button btnCancel;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.AddSessionPage);
            // Create your application here
            btnAddSession = FindViewById<Button>(Resource.Id.btnAddSession);
            btnAddSession.Click += btnAddSession_Click;
            btnCancel = FindViewById<Button>(Resource.Id.btnCancel);
            btnCancel.Click += btnCancel_Click;
        }
        void btnAddSession_Click(object sender, EventArgs eventArgs)
        {
            var sessionsActivity = new Intent(this, typeof(SessionsActivity));
            StartActivity(sessionsActivity);
        }
        void btnCancel_Click(object sender, EventArgs eventArgs)
        {
            var sessionsActivity = new Intent(this, typeof(SessionsActivity));
            StartActivity(sessionsActivity);
        }
    }
}