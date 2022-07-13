using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.OS;
using Android;
using AndroidX.Core.App;
using Xamarin.Forms;
using HGB.Interface;
using HGB.ViewModel;

namespace HGB.Droid
{
    [Activity(Label = "HGB", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize )]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);

            var permissions = new string[]
            {
                Manifest.Permission.WakeLock,
                Manifest.Permission.ForegroundService,
                Manifest.Permission.ReadPhoneState,
                Manifest.Permission.ReadCallLog,
                Manifest.Permission.ReadContacts,
                Manifest.Permission.WriteContacts,
            };
            ActivityCompat.RequestPermissions(this, permissions, 123);

            LoadApplication(new App());
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum]Permission[] grantResults)
        {
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            if (requestCode == 123 && grantResults.Length > 0 && grantResults[0] == Permission.Granted)
            {
                if (!DependencyService.Get<ICallServiceHelper>().IsMyServiceRunning())
                {
                    DependencyService.Get<ICallServiceHelper>().StartMyService();
                }
            }
        }
    }
}