using System;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views;

namespace AllProbe1.Droid
{
    [Activity(Label = "AllProbe", Theme = "@style/MyTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity, GestureDetector.IOnGestureListener
    {
        public bool OnDown(MotionEvent e)
        {
            return true;
        }

        public bool OnFling(MotionEvent e1, MotionEvent e2, float velocityX, float velocityY)
        {
            return true;
        }

        public void OnLongPress(MotionEvent e)
        {
            
        }

        public bool OnScroll(MotionEvent e1, MotionEvent e2, float distanceX, float distanceY)
        {
            return true;
        }

        public void OnShowPress(MotionEvent e)
        {
             
        }

        public bool OnSingleTapUp(MotionEvent e)
        {
            return true;
        }

        protected override void OnCreate(Bundle bundle)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(bundle);

            global::Xamarin.Forms.Forms.Init(this, bundle);

            LoadApplication(new App());

            //// Convert Android.Net.Url to Uri
            //var uri = new Uri(Intent.Data.ToString());

            //// Load redirectUrl page
            //AuthenticationState.Authenticator.OnPageLoading(uri);

            //Finish();  // Convert Android.Net.Url to Uri
        }
    }
}