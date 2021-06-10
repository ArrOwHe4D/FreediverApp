using Android.Bluetooth;
using Android.Content;
using System.Collections.Generic;

namespace FreediverApp.BluetoothCommunication
{

    /**
     * This class handles the receiving of Bluetooth device information while scanning for new devices.
     **/
    public class BluetoothDeviceReceiver : BroadcastReceiver
    {
        public List<BluetoothDevice> foundDevices;
        public BluetoothAdapter m_adapter;

        /**
         *  This function represents a eventhandler that is called when a new bluetooth device was found while scanning.
         *  When a device isn´t already inside the list of known devices, it is being added to the list of found devices.
         **/
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