using System;
using System.Globalization;
using Android.App;
using Android.OS;
using Android.Widget;
using SupportV7 = Android.Support.V7.App;
using FreediverApp.DatabaseConnector;
using Android.Content;
using System.Collections.Generic;
using Android.Content.PM;
using FreediverApp.UI.Fragments;

namespace FreediverApp
{
    /**
     *  This activity handles the whole registration process for new users. 
     **/
    [Activity(Label = "RegisterActivity", ScreenOrientation = ScreenOrientation.Portrait)]
    public class RegisterActivity : Activity
    {
        /*Member Variables*/
        private Button buttonRegister;
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

       /**
        *  This function initializes all UI components and event listeners. 
        **/
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
            texteditDateOfBirth.Click += openDatePickerDialog;

            texteditWeight = FindViewById<EditText>(Resource.Id.textedit_weight);
            texteditHeight = FindViewById<EditText>(Resource.Id.textedit_height);

            userResult = new List<User>();

            buttonRegister = FindViewById<Button>(Resource.Id.button_register);
            buttonRegister.Click += setupDataListener;
        }

        private void openDatePickerDialog(object sender, EventArgs eventArgs) 
        {
            DatePickerFragment datePicker = DatePickerFragment.NewInstance(delegate (DateTime dateTime) { texteditDateOfBirth.Text = dateTime.ToShortDateString(); });
            datePicker.Show(FragmentManager, DatePickerFragment.TAG);
        }

        /**
         *  This function intializes the db listener and queries all userdata to determine if users with the
         *  entered username or email already exist. 
         *  TODO: Optimize using a parameterized query with OR statement on username and email so that we 
         *  dont need to query the full table all the time.
         **/
        private void setupDataListener(object sender, EventArgs eventArgs) 
        {
            userDataListener = new FirebaseDataListener();
            userDataListener.QueryFullTable("users");
            userDataListener.DataRetrieved += UserDataListener_UserDataRetrieved;
        }

        /**
         *  Event that is triggered when new userdata was retrieved from the db listener.
         *  After the result list is set, the registration process is initiated inside the createAccount function.
         **/
        private void UserDataListener_UserDataRetrieved(object sender, FirebaseDataListener.DataEventArgs eventArgs)
        {
            userResult = eventArgs.Users;
            createAccount();
        }

        /**
         *  This function handles the whole registration process after data was retrieved from the db listener. 
         **/
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

            //check if all fields were filled, if not, print a error message on the UI.
            if (checkForMissingField())
            {
                Toast.MakeText(this, Resource.String.register_fill_all_fields, ToastLength.Long).Show();
            }
            else
            {
                //check if the entered email is valid, if not, print a error message on the UI.
                if (!isValidEmail(email))
                {
                    Toast.MakeText(this, Resource.String.register_email_not_valid, ToastLength.Long).Show();
                    return;
                }

                //check if the entered email is valid, if not, print a error message on the UI.
                if (!isValidBirthday(dateofbirth))
                {
                    Toast.MakeText(this, "Birthday is not valid!", ToastLength.Long).Show();
                    return;
                }

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

                //if the username or the email already exists, print a proper error message
                if (userNameExists && !accountCreated)
                {
                    Toast.MakeText(this, Resource.String.register_username_exists, ToastLength.Long).Show();
                    return;
                }
                else if (emailExists && !accountCreated)
                {
                    Toast.MakeText(this, Resource.String.register_email_exists, ToastLength.Long).Show();
                    return;
                }
                else 
                {
                    //If all error checks are successfull, create a user instance and save that instance to db
                    if (!userNameExists && !emailExists)
                    {
                        User saveUser = new User("", username, email, password, firstname, lastname, dateofbirth, weight, height);

                        //Create a confirmation dialog that needs to be accepted in order to complete the registration
                        SupportV7.AlertDialog.Builder saveDataDialog = new SupportV7.AlertDialog.Builder(this);
                        saveDataDialog.SetTitle(Resource.String.dialog_save_user_info);
                        saveDataDialog.SetMessage(Resource.String.dialog_are_you_sure);

                        saveDataDialog.SetPositiveButton(Resource.String.dialog_accept, (senderAlert, args) =>
                        {
                            //if the dialog was accepted, save the user to db, redirect back to the login 
                            //activity and print a toast message that the registration was successful
                            userDataListener.saveEntity("users", saveUser);
                            var loginActivity = new Intent(this, typeof(LoginActivity));
                            StartActivity(loginActivity);
                            accountCreated = true;
                            Toast.MakeText(this, Resource.String.account_created, ToastLength.Long).Show();
                        });
                        saveDataDialog.SetNegativeButton(Resource.String.dialog_cancel, (senderAlert, args) =>
                        {
                            saveDataDialog.Dispose();
                        });

                        //show the confirmation dialog -> Is called afterwards here because the actionhandlers were defined as lambda expressions
                        saveDataDialog.Show();
                    }
                }
            }
        }

        /**
         *  Helperfunction to determine if all fields of the registration form were filled. 
         **/
        private bool checkForMissingField() 
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

        /**
         *  Helper function to determine if the entered email is valid. 
         **/
        private bool isValidEmail(string email)
        {
            try
            {
                var emailAdress = new System.Net.Mail.MailAddress(email);
                return emailAdress.Address == email;
            }
            catch
            {
                return false;
            }
        }

        /**
         *  Helper function to determine if the birtdate entered is valid. 
         **/
        private bool isValidBirthday(string birthday)
        {
            try
            {
                birthday = birthday.Replace('.', Convert.ToChar(@"-"));
                string pattern = "dd-MM-yyyy";

                DateTime bday;
                DateTime.TryParseExact(birthday, pattern, null, DateTimeStyles.None, out bday);

                if (bday.Date <= DateTime.Now.Date)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
    }
}