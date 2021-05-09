using Android.Views;
using Android.Widget;
using System.Collections.Generic;
using Android.Bluetooth;
using System.Collections.ObjectModel;
using Plugin.BLE.Abstractions.Contracts;

namespace FreediverApp
{
    /**
     *  This class represents the custom adapter that is assigned to the Bluetooth listview to display
     *  bluetooth devices as list entries inside the listview.
     **/
    public class BluetoothListViewAdapter : BaseAdapter<Plugin.BLE.Abstractions.DeviceBase>
    {
        //obsolete code for normal bluetooth
        List<BluetoothDevice> bt_devices;

        ObservableCollection<IDevice> btle_devices;

        //obsolete code for normal bluetooth
        public BluetoothListViewAdapter(List<BluetoothDevice> bt_devices)
        {
            this.bt_devices = bt_devices;
        }

        public BluetoothListViewAdapter(ObservableCollection<IDevice> btle_devices)
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

        /**
         *  This function simply returns the count of the btle device list that is holded by this adapter 
         **/
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

        /**
         *  Returns a populated view element to display it inside the custom listview.
         *  A view element contains the bluetooth icon, the device name, Mac Address and the
         *  current connectionState.
         **/
        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var view = convertView;

            if (view == null)
            {
                view = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.BluetoothListViewCell, parent, false);
                view.Tag = new BluetoothDeviceViewHolder(view);
            }

            var holder = (BluetoothDeviceViewHolder)view.Tag;
            holder.Photo.SetImageResource(Resource.Drawable.icon_info);
            if (btle_devices[position].Name != null && btle_devices[position].Name.Length > 14)
            {
                holder.Name.Text = btle_devices[position].Name.Substring(0, 15) + "...";
            }
            else
            {
                holder.Name.Text = btle_devices[position].Name;
            }
            holder.MacAdress.Text = "MAC: " + btle_devices[position].NativeDevice.ToString();            
            holder.ConState.Text = btle_devices[position].State == Plugin.BLE.Abstractions.DeviceState.Connected ? "Connected" : "Disconnected"; 

            return view;
        }

    }
}
