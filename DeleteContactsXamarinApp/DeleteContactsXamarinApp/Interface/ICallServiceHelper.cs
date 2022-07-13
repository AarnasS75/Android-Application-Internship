using HGB.Model;
using HGB.ViewModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace HGB.Interface
{
    public interface ICallServiceHelper
    {
        void StartMyService();
        void StopMyService();
        void UnsubscribeMessages();
        bool IsMyServiceRunning();
    }
}
