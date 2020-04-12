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

        public IList<Vector2Int> GetSmoothedPath(ICellMap map, IList<Vector2Int> rawPath)
        {
            List<Vector2Int> smoothedPath = new List<Vector2Int>(rawPath.Count);
            bool lineCasterFailed = false;
            BresenhamLinePlotter lineCaster = new BresenhamLinePlotter();
            int currentIndex = 0, nextIndex = 1, lastValidIndex = 1;
            bool IsCellPassable(int x, int y)
            {
                bool isPassable = map.IsPassable(x, y);
                if (!isPassable)
                {
                    OnObstacleDetectedEvent?.Invoke(x, y);
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
                lineCasterFailed = false;
                nextIndex = currentIndex;
                lastValidIndex = nextIndex;
                while (nextIndex < rawPath.Count - 1)
                {
                    nextIndex++;
                    if (nextIndex >= rawPath.Count)
                    {
                        break;
                    }
                    //Debug.WriteLine($"Casting line from {rawPath[currentIndex]} to {rawPath[nextIndex]}");
                    lineCaster.CastLine(rawPath[currentIndex], rawPath[nextIndex], IsCellPassable);
                    if (!lineCasterFailed)
                    {
                        lastValidIndex = nextIndex;
                    }
                }
                //Debug.WriteLine($"Drawing line from {rawPath[currentIndex]} to {rawPath[nextIndex - 1]}");
                lineCaster.CastLine(rawPath[currentIndex], rawPath[lastValidIndex], SetPathCell);
                currentIndex = lastValidIndex + 1;
            }
            Debug.WriteLine("Smoothing finished");
            return smoothedPath;
        }
    }
}
