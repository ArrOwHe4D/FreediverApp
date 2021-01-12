using System;
using System.Collections.Generic;
using System.Linq;
using Android.Bluetooth;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using FreediverApp.BluetoothCommunication;
using Android.Content.PM;
using Android;
using Android.Support.V4.Content;
using Android.Support.V4.App;
using Fragment = Android.App.Fragment;

namespace FreediverApp
{
    public class BluetoothDevicesFragment : Fragment
    {
        private List<string> items;
        private List<BluetoothDevice> Devices;
        private ListView listView;
        private BluetoothDeviceReceiver btReceiver;
        private Button btnScan;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.BluetoothDevicesPage, container, false);

            btReceiver = new BluetoothDeviceReceiver();
            btReceiver.m_adapter = BluetoothAdapter.DefaultAdapter;

            items = new List<string>();
            listView = view.FindViewById<ListView>(Resource.Id.lv_con_devices);
            btnScan = view.FindViewById<Button>(Resource.Id.bt_scan_btn);

            btnScan.Click += scanButtonOnClick;

            Devices = new List<BluetoothDevice>();
            addDevicesToList(getBondedBluetoothDevices());
            addDevicesToList(getUnknownBluetoothDevices());
            items = devicesNames(Devices);

            ArrayAdapter<string> adapter = new ArrayAdapter<string>(this.Context, Android.Resource.Layout.SimpleListItem1, items);
            listView.Adapter = adapter;
            listView.ItemClick += ListView_ItemClick;

            return view;
        }

        private void ListView_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            //Toast.MakeText(this, listView.SelectedItemPosition.ToString(), ToastLength.Long).Show();
            //Console.WriteLine(listView.SelectedItemPosition.ToString());
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
                        items.Add(device.Name + " (" + device.Address + ")");
                    }
                }
            }

            ArrayAdapter<string> adapter = new ArrayAdapter<string>(this.Context, Android.Resource.Layout.SimpleListItem1, items);
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
            this.Activity.RegisterReceiver(btReceiver, new IntentFilter(BluetoothDevice.ActionFound));

            const int locationPermissionsRequestCode = 1000;

            var locationPermissions = new[]
            {
                Manifest.Permission.AccessCoarseLocation,
                Manifest.Permission.AccessFineLocation
            };

            var coarseLocationPermissionGranted = ContextCompat.CheckSelfPermission(this.Context, Manifest.Permission.AccessCoarseLocation);

            var fineLocationPermissionGranted = ContextCompat.CheckSelfPermission(this.Context, Manifest.Permission.AccessFineLocation);

            if (coarseLocationPermissionGranted == Permission.Denied || fineLocationPermissionGranted == Permission.Denied)
                ActivityCompat.RequestPermissions(this.Activity, locationPermissions, locationPermissionsRequestCode);

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
            if (_devices != null)
            {
                for (int i = 0; i < _devices.Count; i++)
                {
                    if (!Devices.Contains(_devices.ElementAt(i)))
                        Devices.Add(_devices.ElementAt(i));
                }
            }
        }
    }
}