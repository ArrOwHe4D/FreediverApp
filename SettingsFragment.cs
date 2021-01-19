using System;
using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;

namespace FreediverApp
{
    public class SettingsFragment : Fragment
    {
        private Spinner languageSpinner, designSpinner;
        private Button btnSave;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.SettingsPage, container, false);

            languageSpinner = view.FindViewById<Spinner>(Resource.Id.spinnerLanguage);
            designSpinner = view.FindViewById<Spinner>(Resource.Id.spinnerDesign);
            btnSave = view.FindViewById<Button>(Resource.Id.btnSave);

            languageSpinner.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(languageSpinner_ItemSelected);
            
            btnSave.Click += btnSaveOnClick;

            return view;
        }

        private void languageSpinner_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e) 
        {
            Spinner languageSpinner = (Spinner)sender;
        }

        private void designSpinner_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e) 
        {
            Spinner designSpinner = (Spinner)sender;
        }

        public void btnSaveOnClick(object sender, EventArgs e) 
        {
            //on click save button
            
        }
    }
}