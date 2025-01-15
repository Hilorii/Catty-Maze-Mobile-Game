using Microsoft.Maui.Controls;
using System;
using MobileApp.Models; // <-- Żeby mieć dostęp do GameState i LevelData
using Microsoft.Maui.Storage; // <-- Do Preferences

namespace MobileApp.Pages
{
    public partial class MainMenuPage : ContentPage
    {
        private const string SoundPreferenceKey = "IsSoundEnabled";
        private static MainMenuPage? _instance;

        public MainMenuPage()
        {
            _instance = this;
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            // Ustawienie grafiki przycisku dźwięku w zależności od preferencji
            bool isSoundEnabled = Preferences.Get(SoundPreferenceKey, true); // Domyślnie włączony
            UpdateSoundButton(isSoundEnabled);

            // Włączenie muzyki
            PlayMusic();
        }

        public static void PlayMusic()
        {
            _instance?.Music.Play();
        }

        public static void StopMusic()
        {
            _instance?.Music.Stop();
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

        private void OnToggleSound(object sender, EventArgs e)
        {
            // Pobierz aktualny stan dźwięku
            bool isSoundEnabled = Preferences.Get(SoundPreferenceKey, true);

            // Przełącz stan
            isSoundEnabled = !isSoundEnabled;

            // Zapisz nowy stan
            Preferences.Set(SoundPreferenceKey, isSoundEnabled);

            // Zaktualizuj grafikę przycisku
            UpdateSoundButton(isSoundEnabled);
        }

        private void UpdateSoundButton(bool isSoundEnabled)
        {
            SoundToggleButton.Source = isSoundEnabled ? "musicon.png" : "musicoff.png";

            // Włączanie/wyłączanie dźwięku
            Music.ShouldMute = !isSoundEnabled;
        }
    }
}
