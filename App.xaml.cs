using MobileApp.Pages;

namespace MobileApp
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            // Ustawiamy stronę z grą jako startową
            MainPage = new LabyrinthGamePage();
        }
    }
}