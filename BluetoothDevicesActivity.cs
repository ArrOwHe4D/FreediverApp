using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.Bluetooth;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using FreediverApp.BluetoothCommunication;
using Android.Content.PM;
using Android;
using Android.Support.V4.Content;
using Android.Support.V4.App;

namespace FreediverApp
{
    [Activity(Label = "Bluetooth Devices")]
    public class BluetoothDevicesActivity : Activity
    {
        private List<string> items;
        private List<BluetoothDevice> discoveredDevices;
        private ListView listView;
        private BluetoothDeviceReceiver btReceiver;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            btReceiver = new BluetoothDeviceReceiver();

            SetContentView(Resource.Layout.BluetoothDevicesPage);

            listView = FindViewById<ListView>(Resource.Id.lv_con_devices);
          
            discoveredDevices = getUnknownDevices();
            items = getBondedBluetoothDevices();

            ArrayAdapter<string> adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleListItem1, items);
            listView.Adapter = adapter;
        }

        private List<BluetoothDevice> getUnknownDevices()
        {
            RegisterReceiver(btReceiver, new IntentFilter(BluetoothDevice.ActionFound));
            return btReceiver.foundDevices;
        }

        private List<string> getBondedBluetoothDevices()
        {           
            List<string> foundDevices = new List<string>();         
           
            if (btReceiver.m_adapter.IsEnabled)
            {
                List<BluetoothDevice> devices = btReceiver.m_adapter.BondedDevices.ToList();
                foreach (var device in devices)
                {
                    foundDevices.Add(device.Name);
                }              
            }            
            return foundDevices;
        }
    }
}