using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V4.View;
using Android.Support.V4.Widget;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;

namespace FreediverApp
{
    [Activity(Label = "AccountPanelActivity")]
    public class AccountPanelActivity : AppCompatActivity, NavigationView.IOnNavigationItemSelectedListener
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.AccountPanelPage);
        }

        public bool OnNavigationItemSelected(IMenuItem item)
        {
            int id = item.ItemId;

            if (id == Resource.Id.nav_dive_sessions)
            {
                // Handle the camera action
            }
            else if (id == Resource.Id.nav_connected_devices)
            {

            }
            else if (id == Resource.Id.nav_profile)
            {
                var accountPanelActivity = new Intent(this, typeof(AccountPanelActivity));
                StartActivity(accountPanelActivity);
            }
            else if (id == Resource.Id.nav_settings)
            {

            }
            else if (id == Resource.Id.nav_share)
            {

            }
            else if (id == Resource.Id.nav_send)
            {

            }

            DrawerLayout drawer = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            drawer.CloseDrawer(GravityCompat.Start);
            return true;
        }
    }
}