using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PathFindingAlgorithms.PathFinders
{
    public class AStarAlgorithm : IPathFinder, IComparer<GraphNode>
    {
        public event Action<object, int, int, int> OnCellViewedEvent;
        
        private struct GraphData
        {
            public int HValue { get; set; }
            public int GValue { get; set; }
            public int FValue => HValue + GValue;
            public bool IsExplored { get; set; }
            public GraphNode PathPredecessor { get; set; }
            public Vector2 Position { get; set; }
        }

        private void InsertSorted(IList<GraphNode> nodeList, GraphNode graph)
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

        private bool IsExplored(GraphNode graph)
        {
            return ((GraphData) graph.Data).IsExplored;
        }

        private void MarkExplored(GraphNode graph)
        {
            GraphData data = (GraphData) graph.Data;
            data.IsExplored = true;
            graph.Data = data;
        }

        private void SetPathPredecessor(GraphNode node, GraphNode predecessor)
        {
            GraphData data = (GraphData)node.Data;
            data.PathPredecessor = predecessor;
            node.Data = data;
        }

        private GraphNode GetPathPredecessor(GraphNode node)
        {
            GraphData data = (GraphData)node.Data;
            return data.PathPredecessor;
        }

        private void SetGValue(GraphNode node, int g)
        {
            GraphData data = (GraphData)node.Data;
            data.GValue = g;
            node.Data = data;
        }

        private int GetGValue(GraphNode node)
        {
            GraphData data = (GraphData)node.Data;
            return data.GValue;
        }

        private int GetFValue(GraphNode node)
        {
            GraphData data = (GraphData)node.Data;
            return data.FValue;
        }

        private int GetHValue(GraphNode node)
        {
            GraphData data = (GraphData)node.Data;
            return data.HValue;
        }

        private void SetHValue(GraphNode node, int h)
        {
            GraphData data = (GraphData)node.Data;
            data.HValue = h;
            node.Data = data;
        }

        private void SortFValue(List<GraphNode> nodeList)
        {
            nodeList.Sort(this);
        }

        private GraphNode DequeueFirst(IList<GraphNode> nodeList)
        {
            GraphNode firstNode = nodeList[0];
            nodeList.RemoveAt(0);
            return firstNode;
        }

        private int GetEstimateDistance(GraphNode graphA, GraphNode graphB)
        {
            Vector2 a = GetPosition(graphA);
            Vector2 b = GetPosition(graphB);
            return Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y);
        }

        private Vector2 GetPosition(GraphNode node)
        {
            GraphData data = (GraphData)node.Data;
            return data.Position;
        }

        private void SetPosition(GraphNode node, Vector2 position)
        {
            GraphData data = (GraphData)node.Data;
            data.Position = position;
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
                    graphNodes[i, j].Data = new GraphData();
                    SetPosition(graphNodes[i, j], new Vector2(i, j));
                }
            }

            GraphNode startNode = graphNodes[start.X, start.Y];
            GraphNode stopNode = graphNodes[stop.X, stop.Y];

            List<GraphNode> openNodes = new List<GraphNode>();
            List<GraphNode> closedNodes = new List<GraphNode>();

            openNodes.Add(startNode);

            while (openNodes.Count > 0)
            {
                SortFValue(openNodes);
                GraphNode x = DequeueFirst(openNodes);
                Vector2 position = GetPosition(x);
                OnCellViewedEvent?.Invoke(this, position.X, position.Y, GetGValue(x));
                if (x == stopNode)
                {
                    break;
                }

                closedNodes.Add(x);

                foreach (var y in x)
                {
                    if (closedNodes.Contains(y))
                    {
                        continue;
                    }

                    if (!openNodes.Contains(y))
                    {
                        SetPathPredecessor(y, x);
                        SetGValue(y, GetGValue(x) + 1);
                        int estimateDistance = GetEstimateDistance(y, stopNode);
                        SetHValue(y, estimateDistance);
                        //InsertSorted(openNodes, y);
                        openNodes.Add(y);
                    }
                    else
                    {
                        int xGValue = GetGValue(x) + 1;
                        int yGValue = GetGValue(y);
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

            List<Vector2> path = new List<Vector2>();
            path.Add(GetPosition(stopNode));
            GraphNode currentNode = stopNode;
            while (true)
            {
                currentNode = GetPathPredecessor(currentNode);
                if(currentNode == null)
                    break;
                path.Add(GetPosition(currentNode));
            }
            

            return path;
        }

        public int Compare(GraphNode x, GraphNode y)
        {
            if (x == null || y == null)
                return 0;
            GraphData xData = (GraphData) x.Data;
            GraphData yData = (GraphData) y.Data;
            return xData.FValue - yData.FValue;
        }
    }
}
