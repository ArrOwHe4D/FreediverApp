using Android.App;
using Android.Content;
using Android.Net;
using Android.Net.Wifi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;

namespace FreediverApp.WifiCommunication
{
    class WifiConnector
    {
        private Context context = null;
        private static WifiManager wifiManager;
        private WifiReceiver wifiReceiver;
        public static List<ScanResult> wifiNetworks;

        // OLD STUFF
        private NetworkCallback callback;
        private string ssid = "23PSE";
        private string passphrase = "ZZZDiveZZZ";
        public ConnectivityManager connectivityManager;
        private bool requested;

        public class WifiReceiver : BroadcastReceiver
        {
            public override void OnReceive(Context context, Intent intent)
            {
                IList<ScanResult> foundNetworks = wifiManager.ScanResults;
                foreach (ScanResult wifinetwork in foundNetworks) 
                {
                    wifiNetworks.Add(wifinetwork);
                }
            }
        }

        public WifiConnector(Context context)
        {
            this.context = context;

            //callback = new NetworkCallback
            //{
            //    NetworkAvailable = network =>
            //    {
            //        Console.WriteLine("Request network available");
            //    },
            //    NetworkUnavailable = () =>
            //    {
            //        Console.WriteLine("Request network unavailable");
            //    }
            //};
        }

        public void scan()
        {
            wifiNetworks = new List<ScanResult>();

            // Get a handle to the Wifi
            wifiManager = (WifiManager)context.GetSystemService(Context.WifiService);

            // Start a scan and register the Broadcast receiver to get the list of Wifi Networks
            wifiReceiver = new WifiReceiver();
            context.RegisterReceiver(wifiReceiver, new IntentFilter(WifiManager.ScanResultsAvailableAction));
            wifiManager.StartScan();
        }

        //public void findWifiNetworks(object sender, DoWorkEventArgs e)
        //{
        //    var worker = (BackgroundWorker)sender;
        //    worker.DoWork -= findWifiNetworks;

        //    if (worker.CancellationPending)
        //    {
        //        e.Cancel = true; //Cancel the BackgroungWorker Properly.                    
        //    }
        //    else
        //    {
        //        wifi = (WifiManager)context.GetSystemService(Context.WifiService);
        //        if (wifi.WifiState == WifiState.Enabled)
        //        {
        //            wifi.StartScan();
        //            Thread.Sleep(5000);
        //            foreach (ScanResult scanResult in wifi.ScanResults) 
        //            {
        //                networks.Add(scanResult);
        //            }
        //        }
        //    }
        //}

        public void SuggestNetwork()
        {
            var suggestion = new WifiNetworkSuggestion.Builder()
                .SetSsid(ssid)
                .SetWpa2Passphrase(passphrase)
                .Build();

            var suggestions = new[] { suggestion };

            var wifiManager = Application.Context.GetSystemService(Android.Content.Context.WifiService) as WifiManager;
            var status = wifiManager.AddNetworkSuggestions(suggestions);

            var statusText = status switch
            {
                NetworkStatus.SuggestionsSuccess => "Suggestion Success",
                NetworkStatus.SuggestionsErrorAddDuplicate => "Suggestion Duplicate Added",
                NetworkStatus.SuggestionsErrorAddExceedsMaxPerApp => "Suggestion Exceeds Max Per App"
            };

            Console.WriteLine(statusText);
        }

        public void requestNetwork()
        {
            var specifier = new WifiNetworkSpecifier.Builder()
                .SetSsid(ssid)
                .SetWpa2Passphrase(passphrase)
                .Build();

            var request = new NetworkRequest.Builder()
                .AddTransportType(TransportType.Wifi)
                .SetNetworkSpecifier(specifier)
                .Build();

            connectivityManager = Application.Context.GetSystemService(Android.Content.Context.ConnectivityService) as ConnectivityManager;

            if (requested)
            {
                connectivityManager.UnregisterNetworkCallback(callback);
            }

            connectivityManager.RequestNetwork(request, callback);
            requested = true;
        }

        private class NetworkCallback : ConnectivityManager.NetworkCallback
        {
            public Action<Network> NetworkAvailable { get; set; }
            public Action NetworkUnavailable { get; set; }

            public override void OnAvailable(Network network)
            {
                base.OnAvailable(network);
                NetworkAvailable?.Invoke(network);
            }

            public override void OnUnavailable()
            {
                base.OnUnavailable();
                NetworkUnavailable?.Invoke();
            }
        }
    }
}