using Android.Content;
using Android.Net.Wifi;
using System;
using System.Collections.Generic;

namespace FreediverApp.WifiCommunication
{
    class WifiConnector
    {
        private Context context = null;
        private static WifiManager wifiManager;
        private WifiReceiver wifiReceiver;
        public static IList<ScanResult> wifiNetworks;

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
            Initialize();
        }

        public void scan()
        {
            Initialize();
            wifiNetworks = new List<ScanResult>();

            // Start a scan and register the Broadcast receiver to get the list of Wifi Networks
            wifiReceiver = new WifiReceiver();
            context.RegisterReceiver(wifiReceiver, new IntentFilter(WifiManager.ScanResultsAvailableAction));
            bool success = wifiManager.StartScan();

            if (!success) 
            { 
                Console.WriteLine("WIFI SCAN FAILED!");
                wifiNetworks = wifiManager.ScanResults;
            }
            else 
            {
                wifiNetworks = wifiManager.ScanResults;
            }
        }

        public void Initialize() 
        {
            wifiManager = (WifiManager)context.GetSystemService(Context.WifiService);
        }

        public void SetWifiEnabled(bool enabled) 
        {
            Initialize();
            wifiManager.SetWifiEnabled(enabled);
        }

        public bool IsWifiEnabled() 
        {
            return wifiManager.IsWifiEnabled;
        }
    }
}