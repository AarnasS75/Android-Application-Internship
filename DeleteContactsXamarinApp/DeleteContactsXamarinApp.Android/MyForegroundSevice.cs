using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Telephony;
using Android.Widget;
using HGB.Interface;
using HGB.ViewModel;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace HGB.Droid
{
    [Service]
    public class MyForegroundSevice : Service
    {
        ICallServiceHelper callServiceHelper = DependencyService.Get<ICallServiceHelper>();
        IContactsHelper contactsHelper = DependencyService.Get<IContactsHelper>();

        MyReceiver myReceiver;

        private PowerManager.WakeLock wl = null;

        void GetWakelock()
        {
            PowerManager pmanager = (PowerManager)GetSystemService(PowerService);
            wl = pmanager.NewWakeLock(WakeLockFlags.Partial, "myapp_wakelock");
            wl.SetReferenceCounted(false);
            wl.Acquire();
        }
        public override IBinder OnBind(Intent intent)
        {
            return null;
        }
        [return: GeneratedEnum]
        public override StartCommandResult OnStartCommand(Intent intent, [GeneratedEnum] StartCommandFlags flags, int startId)
        {
            IntentFilter filter = new IntentFilter();
            filter.AddAction(TelephonyManager.ActionPhoneStateChanged);

            switch (intent.Action)
            {
                case null:
                    GetWakelock();
                    myReceiver = new MyReceiver();
                    var notif = DependencyService.Get<INotificationHelper>().ReturnNotif();
                    StartUploadingContactsToServer();
                    RegisterReceiver(myReceiver, filter);
                    StartForeground(1001, notif);
                    break;
                case "stopService":
                    callServiceHelper.UnsubscribeMessages();
                    UnregisterReceiver(myReceiver);
                    wl.Release();
                    callServiceHelper.StopMyService();
                    break;
            }

            return StartCommandResult.NotSticky;
        }
        void StartUploadingContactsToServer()
        {
            Device.StartTimer(TimeSpan.FromMinutes(2), () =>
            {
                Task.Run(async () =>
                {
                    var logs = await App.Database.GetLogsAsync();
                    await contactsHelper.UploadContactsToServer(logs);
                });
                return true;
            });
            Device.StartTimer(TimeSpan.FromHours(24), () =>
            {
                Task.Run(async () =>
                {
                    await contactsHelper.UpdatePhonebook();
                });
                return true;
            });
        }
    }
}