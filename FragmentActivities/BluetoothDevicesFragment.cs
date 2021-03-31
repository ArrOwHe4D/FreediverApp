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
using System.Threading.Tasks;
using FreediverApp.DatabaseConnector;
using Newtonsoft.Json;
using Android.App;

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
        private FirebaseDataListener diveSessionListener;
        private List<DiveSession> diveSessionsFromDatabase;
        private ProgressDialog dataTransferDialog;

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

            listView = view.FindViewById<ListView>(Resource.Id.lv_con_devices);
            btnScan = view.FindViewById<Button>(Resource.Id.bt_scan_btn);

            scanIndicator = view.FindViewById<ProgressBar>(Resource.Id.scan_indicator);
            scanIndicator.Visibility = ViewStates.Gone;

            btnScan.Click += scanButtonOnClick;

            listView.ItemClick += ListView_ItemClick;

            diveSessionListener = new FirebaseDataListener();
            diveSessionsFromDatabase = new List<DiveSession>();
            retrieveDiveSessions();

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
            textViewMacAddress.Text = "MAC:" + clickedDevice.NativeDevice.ToString();
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

                        TemporaryData.CONNECTED_DIVE_COMPUTER = clickedDevice.Name != null ? clickedDevice.Name : "No Device Connected";

                        dataTransferDialog = new ProgressDialog(Context);
                        dataTransferDialog.SetMessage("Transfering session data from arduino...");
                        dataTransferDialog.SetCancelable(false);
                        dataTransferDialog.Show();

                        List<DiveSession> diveSessions = await receiveDataAsync(clickedDevice);

                        dataTransferDialog.Dismiss();

                        List<string> existingSessions = new List<string>();
                        FirebaseDataListener database = new FirebaseDataListener();
                        string id = null;
                        if (diveSessionsFromDatabase != null)
                        {
                            foreach (DiveSession dsDB in diveSessionsFromDatabase)
                            {
                                foreach (DiveSession ds in diveSessions)
                                {
                                    if (dsDB.date == ds.date)
                                    {
                                        existingSessions.Add(ds.date);
                                        foreach (Dive d in ds.dives)
                                        {
                                            d.refDivesession = dsDB.Id;
                                        }

                                    }
                                }
                            }
                        }                        

                        //save whole result set into database one by another
                        foreach (DiveSession DS in diveSessions)
                        {
                            foreach (Dive D in DS.dives)
                            {
                                foreach (Measurepoint MP in D.measurepoints)
                                {
                                    database.saveEntity("measurepoints", MP);
                                }
                                database.saveEntity("dives", D);
                            }

                            if (!existingSessions.Contains(DS.date))
                            {
                                DS.location_lat = "";
                                DS.location_lon = "";
                                DS.weatherCondition_description = "";
                                DS.weatherCondition_main = "";
                                DS.weatherHumidity = "";
                                DS.weatherPressure = "";
                                DS.weatherTemperature = "";
                                DS.weatherTemperatureFeelsLike = "";
                                DS.weatherWindGust = "";
                                DS.weatherWindSpeed = "";
                                database.saveEntity("divesessions", DS);
                            }
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

        private async Task<List<DiveSession>> receiveDataAsync(DeviceBase conDevice)
        {
            conDevice.UpdateConnectionInterval(ConnectionInterval.High);
            //after a connection was established we need to read the service and characteristic data from arduino side
            var service = await conDevice.GetServiceAsync(new Guid(BluetoothServiceData.DIVE_SERVICE_ID));            
            var characteristics = await service.GetCharacteristicsAsync();
            var chara_ack = await service.GetCharacteristicAsync(new Guid(BluetoothServiceData.CHARACTERISTIC_ACK));
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
            bool sendAcknowledgement = false;
            int divecount = 0;
            bool quit = false;

            foreach (ICharacteristic chara in characteristics)
            {
                try
                {
                    chara.ValueUpdated += (o, args) =>
                    {
                        if (BluetoothServiceData.CHARACTERISTIC_ACCELERATOR_X == chara.Uuid)
                        {
                            acc_x_List.Add(BitConverter.ToSingle(args.Characteristic.Value).ToString());
                        }
                        else if (BluetoothServiceData.CHARACTERISTIC_ACCELERATOR_Y == chara.Uuid)
                        {
                            acc_y_List.Add(BitConverter.ToSingle(args.Characteristic.Value).ToString());
                        }
                        else if (BluetoothServiceData.CHARACTERISTIC_ACCELERATOR_Z == chara.Uuid)
                        {
                            acc_z_List.Add(BitConverter.ToSingle(args.Characteristic.Value).ToString());
                        }
                        else if (BluetoothServiceData.CHARACTERISTIC_DEPTH == chara.Uuid)
                        {
                            depth_List.Add(BitConverter.ToSingle(args.Characteristic.Value).ToString());
                        }
                        else if (BluetoothServiceData.CHARACTERISTIC_DURATION == chara.Uuid)
                        {
                            dur_List.Add(BitConverter.ToSingle(args.Characteristic.Value).ToString());
                        }
                        else if (BluetoothServiceData.CHARACTERISTIC_GYROSCOPE_X == chara.Uuid)
                        {
                            gyro_x_List.Add(BitConverter.ToSingle(args.Characteristic.Value).ToString());
                        }
                        else if (BluetoothServiceData.CHARACTERISTIC_GYROSCOPE_Y == chara.Uuid)
                        {
                            gyro_y_List.Add(BitConverter.ToSingle(args.Characteristic.Value).ToString());
                        }
                        else if (BluetoothServiceData.CHARACTERISTIC_GYROSCOPE_Z == chara.Uuid)
                        {
                            gyro_z_List.Add(BitConverter.ToSingle(args.Characteristic.Value).ToString());
                        }
                        else if (BluetoothServiceData.CHARACTERISTIC_HEART_FREQ == chara.Uuid)
                        {
                            heart_freq_List.Add(BitConverter.ToInt32(args.Characteristic.Value).ToString());
                        }
                        else if (BluetoothServiceData.CHARACTERISTIC_HEART_VAR == chara.Uuid)
                        {
                            heart_var_List.Add(BitConverter.ToInt32(args.Characteristic.Value).ToString());
                        }
                        else if (BluetoothServiceData.CHARACTERISTIC_LUMINANCE == chara.Uuid)
                        {
                            lumi_List.Add(BitConverter.ToInt32(args.Characteristic.Value).ToString());
                        }
                        else if (BluetoothServiceData.CHARACTERISTIC_OXYGEN_SATURATION == chara.Uuid)
                        {
                            oxy_List.Add(BitConverter.ToInt32(args.Characteristic.Value).ToString());
                        }
                        else if (BluetoothServiceData.CHARACTERISTIC_REF_DIVE == chara.Uuid)
                        {
                            ref_List.Add(BitConverter.ToInt32(args.Characteristic.Value).ToString());
                        }
                        else if (BluetoothServiceData.CHARACTERISTIC_WATER_TEMP == chara.Uuid)
                        {
                            water_List.Add(BitConverter.ToSingle(args.Characteristic.Value).ToString());
                        }
                        else if (BluetoothServiceData.CHARACTERISTIC_DATETIME == chara.Uuid)
                        {
                            string message = System.Text.Encoding.ASCII.GetString(args.Characteristic.Value);
                            message = message.Split('}')[0] + "}";
                            var temp = JsonConvert.DeserializeObject<Dictionary<string, object>>(message);
                            if (temp.ContainsKey("Date"))
                            {
                                DiveSession d = new DiveSession(TemporaryData.CURRENT_USER.id);
                                diveSessions.Add(d);
                                d.date = temp["Date"].ToString().Replace('_', '.').Insert(6, "20");
                                Dive dive = new Dive();
                                divecount = 0;
                                sendAcknowledgement = true;
                            }
                            else if (temp.ContainsKey("Time"))
                            {                                
                                Dive dive = new Dive(diveSessions.Last().Id, divecount.ToString());
                                dive.timestampBegin = temp["Time"].ToString().Remove(0, 1);
                                
                                diveSessions.Last().dives.Add(dive);
                                if (divecount > 0)
                                {
                                    diveSessions.Last().dives.ElementAt(diveSessions.Last().dives.Count - 2).measurepoints =
                                        createMeasurepoints(diveSessions.Last().dives.ElementAt(diveSessions.Last().dives.Count - 2).id,
                                                            acc_x_List,
                                                            acc_y_List,
                                                            acc_z_List,
                                                            depth_List,
                                                            dur_List,
                                                            gyro_x_List,
                                                            gyro_y_List,
                                                            gyro_z_List,
                                                            heart_freq_List,
                                                            heart_var_List,
                                                            lumi_List,
                                                            oxy_List,
                                                            ref_List,
                                                            water_List);
                                }
                                divecount++;
                                sendAcknowledgement = true;
                            }
                            else if (temp.ContainsKey("EoS"))
                            {
                                diveSessions.Last().dives.Last().measurepoints =
                                    createMeasurepoints(diveSessions.Last().dives.Last().id,
                                                        acc_x_List,
                                                        acc_y_List,
                                                        acc_z_List,
                                                        depth_List,
                                                        dur_List,
                                                        gyro_x_List,
                                                        gyro_y_List,
                                                        gyro_z_List,
                                                        heart_freq_List,
                                                        heart_var_List,
                                                        lumi_List,
                                                        oxy_List,
                                                        ref_List,
                                                        water_List);
                                sendAcknowledgement = true;
                            }
                            else if (temp.ContainsKey("Terminate"))
                            {
                                sendAcknowledgement = true;
                                quit = true;
                            }                            
                        }                        
                    };
                    if (chara.Uuid != BluetoothServiceData.CHARACTERISTIC_ACK)
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

            while (conDevice.State == DeviceState.Connected)
            {
                if (sendAcknowledgement)
                {
                    if (sendAcknowledgement)
                    {
                        await chara_ack.WriteAsync(new Byte[] { Convert.ToByte(1) });
                    }
                    if (sendAcknowledgement)
                    {
                        await chara_ack.WriteAsync(new Byte[] { Convert.ToByte(1) });
                    }
                    sendAcknowledgement = false;
                    if (quit)
                    {
                        foreach (DiveSession ds in diveSessions)
                        {
                            ds.UpdateAll();
                        }
                        return diveSessions;
                    }
                }
            }

            foreach (DiveSession ds in diveSessions)
            {
                ds.UpdateAll();
            }
            return diveSessions;
        }

        List<Measurepoint> createMeasurepoints
            (string diveID,
             List<string> acc_x_List,
             List<string> acc_y_List,
             List<string> acc_z_List,
             List<string> depth_List,
             List<string> dur_List,
             List<string> gyro_x_List,
             List<string> gyro_y_List,
             List<string> gyro_z_List,
             List<string> heart_freq_List,
             List<string> heart_var_List,
             List<string> lumi_List,
             List<string> oxy_List,
             List<string> ref_List,
             List<string> water_List)
        {
            List<Measurepoint> mp_List = new List<Measurepoint>();
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
                mp.ref_dive = diveID;
                mp.water_temp = water_List[i];
                mp_List.Add(mp);
            }
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
            return mp_List;
        }

        private void retrieveDiveSessions()
        {
            diveSessionListener.QueryParameterized("divesessions", "ref_user", TemporaryData.CURRENT_USER.id);
            diveSessionListener.DataRetrieved += database_diveSessionDataRetrieved;
        }

        private void database_diveSessionDataRetrieved(object sender, FirebaseDataListener.DataEventArgs args)
        {
            diveSessionsFromDatabase = args.DiveSessions;
        }
    }
}
