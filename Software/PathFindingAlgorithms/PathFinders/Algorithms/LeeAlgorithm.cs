using System;
using System.Collections.Generic;
using PathFinders.Algorithms.HpaStar;

namespace PathFinders.Algorithms
{
    public class LeeAlgorithm : ICellPathFinder
    {
        public event Action<object, int, int, int> OnCellViewedEvent;

        private LayeredCellMap _layeredCellMap;

        private void UpdateCellSurroundings(ICellMap map, int[,] matrix, int x, int y, int d, NeighbourMode neighbourMode)
        {
            foreach (var step in Steps.GetSteps(neighbourMode))
            {
                int xNew = x + step.X;
                int yNew = y + step.Y;

                if(!map.IsInBounds(xNew, yNew))
                    continue;

                if (map.IsPassable(xNew, yNew) && matrix[xNew, yNew] == -1)
                {
                    matrix[xNew, yNew] = d;
                    OnCellViewedEvent?.Invoke(this, xNew, yNew, d);
                }
            }
        }

        private Vector2Int FindDecrement(int[,] matrix, int x, int y, NeighbourMode neighbourMode)
        {
            int d = matrix[x, y];

            Vector2Int[] steps = Steps.GetSteps(neighbourMode);

            foreach (var step in steps)
            {
                int xNew = x + step.X;
                int yNew = y + step.Y;

                if(xNew < 0 || yNew < 0)
                    continue;
                if (xNew >= matrix.GetLength(0) || yNew >= matrix.GetLength(1))
                    continue;
                

                if (matrix[xNew, yNew] == d - 1)
                {
                    return new Vector2Int(xNew, yNew);
                }
            }

            return default(Vector2Int);
        }

        private struct Parameters
        {
            public ICellMap Map { get; set; }
            public Vector2Int Start { get; set; }
            public Vector2Int Stop { get; set; }
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

        public IList<Vector2Int> GetPath(ICellMap mapBase, Vector2Int start, Vector2Int stop, NeighbourMode neighbourMode)
        {
            if (_layeredCellMap == null)
            {
                _layeredCellMap = new LayeredCellMap(mapBase);
            }

            int[,] matrix = new int[_layeredCellMap.Width, _layeredCellMap.Height];

            for (int i = 0; i < _layeredCellMap.Width; i++)
            {
                for (int j = 0; j < _layeredCellMap.Height; j++)
                {
                    matrix[i, j] = -1;
                }
            }

            matrix[start.X, start.Y] = 0;

            OnCellViewedEvent?.Invoke(this, start.X, start.Y, 0);

            int d = 0;
            bool goFurther = true;
            while (goFurther)
            {
                goFurther = false;

                for (int i = 0; i < _layeredCellMap.Width; i++)
                {
                    if (matrix[stop.X, stop.Y] != -1)
                    {
                        break;
                    }

                    for (int j = 0; j < _layeredCellMap.Height; j++)
                    {
                        if (matrix[stop.X, stop.Y] != -1)
                        {
                            break;
                        }
                        if (matrix[i, j] == d)
                        {
                            UpdateCellSurroundings(_layeredCellMap, matrix, i, j, d + 1, neighbourMode);
                            goFurther = true;
                        }
                    }
                    
                }

                d++;
            }

            if (matrix[stop.X, stop.Y] == -1)
            {
                return null;
            }

            List<Vector2Int> path = new List<Vector2Int>();

            Vector2Int current = stop;
            path.Add(current);
            while (current != start)
            {
                current = FindDecrement(matrix, current.X, current.Y, neighbourMode);
                path.Add(current);
            }

            path.Reverse();

            return path;
        }

        public void AddObstacle(ICellFragment cellCluster)
        {
            _layeredCellMap.AddFragment(cellCluster);
        }

        public void ClearObstacles()
        {
            _layeredCellMap.ClearLayers();
        }

        public void RecalculateObstacles(NeighbourMode neighbourMode = NeighbourMode.SidesAndDiagonals)
        {
            
        }

        public void Initialize(ICellMap mapBase)
        {
            _layeredCellMap = new LayeredCellMap(mapBase);
        }
    }
}
