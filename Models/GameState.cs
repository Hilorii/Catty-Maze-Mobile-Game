using System.Collections.Generic;
using System.Linq;
using Microsoft.Maui.Storage;

namespace MobileApp.Models
{
    public static class GameState
    {
        private const string CompletedLevelsKey = "CompletedLevels";

        // HashSet przechowujący ukończone poziomy w pamięci
        public static HashSet<int> CompletedLevels { get; private set; } = LoadCompletedLevels();

        // Oznacza poziom jako ukończony
        public static void MarkLevelAsCompleted(int levelIndex)
        {
            if (!CompletedLevels.Contains(levelIndex))
            {
                CompletedLevels.Add(levelIndex);
                SaveCompletedLevels();
            }
        }

        // Sprawdza, czy poziom został ukończony
        public static bool IsLevelCompleted(int levelIndex)
        {
            return CompletedLevels.Contains(levelIndex);
        }

        // Metoda do znalezienia pierwszego nieukończonego poziomu
        // Zwraca -1, jeśli wszystkie ukończone.
        public static int GetFirstIncompleteLevel()
        {
            for (int i = 0; i < LevelData.AllLevels.Count; i++)
            {
                if (!IsLevelCompleted(i))
                {
                    return i; // Pierwszy nieukończony
                }
            }
            return -1; // Wszystkie ukończone
        }

        // Ładowanie ukończonych poziomów z Preferences
        private static HashSet<int> LoadCompletedLevels()
        {
            var savedLevels = Preferences.Get(CompletedLevelsKey, string.Empty);
            if (string.IsNullOrEmpty(savedLevels))
                return new HashSet<int>();

            return savedLevels
                .Split(',')
                .Select(int.Parse)
                .ToHashSet();
        }

        // Zapisywanie ukończonych poziomów do Preferences
        private static void SaveCompletedLevels()
        {
            var levelsString = string.Join(",", CompletedLevels);
            Preferences.Set(CompletedLevelsKey, levelsString);
        }
    }
}
