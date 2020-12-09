using System;
using System.Collections.Generic;
using Android;
using Android.App;
using Android.Bluetooth;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V4.App;
using Android.Support.V4.Content;
using Android.Support.V4.View;
using Android.Support.V4.Widget;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using FreediverApp.BluetoothCommunication;
using Microcharts.Droid;
using Microcharts;
using ActionBarDrawerToggle = Android.Support.V7.App.ActionBarDrawerToggle;
using SkiaSharp;

namespace FreediverApp
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = false)]
    public class MainActivity : AppCompatActivity, NavigationView.IOnNavigationItemSelectedListener
    {
        private BluetoothAdapter bt_adapter;
        private BluetoothDeviceReceiver bt_receiver;
        private ChartView chartView;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_main);
            Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);

            bt_adapter = BluetoothAdapter.DefaultAdapter;

            Button btn_scan = FindViewById<Button>(Resource.Id.btn_scan);
            btn_scan.Click += Btn_scan_Click;

            chartView = FindViewById<ChartView>(Resource.Id.chartView);
            generateChart();

            DrawerLayout drawer = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            ActionBarDrawerToggle toggle = new ActionBarDrawerToggle(this, drawer, toolbar, Resource.String.navigation_drawer_open, Resource.String.navigation_drawer_close);
            drawer.AddDrawerListener(toggle);
            toggle.SyncState();

            NavigationView navigationView = FindViewById<NavigationView>(Resource.Id.nav_view);
            navigationView.SetNavigationItemSelectedListener(this);
        }

        private void generateChart() 
        {
            List<ChartEntry> dataList = new List<ChartEntry>();

            dataList.Add(new ChartEntry(0)
            {
                Label = "0:05",
                ValueLabel = "0",
                Color = SKColor.Parse("#5cf739")
            });

            dataList.Add(new ChartEntry(5)
            {
                Label = "0:10",
                ValueLabel = "5",
                Color = SKColor.Parse("#f7c139")
            });

            dataList.Add(new ChartEntry(8)
            {
                Label = "0:15",
                ValueLabel = "8",
                Color = SKColor.Parse("#f75939")
            });

            dataList.Add(new ChartEntry(8)
            {
                Label = "0:20",
                ValueLabel = "8",
                Color = SKColor.Parse("#f75939")
            });

            dataList.Add(new ChartEntry(9)
            {
                Label = "0:25",
                ValueLabel = "9",
                Color = SKColor.Parse("#f75939")
            });

            dataList.Add(new ChartEntry(8)
            {
                Label = "0:30",
                ValueLabel = "8",
                Color = SKColor.Parse("#f75939")
            });

            dataList.Add(new ChartEntry(4)
            {
                Label = "0:35",
                ValueLabel = "4",
                Color = SKColor.Parse("f7c139")
            });

            dataList.Add(new ChartEntry(1)
            {
                Label = "0:40",
                ValueLabel = "4",
                Color = SKColor.Parse("#5cf739")
            });

            var chart = new LineChart { Entries = dataList, LabelTextSize = 30f };
            chartView.Chart = chart;
        }

        private void Btn_scan_Click(object sender, EventArgs e)
        {
            const int locationPermissionsRequestCode = 1000;

            var locationPermissions = new[]
            {
                Manifest.Permission.AccessCoarseLocation,
                Manifest.Permission.AccessFineLocation
            };

            var coarseLocationPermissionGranted = ContextCompat.CheckSelfPermission(this, Manifest.Permission.AccessCoarseLocation);

            var fineLocationPermissionGranted = ContextCompat.CheckSelfPermission(this, Manifest.Permission.AccessFineLocation);

            if (coarseLocationPermissionGranted == Permission.Denied || fineLocationPermissionGranted == Permission.Denied)
                ActivityCompat.RequestPermissions(this, locationPermissions, locationPermissionsRequestCode);

            bt_receiver = new BluetoothDeviceReceiver();

            RegisterReceiver(bt_receiver, new IntentFilter(BluetoothDevice.ActionFound));

            if (BluetoothAdapter.DefaultAdapter != null && BluetoothAdapter.DefaultAdapter.IsEnabled) 
            {
                foreach (var pairedDevice in BluetoothAdapter.DefaultAdapter.BondedDevices) 
                {
                    Console.WriteLine($"Found paired device with name: {pairedDevice.Name} and MAC-Address: {pairedDevice.Address}");
                }
            }

            BluetoothAdapter.DefaultAdapter.StartDiscovery();
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

            if (id == Resource.Id.nav_dive_sessions)
            {
                var diveSessionDetailActivity = new Intent(this, typeof(DiveSessionDetailViewActivity));
                StartActivity(diveSessionDetailActivity);
            }
            else if (id == Resource.Id.nav_connected_devices)
            {
                var bluetoothDevicesPanelActivity = new Intent(this, typeof(BluetoothDevicesActivity));
                StartActivity(bluetoothDevicesPanelActivity);
            }
            else if (id == Resource.Id.nav_profile)
            {
                var accountPanelActivity = new Intent(this, typeof(AccountPanelActivity));
                StartActivity(accountPanelActivity);
            }
            else if (id == Resource.Id.nav_settings)
            {
                var settingsActivity = new Intent(this, typeof(SettingsActivity));
                StartActivity(settingsActivity);
            }
            else if (id == Resource.Id.nav_logout) 
            {
                var loginActivity = new Intent(this, typeof(LoginActivity));
                StartActivity(loginActivity);
            }
            else if (id == Resource.Id.nav_share)
            {
                var sessionsActivity = new Intent(this, typeof(SessionsActivity));
                StartActivity(sessionsActivity);
            }
            else if (id == Resource.Id.nav_send)
            {

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

