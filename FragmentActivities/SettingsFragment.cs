using System;
using Android.App;
using Android.Bluetooth;
using Android.OS;
using Android.Views;
using Android.Widget;
using FreediverApp.BluetoothCommunication;

namespace FreediverApp
{
    [Obsolete]
    public class SettingsFragment : Fragment
    {
        private Switch switchBluetooth;
        private Button btnSave;
        private BluetoothDeviceReceiver btReceiver;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.SettingsPage, container, false);
            
            btnSave = view.FindViewById<Button>(Resource.Id.button_save);
            btnSave.Click += btnSaveOnClick;

            btReceiver = new BluetoothDeviceReceiver();
            btReceiver.m_adapter = BluetoothAdapter.DefaultAdapter;

            switchBluetooth = view.FindViewById<Switch>(Resource.Id.switch_bluetooth);
            switchBluetooth.Click += switchBluetoothOnClick;
            switchBluetooth.Checked = btReceiver.m_adapter.IsEnabled;
                
            return view;
        }

        public void switchBluetoothOnClick(object sender, EventArgs args) 
        {
            if (btReceiver.m_adapter != null)
            {
                if (!btReceiver.m_adapter.IsEnabled)
                {
                    btReceiver.m_adapter.Enable();
                }
                else 
                {
                    btReceiver.m_adapter.Disable();
                }
            }
        }

        public void btnSaveOnClick(object sender, EventArgs args) 
        {
            
        }
    }
}