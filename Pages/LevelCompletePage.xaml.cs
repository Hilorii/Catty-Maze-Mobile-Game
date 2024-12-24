using Microsoft.Maui.Controls;
using System.Diagnostics;

namespace MobileApp.Pages
{
    public partial class LevelCompletePage : ContentPage
    {
        private Action _onNextLevel;
        private Action _onExitToMenu;

        public LevelCompletePage(Action onNextLevel, Action onExitToMenu)
        {
            InitializeComponent();

            if (onNextLevel == null || onExitToMenu == null)
            {
                throw new ArgumentNullException("Przekazane akcje nie mog¹ byæ null");
            }

            _onNextLevel = onNextLevel;
            _onExitToMenu = onExitToMenu;
        }

        private void OnNextLevelClicked(object sender, EventArgs e)
        {
            Debug.WriteLine("Przycisk 'PrzejdŸ do nastêpnego poziomu' klikniêty");
            _onNextLevel?.Invoke();
        }

        private void OnExitToMenuClicked(object sender, EventArgs e)
        {
            Debug.WriteLine("Przycisk 'WyjdŸ do menu' klikniêty");
            _onExitToMenu?.Invoke();
        }
    }
}