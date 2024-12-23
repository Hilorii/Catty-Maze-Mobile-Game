using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobileApp.Models;

public class LabyrinthDrawable : IDrawable
{
    private int[,] _map;
    private int _playerX, _playerY; // Pozycja gracza
    public bool IsGoalReached { get; private set; }

    public LabyrinthDrawable()
    {
        LoadLevel();
    }

    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        float cellSize = 50; // Rozmiar komórki
        for (int y = 0; y < _map.GetLength(0); y++)
        {
            for (int x = 0; x < _map.GetLength(1); x++)
            {
                switch (_map[y, x])
                {
                    case 1: // Ściana
                        canvas.FillColor = Colors.Black;
                        canvas.FillRectangle(x * cellSize, y * cellSize, cellSize, cellSize);
                        break;
                    case 3: // Cel
                        canvas.FillColor = Colors.Green;
                        canvas.FillRectangle(x * cellSize, y * cellSize, cellSize, cellSize);
                        break;
                }
            }
        }

        // Rysowanie gracza
        canvas.FillColor = Colors.Blue;
        canvas.FillRectangle(_playerX * cellSize, _playerY * cellSize, cellSize, cellSize);
    }

    public bool MovePlayer(int deltaX, int deltaY)
    {
        int newX = _playerX, newY = _playerY;

        // Poruszaj się, aż napotkasz przeszkodę
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

        // Aktualizacja pozycji
        _playerX = newX;
        _playerY = newY;

        // Sprawdź, czy dotarłeś do celu
        if (_map[_playerY, _playerX] == 3)
        {
            IsGoalReached = true;
        }

        return true;
    }

    public void LoadLevel()
    {
        // Przykładowy poziom
        _map = new int[,]
        {
            { 1, 1, 1, 1, 1 },
            { 1, 2, 0, 0, 1 },
            { 1, 0, 1, 0, 1 },
            { 1, 0, 1, 3, 1 },
            { 1, 1, 1, 1, 1 },
        };

        // Znajdź pozycję startową gracza
        for (int y = 0; y < _map.GetLength(0); y++)
        {
            for (int x = 0; x < _map.GetLength(1); x++)
            {
                if (_map[y, x] == 2)
                {
                    _playerX = x;
                    _playerY = y;
                    return;
                }
            }
        }
    }

    public void LoadNextLevel()
    {
        // Zaimplementuj kolejne poziomy
        LoadLevel();
    }

    public void ResetLevel()
    {
        LoadLevel();
        IsGoalReached = false;
    }
}
