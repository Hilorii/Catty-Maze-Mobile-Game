using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Storage;

namespace MobileApp.Models
{
    public class LabyrinthDrawable : IDrawable
    {
        private int[,] _map;
        private int _playerX, _playerY;
        private int _currentLevelIndex;
        private int _coinsRemaining;
        public float _animatedX, _animatedY;

        private Microsoft.Maui.Graphics.IImage? _coinImage;

        public int CoinsRemaining => _coinsRemaining;
        public int MovesRemaining { get; private set; }
        public int CurrentLevelIndex => _currentLevelIndex;
        public int PlayerX => _playerX;
        public int PlayerY => _playerY;

        public LabyrinthDrawable()
        {
            _map = new int[1, 1]; // Tymczasowa wartość, aby unikać null
            LoadLevel();
            LoadImage("Coin.png");
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
                        case 0: //tło labiryntu
                            canvas.FillColor = Colors.Transparent;
                            canvas.FillRectangle(left, top, cellSize, cellSize);
                            break;
                        case 1: // Ściana
                            canvas.FillColor = Colors.Transparent;
                            canvas.FillRectangle(left, top, cellSize, cellSize);
                            Microsoft.Maui.Graphics.IImage _wallImage = LoadImage("Wall1.png");
                            if (_wallImage != null)
                            {
                                canvas.DrawImage(_wallImage, left, top, cellSize, cellSize);
                            }
                            break;
                        case 2:
                            canvas.FillColor = Colors.Transparent;
                            canvas.FillRectangle(left, top, cellSize, cellSize);
                            break;
                        case 3: // Moneta
                            canvas.FillColor = Colors.Transparent;
                            canvas.FillRectangle(left, top, cellSize, cellSize);
                            Microsoft.Maui.Graphics.IImage _coinImage = LoadImage("Coin.png");
                            if (_coinImage != null)
                            {
                                canvas.DrawImage(_coinImage, left, top, cellSize, cellSize);
                            }
                            break;
                        case 4: //niewidzialna ściana
                            canvas.FillColor = Colors.Transparent;
                            canvas.FillRectangle(left, top, cellSize, cellSize);
                            break;
                    }
                }
            }

            // Rysowanie gracza jako obraz
            float playerLeft = offsetX + (_animatedX * cellSize);
            float playerTop = offsetY + (_animatedY * cellSize);

            Microsoft.Maui.Graphics.IImage playerImage = LoadImage("playerr.png");
            if (playerImage != null)
            {
                canvas.DrawImage(playerImage, playerLeft, playerTop, cellSize, cellSize);
            }
        }

        public void LoadLevel(int levelIndex = 0)
        {
            _currentLevelIndex = levelIndex;

            var level = LevelData.AllLevels[_currentLevelIndex];
            _map = (int[,])level.Map.Clone();
            MovesRemaining = level.Moves;
            _coinsRemaining = 0;

            for (int y = 0; y < _map.GetLength(0); y++)
            {
                for (int x = 0; x < _map.GetLength(1); x++)
                {
                    if (_map[y, x] == 2)
                    {
                        _playerX = x;
                        _playerY = y;

                        // Synchronizujemy pozycje animowane z rzeczywistą
                        _animatedX = _playerX;
                        _animatedY = _playerY;
                    }
                    else if (_map[y, x] == 3)
                    {
                        _coinsRemaining++;
                    }
                }
            }
        }


        public List<(int X, int Y)> GetPlayerPath(int deltaX, int deltaY)
        {
            List<(int X, int Y)> path = new();
            int currentX = _playerX, currentY = _playerY;

            while (true)
            {
                int nextX = currentX + deltaX;
                int nextY = currentY + deltaY;

                if (nextX < 0 || nextX >= _map.GetLength(1) ||
                    nextY < 0 || nextY >= _map.GetLength(0) ||
                    _map[nextY, nextX] == 1)
                    break;

                path.Add((nextX, nextY));
                currentX = nextX;
                currentY = nextY;
            }

            if (path.Count > 0)
            {
                _playerX = currentX;
                _playerY = currentY;
                MovesRemaining--;
            }

            return path;
        }




        public void SetAnimatedPosition(float x, float y)
        {
            Debug.WriteLine($"Ustawianie animowanej pozycji: X={x}, Y={y}");
            _animatedX = x;
            _animatedY = y;
        }

        public void SetTemporaryPlayerPosition(int x, int y)
        {
            Debug.WriteLine($"Ustawianie rzeczywistej pozycji gracza: X={x}, Y={y}");
            _playerX = x;
            _playerY = y;

            // Synchronizujemy pozycje animowane z rzeczywistą
            _animatedX = _playerX;
            _animatedY = _playerY;
        }



        private Microsoft.Maui.Graphics.IImage? LoadImage(string fileName)
        {
            try
            {
                using var stream = FileSystem.OpenAppPackageFileAsync(fileName).GetAwaiter().GetResult();
                if (stream != null)
                {
                    return Microsoft.Maui.Graphics.Platform.PlatformImage.FromStream(stream);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Błąd ładowania obrazu '{fileName}': {ex.Message}");
            }

            return null;
        }

        private float CalculateCellSize(RectF dirtyRect)
        {
            // Oblicz szerokość i wysokość komórki w zależności od dostępnego miejsca
            float cellWidth = dirtyRect.Width / _map.GetLength(1);
            float cellHeight = dirtyRect.Height / _map.GetLength(0);

            // Zwróć mniejszą wartość, aby zachować proporcje i zmieścić całość
            return Math.Min(cellWidth, cellHeight);
        }


        public void LoadNextLevel()
        {
            _currentLevelIndex++;
            if (_currentLevelIndex >= LevelData.AllLevels.Count)
            {
                _currentLevelIndex = 0; // Wraca na pierwszy poziom
            }
            Debug.WriteLine($"LabyrinthDrawable: Ładowanie poziomu {_currentLevelIndex}");
            LoadLevel(_currentLevelIndex);
        }

        public void ResetLevel()
        {
            Debug.WriteLine($"LabyrinthDrawable: Resetowanie poziomu {_currentLevelIndex}");
            LoadLevel(_currentLevelIndex);
        }

        public bool CheckAndCollectCoin(int x, int y)
        {
            if (_map[y, x] == 3) // Sprawdź, czy na polu jest moneta
            {
                _map[y, x] = 0; // Usuń monetę z mapy
                _coinsRemaining--; // Zmniejsz licznik monet
                return true; // Zwróć informację, że moneta została zebrana
            }
            return false;
        }


    }
}
