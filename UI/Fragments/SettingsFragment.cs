using System;
using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.OS;
using Android.Views;
using Android.Widget;
using Java.Util;
using SupportV7 = Android.Support.V7.App;
using System.Collections.Generic;
using System.Threading.Tasks;

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
        private Spinner spinnerLanguage;
        private ArrayAdapter spinnerAdapter;
        private bool initiateCall = true;
        private TextView textViewSSID;
        private TextView textViewPassword;
        private ImageView buttonEditSSID;
        private ImageView buttonEditPassword;

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

            spinnerLanguage = view.FindViewById<Spinner>(Resource.Id.spinner_language);
            spinnerAdapter = ArrayAdapter.CreateFromResource(Context, Resource.Array.languages_array, Android.Resource.Layout.SimpleSpinnerItem);
            spinnerAdapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            spinnerLanguage.Adapter = spinnerAdapter;
            spinnerLanguage.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(languageSpinner_ItemSelected);

            textViewSSID = view.FindViewById<TextView>(Resource.Id.textview_ssid);

            textViewPassword = view.FindViewById<TextView>(Resource.Id.textview_password);

            buttonEditSSID = view.FindViewById<ImageView>(Resource.Id.button_edit_ssid);
            buttonEditSSID.Click += editSSID;

            buttonEditPassword = view.FindViewById<ImageView>(Resource.Id.button_edit_password);
            buttonEditPassword.Click += editPassword;

            return view;
        }

        public override async void OnActivityCreated(Bundle savedInstanceState)
        {
            base.OnActivityCreated(savedInstanceState);

            Dictionary<string, string> wifiCredentials = await readWifiCredentials();

            textViewSSID.Text = wifiCredentials["ota_ssid"];
            textViewPassword.Text = wifiCredentials["ota_password"];
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

        public void editSSID(object sender, EventArgs eventArgs)
        {
            LayoutInflater layoutInflater = LayoutInflater.From(Context);
            View dialogView = layoutInflater.Inflate(Resource.Layout.UserInputDialog, null);

            Android.Content.Res.Resources res = this.Resources;
            SupportV7.AlertDialog.Builder dialogBuilder = createEditDialog(res.GetString(Resource.String.dialog_change_ssid), res.GetString(Resource.String.dialog_new_ssid), Resource.Drawable.icon_pencil, dialogView);

            var editValueField = dialogView.FindViewById<EditText>(Resource.Id.textfield_input);
            editValueField.InputType = Android.Text.InputTypes.TextVariationEmailAddress;

            dialogBuilder.SetCancelable(false)
                .SetPositiveButton(Resource.String.dialog_save, delegate
                {
                    textViewSSID.Text = editValueField.Text.Trim();
                    saveWifiCredentials();
                    Toast.MakeText(Context, Resource.String.saving_successful, ToastLength.Long).Show();
                })
                .SetNegativeButton(Resource.String.dialog_cancel, delegate
                {
                    dialogBuilder.Dispose();
                });

            SupportV7.AlertDialog dialog = dialogBuilder.Create();
            dialog.Show();
        }

        public void editPassword(object sender, EventArgs eventArgs)
        {
            LayoutInflater layoutInflater = LayoutInflater.From(Context);
            View dialogView = layoutInflater.Inflate(Resource.Layout.UserInputDialog, null);

            Android.Content.Res.Resources res = this.Resources;
            SupportV7.AlertDialog.Builder dialogBuilder = createEditDialog(res.GetString(Resource.String.dialog_change_password), res.GetString(Resource.String.dialog_new_password), Resource.Drawable.icon_pencil, dialogView);

            var editValueField = dialogView.FindViewById<EditText>(Resource.Id.textfield_input);
            editValueField.InputType = Android.Text.InputTypes.TextVariationVisiblePassword;

            dialogBuilder.SetCancelable(false)
                .SetPositiveButton(Resource.String.dialog_save, delegate
                {
                    textViewPassword.Text = editValueField.Text.Trim();
                    saveWifiCredentials();
                    Toast.MakeText(Context, Resource.String.saving_successful, ToastLength.Long).Show();
                })
                .SetNegativeButton(Resource.String.dialog_cancel, delegate
                {
                    dialogBuilder.Dispose();
                });

            SupportV7.AlertDialog dialog = dialogBuilder.Create();
            dialog.Show();
        }

        public SupportV7.AlertDialog.Builder createEditDialog(string title, string placeholder, int iconId, View parentView)
        {
            SupportV7.AlertDialog.Builder dialogBuilder = new SupportV7.AlertDialog.Builder(Context);
            dialogBuilder.SetView(parentView);

            dialogBuilder.SetTitle(title);
            dialogBuilder.SetIcon(iconId);

            var editValueField = parentView.FindViewById<EditText>(Resource.Id.textfield_input);
            editValueField.Hint = placeholder;

            return dialogBuilder;
        }

        private async void saveWifiCredentials()
        {
            try
            {
                await Xamarin.Essentials.SecureStorage.SetAsync("ota_ssid", textViewSSID.Text);
                await Xamarin.Essentials.SecureStorage.SetAsync("ota_password", textViewPassword.Text);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private async Task<Dictionary<string, string>> readWifiCredentials()
        {
            Dictionary<string, string> wifiCredentials = new Dictionary<string, string>();
            wifiCredentials["ota_ssid"] = "n/a";
            wifiCredentials["ota_password"] = "n/a";

            try
            {
                wifiCredentials["ota_ssid"] = await Xamarin.Essentials.SecureStorage.GetAsync("ota_ssid");
                wifiCredentials["ota_password"] = await Xamarin.Essentials.SecureStorage.GetAsync("ota_password");
            }
            catch (Exception ex) 
            {
                Console.WriteLine(ex);
            }

            return wifiCredentials;
        }
    }
}