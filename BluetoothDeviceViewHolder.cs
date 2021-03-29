using Android.Views;
using Android.Widget;
using Android.Support.V7.Widget;

namespace FreediverApp
{
    /**
     *  This class holds the data for a item inside the bluetoothlistview that is created inside 
     *  the BluetoothDeviceFragment.cs. We display a bluetooth logo for every entry such as the Name,
     *  ConnectionState and the MacAddress of the device inside the listview.
     **/
    public class BluetoothDeviceViewHolder : RecyclerView.ViewHolder
    {
        public ImageView Photo { get; private set; }
        public TextView Name { get; private set; }
        public TextView ConState { get; private set; }
        public TextView MacAdress { get; private set; }

        public BluetoothDeviceViewHolder(View itemView) : base(itemView)
        {
            Photo = itemView.FindViewById<ImageView>(Resource.Id.imgview_btdv_info);
            Name = itemView.FindViewById<TextView>(Resource.Id.txtview_btdv_name);
            MacAdress = itemView.FindViewById<TextView>(Resource.Id.txtview_btdv_mac_adress);
            ConState = itemView.FindViewById<TextView>(Resource.Id.txtview_btdv_con_state);
        }
    }
}