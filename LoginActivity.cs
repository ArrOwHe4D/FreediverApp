using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Widget;
using FreediverApp.DatabaseConnector;

namespace FreediverApp
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true, ScreenOrientation = ScreenOrientation.Portrait)]
    public class LoginActivity : Activity
    {
        private TextView textviewCantLogin;
        private Button buttonRegister, buttonLogin;
        private EditText texteditUsername, texteditPassword;

        private FirebaseDataListener userDataListener;
        private List<User> userList;

        private ProgressDialog loginDialog;

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
            loginDialog = new ProgressDialog(this);
            loginDialog.SetMessage("Logging in...");
            loginDialog.SetCancelable(false);
            loginDialog.Show();
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
            userDataListener = new FirebaseDataListener();
            userDataListener.QueryParameterized("users", "username", texteditUsername.Text);
            userDataListener.DataRetrieved += userDataListener_UserDataRetrieved;
        }

        private void userDataListener_UserDataRetrieved(object sender, FirebaseDataListener.DataEventArgs e) 
        {
            userList = e.Users;

            if (userList != null)
            {
                //username was found now check for password match
                if (userList[0].username == texteditUsername.Text && userList[0].password == Encryptor.Encrypt(texteditPassword.Text))
                {
                    //set temporary user data 
                    TemporaryData.USER_EMAIL = userList[0].email;
                    TemporaryData.USER_ID = userList[0].id;
                    TemporaryData.USER_NAME = userList[0].username;
                    User.curUser = userList[0];

                    loginDialog.Cancel();
                    loginDialog.Dispose();
                   
                    var mainMenu = new Intent(this, typeof(MainActivity));
                    StartActivity(mainMenu);
                }
                else 
                {
                    
                    loginDialog.Cancel();
                    loginDialog.Dispose();
                    userList.Clear();
                    Toast.MakeText(this, "You entered a wrong user id or password!", ToastLength.Long).Show();
                }
            }
            else 
            {
                loginDialog.Cancel();
                loginDialog.Dispose();
                Toast.MakeText(this, "You entered a wrong user id or password!", ToastLength.Long).Show();
            }
        }
    }
}