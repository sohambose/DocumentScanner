using DocumentScanner.Views;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DocumentScanner
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new NavigationPage(new DocumentLandingPage());
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
