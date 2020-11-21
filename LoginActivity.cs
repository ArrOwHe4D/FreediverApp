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
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    public class LoginActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.login);
            var buttonLogin = FindViewById<Button>(Resource.Id.button_login);
            var texteditPassword = FindViewById<EditText>(Resource.Id.textedit_password);
            texteditPassword.InputType = Android.Text.InputTypes.TextVariationPassword | Android.Text.InputTypes.ClassText;
            buttonLogin.Click += login;
        }

        private void login(object sender, EventArgs eventArgs)
        {
            var username = FindViewById<EditText>(Resource.Id.textedit_username);
            var password = FindViewById<EditText>(Resource.Id.textedit_password);

            if (username.Text == "Freediver" && password.Text == "123")
            {
                var mainMenu = new Intent(this, typeof(MainActivity));
                StartActivity(mainMenu);
            }
            else
            {
                Toast.MakeText(this, "You entered a wrong user id or password!", ToastLength.Long).Show();
            }
        }
    }
}