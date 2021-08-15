using Android.Views;
using Android.Widget;
using Android.Support.V7.Widget;

namespace FreediverApp.UI.Components
{
    /**
     *  This class holds the data for a item inside the bluetoothlistview that is created inside 
     *  the BluetoothDeviceFragment.cs. We display a bluetooth logo for every entry such as the Name,
     *  ConnectionState and the MacAddress of the device inside the listview.
     **/
    public class WifiDeviceViewHolder : RecyclerView.ViewHolder
    {
        public ImageView Icon { get; private set; }
        public TextView Name { get; private set; }
        public TextView Frequency { get; private set; }
        public TextView Capabilities { get; private set; }

        public WifiDeviceViewHolder(View itemView) : base(itemView)
        {
            Icon = itemView.FindViewById<ImageView>(Resource.Id.imgview_wifi_info_icon);
            Name = itemView.FindViewById<TextView>(Resource.Id.textview_wifi_name);
            Frequency = itemView.FindViewById<TextView>(Resource.Id.textview_wifi_capabilities);
            Capabilities = itemView.FindViewById<TextView>(Resource.Id.textview_wifi_status);
        }
    }
}