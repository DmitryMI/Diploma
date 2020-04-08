using PathFinders.Graphs.SimpleTypes;

namespace PathFinders.Graphs
{
    public static class GraphGenerator
    {
        private static Vector2Int[] GetSteps()
        {
            return new Vector2Int[]{new Vector2Int(-1, 0),new Vector2Int(1, 0),new Vector2Int(0, -1),new Vector2Int(0, 1)};
        }

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

        private static void MakeConnections(WeightedGraph<int> weightedGraph, ICellMap map, int x, int y, Vector2Int[] steps)
        {
            void SetWeightLambda(int fromX, int fromY, int toX, int toY, int weight) => weightedGraph.SetWeight(fromY * map.Width + fromX, toY * map.Width + toX, weight);

            foreach (var step in steps)
            {
                int xNew = x + step.X;
                int yNew = y + step.Y;

                if (!map.IsInBounds(xNew, yNew))
                {
                    continue;
                }

                if (map.IsPassable(xNew, yNew))
                {
                    SetWeightLambda(x, y, xNew, yNew, 1);
                }
            }
        }

        public static GraphNode[,] GetGraph(ICellMap map)
        {
            GraphNode[,] nodes = new GraphNode[map.Width,map.Height];
            for (int x = 0; x < map.Width; x++)
            {
                for (int y = 0; y < map.Height; y++)
                {
                    if (map.IsPassable(x, y))
                    {
                        nodes[x, y] = new GraphNode();
                    }
                    else
                    {
                        nodes[x, y] = null;
                    }
                }
            }

            var steps = GetSteps();

            for (int x = 0; x < map.Width; x++)
            {
                for (int y = 0; y < map.Height; y++)
                {
                    MakeConnections(nodes, x, y, steps);
                }
            }

            return nodes;
        }

        public static WeightedGraph<int> GetWeightedGraph(ICellMap map)
        {
            int graphSize = map.Width * map.Height;
            WeightedGraph<int> weightedGraph = new WeightedGraph<int>(graphSize, int.MaxValue);

            Vector2Int[] steps = GetSteps();

            for (int x = 0; x < map.Width; x++)
            {
                for (int y = 0; y < map.Height; y++)
                {
                    MakeConnections(weightedGraph, map, x, y, steps);
                }
            }

            return weightedGraph;
;        }
    }
}
