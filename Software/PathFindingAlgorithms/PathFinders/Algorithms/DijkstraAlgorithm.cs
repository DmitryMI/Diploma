using System;
using System.Collections.Generic;
using PathFinders.Graphs;
using PathFinders.Graphs.SimpleTypes;

namespace PathFinders.Algorithms
{
    public class DijkstraAlgorithm : ICellPathFinder
    {
        public event Action<object, int, int, int> OnCellViewedEvent;

        private void FillArray<T>(T[] array, T value)
        {
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = value;
            }
        }
        

        public IList<Vector2Int> GetPath(ICellMap map, Vector2Int start, Vector2Int stop)
        {
            WeightedGraph<int> weightedGraph = GraphGenerator.GetWeightedGraph(map);

            int GetNodeIndex(int x, int y) => y * map.Width + x;
            Vector2Int GetNodePosition(int index)
            {
                int y = index / map.Width;
                int x = index - map.Width * y;
                return new Vector2Int(x, y);
            }

            void SetArrayValue<T>(T[] array, int x, int y, T value) => array[GetNodeIndex(x, y)] = value;

            int[] distances = new int[weightedGraph.Count];
            bool[] used = new bool[weightedGraph.Count];
            int[] prev = new int[weightedGraph.Count];
            FillArray(distances, Int32.MaxValue);
            FillArray(prev, -1);
            FillArray(used, false); // Can be removed
            
            SetArrayValue(distances, start.X, start.Y, 0);
            
            int minDistance = 0;
            int minVertex = GetNodeIndex(start.X, start.Y);

            while (minDistance < Int32.MaxValue)
            {
                int i = minVertex;
                used[i] = true;
                for (int j = 0; j < weightedGraph.Count; j++)
                {
                    int distanceI = distances[i];
                    int distanceJ = distances[j];
                    int weightIj = weightedGraph[i, j];
                    if (weightIj == int.MaxValue)
                        continue;

                    int nDistance;
                    if (distanceI == int.MaxValue)
                    {
                        nDistance = Int32.MaxValue;
                    }
                    else
                    {
                        nDistance = distanceI + weightIj;
                    }

                    if (nDistance < distanceJ)
                    {
                        distanceJ = nDistance;
                        distances[j] = distanceJ;
                        prev[j] = i;
                        Vector2Int nodePosition = GetNodePosition(j);
                        OnCellViewedEvent?.Invoke(this, nodePosition.X, nodePosition.Y, distanceJ);
                    }
                }
                minDistance = Int32.MaxValue;
                for (int j = 0; j < weightedGraph.Count; j++)
                {
                    if (!used[j] && distances[j] < minDistance)
                    {
                        minDistance = distances[j];
                        minVertex = j;
                    }
                }
            }

            List<Vector2Int> path = new List<Vector2Int>();


            int pathIndex = GetNodeIndex(stop.X, stop.Y);
            while (pathIndex != -1)
            {
                path.Add(GetNodePosition(pathIndex));
                pathIndex = prev[pathIndex];
            }

            path.Reverse();

            return path;
        }
    }
}
