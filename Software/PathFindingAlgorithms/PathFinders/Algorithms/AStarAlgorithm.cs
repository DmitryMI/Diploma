﻿using System;
using System.Collections.Generic;
using PathFinders.Graphs;
using PathFinders.Graphs.SimpleTypes;

namespace PathFinders.Algorithms
{
    public class AStarAlgorithm : ICellPathFinder, IGraphPathFinder, IComparer<IGraphNode>
    {
        public event Action<object, int, int, int> OnCellViewedEvent;
        

        private struct GraphData
        {
            public int HValue { get; set; }
            public int GValue { get; set; }
            public int FValue => HValue + GValue;
            public bool IsExplored { get; set; }
            public IGraphNode PathPredecessor { get; set; }
            public Vector2Int Position { get; set; }
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

        private void SetGValue(IGraphNode node, int g)
        {
            GraphData data = (GraphData)node.Data;
            data.GValue = g;
            node.Data = data;
        }

        private int GetGValue(IGraphNode node)
        {
            GraphData data = (GraphData)node.Data;
            return data.GValue;
        }

        private int GetFValue(IGraphNode node)
        {
            GraphData data = (GraphData)node.Data;
            return data.FValue;
        }

        private int GetHValue(IGraphNode node)
        {
            GraphData data = (GraphData)node.Data;
            return data.HValue;
        }

        private void SetHValue(IGraphNode node, int h)
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

        private int GetEstimateDistance(IGraphNode graphA, IGraphNode graphB)
        {
            Vector2Int a = GetPosition(graphA);
            Vector2Int b = GetPosition(graphB);
            return Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y);
        }

        private Vector2Int GetPosition(IGraphNode node)
        {
            GraphData data = (GraphData)node.Data;
            return data.Position;
        }

        private void SetPosition(IGraphNode node, Vector2Int position)
        {
            GraphData data = (GraphData)node.Data;
            data.Position = position;
            node.Data = data;
        }

        public IList<Vector2Int> GetPath(IGraph map, IGraphNode startNode, IGraphNode stopNode)
        {
            List<IGraphNode> openNodes = new List<IGraphNode>();
            List<IGraphNode> closedNodes = new List<IGraphNode>();

            openNodes.Add(startNode);

            while (openNodes.Count > 0)
            {
                SortFValue(openNodes);
                IGraphNode x = DequeueFirst(openNodes);
                Vector2Int position = GetPosition(x);
                OnCellViewedEvent?.Invoke(this, position.X, position.Y, GetGValue(x));
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

        public IList<Vector2Int> GetPath(ICellMap map, Vector2Int start, Vector2Int stop)
        {
            IGraphNode[,] graphNodes = GraphGenerator.GetGraph(map);

            for (int i = 0; i < map.Width; i++)
            {
                for (int j = 0; j < map.Height; j++)
                {
                    if(graphNodes[i, j] == null)
                        continue;
                    graphNodes[i, j].Data = new GraphData();
                    SetPosition(graphNodes[i, j], new Vector2Int(i, j));
                }
            }

            IGraphNode startNode = graphNodes[start.X, start.Y];
            IGraphNode stopNode = graphNodes[stop.X, stop.Y];

            Graph graph = new Graph(graphNodes);

            return GetPath(graph, startNode, stopNode);
        }

        public int Compare(IGraphNode x, IGraphNode y)
        {
            if (x == null || y == null)
                return 0;
            GraphData xData = (GraphData) x.Data;
            GraphData yData = (GraphData) y.Data;
            return xData.FValue - yData.FValue;
        }
    }
}
