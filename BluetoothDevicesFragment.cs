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
using Java.Util;
using Android.App;
using Plugin.BLE.Abstractions.Contracts;
using IAdapter = Plugin.BLE.Abstractions.Contracts.IAdapter;
using System.Collections.ObjectModel;
using Plugin.BLE;

namespace FreediverApp
{
    [Obsolete]
    public class BluetoothDevicesFragment : Fragment
    {
        private List<BluetoothDevice> Devices;
        private ListView listView;
        private BluetoothDeviceReceiver btReceiver;
        private Button btnScan;
        private ProgressBar scanIndicator;
        private BluetoothSocket btSocket;
        private IBluetoothLE ble;
        private IAdapter bleAdapter;
        private ObservableCollection<IDevice> bleDeviceList;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.BluetoothDevicesPage, container, false);

            btReceiver = new BluetoothDeviceReceiver();
            btReceiver.m_adapter = BluetoothAdapter.DefaultAdapter;

            ble = CrossBluetoothLE.Current;
            bleAdapter = CrossBluetoothLE.Current.Adapter;
            bleAdapter.ScanTimeout = 5000;
            bleAdapter.ScanTimeoutElapsed += stopScan;

            bleDeviceList = new ObservableCollection<IDevice>();

            //var systemDevices = bleAdapter.GetSystemConnectedOrPairedDevices();

            listView = view.FindViewById<ListView>(Resource.Id.lv_con_devices);
            btnScan = view.FindViewById<Button>(Resource.Id.bt_scan_btn);

            scanIndicator = view.FindViewById<ProgressBar>(Resource.Id.scan_indicator);
            scanIndicator.Visibility = ViewStates.Gone;

            btnScan.Click += scanButtonOnClick;

            listView.ItemClick += ListView_ItemClick;

            //Devices = new List<BluetoothDevice>();

            //if (btReceiver.m_adapter == null)
            //{
            //    Toast.MakeText(Context, "Your device does not support Bluetooth!", ToastLength.Long);
            //}
            //else if (!btReceiver.m_adapter.IsEnabled)
            //{
            //    SupportV7.AlertDialog.Builder bluetoothActivationDialog = new SupportV7.AlertDialog.Builder(Context);
            //    bluetoothActivationDialog.SetTitle("Bluetooth is not activated!");
            //    bluetoothActivationDialog.SetMessage("Do you want to activate Bluetooth on your device?");

            //    bluetoothActivationDialog.SetPositiveButton("Accept", (senderAlert, args) =>
            //    {
            //        btReceiver.m_adapter.Enable();

            //        Thread.Sleep(1500);

            //        if (btReceiver.m_adapter.IsEnabled)
            //        {
            //            Toast.MakeText(Context, "Bluetooth activated!", ToastLength.Long);

            //            //If the adapter was successfully activated, start the thread that scans for new bluetooth devices
            //            Thread bluetoothDeviceListenerThread = new Thread(discoverDevices);
            //            bluetoothDeviceListenerThread.IsBackground = true;
            //            bluetoothDeviceListenerThread.Start();
            //        }
            //        else
            //        {
            //            Toast.MakeText(Context, "Bluetooth activation failed!", ToastLength.Long);
            //        }
            //    });
            //    bluetoothActivationDialog.SetNegativeButton("Cancel", (senderAlert, args) =>
            //    {
            //        bluetoothActivationDialog.Dispose();
            //    });

            //    bluetoothActivationDialog.Show();
            //}
            //else
            //{
            //    initBluetoothListView();
            //}
            return view;
        }

        private void ListView_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            var device = (Plugin.BLE.Abstractions.DeviceBase)bleDeviceList[e.Position]; ///ElementAt(e.Position);
            runBluetoothConnectionDialog(device);

            //Boolean fail = false;
            //try
            //{
            //    btSocket = device.CreateInsecureRfcommSocketToServiceRecord(uuid);
            //}

            //catch(Exception exp)
            //{
            //    fail = true;
            //    Toast.MakeText(Context, "Socket creation failed!", ToastLength.Long);
            //}
            //try
            //{
            //    btSocket.Connect();
            //}
            //catch(Exception exp)
            //{
            //    try
            //    {
            //        fail = true;
            //        btSocket.Close();
            //    }
            //    catch(Exception exp2)
            //    {
            //        Toast.MakeText(Context, "Socket creation failed after connection!", ToastLength.Long);
            //    }
            //}
            //if (!fail)
            //{
            //    Toast.MakeText(Context, "Connected!", ToastLength.Long);
            //}

            /*
            try
            {
                Devices.ElementAt(e.Position).CreateBond();
                refreshGui();
            }
            catch (Exception ex)
            {
                Toast.MakeText(Context, "Pairing with selected device failed!", ToastLength.Long);
            }
            */
        }

        private async void scanButtonOnClick(object sender, EventArgs eventArgs)
        {
            //bleDeviceList.Clear();
            scanIndicator.Visibility = ViewStates.Visible;
            bleAdapter.DeviceDiscovered += (s, a) =>
            {
                if (!bleDeviceList.Contains(a.Device))
                    bleDeviceList.Add(a.Device);
                
                refreshGui();
            };
            await bleAdapter.StartScanningForDevicesAsync();

            //if (btReceiver.m_adapter.IsEnabled)
            //{
            //    // if the user clicks on the scanButton the thread for discovering new Bluetooth Devices is started
            //    Thread bluetoothDeviceListenerThread = new Thread(discoverDevices);
            //    bluetoothDeviceListenerThread.IsBackground = true;
            //    bluetoothDeviceListenerThread.Start();
            //}
            //else
            //{
            //    Toast.MakeText(Context, "Please enable Bluetooth on your device to be able to scan for bluetooth devices!", ToastLength.Long).Show();
            //}
        }

        private void stopScan(object sender, EventArgs eventArgs) 
        {
            scanIndicator.Visibility = ViewStates.Gone;
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
                List<string> devices = getDeviceNames();

                for (int i = 0; i < _devices.Count; i++)
                {
                    if (!devices.Contains(_devices.ElementAt(i).Name))
                        Devices.Add(_devices.ElementAt(i));
                }
            }
        }

        private void addBleDevicesToList(List<Plugin.BLE.Abstractions.DeviceBase> _devices)
        {
            if (_devices != null)
            {
                List<string> devices = getDeviceNames();

                for (int i = 0; i < _devices.Count; i++)
                {
                    //if (!devices.Contains(_devices.ElementAt(i).Name))
                        //bleDevices.Add(_devices.ElementAt(i));
                }
            }
        }

        private void initBluetoothListView()
        {
            addDevicesToList(getBondedBluetoothDevices());
            addDevicesToList(getUnknownBluetoothDevices());
            btReceiver.m_adapter.CancelDiscovery();
            refreshGui();
        }

        private void discoverDevices()
        {
            // let the thread search 10 sec for every second it runs and close the thread after the search period has finished
            Activity.RunOnUiThread(() => { scanIndicator.Visibility = ViewStates.Visible; });
            for (int i = 0; i < 10; i++)
            {
                addDevicesToList(getBondedBluetoothDevices());
                addDevicesToList(getUnknownBluetoothDevices());

                Activity.RunOnUiThread(() => { refreshGui(); });
                Thread.Sleep(1000);
            }
            Activity.RunOnUiThread(() => 
            {
                btReceiver.m_adapter.CancelDiscovery();
                scanIndicator.Visibility = ViewStates.Gone;  
            });
            Thread.CurrentThread.Abort();
        }

        private void refreshGui()
        {
            listView.Adapter = new CustomListViewAdapter(bleDeviceList);
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

        private void runBluetoothConnectionDialog(Plugin.BLE.Abstractions.DeviceBase clickedDevice)
        {
            LayoutInflater layoutInflater = LayoutInflater.From(this.Context);
            View dialogView = layoutInflater.Inflate(Resource.Layout.BluetoothConnectionDialog, null);
            SupportV7.AlertDialog.Builder dialogBuilder = new SupportV7.AlertDialog.Builder(this.Context);
            dialogBuilder.SetView(dialogView);
            dialogBuilder.SetTitle("Connect to Device");
            dialogBuilder.SetIcon(Resources.GetDrawable(Resource.Drawable.icon_connected_devices));

            var textViewDeviceName = dialogView.FindViewById<TextView>(Resource.Id.textview_device_name);
            var textViewMacAddress = dialogView.FindViewById<TextView>(Resource.Id.textview_mac_address);
            var textViewConState = dialogView.FindViewById<TextView>(Resource.Id.textview_con_state);

            textViewDeviceName.Text = clickedDevice.Name;
            textViewMacAddress.Text = clickedDevice.Id.ToString();
            textViewConState.Text = clickedDevice.State == Plugin.BLE.Abstractions.DeviceState.Connected ? "Connected" : "Disconnected";

            var editValueField = dialogView.FindViewById<EditText>(Resource.Id.userInput);
            dialogBuilder.SetCancelable(false)
                .SetPositiveButton("Connect", async delegate
                {
                    try
                    {
                        await bleAdapter.ConnectToDeviceAsync(clickedDevice);
                    }
                    catch 
                    {
                        //BTLE Connection error
                    }
                    dialogBuilder.Dispose();
                })
                .SetNegativeButton("Cancel", delegate
                {
                    dialogBuilder.Dispose();
                });

            SupportV7.AlertDialog dialog = dialogBuilder.Create();
            dialog.Show();
        }
    }
}
