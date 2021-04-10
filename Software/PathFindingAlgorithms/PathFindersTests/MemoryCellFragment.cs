using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PathFinders;

namespace PathFindersTests
{
    public class MemoryCellFragment : ICellFragment, IMutableCellMap
    {
        private int _width;
        private int _height;
        private Vector2Int _leftBottom;

        private bool[,] _cells;

        public bool IsPassable(int x, int y)
        {
            return _cells[x, y];
        }

        public bool IsInBounds(int x, int y)
        {
            return x >= 0 && y >= 0 && x < Width && y < Height;
        }

        public int Width
        {
            get => _width;
        }

        public int Height
        {
            get => _height;
        }

        public void SetPassable(int x, int y, bool isPassable)
        {
            _cells[x, y] = isPassable;
        }

        public Vector2Int LeftBottom
        {
            get => _leftBottom;
        }

        public MemoryCellFragment(int width, int height, Vector2Int leftBottom)
        {
            _width = width;
            _height = height;
            _cells = new bool[width, height];
            _leftBottom = leftBottom;

            for (int i = 0; i < _width; i++)
            {
                for (int j = 0; j < _height; j++)
                {
                    _cells[i, j] = true;
                }
            }
        }

        public MemoryCellFragment(int width, int height)
        {
            _width = width;
            _height = height;
            _cells = new bool[width, height];
            _leftBottom = Vector2Int.Zero;

            for (int i = 0; i < _width; i++)
            {
                for (int j = 0; j < _height; j++)
                {
                    _cells[i, j] = true;
                }
            }
        }

        public bool this[int x, int y]
        {
            get => _cells[x, y];
            set => _cells[x, y] = value;
        }

        public bool this[Vector2Int cell]
        {
            get => this[cell.X, cell.Y];
            set => this[cell.X, cell.Y] = value;
        }
    }
}
