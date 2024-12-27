using MobileApp.Pages;

namespace MobileApp
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            MainPage = new NavigationPage(new MainMenuPage())
            {
                BarBackgroundColor = Colors.Transparent, 
                BarTextColor = Colors.Transparent      
            };
            NavigationPage.SetHasNavigationBar(MainPage, false);
        }
    }


}