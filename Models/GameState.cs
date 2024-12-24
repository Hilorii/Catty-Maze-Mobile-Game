using System.Collections.Generic;

namespace MobileApp.Models
{
    public static class GameState
    {
        public static HashSet<int> CompletedLevels { get; } = new HashSet<int>();

        public static void MarkLevelAsCompleted(int levelIndex)
        {
            if (!CompletedLevels.Contains(levelIndex))
                CompletedLevels.Add(levelIndex);
        }

        public static bool IsLevelCompleted(int levelIndex)
        {
            return CompletedLevels.Contains(levelIndex);
        }
    }
}