using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V4.View;
using Android.Support.V4.Widget;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;

namespace FreediverApp
{
#pragma warning disable CS0618 // Type or member is obsolete

    public class AccountPanelFragment : Fragment
    {
        // use edit buttons as Imageviews as it is easier and costs less resources
        ImageView btnPencilEmail, btnPencilPassword, btnPencilFirstname, btnPencilLastname, btnPencilDateOfBirth, btnPencilHeight, btnPencilWeight;

        TextView txtViewEmail, txtViewPassword, txtViewFirstname, txtViewLastname, txtViewDateOfBirth, txtViewHeight, txtViewWeight;

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

            return view;
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
    }
#pragma warning restore CS0618 // Type or member is obsolete
}