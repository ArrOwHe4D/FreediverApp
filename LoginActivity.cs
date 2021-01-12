﻿using System;
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

        private UserDataListener userDataListener;
        private List<User> userList;
        private bool dataretrieved = false;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.LoginPage);

            buttonLogin = FindViewById<Button>(Resource.Id.button_login);
            buttonLogin.Click += login;

            texteditUsername = FindViewById<EditText>(Resource.Id.textedit_username);

            texteditPassword = FindViewById<EditText>(Resource.Id.textedit_password);
            texteditPassword.InputType = Android.Text.InputTypes.TextVariationPassword | Android.Text.InputTypes.ClassText;

            buttonRegister = FindViewById<Button>(Resource.Id.button_register);
            buttonRegister.Click += redirectToRegisterActivity;

            textviewCantLogin = FindViewById<TextView>(Resource.Id.textview_cantlogin);
            textviewCantLogin.Click += redirectToLoginProblemsActivity;

            userList = new List<User>();
        }

        private void login(object sender, EventArgs eventArgs)
        {
            retrieveUserData();

            if (!dataretrieved) 
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

        private void redirectToDiveSessionDetailViewActivity(object sender, EventArgs eventArgs)
        {
            var diveSessionDetailActivity = new Intent(this, typeof(DiveSessionDetailViewActivity));
            StartActivity(diveSessionDetailActivity);
        }

        public void retrieveUserData() 
        {
            userDataListener = new UserDataListener();
            userDataListener.Query("users", "username", texteditUsername.Text);
            userDataListener.UserDataRetrieved += userDataListener_UserDataRetrieved;
        }

        private void userDataListener_UserDataRetrieved(object sender, UserDataListener.UserDataEventArgs e) 
        {
            userList = e.Users;
            dataretrieved = true;
            
            if (userList.Count > 0)
            {
                if (userList[0].username == texteditUsername.Text && userList[0].password == Encryptor.Encrypt(texteditPassword.Text))
                {
                    TemporaryData.USER_EMAIL = userList[0].email;
                    TemporaryData.USER_ID = userList[0].id;
                    TemporaryData.USER_NAME = userList[0].username;

                    var mainMenu = new Intent(this, typeof(MainActivity));
                    StartActivity(mainMenu);
                }
            } 
        }
    }
}