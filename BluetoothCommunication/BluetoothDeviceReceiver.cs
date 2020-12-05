using System;
using Android.Bluetooth;
using Android.Content;
using System.Collections.Generic;

namespace FreediverApp.BluetoothCommunication
{
    public class BluetoothDeviceReceiver : BroadcastReceiver
    {
        public List<BluetoothDevice> foundDevices;
        public override void OnReceive(Context context, Intent intent)
        {
            foundDevices = new List<BluetoothDevice>();
            var action = intent.Action;

            if (action != BluetoothDevice.ActionFound)
                return;

            BluetoothDevice device = (BluetoothDevice)intent.GetParcelableExtra(BluetoothDevice.ExtraDevice);

            if (device.BondState != Bond.Bonded)
                foundDevices.Add(device);
        }
    }
}