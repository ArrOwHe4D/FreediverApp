using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V4.View;
using Android.Support.V4.Widget;
using Android.Support.V7.App;
using Android.Views;
using FreediverApp.FragmentActivities;
using FragmentTransaction = Android.App.FragmentTransaction;
using ActionBarDrawerToggle = Android.Support.V7.App.ActionBarDrawerToggle;

namespace FreediverApp
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", ScreenOrientation = ScreenOrientation.Portrait)]
    public class MainActivity : AppCompatActivity, NavigationView.IOnNavigationItemSelectedListener
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);

            SetContentView(Resource.Layout.activity_main);

            Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);

            DrawerLayout drawer = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            ActionBarDrawerToggle toggle = new ActionBarDrawerToggle(this, drawer, toolbar, Resource.String.navigation_drawer_open, Resource.String.navigation_drawer_close);
            drawer.AddDrawerListener(toggle);
            toggle.SyncState();

            NavigationView navigationView = FindViewById<NavigationView>(Resource.Id.nav_view);
            navigationView.SetNavigationItemSelectedListener(this);

            FragmentTransaction menuTransaction = this.FragmentManager.BeginTransaction();
            MainFragment mainContent = new MainFragment();
            menuTransaction.Add(Resource.Id.framelayout, mainContent).Commit();
        }

        public override void OnBackPressed()
        {
            DrawerLayout drawer = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            if(drawer.IsDrawerOpen(GravityCompat.Start))
            {
                drawer.CloseDrawer(GravityCompat.Start);
            }
            else
            {
                base.OnBackPressed();
            }
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu_main, menu);
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            int id = item.ItemId;
            if (id == Resource.Id.action_settings)
            {
                return true;
            }

            return base.OnOptionsItemSelected(item);
        }

        public bool OnNavigationItemSelected(IMenuItem item)
        {
            int id = item.ItemId;
            FragmentTransaction menuTransaction = this.FragmentManager.BeginTransaction();

            if (id == Resource.Id.nav_dive_sessions)
            {
                DiveSessionsFragment divesessionsFragment = new DiveSessionsFragment();
                menuTransaction.Replace(Resource.Id.framelayout, divesessionsFragment).AddToBackStack(null).Commit();
            }
            else if (id == Resource.Id.nav_connected_devices)
            {
                BluetoothDevicesFragment bluetoothDevicesFragment = new BluetoothDevicesFragment();
                menuTransaction.Replace(Resource.Id.framelayout, bluetoothDevicesFragment).AddToBackStack(null).Commit();
            }
            else if (id == Resource.Id.nav_profile)
            {
                AccountPanelFragment accountPanelFragment = new AccountPanelFragment();
                menuTransaction.Replace(Resource.Id.framelayout, accountPanelFragment).AddToBackStack(null).Commit();
            }
            else if (id == Resource.Id.nav_settings)
            {
                SettingsFragment settingsFragment = new SettingsFragment();
                menuTransaction.Replace(Resource.Id.framelayout, settingsFragment).AddToBackStack(null).Commit();
            }
            else if (id == Resource.Id.nav_logout) 
            {
                var loginActivity = new Intent(this, typeof(LoginActivity));
                StartActivity(loginActivity);
            }

            DrawerLayout drawer = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            drawer.CloseDrawer(GravityCompat.Start);
            return true;
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}

