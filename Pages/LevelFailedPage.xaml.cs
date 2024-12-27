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
                "Akcja powt�rzenia poziomu nie mo�e by� null.");

            _onExitToMenu = onExitToMenu ?? throw new ArgumentNullException(
                nameof(onExitToMenu),
                "Akcja powrotu do menu nie mo�e by� null.");
        }

        private void OnRetryLevelClicked(object sender, EventArgs e)
        {
            Debug.WriteLine("LevelFailedPage: Przycisk 'Powt�rz poziom' klikni�ty");
            _onRetryLevel?.Invoke();
        }

        private void OnExitToMenuClicked(object sender, EventArgs e)
        {
            Debug.WriteLine("LevelFailedPage: Przycisk 'Wyjd� do menu' klikni�ty");
            _onExitToMenu?.Invoke();
        }
    }
}