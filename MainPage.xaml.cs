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
    }
}