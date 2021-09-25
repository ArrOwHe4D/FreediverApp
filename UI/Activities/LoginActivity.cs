using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Widget;
using FreediverApp.DatabaseConnector;
using Xamarin.Essentials;

namespace FreediverApp
{
    /**
     *  This activity is the entry activity of our app where a user is able to login with his account data or 
     *  call the register activity to register a new account in case he is not yet registered.
     **/
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true, ScreenOrientation = ScreenOrientation.Portrait)]
    public class LoginActivity : Activity
    {
        /*Member variables of this activity*/
        private TextView textviewCantLogin;
        private Button buttonRegister, buttonLogin;
        private EditText texteditUsername, texteditPassword;
        private CheckBox checkboxRemember;

        private FirebaseDataListener userDataListener;
        private List<User> userList;

        private ProgressDialog loginDialog;

        /**
         * This function is called when the activity is going to be initialized on start.
         * Like in any other activity or fragment, the ui components are being initialized
         * by getting there IDs from the corresponding frontend XML files. At the end the list
         * that will hold the data retrieved from our db listener will also be initialized.
         **/
        protected override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.LoginPage);

            Platform.Init(this, savedInstanceState);

            buttonLogin = FindViewById<Button>(Resource.Id.button_login);
            buttonLogin.Click += login;

            texteditUsername = FindViewById<EditText>(Resource.Id.textedit_username);

            texteditPassword = FindViewById<EditText>(Resource.Id.textedit_password);
            texteditPassword.InputType = Android.Text.InputTypes.TextVariationPassword | Android.Text.InputTypes.ClassText;

            checkboxRemember = FindViewById<CheckBox>(Resource.Id.checkbox_remember);
            try
            {
                var rememberMeChecked = await SecureStorage.GetAsync("rememberMeChecked");
                if(rememberMeChecked == "true")
                {
                    checkboxRemember.Checked = true;
                    autoFillUserCredentials();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            buttonRegister = FindViewById<Button>(Resource.Id.button_register);
            buttonRegister.Click += redirectToRegisterActivity;

            textviewCantLogin = FindViewById<TextView>(Resource.Id.textview_cantlogin);
            textviewCantLogin.Click += redirectToLoginProblemsActivity;

            userList = new List<User>();

        }

        /**
         *  This function represents the whole login process up to the point where the user is successfully logged in and
         *  redirected to our main menu (MainActivity).
         **/
        private async void login(object sender, EventArgs eventArgs)
        {
            var connectionState = Connectivity.NetworkAccess;

            //File.WriteAllText(System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData) + "/pending_sessions.ps", "");
            //string content = File.ReadAllText(System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData) + "/pending_sessions.ps");

            if(checkboxRemember.Checked)
            {
                try
                {
                    await SecureStorage.SetAsync("rememberMeChecked", "true");
                    await SecureStorage.SetAsync("username", texteditUsername.Text);
                    await SecureStorage.SetAsync("password", texteditPassword.Text);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }

            //check if the phone is connected to the internet, otherwise print a error message.
            if (connectionState == NetworkAccess.Internet)
            {
                //setup the db listener and wait for results to be fetched by the eventlistener
                //while the results are being fetched, show a loading dialog to notify the user that data is being retrieved right now
                retrieveUserData();
                runLoginDialog();
            }
            else
            {
                Toast.MakeText(this, "Your Phone is not connected to the internet, please establish a connection first!", ToastLength.Long).Show();
            }
        }

        /**
         *  This function redirects to a new activity in which the user is able to restore his password 
         *  NOTE: Password recovery is not implemented in this version so the activity has no logic and only the frontend is setup!
         **/
        private void redirectToLoginProblemsActivity(object sender, EventArgs eventArgs) 
        {
            var loginProblemsActivity = new Intent(this, typeof(LoginProblemsActivity));
            StartActivity(loginProblemsActivity);
        }

        /**
         *  This function redirects to a the register activity where the user can create a new account for our app 
         **/
        private void redirectToRegisterActivity(object sender, EventArgs eventArgs) 
        {
            var registerActivity = new Intent(this, typeof(RegisterActivity));
            StartActivity(registerActivity);
        }

        /**
         *  This function initializes our database listener and starts a query for a user with the entered
         *  username from the textedit. It also binds the DataRetrieved event of our listen that is defined below
         *  inside the next function. This event is triggered when new data was received from the firebase db.
         **/
        public void retrieveUserData() 
        {
            userDataListener = new FirebaseDataListener();
            userDataListener.QueryParameterized("users", "username", texteditUsername.Text.Trim());
            userDataListener.DataRetrieved += userDataListener_UserDataRetrieved;         
        }

        /**
         *  This is the event that is triggered when new userdata was received from the db. At first we
         *  set the userList that holds all userdata to the result list that was filled by the db listener
         *  and is stored inside the DataEventArgs to pass it to our activity. This event also contains the 
         *  logic for the logincheck. Since the saved password was hashed we need to compare the password
         *  with the hashed value of the text inside the password textedit.
         **/
        private void userDataListener_UserDataRetrieved(object sender, FirebaseDataListener.DataEventArgs e) 
        {
            userList = e.Users;

            if (userList != null)
            {
                //username was found now check for password match
                if (userList[0].username == texteditUsername.Text.Trim() && userList[0].password == CryptoService.Encrypt(texteditPassword.Text))
                {
                    //set temporary user data (just for convenience purposes to populate queries easier that refer the user inside the app) 
                    TemporaryData.CURRENT_USER = userList[0];

                    //close the login loading popup, build and start the main activity and print a toast message afterwards
                    loginDialog.Dismiss();
                    var mainMenu = new Intent(this, typeof(MainActivity));
                    StartActivity(mainMenu);
                    Toast.MakeText(this, Resource.String.login_successful, ToastLength.Long).Show();
                }
                else 
                {
                    //login was not successfull, print a error message
                    loginDialog.Dismiss();
                    Toast.MakeText(this, Resource.String.wrong_username_or_pass, ToastLength.Long).Show();
                }
            }
            else 
            {
                //if the user list is null, login also failed so print a error message
                loginDialog.Dismiss();
                Toast.MakeText(this, Resource.String.wrong_username_or_pass, ToastLength.Long).Show();
            }
        }

        private void runLoginDialog() 
        {
            loginDialog = new ProgressDialog(this);
            loginDialog.SetMessage(ApplicationContext.Resources.GetString(Resource.String.dialog_logging_in));
            loginDialog.SetCancelable(false);
            loginDialog.Show();
        }

        /**
         *  This function reads user credentials from Android Secure Storage API to automatically fill the login text fields.
         **/
        private async void autoFillUserCredentials()
        {
            try
            {
                var username = await SecureStorage.GetAsync("username");
                var password = await SecureStorage.GetAsync("password");
                texteditUsername.Text = username;
                texteditPassword.Text = password;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}