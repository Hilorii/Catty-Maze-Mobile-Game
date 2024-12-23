using System.Collections.Generic;

namespace MobileApp.Models
{
    public static class LevelData
    {
        public static List<Level> AllLevels { get; } = new List<Level>
        {
            new Level
            {
                Map = new int[,]
                {
                    { 1, 1, 1, 1, 1 },
                    { 1, 2, 0, 0, 1 },
                    { 1, 0, 1, 0, 1 },
                    { 1, 0, 1, 3, 1 },
                    { 1, 1, 1, 1, 1 }
                },
                Moves = 10
            },
            new Level
            {
                Map = new int[,]
                {
                    { 1, 1, 1, 1, 1, 1 },
                    { 1, 2, 0, 1, 0, 1 },
                    { 1, 0, 1, 0, 0, 1 },
                    { 1, 0, 0, 0, 1, 1 },
                    { 1, 1, 1, 3, 1, 1 }
                },
                Moves = 12
            }
        };
    }

    public class Level
    {
        public int[,] Map { get; set; }
        public int Moves { get; set; }
    }
}