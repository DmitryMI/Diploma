using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PathFindingAlgorithms.PathFinders
{
    public class LeeAlgorithm : IPathFinder
    {
        public event Action<object, int, int, int> OnCellViewedEvent;

        private static Vector2[] GetSteps()
        {
            Vector2[] steps = new Vector2[]
                {new Vector2(-1, 0), new Vector2(1, 0), new Vector2(0, -1), new Vector2(0, 1)};
            return steps;
        }

        private void UpdateCellSurroundings(IMap map, int[,] matrix, int x, int y, int d)
        {
            Vector2[] steps = GetSteps();

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

        private Vector2 FindDecrement(int[,] matrix, int x, int y)
        {
            int d = matrix[x, y];

            Vector2[] steps = GetSteps();

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
                    return new Vector2(xNew, yNew);
                }
            }

            return default(Vector2);
        }

        private struct Parameters
        {
            public IMap Map { get; set; }
            public Vector2 Start { get; set; }
            public Vector2 Stop { get; set; }
        }

        private IList<Vector2> GetPathWrapper(object parameters)
        {
            Parameters parametersStruct = (Parameters) parameters;
            return GetPath(parametersStruct.Map, parametersStruct.Start, parametersStruct.Stop);
        }

        public IList<Vector2> GetPath(IMap map, Vector2 start, Vector2 stop)
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

            List<Vector2> path = new List<Vector2>();

            Vector2 current = stop;
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
