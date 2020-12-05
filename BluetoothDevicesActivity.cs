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
        private List<string> mItems;
        private ListView mListView;
        private BluetoothDeviceReceiver bt_receiver;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.BluetoothDevicesPage);

            mListView = FindViewById<ListView>(Resource.Id.lv_con_devices);
            mItems = new List<string>();
            //mItems = getBluetoothDevices();

            ArrayAdapter<string> adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleListItem1, mItems);
            mListView.Adapter = adapter;
        }

        private List<string> getBluetoothDevices()
        {
            bt_receiver = new BluetoothDeviceReceiver();

            RegisterReceiver(bt_receiver, new IntentFilter(BluetoothDevice.ActionFound));
            List<string> foundDevices = new List<string>();
            /*BluetoothAdapter adapter*/ 
            bt_receiver.m_adapter = BluetoothAdapter.DefaultAdapter;
            bt_receiver.m_adapter.StartDiscovery();
            //adapter.StartDiscovery();

            //TESTING AREA------------
            foreach (BluetoothDevice bd in bt_receiver.foundDevices)
            {
                if (!foundDevices.Contains(bd.Name))
                    foundDevices.Add(bd.Name);
            }
            //------------------------

            if (bt_receiver.m_adapter.IsEnabled)
            {
                List<BluetoothDevice> devices = bt_receiver.m_adapter.BondedDevices.ToList();
                foreach (var device in devices)
                {
                    foundDevices.Add(device.Name);
                }              
            }            
            return foundDevices;
        }
    }
}