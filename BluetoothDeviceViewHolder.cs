using Android.Views;
using Android.Widget;
using Android.Support.V7.Widget;

namespace FreediverApp
{
    public class BluetoothDeviceViewHolder : RecyclerView.ViewHolder
    {
        public ImageView Photo { get; private set; }
        public TextView Name { get; private set; }
        public TextView ConState { get; private set; }
        public TextView MacAdress { get; private set; }

        public BluetoothDeviceViewHolder(View itemView) : base(itemView)
        {
            Photo = itemView.FindViewById<ImageView>(Resource.Drawable.icon_info);
            Name = itemView.FindViewById<TextView>(Resource.Id.txtview_btdv_name);
            MacAdress = itemView.FindViewById<TextView>(Resource.Id.txtview_btdv_mac_adress);
            MacAdress = itemView.FindViewById<TextView>(Resource.Id.txtview_btdv_con_state);
        }
    }
}