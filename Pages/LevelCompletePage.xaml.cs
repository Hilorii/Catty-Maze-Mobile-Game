using Microsoft.Maui.Controls;
using System.Diagnostics;

namespace MobileApp.Pages
{
    public partial class LevelCompletePage : ContentPage
    {
        private readonly Action _onNextLevel;
        private readonly Action _onExitToMenu;

        public LevelCompletePage(Action onNextLevel, Action onExitToMenu)
        {
            InitializeComponent();

            // Walidacja przekazanych akcji
            _onNextLevel = onNextLevel ?? throw new ArgumentNullException(nameof(onNextLevel), "Akcja przej�cia do nast�pnego poziomu nie mo�e by� null.");
            _onExitToMenu = onExitToMenu ?? throw new ArgumentNullException(nameof(onExitToMenu), "Akcja powrotu do menu nie mo�e by� null.");
        }

        private void OnNextLevelClicked(object sender, EventArgs e)
        {
            Debug.WriteLine("LevelCompletePage: Przycisk 'Przejd� do nast�pnego poziomu' klikni�ty");
            _onNextLevel?.Invoke();
        }

        private void OnExitToMenuClicked(object sender, EventArgs e)
        {
            Debug.WriteLine("LevelCompletePage: Przycisk 'Wyjd� do menu' klikni�ty");
            _onExitToMenu?.Invoke();
        }
    }
}