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
using FreediverApp.DatabaseConnector;

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
        private FirebaseDataListener measurepointsListener;


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
                Toast.MakeText(Context, Resource.String.bluetooth_not_supported, ToastLength.Long).Show();
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
            bluetoothActivationDialog.SetTitle(Resource.String.dialog_bluetooth_not_activated);
            bluetoothActivationDialog.SetMessage(Resource.String.dialog_do_you_want_to_activate_blueooth);

            bluetoothActivationDialog.SetPositiveButton(Resource.String.dialog_accept, (senderAlert, args) =>
            {
                btReceiver.m_adapter.Enable();

                Thread.Sleep(2500);

                refreshBleAdapter();

                if (btReceiver.m_adapter.IsEnabled)
                {
                    Toast.MakeText(Context, Resource.String.bluetooth_activated, ToastLength.Long).Show();
                }
                else
                {
                    Toast.MakeText(Context, Resource.String.bluetooth_activation_failed, ToastLength.Long).Show();
                }
            });
            bluetoothActivationDialog.SetNegativeButton(Resource.String.dialog_cancel, (senderAlert, args) =>
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
            dialogBuilder.SetTitle(Resource.String.dialog_connect_to_device);
            dialogBuilder.SetIcon(Resources.GetDrawable(Resource.Drawable.icon_connected_devices));

            var textViewDeviceName = dialogView.FindViewById<TextView>(Resource.Id.textview_device_name);
            var textViewMacAddress = dialogView.FindViewById<TextView>(Resource.Id.textview_mac_address);
            var textViewConState = dialogView.FindViewById<TextView>(Resource.Id.textview_con_state);

            textViewDeviceName.Text = clickedDevice.Name;
            textViewMacAddress.Text = clickedDevice.Id.ToString();
            textViewConState.Text = clickedDevice.State == DeviceState.Connected ? "Connected" : "Disconnected";

            var editValueField = dialogView.FindViewById<EditText>(Resource.Id.userInput);
            dialogBuilder.SetCancelable(false)
                .SetPositiveButton(Resource.String.dialog_connect, async delegate
                {
                    try
                    {
                        await bleAdapter.ConnectToDeviceAsync(clickedDevice);
                        refreshGui();


                        //List<string> list = await receiveDataAsync(clickedDevice);


                        List<Measurepoint> list = await receiveDataAsync(clickedDevice);
                        //saveInDatabase(jsonObject);
                        Console.WriteLine("success :)");

                    }
                    catch(Exception ex)
                    {
                        string a = ex.Message;
                        Toast.MakeText(Context, Resource.String.connection_to_device_failed, ToastLength.Long).Show();
                        await bleAdapter.DisconnectDeviceAsync(clickedDevice);
                    }
                    dialogBuilder.Dispose();
                })
                .SetNegativeButton(Resource.String.dialog_cancel, delegate
                {
                    dialogBuilder.Dispose();
                });

            SupportV7.AlertDialog dialog = dialogBuilder.Create();
            dialog.Show();
        }





        private async Task<List<Measurepoint>> receiveDataAsync(DeviceBase conDevice)
        {
            List<Measurepoint> measurepoints = new List<Measurepoint>();

            int time = DateTime.Now.Millisecond;
            var service = await conDevice.GetServiceAsync(Guid.Parse(BluetoothServiceData.DIVE_SERVICE_ID));
            //var characteristics = await service.GetCharacteristicsAsync();                  
            var characteristicAcceleratorX = await service.GetCharacteristicAsync(Guid.Parse(BluetoothServiceData.characteristic_acceleator_x));
            Toast.MakeText(Context, "erste chara", ToastLength.Long).Show();
            var characteristicAcceleratorY = await service.GetCharacteristicAsync(Guid.Parse(BluetoothServiceData.characteristic_acceleator_y));
            var characteristicAcceleratorZ = await service.GetCharacteristicAsync(Guid.Parse(BluetoothServiceData.characteristic_acceleator_z));
            var characteristicDepth = await service.GetCharacteristicAsync(Guid.Parse(BluetoothServiceData.characteristic_depth));
            var characteristicDuration = await service.GetCharacteristicAsync(Guid.Parse(BluetoothServiceData.characteristic_duration));
            var characteristicGyroscopeX = await service.GetCharacteristicAsync(Guid.Parse(BluetoothServiceData.characteristic_gyroscope_x));
            var characteristicGyroscopeY = await service.GetCharacteristicAsync(Guid.Parse(BluetoothServiceData.characteristic_gyroscope_y));
            var characteristicGyroscopeZ = await service.GetCharacteristicAsync(Guid.Parse(BluetoothServiceData.characteristic_gyroscope_z));
            var characteristicHeartFreq = await service.GetCharacteristicAsync(Guid.Parse(BluetoothServiceData.characteristic_heart_freq));
            var characteristicHeartVar = await service.GetCharacteristicAsync(Guid.Parse(BluetoothServiceData.characteristic_heat_var));
            var characteristicLuminance = await service.GetCharacteristicAsync(Guid.Parse(BluetoothServiceData.characteristic_luminance));
            var characteristicOxygenSaturation = await service.GetCharacteristicAsync(Guid.Parse(BluetoothServiceData.characteristic_oxygen_saturation));            
            var characteristicRefDive = await service.GetCharacteristicAsync(Guid.Parse(BluetoothServiceData.characteristic_ref_dive));            
            var characteristicWaterTemp = await service.GetCharacteristicAsync(Guid.Parse(BluetoothServiceData.characteristic_water_temp));
            Console.WriteLine("chara read: " + (DateTime.Now.Millisecond - time) + " ms");
            Toast.MakeText(Context, "alle chara", ToastLength.Long).Show();            
            Measurepoint tempMeasurepoint = new Measurepoint();
            byte[] tempBytes;

            try
            {
                //for (int i = 0; i < 1; i++)
                while (conDevice.State == DeviceState.Connected)
                {
                    time = DateTime.Now.Millisecond;
                    tempMeasurepoint = new Measurepoint();

                    tempBytes = await characteristicAcceleratorX.ReadAsync();
                    tempMeasurepoint.accelerator_x = BitConverter.ToSingle(tempBytes).ToString();

                    Toast.MakeText(Context, "erste Übertragung", ToastLength.Long).Show();

                    tempBytes = await characteristicAcceleratorY.ReadAsync();
                    tempMeasurepoint.accelerator_y = BitConverter.ToSingle(tempBytes).ToString();

                    tempBytes = await characteristicAcceleratorZ.ReadAsync();
                    tempMeasurepoint.accelerator_z = BitConverter.ToSingle(tempBytes).ToString();

                    tempBytes = await characteristicDepth.ReadAsync();
                    tempMeasurepoint.depth = System.BitConverter.ToSingle(tempBytes).ToString();

                    tempBytes = await characteristicDuration.ReadAsync();
                    tempMeasurepoint.duration = BitConverter.ToInt32(tempBytes).ToString();

                    tempBytes = await characteristicGyroscopeX.ReadAsync();
                    tempMeasurepoint.gyroscope_x = BitConverter.ToSingle(tempBytes).ToString();

                    tempBytes = await characteristicGyroscopeY.ReadAsync();
                    tempMeasurepoint.gyroscope_y = BitConverter.ToSingle(tempBytes).ToString();

                    tempBytes = await characteristicGyroscopeZ.ReadAsync();
                    tempMeasurepoint.gyroscope_z = BitConverter.ToSingle(tempBytes).ToString();

                    tempBytes = await characteristicHeartFreq.ReadAsync();
                    tempMeasurepoint.heart_freq = BitConverter.ToInt32(tempBytes).ToString();

                    tempBytes = await characteristicHeartVar.ReadAsync();
                    tempMeasurepoint.heart_var = BitConverter.ToInt32(tempBytes).ToString();

                    tempBytes = await characteristicLuminance.ReadAsync();
                    tempMeasurepoint.luminance = BitConverter.ToInt32(tempBytes).ToString();

                    tempBytes = await characteristicOxygenSaturation.ReadAsync();
                    tempMeasurepoint.oxygen_saturation = BitConverter.ToInt32(tempBytes).ToString();
                    
                    tempBytes = await characteristicRefDive.ReadAsync();
                    tempMeasurepoint.ref_dive = BitConverter.ToInt32(tempBytes).ToString();

                    tempBytes = await characteristicWaterTemp.ReadAsync();
                    tempMeasurepoint.water_temp = BitConverter.ToSingle(tempBytes).ToString();
                    Console.WriteLine("measure read: " + (DateTime.Now.Millisecond - time) + " ms");
                    measurepoints.Add(tempMeasurepoint);                    
                }
            }
            catch (Exception ex)
            {
                Toast.MakeText(Context, ex.Message, ToastLength.Long).Show();
            }
            // chara werden empfangen aber langsam jedes 14. von denen die gesendet werden
            // die charas kommen von verschiedenen measurepoints
            // müssen eine chara auf android seite erstellen die dem arduino zu erkennen gibt wann
            // ein Measurepoint gelesen wurde
            return measurepoints;
        }

        private void saveInDatabase(object JSONObject)
        {
            measurepointsListener = new FirebaseDataListener();
            measurepointsListener.saveEntity("measurepoints", JSONObject);
        }
    }
}
