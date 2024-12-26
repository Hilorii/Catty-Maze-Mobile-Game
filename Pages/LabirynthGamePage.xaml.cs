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
                // Pobieramy bie��ce animowane pozycje (synchronizowane)
                float startX = _drawable._animatedX; // <- U�yj animowanych pozycji
                float startY = _drawable._animatedY;

                for (int frame = 1; frame <= framesPerStep; frame++)
                {
                    // Obliczamy interpolowane warto�ci pozycji
                    float interpolatedX = startX + (targetX - startX) * (frame / (float)framesPerStep);
                    float interpolatedY = startY + (targetY - startY) * (frame / (float)framesPerStep);

                    // Logowanie dla diagnostyki
                    Debug.WriteLine($"Interpolacja: Frame={frame}, X={interpolatedX}, Y={interpolatedY}");

                    // Ustawiamy animowan� pozycj�
                    _drawable.SetAnimatedPosition(interpolatedX, interpolatedY);

                    // Od�wie�amy widok
                    GameCanvas.Invalidate();

                    // Czekamy na kolejn� klatk�
                    await Task.Delay(16); // ~60 FPS
                }

                // Na ko�cu kroku ustawiamy rzeczywist� pozycj�
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
