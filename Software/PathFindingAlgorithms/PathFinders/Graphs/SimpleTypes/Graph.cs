using System.Collections.Generic;

namespace PathFinders.Graphs.SimpleTypes
{
    public class Graph : IGraph
    {
        private readonly IGraphNode[,] _graphNodes;

        public int Width => _graphNodes.GetLength(0);
        public int Height => _graphNodes.GetLength(1);

        public Graph(IGraphNode[] graphNodes)
        {
            foreach (var node in graphNodes)
            {
                _graphNodes[node.Position.X, node.Position.Y] = node;
            }
        }

        public Graph(IGraphNode[,] graphNodes)
        {
            _graphNodes = graphNodes;
        }

        public IGraphNode this[int index]
        {
            get
            {
                int y = index / Width;
                int x = index - y * Width;
                return this[x, y];
            }
        }

        public IGraphNode this[int x, int y]
        {
            get
            {
                if (x < 0 || y < 0 || x >= Width || y >= Height) return null;
                return _graphNodes[x, y];
            }
        }

        public IGraphNode this[Vector2Int pos] => this[pos.X, pos.Y];
        public ICollection<IGraphNode> GetGraphNodes()
        {
            List<IGraphNode> result = new List<IGraphNode>();
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    if (_graphNodes[x, y] != null)
                    {
                        result.Add(_graphNodes[x, y]);
                    }
                }
            }

            return result;
        }
    }
}
