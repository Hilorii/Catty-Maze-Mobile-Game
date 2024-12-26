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
                Debug.WriteLine("Ruch jest ju� w trakcie, poczekaj na zako�czenie.");
                return;
            }

            if (_drawable.MovesRemaining > 0)
            {
                var path = _drawable.GetPlayerPath(deltaX, deltaY);
                if (path.Count > 0)
                {
                    _isAnimating = true; // Zablokuj mo�liwo�� kolejnego ruchu
                    await AnimatePlayerMovement(path);
                    _isAnimating = false; // Odblokuj mo�liwo�� kolejnego ruchu

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

                // Sprawdzenie i zebranie monety, je�li gracz na niej stan��
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
            MovesRemainingLabel.Text = $"Pozosta�e ruchy: {_drawable.MovesRemaining}";
        }

        private void UpdateCoinsRemaining()
        {
            MovesRemainingLabel.Text += $"\nPozosta�e monety: {_drawable.CoinsRemaining}";
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


    }
}
