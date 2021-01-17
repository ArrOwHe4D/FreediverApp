using Android.App;
using Android.OS;
using Android.Widget;

namespace FreediverApp
{
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