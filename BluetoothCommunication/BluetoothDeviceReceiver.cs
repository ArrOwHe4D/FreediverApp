using System;
using Android.Bluetooth;
using Android.Content;

namespace FreediverApp.BluetoothCommunication
{
    public class BluetoothDeviceReceiver : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {
            var action = intent.Action;

            if (action != BluetoothDevice.ActionFound)
                return;

            BluetoothDevice device = (BluetoothDevice)intent.GetParcelableExtra(BluetoothDevice.ExtraDevice);

            if (device.BondState != Bond.Bonded)
                Console.WriteLine($"Found device with id: {device.Name} and MAC-Address: {device.Address}"); 
        }
    }
}