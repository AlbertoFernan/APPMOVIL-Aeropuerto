using APPMOVIL.Views;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace APPMOVIL
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new NavigationPage( new PrincipalView());
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
