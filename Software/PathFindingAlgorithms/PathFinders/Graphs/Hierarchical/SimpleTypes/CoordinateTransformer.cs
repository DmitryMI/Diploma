using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PathFinders.Graphs.Hierarchical.SimpleTypes
{
    public class CoordinateTransformer : ICellMap
    {
        public Vector2Int Transform { get; set; }
        public ICellMap Map { get; set; }
        public bool IsPassable(int x, int y)
        {
            return Map.IsPassable(x + Transform.X, y + Transform.Y);
        }

        public bool IsInBounds(int x, int y)
        {
            return Map.IsInBounds(x + Transform.X, y + Transform.Y);
        }

        public int Width => Map.Width;
        public int Height => Map.Height;

        public CoordinateTransformer(ICellMap map, Vector2Int transform)
        {
            Transform = transform;
            Map = map;
        }
    }
}
