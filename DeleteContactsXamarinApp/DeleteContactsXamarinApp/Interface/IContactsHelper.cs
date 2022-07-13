using HGB.Model;
using HGB.ViewModel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HGB.Interface
{
    public interface IContactsHelper
    {
        void DeleteContact();
        void CreateContacts(string name, string phone);
        string GetIdentifier();
        Task SendData(List<PhoneCall> phoneCall);
        Task UpdatePhonebook();
        Task UploadContactsToServer(List<Logs> logs);
    }
}
