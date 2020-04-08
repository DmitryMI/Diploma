using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace PathFindingAlgorithms.PathFinders
{
    public class BestFirstSearch : IPathFinder, IComparer<GraphNode>
    {
        public event Action<object, int, int, int> OnCellViewedEvent;

        private struct GraphData
        {
            public Vector2 Position { get; set; }
            public int DistanceToStop { get; set; }
            public bool IsClosed { get; set; }
            public GraphNode ParentNode { get; set; }
            public int StepsToStart { get; set; }
        }

        private int GetDistance(Vector2 from, Vector2 to)
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

        private Vector2 GetPosition(GraphNode node)
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

        public IList<Vector2> GetPath(IMap map, Vector2 start, Vector2 stop)
        {
            GraphNode[,] graphNodes = GraphGenerator.GetGraph(map);

            for (int i = 0; i < map.Width; i++)
            {
                for (int j = 0; j < map.Height; j++)
                {
                    if(graphNodes[i, j] == null)
                        continue;
                    
                    Vector2 pos = new Vector2(i, j);
                    graphNodes[i, j].Data = new GraphData(){DistanceToStop = GetDistance(pos, stop), Position = pos };
                }
            }

            List<GraphNode> openNodes = new List<GraphNode>();

            GraphNode startNode = graphNodes[start.X, start.Y];

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

                Vector2 position = currentNode.GetData<GraphData>().Position;
                OnCellViewedEvent?.Invoke(this, position.X, position.Y, GetStepsToStart(currentNode));
                
                SetClosed(currentNode);

                if (currentNode.GetData<GraphData>().Position == stop)
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

            if (!graphNodes[stop.X, stop.Y].GetData<GraphData>().IsClosed)
            {
                return null;
            }

            List<Vector2> path = new List<Vector2>();

            GraphNode currentPathNode = graphNodes[stop.X, stop.Y];
            path.Add(currentPathNode.GetData<GraphData>().Position);
            while (currentPathNode != graphNodes[start.X, start.Y])
            {
                currentPathNode = currentPathNode.GetData<GraphData>().ParentNode;
                path.Add(currentPathNode.GetData<GraphData>().Position);
            }

            return path;
        }

        public int Compare(GraphNode a, GraphNode b)
        {
            if (a == null || b == null)
                return 0;
            return a.GetData<GraphData>().DistanceToStop - b.GetData<GraphData>().DistanceToStop;
        }
    }
}
