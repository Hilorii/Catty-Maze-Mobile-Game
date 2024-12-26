using System.Collections.Generic;

namespace MobileApp.Models
{
    public static class LevelData
    {
        public static List<Level> AllLevels { get; } = new List<Level>
        {

            // 1 = ściana
            // 2 = gracz
            // 3 moneta
            // 4 = niewidzialna ściana
            new Level
            {
                Map = new int[,]
                {
                    { 1, 1, 1, 1, 1 },
                    { 1, 2, 0, 0, 1 },
                    { 1, 0, 1, 3, 1 },
                    { 1, 0, 1, 0, 1 },
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
                    { 1, 0, 1, 3, 0, 1 },
                    { 1, 0, 0, 0, 1, 1 },
                    { 1, 1, 1, 1, 1, 1 }
                },
                Moves = 12
            },
            new Level
            {
                Map = new int[,]
                {
                    { 1, 1, 1, 1, 1, 1, 4 },
                    { 1, 0, 0, 0, 0, 1, 4 },
                    { 1, 0, 0, 0, 0, 1, 1 },
                    { 1, 0, 0, 0, 0, 3, 1 },
                    { 1, 1, 0, 0, 0, 1, 1 },
                    { 1, 0, 0, 0, 0, 1, 4 },
                    { 1, 2, 0, 0, 0, 1, 4 },
                    { 1, 1, 1, 1, 1, 1, 4 }
                },
                Moves = 5
            }
        };
    }

    public class Level
    {
        public int[,] Map { get; set; }
        public int Moves { get; set; }
    }
}