using System;
using Android.App;
using Android.Bluetooth;
using Android.OS;
using Android.Views;
using Android.Widget;
using FreediverApp.BluetoothCommunication;

namespace FreediverApp
{
    /**
     *  This Fragment represents the options menu. Currently there is only one slider that can be used to switch bluetooth
     *  on or off. If there are any other options and settings that need to be handled they can easily be added to this 
     *  fragment.
     **/
    [Obsolete]
    public class SettingsFragment : Fragment
    {
        /*Member variables (UI components from XML)*/
        private Switch switchBluetooth;
        private Button btnSave;
        private BluetoothDeviceReceiver btReceiver;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        /**
         *  This function initializes all UI components of this view and adds the action handlers to them. 
         **/
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

        /**
         *  This function represents the onclick handler of the bluetooth switch.
         *  According to the current state of the bluetooth adapter, bluetooth is 
         *  turned on or off.
         **/
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

        /**
         *  Onclick handler for the save button. Is currently not used but can be useful when 
         *  additional settings are added to this fragment.
         **/
        public void btnSaveOnClick(object sender, EventArgs args) 
        {
            
        }
    }
}