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

        public IList<Vector2Int> GetSmoothedPath(ICellMap map, Vector2Int start, Vector2Int stop, NeighbourMode neighbourMode)
        {
            var rawPath = GetPath(map, start, stop, neighbourMode);
            if (rawPath == null)
            {
                return null;
            }
            PathSmoother smoother = new PathSmoother();
            var path = smoother.GetSmoothedPath(map, rawPath);
            return path;
        }


        public IList<Vector2Int> GetPath(ICellMap map, Vector2Int start, Vector2Int stop, NeighbourMode neighbourMode)
        {
            WeightedGraph<double> weightedGraph = GraphGenerator.GetWeightedGraph(map, neighbourMode);

            int GetNodeIndex(int x, int y) => y * map.Width + x;
            Vector2Int GetNodePosition(int index)
            {
                int y = index / map.Width;
                int x = index - map.Width * y;
                return new Vector2Int(x, y);
            }

            void SetArrayValue<T>(T[] array, int x, int y, T value) => array[GetNodeIndex(x, y)] = value;

            int count = weightedGraph.Width * weightedGraph.Height;
            double[] distances = new double[count];
            bool[] used = new bool[count];
            int[] prev = new int[count];
            FillArray(distances, Int32.MaxValue);
            FillArray(prev, -1);
            FillArray(used, false); // Can be removed
            
            SetArrayValue(distances, start.X, start.Y, 0);
            
            double minDistance = 0;
            int minVertex = GetNodeIndex(start.X, start.Y);

            while (minDistance < Int32.MaxValue)
            {
                int i = minVertex;
                used[i] = true;
                for (int j = 0; j < count; j++)
                {
                    double distanceI = distances[i];
                    double distanceJ = distances[j];
                    double weightIj = weightedGraph.GetWeight(i, j);
                    if (double.IsPositiveInfinity(weightIj))
                        continue;

                    double nDistance;
                    if (double.IsPositiveInfinity(distanceI))
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
                        OnCellViewedEvent?.Invoke(this, nodePosition.X, nodePosition.Y, (int)distanceJ);
                    }
                }
                minDistance = Double.PositiveInfinity;
                for (int j = 0; j < count; j++)
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

        public void AddObstacle(ICellFragment cellCluster)
        {
            throw new NotImplementedException();
        }

        public void ClearObstacles()
        {
            throw new NotImplementedException();
        }

        public void RecalculateObstacles(NeighbourMode neighbourMode = NeighbourMode.SidesAndDiagonals)
        {
            throw new NotImplementedException();
        }

        public void Initialize(ICellMap mapBase)
        {
            throw new NotImplementedException();
        }
    }
}
