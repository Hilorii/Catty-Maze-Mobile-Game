using System;
using Microsoft.Maui.Graphics;
using MobileApp.Models;


namespace MobileApp.Models
{
    public class LabyrinthDrawable : IDrawable
    {
        private int[,] _map; // Aktualny układ labiryntu
        private int _playerX, _playerY; // Pozycja gracza
        private int _currentLevelIndex; // Aktualny poziom

        public bool IsGoalReached { get; private set; }
        public int MovesRemaining { get; private set; } // Liczba pozostałych ruchów

        public LabyrinthDrawable()
        {
            _currentLevelIndex = 0;
            LoadLevel();
        }

        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            float cellSize = CalculateCellSize(dirtyRect); // Dynamiczny rozmiar komórki
            float offsetX = (dirtyRect.Width - (_map.GetLength(1) * cellSize)) / 2;
            float offsetY = (dirtyRect.Height - (_map.GetLength(0) * cellSize)) / 2;

            // Rysowanie labiryntu
            for (int y = 0; y < _map.GetLength(0); y++)
            {
                for (int x = 0; x < _map.GetLength(1); x++)
                {
                    float left = offsetX + (x * cellSize);
                    float top = offsetY + (y * cellSize);

                    switch (_map[y, x])
                    {
                        case 1: // Ściana
                            canvas.FillColor = Colors.Black;
                            canvas.FillRectangle(left, top, cellSize, cellSize);
                            break;
                        case 3: // Cel
                            canvas.FillColor = Colors.Green;
                            canvas.FillRectangle(left, top, cellSize, cellSize);
                            break;
                    }
                }
            }

            // Rysowanie gracza
            canvas.FillColor = Colors.Blue;
            float playerLeft = offsetX + (_playerX * cellSize);
            float playerTop = offsetY + (_playerY * cellSize);
            canvas.FillRectangle(playerLeft, playerTop, cellSize, cellSize);
        }

        public bool MovePlayer(int deltaX, int deltaY)
        {
            int newX = _playerX, newY = _playerY;

            // Poruszanie gracza do napotkania przeszkody
            while (true)
            {
                int nextX = newX + deltaX;
                int nextY = newY + deltaY;

                if (nextX < 0 || nextX >= _map.GetLength(1) ||
                    nextY < 0 || nextY >= _map.GetLength(0) ||
                    _map[nextY, nextX] == 1) break;

                newX = nextX;
                newY = nextY;
            }

            if (newX == _playerX && newY == _playerY)
                return false; // Gracz nie zmienił pozycji

            _playerX = newX;
            _playerY = newY;
            MovesRemaining--;

            // Sprawdzenie, czy osiągnięto cel
            if (_map[_playerY, _playerX] == 3)
            {
                IsGoalReached = true;
            }

            return true;
        }

        public void LoadLevel()
        {
            if (_currentLevelIndex >= LevelData.AllLevels.Count)
            {
                _currentLevelIndex = 0; // Restart do pierwszego poziomu
            }

            var level = LevelData.AllLevels[_currentLevelIndex];
            _map = level.Map;
            MovesRemaining = level.Moves;

            // Znajdź pozycję startową gracza
            for (int y = 0; y < _map.GetLength(0); y++)
            {
                for (int x = 0; x < _map.GetLength(1); x++)
                {
                    if (_map[y, x] == 2)
                    {
                        _playerX = x;
                        _playerY = y;
                        IsGoalReached = false;
                        return;
                    }
                }
            }
        }

        public void LoadNextLevel()
        {
            _currentLevelIndex++;
            LoadLevel();
        }

        public void ResetLevel()
        {
            LoadLevel();
        }

        private float CalculateCellSize(RectF dirtyRect)
        {
            // Obliczanie dynamicznego rozmiaru komórki na podstawie dostępnej przestrzeni
            float cellWidth = dirtyRect.Width / _map.GetLength(1);
            float cellHeight = dirtyRect.Height / _map.GetLength(0);
            return Math.Min(cellWidth, cellHeight);
        }
    }
}
