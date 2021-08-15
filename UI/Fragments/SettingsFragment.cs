using System;
using Android.App;
using Android.Bluetooth;
using Android.Content;
using Android.Content.Res;
using Android.OS;
using Android.Views;
using Android.Widget;
using FreediverApp.BluetoothCommunication;
using Java.Util;

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
        private Spinner spinnerLanguage;
        private ArrayAdapter spinnerAdapter;
        private bool initiateCall = true;

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

            spinnerLanguage = view.FindViewById<Spinner>(Resource.Id.spinner_language);
            spinnerAdapter = ArrayAdapter.CreateFromResource(Context, Resource.Array.languages_array, Android.Resource.Layout.SimpleSpinnerItem);
            spinnerAdapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            spinnerLanguage.Adapter = spinnerAdapter;
            spinnerLanguage.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(languageSpinner_ItemSelected);

            return view;
        }

        private void languageSpinner_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e) 
        {
            if (initiateCall) 
            {
                initiateCall = false;
                return;
            }

            Spinner spinner = (Spinner)sender;
            var selectedItem = spinner.SelectedItem;
            
            string languageChangedMessage = string.Format("Current Language is set to: {0}", spinner.GetItemAtPosition(e.Position));
            Toast.MakeText(Context, languageChangedMessage, ToastLength.Long).Show();

            string selectedLanguage = (string) spinner.GetItemAtPosition(e.Position);

            switch (selectedLanguage) 
            {
                case "Deutsch": 
                {
                    Locale locale = new Locale("de");
                    Locale.SetDefault(Locale.Category.Display, locale);
                    Configuration config = new Configuration();
                    config.Locale = locale;
                    Context.Resources.UpdateConfiguration(config, Context.Resources.DisplayMetrics);
                    break;
                }
                case "English": 
                {
                    Locale locale = new Locale("en");
                    Locale.SetDefault(Locale.Category.Display, locale);
                    Configuration config = new Configuration();
                    config.Locale = locale;
                    Context.Resources.UpdateConfiguration(config, Context.Resources.DisplayMetrics);
                    break;
                }
            }
            restartMainActivity();
            refreshFragment();
        }

        private void refreshFragment() 
        {
            FragmentTransaction menuTransaction = FragmentManager.BeginTransaction();
            SettingsFragment settingsFragment = new SettingsFragment();
            menuTransaction.Replace(Resource.Id.framelayout, settingsFragment).AddToBackStack(null).Commit();
        }

        private void restartMainActivity()
        {
            var mainActivity = new Intent(Context, typeof(MainActivity));
            StartActivity(mainActivity);
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
            Toast.MakeText(Context, Resource.String.saving_successful, ToastLength.Long).Show();
        }
    }
}