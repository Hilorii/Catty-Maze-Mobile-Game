using MobileApp.Models;
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
            _drawable.LoadLevel(levelIndex);
            GameCanvas.Invalidate();

            UpdateMovesRemaining();
            UpdateCoinsRemaining();
        }

        private bool _isAnimating = false;

        private async void MovePlayer(int deltaX, int deltaY)
        {
            if (_isAnimating)
            {
                Debug.WriteLine("Ruch jest ju¿ w trakcie, poczekaj na zakoñczenie.");
                return;
            }

            if (_drawable.MovesRemaining > 0)
            {
                var path = _drawable.GetPlayerPath(deltaX, deltaY);
                if (path.Count > 0)
                {
                    _isAnimating = true; // Zablokuj mo¿liwoœæ kolejnego ruchu
                    await AnimatePlayerMovement(path);
                    _isAnimating = false; // Odblokuj mo¿liwoœæ kolejnego ruchu

                    GameCanvas.Invalidate();
                    UpdateMovesRemaining();
                    UpdateCoinsRemaining();

                    if (_drawable.CoinsRemaining == 0)
                    {
                        ShowLevelCompletePage();
                    }
                    else if (_drawable.MovesRemaining == 0)
                    {
                        ShowLevelFailedPage();
                    }
                }
            }
        }

        private async Task AnimatePlayerMovement(List<(int X, int Y)> path)
        {
            const int framesPerStep = 10; // Liczba klatek na jeden krok

            foreach (var (targetX, targetY) in path)
            {
                float startX = _drawable._animatedX;
                float startY = _drawable._animatedY;

                for (int frame = 1; frame <= framesPerStep; frame++)
                {
                    float interpolatedX = startX + (targetX - startX) * (frame / (float)framesPerStep);
                    float interpolatedY = startY + (targetY - startY) * (frame / (float)framesPerStep);

                    _drawable.SetAnimatedPosition(interpolatedX, interpolatedY);
                    GameCanvas.Invalidate();

                    await Task.Delay(16); // ~60 FPS
                }

                _drawable.SetTemporaryPlayerPosition(targetX, targetY);

                // Sprawdzenie i zebranie monety, jeœli gracz na niej stan¹³
                if (_drawable.CheckAndCollectCoin(targetX, targetY))
                {
                    Debug.WriteLine($"Moneta zebrana na pozycji: X={targetX}, Y={targetY}");
                    UpdateCoinsRemaining();
                }
            }
        }

        private void ShowLevelCompletePage()
        {
            Navigation.PushModalAsync(new LevelCompletePage(
                onNextLevel: async () =>
                {
                    _drawable.LoadNextLevel();
                    GameCanvas.Invalidate();
                    UpdateMovesRemaining();
                    UpdateCoinsRemaining();
                    await Navigation.PopModalAsync();
                },
                onExitToMenu: async () =>
                {
                    Application.Current.MainPage = new NavigationPage(new MainMenuPage());
                }
            ));
        }

        private void ShowLevelFailedPage()
        {
            Navigation.PushModalAsync(new LevelFailedPage(
                onRetryLevel: () =>
                {
                    _drawable.ResetLevel();
                    GameCanvas.Invalidate();
                    UpdateMovesRemaining();
                    UpdateCoinsRemaining();
                    Navigation.PopModalAsync();
                },
                onExitToMenu: () =>
                {
                    Application.Current.MainPage = new NavigationPage(new MainMenuPage());
                }
            ));
        }

        private void UpdateMovesRemaining()
        {
            MovesRemainingLabel.Text = $"Pozosta³e ruchy: {_drawable.MovesRemaining}";
        }

        private void UpdateCoinsRemaining()
        {
            MovesRemainingLabel.Text += $"\nPozosta³e monety: {_drawable.CoinsRemaining}";
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


    }
}
