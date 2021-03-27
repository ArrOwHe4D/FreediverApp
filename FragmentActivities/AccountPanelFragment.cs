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
    /**
     *  This Fragment displays the account data of the current user that is signed in. The user also has the possibility to 
     *  delete his account with a button on the bottom of the form. All dive data from that user still remains inside the databse 
     *  after deletion.
     **/
    public class AccountPanelFragment : Fragment
    {
        /*Member Variables (UI Components from XML)*/
        private TextView txtViewEmail, txtViewPassword, txtViewFirstname, txtViewLastname, txtViewDateOfBirth, txtViewHeight, txtViewWeight;
        private TextView titleUsername, titleRegisteredSince;
        private Button btnDeleteAccount;

        private FirebaseDataListener userDataListener;
        private List<User> userList;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        /**
         *  This function initializes all UI components from the corresponding XML file that is passed into the inflate function.
         *  In the last step the db listener is being initialized and activated to retrieve userdata from db.
         **/
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

            //setup the db listener to retrieve userdata from db
            retrieveAccountData();

            return view;
        }

        /**
         *  This function fills all the textfields with the userdata that was retrieved by the db listener. 
         **/
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

        /**
         *  This function initializes the db listener and the eventhandler that is triggered when new data was received from db.
         *  In this case we want to query for a dataset which has the same username as our currently logged in user that was saved
         *  inside the TemporaryData class. Since we don´t store all user info inside that class we need to query to get all attributes of the
         *  user dataset.
         **/
        public void retrieveAccountData() 
        {
            userDataListener = new FirebaseDataListener();
            userDataListener.QueryParameterized("users", "username", TemporaryData.CURRENT_USER.username);
            userDataListener.DataRetrieved += UserDataListener_UserDataRetrieved;
        }

        /**
         *  This function handles the retrieval of data from db. When data is retrieved, set it to the userList of this 
         *  fragment and then fill all the textfields with the received userdata.
         **/
        private void UserDataListener_UserDataRetrieved(object sender, FirebaseDataListener.DataEventArgs e)
        {
            userList = e.Users;
            fillUserData(userList);
        }

        /**
         *  This function handles the deletion of the user account. At first a confirmation dialog is displayed so that 
         *  the user is not automatically deleted when he pressed the delete button by accident for example. When the dialog
         *  was accepted, the deleteEntity function of the FirebaseDatalistener is called to delete the current user from db.
         *  After deletion was completed, the user is redirected to the login activity and a toast message is printed to notify 
         *  the user that the deletion was successful.
         **/
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