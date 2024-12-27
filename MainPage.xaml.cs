using Microsoft.Maui.Controls;
using System;

namespace MobileApp.Pages
{
    public partial class MainMenuPage : ContentPage
    {
        public MainMenuPage()
        {
            InitializeComponent();
        }

        private async void OnStartGame(object sender, EventArgs e)
        {
            // Nawigacja do strony z grą:
            await Navigation.PushAsync(new LabyrinthGamePage());
        }

        private async void OnSelectLevel(object sender, EventArgs e)
        {
            // Nawigacja do strony z wyborem poziomu:
            await Navigation.PushAsync(new LevelSelectionPage());
        }

        private void OnQuitApp(object sender, EventArgs e)
        {
            // Wyjście z aplikacji – najprostszy cross-platform (choć nieidealny)
            Environment.Exit(0);

            // UWAGA: Nie ma w MAUI oficjalnej funkcji "Application.Quit()".
            // Na Androidzie można ewentualnie zabić proces:
            //   System.Diagnostics.Process.GetCurrentProcess().Kill();
            // Ale Environment.Exit(0) zwykle wystarczy w prostych projektach.
        }
    }
}