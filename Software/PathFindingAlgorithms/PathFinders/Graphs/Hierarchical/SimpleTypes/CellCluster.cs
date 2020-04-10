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

        public int Level { get; set; }
        public Vector2Int LeftBottom { get; set; }
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

        public int Width { get; set; }
        public int Height { get; set; }
        public ICellMap Map { get; set; }

        private void ShiftPath(IList<Vector2Int> path, Vector2Int shift)
        {
            for (int i = 0; i < path.Count; i++)
            {
                path[i] += shift;
            }
        }

        public void CalculatePaths(NeighbourMode neighbourMode)
        {
            _pathMatrix = new IList<Vector2Int>[_transitionNodes.Count, _transitionNodes.Count];
            AStarAlgorithm aStarAlgorithm = new AStarAlgorithm();
            CoordinateTransformer transformer = new CoordinateTransformer(this, LeftBottom);
            for (int i = 0; i < _transitionNodes.Count; i++)
            {
                for (int j = i + 1; j < _transitionNodes.Count; j++)
                {
                    var nodeA = _transitionNodes[i];
                    var nodeB = _transitionNodes[j];
                    IList<Vector2Int> path =
                        aStarAlgorithm.GetPath(transformer, nodeA.Position - LeftBottom, nodeB.Position - LeftBottom, neighbourMode);
                    //ShiftPath(path, LeftBottom);
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

        public List<IWeightedGraphNode<double>> TransitionNodes
        {
            get =>_transitionNodes;
            set => _transitionNodes = value;
        }

        public IList<Vector2Int> GetPath(int nodeIndexA, int nodeIndexB)
        {
            return _pathMatrix[nodeIndexA, nodeIndexB];
        }
    }
}
