using MobileApp.Models;
using MobileApp.Pages;
using System.Diagnostics;

namespace MobileApp.Pages
{
    public partial class LabyrinthGamePage : ContentPage
    {
        private LabyrinthDrawable _drawable;

        public LabyrinthGamePage()
        {
            InitializeComponent();
            Debug.WriteLine("LabyrinthGamePage: Konstruktor wywo³any");

            _drawable = new LabyrinthDrawable();
            GameCanvas.Drawable = _drawable;

            _drawable.LoadLevel();
            UpdateMovesRemaining();
            UpdateCoinsRemaining();
        }

        public void SetLevel(int levelIndex)
        {
            Debug.WriteLine($"LabyrinthGamePage: Ustawianie poziomu {levelIndex}");
            _drawable = new LabyrinthDrawable();
            _drawable.LoadLevel(levelIndex);
            GameCanvas.Drawable = _drawable;

            UpdateMovesRemaining();
            UpdateCoinsRemaining();
        }

        private void MovePlayer(int deltaX, int deltaY)
        {
            if (_drawable.MovesRemaining > 0)
            {
                bool success = _drawable.MovePlayer(deltaX, deltaY);
                if (success)
                {
                    Debug.WriteLine($"LabyrinthGamePage: Gracz poruszy³ siê w kierunku deltaX={deltaX}, deltaY={deltaY}");
                    GameCanvas.Invalidate();
                    UpdateMovesRemaining();
                    UpdateCoinsRemaining();

                    if (_drawable.CoinsRemaining == 0) // Ukoñczono poziom
                    {
                        Debug.WriteLine("LabyrinthGamePage: Ukoñczono poziom");
                        ShowLevelCompletePage();
                    }
                    else if (_drawable.MovesRemaining == 0) // Koniec ruchów
                    {
                        Debug.WriteLine("LabyrinthGamePage: Koniec ruchów, wyœwietlanie strony przegranej");
                        ShowLevelFailedPage(); // Wywo³anie nowej strony po przegranej
                    }
                }
            }
        }

        private void ShowLevelCompletePage()
        {
            try
            {
                GameState.MarkLevelAsCompleted(_drawable.CurrentLevelIndex);

                Debug.WriteLine("LabyrinthGamePage: Wyœwietlanie strony ukoñczenia poziomu...");
                Navigation.PushModalAsync(new LevelCompletePage(
                    onNextLevel: async () =>
                    {
                        Debug.WriteLine("LevelCompletePage: Przechodzenie do nastêpnego poziomu...");
                        _drawable.LoadNextLevel();
                        Debug.WriteLine("LabyrinthDrawable: Nastêpny poziom za³adowany.");
                        GameCanvas.Invalidate();
                        UpdateMovesRemaining();
                        UpdateCoinsRemaining();
                        await Navigation.PopModalAsync();
                    },

                    onExitToMenu: async () =>
                    {
                        Debug.WriteLine("LevelCompletePage: Powrót do menu g³ównego...");
                        Application.Current.MainPage = new NavigationPage(new MainMenuPage());
                    }

                ));
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"LabyrinthGamePage: B³¹d podczas otwierania strony: {ex.Message}");
            }
        }

        private void ShowLevelFailedPage()
        {
            try
            {
                Debug.WriteLine("LabyrinthGamePage: Wyœwietlanie strony przegranej...");
                Navigation.PushModalAsync(new LevelFailedPage(
                    onRetryLevel: () =>
                    {
                        Debug.WriteLine("LevelFailedPage: Powtórzenie poziomu...");
                        _drawable.ResetLevel();
                        GameCanvas.Invalidate();
                        UpdateMovesRemaining();
                        UpdateCoinsRemaining();
                        Navigation.PopModalAsync();
                    },

                    onExitToMenu: () =>
                    {
                        Debug.WriteLine("LevelFailedPage: Powrót do menu g³ównego...");
                        Application.Current.MainPage = new NavigationPage(new MainMenuPage());
                    }
                ));
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"LabyrinthGamePage: B³¹d podczas otwierania strony przegranej: {ex.Message}");
            }
        }

        private void OnSwipedLeft(object sender, SwipedEventArgs e)
        {
            Debug.WriteLine("LabyrinthGamePage: Przesuniêcie w lewo");
            MovePlayer(-1, 0);
        }

        private void OnSwipedRight(object sender, SwipedEventArgs e)
        {
            Debug.WriteLine("LabyrinthGamePage: Przesuniêcie w prawo");
            MovePlayer(1, 0);
        }

        private void OnSwipedUp(object sender, SwipedEventArgs e)
        {
            Debug.WriteLine("LabyrinthGamePage: Przesuniêcie w górê");
            MovePlayer(0, -1);
        }

        private void OnSwipedDown(object sender, SwipedEventArgs e)
        {
            Debug.WriteLine("LabyrinthGamePage: Przesuniêcie w dó³");
            MovePlayer(0, 1);
        }

        private void UpdateMovesRemaining()
        {
            MovesRemainingLabel.Text = $"Pozosta³e ruchy: {_drawable.MovesRemaining}";
        }

        private void UpdateCoinsRemaining()
        {
            MovesRemainingLabel.Text += $"\nPozosta³e monety: {_drawable.CoinsRemaining}";
        }
    }
}
