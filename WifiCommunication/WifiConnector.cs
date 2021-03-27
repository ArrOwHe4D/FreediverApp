using Android.App;
using Android.Content;
using Android.Net;
using Android.Net.Wifi;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Android.Net.ConnectivityManager;

namespace FreediverApp.WifiCommunication
{
    class WifiConnector
    {
        private NetworkCallback callback;
        private string ssid = "yournetworkname";
        private string passphrase = "yourcode";
        public ConnectivityManager connectivityManager;

        public WifiConnector()
        {
            callback = new NetworkCallback
            {
                NetworkAvailable = network =>
                {
                    Console.WriteLine("Request network available");
                },
                NetworkUnavailable = () =>
                {
                    Console.WriteLine("Request network unavailable");
                }
            };
        }


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


        public bool requested;
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