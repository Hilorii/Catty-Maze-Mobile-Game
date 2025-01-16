using Microsoft.Maui.Controls;
using System;
using System.Diagnostics;
using MobileApp.Models;

namespace MobileApp.Pages
{
    public partial class LevelCompletePage : ContentPage
    {
        private readonly Action _onNextLevel;
        private readonly Action _onExitToMenu;

        public LevelCompletePage(
            Action onNextLevel,
            Action onExitToMenu,
            int currentLevelIndex)
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
            GameState.MarkLevelAsCompleted(currentLevelIndex);
            _onNextLevel = onNextLevel
                           ?? throw new ArgumentNullException(nameof(onNextLevel),
                               "Akcja przejœcia do nastêpnego poziomu nie mo¿e byæ null.");

            _onExitToMenu = onExitToMenu
                            ?? throw new ArgumentNullException(nameof(onExitToMenu),
                                "Akcja powrotu do menu nie mo¿e byæ null.");

            // Sprawdzamy, czy to ostatni level
            if (currentLevelIndex >= LevelData.AllLevels.Count - 1)
            {
                NextLevelButton.IsVisible = false;
            }
        }

        public LevelCompletePage(Action onNextLevel, Action onExitToMenu)
            : this(onNextLevel, onExitToMenu, 0)
        {
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