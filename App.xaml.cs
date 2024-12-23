namespace MobileApp
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent(); // Możesz to usunąć, jeśli całkowicie rezygnujesz z XAML
            MainPage = new MainPage(); // Ustaw główną stronę aplikacji
        }
    }

}
