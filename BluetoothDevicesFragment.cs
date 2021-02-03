using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Android.Bluetooth;
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
using Plugin.BLE.Abstractions.Contracts;
using IAdapter = Plugin.BLE.Abstractions.Contracts.IAdapter;
using System.Collections.ObjectModel;
using Plugin.BLE;
using Plugin.BLE.Abstractions;
using Plugin.BLE.Abstractions.EventArgs;
using System.Threading.Tasks;

namespace FreediverApp
{
    [Obsolete]
    public class BluetoothDevicesFragment : Fragment
    {
        private ListView listView;
        private BluetoothDeviceReceiver btReceiver;
        private Button btnScan;
        private ProgressBar scanIndicator;
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
            //ble.StateChanged += bleStateChanged;   
            
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

            if (ble == null) 
            {
                Toast.MakeText(Context, "Your device does not support Bluetooth!", ToastLength.Long).Show();
            }
            if (ble.State == BluetoothState.Off) 
            {
                runBluetoothActivationDialog();
            }
            else
            {
                initBluetoothListView();
            }
            return view;
        }

        private void refreshBleAdapter() 
        {
            ble = CrossBluetoothLE.Current;
            bleAdapter = CrossBluetoothLE.Current.Adapter;
            bleAdapter.ScanTimeout = 5000;
            bleAdapter.ScanTimeoutElapsed += stopScan;
        }

        private void ListView_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            var device = (DeviceBase)bleDeviceList[e.Position]; 
            runBluetoothConnectionDialog(device); 
        }

        private async void bleStateChanged(BluetoothStateChangedArgs args) 
        {

        }

        private async void scanButtonOnClick(object sender, EventArgs eventArgs)
        {
            if (ble.State == BluetoothState.On)
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
            }
            else 
            {
                runBluetoothActivationDialog();
            }
        }

        private void stopScan(object sender, EventArgs eventArgs) 
        {
            scanIndicator.Visibility = ViewStates.Gone;
        }

        private void checkBluetoothPermission() 
        {
            const int locationPermissionsRequestCode = 1000;

            var locationPermissions = new[]
            {
                Manifest.Permission.AccessCoarseLocation,
                Manifest.Permission.AccessFineLocation
            };

            var coarseLocationPermissionGranted = ContextCompat.CheckSelfPermission(Context, Manifest.Permission.AccessCoarseLocation);

            var fineLocationPermissionGranted = ContextCompat.CheckSelfPermission(Context, Manifest.Permission.AccessFineLocation);

            if (coarseLocationPermissionGranted == Permission.Denied || fineLocationPermissionGranted == Permission.Denied)
                ActivityCompat.RequestPermissions(Activity, locationPermissions, locationPermissionsRequestCode);
        }

        private IReadOnlyList<IDevice> getBondedBluetoothDevices()
        {
            return bleAdapter.GetSystemConnectedOrPairedDevices();
        }

        private void initBluetoothListView()
        {
            foreach (var device in getBondedBluetoothDevices()) 
            {
                bleDeviceList.Add(device);
            }
             
            refreshGui();
        }

        private void refreshGui()
        {
            listView.Adapter = new CustomListViewAdapter(bleDeviceList);
        }

        private List<string> getDeviceNames()
        {
            List<string> result = new List<string>();

            for (int i = 0; i < bleDeviceList.Count; i++)
            {
                result.Add(bleDeviceList.ElementAt(i).Name);
            }

            return result;
        }

        private void runBluetoothActivationDialog() 
        {
            SupportV7.AlertDialog.Builder bluetoothActivationDialog = new SupportV7.AlertDialog.Builder(Context);
            bluetoothActivationDialog.SetTitle("Bluetooth is not activated!");
            bluetoothActivationDialog.SetMessage("Do you want to activate Bluetooth on your device?");

            bluetoothActivationDialog.SetPositiveButton("Accept", (senderAlert, args) =>
            {
                btReceiver.m_adapter.Enable();

                Thread.Sleep(2500);

                refreshBleAdapter();

                if (btReceiver.m_adapter.IsEnabled)
                {
                    Toast.MakeText(Context, "Bluetooth activated!", ToastLength.Long).Show();
                }
                else
                {
                    Toast.MakeText(Context, "Bluetooth activation failed!", ToastLength.Long).Show();
                }
            });
            bluetoothActivationDialog.SetNegativeButton("Cancel", (senderAlert, args) =>
            {
                bluetoothActivationDialog.Dispose();
            });

            bluetoothActivationDialog.Show();
        }

        private void runBluetoothConnectionDialog(DeviceBase clickedDevice)
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
            textViewConState.Text = clickedDevice.State == DeviceState.Connected ? "Connected" : "Disconnected";

            var editValueField = dialogView.FindViewById<EditText>(Resource.Id.userInput);
            dialogBuilder.SetCancelable(false)
                .SetPositiveButton("Connect", async delegate
                {
                    try
                    {
                        await bleAdapter.ConnectToDeviceAsync(clickedDevice);
                        refreshGui();
                        await receiveDataAsync(clickedDevice);
                    }
                    catch 
                    {
                        Toast.MakeText(Context, "Connection to Device failed!", ToastLength.Long).Show();
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

        private async Task receiveDataAsync(DeviceBase conDevice)
        {
            var service = await conDevice.GetServiceAsync(Guid.Parse(BluetoothServiceData.DIVE_SERVICE_ID));
            var characteristic = await service.GetCharacteristicAsync(Guid.Parse(BluetoothServiceData.DIVE_CHARACTERISTIC_ID));
            var bytes = await characteristic.ReadAsync();
            string result = System.Text.Encoding.Default.GetString(bytes).Split('}', StringSplitOptions.RemoveEmptyEntries)[0] + "}";

            Console.WriteLine(result);

            var DataConverter = new DiveDataConverter(result);

            var resultObject = DataConverter.toJsonObject();
            Console.WriteLine("Hierhin springen");
        }
    }
}
