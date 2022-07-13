using Android.Widget;
using HGB.Interface;
using HGB.ViewModel;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HGB
{
    public partial class MainPage : ContentPage
    {
        IContactsHelper contactsHelper = DependencyService.Get<IContactsHelper>();

        public MainPage()
        {
            InitializeComponent();
            BindingContext = new LabelModel();
            deviceNoLabel.Text = contactsHelper.GetIdentifier();
        }
        private async void updateContactsBtn_Clicked(object sender, EventArgs e)
        {
            await contactsHelper.UpdatePhonebook();
        }
    }
}
