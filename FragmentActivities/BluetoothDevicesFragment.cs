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
using Android.Net.Wifi;
using Android.App;
//using Android.Net;
using FreediverApp.WifiCommunication;
using System.Net;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Sockets;

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
        private string ssid = "yournetworkname";
        private string pass = "yourcode";
        private WifiConnector wifiConnector;
        bool suggestNetwork;
        bool requestNetwork;
        bool alreadyConnected = false;

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


            wifiConnector = new WifiConnector();
            suggestNetwork = false;
            requestNetwork = false;

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

        public void httpRequest()
        {
            var rxcui = "198440";
            var request = HttpWebRequest.Create(string.Format(@"192.168.4.1", rxcui));
            request.ContentType = "text/plain";
            //request.ContentLength = 
            request.Method = "GET";

            using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
            {
                if (response.StatusCode != HttpStatusCode.OK)
                    Console.Out.WriteLine("Error fetching data. Server returned status code: {0}", response.StatusCode);
                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    var content = reader.ReadToEnd();
                    if (string.IsNullOrWhiteSpace(content))
                    {
                        Console.Out.WriteLine("Response contained empty body...");
                    }
                    else
                    {
                        Console.Out.WriteLine("Response Body: \r\n {0}", content);
                    }
                }
            }
        }

        private async void scanButtonOnClick(object sender, EventArgs eventArgs)
        {

            if (!alreadyConnected)
            {
                wifiConnector.SuggestNetwork();
                wifiConnector.RequestNetwork();
                alreadyConnected = true;
            }


            HttpClient _client = new HttpClient();
            _client.BaseAddress = new Uri("http://192.168.4.1/");
            //_client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/plain"));

            string resource = "";
            
            try
            {
                var request = new HttpRequestMessage()
                {
                    RequestUri = new Uri(_client.BaseAddress, resource),
                    Method = HttpMethod.Get,
                };
                var response = await _client.SendAsync(request);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var responseString = await response.Content.ReadAsStringAsync();
                    Console.WriteLine(responseString);
                }
                else if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    // you need to maybe re-authenticate here
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        

        //// Declare Variables
        //string host = "192.168.4.1";
        //int port = 80;
        //int timeout = 5000;

        //// Create TCP client and connect
        //// Then get the netstream and pass it
        //// To our StreamWriter and StreamReader
        //try
        //{
        //    var client = new TcpClient();

        //    // Asynchronsly attempt to connect to server
        //    await client.ConnectAsync(host, port);

        //    var netstream = client.GetStream();
        //    var writer = new StreamWriter(netstream);
        //    var reader = new StreamReader(netstream);

        //    // AutoFlush the StreamWriter
        //    // so we don't go over the buffer
        //    writer.AutoFlush = true;

        //    // Optionally set a timeout
        //    netstream.ReadTimeout = timeout;

        //    // Write a message over the TCP Connection
        //    string message = "Freedivergang";
        //    await writer.WriteLineAsync(message);

        //    // Read server response
        //    string response = await reader.ReadLineAsync();
        //    Console.WriteLine(string.Format($"Server: {response}"));
        //}
        //catch (Exception ex) 
        //{
        //    Console.WriteLine(ex.StackTrace);
        //}
        // The client and stream will close as control exits
        // the using block (Equivilent but safer than calling Close();



        //string ip = "192.168.4.1";

        //try
        //{
        //    // Create a TcpClient.
        //    // Note, for this client to work you need to have a TcpServer
        //    // connected to the same address as specified by the server, port
        //    // combination.
        //    Int32 port = 13000;
        //    TcpClient client = new TcpClient();
        //    client.Connect(new IPAddress(0xc0a80401), 80);

        //    // Translate the passed message into ASCII and store it as a Byte array.
        //    Byte[] data = System.Text.Encoding.ASCII.GetBytes("ABC");

        //    // Get a client stream for reading and writing.
        //    //  Stream stream = client.GetStream();

        //    NetworkStream stream = client.GetStream();

        //    // Send the message to the connected TcpServer.
        //    stream.Write(data, 0, data.Length);

        //    Console.WriteLine("Sent: {0}", "ABC");

        //    // Receive the TcpServer.response.

        //    // Buffer to store the response bytes.
        //    data = new Byte[256];

        //    // String to store the response ASCII representation.
        //    String responseData = String.Empty;

        //    // Read the first batch of the TcpServer response bytes.
        //    Int32 bytes = stream.Read(data, 0, data.Length);
        //    responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
        //    Console.WriteLine("Received: {0}", responseData);

        //    // Close everything.
        //    stream.Close();
        //    client.Close();
        //}
        //catch (ArgumentNullException e)
        //{
        //    Console.WriteLine("ArgumentNullException: {0}", e);
        //}
        //catch (SocketException e)
        //{
        //    Console.WriteLine("SocketException: {0}", e);
        //}


        //Byte[] data = System.Text.Encoding.ASCII.GetBytes("ABC");
        //var success = result.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(3));
        //if (!success)
        //{
        //    Console.WriteLine("Failed to connect...");
        //}
        //else
        //{
        //    Console.WriteLine("Connected...");
        //    NetworkStream stream = client.GetStream();

        //    client.SendTimeout = 2000;
        //    client.ReceiveTimeout = 8000;

        //    stream.Write(data, 0, data.Length);

        //    byte[] response = new byte[200];

        //    stream.Read(response, 0, response.Length);

        //    string responseString = System.Text.Encoding.ASCII.GetString(response, 0, response.Length);
        //    Console.WriteLine(responseString);

        //    client.Close();
        //}



        //if (true)
        //{
        //    HttpClient client = new HttpClient();
        //    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/html"));
        //    try
        //    {

        //        HttpResponseMessage response = client.GetAsync("http://192.168.4.1").Result;
        //    //string result = await client.GetStringAsync("http://192.168.4.1");

        //    if (response.IsSuccessStatusCode)
        //    {
        //        string result = await response.Content.ReadAsStringAsync();
        //        Console.WriteLine(result);

        //    }
        //}
        //    catch(Exception e)
        //    {
        //        Console.WriteLine(e);
        //        Console.WriteLine("HTTP GET REQUEST FAILED.");
        //    }
        //}










        //try
        //{
        //    WifiConfiguration wifiConfiguration = new WifiConfiguration();
        //    wifiConfiguration.Ssid = String.Format("\"%s\"", ssid);   //"\"yournetworkname\"";
        //    wifiConfiguration.PreSharedKey = String.Format("\"%s\"", pass);

        //    WifiManager wifiManager = (WifiManager)(Application.Context.GetSystemService(Android.Content.Context.WifiService));
        //    int networkId = wifiManager.ConnectionInfo.NetworkId;
        //    wifiManager.RemoveNetwork(networkId);
        //    wifiManager.SaveConfiguration();
        //    //remember id
        //    int netId = wifiManager.AddNetwork(wifiConfiguration);

        //    List<string> ssids = new List<string>();
        //    List<WifiConfiguration> list = wifiManager.ConfiguredNetworks.ToList();
        //    for (int i = 0; i < list.Count; i++)
        //    {
        //        ssids.Add(list.ElementAt(i).Ssid);
        //        if (list.ElementAt(i).Ssid != null && list.ElementAt(i).Ssid.Equals("\"" + ssid + "\""))
        //        {

        //            wifiManager.Disconnect();
        //            wifiManager.EnableNetwork(list.ElementAt(i).NetworkId, true);
        //            wifiManager.Reconnect();

        //            break;
        //        }
        //    }

        //    wifiManager.Disconnect();
        //    wifiManager.EnableNetwork(netId, true);
        //    wifiManager.Reconnect();
        //}
        //catch (Exception exp)
        //{
        //    Console.WriteLine(exp);
        //}


        //wifiConfiguration.PreSharedKey = pass;
        //wifiConfiguration.StatusField = WifiConfiguration.Status.Disabled;
        //wifiConfiguration.Priority = 40;
        //wifiConfiguration.HiddenSSID = true;
        //wifiConfiguration.NetworkId = 420;


        //WifiManager wifiManager = (WifiManager)Application.Context.GetSystemService(Android.Content.Context.WifiService);

        //if (!wifiManager.IsWifiEnabled)
        //    wifiManager.SetWifiEnabled(true);
        //else
        //    wifiManager.SetWifiEnabled(false);
        //WifiNetworkSuggestion suggestion;
        //List<WifiNetworkSuggestion> configList = new List<WifiNetworkSuggestion>();
        //configList.Add();
        //int netID = wifiManager.AddNetwork(wifiConfiguration);
        //wifiManager.AddNetworkSuggestions((List<WifiConfiguration>) configList);
        //wifiManager.SaveConfiguration();


        //wifiManager.Disconnect();
        //wifiManager.EnableNetwork(wifiConfiguration.NetworkId, true);
        //wifiManager.Reconnect();


        //List<WifiConfiguration> list = wifiManager.ConfiguredNetworks.ToList();
        //for (int i = 0; i < list.Count; i++)
        //{
        //    if (list.ElementAt(i).Ssid != null && list.ElementAt(i).Ssid.Equals("\"" + ssid + "\""))
        //    {
        //        wifiManager.Disconnect();
        //        wifiManager.EnableNetwork(list.ElementAt(i).NetworkId, true);
        //        wifiManager.Reconnect();

        //        break;
        //    }
        //}


        //ConnectToWifi();



        //if (ble.State == BluetoothState.On)
        //{
        //    //bleDeviceList.Clear();
        //    scanIndicator.Visibility = ViewStates.Visible;
        //    bleAdapter.DeviceDiscovered += (s, a) =>
        //    {
        //        if (!bleDeviceList.Contains(a.Device))
        //            bleDeviceList.Add(a.Device);

        //        refreshGui();
        //    };
        //    await bleAdapter.StartScanningForDevicesAsync();
        //}
        //else
        //{
        //    runBluetoothActivationDialog();
        //}
    }


        //private void ConnectToWifi()
        //{
        //    const string ssid = "\"yournetworkname\"";
        //    const string password = "\"yourcode\"";

        //    var wifiConfig = new WifiConfiguration
        //    {
        //        Ssid = ssid,
        //        PreSharedKey = password
        //    };

        //    var wifiManager = (WifiManager)Application.Context.GetSystemService(Android.Content.Context.WifiService);
        //    int test = wifiManager.UpdateNetwork(wifiConfig);
        //    var addNetwork = wifiManager.AddNetwork(wifiConfig);
        //    System.Diagnostics.Debug.WriteLine("addNetwork = " + addNetwork);

        //    var network = wifiManager.ConfiguredNetworks.FirstOrDefault((n) => n.Ssid == ssid);
        //    if (network == null)
        //    {
        //        System.Diagnostics.Debug.WriteLine("Didn't find the network, not connecting.");
        //        return;
        //    }


        //    wifiManager.Disconnect();

        //    var enableNetwork = wifiManager.EnableNetwork(network.NetworkId, true);
        //    System.Diagnostics.Debug.WriteLine("enableNetwork = " + enableNetwork);

        //    if (wifiManager.Reconnect())
        //    {
        //        Console.WriteLine("connected");
        //    }
        //    else
        //    {
        //        Console.WriteLine("nope");
        //    }
        //}

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
            var characteristic = await service.GetCharacteristicAsync(new Guid(BluetoothServiceData.DIVE_CHARACTERISTIC_ID));
            conDevice.UpdateConnectionInterval(ConnectionInterval.High);

            List<string> resList = new List<string>();

            characteristic.ValueUpdated += (o, args) =>
            {
                var bytes = args.Characteristic.Value;
                string result = System.Text.Encoding.ASCII.GetString(bytes);
                resList.Add(result);
            };

            await characteristic.StartUpdatesAsync();

            //create the result list which will be returned and set connection interval to high for a small performance boost
            List<Measurepoint> measurepoints = new List<Measurepoint>();           

            while (false)//conDevice.State == DeviceState.Connected)
            {
                var bytes = await characteristic.ReadAsync();
                string result = System.Text.Encoding.ASCII.GetString(bytes);

                ////split the bluetooth result into single json strings for each measurepoint received (3 at each transmission)
                ////json strings are recognized by their closing bracket
                List<string> jsonResult = result.Split('}').ToList();

                ////iterate through the received measurepoints and add a new closing bracket 
                ////since we removed it above to avoid adding a special separator for our result set, then add every entry to the result list 
                ////which will be returned by this function

                //for (int i = 0; i < jsonResult.Count; i++)
                //{
                //    if (isMeasurepoint((jsonResult[i]) + "}"))
                //    {
                //        Measurepoint measurepoint = Measurepoint.fromJson(jsonResult.ElementAt(i) + "}");
                //        measurepoints.Add(measurepoint);
                //    }
                //}

            }

            return measurepoints;
        }

        private bool isMeasurepoint(string m)
        {            
            return m.StartsWith('{') && m.EndsWith('}');
        }



        //class NetworkCallback : ConnectivityManager.NetworkCallback
        //{
        //    private ConnectivityManager _conn;
        //    public Action<Network> NetworkAvailable { get; set; }
        //    public Action NetworkUnavailable { get; set; }

        //    public NetworkCallback(ConnectivityManager connectivityManager)
        //    {
        //        _conn = connectivityManager;
        //    }

        //    public override void OnAvailable(Network network)
        //    {
        //        base.OnAvailable(network);
        //        // Need this to bind to network otherwise it is connected to wifi 
        //        // but traffic is not routed to the wifi specified
        //        _conn.BindProcessToNetwork(network);
        //        NetworkAvailable?.Invoke(network);
        //    }

        //    public override void OnUnavailable()
        //    {
        //        base.OnUnavailable();

        //        NetworkUnavailable?.Invoke();
        //    }
        //}


        public class WifiEventArgs : EventArgs
        {
            internal bool IsConnected { get; set; }
        }

        /*

        private void connect_Wifi()
        {
            ConnectivityManager wifiManager = Android.App.Application.Context.GetSystemService(Android.App.Activity.ConnectivityService) as ConnectivityManager;
            NetworkCallback networkCallback = new NetworkCallback(wifiManager)
            {
                NetworkAvailable = network =>
                {
                    // we are connected!


                    WifiEventArgs args = new WifiEventArgs { IsConnected = true };
                    OnWifiConnected(args);

                },
                NetworkUnavailable = () =>
                {
                    WifiEventArgs args = new WifiEventArgs { IsConnected = false };
                    OnWifiConnected(args);

                }
            };

            var wifiSpecifier = new WifiNetworkSpecifier.Builder()
               .SetSsid(ssid)
               .SetWpa2Passphrase(preSharedKey)
               .Build();

            var request = new NetworkRequest.Builder()
                .AddTransportType(TransportType.Wifi) // we want WiFi
                .RemoveCapability(NetCapability.Internet) // Internet not required
                .SetNetworkSpecifier(wifiSpecifier) // we want _our_ network
                .Build();

            _wifiManager.RequestNetwork(request, _networkCallback);

        }



        private void connectWifiTraditionally()
        {
            ConnectivityManager connectivityManager = Android.Content.Context.co
        }

        */


    }
}
