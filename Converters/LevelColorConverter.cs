using System;
using System.Diagnostics;
using System.Globalization;
using Microsoft.Maui.Controls;
using MobileApp.Models;

namespace MobileApp.Converters
{
    public class LevelColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                Debug.WriteLine("LevelColorConverter: Otrzymano wartość null");
                return Colors.Black; // Domyślny kolor, jeśli wartość jest null
            }

            if (value is not int levelIndex || levelIndex <= 0)
            {
                Debug.WriteLine($"LevelColorConverter: Nieprawidłowa wartość indeksu poziomu: {value}");
                return Colors.Black; // Domyślny kolor dla nieprawidłowej wartości
            }

            levelIndex -= 1; // Dostosowanie poziomu do indeksu zero-based

            // Sprawdzenie, czy poziom został ukończony
            bool isCompleted = GameState.IsLevelCompleted(levelIndex);
            Debug.WriteLine($"LevelColorConverter: Poziom {levelIndex} ukończony: {isCompleted}");

            return isCompleted ? Colors.Gray : Colors.Black;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}