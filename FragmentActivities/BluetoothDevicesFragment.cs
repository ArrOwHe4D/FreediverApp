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
using Newtonsoft.Json;

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

            measurepointsListener = new FirebaseDataListener();

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
                        await bleAdapter.StopScanningForDevicesAsync();
                        refreshGui();

                        List<Measurepoint> measurepointResult = await receiveDataAsync(clickedDevice);

                        //save whole result set into database one by another
;                       foreach (var measurepoint in measurepointResult) 
                        {
                            measurepointsListener.saveEntity("measurepoints", measurepoint);
                        }
                    }
                    catch(Exception ex)
                    {
                        Console.WriteLine(ex.Message);
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
            //after a connection was established we need to read the service and characteristic data from arduino side
            var service = await conDevice.GetServiceAsync(new Guid(BluetoothServiceData.DIVE_SERVICE_ID));            
            var characteristics = await service.GetCharacteristicsAsync();
            var chara_ack = await service.GetCharacteristicAsync(new Guid(BluetoothServiceData.characteristic_ack));
            List<Measurepoint> measurepoints = new List<Measurepoint>();
            List<DiveSession> diveSessions = new List<DiveSession>();
            

            List<string> acc_x_List = new List<string>();
            List<string> acc_y_List = new List<string>();
            List<string> acc_z_List = new List<string>();
            List<string> depth_List = new List<string>();
            List<string> dur_List = new List<string>();
            List<string> gyro_x_List = new List<string>();
            List<string> gyro_y_List = new List<string>();
            List<string> gyro_z_List = new List<string>();
            List<string> heart_freq_List = new List<string>();
            List<string> heart_var_List = new List<string>();
            List<string> lumi_List = new List<string>();
            List<string> oxy_List = new List<string>();
            List<string> ref_List = new List<string>();
            List<string> water_List = new List<string>();
            bool doit = false;
            bool doitd = false;
            int divecount = 0;
            bool quit = false;

            foreach (ICharacteristic chara in characteristics)
            {
                try
                {
                    chara.ValueUpdated += (o, args) =>
                    {
                        if (BluetoothServiceData.characteristic_acceleator_x == chara.Uuid)
                        {
                            acc_x_List.Add(BitConverter.ToSingle(args.Characteristic.Value).ToString());
                        }
                        else if (BluetoothServiceData.characteristic_acceleator_y == chara.Uuid)
                        {
                            acc_y_List.Add(BitConverter.ToSingle(args.Characteristic.Value).ToString());
                        }
                        else if (BluetoothServiceData.characteristic_acceleator_z == chara.Uuid)
                        {
                            acc_z_List.Add(BitConverter.ToSingle(args.Characteristic.Value).ToString());
                        }
                        else if (BluetoothServiceData.characteristic_depth == chara.Uuid)
                        {
                            depth_List.Add(BitConverter.ToSingle(args.Characteristic.Value).ToString());
                        }
                        else if (BluetoothServiceData.characteristic_duration == chara.Uuid)
                        {
                            dur_List.Add(BitConverter.ToSingle(args.Characteristic.Value).ToString());
                        }
                        else if (BluetoothServiceData.characteristic_gyroscope_x == chara.Uuid)
                        {
                            gyro_x_List.Add(BitConverter.ToSingle(args.Characteristic.Value).ToString());
                        }
                        else if (BluetoothServiceData.characteristic_gyroscope_y == chara.Uuid)
                        {
                            gyro_y_List.Add(BitConverter.ToSingle(args.Characteristic.Value).ToString());
                        }
                        else if (BluetoothServiceData.characteristic_gyroscope_z == chara.Uuid)
                        {
                            gyro_z_List.Add(BitConverter.ToSingle(args.Characteristic.Value).ToString());
                        }
                        else if (BluetoothServiceData.characteristic_heart_freq == chara.Uuid)
                        {
                            heart_freq_List.Add(BitConverter.ToInt32(args.Characteristic.Value).ToString());
                        }
                        else if (BluetoothServiceData.characteristic_heat_var == chara.Uuid)
                        {
                            heart_var_List.Add(BitConverter.ToInt32(args.Characteristic.Value).ToString());
                        }
                        else if (BluetoothServiceData.characteristic_luminance == chara.Uuid)
                        {
                            lumi_List.Add(BitConverter.ToInt32(args.Characteristic.Value).ToString());
                        }
                        else if (BluetoothServiceData.characteristic_oxygen_saturation == chara.Uuid)
                        {
                            oxy_List.Add(BitConverter.ToInt32(args.Characteristic.Value).ToString());
                        }
                        else if (BluetoothServiceData.characteristic_ref_dive == chara.Uuid)
                        {
                            ref_List.Add(BitConverter.ToInt32(args.Characteristic.Value).ToString());
                        }
                        else if (BluetoothServiceData.characteristic_water_temp == chara.Uuid)
                        {
                            water_List.Add(BitConverter.ToSingle(args.Characteristic.Value).ToString());
                        }
                        else if (BluetoothServiceData.characteristic_datetime == chara.Uuid)
                        {
                            string message = System.Text.Encoding.ASCII.GetString(args.Characteristic.Value);
                            message = message.Split('}')[0] + "}";
                            var temp = JsonConvert.DeserializeObject<Dictionary<string, object>>(message);
                            if (temp.ContainsKey("Date"))
                            {
                                DiveSession d = new DiveSession();
                                diveSessions.Add(d);
                                d.date = temp["Date"].ToString();
                                Dive dive = new Dive();
                                divecount = 0;
                                doit = true;
                            }
                            else if (temp.ContainsKey("Time"))
                            {                                
                                Dive dive = new Dive(diveSessions.Last().Id, divecount.ToString());
                                dive.timestampBegin = temp["Time"].ToString();
                                
                                diveSessions.Last().dives.Add(dive);
                                if (divecount > 0)
                                {
                                    for (int i = 0; i < water_List.Count; i++)
                                    {
                                        Measurepoint mp = new Measurepoint();
                                        mp.accelerator_x = acc_x_List[i];
                                        mp.accelerator_y = acc_y_List[i];
                                        mp.accelerator_z = acc_z_List[i];
                                        mp.depth = depth_List[i];
                                        mp.duration = dur_List[i];
                                        mp.gyroscope_x = gyro_x_List[i];
                                        mp.gyroscope_y = gyro_y_List[i];
                                        mp.gyroscope_z = gyro_z_List[i];
                                        mp.heart_freq = heart_freq_List[i];
                                        mp.heart_var = heart_var_List[i];
                                        mp.luminance = lumi_List[i];
                                        mp.oxygen_saturation = oxy_List[i];
                                        mp.ref_dive = ref_List[i];
                                        mp.water_temp = water_List[i];
                                        diveSessions.Last().dives.ElementAt(diveSessions.Last().dives.Count - 2).measurepoints.Add(mp);
                                    }                                
                                }
                                divecount++;
                                acc_x_List.Clear();
                                acc_y_List.Clear();
                                acc_z_List.Clear();
                                depth_List.Clear();
                                dur_List.Clear();
                                gyro_x_List.Clear();
                                gyro_y_List.Clear();
                                gyro_z_List.Clear();
                                heart_freq_List.Clear();
                                heart_var_List.Clear();
                                lumi_List.Clear();
                                oxy_List.Clear();
                                ref_List.Clear();
                                water_List.Clear();
                                doitd = true;
                            }
                            else if (temp.ContainsKey("Terminate"))
                            {
                                for (int i = 0; i < water_List.Count; i++)
                                {
                                    Measurepoint mp = new Measurepoint();
                                    mp.accelerator_x = acc_x_List[i];
                                    mp.accelerator_y = acc_y_List[i];
                                    mp.accelerator_z = acc_z_List[i];
                                    mp.depth = depth_List[i];
                                    mp.duration = dur_List[i];
                                    mp.gyroscope_x = gyro_x_List[i];
                                    mp.gyroscope_y = gyro_y_List[i];
                                    mp.gyroscope_z = gyro_z_List[i];
                                    mp.heart_freq = heart_freq_List[i];
                                    mp.heart_var = heart_var_List[i];
                                    mp.luminance = lumi_List[i];
                                    mp.oxygen_saturation = oxy_List[i];
                                    mp.ref_dive = ref_List[i];
                                    mp.water_temp = water_List[i];
                                    diveSessions.Last().dives.Last().measurepoints.Add(mp);
                                }
                                doit = true;
                                quit = true;
                            }                            
                        }                        
                    };
                    if (chara.Uuid != BluetoothServiceData.characteristic_ack)
                    {
                        await chara.StartUpdatesAsync();
                    }                    
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }  
            }

            await chara_ack.WriteAsync(new Byte[] { Convert.ToByte(1) });

            conDevice.UpdateConnectionInterval(ConnectionInterval.High);

            while (conDevice.State == DeviceState.Connected)
            {
                if (doit || doitd)
                {
                    if (doit)
                    {
                        await chara_ack.WriteAsync(new Byte[] { Convert.ToByte(1) });
                    }
                    if (doitd)
                    {
                        await chara_ack.WriteAsync(new Byte[] { Convert.ToByte(1) });
                    }
                    doit = false;
                    doitd = false;
                    if (quit)
                    {
                        return measurepoints;
                    }
                }
            }

            return measurepoints;
        }

        private bool isJson(string m)
        {            
            return m.StartsWith('{') && m.EndsWith('}');
        }

        private List<DiveSession> processData(List<string> rawData)
        {
            List<DiveSession> diveSessions = new List<DiveSession>();
            int divecount = 0;
            foreach (var item in rawData)
            {
                if (isJson(item))
                {
                    var temp = JsonConvert.DeserializeObject<Dictionary<string, object>>(item);
                    if (temp.ContainsKey("Date"))
                    {
                        DiveSession ds = new DiveSession(TemporaryData.CURRENT_USER.id);
                        // add Dive Session Data
                        ds.date = temp["Date"].ToString();
                        //ds.GetMetaData();
                        //
                        diveSessions.Add(ds);
                        divecount = 0;
                    }
                    else if (temp.ContainsKey("Time"))
                    {
                        Dive d = new Dive(diveSessions.Last().Id, divecount.ToString());
                        d.timestampBegin = temp["Time"].ToString();
                        divecount++;
                        diveSessions.Last().dives.Add(d);
                    }
                    else
                    {
                        try
                        {
                            diveSessions.Last().dives.Last().measurepoints.Add(Measurepoint.fromJson(item));
                        }
                        catch (Exception)
                        {
                            
                        }
                    }
                }
            }

            foreach (var session in diveSessions)
            {
                foreach (var dive in session.dives)
                {
                    dive.UpdateAll();
                }
                session.UpdateDuration();
            }

            return diveSessions;
        }
    }
}
