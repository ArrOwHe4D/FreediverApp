using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.Bluetooth;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using FreediverApp.BluetoothCommunication;
using Android.Content.PM;
using Android;
using Android.Support.V4.Content;
using Android.Support.V4.App;

namespace FreediverApp
{
    [Activity(Label = "Bluetooth Devices")]
    public class BluetoothDevicesActivity : Activity
    {
        private List<string> items;
        private List<BluetoothDevice> discoveredDevices;
        private ListView listView;
        private BluetoothDeviceReceiver btReceiver;
        private Button btnScan;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            btReceiver = new BluetoothDeviceReceiver();
            btReceiver.m_adapter = BluetoothAdapter.DefaultAdapter;

            SetContentView(Resource.Layout.BluetoothDevicesPage);

            listView = FindViewById<ListView>(Resource.Id.lv_con_devices);
            btnScan = FindViewById<Button>(Resource.Id.bt_scan_btn);

            btnScan.Click += scanButtonOnClick;
          
            discoveredDevices = getUnknownDevices();
            items = getBondedBluetoothDevices();

            ArrayAdapter<string> adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleListItem1, items);
            listView.Adapter = adapter;
        }

        private void scanButtonOnClick(object sender, EventArgs eventArgs) 
        {
            discoveredDevices = btReceiver.foundDevices;

            if (discoveredDevices != null)
            {
                foreach (var device in discoveredDevices)
                {
                    if (!items.Contains(device.Name))
                    {
                        items.Add(device.Name);
                    }
                }
            }

            ArrayAdapter<string> adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleListItem1, items);
            listView.Adapter = adapter;
        }

        private List<BluetoothDevice> getUnknownDevices()
        {
            RegisterReceiver(btReceiver, new IntentFilter(BluetoothDevice.ActionFound));

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

            if (btReceiver.m_adapter != null) 
            {
                //BluetoothDeviceReceiver.Adapter.StartDiscovery();
                btReceiver.m_adapter.StartDiscovery();
            }
                
            return btReceiver.foundDevices;
        }

        private List<string> getBondedBluetoothDevices()
        {           
            List<string> foundDevices = new List<string>();         
           
            if (btReceiver.m_adapter.IsEnabled)
            {
                List<BluetoothDevice> devices = btReceiver.m_adapter.BondedDevices.ToList();
                foreach (var device in devices)
                {
                    foundDevices.Add(device.Name);
                }              
            }            
            return foundDevices;
        }
    }
}