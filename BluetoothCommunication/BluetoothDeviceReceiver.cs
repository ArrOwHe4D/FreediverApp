using Android.Bluetooth;
using Android.Content;
using System.Collections.Generic;

namespace FreediverApp.BluetoothCommunication
{

    /*This class handles the receiving of Bluetooth device data information
     * 
     */
    public class BluetoothDeviceReceiver : BroadcastReceiver
    {
        public List<BluetoothDevice> foundDevices;

        public BluetoothAdapter m_adapter;

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