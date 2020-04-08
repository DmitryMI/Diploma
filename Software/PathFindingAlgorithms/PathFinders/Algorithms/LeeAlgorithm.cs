using System;
using System.Collections.Generic;

namespace PathFinders.Algorithms
{
    public class LeeAlgorithm : ICellPathFinder
    {
        public event Action<object, int, int, int> OnCellViewedEvent;

        private static Vector2Int[] GetSteps()
        {
            Vector2Int[] steps = new Vector2Int[]
                {new Vector2Int(-1, 0), new Vector2Int(1, 0), new Vector2Int(0, -1), new Vector2Int(0, 1)};
            return steps;
        }

        private void UpdateCellSurroundings(ICellMap map, int[,] matrix, int x, int y, int d)
        {
            Vector2Int[] steps = GetSteps();

            foreach (var step in steps)
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

        private Vector2Int FindDecrement(int[,] matrix, int x, int y)
        {
            int d = matrix[x, y];

            Vector2Int[] steps = GetSteps();

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

        private IList<Vector2Int> GetPathWrapper(object parameters)
        {
            Parameters parametersStruct = (Parameters) parameters;
            return GetPath(parametersStruct.Map, parametersStruct.Start, parametersStruct.Stop);
        }

        public IList<Vector2Int> GetPath(ICellMap map, Vector2Int start, Vector2Int stop)
        {
            int[,] matrix = new int[map.Width,map.Height];

            for (int i = 0; i < map.Width; i++)
            {
                for (int j = 0; j < map.Height; j++)
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

                for (int i = 0; i < map.Width; i++)
                {
                    if (matrix[stop.X, stop.Y] != -1)
                    {
                        break;
                    }

                    for (int j = 0; j < map.Height; j++)
                    {
                        if (matrix[stop.X, stop.Y] != -1)
                        {
                            break;
                        }
                        if (matrix[i, j] == d)
                        {
                            UpdateCellSurroundings(map, matrix, i, j, d + 1);
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
                current = FindDecrement(matrix, current.X, current.Y);
                path.Add(current);
            }

            path.Reverse();

            return path;
        }
    }
}
