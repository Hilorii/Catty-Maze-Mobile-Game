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
            _onNextLevel = onNextLevel
                           ?? throw new ArgumentNullException(nameof(onNextLevel),
                               "Akcja przej�cia do nast�pnego poziomu nie mo�e by� null.");

            _onExitToMenu = onExitToMenu
                            ?? throw new ArgumentNullException(nameof(onExitToMenu),
                                "Akcja powrotu do menu nie mo�e by� null.");

            // Sprawdzamy, czy to ostatni level
            if (currentLevelIndex >= LevelData.AllLevels.Count - 1)
            {
                // Ukrywamy przycisk NEXT
                NextLevelButton.IsVisible = false;
            }
        }

        // Drugi konstruktor - je�eli wolisz zachowa� stary podpis
        // (ale w Twoim kodzie pewnie ju� zawsze passujesz currentLevelIndex)
        public LevelCompletePage(Action onNextLevel, Action onExitToMenu)
            : this(onNextLevel, onExitToMenu, 0)
        {
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