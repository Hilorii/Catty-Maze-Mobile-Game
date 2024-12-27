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

        private MovementDirection _lastMovementDirection;
        public enum MovementDirection
        {
            Left,
            Right,
            Up,
            Down
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

                    // SprawdŸ, czy wszystkie monety zosta³y zebrane
                    if (_drawable.CoinsRemaining == 0)
                    {
                        // Jeœli poziom ukoñczony, przejdŸ do ekranu sukcesu
                        ShowLevelCompletePage();
                        return; // Przerwij dalsze przetwarzanie
                    }

                    // Jeœli ruchy siê skoñczy³y, poka¿ ekran pora¿ki
                    if (_drawable.MovesRemaining == 0)
                    {
                        ShowLevelFailedPage();
                        return; // Przerwij dalsze przetwarzanie
                    }

                    // Odœwie¿ UI tylko w przypadku, gdy poziom nie jest ukoñczony
                    GameCanvas.Invalidate();
                    UpdateMovesRemaining();

                    // Aktualizuj liczbê monet tylko, jeœli s¹ jeszcze monety do zebrania
                    if (_drawable.CoinsRemaining > 0)
                    {
                        UpdateCoinsRemaining();
                    }
                }
            }
        }



        private async Task AnimatePlayerMovement(List<(int X, int Y)> path)
        {
            const int framesPerStep = 3; // Liczba klatek na jeden krok

            foreach (var (targetX, targetY) in path)
            {
                float startX = _drawable._animatedX;
                float startY = _drawable._animatedY;

                // animacja przesuniêcia
                for (int frame = 1; frame <= framesPerStep; frame++)
                {
                    float interpolatedX = startX + (targetX - startX) * (frame / (float)framesPerStep);
                    float interpolatedY = startY + (targetY - startY) * (frame / (float)framesPerStep);

                    _drawable.SetAnimatedPosition(interpolatedX, interpolatedY);
                    GameCanvas.Invalidate();

                    await Task.Delay(5); // 16 = ~60 FPS
                }

                // finalne ustawienie pozycji
                _drawable.SetTemporaryPlayerPosition(targetX, targetY);

                // sprawdŸ monety
                if (_drawable.CheckAndCollectCoin(targetX, targetY))
                {
                    Debug.WriteLine($"Moneta zebrana na pozycji: X={targetX}, Y={targetY}");
                    UpdateCoinsRemaining();
                }
            }

            // Po zakoñczeniu ca³ego path:
            // ustaw grafikê "stoj¹c¹" w zale¿noœci od _lastMovementDirection
            switch (_lastMovementDirection)
            {
                case MovementDirection.Left:
                    _drawable.SetPlayerImage("PlayerStandLeft.png");
                    break;
                case MovementDirection.Right:
                    _drawable.SetPlayerImage("PlayerStandRight.png");
                    break;
                case MovementDirection.Up:
                    _drawable.SetPlayerImage("PlayerStandUp.png");
                    break;
                case MovementDirection.Down:
                    // Na dó³ chcesz domyœln¹ Player.png
                    _drawable.SetPlayerImage("Player.png");
                    break;
            }

            GameCanvas.Invalidate(); // odœwie¿ widok
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


        private void OnSwipedLeft(object sender, SwipedEventArgs e)
        {
            Debug.WriteLine("LabyrinthGamePage: Przesuniêcie w lewo");
            _lastMovementDirection = MovementDirection.Left; // zapamiêtujemy kierunek

            // Ustaw grafikê „skoku” w lewo
            _drawable.SetPlayerImage("PlayerJumpLeft.png");
            MovePlayer(-1, 0);
        }

        private void OnSwipedRight(object sender, SwipedEventArgs e)
        {
            Debug.WriteLine("LabyrinthGamePage: Przesuniêcie w prawo");
            _lastMovementDirection = MovementDirection.Right;

            _drawable.SetPlayerImage("PlayerJumpRight.png");
            MovePlayer(1, 0);
        }

        private void OnSwipedUp(object sender, SwipedEventArgs e)
        {
            Debug.WriteLine("LabyrinthGamePage: Przesuniêcie w górê");
            _lastMovementDirection = MovementDirection.Up;

            _drawable.SetPlayerImage("PlayerJumpRight.png");
            MovePlayer(0, -1);
        }

        private void OnSwipedDown(object sender, SwipedEventArgs e)
        {
            Debug.WriteLine("LabyrinthGamePage: Przesuniêcie w dó³");
            _lastMovementDirection = MovementDirection.Down;

            _drawable.SetPlayerImage("PlayerJumpRight.png");
            MovePlayer(0, 1);
        }




    }
}
