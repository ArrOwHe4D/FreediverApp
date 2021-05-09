using Android.App;
using Android.OS;
using Android.Widget;

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
        private EditText txtUsernameEmail;
        private Button btnRecoverPassword;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.LoginProblemsPage);

            txtUsernameEmail = FindViewById<EditText>(Resource.Id.textedit_username_email);
            btnRecoverPassword = FindViewById<Button>(Resource.Id.button_recover_password);
        }
    }
}