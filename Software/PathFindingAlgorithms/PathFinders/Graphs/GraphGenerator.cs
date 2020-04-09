using System;
using PathFinders.Graphs.SimpleTypes;

namespace PathFinders.Graphs
{
    public static class GraphGenerator
    {
        private static void MakeConnections(GraphNode[,] nodes, int x, int y, Vector2Int[] steps)
        {
            if(nodes[x, y] == null)
                return;

            GraphNode currentNode = nodes[x, y];
            
            foreach (var step in steps)
            {
                int xNew = x + step.X;
                int yNew = y + step.Y;

                if(xNew < 0 || yNew < 0)
                    continue;
                if(xNew >= nodes.GetLength(0) || yNew >= nodes.GetLength(1))
                    continue;

                GraphNode connectedNode = nodes[xNew, yNew];


                if (nodes[xNew, yNew] != null)
                {
                    if(!currentNode.Contains(connectedNode))
                        currentNode.Add(connectedNode);
                    if(!connectedNode.Contains(currentNode))
                        connectedNode.Add(currentNode);
                }
            }
        }

        private static void MakeWeightedConnections(WeightedGraph<double> weightedGraph, ICellMap map, int x, int y, Vector2Int[] steps)
        {
            void SetWeightLambda(int fromX, int fromY, int toX, int toY, double weight)
            {
                IWeightedGraphNode<double> nodeA = (IWeightedGraphNode<double>)weightedGraph[fromX, fromY];
                if (nodeA == null)
                    return;
                IWeightedGraphNode<double> nodeB = (IWeightedGraphNode<double>)weightedGraph[toX, toY];
                if(nodeB == null)
                    return;
                
                
                nodeA.SetWeight(nodeB, weight);
            }

            foreach (var step in steps)
            {
                int xNew = x + step.X;
                int yNew = y + step.Y;

                if (!map.IsInBounds(xNew, yNew))
                {
                    continue;
                }

                double dx = Math.Abs(xNew - x);
                double dy = Math.Abs(yNew - y);

                double weight = Math.Sqrt(dx * dx + dy * dy);

                if (map.IsPassable(xNew, yNew))
                {
                    SetWeightLambda(x, y, xNew, yNew, weight);
                }
            }
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
                        nodes[x, y] = new GraphNode();
                        nodes[x, y].Position = new Vector2Int(x, y);
                    }
                    else
                    {
                        nodes[x, y] = null;
                    }
                }
            }

            var steps = Steps.GetSteps(neighbourMode);

            for (int x = 0; x < map.Width; x++)
            {
                for (int y = 0; y < map.Height; y++)
                {
                    MakeConnections(nodes, x, y, steps);
                }
            }

            return nodes;
        }

        public static WeightedGraph<double> GetWeightedGraph(ICellMap map, NeighbourMode neighbourMode)
        {
            WeightedGraphNode<double>[,] nodes = new WeightedGraphNode<double>[map.Width,map.Height];

            for (int x = 0; x < map.Width; x++)
            {
                for (int y = 0; y < map.Height; y++)
                {
                    if (map.IsPassable(x, y))
                    {
                        nodes[x, y] = new WeightedGraphNode<double>(Double.PositiveInfinity);
                        nodes[x, y].Position = new Vector2Int(x, y);
                    }
                    else
                    {
                        nodes[x, y] = null;
                    }
                }
            }

            WeightedGraph<double> weightedGraph = new WeightedGraph<double>(nodes, Double.PositiveInfinity);

            Vector2Int[] steps = Steps.GetSteps(neighbourMode);
            for (int x = 0; x < map.Width; x++)
            {
                for (int y = 0; y < map.Height; y++)
                {
                    MakeWeightedConnections(weightedGraph, map, x, y, steps);
                }
            }

            return weightedGraph;
;        }
    }
}
