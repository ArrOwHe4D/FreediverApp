using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace FreediverApp
{
    [Activity(Label = "Bluetooth Devices")]
    public class BluetoothDevicesActivity : Activity
    {
        private List<string> mItems;
        private ListView mListView;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.BluetoothDevicesPage);

            mListView = FindViewById<ListView>(Resource.Id.lv_con_devices);

            mItems = new List<string>();
            mItems.Add("Device1");
            mItems.Add("Device2");
        }
    }
}