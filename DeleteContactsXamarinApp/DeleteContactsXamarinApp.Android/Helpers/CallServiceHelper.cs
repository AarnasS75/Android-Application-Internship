using Android.Content;
using Android.OS;
using HGB.Interface;
using HGB.Droid.Helpers;
using Xamarin.Forms;
using HGB.ViewModel;
using Android.App;

[assembly: Dependency(typeof(CallServiceHelper))]
namespace HGB.Droid.Helpers
{
    public class CallServiceHelper : ICallServiceHelper
    {
        Context context = Android.App.Application.Context;

        LabelModel labelModel = new LabelModel();

        public void StartMyService()
        {
            var intent = new Intent(context, typeof(MyForegroundSevice));

            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                context.StartForegroundService(intent);
            }
            else
            {
                context.StartService(intent);
            }
        }

        public bool IsMyServiceRunning()
        {
            ActivityManager manager = (ActivityManager)context.GetSystemService(Context.ActivityService);
            foreach (var service in manager.GetRunningServices(int.MaxValue))
            {
                if (Java.Lang.Class.FromType(typeof(MyForegroundSevice)).Name.Equals(service.Service.ClassName))
                {
                    return true;
                }
            }
            return false;
        }

        public void StopMyService()
        {
            var intent = new Intent(context, typeof(MyForegroundSevice));
            context.StopService(intent);
        }

        public void UnsubscribeMessages()
        {
            MessagingCenter.Unsubscribe<object, string>(labelModel, "Sekmingai"); 
            MessagingCenter.Unsubscribe<object, string>(labelModel, "Atnaujinti");
            MessagingCenter.Unsubscribe<object, string>(labelModel, "Bandyta");
        }
    }
}