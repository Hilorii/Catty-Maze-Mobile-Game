using MobileApp.Models;

namespace MobileApp.Pages;

public partial class LabyrinthGamePage : ContentPage
{
    private LabyrinthDrawable _drawable;
    private int _remainingMoves = 10; // Liczba ruchów na poziom

    public LabyrinthGamePage()
    {
        InitializeComponent();

        _drawable = new LabyrinthDrawable();
        GameCanvas.Drawable = _drawable;
    }

    private void OnMoveUp(object sender, EventArgs e) => MovePlayer(0, -1);
    private void OnMoveDown(object sender, EventArgs e) => MovePlayer(0, 1);
    private void OnMoveLeft(object sender, EventArgs e) => MovePlayer(-1, 0);
    private void OnMoveRight(object sender, EventArgs e) => MovePlayer(1, 0);

    private void MovePlayer(int deltaX, int deltaY)
    {
        if (_remainingMoves > 0)
        {
            bool success = _drawable.MovePlayer(deltaX, deltaY);
            if (success)
            {
                _remainingMoves--;
                GameCanvas.Invalidate();

                if (_drawable.IsGoalReached)
                {
                    DisplayAlert("Gratulacje!", "Ukoñczy³eœ poziom!", "OK");
                    _drawable.LoadNextLevel();
                    _remainingMoves = 10; // Reset ruchów na nowy poziom
                }
                else if (_remainingMoves == 0)
                {
                    DisplayAlert("Koniec gry", "Przegra³eœ! Spróbuj jeszcze raz.", "OK");
                    _drawable.ResetLevel();
                    _remainingMoves = 10; // Reset poziomu
                }
            }
        }
    }
}