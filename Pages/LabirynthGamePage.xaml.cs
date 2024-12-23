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

                if (_drawable.IsGoalReached)
                {
                    DisplayAlert("Gratulacje!", "Ukoñczy³eœ poziom!", "OK");
                    _drawable.LoadNextLevel();
                    UpdateMovesRemaining();
                }
                else if (_drawable.MovesRemaining == 0)
                {
                    DisplayAlert("Koniec gry", "Przegra³eœ! Spróbuj jeszcze raz.", "OK");
                    _drawable.ResetLevel();
                    UpdateMovesRemaining();
                }
            }
        }
    }

    private void UpdateMovesRemaining()
    {
        MovesRemainingLabel.Text = $"Pozosta³e ruchy: {_drawable.MovesRemaining}";
    }
}