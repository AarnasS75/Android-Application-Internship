using Android.Content;
using Android.Telephony;
using HGB.Interface;
using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace HGB.Droid
{
    public class MyReceiver : BroadcastReceiver
    {
        IContactsHelper contactsHelper = DependencyService.Get<IContactsHelper>();
        bool isIncomingCall;
        string formatas = "yyyy-MM-dd HH:mm:ss";

        List<PhoneCall> tempCallList = new List<PhoneCall>();

        string IMEI()
        {
            return contactsHelper.GetIdentifier();
        }
        public override void OnReceive(Context context, Intent intent)
        {
            var state = intent.GetStringExtra(TelephonyManager.ExtraState);
            var number = intent.GetStringExtra(TelephonyManager.ExtraIncomingNumber);

            if (state == TelephonyManager.ExtraStateRinging)
            {
                if (number != null)
                {
                    isIncomingCall = true;
                    var phoneCall = new PhoneCall(IMEI(), number, "I", DateTime.Now.ToString(formatas), "N");
                    tempCallList.Add(phoneCall);
                }
            }
            else if (state == TelephonyManager.ExtraStateOffhook)
            {
                if (number != null)
                {
                    if (isIncomingCall)
                    {
                        var phoneCall = new PhoneCall(IMEI(), number, "I", DateTime.Now.ToString(formatas), "P");
                        tempCallList.Add(phoneCall);
                    }
                    else
                    {
                        var phoneCall = new PhoneCall(IMEI(), number, "O", DateTime.Now.ToString(formatas), "P");
                        tempCallList.Add(phoneCall);
                    }
                }
            }
            else if (state == TelephonyManager.ExtraStateIdle)
            {
                if (number != null)
                {
                    if (isIncomingCall)
                    {
                        var phoneCall = new PhoneCall(IMEI(), number, "I", DateTime.Now.ToString(formatas), "H");
                        tempCallList.Add(phoneCall);
                    }
                    else
                    {
                        var phoneCall = new PhoneCall(IMEI(), number, "O", DateTime.Now.ToString(formatas), "H");
                        tempCallList.Add(phoneCall);
                    }
                    contactsHelper.SendData(tempCallList);
                }
            }
        }
    }
}