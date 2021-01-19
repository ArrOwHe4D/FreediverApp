using Android.App;
using Android.OS;

namespace FreediverApp
{
    [Activity(Label = "DivesPerSessionActivity")]
    public class DivesPerSessionActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.DivesPerSessionPage);
        }
    }
}