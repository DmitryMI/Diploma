using System;
using PathFinders.Graphs.SimpleTypes;

namespace PathFinders.Graphs
{
    public static class GraphGenerator
    {
        public static double Sqrt2 = Math.Sqrt(2);
        

        public static GraphNode[,] GetGraph(ICellMap map, NeighbourMode neighbourMode)
        {
            GraphNode[,] nodes = new GraphNode[map.Width,map.Height];
            for (int x = 0; x < map.Width; x++)
            {
                for (int y = 0; y < map.Height; y++)
                {
                    if (map.IsPassable(x, y))
                    {
                        GraphNode prevNode;
                        GraphNode currentNode = new GraphNode();
                        nodes[x, y] = currentNode;
                        currentNode.Position = new Vector2Int(x, y);

                        int prevX = x - 1;
                        int prevY = y - 1;
                        if (prevX >= 0)
                        {
                            prevNode = nodes[prevX, y];
                            ConnectNodes(currentNode, prevNode);
                        }

                        if (prevY >= 0)
                        {
                            prevNode = nodes[x, prevY];
                            ConnectNodes(currentNode, prevNode);
                        }

                        if (prevX >= 0 && prevY >= 0 && neighbourMode == NeighbourMode.SidesAndDiagonals)
                        {
                            prevNode = nodes[prevX, prevY];
                            ConnectNodes(currentNode, prevNode);
                        }
                    }
                    else
                    {
                        nodes[x, y] = null;
                    }
                }
            }

            return nodes;
        }

        private static void ConnectNodes(GraphNode nodeA, GraphNode nodeB)
        {
            if (nodeA == null || nodeB == null)
            {
                return;
            }
            nodeA.Connections.Add(nodeB);
            nodeB.Connections.Add(nodeA);
        }

        private static void ConnectNodes(WeightedGraphNode<double> nodeA, WeightedGraphNode<double> nodeB,
            double weight)
        {
            if (nodeA == null || nodeB == null)
            {
                return;
            }

            nodeA.SetWeight(nodeB, weight);
            nodeB.SetWeight(nodeA, weight);
        }

        public static WeightedGraph<double> GetWeightedGraph(ICellMap map, NeighbourMode neighbourMode)
        {
            WeightedGraphNode<double>[,] nodes = new WeightedGraphNode<double>[map.Width, map.Height];

            for (int x = 0; x < map.Width; x++)
            {
                for (int y = 0; y < map.Height; y++)
                {
                    if (map.IsPassable(x, y))
                    {
                        WeightedGraphNode<double> prevNode;
                        WeightedGraphNode<double> currentNode = new WeightedGraphNode<double>(Double.PositiveInfinity);
                        nodes[x, y] = currentNode;
                        currentNode.Position = new Vector2Int(x, y);

                        int prevX = x - 1;
                        int prevY = y - 1;
                        if (prevX >= 0)
                        {
                            prevNode = nodes[prevX, y];
                            ConnectNodes(currentNode, prevNode, 1);
                        }
                        if (prevY >= 0)
                        {
                            prevNode = nodes[x, prevY];
                            ConnectNodes(currentNode, prevNode, 1);
                        }

                        if (prevX >= 0 && prevY >= 0 && neighbourMode == NeighbourMode.SidesAndDiagonals)
                        {
                            prevNode = nodes[prevX, prevY];
                            ConnectNodes(currentNode, prevNode, Sqrt2);
                        }
                    }
                    else
                    {
                        nodes[x, y] = null;
                    }
                }
            }

            WeightedGraph<double> weightedGraph = new WeightedGraph<double>(nodes, Double.PositiveInfinity);

            return weightedGraph;
;        }
    }
}
