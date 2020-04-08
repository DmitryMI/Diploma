using System;
using System.Collections.Generic;
using PathFinders.Graphs;
using PathFinders.Graphs.SimpleTypes;

namespace PathFinders.Algorithms
{
    public class BestFirstSearch : ICellPathFinder, IGraphPathFinder, IComparer<IGraphNode>
    {
        public event Action<object, int, int, int> OnCellViewedEvent;

        private struct GraphData
        {
            public Vector2Int Position { get; set; }
            public int DistanceToStop { get; set; }
            public bool IsClosed { get; set; }
            public IGraphNode ParentNode { get; set; }
            public int StepsToStart { get; set; }
        }

        private T GetData<T>(IGraphNode node)
        {
            return (T) node.Data;
        }

        private int GetDistance(Vector2Int from, Vector2Int to)
        {
            return Math.Abs(from.X - to.X) + Math.Abs(from.Y - to.Y);
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

        private Vector2Int GetPosition(IGraphNode node)
        {
            return GetData<GraphData>(node).Position;
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

        public IList<Vector2Int> GetPath(IGraph graph, IGraphNode startNode, IGraphNode stopNode)
        {
            List<IGraphNode> openNodes = new List<IGraphNode>();

            openNodes.Add(startNode);

            while (openNodes.Count > 0)
            {
                openNodes.Sort(this);
                IGraphNode currentNode = openNodes[0];
                openNodes.RemoveAt(0);

                if (openNodes.Count > 0 && openNodes[0] == currentNode)
                {
                    //Console.WriteLine("Node was not deleted from list");
                }

                if (IsClosed(currentNode))
                {
                    //Console.WriteLine("Processing closed node!");
                    continue;
                }

                Vector2Int position = GetData<GraphData>(currentNode).Position;
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

            List<Vector2Int> path = new List<Vector2Int>();

            IGraphNode currentPathNode = stopNode;
            path.Add(GetData<GraphData>(currentPathNode).Position);
            while (currentPathNode != startNode)
            {
                currentPathNode = GetData<GraphData>(currentPathNode).ParentNode;
                path.Add(GetData<GraphData>(currentPathNode).Position);
            }

            return path;
        }

        public IList<Vector2Int> GetPath(ICellMap map, Vector2Int start, Vector2Int stop, NeighbourMode neighbourMode)
        {
            IGraphNode[,] graphNodes = GraphGenerator.GetGraph(map, neighbourMode);

            for (int i = 0; i < map.Width; i++)
            {
                for (int j = 0; j < map.Height; j++)
                {
                    if(graphNodes[i, j] == null)
                        continue;
                    
                    Vector2Int pos = new Vector2Int(i, j);
                    graphNodes[i, j].Data = new GraphData(){DistanceToStop = GetDistance(pos, stop), Position = pos };
                }
            }

            IGraphNode startNode = graphNodes[start.X, start.Y];
            IGraphNode stopNode = graphNodes[stop.X, stop.Y];
            IGraph graph = new Graph(graphNodes);

            return GetPath(graph, startNode, stopNode);
        }

        public int Compare(IGraphNode a, IGraphNode b)
        {
            if (a == null || b == null)
                return 0;
            return GetData<GraphData>(a).DistanceToStop - GetData<GraphData>(b).DistanceToStop;
        }
    }
}
