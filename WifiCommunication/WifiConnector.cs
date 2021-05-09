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
        private NetworkCallback callback;
        private string ssid = "yournetworkname";
        private string passphrase = "yourcode";
        public ConnectivityManager connectivityManager;

        private static WifiManager wifi;
        private static List<ScanResult> networks;
        private WifiReceiver wifiReceiver;
        
        private Context context = null;
        private bool requested;

        public class WifiReceiver : BroadcastReceiver
        {
            public override void OnReceive(Context context, Intent intent)
            {
                foreach (ScanResult wifinetwork in wifi.ScanResults)
                {
                    networks.Add(wifinetwork);
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
            networks = new List<ScanResult>();

            // Get a handle to the Wifi
            wifi = (WifiManager)context.GetSystemService(Context.WifiService);

            // Start a scan and register the Broadcast receiver to get the list of Wifi Networks
            wifiReceiver = new WifiReceiver();
            context.RegisterReceiver(wifiReceiver, new IntentFilter(WifiManager.ScanResultsAvailableAction));
            wifi.StartScan();
        }

        //public void findWifiNetworks(object sender, DoWorkEventArgs e)
        //{
        //    var worker = (BackgroundWorker)sender;
        //    worker.DoWork -= findWifiNetworks;

        //    if (worker.CancellationPending)
        //    {
        //        e.Cancel = true; //Cancel the BackgroungWirker Properly.                    
        //    }
        //    else
        //    {
        //        wifi = (WifiManager)context.GetSystemService(Context.WifiService);
        //        if (wifi.WifiState == WifiState.Enabled)
        //        {
        //            wifi.StartScan();
        //            Thread.Sleep(5000);
        //            wifi.ScanResults.CopyTo(scanResults, 0);
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

        public void RequestNetwork()
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