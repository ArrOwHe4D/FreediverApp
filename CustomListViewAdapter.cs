using Android.Views;
using Android.Widget;
using System.Collections.Generic;
using Android.Bluetooth;
using Android.Graphics.Drawables;
using static Android.Support.V7.Widget.RecyclerView;
using Java.Util;
using Android.Net;

namespace FreediverApp
{
    public class CustomListViewAdapter : BaseAdapter<BluetoothDevice>
    {
        List<BluetoothDevice> bt_devices;
        private UUID uuid;

        public CustomListViewAdapter(List<BluetoothDevice> bt_devices)
        {
            this.bt_devices = bt_devices;
        }

        public override BluetoothDevice this[int position]
        {
            get
            {
                return bt_devices[position];
            }
        }

        public override int Count
        {
            get
            {
                return bt_devices.Count;
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
            holder.Name.Text = bt_devices[position].Name;
            holder.MacAdress.Text = bt_devices[position].Address;
            holder.ConState.Text = bt_devices[position].BondState == Bond.Bonded ? "Paired" : "Not Connected"; 
            //BluetoothSocket socket = bt_devices[position].CreateInsecureRfcommSocketToServiceRecord(uuid);
            //if (socket.IsConnected)
            //    holder.ConState.Text = "Connected";
            //else
            //    holder.ConState.Text = "Not Conncected";
            return view;
        }

    }
}
