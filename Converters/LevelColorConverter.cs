using System;
using System.Globalization;
using Microsoft.Maui.Controls;
using MobileApp.Models;

namespace MobileApp.Converters
{
    public class LevelColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Sprawdzenie, czy `value` nie jest null
            if (value == null)
            {
                return Colors.Black; // Domyślny kolor, jeśli wartość jest null
            }

            if (value is not int levelIndex || levelIndex <= 0)
            {
                return Colors.Black; // Domyślny kolor, jeśli wartość jest nieprawidłowa
            }

            levelIndex -= 1; // Konwertuj na indeks (zakładamy, że poziomy zaczynają się od 1)

            // Sprawdzenie, czy GameState jest poprawnie skonfigurowany
            if (GameState.IsLevelCompleted(levelIndex))
            {
                return Colors.Gray;
            }

            return Colors.Black;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}