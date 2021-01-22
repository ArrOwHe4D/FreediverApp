using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.Bluetooth;

namespace FreediverApp.BluetoothCommunication
{
    public class BluetoothConnection
    {
        public void getDevice() { this.Device = (from bd in this.Adapter.BondedDevices where bd.Name == "HC-05" select bd).FirstOrDefault(); }
        public BluetoothAdapter Adapter { get; set; }
        public BluetoothDevice Device { get; set; }
        public BluetoothSocket Socket { get; set; }
    }
}