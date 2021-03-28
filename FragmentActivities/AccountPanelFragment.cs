using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using FreediverApp.DatabaseConnector;
using SupportV7 = Android.Support.V7.App;

namespace FreediverApp
{
    public class AccountPanelFragment : Fragment
    {
        private TextView txtViewEmail, txtViewPassword, txtViewFirstname, txtViewLastname, txtViewDateOfBirth, txtViewHeight, txtViewWeight;
        private TextView titleUsername, titleRegisteredSince;
        private Button btnDeleteAccount;

        private FirebaseDataListener userDataListener;
        private List<User> userList;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.AccountPanelPage, container, false);

            btnDeleteAccount = view.FindViewById<Button>(Resource.Id.button_delete_account);

            btnDeleteAccount.Click += deleteUserAccount;

            //Instantiate TextView fields that hold the account data values that are being edited by modal dialogs
            txtViewEmail = view.FindViewById<TextView>(Resource.Id.txtview_email);
            txtViewPassword = view.FindViewById<TextView>(Resource.Id.txtview_password);
            txtViewFirstname = view.FindViewById<TextView>(Resource.Id.txtview_firstname);
            txtViewLastname = view.FindViewById<TextView>(Resource.Id.txtview_lastname);
            txtViewDateOfBirth = view.FindViewById<TextView>(Resource.Id.txtview_date_of_birth);
            txtViewHeight = view.FindViewById<TextView>(Resource.Id.txtview_height);
            txtViewWeight = view.FindViewById<TextView>(Resource.Id.txtview_weight);

            titleUsername = view.FindViewById<TextView>(Resource.Id.title_username);
            titleRegisteredSince = view.FindViewById<TextView>(Resource.Id.title_registered_since);

            //get userdata from db
            retrieveAccountData();

            return view;
        }

        private void fillUserData(List<User> userdata) 
        {   
            if (userdata != null) 
            {
                titleUsername.Text = userdata[0].username;
                titleRegisteredSince.Text = Context.Resources.GetString(Resource.String.registered_since) + " " + userdata[0].registerdate;
                txtViewEmail.Text = userdata[0].email;
                txtViewPassword.Text = "********";
                txtViewFirstname.Text = userdata[0].firstname;
                txtViewLastname.Text = userdata[0].lastname;
                txtViewDateOfBirth.Text = userdata[0].dateOfBirth;
                txtViewWeight.Text = userdata[0].weight + " kg";
                txtViewHeight.Text = userdata[0].height + " cm";
            }
        }

        public void retrieveAccountData() 
        {
            userDataListener = new FirebaseDataListener();
            userDataListener.QueryParameterized("users", "username", TemporaryData.CURRENT_USER.username);
            userDataListener.DataRetrieved += UserDataListener_UserDataRetrieved;
        }

        private void UserDataListener_UserDataRetrieved(object sender, FirebaseDataListener.DataEventArgs e)
        {
            userList = e.Users;
            fillUserData(userList);
        }

        private void deleteUserAccount(object sender, EventArgs e) 
        {
            SupportV7.AlertDialog.Builder deleteUserDialog = new SupportV7.AlertDialog.Builder(this.Context);
            deleteUserDialog.SetTitle(Resource.String.dialog_delete_account);
            deleteUserDialog.SetMessage(Resource.String.dialog_are_you_sure);

            deleteUserDialog.SetPositiveButton(Resource.String.dialog_accept, (senderAlert, args) =>
            {
                userDataListener.deleteEntity("users", TemporaryData.CURRENT_USER.id);
                var loginActivity = new Intent(Context, typeof(LoginActivity));
                StartActivity(loginActivity);
                Toast.MakeText(Context, Resource.String.account_deleted, ToastLength.Long).Show();
            });
            deleteUserDialog.SetNegativeButton(Resource.String.dialog_cancel, (senderAlert, args) =>
            {
                deleteUserDialog.Dispose();
            });

            deleteUserDialog.Show();
        }
    }
}