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
            Debug.WriteLine("LabyrinthGamePage: Konstruktor wywo�any");

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
                    Debug.WriteLine($"LabyrinthGamePage: Gracz poruszy� si� w kierunku deltaX={deltaX}, deltaY={deltaY}");
                    GameCanvas.Invalidate();
                    UpdateMovesRemaining();
                    UpdateCoinsRemaining();

                    if (_drawable.CoinsRemaining == 0) // Uko�czono poziom
                    {
                        Debug.WriteLine("LabyrinthGamePage: Uko�czono poziom");
                        ShowLevelCompletePage();
                    }
                    else if (_drawable.MovesRemaining == 0) // Koniec ruch�w
                    {
                        Debug.WriteLine("LabyrinthGamePage: Koniec ruch�w, wy�wietlanie strony przegranej");
                        ShowLevelFailedPage(); // Wywo�anie nowej strony po przegranej
                    }
                }
            }
        }

        private void ShowLevelCompletePage()
        {
            try
            {
                GameState.MarkLevelAsCompleted(_drawable.CurrentLevelIndex);

                Debug.WriteLine("LabyrinthGamePage: Wy�wietlanie strony uko�czenia poziomu...");
                Navigation.PushModalAsync(new LevelCompletePage(
                    onNextLevel: async () =>
                    {
                        Debug.WriteLine("LevelCompletePage: Przechodzenie do nast�pnego poziomu...");
                        _drawable.LoadNextLevel();
                        Debug.WriteLine("LabyrinthDrawable: Nast�pny poziom za�adowany.");
                        GameCanvas.Invalidate();
                        UpdateMovesRemaining();
                        UpdateCoinsRemaining();
                        await Navigation.PopModalAsync();
                    },

                    onExitToMenu: async () =>
                    {
                        Debug.WriteLine("LevelCompletePage: Powr�t do menu g��wnego...");
                        Application.Current.MainPage = new NavigationPage(new MainMenuPage());
                    }

                ));
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"LabyrinthGamePage: B��d podczas otwierania strony: {ex.Message}");
            }
        }

        private void ShowLevelFailedPage()
        {
            try
            {
                Debug.WriteLine("LabyrinthGamePage: Wy�wietlanie strony przegranej...");
                Navigation.PushModalAsync(new LevelFailedPage(
                    onRetryLevel: () =>
                    {
                        Debug.WriteLine("LevelFailedPage: Powt�rzenie poziomu...");
                        _drawable.ResetLevel();
                        GameCanvas.Invalidate();
                        UpdateMovesRemaining();
                        UpdateCoinsRemaining();
                        Navigation.PopModalAsync();
                    },

                    onExitToMenu: () =>
                    {
                        Debug.WriteLine("LevelFailedPage: Powr�t do menu g��wnego...");
                        Application.Current.MainPage = new NavigationPage(new MainMenuPage());
                    }
                ));
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"LabyrinthGamePage: B��d podczas otwierania strony przegranej: {ex.Message}");
            }
        }

        private void OnSwipedLeft(object sender, SwipedEventArgs e)
        {
            Debug.WriteLine("LabyrinthGamePage: Przesuni�cie w lewo");
            MovePlayer(-1, 0);
        }

        private void OnSwipedRight(object sender, SwipedEventArgs e)
        {
            Debug.WriteLine("LabyrinthGamePage: Przesuni�cie w prawo");
            MovePlayer(1, 0);
        }

        private void OnSwipedUp(object sender, SwipedEventArgs e)
        {
            Debug.WriteLine("LabyrinthGamePage: Przesuni�cie w g�r�");
            MovePlayer(0, -1);
        }

        private void OnSwipedDown(object sender, SwipedEventArgs e)
        {
            Debug.WriteLine("LabyrinthGamePage: Przesuni�cie w d�");
            MovePlayer(0, 1);
        }

        private void UpdateMovesRemaining()
        {
            MovesRemainingLabel.Text = $"Pozosta�e ruchy: {_drawable.MovesRemaining}";
        }

        private void UpdateCoinsRemaining()
        {
            MovesRemainingLabel.Text += $"\nPozosta�e monety: {_drawable.CoinsRemaining}";
        }
    }
}
