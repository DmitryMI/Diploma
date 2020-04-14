using System;
using PathFinders.Graphs.SimpleTypes;

namespace PathFinders.Graphs
{
    public static class GraphGenerator
    {
        public static double Sqrt2 = Math.Sqrt(2);

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


        public static GraphNode[,] GetGraph(ICellMap map, NeighbourMode neighbourMode)
        {
            GraphNode[,] nodes = new GraphNode[map.Width,map.Height];
            for (int x = 0; x < map.Width; x++)
            {
                for (int y = 0; y < map.Height; y++)
                {
                    if (map.IsPassable(x, y))
                    {
                        GraphNode connectionNode;
                        GraphNode currentNode = new GraphNode();
                        nodes[x, y] = currentNode;
                        currentNode.Position = new Vector2Int(x, y);

                        int prevX = x - 1;
                        int prevY = y - 1;
                        int nextX = x + 1;

                        if (nextX < map.Width && prevY >= 0 && neighbourMode == NeighbourMode.SidesAndDiagonals)
                        {
                            connectionNode = nodes[nextX, prevY];
                            ConnectNodes(currentNode, connectionNode);
                        }
                        if (prevX >= 0)
                        {
                            connectionNode = nodes[prevX, y];
                            ConnectNodes(currentNode, connectionNode);
                        }

                        if (prevY >= 0)
                        {
                            connectionNode = nodes[x, prevY];
                            ConnectNodes(currentNode, connectionNode);
                        }

                        if (prevX >= 0 && prevY >= 0 && neighbourMode == NeighbourMode.SidesAndDiagonals)
                        {
                            connectionNode = nodes[prevX, prevY];
                            ConnectNodes(currentNode, connectionNode);
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

        public static WeightedGraph<double> GetWeightedGraph(ICellMap map, NeighbourMode neighbourMode)
        {
            WeightedGraphNode<double>[,] nodes = new WeightedGraphNode<double>[map.Width, map.Height];

            for (int x = 0; x < map.Width; x++)
            {
                for (int y = 0; y < map.Height; y++)
                {
                    if (map.IsPassable(x, y))
                    {
                        WeightedGraphNode<double> connectionNode;
                        WeightedGraphNode<double> currentNode = new WeightedGraphNode<double>(Double.PositiveInfinity);
                        nodes[x, y] = currentNode;
                        currentNode.Position = new Vector2Int(x, y);

                        int prevX = x - 1;
                        int prevY = y - 1;
                        int nextY = y + 1;

                        if (prevX >= 0)
                        {
                            connectionNode = nodes[prevX, y];
                            ConnectNodes(currentNode, connectionNode, 1);
                        }
                        if (prevY >= 0)
                        {
                            connectionNode = nodes[x, prevY];
                            ConnectNodes(currentNode, connectionNode, 1);
                        }

                        if (prevX >= 0 && prevY >= 0 && neighbourMode == NeighbourMode.SidesAndDiagonals)
                        {
                            connectionNode = nodes[prevX, prevY];
                            ConnectNodes(currentNode, connectionNode, Sqrt2);
                        }

                        if (nextY < map.Height && prevX >= 0 && neighbourMode == NeighbourMode.SidesAndDiagonals)
                        {
                            connectionNode = nodes[prevX, nextY];
                            ConnectNodes(currentNode, connectionNode, Sqrt2);
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
