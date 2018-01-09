using System;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using Android.Support.V4.Content.Res;

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
            ///Change tab's icon color according to "state_selected" status.
            if (Convert.ToInt32(Build.VERSION.SdkInt) >= 23)    //Couldn't find the equivalent for android5.1 or less. Probably need to use 2 different icon files for each icon (in 2 different colors).
            {
                Android.Support.V4.Graphics.Drawable.DrawableCompat.SetTintList(GetDrawable(Resource.Drawable.icon_events), GetColorStateList(Resource.Color.TabIconsColors));
                Android.Support.V4.Graphics.Drawable.DrawableCompat.SetTintList(GetDrawable(Resource.Drawable.icon_websites), GetColorStateList(Resource.Color.TabIconsColors));
                Android.Support.V4.Graphics.Drawable.DrawableCompat.SetTintList(GetDrawable(Resource.Drawable.icon_more), GetColorStateList(Resource.Color.TabIconsColors));
            }
            //Android.Support.V4.Graphics.Drawable.DrawableCompat.SetTintMode(GetDrawable(Resource.Drawable.icon_events), Android.Graphics.PorterDuff.Mode.SrcAtop);
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(bundle);
            //AlertDialog.Builder builder = new AlertDialog.Builder(this);
            //builder.SetMessage("OnCreate!");
            //builder.Show();

            global::Xamarin.Forms.Forms.Init(this, bundle);

            LoadApplication(new App());
        }
    }
}