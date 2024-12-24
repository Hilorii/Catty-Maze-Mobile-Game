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

        private void OnMoveUp(object sender, EventArgs e) => MovePlayer(0, -1);
        private void OnMoveDown(object sender, EventArgs e) => MovePlayer(0, 1);
        private void OnMoveLeft(object sender, EventArgs e) => MovePlayer(-1, 0);
        private void OnMoveRight(object sender, EventArgs e) => MovePlayer(1, 0);

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
                        DisplayAlert("Koniec gry", "Przegra�e�! Spr�buj jeszcze raz.", "OK");
                        Debug.WriteLine("LabyrinthGamePage: Koniec ruch�w, resetowanie poziomu");
                        _drawable.ResetLevel();
                        UpdateMovesRemaining();
                        UpdateCoinsRemaining();
                    }
                }
            }
        }

        private void ShowLevelCompletePage()
        {
            try
            {
                Debug.WriteLine("LabyrinthGamePage: Wy�wietlanie strony uko�czenia poziomu...");
                Navigation.PushModalAsync(new LevelCompletePage(
                    onNextLevel: async () =>
                    {
                        Debug.WriteLine("LevelCompletePage: Przechodzenie do nast�pnego poziomu...");
                        _drawable.LoadNextLevel();
                        Debug.WriteLine("LabyrinthDrawable: Nast�pny poziom za�adowany.");
                        GameCanvas.Invalidate(); // Od�wie� widok labiryntu
                        UpdateMovesRemaining();
                        UpdateCoinsRemaining();
                        await Navigation.PopModalAsync(); // Zamknij modalny ekran
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
