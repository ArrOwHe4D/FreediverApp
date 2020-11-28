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

namespace FreediverApp
{
    [Activity(Label = "RegisterActivity")]
    public class RegisterActivity : Activity
    {
        private Button button_register;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.RegisterPage);

            button_register = FindViewById<Button>(Resource.Id.button_register);
            button_register.Click += createAccount;
        }

        private void createAccount(object sender, EventArgs eventArgs) 
        {
            //TODO: Pickup all the form data and send it to the firebase db to create a new account entry 
        }
    }
}