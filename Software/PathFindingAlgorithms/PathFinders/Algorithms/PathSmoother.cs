using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PathFinders.Algorithms.PathSmoothing;

namespace PathFinders.Algorithms
{
    public class PathSmoother
    {
        // TODO Check if line is not built near the obstacle

        public event Action<int, int> OnObstacleDetectedEvent;

        private IList<Vector2Int> ForwardSmoothing(ICellMap map, IList<Vector2Int> rawPath)
        {
            List<Vector2Int> smoothedPath = new List<Vector2Int>(rawPath.Count);
            bool lineCasterFailed = false;
            BresenhamLinePlotter lineCaster = new BresenhamLinePlotter();
            int currentIndex = 0, nextIndex = 1, lastValidIndex = 1;
            Vector2Int lastObstacle = default;
            bool IsCellPassable(int x, int y)
            {
                bool isPassable = map.IsPassable(x, y);
                if (!isPassable)
                {
                    OnObstacleDetectedEvent?.Invoke(x, y);
                    lastObstacle = new Vector2Int(x, y);
                    lineCasterFailed = true;
                }

                return isPassable;
            }

            bool SetPathCell(int x, int y)
            {
                smoothedPath.Add(new Vector2Int(x, y));
                return true;
            }


            while (currentIndex < rawPath.Count - 1)
            {
                
                nextIndex = currentIndex + 1;
                lastValidIndex = currentIndex;
                while (nextIndex < rawPath.Count)
                {
                    lineCasterFailed = false;
                    lineCaster.CastLine(rawPath[currentIndex], rawPath[nextIndex], IsCellPassable);
                    
                    if (!lineCasterFailed)
                    {
                        lastValidIndex = nextIndex;
                    }
                    nextIndex++;
                }

                lineCaster.CastLine(rawPath[currentIndex], rawPath[lastValidIndex], SetPathCell);
                currentIndex = lastValidIndex + 1;
            }

            return smoothedPath;
        }

        public IList<Vector2Int> GetSmoothedPath(ICellMap map, IList<Vector2Int> rawPath)
        {
            return ForwardSmoothing(map, rawPath);
        }
    }
}
