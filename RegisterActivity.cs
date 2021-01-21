using System;
using Android.App;
using Android.OS;
using Android.Widget;
using Java.Util;
using SupportV7 = Android.Support.V7.App;
using Firebase.Database;
using FreediverApp.DatabaseConnector;
using DBConnector = FreediverApp.DatabaseConnector.DatabaseConnector;
using Android.Content;
using System.Collections.Generic;
using Android.Content.PM;

namespace FreediverApp
{
    [Activity(Label = "RegisterActivity", ScreenOrientation = ScreenOrientation.Portrait)]
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

        private FirebaseDataListener userDataListener;
        private List<User> userList;

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

            userList = new List<User>();
            setupDataListener();
        }

        private void setupDataListener() 
        {
            userDataListener = new FirebaseDataListener();
            userDataListener.QueryParameterized("users", "email", texteditEmail.Text);
            userDataListener.DataRetrieved += UserDataListener_UserDataRetrieved;
        }

        private void UserDataListener_UserDataRetrieved(object sender, FirebaseDataListener.DataEventArgs e)
        {
            userList = e.Users;   
        }

        private void createAccount(object sender, EventArgs eventArgs) 
        {
            setupDataListener();
            string email = texteditEmail.Text;
            string username = texteditUsername.Text;
            string password = texteditPassword.Text;
            string firstname = texteditFirstname.Text;
            string lastname = texteditLastname.Text;
            string dateofbirth = texteditDateOfBirth.Text;
            string weight = texteditWeight.Text;
            string height = texteditHeight.Text;

            if (string.IsNullOrEmpty(email)         ||
                string.IsNullOrEmpty(username)      ||
                string.IsNullOrEmpty(password)      ||   
                string.IsNullOrEmpty(firstname)     ||
                string.IsNullOrEmpty(lastname)      ||
                string.IsNullOrEmpty(dateofbirth)   ||
                string.IsNullOrEmpty(weight)        ||
                string.IsNullOrEmpty(height))
            {
                Toast.MakeText(this, "Please fill all the fields in order to register a new account!", ToastLength.Long).Show();
            }
            else
            {
                bool userNameExists = false;
                bool emailExists = false;

                if (userList != null) 
                {
                    userNameExists = userList[0].username.Equals(texteditUsername.Text);
                    emailExists = userList[0].email.Equals(texteditEmail.Text);
                }

                if (userNameExists)
                {
                    Toast.MakeText(this, "This username is already in use, please choose another one!", ToastLength.Long).Show();
                    return;
                }
                else if (emailExists)
                {
                    Toast.MakeText(this, "This email is already in use, please choose another one!", ToastLength.Long).Show();
                    return;
                }
                else 
                {
                    if (!userNameExists && !emailExists)
                    {
                        HashMap userData = new HashMap();
                        userData.Put("email", email);
                        userData.Put("username", username);
                        userData.Put("password", Encryptor.Encrypt(password));
                        userData.Put("firstname", firstname);
                        userData.Put("lastname", lastname);
                        userData.Put("birthday", dateofbirth);
                        userData.Put("weight", weight);
                        userData.Put("height", height);
                        userData.Put("registerdate", DateTime.Now.Date.ToString("dd.MM.yyyy"));

                        SupportV7.AlertDialog.Builder saveDataDialog = new SupportV7.AlertDialog.Builder(this);
                        saveDataDialog.SetTitle("Save User Information");
                        saveDataDialog.SetMessage("Are you sure?");

                        saveDataDialog.SetPositiveButton("Accept", (senderAlert, args) =>
                        {
                            DatabaseReference newUserRef = DBConnector.GetDatabase().GetReference("users").Push();
                            newUserRef.SetValue(userData);
                            var loginActivity = new Intent(this, typeof(LoginActivity));
                            StartActivity(loginActivity);
                        });
                        saveDataDialog.SetNegativeButton("Cancel", (senderAlert, args) =>
                        {
                            saveDataDialog.Dispose();
                        });

                        saveDataDialog.Show();
                    }
                }
            }
        }
    }
}