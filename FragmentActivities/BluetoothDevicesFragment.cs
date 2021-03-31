using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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
using Android.Bluetooth;

namespace FreediverApp
{
    [Obsolete]
    public class BluetoothDevicesFragment : Fragment
    {
        /*Member Variables including UI components from XML and all needed BLE components*/
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

        /**
         *  This function initializes all UI and BLE components. 
         **/
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.BluetoothDevicesPage, container, false);

            //setup default bluetooth adapter
            btReceiver = new BluetoothDeviceReceiver();
            btReceiver.m_adapter = BluetoothAdapter.DefaultAdapter;

            //setup the bluetooth low energy component
            ble = CrossBluetoothLE.Current;

            //setup the BLE adapter to be able to scan and connect
            bleAdapter = CrossBluetoothLE.Current.Adapter;

            //set the scan interval to 5 seconds and set a callback function that stops the scan process and the animation
            bleAdapter.ScanTimeout = 5000;
            bleAdapter.ScanTimeoutElapsed += stopScan;

            //init device list to store scanned bluetooth devices
            bleDeviceList = new ObservableCollection<IDevice>();

            //init ui components
            listView = view.FindViewById<ListView>(Resource.Id.lv_con_devices);
            btnScan = view.FindViewById<Button>(Resource.Id.bt_scan_btn);

            scanIndicator = view.FindViewById<ProgressBar>(Resource.Id.scan_indicator);
            scanIndicator.Visibility = ViewStates.Gone;

            //setup onclick listeners for scan button and listview items
            btnScan.Click += scanButtonOnClick;
            listView.ItemClick += ListView_ItemClick;

            //setup the db listener to be able to query data from firebase
            diveSessionListener = new FirebaseDataListener();

            //query all divesessions of the current user since we need to determine if a session already exists when receiving data from arduino

            retrieveDiveSessions();

            //different error handlings for errors that can occur while initializing ble
            if (ble == null)
            {
                //If we cannot init ble print a error message in form of a android toast
                Toast.MakeText(Context, Resource.String.bluetooth_not_supported, ToastLength.Long).Show();
            }
            if (ble.State == BluetoothState.Off)
            {
                //If bluetooth is turned off, print a dialog that asks the user to activate bluetooth
                runBluetoothActivationDialog();
            }
            else
            {
                //If bluetooth is on, fill the listview with already connected and paired devices from ble adapter
                initBluetoothListView();
            }
            return view;
        }

        /**
         *  This function reinitializes the BLE adapter. Is used when the user accepted the dialog
         *  to activate bluetooth on his phone.
         **/
        private void refreshBleAdapter()
        {
            ble = CrossBluetoothLE.Current;
            bleAdapter = CrossBluetoothLE.Current.Adapter;
            bleAdapter.ScanTimeout = 5000;
            bleAdapter.ScanTimeoutElapsed += stopScan;
        }

        /**
         *  When a device inside the bluetooth listview was clicked, run the connection dialog
         *  that displays the device information and initiates the data transmission between the dive
         *  computer and the app.
         **/
        private void ListView_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            var device = (DeviceBase)bleDeviceList[e.Position];
            runBluetoothConnectionDialog(device);
        }

        /**
         *  This function initiates the scan process to find bluetooth devices in near range.
         *  It is defined as async so we wait for a scan result asynchronously and add all found devices
         *  to our list that is passed to the listview in form of a adapter component.
         **/
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
                //error handling if bluetooth is off when user tries to scan, run the activation dialog to force the user to activate ble first
                runBluetoothActivationDialog();
            }
        }

        /**
         *  This callback function is called when the scan process of the ble adapter ended.
         *  It simply makes the loading animation that indicates that the scan process is running invisible.
         **/
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

        /**
         *  This function returns a interface that holds all known or bonded ble devices (connected + paired devices) 
         **/
        private IReadOnlyList<IDevice> getBondedBluetoothDevices()
        {
            return bleAdapter.GetSystemConnectedOrPairedDevices();
        }

        /**
         *  This function initializes the listview with all known bluetooth devices and refreshes the GUI at the end. 
         **/
        private void initBluetoothListView()
        {
            foreach (var device in getBondedBluetoothDevices())
            {
                bleDeviceList.Add(device);
            }

            refreshGui();
        }

        /**
         *  This function reinitializes the listview adapter to populate the GUI with new data that was
         *  returned by the ble adapter in form of a list of bt devices.
         **/
        private void refreshGui()
        {
            listView.Adapter = new CustomListViewAdapter(bleDeviceList);
        }

        /**
         *  This function builds a modal dialog that forces the user to either accept or decline 
         *  the activation of the bluetooth adapter of his phone.
         **/
        private void runBluetoothActivationDialog()
        {
            //setup UI for the activation dialog
            SupportV7.AlertDialog.Builder bluetoothActivationDialog = new SupportV7.AlertDialog.Builder(Context);
            bluetoothActivationDialog.SetTitle(Resource.String.dialog_bluetooth_not_activated);
            bluetoothActivationDialog.SetMessage(Resource.String.dialog_do_you_want_to_activate_blueooth);

            bluetoothActivationDialog.SetPositiveButton(Resource.String.dialog_accept, (senderAlert, args) =>
            {
                //lambda expression that handles the case that the user accepted to activate bluetooth.
                btReceiver.m_adapter.Enable();

                //We let the main thread sleep for 2,5 sec since we encountered that on some phones it takes a bit to activate bluetooth 
                //and to prevent that activation is not working even if it would work, we wait some seconds to ensure that activation was successfull 
                //on each of the different custom android versions from different manufacturers. 
                //(Should have something to do with how internal system calls are handled on different android versions)
                Thread.Sleep(2500);

                //reinitialize the ble adapter after we waited for android to enable it
                refreshBleAdapter();

                //print a toast message whether or not the bt adapter was sucessfully activated
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
                //close dialog on cancel
                bluetoothActivationDialog.Dispose();
            });

            bluetoothActivationDialog.Show();
        }

        /**
         *  This function shows a dialog that lets the user connect to the chosen device.
         *  After a connection was established, we begin to read the data from the arduino using the
         *  receiveDataAsync method we implemented below. While transfering data, we print a loading dialog onto 
         *  the screen that notifies the user that a data transmission is ongoing at the moment. 
         **/
        private void runBluetoothConnectionDialog(DeviceBase clickedDevice)
        {
            //build the dialog
            LayoutInflater layoutInflater = LayoutInflater.From(Context);
            View dialogView = layoutInflater.Inflate(Resource.Layout.BluetoothConnectionDialog, null);
            SupportV7.AlertDialog.Builder dialogBuilder = new SupportV7.AlertDialog.Builder(this.Context);
            dialogBuilder.SetView(dialogView);
            dialogBuilder.SetTitle(Resource.String.dialog_connect_to_device);
            dialogBuilder.SetIcon(Resources.GetDrawable(Resource.Drawable.icon_connected_devices));

            //init dialog textviews 
            var textViewDeviceName = dialogView.FindViewById<TextView>(Resource.Id.textview_device_name);
            var textViewMacAddress = dialogView.FindViewById<TextView>(Resource.Id.textview_mac_address);
            var textViewConState = dialogView.FindViewById<TextView>(Resource.Id.textview_con_state);

            //set textview values
            textViewDeviceName.Text = clickedDevice.Name;
            textViewMacAddress.Text = "MAC:" + clickedDevice.NativeDevice.ToString();
            textViewConState.Text = clickedDevice.State == DeviceState.Connected ? "Connected" : "Disconnected";

            dialogBuilder.SetCancelable(false)
                .SetPositiveButton(Resource.String.dialog_connect, async delegate
                {
                    try
                    {
                        //Asynchronously try to connect to the clicked ble device inside the listview
                        await bleAdapter.ConnectToDeviceAsync(clickedDevice);
                        await bleAdapter.StopScanningForDevicesAsync();
                        refreshGui();

                        //Set the currently connected dive computer SSID/Name inside temporary data to be able to read the value out in AddSessionActivity
                        TemporaryData.CONNECTED_DIVE_COMPUTER = clickedDevice.Name != null ? clickedDevice.Name : "No Device Connected";

                        //Create a Progressdialog with a loading animation that notifies the user that data is transferred between arduino and the app
                        dataTransferDialog = new ProgressDialog(Context);
                        dataTransferDialog.SetMessage("Transfering session data from arduino...");
                        dataTransferDialog.SetCancelable(false);
                        dataTransferDialog.Show();

                        //Call the method that handles the connection to arduino and receives the data 
                        List<DiveSession> diveSessions = await receiveDataAsync(clickedDevice);

                        //Close the Progressdialog after transmission is completed
                        dataTransferDialog.Dismiss();

                        //Store dates of existing sessions and setup a new listener to save Dives and Measurepoints after 
                        //creating the correct reference to a existing divesession
                        List<string> existingSessions = new List<string>();
                        FirebaseDataListener database = new FirebaseDataListener();

                        //Check if a divesession already exists in db and set the ref_divesession field to
                        //the id of the existing divesession to realize the 1:n relation of divession and dives
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

                        //save all divesessions from the result set into db
                        foreach (DiveSession DS in diveSessions)
                        {
                            //save all measurepoints and all dives to db
                            foreach (Dive D in DS.dives)
                            {
                                foreach (Measurepoint MP in D.measurepoints)
                                {
                                    database.saveEntity("measurepoints", MP);
                                }
                                database.saveEntity("dives", D);
                            }

                            //if the session not exists in db, set the divesession data to empty strings but save 
                            //it in db anyway without weather and location data so that we don´t loose any collected data from arduino side
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
                        //handle any error that occurs while connecting and transfering data
                        //disconnect from the device after the error was printed.
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

        /**
         *  This function is the key entry point for the data transfer between the arduino and our app. It is defined as async
         *  since we need to wait for the results to be received in order to proceed with saving them to the database. We use different
         *  characteristics to communicate with the arduino. Every characteristic is used to transfer a particular datapoint or sensor value.
         *  We first tried to transfer whole datasets for sessions in json format but this turned out to be way too slow to even consider using this
         *  technique. We managed to get a slightly better version running that is connection oriented and works with a bunch of single characteristics,
         *  that transfer one variable of each measurepoint that belongs to a dive. In future versions, we plan on switching to wifi using a proper tcp connection
         *  that can transfer way more data than the limited BLE protocol that only supports a MTU of 512 Byte. With wifi we will try to transmit whole files instead
         *  of truncated sensor values. With that technique, we wouldn´t need to rebuild all the data we receive from arduino side that would boost performance and 
         *  lower complexity.
         **/
        private async Task<List<DiveSession>> receiveDataAsync(DeviceBase conDevice)
        {
            //set connection interval to high to get a slight performance boost
            conDevice.UpdateConnectionInterval(ConnectionInterval.High);

            //after a connection was established we need to read the service and characteristic data from arduino side
            var service = await conDevice.GetServiceAsync(new Guid(BluetoothServiceData.DIVE_SERVICE_ID));            
            var characteristics = await service.GetCharacteristicsAsync();
            var chara_ack = await service.GetCharacteristicAsync(new Guid(BluetoothServiceData.CHARACTERISTIC_ACK));
            List<DiveSession> diveSessions = new List<DiveSession>();

            //Creation of string list for each sensor value that needs to be read from arduino side.
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

        /**
         *  This function creates Measurpoints based on the StringLists we received from the arduino by all the characteristics we read.
         **/
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

        /**
         *  This function queries for all divesessions that were created by the current user. We need
         *  this query to determine if a session already exists to connect the data read from arduino side with
         *  the already existing sessions. 
         **/
        private void retrieveDiveSessions()
        {
            diveSessionListener.QueryParameterized("divesessions", "ref_user", TemporaryData.CURRENT_USER.id);
            diveSessionListener.DataRetrieved += database_diveSessionDataRetrieved;
        }

        /**
         *  This function is called when divesession data was received by the db listener to check for existing sessions inside the list
         *  that is being set by this function. 
         **/
        private void database_diveSessionDataRetrieved(object sender, FirebaseDataListener.DataEventArgs args)
        {
            diveSessionsFromDatabase = args.DiveSessions;
        }
    }
}
