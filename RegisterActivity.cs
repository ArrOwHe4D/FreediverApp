using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Support.V7;
using Java.Util;
using SupportV7 = Android.Support.V7.App;
using Firebase.Database;
using DBConnector = FreediverApp.DatabaseConnector.DatabaseConnector;

namespace FreediverApp
{
    [Activity(Label = "RegisterActivity")]
    public class RegisterActivity : Activity
    {
        private Button button_register;
        private EditText texteditEmail, 
            texteditUsername, 
            texteditPassword, 
            texteditFirstname, 
            texteditLastname, 
            texteditDateOfBirth, 
            texteditWeight, 
            texteditHeight;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.RegisterPage);

            texteditEmail = FindViewById<EditText>(Resource.Id.textedit_email);
            texteditUsername = FindViewById<EditText>(Resource.Id.textedit_username);
            texteditPassword = FindViewById<EditText>(Resource.Id.textedit_password);
            texteditFirstname = FindViewById<EditText>(Resource.Id.textedit_firstname);
            texteditLastname = FindViewById<EditText>(Resource.Id.textedit_lastname);
            texteditDateOfBirth = FindViewById<EditText>(Resource.Id.textedit_date_of_birth);
            texteditWeight = FindViewById<EditText>(Resource.Id.textedit_weight);
            texteditHeight = FindViewById<EditText>(Resource.Id.textedit_height);

            button_register = FindViewById<Button>(Resource.Id.button_register);
            button_register.Click += createAccount;
        }

        private void createAccount(object sender, EventArgs eventArgs) 
        {
            string email = texteditEmail.Text;
            string username = texteditUsername.Text;
            string password = texteditPassword.Text;
            string firstname = texteditFirstname.Text;
            string lastname = texteditLastname.Text;
            string dateofbirth = texteditDateOfBirth.Text;
            string weight = texteditWeight.Text;
            string height = texteditHeight.Text;

            HashMap userData = new HashMap();
            userData.Put("email", email);
            userData.Put("username", username);
            userData.Put("password", password);
            userData.Put("firstname", firstname);
            userData.Put("lastname", lastname);
            userData.Put("birthday", dateofbirth);
            userData.Put("weight", weight);
            userData.Put("height", height);

            SupportV7.AlertDialog.Builder saveDataDialog = new SupportV7.AlertDialog.Builder(this);
            saveDataDialog.SetTitle("Save User Information");
            saveDataDialog.SetMessage("Are you sure?");
            saveDataDialog.SetPositiveButton("Accept", (senderAlert, args) =>
            {
                DatabaseReference newUserRef = DBConnector.GetDatabase().GetReference("users").Push();
                newUserRef.SetValue(userData);
            });
            saveDataDialog.SetNegativeButton("Cancel", (senderAlert, args) =>
            {
                saveDataDialog.Dispose();
            });

            saveDataDialog.Show();
        }
    }
}