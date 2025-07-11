﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Maui.Storage;


namespace MobileApp.Models
{
    public class LabyrinthDrawable : Microsoft.Maui.Graphics.IDrawable
    {
        // Mapa
        private int[,] _map;
        private int _currentLevelIndex;
        private int _coinsRemaining;

        // Pozycja gracza na mapie
        private int _playerX, _playerY;

        // Pozycja do płynnej animacji
        private float _animatedX, _animatedY;
        public float AnimatedX => _animatedX;
        public float AnimatedY => _animatedY;

        // Kierunek postaci (do obrotu w idle)
        public MovementDirection LastDirection { get; set; } = MovementDirection.Down;

        // Flaga, czy jesteśmy w trakcie animacji skoku
        public bool IsJumping { get; set; } = false;

        // Liczba ruchów
        public int MovesRemaining { get; private set; }
        public int CoinsRemaining => _coinsRemaining;
        public int CurrentLevelIndex => _currentLevelIndex;
        public int PlayerX => _playerX;
        public int PlayerY => _playerY;

        // Animacja IDLE
        private readonly string[] _idleFrames = { "Player1.png", "Player2.png", "Player3.png" };
        private int _currentIdleFrame = 0;

        // Aktualna grafika (gdy IsJumping = true)
        private string _currentPlayerImage = "PlayerJumpRight.png";

        // =========[ NOWOŚĆ: Cache obrazów ]=========
        private static readonly Dictionary<string, Microsoft.Maui.Graphics.IImage?> _imageCache
            = new Dictionary<string, Microsoft.Maui.Graphics.IImage?>();

        public LabyrinthDrawable()
        {
            _map = new int[1, 1];
            LoadLevel();
        }

        public void Draw(Microsoft.Maui.Graphics.ICanvas canvas, Microsoft.Maui.Graphics.RectF dirtyRect)
        {
            float cellSize = CalculateCellSize(dirtyRect);
            float offsetX = (dirtyRect.Width - (_map.GetLength(1) * cellSize)) / 2;
            float offsetY = (dirtyRect.Height - (_map.GetLength(0) * cellSize)) / 2;

            // Rysowanie tła
            var bgImage = LoadImage("LBG.png");
            if (bgImage != null)
            {
                canvas.DrawImage(bgImage, 0, 0, dirtyRect.Width, dirtyRect.Height);
            }

            // Rysowanie mapy
            for (int y = 0; y < _map.GetLength(0); y++)
            {
                for (int x = 0; x < _map.GetLength(1); x++)
                {
                    float left = offsetX + x * cellSize;
                    float top = offsetY + y * cellSize;

                    switch (_map[y, x])
                    {
                        case 0: // puste pole
                            var backImg = LoadImage("LabirynthBackground1.png");
                            if (backImg != null)
                                canvas.DrawImage(backImg, left, top, cellSize, cellSize);
                            break;

                        case 1: // ściana
                            var wallImg = LoadImage("Wall1.png");
                            if (wallImg != null)
                                canvas.DrawImage(wallImg, left, top, cellSize, cellSize);
                            break;

                        case 2: // start gracza
                            var behindPlayer = LoadImage("LabirynthBackground1.png");
                            if (behindPlayer != null)
                                canvas.DrawImage(behindPlayer, left, top, cellSize, cellSize);
                            break;

                        case 3: // moneta
                            var behindCoin = LoadImage("LabirynthBackground1.png");
                            if (behindCoin != null)
                                canvas.DrawImage(behindCoin, left, top, cellSize, cellSize);

                            var coinImg = LoadImage("Coin.png");
                            if (coinImg != null)
                                canvas.DrawImage(coinImg, left, top, cellSize, cellSize);
                            break;

                        case 4: // niewidzialna ściana
                            // Nic nie rysujemy
                            break;
                    }
                }
            }

            // Rysowanie gracza
            float playerLeft = offsetX + (_animatedX * cellSize);
            float playerTop = offsetY + (_animatedY * cellSize);

            // 1. Jeśli skaczemy, używamy _currentPlayerImage
            // 2. Jeśli nie skaczemy (IsJumping==false), używamy klatki idle + obrót
            if (IsJumping)
            {
                var jumpImage = LoadImage(_currentPlayerImage);
                if (jumpImage != null)
                {
                    canvas.SaveState();

                    float pivotX = playerLeft + (cellSize / 2f);
                    float pivotY = playerTop + (cellSize / 2f);
                    float angle = GetRotationAngle(LastDirection);
                    canvas.Rotate(angle, pivotX, pivotY);

                    canvas.DrawImage(jumpImage, playerLeft, playerTop, cellSize, cellSize);
                    canvas.RestoreState();
                }
            }
            else
            {
                // Animacja IDLE
                string currentFrame = _idleFrames[_currentIdleFrame];
                var playerImage = LoadImage(currentFrame);
                if (playerImage != null)
                {
                    canvas.SaveState();

                    float pivotX = playerLeft + (cellSize / 2f);
                    float pivotY = playerTop + (cellSize / 2f);
                    float angle = GetRotationAngle(LastDirection);
                    canvas.Rotate(angle, pivotX, pivotY);

                    canvas.DrawImage(playerImage, playerLeft, playerTop, cellSize, cellSize);
                    canvas.RestoreState();
                }
            }
        }

        // Metoda do wczytywania poziomu
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
                        _animatedX = x;
                        _animatedY = y;
                    }
                    else if (_map[y, x] == 3)
                    {
                        _coinsRemaining++;
                    }
                }
            }
        }

        // Ruch gracza
        public List<(int X, int Y)> GetPlayerPath(int deltaX, int deltaY)
        {
            List<(int X, int Y)> path = new();
            int currentX = _playerX;
            int currentY = _playerY;

            while (true)
            {
                int nextX = currentX + deltaX;
                int nextY = currentY + deltaY;

                if (nextX < 0 || nextX >= _map.GetLength(1) ||
                    nextY < 0 || nextY >= _map.GetLength(0) ||
                    _map[nextY, nextX] == 1)
                {
                    // Kolizja albo wychodzimy poza mapę
                    break;
                }

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

        // Zmieniamy klatkę IDLE
        public void NextIdleFrame()
        {
            _currentIdleFrame++;
            if (_currentIdleFrame >= _idleFrames.Length)
                _currentIdleFrame = 0;
        }

        // Ustawiamy grafikę skoku (z OnSwipedX)
        public void SetPlayerImage(string imageName)
        {
            _currentPlayerImage = imageName;
        }

        // Zbieranie monety
        public bool CheckAndCollectCoin(int x, int y)
        {
            if (_map[y, x] == 3)
            {
                _map[y, x] = 0;
                _coinsRemaining--;
                return true;
            }
            return false;
        }

        // Płynne przesuwanie
        public void SetAnimatedPosition(float x, float y)
        {
            Debug.WriteLine($"SetAnimatedPosition: X={x}, Y={y}");
            _animatedX = x;
            _animatedY = y;
        }

        // Ustawienie ostateczne
        public void SetTemporaryPlayerPosition(int x, int y)
        {
            Debug.WriteLine($"SetTemporaryPlayerPosition: X={x}, Y={y}");
            _playerX = x;
            _playerY = y;
            _animatedX = x;
            _animatedY = y;
        }

        public void LoadNextLevel()
        {
            _currentLevelIndex++;
            if (_currentLevelIndex >= LevelData.AllLevels.Count)
            {
                _currentLevelIndex = 0;
            }
            Debug.WriteLine($"LabyrinthDrawable: Ładowanie poziomu {_currentLevelIndex}");
            LoadLevel(_currentLevelIndex);

            // Przywróć „domyślny” kierunek
            LastDirection = MovementDirection.Down;
            IsJumping = false;
        }

        public void ResetLevel()
        {
            Debug.WriteLine($"LabyrinthDrawable: Resetowanie poziomu {_currentLevelIndex}");
            LoadLevel(_currentLevelIndex);

            // Przywróć „domyślny” kierunek
            LastDirection = MovementDirection.Down;
            IsJumping = false;
        }

        private float CalculateCellSize(Microsoft.Maui.Graphics.RectF dirtyRect)
        {
            float cellWidth = dirtyRect.Width / _map.GetLength(1);
            float cellHeight = dirtyRect.Height / _map.GetLength(0);
            return Math.Min(cellWidth, cellHeight);
        }

        private Microsoft.Maui.Graphics.IImage? LoadImage(string fileName)
        {
            // Najpierw sprawdzamy, czy w cache już jest ten obraz
            if (_imageCache.TryGetValue(fileName, out var cachedImage))
            {
                return cachedImage;
            }

            try
            {
                using var stream = FileSystem.OpenAppPackageFileAsync(fileName).GetAwaiter().GetResult();
                if (stream != null)
                {
                    var loadedImage = Microsoft.Maui.Graphics.Platform.PlatformImage.FromStream(stream);
                    _imageCache[fileName] = loadedImage;
                    return loadedImage;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Błąd ładowania obrazu '{fileName}': {ex.Message}");
            }

            // Jeśli nie udało się wczytać obrazu – zapisz null w cache, żeby uniknąć kolejnych prób
            _imageCache[fileName] = null;
            return null;
        }

        // Obrót w zależności od kierunku
        private float GetRotationAngle(MovementDirection direction)
        {
            // Dostosuj wartości do swojej logiki
            switch (direction)
            {
                case MovementDirection.Right: return 270f;
                case MovementDirection.Up: return 180f;
                case MovementDirection.Left: return 90f;
                case MovementDirection.Down: return 0f;
                default: return 0f;
            }
        }

        // Kierunki
        public enum MovementDirection
        {
            Left,
            Right,
            Up,
            Down
        }
    }
}
