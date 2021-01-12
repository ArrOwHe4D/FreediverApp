using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using Firebase.Database;
using FreediverApp.DatabaseConnector;
using DBConnector = FreediverApp.DatabaseConnector.DatabaseConnector;

namespace FreediverApp
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    public class LoginActivity : Activity
    {
        private TextView textviewCantLogin;
        private Button buttonRegister, buttonLogin;
        private EditText texteditUsername, texteditPassword;
        private LoginInfoListener loginInfoListener;
        private User loginUser;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.LoginPage);

            //Init UI Components
            buttonLogin = FindViewById<Button>(Resource.Id.button_login);
            buttonLogin.Click += login;

            texteditUsername = FindViewById<EditText>(Resource.Id.textedit_username);

            texteditPassword = FindViewById<EditText>(Resource.Id.textedit_password);
            texteditPassword.InputType = Android.Text.InputTypes.TextVariationPassword | Android.Text.InputTypes.ClassText;

            buttonRegister = FindViewById<Button>(Resource.Id.button_register);
            buttonRegister.Click += redirectToRegisterActivity;

            textviewCantLogin = FindViewById<TextView>(Resource.Id.textview_cantlogin);
            textviewCantLogin.Click += redirectToLoginProblemsActivity;
        }

        private void login(object sender, EventArgs eventArgs)
        {
            //Dummy Login since we have no db connection yet
            //var loginInfoListener = new LoginInfoListener((sender, e) =>
            //{
            //    bool found = false;//(e as LoginInfoListener.LoginInfoEventArgs).found;
            //    string username = (e as LoginInfoListener.LoginInfoEventArgs).userdata.username;
            //    string password = (e as LoginInfoListener.LoginInfoEventArgs).userdata.password;

            //    if (found)
            //    {
            //        var mainMenu = new Intent(this, typeof(MainActivity));
            //        StartActivity(mainMenu);
            //    }
            //    else 
            //    {
            //        Toast.MakeText(this, "Your username or password was wrong, please try again!", ToastLength.Long);
            //    }

            //}, texteditUsername.Text, texteditPassword.Text);

            var dbRef = DBConnector.GetDatabase().GetReference("users");
            Query query = dbRef.OrderByChild("username").EqualTo(texteditUsername.Text);

            if (texteditUsername.Text == "Freediver" && texteditPassword.Text == "123")
            {
                var mainMenu = new Intent(this, typeof(MainActivity));
                StartActivity(mainMenu);
            }
            else
            {
                Toast.MakeText(this, "you entered a wrong user id or password!", ToastLength.Long).Show();
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

        private void redirectToDiveSessionDetailViewActivity(object sender, EventArgs eventArgs)
        {
            var diveSessionDetailActivity = new Intent(this, typeof(DiveSessionDetailViewActivity));
            StartActivity(diveSessionDetailActivity);
        }
    }
}