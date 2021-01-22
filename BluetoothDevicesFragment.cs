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
using SupportV7 = Android.Support.V7.App;

namespace FreediverApp
{
    [Obsolete]
    public class BluetoothDevicesFragment : Fragment
    {
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

            

            if (btReceiver.m_adapter == null)
            {
                Toast.MakeText(Context, "Your device does not support Bluetooth!", ToastLength.Long);
            }
            else if (!btReceiver.m_adapter.IsEnabled)
            {
                SupportV7.AlertDialog.Builder saveDataDialog = new SupportV7.AlertDialog.Builder(Context);
                saveDataDialog.SetTitle("Bluetooth is not activated!");
                saveDataDialog.SetMessage("Do you want to activate Bluetooth on your device?");

                saveDataDialog.SetPositiveButton("Accept", (senderAlert, args) =>
                {
                    btReceiver.m_adapter.Enable();
                });
                saveDataDialog.SetNegativeButton("Cancel", (senderAlert, args) =>
                {
                    saveDataDialog.Dispose();
                });

                saveDataDialog.Show();
            }
           
            listView = view.FindViewById<ListView>(Resource.Id.lv_con_devices);
            btnScan = view.FindViewById<Button>(Resource.Id.bt_scan_btn);

            btnScan.Click += scanButtonOnClick;

            Devices = new List<BluetoothDevice>();
            addDevicesToList(getBondedBluetoothDevices());
            addDevicesToList(getUnknownBluetoothDevices());

            listView.Adapter = new CustomListViewAdapter(Devices);
            listView.ItemClick += ListView_ItemClick;

            return view;
        }

        private void ListView_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            try
            {
                Devices.ElementAt(e.Position).CreateBond();
                listView.Adapter = new CustomListViewAdapter(Devices);
            }
            catch (Exception ex) 
            {
                Toast.MakeText(Context, "Pairing with selected device failed!", ToastLength.Long);
            }  
        }

        private void scanButtonOnClick(object sender, EventArgs eventArgs) 
        {
            if (btReceiver.m_adapter.IsEnabled)
            {
                if (Devices != null)
                {
                    addDevicesToList(btReceiver.foundDevices);
                }
            }
            else 
            {
                Toast.MakeText(Context, "Please enable Bluetooth on your device to be able to scan for bluetooth devices!", ToastLength.Long).Show();
            }
            listView.Adapter = new CustomListViewAdapter(Devices);
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

            if (btReceiver.m_adapter != null && btReceiver.m_adapter.IsEnabled)
            {
                btReceiver.m_adapter.StartDiscovery();
            }
                
            return btReceiver.foundDevices;
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