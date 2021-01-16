using System;
using System.Collections.Generic;
using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using FreediverApp.DatabaseConnector;

namespace FreediverApp
{
    public class AccountPanelFragment : Fragment
    {
        // use edit buttons as Imageviews as it is easier and costs less resources
        private ImageView btnPencilEmail, btnPencilPassword, btnPencilFirstname, btnPencilLastname, btnPencilDateOfBirth, btnPencilHeight, btnPencilWeight;
        private TextView txtViewEmail, txtViewPassword, txtViewFirstname, txtViewLastname, txtViewDateOfBirth, txtViewHeight, txtViewWeight;
        private TextView titleUsername, titleRegisteredSince;

        private FirebaseDataListener userDataListener;
        private List<User> userList;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.AccountPanelPage, container, false);

            //Instantiate ImageView pencil Buttons for editing data
            btnPencilEmail = view.FindViewById<ImageView>(Resource.Id.btn_pencil_email);
            btnPencilPassword = view.FindViewById<ImageView>(Resource.Id.btn_pencil_password);
            btnPencilFirstname = view.FindViewById<ImageView>(Resource.Id.btn_pencil_firstname);
            btnPencilLastname = view.FindViewById<ImageView>(Resource.Id.btn_pencil_lastname);
            btnPencilDateOfBirth = view.FindViewById<ImageView>(Resource.Id.btn_pencil_date_of_birth);
            btnPencilHeight = view.FindViewById<ImageView>(Resource.Id.btn_pencil_height);
            btnPencilWeight = view.FindViewById<ImageView>(Resource.Id.btn_pencil_weight);

            btnPencilEmail.Click += editEmail;

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
            titleUsername.Text = userdata[0].username;
            titleRegisteredSince.Text = "registered since: " + userdata[0].registerdate;
            txtViewEmail.Text = userdata[0].email;
            txtViewPassword.Text = "********";
            txtViewFirstname.Text = userdata[0].firstname;
            txtViewLastname.Text = userdata[0].lastname;
            txtViewDateOfBirth.Text = userdata[0].dateOfBirth;
            txtViewWeight.Text = userdata[0].weight + " kg";
            txtViewHeight.Text = userdata[0].height + " cm"; 
        }

        public void editEmail(object sender, EventArgs eventArgs) 
        {
            LayoutInflater layoutInflater = LayoutInflater.From(this.Context);
            View dialogView = layoutInflater.Inflate(Resource.Layout.UserInputDialog, null);
            Android.Support.V7.App.AlertDialog.Builder dialogBuilder = new Android.Support.V7.App.AlertDialog.Builder(this.Context);
            dialogBuilder.SetView(dialogView);

            var editValueField = dialogView.FindViewById<EditText>(Resource.Id.userInput);
            dialogBuilder.SetCancelable(false)
                .SetPositiveButton("Speichern", delegate
                {
                    txtViewEmail.Text = editValueField.Text;
                    Toast.MakeText(this.Context, "Wert wurde erfolgreich geändert!", ToastLength.Long).Show();
                    dialogBuilder.Dispose();
                })
                .SetNegativeButton("Abbrechen", delegate
                {
                    dialogBuilder.Dispose();
                });

            Android.Support.V7.App.AlertDialog dialog = dialogBuilder.Create();
            dialog.Show();
        }

        public void retrieveAccountData() 
        {
            userDataListener = new FirebaseDataListener();
            string id = TemporaryData.USER_ID;
            userDataListener.QueryParameterized("users", "username", TemporaryData.USER_NAME);
            userDataListener.DataRetrieved += UserDataListener_UserDataRetrieved;
        }

        private void UserDataListener_UserDataRetrieved(object sender, FirebaseDataListener.DataEventArgs e)
        {
            userList = e.Users;
            fillUserData(userList);
        }
    }
}