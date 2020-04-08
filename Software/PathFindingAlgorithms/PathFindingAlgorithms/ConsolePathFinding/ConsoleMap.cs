using System;

namespace PathFindingAlgorithms.ConsolePathFinding
{
    public class ConsoleMap : IMap
    {
        private readonly bool[,] _cells;
        public bool IsPassable(int x, int y)
        {
            if (!IsInBounds(x, y))
                return false;

            return _cells[x, y];
        }

        public bool IsInBounds(int x, int y)
        {
            if (x < 0 || y < 0)
                return false;
            if (x >= Width || y >= Height)
                return false;
            return true;
        }

        public int Width { get; set; }
        public int Height { get; set; }

        public Vector2 DefaultStart { get; set; }
        public Vector2 DefaultStop { get; set; }

        public void SetCell(int x, int y, bool passable)
        {
            _cells[x, y] = passable;
        }

        public ConsoleMap(int width, int height)
        {
            _cells = new bool[width,height];
            Width = width;
            Height = height;
        }

        public char[,] GetCharLayer()
        {
            char[,] buffer = new char[Width,Height];
            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    if(IsPassable(i, j))
                        buffer[i, j] = ' ';
                    else
                    {
                        buffer[i, j] = 'X';
                    }
                }
            }

            return buffer;
        }

        public ConsoleColorLayer GetColorLayer(ConsoleColor wall = ConsoleColor.DarkGreen, ConsoleColor passable = ConsoleColor.Black)
        {
            ConsoleColor[,] buffer = new ConsoleColor[Width, Height];
            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    if (IsPassable(i, j))
                        buffer[i, j] = passable;
                    else
                    {
                        buffer[i, j] = wall;
                    }
                }
            }

            return new ConsoleColorLayer(buffer);
        }
    }
}
