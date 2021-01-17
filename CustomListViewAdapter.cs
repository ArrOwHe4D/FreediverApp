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
using FreediverApp.BluetoothCommunication;
using Android.Bluetooth;
using Android.Support.V7.RecyclerView;
using Android.Graphics.Drawables;
using static Android.Support.V7.Widget.RecyclerView;

namespace FreediverApp
{
    public class CustomListViewAdapter : BaseAdapter<BluetoothDevice>
    {
        List<BluetoothDevice> bt_devices;

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
                var info_icon = view.FindViewById<ImageView>(Resource.Drawable.icon_info);
                var btdv_name = view.FindViewById<TextView>(Resource.Id.txtview_btdv_name);
                var btdv_mac_adress = view.FindViewById<TextView>(Resource.Id.txtview_btdv_mac_adress);
                var btdv_con_state = view.FindViewById<TextView>(Resource.Id.txtview_btdv_con_state);

                view.Tag = new BluetoothDeviceViewHolder(view);
            }

            var holder = (BluetoothDeviceViewHolder)view.Tag;
            //holder.Photo.SetImageDrawable(ImageManager.Get(parent.Context));
            holder.Name.Text = bt_devices[position].Name;
            holder.MacAdress.Text = bt_devices[position].Address;
            //holder.ConState.Text = bt_devices[position].

            return view;
        }

    }
}