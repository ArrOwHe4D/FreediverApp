﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V4.View;
using Android.Support.V4.Widget;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;

namespace FreediverApp
{
    [Activity(Label = "Freediver App - Profile", Theme = "@style/AppTheme.NoActionBar")]
    public class AccountPanelActivity : AppCompatActivity, NavigationView.IOnNavigationItemSelectedListener
    {
        // use edit buttons as Imageviews as it is easier and costs less resources
        ImageView btnPencilEmail, btnPencilPassword, btnPencilFirstname, btnPencilLastname, btnPencilDateOfBirth, btnPencilHeight, btnPencilWeight;

        TextView txtViewEmail, txtViewPassword, txtViewFirstname, txtViewLastname, txtViewDateOfBirth, txtViewHeight, txtViewWeight;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.AccountPanelPage);

            //Instantiate ImageView pencil Buttons for editing data
            btnPencilEmail = FindViewById<ImageView>(Resource.Id.btn_pencil_email);
            btnPencilPassword = FindViewById<ImageView>(Resource.Id.btn_pencil_password);
            btnPencilFirstname = FindViewById<ImageView>(Resource.Id.btn_pencil_firstname);
            btnPencilLastname = FindViewById<ImageView>(Resource.Id.btn_pencil_lastname);
            btnPencilDateOfBirth = FindViewById<ImageView>(Resource.Id.btn_pencil_date_of_birth);
            btnPencilHeight = FindViewById<ImageView>(Resource.Id.btn_pencil_height);
            btnPencilWeight = FindViewById<ImageView>(Resource.Id.btn_pencil_weight);

            btnPencilEmail.Click += editEmail;

            //Instantiate TextView fields that hold the account data values that are being edited by modal dialogs
            txtViewEmail = FindViewById<TextView>(Resource.Id.txtview_email);
            txtViewPassword = FindViewById<TextView>(Resource.Id.txtview_password);
            txtViewFirstname = FindViewById<TextView>(Resource.Id.txtview_firstname);
            txtViewLastname = FindViewById<TextView>(Resource.Id.txtview_lastname);
            txtViewDateOfBirth = FindViewById<TextView>(Resource.Id.txtview_date_of_birth);
            txtViewHeight = FindViewById<TextView>(Resource.Id.txtview_height);
            txtViewWeight = FindViewById<TextView>(Resource.Id.txtview_weight);

        }

        public void editEmail(object sender, EventArgs eventArgs) 
        {
            LayoutInflater layoutInflater = LayoutInflater.From(this);
            View dialogView = layoutInflater.Inflate(Resource.Layout.UserInputDialog, null);
            Android.Support.V7.App.AlertDialog.Builder dialogBuilder = new Android.Support.V7.App.AlertDialog.Builder(this);
            dialogBuilder.SetView(dialogView);

            var editValueField = dialogView.FindViewById<EditText>(Resource.Id.userInput);
            dialogBuilder.SetCancelable(false)
                .SetPositiveButton("Speichern", delegate
                {
                    txtViewEmail.Text = editValueField.Text;
                    Toast.MakeText(this, "Wert wurde erfolgreich geändert!", ToastLength.Long).Show();
                    dialogBuilder.Dispose();
                })
                .SetNegativeButton("Abbrechen", delegate
                {
                    dialogBuilder.Dispose();
                });

            Android.Support.V7.App.AlertDialog dialog = dialogBuilder.Create();
            dialog.Show();
        }

        public bool OnNavigationItemSelected(IMenuItem item)
        {
            int id = item.ItemId;

            if (id == Resource.Id.nav_dive_sessions)
            {
                // Handle the camera action
            }
            else if (id == Resource.Id.nav_connected_devices)
            {

            }
            else if (id == Resource.Id.nav_profile)
            {
                var accountPanelActivity = new Intent(this, typeof(AccountPanelActivity));
                StartActivity(accountPanelActivity);
            }
            else if (id == Resource.Id.nav_settings)
            {

            }
            else if (id == Resource.Id.nav_share)
            {

            }
            else if (id == Resource.Id.nav_send)
            {

            }

            DrawerLayout drawer = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            drawer.CloseDrawer(GravityCompat.Start);
            return true;
        }
    }
}