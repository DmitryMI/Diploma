using System;
using System.Collections.Generic;
using System.Linq;
using PathFinders.Graphs;
using PathFinders.Graphs.SimpleTypes;

namespace PathFinders.Algorithms
{
    public class AStarAlgorithm : ICellPathFinder, IGraphPathFinder, IWeightedGraphPathFinder<double>, IComparer<IGraphNode>
    {
        public event Action<object, int, int, int> OnCellViewedEvent;
        

        private struct GraphData
        {
            public double HValue { get; set; }
            public double GValue { get; set; }
            public double FValue => HValue + GValue;
            public bool IsExplored { get; set; }
            public IGraphNode PathPredecessor { get; set; }
        }

        private void InsertSorted(IList<IGraphNode> nodeList, IGraphNode graph)
        {
            int index = 0;
            while (index < nodeList.Count)
            {
                GraphData graphData = (GraphData)nodeList[index].Data;
                GraphData insertGraphData = (GraphData) graph.Data;

                if (insertGraphData.FValue > graphData.FValue)
                {
                    index++;
                }
                else
                {
                    index++;
                    break;
                }
            }

            nodeList.Insert(index, graph);
        }

        private bool IsExplored(IGraphNode graph)
        {
            return ((GraphData) graph.Data).IsExplored;
        }

        private void MarkExplored(IGraphNode graph)
        {
            GraphData data = (GraphData) graph.Data;
            data.IsExplored = true;
            graph.Data = data;
        }

        private void SetPathPredecessor(IGraphNode node, IGraphNode predecessor)
        {
            GraphData data = (GraphData)node.Data;
            data.PathPredecessor = predecessor;
            node.Data = data;
        }

        private IGraphNode GetPathPredecessor(IGraphNode node)
        {
            GraphData data = (GraphData)node.Data;
            return data.PathPredecessor;
        }

        private void SetGValue(IGraphNode node, double g)
        {
            GraphData data = (GraphData)node.Data;
            data.GValue = g;
            node.Data = data;
        }

        private double GetGValue(IGraphNode node)
        {
            GraphData data = (GraphData)node.Data;
            return data.GValue;
        }

        private double GetFValue(IGraphNode node)
        {
            GraphData data = (GraphData)node.Data;
            return data.FValue;
        }

        private double GetHValue(IGraphNode node)
        {
            GraphData data = (GraphData)node.Data;
            return data.HValue;
        }

        private void SetHValue(IGraphNode node, double h)
        {
            GraphData data = (GraphData)node.Data;
            data.HValue = h;
            node.Data = data;
        }

        private void SortFValue(List<IGraphNode> nodeList)
        {
            nodeList.Sort(this);
        }

        private IGraphNode DequeueFirst(IList<IGraphNode> nodeList)
        {
            IGraphNode firstNode = nodeList[0];
            nodeList.RemoveAt(0);
            return firstNode;
        }

        private double GetEstimateDistance(IGraphNode graphA, IGraphNode graphB)
        {
            Vector2Int a = GetPosition(graphA);
            Vector2Int b = GetPosition(graphB);
            double dx = a.X - b.X;
            double dy = a.Y - b.Y;
                //return (Math.Abs(dx) + Math.Abs(dy));

            return Math.Sqrt(dx*dx + dy*dy);
        }

        private Vector2Int GetPosition(IGraphNode node)
        {
            return node.Position;
        }

        private void SetPosition(IGraphNode node, Vector2Int position)
        {
            node.Position = position;
        }

        private double GetConnectionWeight(IGraphNode nodeA, IGraphNode nodeB)
        {
            return 1;
        }

        private double GetConnectionWeight(IWeightedGraphNode<double> weightedNodeA,
            IWeightedGraphNode<double> weightedNodeB)
        {
            return weightedNodeA.GetWeight(weightedNodeB);
        }

        public IList<Vector2Int> GetPath(IWeightedGraph<double> map, IWeightedGraphNode<double> start, IWeightedGraphNode<double> stop)
        {
            return GetPath((IGraph)map, start, stop);
        }

        public IList<Vector2Int> GetPath(IGraph map, IGraphNode startNode, IGraphNode stopNode)
        {
            List<IGraphNode> openNodes = new List<IGraphNode>();
            List<IGraphNode> closedNodes = new List<IGraphNode>();

            bool isWeighted = map is IWeightedGraph<double>;

            openNodes.Add(startNode);

            while (openNodes.Count > 0)
            {
                SortFValue(openNodes);
                IGraphNode x = DequeueFirst(openNodes);
                Vector2Int position = GetPosition(x);
                OnCellViewedEvent?.Invoke(this, position.X, position.Y, (int)GetGValue(x));
                if (x == stopNode)
                {
                    break;
                }

                closedNodes.Add(x);

                foreach (var y in x.GetConnectedNodes())
                {
                    if (closedNodes.Contains(y))
                    {
                        continue;
                    }

                    if (!openNodes.Contains(y))
                    {
                        SetPathPredecessor(y, x);
                        double nodesDistance;
                        if (isWeighted)
                        {
                            nodesDistance = GetConnectionWeight((IWeightedGraphNode<double>) x, (IWeightedGraphNode<double>)y);
                        }
                        else
                        {
                            nodesDistance = GetConnectionWeight(x, y);
                        }

                        SetGValue(y, GetGValue(x) + nodesDistance);
                        double estimateDistance = GetEstimateDistance(y, stopNode);
                        SetHValue(y, estimateDistance);
                        //InsertSorted(openNodes, y);
                        openNodes.Add(y);
                    }
                    else
                    {
                        double xGValue = GetGValue(x) + 1;
                        double yGValue = GetGValue(y);
                        if (xGValue < yGValue)
                        {
                            SetGValue(y, xGValue);
                            SetPathPredecessor(y, x);
                        }
                    }
                }
            }

            if (GetPathPredecessor(stopNode) == null)
            {
                return null;
            }

            List<Vector2Int> path = new List<Vector2Int>();
            path.Add(GetPosition(stopNode));
            IGraphNode currentNode = stopNode;
            while (true)
            {
                currentNode = GetPathPredecessor(currentNode);
                if (currentNode == null)
                    break;
                path.Add(GetPosition(currentNode));
            }


            return path;
        }

        public IList<Vector2Int> GetPath(ICellMap map, Vector2Int start, Vector2Int stop, NeighbourMode neighbourMode)
        {
            if (neighbourMode == NeighbourMode.SideOnly)
            {
                GraphNode[,] graphNodes = GraphGenerator.GetGraph(map, neighbourMode);
                IGraphNode[,] nodesInterface = new IGraphNode[map.Width, map.Height];
                for (int i = 0; i < map.Width; i++)
                {
                    for (int j = 0; j < map.Height; j++)
                    {
                        if (graphNodes[i, j] == null)
                            continue;
                        graphNodes[i, j].Data = new GraphData();
                        SetPosition(graphNodes[i, j], new Vector2Int(i, j));
                        nodesInterface[i, j] = graphNodes[i, j];
                    }
                }

                IGraphNode startNode = graphNodes[start.X, start.Y];
                IGraphNode stopNode = graphNodes[stop.X, stop.Y];
                

                Graph graph = new Graph(nodesInterface);

                return GetPath(graph, startNode, stopNode);
            }
            else
            {
                IWeightedGraph<double> graph = GraphGenerator.GetWeightedGraph(map, NeighbourMode.SidesAndDiagonals);
                var nodes = graph.GetWeightedGraphNodes();
                IWeightedGraphNode<double> startNode = null, stopNode = null;
                foreach (var node in nodes)
                {
                    if (node == null)
                        continue;
                    node.Data = new GraphData();
                    if (node.Position == start)
                    {
                        startNode = node;
                    }

                    if (node.Position == stop)
                    {
                        stopNode = node;
                    }
                }

                return GetPath(graph, startNode, stopNode);
            }
        }

        public int Compare(IGraphNode x, IGraphNode y)
        {
            if (x == null || y == null)
                return 0;
            GraphData xData = (GraphData) x.Data;
            GraphData yData = (GraphData) y.Data;
            if (xData.FValue >= yData.FValue)
                return 1;
            return -1;
        }
    }
}
