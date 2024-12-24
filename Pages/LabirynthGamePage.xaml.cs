using MobileApp.Models;

namespace MobileApp.Pages;

public partial class LabyrinthGamePage : ContentPage
{
    private LabyrinthDrawable _drawable;

    public LabyrinthGamePage()
    {
        InitializeComponent();

        _drawable = new LabyrinthDrawable();
        GameCanvas.Drawable = _drawable;

        // £adowanie pierwszego poziomu i aktualizacja etykiety ruchów
        _drawable.LoadLevel();
        UpdateMovesRemaining();
        UpdateCoinsRemaining();
    }

    public void SetLevel(int levelIndex)
    {
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
                GameCanvas.Invalidate();
                UpdateMovesRemaining();
                UpdateCoinsRemaining();

                if (_drawable.CoinsRemaining == 0) // Ukoñczono poziom
                {
                    DisplayAlert("Gratulacje!", "Ukoñczy³eœ poziom!", "OK");

                    // Oznaczenie poziomu jako ukoñczonego
                    GameState.MarkLevelAsCompleted(_drawable.CurrentLevelIndex);

                    _drawable.LoadNextLevel();
                    UpdateMovesRemaining();
                    UpdateCoinsRemaining();
                }
                else if (_drawable.MovesRemaining == 0) // Koniec ruchów
                {
                    DisplayAlert("Koniec gry", "Przegra³eœ! Spróbuj jeszcze raz.", "OK");
                    _drawable.ResetLevel();
                    UpdateMovesRemaining();
                    UpdateCoinsRemaining();
                }
            }
        }
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
