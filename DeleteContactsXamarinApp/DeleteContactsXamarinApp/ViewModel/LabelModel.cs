using HGB.Interface;
using HGB.ViewModel;
using System;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace HGB.ViewModel
{
    public class LabelModel : BindableObject
    {
        string dataSentSuccessful = "Paskutinį kartą pavyko išsiųsti duomenis sėkmingai: ";
        string dataSentTried = "Paskutinį kartą bandyta išsiųsti duomenis: ";
        string lastTimeContactsUpdated = "Paskutinį kartą kontaktai buvo atnaujinti: ";

        public LabelModel()
        {
            MessagingCenter.Subscribe<object, string>(this, "Atnaujinti", (sender, args) =>
            {
                LastTimeContactsUpdated = $"Paskutinį kartą kontaktai buvo atnaujinti: \n {args}";
            });
            MessagingCenter.Subscribe<object, string>(this, "Sekmingai", (sender, args) =>
            {
                DataSentSuccessful = $"Paskutinį kartą pavyko išsiųsti duomenis sėkmingai: \n {args}";
            });
            MessagingCenter.Subscribe<object, string>(this, "Bandyta", (sender, args) =>
            {
                DataSentTried = $"Paskutinį kartą bandyta išsiųsti duomenis: \n {args}";
            });
        }

        public string DataSentTried
        {
            get => Preferences.Get(nameof(DataSentTried), dataSentTried);
            set
            {
                Preferences.Set(nameof(DataSentTried), value);
                OnPropertyChanged(nameof(DataSentTried));
            }
        }
        public string DataSentSuccessful
        {
            get => Preferences.Get(nameof(DataSentSuccessful), dataSentSuccessful);
            set
            {
                Preferences.Set(nameof(DataSentSuccessful), value);
                OnPropertyChanged(nameof(DataSentSuccessful));
            }
        }
        public string LastTimeContactsUpdated
        {
            get => Preferences.Get(nameof(LastTimeContactsUpdated), lastTimeContactsUpdated);
            set
            {
                Preferences.Set(nameof(LastTimeContactsUpdated), value);
                OnPropertyChanged(nameof(LastTimeContactsUpdated));
            }
        }
    }
}
