using Android.Views;
using Android.Widget;
using System.Collections.ObjectModel;
using Android.Net.Wifi;
using FreediverApp.UI.Components;
using System.Collections.Generic;

namespace FreediverApp
{
    /**
     *  This class represents the custom adapter that is assigned to the Bluetooth listview to display
     *  bluetooth devices as list entries inside the listview.
     **/
    public class WifiListViewAdapter : BaseAdapter<ScanResult>
    {
        List<ScanResult> wifiDevices;

        public WifiListViewAdapter(List<ScanResult> wifiDevices)
        {
            this.wifiDevices = wifiDevices;
        }

        public override ScanResult this[int position]
        {
            get
            {
                return wifiDevices[position];
            }
        }

        /**
         *  This function simply returns the count of the btle device list that is holded by this adapter 
         **/
        public override int Count
        {
            get
            {
                return wifiDevices.Count;
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
                view = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.WifiListViewCell, parent, false);
                view.Tag = new WifiDeviceViewHolder(view);
            }

            var holder = (WifiDeviceViewHolder)view.Tag;
            holder.Icon.SetImageResource(Resource.Drawable.icon_info);
            if (wifiDevices[position].Ssid != null && wifiDevices[position].Ssid.Length > 14)
            {
                holder.Name.Text = wifiDevices[position].Ssid.Substring(0, 15) + "...";
            }
            else
            {
                holder.Name.Text = wifiDevices[position].Ssid;
            }
            holder.Frequency.Text = "FREQ: " + wifiDevices[position].CenterFreq0.ToString();
            holder.Capabilities.Text = wifiDevices[position].Capabilities;

            return view;
        }

    }
}
