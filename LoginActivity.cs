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
        private TextView textview_cantlogin;
        private Button button_register, button_login;
        private EditText textedit_username, textedit_password;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.LoginPage);

            //Init UI Components
            button_login = FindViewById<Button>(Resource.Id.button_login);
            button_login.Click += login;

            textedit_username = FindViewById<EditText>(Resource.Id.textedit_username);

            textedit_password = FindViewById<EditText>(Resource.Id.textedit_password);
            textedit_password.InputType = Android.Text.InputTypes.TextVariationPassword | Android.Text.InputTypes.ClassText;

            button_register = FindViewById<Button>(Resource.Id.button_register);
            button_register.Click += redirectToRegisterActivity;

            textview_cantlogin = FindViewById<TextView>(Resource.Id.textview_cantlogin);
            textview_cantlogin.Click += redirectToLoginProblemsActivity;
        }

        private void login(object sender, EventArgs eventArgs)
        {
            //Dummy Login since we have no db connection yet
            if (textedit_username.Text == "Freediver" && textedit_password.Text == "123")
            {
                var mainMenu = new Intent(this, typeof(MainActivity));
                StartActivity(mainMenu);
            }
            else
            {
                Toast.MakeText(this, "You entered a wrong user id or password!", ToastLength.Long).Show();
            }
        }

        private void redirectToLoginProblemsActivity(object sender, EventArgs eventArgs) 
        {
            var loginProblemsActivity = new Intent(this, typeof(LoginProblemsActivity));
            StartActivity(loginProblemsActivity);
        }

        private void redirectToRegisterActivity(object sender, EventArgs eventArgs) 
        {
            var registerActivity = new Intent(this, typeof(RegisterActivity));
            StartActivity(registerActivity);
        }
    }
}