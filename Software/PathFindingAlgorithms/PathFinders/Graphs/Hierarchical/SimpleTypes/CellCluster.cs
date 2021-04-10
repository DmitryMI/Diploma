using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PathFinders.Algorithms;

namespace PathFinders.Graphs.Hierarchical.SimpleTypes
{
    public class CellCluster : ICellCluster
    {
        private List<IWeightedGraphNode<double>> _transitionNodes = new List<IWeightedGraphNode<double>>();
        private IList<Vector2Int>[,] _pathMatrix;

        public Vector2Int ClusterIndex { get; set; }

        public Vector2Int LeftBottom { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public ICellMap Map { get; set; }

        public bool IsPassable(int x, int y)
        {
            return Map.IsPassable(x, y);
        }

        public bool IsInBounds(int x, int y)
        {
            if (x < LeftBottom.X || y < LeftBottom.Y || x >= LeftBottom.X+Width || y >= LeftBottom.Y + Height)
            {
                return false;
            }

            return true;
        }

        public CellCluster(Vector2Int clusterIndex)
        {
            ClusterIndex = clusterIndex;
        }


        public void CalculatePaths(NeighbourMode neighbourMode, Action<object, int, int, int> onCellViewedCallback)
        {
            _pathMatrix = new IList<Vector2Int>[_transitionNodes.Count, _transitionNodes.Count];
            AStarAlgorithm aStarAlgorithm = new AStarAlgorithm();
            aStarAlgorithm.OnCellViewedEvent += onCellViewedCallback;
            CoordinateTransformer transformer = new CoordinateTransformer(this, LeftBottom);
            for (int i = 0; i < _transitionNodes.Count; i++)
            {
                for (int j = i + 1; j < _transitionNodes.Count; j++)
                {
                    var nodeA = _transitionNodes[i];
                    var nodeB = _transitionNodes[j];
                    IList<Vector2Int> path =
                        aStarAlgorithm.GetPath(transformer, nodeA.Position - LeftBottom, nodeB.Position - LeftBottom,
                            neighbourMode);

                    _pathMatrix[i, j] = path;
                    _pathMatrix[j, i] = Utils.GetInvertedList(path);
                    if (path != null)
                    {
                        nodeA.SetWeight(nodeB, path.Count);
                        nodeB.SetWeight(nodeA, path.Count);
                    }
                }
            }
        }

        public void ConnectNode(IWeightedGraphNode<double> node, NeighbourMode neighbourMode)
        {
            AStarAlgorithm aStarAlgorithm = new AStarAlgorithm();
            CoordinateTransformer transformer = new CoordinateTransformer(this, LeftBottom);

            foreach (var transitionNode in _transitionNodes)
            {
                IList<Vector2Int> path = aStarAlgorithm.GetPath(transformer, node.Position - LeftBottom,
                    transitionNode.Position - LeftBottom, neighbourMode);
                if (path != null)
                {
                    node.SetWeight(transitionNode, path.Count);
                    transitionNode.SetWeight(node, path.Count);
                    // Other mode will not be connected
                }
            }
        }

        public List<IWeightedGraphNode<double>> TransitionNodes
        {
            get =>_transitionNodes;
            set => _transitionNodes = value;
        }

        public IList<Vector2Int> GetPath(int nodeIndexA, int nodeIndexB)
        {
            return _pathMatrix[nodeIndexA, nodeIndexB];
        }
        

        public IList<Vector2Int> GetPath(Vector2Int from, Vector2Int to)
        {
            int[] indexes = Utils.GetIndexesByFilters(_transitionNodes, n => n.Position == from, n => n.Position == to);
            int startIndex = indexes[0];
            int goalIndex = indexes[1];
            if (startIndex != -1 && goalIndex != -1)
            {
                
                return GetPath(indexes[0], indexes[1]);
            }

            return null;
        }
    }
}
