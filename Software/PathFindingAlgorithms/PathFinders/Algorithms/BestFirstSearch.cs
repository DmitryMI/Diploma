using System;
using System.Collections.Generic;
using PathFinders.Algorithms.HpaStar;
using PathFinders.Graphs;
using PathFinders.Graphs.SimpleTypes;

namespace PathFinders.Algorithms
{
    public class BestFirstSearch : ICellPathFinder, IGraphPathFinder, IComparer<IGraphNode>
    {
        public event Action<object, int, int, int> OnCellViewedEvent;

        private LayeredCellMap _layeredCellMap;

        private struct GraphData
        { 
            public double DistanceToStop { get; set; }
            public bool IsClosed { get; set; }
            public IGraphNode ParentNode { get; set; }
            public int StepsToStart { get; set; }
        }

        private T GetData<T>(IGraphNode node)
        {
            return (T) node.Data;
        }

        private double GetDistance(Vector2Int from, Vector2Int to)
        {
            double dx = from.X - to.X;
            double dy = from.Y - to.Y;
            return Math.Sqrt(dx * dx + dy * dy);
            //return Math.Abs(from.X - to.X) + Math.Abs(from.Y - to.Y);
        }

        private void SetClosed(IGraphNode node)
        {
            GraphData data = GetData<GraphData>(node);
            data.IsClosed = true;
            node.Data = data;
        }

        private bool IsClosed(IGraphNode node)
        {
            GraphData data = GetData<GraphData>(node);
            return data.IsClosed;
        }

        private int GetStepsToStart(IGraphNode node)
        {
            return GetData<GraphData>(node).StepsToStart;
        }

        private void SetParentNode(IGraphNode node, IGraphNode parent)
        {
            GraphData data = GetData<GraphData>(node);
            data.ParentNode = parent;
            data.StepsToStart = GetStepsToStart(parent) + 1;
            node.Data = data;
        }

        public IList<IGraphNode> GetPath(IGraph graph, IGraphNode startNode, IGraphNode stopNode)
        {
            List<IGraphNode> openNodes = new List<IGraphNode> {startNode};

            while (openNodes.Count > 0)
            {
                openNodes.Sort(this);
                IGraphNode currentNode = openNodes[0];
                openNodes.RemoveAt(0);

                if (IsClosed(currentNode))
                {
                    //Console.WriteLine("Processing closed node!");
                    continue;
                }

                Vector2Int position = currentNode.Position;
                OnCellViewedEvent?.Invoke(this, position.X, position.Y, GetStepsToStart(currentNode));

                SetClosed(currentNode);

                if (currentNode == stopNode)
                {
                    break;
                }

                foreach (var connected in currentNode.GetConnectedNodes())
                {
                    if (!IsClosed(connected))
                    {
                        SetParentNode(connected, currentNode);
                        openNodes.Add(connected);
                    }
                }
            }

            if (!GetData<GraphData>(startNode).IsClosed)
            {
                return null;
            }

            List<IGraphNode> path = new List<IGraphNode>();

            IGraphNode currentPathNode = stopNode;
            path.Add(currentPathNode);
            while (currentPathNode != startNode)
            {
                currentPathNode = GetData<GraphData>(currentPathNode).ParentNode;
                path.Add(currentPathNode);
            }

            return path;
        }

        public IList<Vector2Int> GetPath(ICellMap mapBase, Vector2Int start, Vector2Int stop, NeighbourMode neighbourMode)
        {
            if (_layeredCellMap == null)
            {
                _layeredCellMap = new LayeredCellMap(mapBase);
            }

            bool StartNodeFilter(IWeightedGraphNode<double> node)
            {
                return node != null && node.Position == start;
            }

            bool StopNodeFilter(IWeightedGraphNode<double> node)
            {
                return node != null && node.Position == stop;
            }

            IWeightedGraph<double> graph = GraphGenerator.GetWeightedGraph(_layeredCellMap, neighbourMode);
            var nodes = graph.GetWeightedGraphNodes();
            IWeightedGraphNode<double>[] keyNodes = Utils.GetItemsByFilters(nodes, StartNodeFilter, StopNodeFilter);
            var startNode = keyNodes[0];
            var stopNode = keyNodes[1];

            foreach (var node in nodes)
            {
                if(node == null)
                    continue;
                node.Data = new GraphData(){DistanceToStop = GetDistance(node.Position, stop)};
            }

            var nodePath = GetPath(graph, startNode, stopNode);
            List<Vector2Int> path = new List<Vector2Int>(nodePath.Count);
            foreach (var node in nodePath)
            {
                path.Add(node.Position);
            }

            return path;
        }

        public IList<Vector2Int> GetSmoothedPath(ICellMap map, Vector2Int start, Vector2Int stop, NeighbourMode neighbourMode)
        {
            var rawPath = GetPath(map, start, stop, neighbourMode);
            if (rawPath == null)
            {
                return null;
            }
            PathSmoother smoother = new PathSmoother();
            var path = smoother.GetSmoothedPath(map, rawPath);
            return path;
        }

        public void AddObstacle(ICellFragment cellCluster)
        {
            _layeredCellMap.AddFragment(cellCluster);
        }

        public void ClearObstacles()
        {
            _layeredCellMap.ClearLayers();
        }

        public void RecalculateObstacles(NeighbourMode neighbourMode = NeighbourMode.SidesAndDiagonals)
        {
            
        }

        public void Initialize(ICellMap mapBase)
        {
            _layeredCellMap = new LayeredCellMap(mapBase);
        }

        public int Compare(IGraphNode a, IGraphNode b)
        {
            if (a == null || b == null)
                return 0;

            if (GetData<GraphData>(a).DistanceToStop >= GetData<GraphData>(b).DistanceToStop)
            {
                return 1;
            }

            return -1;
        }
    }
}
