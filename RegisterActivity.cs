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
        private List<User> userResult;

        private bool accountCreated = false;

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

            userResult = new List<User>();

            button_register = FindViewById<Button>(Resource.Id.button_register);
            button_register.Click += setupDataListener;
        }

        private void setupDataListener(object sender, EventArgs e) 
        {
            userDataListener = new FirebaseDataListener();
            userDataListener.QueryFullTable("users");
            userDataListener.DataRetrieved += UserDataListener_UserDataRetrieved;
        }

        private void UserDataListener_UserDataRetrieved(object sender, FirebaseDataListener.DataEventArgs e)
        {
            userResult = e.Users;
            createAccount();
        }

        private void createAccount() 
        {
            string email = texteditEmail.Text.Trim();
            string username = texteditUsername.Text.Trim();
            string password = texteditPassword.Text.Trim();
            string firstname = texteditFirstname.Text.Trim();
            string lastname = texteditLastname.Text.Trim();
            string dateofbirth = texteditDateOfBirth.Text.Trim();
            string weight = texteditWeight.Text.Trim();
            string height = texteditHeight.Text.Trim();

            if (checkFieldsFilled())
            {
                Toast.MakeText(this, "Please fill all the fields in order to register a new account!", ToastLength.Long).Show();
            }
            else
            {
                bool userNameExists = false;
                bool emailExists = false;

                if (userResult != null) 
                {
                    foreach (User user in userResult) 
                    {
                        if (user.username.Equals(texteditUsername.Text.Trim())) 
                        {
                            userNameExists = true;
                            break;
                        }
                        if (user.email.Equals(texteditEmail.Text.Trim())) 
                        {
                            emailExists = true;
                            break;
                        }
                    }
                }

                if (userNameExists && !accountCreated)
                {
                    Toast.MakeText(this, "This username is already in use, please choose another one!", ToastLength.Long).Show();
                    return;
                }
                else if (emailExists && !accountCreated)
                {
                    Toast.MakeText(this, "This email is already in use, please choose another one!", ToastLength.Long).Show();
                    return;
                }
                else 
                {
                    if (!userNameExists && !emailExists)
                    {
                        User saveUser = new User("", username, email, password, firstname, lastname, dateofbirth, weight, height);

                        SupportV7.AlertDialog.Builder saveDataDialog = new SupportV7.AlertDialog.Builder(this);
                        saveDataDialog.SetTitle("Save User Information");
                        saveDataDialog.SetMessage("Are you sure?");

                        saveDataDialog.SetPositiveButton("Accept", (senderAlert, args) =>
                        {
                            userDataListener.saveEntity("users", saveUser);
                            var loginActivity = new Intent(this, typeof(LoginActivity));
                            StartActivity(loginActivity);
                            accountCreated = true;
                            Toast.MakeText(this, "Your Account has been created!", ToastLength.Long).Show();
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

        private bool checkFieldsFilled() 
        {
            return 
            string.IsNullOrEmpty(texteditEmail.Text.Trim())        ||
            string.IsNullOrEmpty(texteditUsername.Text.Trim())     ||
            string.IsNullOrEmpty(texteditPassword.Text.Trim())     ||
            string.IsNullOrEmpty(texteditFirstname.Text.Trim())    ||
            string.IsNullOrEmpty(texteditLastname.Text.Trim())     ||
            string.IsNullOrEmpty(texteditDateOfBirth.Text.Trim())  ||
            string.IsNullOrEmpty(texteditWeight.Text.Trim())       ||
            string.IsNullOrEmpty(texteditHeight.Text.Trim());
        }
    }
}