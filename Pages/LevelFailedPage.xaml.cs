using Microsoft.Maui.Controls;
using System;
using System.Diagnostics;

namespace MobileApp.Pages
{
    public partial class LevelFailedPage : ContentPage
    {
        private readonly Action _onRetryLevel;
        private readonly Action _onExitToMenu;

        public LevelFailedPage(Action onRetryLevel, Action onExitToMenu)
        {
            InitializeComponent();

            _onRetryLevel = onRetryLevel ?? throw new ArgumentNullException(
                nameof(onRetryLevel),
                "Akcja powtórzenia poziomu nie mo¿e byæ null.");

            _onExitToMenu = onExitToMenu ?? throw new ArgumentNullException(
                nameof(onExitToMenu),
                "Akcja powrotu do menu nie mo¿e byæ null.");
        }

        private void OnRetryLevelClicked(object sender, EventArgs e)
        {
            Debug.WriteLine("LevelFailedPage: Przycisk 'Powtórz poziom' klikniêty");
            _onRetryLevel?.Invoke();
        }

        private void OnExitToMenuClicked(object sender, EventArgs e)
        {
            Debug.WriteLine("LevelFailedPage: Przycisk 'WyjdŸ do menu' klikniêty");
            _onExitToMenu?.Invoke();
        }
    }
}