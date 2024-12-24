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

        // �adowanie pierwszego poziomu i aktualizacja etykiety ruch�w
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

                if (_drawable.CoinsRemaining == 0) // Uko�czono poziom
                {
                    DisplayAlert("Gratulacje!", "Uko�czy�e� poziom!", "OK");

                    // Oznaczenie poziomu jako uko�czonego
                    GameState.MarkLevelAsCompleted(_drawable.CurrentLevelIndex);

                    _drawable.LoadNextLevel();
                    UpdateMovesRemaining();
                    UpdateCoinsRemaining();
                }
                else if (_drawable.MovesRemaining == 0) // Koniec ruch�w
                {
                    DisplayAlert("Koniec gry", "Przegra�e�! Spr�buj jeszcze raz.", "OK");
                    _drawable.ResetLevel();
                    UpdateMovesRemaining();
                    UpdateCoinsRemaining();
                }
            }
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
