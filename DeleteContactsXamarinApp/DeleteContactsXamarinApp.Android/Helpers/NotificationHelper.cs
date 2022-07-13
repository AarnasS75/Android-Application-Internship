using Android.App;
using Android.Content;
using Android.OS;
using AndroidX.Core.App;
using HGB.Droid;
using HGB.Interface;
using HGB.Droid.Helpers;
using Xamarin.Forms;

[assembly: Dependency(typeof(NotificationHelper))]
namespace HGB.Droid.Helpers
{
    public class NotificationHelper : INotificationHelper
    {
        private static string foregroundChannelId = "9001";
        private static Context context = Android.App.Application.Context;
        public Notification ReturnNotif()
        {
            // Building intent
            var intent = new Intent(context, typeof(MainActivity));
            intent.AddFlags(ActivityFlags.SingleTop);
            intent.PutExtra("Title", "Message");

            // Stop Foreground service intent
            Intent stopIntent = new Intent(context, typeof(MyForegroundSevice));
            stopIntent.SetAction("stopService");
            PendingIntent stopSeriveIntent;

            if (Build.VERSION.SdkInt >= BuildVersionCodes.M)
            {
                stopSeriveIntent = PendingIntent.GetService(context, 0, stopIntent, PendingIntentFlags.Immutable);
            }
            else
            {
                stopSeriveIntent = PendingIntent.GetService(context, 0, stopIntent, 0);
            }

            var notifBuilder = new NotificationCompat.Builder(context, foregroundChannelId)
                .SetContentTitle("Klausoma skambučių")
                .SetSmallIcon(Resource.Drawable.notification_template_icon_low_bg)
                .AddAction(Resource.Drawable.abc_btn_check_material, "STOP", stopSeriveIntent);
                

            // Building channel if API verion is 26 or above
            if (global::Android.OS.Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                NotificationChannel notificationChannel = new NotificationChannel(foregroundChannelId, "Title", NotificationImportance.High);
                notificationChannel.Importance = NotificationImportance.High;
                notificationChannel.EnableLights(false);
                notificationChannel.EnableVibration(false);
                notificationChannel.SetShowBadge(true);

                var notifManager = context.GetSystemService(Context.NotificationService) as NotificationManager;
                if (notifManager != null)
                {
                    notifBuilder.SetChannelId(foregroundChannelId);
                    notifManager.CreateNotificationChannel(notificationChannel);
                }
            }

            return notifBuilder.Build();
        }
    }
}