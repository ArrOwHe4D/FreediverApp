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
using Android.Widget;

namespace FreediverApp
{
    /**
     *  This is the main activity or the main menu of our app. The UI is built with a sidebar menu than can be opened 
     *  with a menu button on the top left or via swiping from left to right. The sidebar contains menu elements that
     *  redirect to the other pages of the app. All other pages are defined as so called Fragments that can be instantiated inside
     *  a running activity. According to the Android documentation, fragments are deprecated but it was the only possibility to realize
     *  this kind of menu structure in Xamarin.Android. We didn´t face any errors with that and everything runs fine but since we don´t know,
     *  how long the Fragments will be supported by the Android SDK, a switch to another menu structure should be considered in future releases.
     *  The main view is a simple welcome text that prints a welcome message involving the username of the current user that logged into the app.
     *  The user is then able to navigate to the other subpages of the app.
     **/
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", ScreenOrientation = ScreenOrientation.Portrait)]
    public class MainActivity : AppCompatActivity, NavigationView.IOnNavigationItemSelectedListener
    {
        /**
         *  This function initializes all our basic menu components like the navigation bar and the Fragment manager, that is used
         *  to display all the subpages as they are defined as Fragments instead of normal Android activities.
         **/
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);

            SetContentView(Resource.Layout.MainActivity);

            Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);

            DrawerLayout drawer = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            ActionBarDrawerToggle toggle = new ActionBarDrawerToggle(this, drawer, toolbar, Resource.String.navigation_drawer_open, Resource.String.navigation_drawer_close);
            drawer.AddDrawerListener(toggle);
            toggle.SyncState();

            NavigationView navigationView = FindViewById<NavigationView>(Resource.Id.nav_view);
            navigationView.SetNavigationItemSelectedListener(this);

            FragmentTransaction menuTransaction = FragmentManager.BeginTransaction();
            MainFragment mainContent = new MainFragment();
            menuTransaction.Add(Resource.Id.framelayout, mainContent).Commit();
        }

        /**
         *  This function handles the logic when the back button on android was pressed.
         *  If the navigation menu is open, close it first, otherwise execute standard back button
         *  behavior (get last activity/fragment from stack and display it)
         **/
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

        /**
         *  This function opens the submenu from top right menu button. 
         **/
        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            //MenuInflater.Inflate(Resource.Menu.menu_main, menu);
            return true;
        }

        /**
         *  This function handles the selection of the submenu which is defined above.
         */
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            int id = item.ItemId;
            if (id == Resource.Id.action_settings)
            {
                return true;
            }

            return base.OnOptionsItemSelected(item);
        }

        /**
         *  This function handles the selection of a sidebar menu entry. It opens the corresponding
         *  Fragment and displays it inside our main activity. 
         **/
        public bool OnNavigationItemSelected(IMenuItem item)
        {
            int id = item.ItemId;
            FragmentTransaction menuTransaction = this.FragmentManager.BeginTransaction();

            if (id == Resource.Id.nav_landing_page)
            {
                MainFragment landingPageFragment = new MainFragment();
                menuTransaction.Replace(Resource.Id.framelayout, landingPageFragment).AddToBackStack(null).Commit();
            }
            if (id == Resource.Id.nav_dive_sessions)
            {
                DiveSessionsFragment divesessionsFragment = new DiveSessionsFragment();
                menuTransaction.Replace(Resource.Id.framelayout, divesessionsFragment).AddToBackStack(null).Commit();
            }
            else if (id == Resource.Id.nav_connected_devices)
            {
                ConnectiveDevicesFragment bluetoothDevicesFragment = new ConnectiveDevicesFragment();
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
                Toast.MakeText(this, Resource.String.logout_successful, ToastLength.Long).Show();
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

