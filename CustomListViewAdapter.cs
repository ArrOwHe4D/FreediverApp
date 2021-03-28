using Android.Views;
using Android.Widget;
using System.Collections.Generic;
using Android.Bluetooth;
using Java.Util;
using System.Collections.ObjectModel;
using Plugin.BLE.Abstractions.Contracts;

namespace FreediverApp
{
    public class CustomListViewAdapter : BaseAdapter<Plugin.BLE.Abstractions.DeviceBase>
    {
        List<BluetoothDevice> bt_devices;
        ObservableCollection<IDevice> btle_devices;

        public CustomListViewAdapter(List<BluetoothDevice> bt_devices)
        {
            this.bt_devices = bt_devices;
        }

        public CustomListViewAdapter(ObservableCollection<IDevice> btle_devices)
        {
            this.btle_devices = btle_devices;
        }

        public override Plugin.BLE.Abstractions.DeviceBase this[int position]
        {
            get
            {
                return (Plugin.BLE.Abstractions.DeviceBase)btle_devices[position];
            }
        }

        public override int Count
        {
            get
            {
                return btle_devices.Count;
            }
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var view = convertView;

            if(view == null)
            {
                view = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.ListViewCell, parent, false);
                view.Tag = new BluetoothDeviceViewHolder(view);
            }

            var holder = (BluetoothDeviceViewHolder)view.Tag;
            holder.Photo.SetImageResource(Resource.Drawable.icon_info);
            holder.Name.Text = btle_devices[position].Name;
            holder.MacAdress.Text = btle_devices[position].Id.ToString();
            holder.ConState.Text = btle_devices[position].State == Plugin.BLE.Abstractions.DeviceState.Connected ? "Connected" : "Disconnected"; 
            //BluetoothSocket socket = bt_devices[position].CreateInsecureRfcommSocketToServiceRecord(uuid);
            //if (socket.IsConnected)
            //    holder.ConState.Text = "Connected";
            //else
            //    holder.ConState.Text = "Not Conncected";
            return view;
        }

    }
}
