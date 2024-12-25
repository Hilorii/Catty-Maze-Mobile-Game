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
            _onNextLevel = onNextLevel ?? throw new ArgumentNullException(nameof(onNextLevel), "Akcja przejœcia do nastêpnego poziomu nie mo¿e byæ null.");
            _onExitToMenu = onExitToMenu ?? throw new ArgumentNullException(nameof(onExitToMenu), "Akcja powrotu do menu nie mo¿e byæ null.");
        }

        private void OnNextLevelClicked(object sender, EventArgs e)
        {
            Debug.WriteLine("LevelCompletePage: Przycisk 'PrzejdŸ do nastêpnego poziomu' klikniêty");
            _onNextLevel?.Invoke();
        }

        private void OnExitToMenuClicked(object sender, EventArgs e)
        {
            Debug.WriteLine("LevelCompletePage: Przycisk 'WyjdŸ do menu' klikniêty");
            _onExitToMenu?.Invoke();
        }
    }
}