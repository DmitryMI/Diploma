using System;
using System.Collections.Generic;
using PathFinders.Graphs;

namespace PathFinders.Algorithms
{
    public class BestFirstSearch : ICellPathFinder, IGraphPathFinder, IComparer<GraphNode>
    {
        public event Action<object, int, int, int> OnCellViewedEvent;

        private struct GraphData
        {
            public Vector2Int Position { get; set; }
            public int DistanceToStop { get; set; }
            public bool IsClosed { get; set; }
            public GraphNode ParentNode { get; set; }
            public int StepsToStart { get; set; }
        }

        private int GetDistance(Vector2Int from, Vector2Int to)
        {
            return Math.Abs(from.X - to.X) + Math.Abs(from.Y - to.Y);
        }

        private void SetClosed(GraphNode node)
        {
            GraphData data = node.GetData<GraphData>();
            data.IsClosed = true;
            node.Data = data;
        }

        private bool IsClosed(GraphNode node)
        {
            GraphData data = node.GetData<GraphData>();
            return data.IsClosed;
        }

        private Vector2Int GetPosition(GraphNode node)
        {
            return node.GetData<GraphData>().Position;
        }

        private int GetStepsToStart(GraphNode node)
        {
            return node.GetData<GraphData>().StepsToStart;
        }

        private void SetParentNode(GraphNode node, GraphNode parent)
        {
            GraphData data = node.GetData<GraphData>();
            data.ParentNode = parent;
            data.StepsToStart = GetStepsToStart(parent) + 1;
            node.Data = data;
        }

        public IList<Vector2Int> GetPath(GraphNode[,] graphNodes, GraphNode startNode, GraphNode stopNode)
        {
            List<GraphNode> openNodes = new List<GraphNode>();

            openNodes.Add(startNode);

            while (openNodes.Count > 0)
            {
                openNodes.Sort(this);
                GraphNode currentNode = openNodes[0];
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

                Vector2Int position = currentNode.GetData<GraphData>().Position;
                OnCellViewedEvent?.Invoke(this, position.X, position.Y, GetStepsToStart(currentNode));

                SetClosed(currentNode);

                if (currentNode == stopNode)
                {
                    break;
                }

                foreach (var connected in currentNode)
                {
                    if (!IsClosed(connected))
                    {
                        SetParentNode(connected, currentNode);
                        openNodes.Add(connected);
                    }
                }
            }

            if (!startNode.GetData<GraphData>().IsClosed)
            {
                return null;
            }

            List<Vector2Int> path = new List<Vector2Int>();

            GraphNode currentPathNode = stopNode;
            path.Add(currentPathNode.GetData<GraphData>().Position);
            while (currentPathNode != startNode)
            {
                currentPathNode = currentPathNode.GetData<GraphData>().ParentNode;
                path.Add(currentPathNode.GetData<GraphData>().Position);
            }

            return path;
        }

        public IList<Vector2Int> GetPath(ICellMap map, Vector2Int start, Vector2Int stop)
        {
            GraphNode[,] graphNodes = GraphGenerator.GetGraph(map);

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

            GraphNode startNode = graphNodes[start.X, start.Y];
            GraphNode stopNode = graphNodes[stop.X, stop.Y];

            return GetPath(graphNodes, startNode, stopNode);
        }

        public int Compare(GraphNode a, GraphNode b)
        {
            if (a == null || b == null)
                return 0;
            return a.GetData<GraphData>().DistanceToStop - b.GetData<GraphData>().DistanceToStop;
        }
    }
}
