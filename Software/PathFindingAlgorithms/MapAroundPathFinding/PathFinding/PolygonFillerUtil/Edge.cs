using PathFinders;

namespace MapAroundPathFinding.PathFinding.PolygonFillerUtil
{
    public class Edge
    {
        public Vector2Int Start { get; set; }
        public Vector2Int End { get; set; }

        public Edge(Vector2Int a, Vector2Int b)
        {
            if (a.Y <= b.Y)
            {
                Start = a;
                End = b;
            }
            else
            {
                Start = b;
                End = a;
            }
        }

        public override string ToString()
        {
            return $"[{Start}, {End}]";
        }
    }
}