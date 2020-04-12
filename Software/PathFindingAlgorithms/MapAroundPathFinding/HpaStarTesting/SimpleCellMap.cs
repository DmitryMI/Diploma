using PathFinders;

namespace MapAroundPathFinding
{
    public class SimpleCellMap : ICellMap
    {
        private bool[,] _cells;

        public Vector2Int DefaultStart { get; set; }
        public Vector2Int DefaultStop { get; set; }

        public SimpleCellMap(int width, int height)
        {
            _cells = new bool[width,height];
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    _cells[x, y] = true;
                }
            }
        }

        public bool this[int x, int y]
        {
            get => _cells[x, y];
            set => _cells[x, y] = value;
        }

        public bool IsPassable(int x, int y)
        {
            return _cells[x, y];
        }

        public bool IsInBounds(int x, int y)
        {
            if (x < 0 || y < 0 || x >= Width || y >= Height)
            {
                return false;
            }

            return true;
        }

        public int Width => _cells.GetLength(0);
        public int Height => _cells.GetLength(1);
    }
}
