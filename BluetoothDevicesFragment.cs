using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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
        private Thread bluetoothListenerThread;
        private Timer scanTimer;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {            
            var view = inflater.Inflate(Resource.Layout.BluetoothDevicesPage, container, false);

            btReceiver = new BluetoothDeviceReceiver();
            btReceiver.m_adapter = BluetoothAdapter.DefaultAdapter;

            listView = view.FindViewById<ListView>(Resource.Id.lv_con_devices);
            btnScan = view.FindViewById<Button>(Resource.Id.bt_scan_btn);

            btnScan.Click += scanButtonOnClick;

            listView.ItemClick += ListView_ItemClick;

            Devices = new List<BluetoothDevice>();
            bluetoothListenerThread = new Thread(discoverDevices);

            if (btReceiver.m_adapter == null)
            {
                Toast.MakeText(Context, "Your device does not support Bluetooth!", ToastLength.Long);
            }
            else if (!btReceiver.m_adapter.IsEnabled)
            {
                SupportV7.AlertDialog.Builder bluetoothActivationDialog = new SupportV7.AlertDialog.Builder(Context);
                bluetoothActivationDialog.SetTitle("Bluetooth is not activated!");
                bluetoothActivationDialog.SetMessage("Do you want to activate Bluetooth on your device?");

                bluetoothActivationDialog.SetPositiveButton("Accept", (senderAlert, args) =>
                {
                    btReceiver.m_adapter.Enable();
                    if (btReceiver.m_adapter.IsEnabled)
                    {
                        Toast.MakeText(Context, "Bluetooth activated!", ToastLength.Long);
                        //scanTimer = new Timer(new TimerCallback(tickTimer), null, 0, 500);
                        bluetoothListenerThread.Start();
                    }
                    else
                    {
                        Toast.MakeText(Context, "Bluetooth activation failed!", ToastLength.Long);
                    }
                });
                bluetoothActivationDialog.SetNegativeButton("Cancel", (senderAlert, args) =>
                {
                    bluetoothActivationDialog.Dispose();
                });

                bluetoothActivationDialog.Show();
            }
            else 
            {
                initBluetoothListView();
            }
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
                if (!bluetoothListenerThread.IsAlive)
                {
                    bluetoothListenerThread.Start();
                } 
                //scanTimer = new Timer(new TimerCallback(tickTimer), null, 0, 300);
            }
            else 
            {
                Toast.MakeText(Context, "Please enable Bluetooth on your device to be able to scan for bluetooth devices!", ToastLength.Long).Show();
            }       
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
            Activity.RegisterReceiver(btReceiver, new IntentFilter(BluetoothDevice.ActionFound));

            const int locationPermissionsRequestCode = 1000;

            var locationPermissions = new[]
            {
                Manifest.Permission.AccessCoarseLocation,
                Manifest.Permission.AccessFineLocation
            };

            var coarseLocationPermissionGranted = ContextCompat.CheckSelfPermission(this.Context, Manifest.Permission.AccessCoarseLocation);

            var fineLocationPermissionGranted = ContextCompat.CheckSelfPermission(this.Context, Manifest.Permission.AccessFineLocation);

            if (coarseLocationPermissionGranted == Permission.Denied || fineLocationPermissionGranted == Permission.Denied)
                ActivityCompat.RequestPermissions(Activity, locationPermissions, locationPermissionsRequestCode);

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
                    List<string> devices = getDeviceNames();

                    if (!devices.Contains(_devices.ElementAt(i).Name))
                        Devices.Add(_devices.ElementAt(i));
                }
            }
        }

        private void initBluetoothListView() 
        {
            addDevicesToList(getBondedBluetoothDevices());
            addDevicesToList(getUnknownBluetoothDevices());
            refreshGui();
        }

        private void discoverDevices()
        {
            for (int i = 0; i < 10; i++) 
            {
                addDevicesToList(getBondedBluetoothDevices());
                addDevicesToList(getUnknownBluetoothDevices());
                Activity.RunOnUiThread(() => { refreshGui(); });
                Thread.Sleep(500);
            }
            bluetoothListenerThread.Abort();
        }

        private void tickTimer(object state)
        { 
            Thread listenerThread = new Thread(() => { discoverDevices(); });
            listenerThread.Start();
            scanTimer.Change(Timeout.Infinite, Timeout.Infinite);
        }

        private void refreshGui() 
        {
            listView.Adapter = new CustomListViewAdapter(Devices);
        }

        private List<string> getDeviceNames() 
        {
            List<string> result = new List<string>();

            for (int i = 0; i < Devices.Count; i++) 
            {
                result.Add(Devices.ElementAt(i).Name);
            }

            return result;
        }
    }
}