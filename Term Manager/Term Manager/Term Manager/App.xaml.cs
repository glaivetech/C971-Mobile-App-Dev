using System;
using Term_Manager.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Term_Manager
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            HomePage home = new HomePage();
            NavigationPage navPage = new NavigationPage(home);
            home.NavPage = navPage;

            MainPage = navPage;
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
