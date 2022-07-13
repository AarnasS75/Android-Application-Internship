using Android.Content;
using Android.Database;
using Android.Provider;
using Android.Telephony;
using Android.Widget;
using HGB.Droid.Helpers;
using HGB.Interface;
using HGB.Model;
using Newtonsoft.Json;
using System.Collections.Generic;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Xamarin.Forms;
using static Android.Provider.ContactsContract;
using System.Threading.Tasks;
using HGB.ViewModel;

[assembly: Dependency(typeof(ContactsHelper))]
namespace HGB.Droid.Helpers
{
    public class ContactsHelper : IContactsHelper
    {
        Context thisContext = Android.App.Application.Context;
        string formatas = "yyyy-MM-dd HH:mm:ss";

        HttpClient client = new HttpClient();

        List<Repository> ObjContactList = new List<Repository>();

        public string GetIdentifier()
        {
            return Android.Provider.Settings.Secure.GetString(thisContext.ContentResolver, Android.Provider.Settings.Secure.AndroidId);
        }
        public void DeleteContact()
        {
            string[] Projection = new string[] { IContactsColumns.LookupKey, IContactsColumns.DisplayName };

            ICursor cursor = thisContext.ContentResolver.Query(ContactsContract.Contacts.ContentUri,
                       Projection, null, null, null);

            if (cursor == null)
            {
                return;
            }
            else
            {
                while (cursor.MoveToNext())
                {
                    string lookupKey = cursor.GetString(0);
                    var uri = Android.Net.Uri.WithAppendedPath(ContactsContract.Contacts.ContentLookupUri, lookupKey);
                    thisContext.ContentResolver.Delete(uri, null, null);
                }
                cursor.Close();
            }
        }
        public void CreateContacts(string name, string phone)
        {
            List<ContentProviderOperation> ops = new List<ContentProviderOperation>();
            int rawContactInsertIndex = ops.Count;

            ops.Add(ContentProviderOperation.NewInsert(RawContacts.ContentUri)
            .WithValue(RawContacts.InterfaceConsts.AccountType, null)
            .WithValue(RawContacts.InterfaceConsts.AccountName, null)
            .Build());

            ops.Add(ContentProviderOperation.NewInsert(Data.ContentUri)
                .WithValueBackReference(Data.InterfaceConsts.RawContactId, rawContactInsertIndex)
                .WithValue(Data.InterfaceConsts.Mimetype, CommonDataKinds.StructuredName.ContentItemType)
                .WithValue(CommonDataKinds.StructuredName.DisplayName, name)
                .Build());

            ops.Add(ContentProviderOperation.NewInsert(Data.ContentUri)
                .WithValueBackReference(Data.InterfaceConsts.RawContactId, rawContactInsertIndex)
                .WithValue(Data.InterfaceConsts.Mimetype, CommonDataKinds.Phone.ContentItemType)
                .WithValue(CommonDataKinds.Phone.Number, phone)
                .Build());

            thisContext.ContentResolver.ApplyBatch(Authority, ops);
        }
        public async Task SendData(List<PhoneCall> phoneCalls)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                MessagingCenter.Send<object, string>(this, "Bandyta", DateTime.Now.ToString());
            });

            if (NetworkCheck.IsInternet())
            {
                
                var uri = new Uri("http://phone.higsobozonas.lt/call.aspx");
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response;

                foreach (var item in phoneCalls)
                {
                    string json = JsonConvert.SerializeObject(item);

                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    response = await client.PostAsync(uri, content);

                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        Device.BeginInvokeOnMainThread(() =>
                        {
                            MessagingCenter.Send<object, string>(this, "Sekmingai", DateTime.Now.ToString());
                        });
                    }
                    else
                    {
                        await App.Database.SaveLogAsync(new Logs
                        {
                            deviceno = item.deviceno,
                            phoneno = item.phoneno,
                            direction = item.direction,
                            dt = item.dt,
                            action = item.action
                        });

                    }
                }
            }
            else
            {
                foreach (var item in phoneCalls)
                {
                    await App.Database.SaveLogAsync(new Logs
                    {
                        deviceno = item.deviceno,
                        phoneno = item.phoneno,
                        direction = item.direction,
                        dt = item.dt,
                        action = item.action
                    });
                }
            }
        }
        public async Task UpdatePhonebook()
        {
            if (NetworkCheck.IsInternet())
            {
                var response = await client.GetAsync("http://phone.higsobozonas.lt/phonebook.aspx");
                if (response.IsSuccessStatusCode)
                {
                    string contactsJson = await response.Content.ReadAsStringAsync();
                    var list = JsonConvert.DeserializeObject<List<Repository>>(contactsJson);

                    DeleteContact();
                    ObjContactList = list;
                    foreach (Repository obj in ObjContactList)
                    {
                        CreateContacts(obj.name, obj.phone);
                    }

                    Device.BeginInvokeOnMainThread(() =>
                    {
                        MessagingCenter.Send<object, string>(this, "Atnaujinti", DateTime.Now.ToString());
                    });
                }
            }
            else
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    Toast.MakeText(thisContext, "Ryšio klaida: Nepavyko atnaujinti kontaktų", ToastLength.Long).Show();
                });

            }
        }
        public async Task UploadContactsToServer(List<Logs> logs)
        {
            if (NetworkCheck.IsInternet())
            {
                var uri = new Uri("http://phone.higsobozonas.lt/call.aspx");
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response;

                foreach (var item in logs)
                {
                    PhoneCall phoneCall = new PhoneCall(
                        item.deviceno, item.phoneno, item.direction, item.dt, item.action
                    );
                    string json = JsonConvert.SerializeObject(phoneCall);

                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    response = await client.PostAsync(uri, content);

                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        await App.Database.DeleteLogAsync(item);
                    }
                    else
                    {
                        Device.BeginInvokeOnMainThread(() =>
                        {
                            Toast.MakeText(thisContext, "Serverio klaida: nepavyko sukelti kontaktų", ToastLength.Short).Show();
                        });
                    }
                }
            }
            else
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    Toast.MakeText(thisContext, "Ryšio klaida: nepavyko sukelti kontaktų", ToastLength.Long).Show();
                });

            }
        }
    }
}