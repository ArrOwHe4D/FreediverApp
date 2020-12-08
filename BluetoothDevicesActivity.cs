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
    [Activity(Label = "Bluetooth Devices",Theme = "@style/AppTheme.NoActionBar")]
    public class BluetoothDevicesActivity : Activity
    {
        private List<string> items;
        private List<BluetoothDevice> Devices;
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

            Devices = new List<BluetoothDevice>();
            addDevicesToList(getBondedBluetoothDevices());
            addDevicesToList(getUnknownBluetoothDevices());           
            items = devicesNames(Devices);

            ArrayAdapter<string> adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleListItem1, items);
            listView.Adapter = adapter;
            listView.ItemClick += ListView_ItemClick;
        }

        private void ListView_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            Console.WriteLine(listView.SelectedItem.ToString());
        }

        private void scanButtonOnClick(object sender, EventArgs eventArgs) 
        {
            Devices = btReceiver.foundDevices;

            if (Devices != null)
            {
                foreach (var device in Devices)
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

        private List<BluetoothDevice> getBondedBluetoothDevices()
        {
            if (btReceiver.m_adapter.IsEnabled)
            {
               return btReceiver.m_adapter.BondedDevices.ToList();               
            }
            return new List<BluetoothDevice>();
        }

        private List<BluetoothDevice> getUnknownBluetoothDevices()
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
                btReceiver.m_adapter.StartDiscovery();
            }
                
            return btReceiver.foundDevices;
        }

        private List<String> devicesNames(List<BluetoothDevice> _devices)
        {
            List<String> temp = new List<string>();
            for (int i = 0; i < _devices.Count; i++)
            {
                temp.Add(_devices.ElementAt(i).Name);
            }
            return temp;
        }

        private void addDevicesToList(List<BluetoothDevice> _devices)
        {
            for (int i = 0; i < _devices.Count; i++)
            {
                if (Devices.Contains(_devices.ElementAt(i)))
                    Devices.Add(_devices.ElementAt(i));
            }
        }
    }
}