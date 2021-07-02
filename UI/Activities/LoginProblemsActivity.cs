using Android.App;
using Android.OS;
using Android.Widget;
using FreediverApp.DatabaseConnector;
using FreediverApp.Utils;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography.X509Certificates;

namespace FreediverApp
{
    /**
     *  This activity is used to initiate the password recovery service via smtp client and a generated link 
     *  inside a email that is received by the user if a valid email was entered.
     *  NOTE: This is not implemented in the current version and needs to be done in a future release!
     **/
    [Activity(Label = "LoginProblemsActivity")]
    public class LoginProblemsActivity : Activity
    {
        private EditText txtEmail;
        private Button btnRecoverPassword;
        private ProgressDialog sendingMailDialog;

        private FirebaseDataListener userDataListener;
        private List<User> userList;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.LoginProblemsPage);

            txtEmail = FindViewById<EditText>(Resource.Id.textedit_username_email);
            btnRecoverPassword = FindViewById<Button>(Resource.Id.button_recover_password);

            btnRecoverPassword.Click += initiatePasswordRecovery;
        }

        private void initiatePasswordRecovery(object sender, EventArgs eventArgs) 
        {
            if (FreediverHelper.validateEmail(txtEmail.Text))
            {
                sendingMailDialog = new ProgressDialog(this);
                sendingMailDialog.SetMessage("Email wird gesendet...");
                sendingMailDialog.SetCancelable(false);
                sendingMailDialog.Show();
                retrieveUserData();
            }
            else 
            {
                Toast.MakeText(this, "Bitte geben Sie eine gültige Email Adresse an!", ToastLength.Long).Show();
            }
        }

        private void retrieveUserData() 
        {
            userDataListener = new FirebaseDataListener();
            userDataListener.QueryParameterized("users", "email", txtEmail.Text);
            userDataListener.DataRetrieved += UserDataListener_UserDataRetrieved;
        }

        private void UserDataListener_UserDataRetrieved(object sender, FirebaseDataListener.DataEventArgs eventArgs) 
        {
            userList = eventArgs.Users;

            //user with given email exists
            if (userList != null) 
            {
                sendPasswordRecoveryMail();
            }
            sendingMailDialog.Dismiss();
            Toast.MakeText(this, "Falls die angegebene Email registriert ist, wurde Ihnen ein neues Passwort zugeschickt. Bitte Kontrollieren Sie Ihr Email Postfach!", ToastLength.Long).Show();
        }

        private void sendPasswordRecoveryMail() 
        {
            try
            {
                MailMessage mail = new MailMessage();

                //Create the Mailobject
                mail.From = new MailAddress(AuthenticationHelper.RECOVERY_SERVICE_EMAIL);
                mail.To.Add(txtEmail.Text);
                mail.Subject = "FreediverApp Password recovery"; 
                mail.Body = "Your new password: d8dj8923jd983j"; //TODO: generate password via freediver Crypto service class

                //Create the SMTP Client that transmits via smtp.gmail.com
                SmtpClient smtpServer = new SmtpClient(AuthenticationHelper.RECOVERY_SERVICE_MAILSERVER);
                smtpServer.Port = 587;
                smtpServer.UseDefaultCredentials = false;
                smtpServer.Credentials = new NetworkCredential(AuthenticationHelper.RECOVERY_SERVICE_EMAIL, AuthenticationHelper.RECOVERY_SERVICE_PASSWORD);
                smtpServer.EnableSsl = true;
                ServicePointManager.ServerCertificateValidationCallback = delegate (object sender, X509Certificate certificate, X509Chain chain, System.Net.Security.SslPolicyErrors sslPolicyErrors)
                {
                    return true;
                };
                smtpServer.Send(mail);
                //TODO: store generated password to userdata with updateEntity function
            }
            catch (Exception ex)
            {
                Toast.MakeText(Application.Context, ex.ToString(), ToastLength.Long).Show();
            }
        }
    }
}