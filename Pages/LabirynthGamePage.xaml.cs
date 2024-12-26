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

        private async void MovePlayer(int deltaX, int deltaY)
        {
            if (_drawable.MovesRemaining > 0)
            {
                var path = _drawable.GetPlayerPath(deltaX, deltaY);
                if (path.Count > 0)
                {
                    await AnimatePlayerMovement(path);
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
                // Pobieramy bie¿¹ce animowane pozycje (synchronizowane)
                float startX = _drawable._animatedX; // <- U¿yj animowanych pozycji
                float startY = _drawable._animatedY;

                for (int frame = 1; frame <= framesPerStep; frame++)
                {
                    // Obliczamy interpolowane wartoœci pozycji
                    float interpolatedX = startX + (targetX - startX) * (frame / (float)framesPerStep);
                    float interpolatedY = startY + (targetY - startY) * (frame / (float)framesPerStep);

                    // Logowanie dla diagnostyki
                    Debug.WriteLine($"Interpolacja: Frame={frame}, X={interpolatedX}, Y={interpolatedY}");

                    // Ustawiamy animowan¹ pozycjê
                    _drawable.SetAnimatedPosition(interpolatedX, interpolatedY);

                    // Odœwie¿amy widok
                    GameCanvas.Invalidate();

                    // Czekamy na kolejn¹ klatkê
                    await Task.Delay(16); // ~60 FPS
                }

                // Na koñcu kroku ustawiamy rzeczywist¹ pozycjê
                _drawable.SetTemporaryPlayerPosition(targetX, targetY);
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
