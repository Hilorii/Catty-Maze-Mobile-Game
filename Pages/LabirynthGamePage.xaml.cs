using MobileApp.Models;
using System.Diagnostics;
using static MobileApp.Models.LabyrinthDrawable;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace MobileApp.Pages
{
    public partial class LabyrinthGamePage : ContentPage
    {
        private LabyrinthDrawable _drawable;
        private bool _isAnimating = false;

        public LabyrinthGamePage()
        {
            InitializeComponent();
            Debug.WriteLine("LabyrinthGamePage: Konstruktor wywo³any");

            _drawable = new LabyrinthDrawable();
            GameCanvas.Drawable = _drawable;

            _drawable.LoadLevel();
            UpdateMovesRemaining();
            UpdateCoinsRemaining();

            // Timer do animacji IDLE
            Device.StartTimer(TimeSpan.FromMilliseconds(500), () =>
            {
                if (!_drawable.IsJumping)
                {
                    _drawable.NextIdleFrame();
                    GameCanvas.Invalidate();
                }
                return true;
            });
        }

        public void SetLevel(int levelIndex)
        {
            Debug.WriteLine($"LabyrinthGamePage: Ustawianie poziomu {levelIndex}");
            _drawable.LoadLevel(levelIndex);
            GameCanvas.Invalidate();

            UpdateMovesRemaining();
            UpdateCoinsRemaining();
        }

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
                    _isAnimating = true; // Zablokuj kolejny ruch
                    await AnimatePlayerMovement(path);
                    _isAnimating = false; // Odblokuj

                    // SprawdŸ, czy wszystkie monety zebrane
                    if (_drawable.CoinsRemaining == 0)
                    {
                        ShowLevelCompletePage();
                        return;
                    }
                    // SprawdŸ, czy skoñczy³y siê ruchy
                    if (_drawable.MovesRemaining == 0)
                    {
                        ShowLevelFailedPage();
                        return;
                    }

                    // Odœwie¿
                    GameCanvas.Invalidate();
                    UpdateMovesRemaining();
                    if (_drawable.CoinsRemaining > 0)
                        UpdateCoinsRemaining();
                }
            }
        }

        private async Task AnimatePlayerMovement(List<(int X, int Y)> path)
        {
            const int framesPerStep = 3;

            foreach (var (targetX, targetY) in path)
            {
                float startX = _drawable.AnimatedX;
                float startY = _drawable.AnimatedY;

                for (int frame = 1; frame <= framesPerStep; frame++)
                {
                    float t = frame / (float)framesPerStep;
                    float interpolatedX = startX + (targetX - startX) * t;
                    float interpolatedY = startY + (targetY - startY) * t;

                    _drawable.SetAnimatedPosition(interpolatedX, interpolatedY);
                    GameCanvas.Invalidate();

                    await Task.Delay(5);
                }

                // Ustaw finaln¹ pozycjê
                _drawable.SetTemporaryPlayerPosition(targetX, targetY);

                // SprawdŸ monety
                if (_drawable.CheckAndCollectCoin(targetX, targetY))
                {
                    Debug.WriteLine($"Moneta zebrana na pozycji: X={targetX}, Y={targetY}");
                    UpdateCoinsRemaining();
                }
            }

            // Po zakoñczeniu ruchu: wracamy do IDLE
            _drawable.IsJumping = false;
            GameCanvas.Invalidate();
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
            if (_drawable.CoinsRemaining == 0)
            {
                CoinsRemainingLabel.IsVisible = false;
            }
            else
            {
                CoinsRemainingLabel.IsVisible = true;
                CoinsRemainingLabel.Text = $"Pozosta³e monety: {_drawable.CoinsRemaining}";
            }
        }

        // --- GESTY ---

        private void OnSwipedLeft(object sender, SwipedEventArgs e)
        {
            // Naprawia bug z mozliwoscia obrotu graczem w trakcie skoku!
            if (_drawable.IsJumping)
            {
                return;
            }

            Debug.WriteLine("LabyrinthGamePage: Przesuniêcie w lewo");
            _drawable.LastDirection = MovementDirection.Left;

            _drawable.IsJumping = true;
            _drawable.SetPlayerImage("PlayerJumpLeft.png");

            MovePlayer(-1, 0);
        }

        private void OnSwipedRight(object sender, SwipedEventArgs e)
        {
            if (_drawable.IsJumping)
            {
                return;
            }
            Debug.WriteLine("LabyrinthGamePage: Przesuniêcie w prawo");
            _drawable.LastDirection = MovementDirection.Right;

            _drawable.IsJumping = true;
            _drawable.SetPlayerImage("PlayerJumpRight.png");

            MovePlayer(1, 0);
        }

        private void OnSwipedUp(object sender, SwipedEventArgs e)
        {
            if (_drawable.IsJumping)
            {
                return;
            }
            Debug.WriteLine("LabyrinthGamePage: Przesuniêcie w górê");
            _drawable.LastDirection = MovementDirection.Up;

            _drawable.IsJumping = true;
            // Za³ó¿my, ¿e skok do góry u¿ywa tego samego sprite’a co w prawo
            // (lub stwórz PlayerJumpUp.png, jeœli masz)
            _drawable.SetPlayerImage("PlayerJumpRight.png");

            MovePlayer(0, -1);
        }

        private void OnSwipedDown(object sender, SwipedEventArgs e)
        {
            if (_drawable.IsJumping)
            {
                return;
            }
            Debug.WriteLine("LabyrinthGamePage: Przesuniêcie w dó³");
            _drawable.LastDirection = MovementDirection.Down;

            _drawable.IsJumping = true;
            // Równie¿ u¿ywamy PlayerJumpRight.png? Lub stwórz PlayerJumpDown.png
            _drawable.SetPlayerImage("PlayerJumpRight.png");

            MovePlayer(0, 1);
        }
    }
}
