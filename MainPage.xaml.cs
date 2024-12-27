using Microsoft.Maui.Controls;
using System;
using MobileApp.Models; // <-- żeby mieć dostęp do GameState i LevelData

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
            // 1. Znajdź pierwszy nieukończony level
            int firstIncomplete = GameState.GetFirstIncompleteLevel();

            int levelToStart;
            if (firstIncomplete == -1)
            {
                // Wszystkie ukończone, więc startujemy ostatni
                levelToStart = LevelData.AllLevels.Count - 1;
            }
            else
            {
                levelToStart = firstIncomplete;
            }

            // 2. Tworzymy stronę z grą i ustawiamy poziom:
            var labyrinthGamePage = new LabyrinthGamePage();
            labyrinthGamePage.SetLevel(levelToStart);

            // 3. Nawigacja
            await Navigation.PushAsync(labyrinthGamePage);
        }

        private async void OnSelectLevel(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new LevelSelectionPage());
        }

        private void OnQuitApp(object sender, EventArgs e)
        {
            // Najprostsze wyjście z aplikacji
            Environment.Exit(0);
        }
    }
}