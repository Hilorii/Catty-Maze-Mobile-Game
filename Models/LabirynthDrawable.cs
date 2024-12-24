using System;
using Microsoft.Maui.Graphics;

namespace MobileApp.Models
{
    public class LabyrinthDrawable : IDrawable
    {
        private int[,] _map;
        private int _playerX, _playerY;
        private int _currentLevelIndex;
        private int _coinsRemaining;

        public int CoinsRemaining => _coinsRemaining; // Dodana właściwość
        public int MovesRemaining { get; private set; }

        public LabyrinthDrawable()
        {
            _currentLevelIndex = 0;
            LoadLevel();
        }

        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            float cellSize = CalculateCellSize(dirtyRect);
            float offsetX = (dirtyRect.Width - (_map.GetLength(1) * cellSize)) / 2;
            float offsetY = (dirtyRect.Height - (_map.GetLength(0) * cellSize)) / 2;

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
                        case 3: // Moneta
                            canvas.FillColor = Colors.Gold;
                            canvas.FillEllipse(left, top, cellSize, cellSize);
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

            while (true)
            {
                int nextX = newX + deltaX;
                int nextY = newY + deltaY;

                // Sprawdzenie, czy gracz nie wychodzi poza mapę lub nie napotyka przeszkody
                if (nextX < 0 || nextX >= _map.GetLength(1) ||
                    nextY < 0 || nextY >= _map.GetLength(0) ||
                    _map[nextY, nextX] == 1) break;

                newX = nextX;
                newY = nextY;

                // Jeśli gracz przechodzi przez monetę, zbiera ją
                if (_map[newY, newX] == 3)
                {
                    _map[newY, newX] = 0; // Usuń monetę z mapy
                    _coinsRemaining--;
                }
            }

            // Sprawdzenie, czy gracz faktycznie się poruszył
            if (newX == _playerX && newY == _playerY)
                return false;

            _playerX = newX;
            _playerY = newY;
            MovesRemaining--;

            return true;
        }

        public void LoadLevel()
        {
            if (_currentLevelIndex >= LevelData.AllLevels.Count)
            {
                _currentLevelIndex = 0;
            }

            var level = LevelData.AllLevels[_currentLevelIndex];
            _map = level.Map;
            MovesRemaining = level.Moves;
            _coinsRemaining = 0;

            for (int y = 0; y < _map.GetLength(0); y++)
            {
                for (int x = 0; x < _map.GetLength(1); x++)
                {
                    if (_map[y, x] == 2) // Gracz
                    {
                        _playerX = x;
                        _playerY = y;
                    }
                    else if (_map[y, x] == 3) // Moneta
                    {
                        _coinsRemaining++;
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
            float cellWidth = dirtyRect.Width / _map.GetLength(1);
            float cellHeight = dirtyRect.Height / _map.GetLength(0);
            return Math.Min(cellWidth, cellHeight);
        }
    }
}
