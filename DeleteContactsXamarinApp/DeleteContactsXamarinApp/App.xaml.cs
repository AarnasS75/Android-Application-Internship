using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using HGB.DataFolder;
using System.IO;

namespace HGB
{
    public partial class App : Application
    {
        static LogDatabase database;

        public static LogDatabase Database
        {
            get
            {
                if (database == null)
                {
                    database = new LogDatabase(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "CallLogs.db3"));
                }
                return database;
            }
        }

        public App()
        {
            InitializeComponent();
            MainPage = new MainPage();
        }
    }
}
